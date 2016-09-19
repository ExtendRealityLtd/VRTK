// Player Presence|Scripts|0130
namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="fallDistance">The total height the player has dropped from a gravity based fall.</param>
    public struct PlayerPresenceEventArgs
    {
        public float fallDistance;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PlayerPresenceEventArgs"/></param>
    public delegate void PlayerPresenceEventHandler(object sender, PlayerPresenceEventArgs e);

    /// <summary>
    /// The concept that the VR user has a physical in game presence which is accomplished by adding a collider and a rigidbody at the position the user is standing within their play area. This physical collider and rigidbody will prevent the user from ever being able to walk through walls or intersect other collidable objects. The height of the collider is determined by the height the user has the headset at, so if the user crouches then the collider shrinks with them, meaning it's possible to crouch and crawl under low ceilings.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad but the user cannot pass through the objects as they are collidable and the rigidbody physics won't allow the intersection to occur.
    /// </example>
    public class VRTK_PlayerPresence : MonoBehaviour
    {
        [Tooltip("The collider which is created for the user is set at a height from the user's headset position. If the collider is required to be lower to allow for room between the play area collider and the headset then this offset value will shorten the height of the generated collider.")]
        public float headsetYOffset = 0.2f;
        [Tooltip("If this is checked then any items that are grabbed with the controller will not collide with the player presence collider. This is very useful if the user is required to grab and wield objects because if the collider was active they would bounce off the collider.")]
        public bool ignoreGrabbedCollisions = true;
        [Tooltip("If this is checked then if the Headset Collision script is present and a headset collision occurs, the CameraRig is moved back to the last good known standing position. This deals with any collision issues if a user stands up whilst moving through a crouched area as instead of them being able to clip into objects they are transported back to a position where they are able to stand.")]
        public bool resetPositionOnCollision = true;
        [Tooltip("Only use physics when an explicit falling state is set.")]
        public bool fallingPhysicsOnly = false;

        /// <summary>
        /// Emitted when a gravity based fall has started.
        /// </summary>
        public event PlayerPresenceEventHandler PresenceFallStarted;
        /// <summary>
        /// Emitted when a gravity based fall has ended.
        /// </summary>
        public event PlayerPresenceEventHandler PresenceFallEnded;

        private Transform headset;
        private Rigidbody rb;
        private CapsuleCollider presenceCollider;
        private Vector3 lastGoodPosition;
        private bool lastGoodPositionSet = false;
        private float highestHeadsetY = 0f;
        private float crouchMargin = 0.5f;
        private float lastPlayAreaY = 0f;
        private float fallStartHeight = 0.0f;
        private bool isFalling = false;
        private bool customRigidBody = false;
        private bool customCollider = false;

        /// <summary>
        /// The SetFallingPhysicsOnlyParams method will toggle the `fallingPhysicsOnly` class state as well as enable or disable physics if needed.
        /// </summary>
        /// <param name="falling">Toggle the physics falling on or off.</param>
        public void SetFallingPhysicsOnlyParams(bool falling)
        {
            fallingPhysicsOnly = falling;

            if (fallingPhysicsOnly)
            {
                DisablePhysics();
            }
            else
            {
                EnablePhysics();
            }
        }

        /// <summary>
        /// The IsFalling method will return if the class is using physics based falling and is currently in a falling state.
        /// </summary>
        /// <returns>Returns if the player is in a physics falling state or not.</returns>
        public bool IsFalling()
        {
            return fallingPhysicsOnly && isFalling;
        }

        /// <summary>
        /// The StartPhysicsFall method initializes the physics based fall state, enable physics and send out the `PresenceFallStarted` event.
        /// </summary>
        /// <param name="velocity">The starting velocity to use at the start of a fall.</param>
        public void StartPhysicsFall(Vector3 velocity)
        {
            if (!isFalling && fallingPhysicsOnly)
            {
                OnPresenceFallStarted(SetPlayerPhysicsEvent(0));

                isFalling = true;
                EnablePhysics();
                if (rb)
                {
                    rb.velocity = velocity + new Vector3(0.0f, -0.001f, 0.0f);
                }
                fallStartHeight = transform.position.y;
            }
        }

        /// <summary>
        /// The StopPhysicsFall method ends the physics based fall state, disables physics and send out the `PresenceFallEnded` event.
        /// </summary>
        public void StopPhysicsFall()
        {
            if (!fallingPhysicsOnly)
            {
                return;
            }

            float fallHeight = fallStartHeight - transform.position.y;
            OnPresenceFallEnded(SetPlayerPhysicsEvent(fallHeight));

            isFalling = false;
            DisablePhysics();
        }

        private void OnPresenceFallStarted(PlayerPresenceEventArgs e)
        {
            if (PresenceFallStarted != null)
            {
                PresenceFallStarted(this, e);
            }
        }

        private void OnPresenceFallEnded(PlayerPresenceEventArgs e)
        {
            if (PresenceFallEnded != null)
            {
                PresenceFallEnded(this, e);
            }
        }

        private PlayerPresenceEventArgs SetPlayerPhysicsEvent(float fallDistance)
        {
            PlayerPresenceEventArgs e;
            e.fallDistance = fallDistance;
            return e;
        }

        private void Awake()
        {
            Utilities.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
            customRigidBody = false;
            customCollider = false;
        }

        private void OnEnable()
        {
            CreateCollider();
            lastGoodPositionSet = false;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            StartCoroutine(WaitForHeadsetCollision(true));

            InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), true);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), true);
        }

        private void OnDisable()
        {
            DestroyCollider();
            InitHeadsetListeners(false);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), false);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), false);
        }

        private IEnumerator WaitForHeadsetCollision(bool state)
        {
            yield return new WaitForEndOfFrame();

            InitHeadsetListeners(state);
        }

        private void InitHeadsetListeners(bool state)
        {
            var headsetCollision = headset.GetComponent<VRTK_HeadsetCollision>();
            if (headsetCollision)
            {
                if (state)
                {
                    headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(OnHeadsetCollision);
                }
                else
                {
                    headsetCollision.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(OnHeadsetCollision);
                }
            }
        }

        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if(e.target)
            {
                IgnoreCollisions(e.target.GetComponentsInChildren<Collider>(), true);
            }
        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (e.target && e.target.GetComponent<VRTK_InteractableObject>())
            {
                IgnoreCollisions(e.target.GetComponentsInChildren<Collider>(), false);
            }
        }

        private void IgnoreCollisions(Collider[] colliders, bool state)
        {
            foreach (var controllerCollider in colliders)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), controllerCollider, state);
            }
        }

        private void OnHeadsetCollision(object sender, HeadsetCollisionEventArgs e)
        {
            if (resetPositionOnCollision && lastGoodPositionSet)
            {
                transform.position = lastGoodPosition;
            }
        }

        private void CreateCollider()
        {
            customRigidBody = true;
            customCollider = true;
            rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.mass = 100;
                rb.freezeRotation = true;
                customRigidBody = false;
            }

            presenceCollider = gameObject.GetComponent<CapsuleCollider>();
            if (presenceCollider == null)
            {
                presenceCollider = gameObject.AddComponent<CapsuleCollider>();
                presenceCollider.center = new Vector3(0f, 1f, 0f);
                presenceCollider.height = 1f;
                presenceCollider.radius = 0.15f;
                customCollider = false;
            }

            if (fallingPhysicsOnly)
            {
                DisablePhysics();
            }

            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void DestroyCollider()
        {
            if (!customRigidBody)
            {
                Destroy(rb);
            }
            if (!customCollider)
            {
                Destroy(presenceCollider);
            }
        }

        private void TogglePhysics(bool state)
        {
            if (rb)
            {
                rb.isKinematic = state;
            }
            if (presenceCollider)
            {
                presenceCollider.isTrigger = state;
            }
        }

        private void EnablePhysics()
        {
            TogglePhysics(false);
        }

        private void DisablePhysics()
        {
            TogglePhysics(true);
        }

        private void UpdateCollider()
        {
            var playAreaHeightAdjustment = 0.009f;
            var newpresenceColliderYSize = (headset.transform.localPosition.y - headsetYOffset);
            var newpresenceColliderYCenter = (newpresenceColliderYSize != 0 ? (newpresenceColliderYSize / 2) + playAreaHeightAdjustment : 0);

            if (presenceCollider)
            {
                presenceCollider.height = newpresenceColliderYSize;
                presenceCollider.center = new Vector3(headset.localPosition.x, newpresenceColliderYCenter, headset.localPosition.z);
            }
        }

        private void SetHeadsetY()
        {
            //if the play area height has changed then always recalc headset height
            var floorVariant = 0.005f;
            if (transform.position.y > lastPlayAreaY + floorVariant || transform.position.y < lastPlayAreaY - floorVariant)
            {
                highestHeadsetY = 0f;
            }

            if (headset.transform.position.y > highestHeadsetY)
            {
                highestHeadsetY = headset.transform.position.y;
            }

            if (headset.transform.position.y > highestHeadsetY - crouchMargin)
            {
                lastGoodPositionSet = true;
                lastGoodPosition = transform.position;
            }

            lastPlayAreaY = transform.position.y;
        }

        private void FixedUpdate()
        {
            SetHeadsetY();
            UpdateCollider();
        }

        private void Update()
        {
            if (isFalling && fallingPhysicsOnly)
            {
                if (rb && rb.velocity == Vector3.zero)
                {
                    StopPhysicsFall();
                }
            }
        }

        private void InitControllerListeners(GameObject controller, bool state)
        {
            if (controller)
            {
                IgnoreCollisions(controller.GetComponentsInChildren<Collider>(), true);

                var grabbingController = controller.GetComponent<VRTK_InteractGrab>();
                if (grabbingController && ignoreGrabbedCollisions)
                {
                    if (state)
                    {
                        grabbingController.ControllerGrabInteractableObject += new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject += new ObjectInteractEventHandler(OnUngrabObject);
                    }
                    else
                    {
                        grabbingController.ControllerGrabInteractableObject -= new ObjectInteractEventHandler(OnGrabObject);
                        grabbingController.ControllerUngrabInteractableObject -= new ObjectInteractEventHandler(OnUngrabObject);
                    }
                }
            }
        }
    }
}