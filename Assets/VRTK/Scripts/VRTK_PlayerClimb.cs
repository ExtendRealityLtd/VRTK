// Player Climb|Scripts|0210
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the controller doing the interaction.</param>
    /// <param name="target">The GameObject of the interactable object that is being interacted with by the controller.</param>
    public struct PlayerClimbEventArgs
    {
        public uint controllerIndex;
        public GameObject target;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PlayerClimbEventArgs"/></param>
    public delegate void PlayerClimbEventHandler(object sender, PlayerClimbEventArgs e);

    /// <summary>
    /// This class allows player movement based on grabbing of `VRTK_InteractableObject` objects that are tagged as `Climbable`. It should be attached to the `[CameraRig]` object. Because it works by grabbing, each controller should have a `VRTK_InteractGrab` and `VRTK_InteractTouch` component attached.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/037_CameraRig_ClimbingFalling` shows how to set up a scene with player climbing. There are many different examples showing how the same system can be used in unique ways.
    /// </example>
    public class VRTK_PlayerClimb : MonoBehaviour
    {
        [Tooltip("Will scale movement up and down based on the player transform's scale.")]
        public bool usePlayerScale = true;
        [Tooltip("Will allow physics based falling when the user lets go of objects above ground.")]
        public bool useGravity = true;
        [Tooltip("An additional amount to move the player away from a wall if an ungrab teleport happens due to camera/object collisions.")]
        public float safeZoneTeleportOffset = 0.4f;

        /// <summary>
        /// Emitted when player climbing has started.
        /// </summary>
        public event PlayerClimbEventHandler PlayerClimbStarted;
        /// <summary>
        /// Emitted when player climbing has ended.
        /// </summary>
        public event PlayerClimbEventHandler PlayerClimbEnded;

        private Transform headCamera;
        private Transform controllerTransform;
        private Vector3 startControllerPosition;
        private Vector3 startPosition;
        private Vector3 lastGoodHeadsetPosition;
        private bool headsetColliding = false;
        private bool isClimbing = false;
        private VRTK_PlayerPresence playerPresence;
        private VRTK_HeadsetCollision headsetCollision;
        private VRTK_HeadsetFade headsetFade;
        private GameObject climbingObject;

        private void OnPlayerClimbStarted(PlayerClimbEventArgs e)
        {
            if (PlayerClimbStarted != null)
            {
                PlayerClimbStarted(this, e);
            }
        }

        private void OnPlayerClimbEnded(PlayerClimbEventArgs e)
        {
            if (PlayerClimbEnded != null)
            {
                PlayerClimbEnded(this, e);
            }
        }

        private PlayerClimbEventArgs SetPlayerClimbEvent(uint controllerIndex, GameObject target)
        {
            PlayerClimbEventArgs e;
            e.controllerIndex = controllerIndex;
            e.target = target;
            return e;
        }

        private void Awake()
        {
            playerPresence = GetComponent<VRTK_PlayerPresence>();
            if (useGravity)
            {
                if (!playerPresence)
                {
                    playerPresence = gameObject.AddComponent<VRTK_PlayerPresence>();
                }

                playerPresence.SetFallingPhysicsOnlyParams(true);
            }

            headCamera = VRTK_DeviceFinder.HeadsetTransform();
            headsetCollision = headCamera.GetComponent<VRTK_HeadsetCollision>();
            if (headsetCollision == null)
            {
                headsetCollision = headCamera.gameObject.AddComponent<VRTK_HeadsetCollision>();
            }

            headsetFade = headCamera.GetComponent<VRTK_HeadsetFade>();
            if (headsetFade == null)
            {
                headsetFade = headCamera.gameObject.AddComponent<VRTK_HeadsetFade>();
            }
        }

        private void OnEnable()
        {
            InitListeners(true);
        }

        private void OnDisable()
        {
            Ungrab(false, 0, climbingObject);
            InitListeners(false);
        }

        private void InitListeners(bool state)
        {
            InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), state);
            InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), state);

            InitTeleportListener(state);
            InitCollisionFadeListener(state);
        }

        private void InitTeleportListener(bool state)
        {
            // Listen for teleport events 
            VRTK_BasicTeleport teleportComponent = GetComponent<VRTK_BasicTeleport>();
            if (teleportComponent)
            {
                if (state)
                {
                    teleportComponent.Teleporting += new TeleportEventHandler(OnTeleport);
                }
                else
                {
                    teleportComponent.Teleporting -= new TeleportEventHandler(OnTeleport);
                }
            }
        }

        private void InitCollisionFadeListener(bool state)
        {
            if (state)
            {
                headsetCollision.HeadsetCollisionDetect += new HeadsetCollisionEventHandler(OnHeadsetCollisionDetected);
                headsetCollision.HeadsetCollisionEnded += new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
            }
            else
            {
                headsetCollision.HeadsetCollisionDetect -= new HeadsetCollisionEventHandler(OnHeadsetCollisionDetected);
                headsetCollision.HeadsetCollisionEnded -= new HeadsetCollisionEventHandler(OnHeadsetCollisionEnded);
            }
        }

        private Vector3 GetPosition(Transform objTransform)
        {
            if (usePlayerScale)
            {
                return transform.localRotation * Vector3.Scale(objTransform.localPosition, transform.localScale);
            }

            return transform.localRotation * objTransform.localPosition;
        }

        private void OnGrabObject(object sender, ObjectInteractEventArgs e)
        {
            if (IsClimbableObject(e.target))
            {
                climbingObject = e.target;
                if (useGravity)
                {
                    playerPresence.StopPhysicsFall();
                }

                OnPlayerClimbStarted(SetPlayerClimbEvent(e.controllerIndex, climbingObject));
                isClimbing = true;
                controllerTransform = ((VRTK_InteractGrab)sender).transform;
                startControllerPosition = GetPosition(controllerTransform);
                startPosition = transform.position;
            }
        }

        private void OnUngrabObject(object sender, ObjectInteractEventArgs e)
        {
            var controller = ((VRTK_InteractGrab)sender).gameObject;

            if (e.target && IsClimbableObject(e.target) && IsActiveClimbingController(controller))
            {
                Ungrab(true, e.controllerIndex, e.target);
            }
        }

        private void OnTeleport(object sender, DestinationMarkerEventArgs e)
        {
            Ungrab(false, e.controllerIndex, e.target.gameObject);
        }

        private void OnHeadsetCollisionDetected(object sender, HeadsetCollisionEventArgs e)
        {
            headsetFade.Fade(Color.black, 0.1f);
            headsetColliding = true;
        }

        private void OnHeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
        {
            headsetFade.Unfade(0.1f);
            headsetColliding = false;
        }

        // Should the object physics fall down, or teleport up
        private bool ShouldEnablePhysics()
        {
            bool enablePhysics = true;

            Transform headset = VRTK_DeviceFinder.HeadsetCamera();

            if (headset != null)
            {
                Ray ray = new Ray(headset.position, -transform.up);
                RaycastHit rayCollidedWith;
                bool rayHit = Physics.Raycast(ray, out rayCollidedWith);

                // if the in game floor is already at or above our playspace floor, 
                // just let VRTK_HeightAdjustTeleport teleport us up
                if (rayHit && rayCollidedWith.point.y > transform.position.y)
                {
                    enablePhysics = false;
                }

            }

            return enablePhysics;
        }

        private void Ungrab(bool carryMomentum, uint controllerIndex, GameObject target)
        {
            OnPlayerClimbEnded(SetPlayerClimbEvent(controllerIndex, target));
            isClimbing = false;

            // Move to the last safe spot
            if (headsetColliding)
            {
                Vector3 headsetPosition = headCamera.transform.position;

                Vector3 moveVector = lastGoodHeadsetPosition - headsetPosition;
                Vector3 moveDirection = moveVector.normalized;
                Vector3 moveOffset = moveDirection * safeZoneTeleportOffset;

                transform.position += moveVector + moveOffset;
            }

            if (useGravity && carryMomentum)
            {
                Vector3 velocity = Vector3.zero;
                var device = VRTK_DeviceFinder.TrackedObjectByIndex(controllerIndex);

                if (device)
                {
                    velocity = -device.GetComponent<VRTK_ControllerEvents>().GetVelocity();
                    if (usePlayerScale)
                    {
                        velocity = Vector3.Scale(velocity, transform.localScale);
                    }
                }

                // check if we should teleport up or physics fall down
                if (ShouldEnablePhysics())
                {
                    playerPresence.StartPhysicsFall(velocity);
                }
            }
            climbingObject = null;
        }

        private bool IsActiveClimbingController(GameObject controller)
        {
            return controller.transform == controllerTransform;
        }

        private bool IsClimbableObject(GameObject obj)
        {
            var interactObject = obj.GetComponent<VRTK_InteractableObject>();
            return interactObject != null && interactObject.AttachIsClimbObject();
        }

        private void Update()
        {
            if (isClimbing)
            {
                transform.position = startPosition - (GetPosition(controllerTransform) - startControllerPosition);
            }

            if (!headsetColliding)
            {
                lastGoodHeadsetPosition = headCamera.transform.position;
            }
        }


        private void InitControllerListeners(GameObject controller, bool state)
        {
            if (controller)
            {
                var grabbingController = controller.GetComponent<VRTK_InteractGrab>();
                if (grabbingController)
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