// Position Rewind|Presence|70070
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="collidedPosition">The position of the play area when it collded.</param>
    /// <param name="resetPosition">The position of the play area when it has been rewinded to a safe position.</param>
    public struct PositionRewindEventArgs
    {
        public Vector3 collidedPosition;
        public Vector3 resetPosition;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="PositionRewindEventArgs"/></param>
    public delegate void PositionRewindEventHandler(object sender, PositionRewindEventArgs e);

    /// <summary>
    /// Attempts to rewind the position of the play area to a last know valid position upon the headset collision event.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `VRTK_BodyPhysics` - A Body Physics script to manage the collisions of the body presence within the scene.
    ///  * `VRTK_HeadsetCollision` - A Headset Collision script to determine when the headset is colliding with valid geometry.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_PositionRewind` script on any active scene GameObject.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has the position rewind script to reset the user's position if they walk into objects.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_PositionRewind")]
    public class VRTK_PositionRewind : MonoBehaviour
    {
        /// <summary>
        /// Valid collision detectors.
        /// </summary>
        public enum CollisionDetectors
        {
            /// <summary>
            /// Listen for collisions on the headset collider only.
            /// </summary>
            HeadsetOnly,
            /// <summary>
            /// Listen for collisions on the body physics collider only.
            /// </summary>
            BodyOnly,
            /// <summary>
            /// Listen for collisions on both the headset collider and body physics collider.
            /// </summary>
            HeadsetAndBody
        }

        [Header("Rewind Settings")]

        [Tooltip("The colliders to determine if a collision has occured for the rewind to be actioned.")]
        public CollisionDetectors collisionDetector = CollisionDetectors.HeadsetOnly;
        [Tooltip("If this is checked then the collision detector will ignore colliders set to `Is Trigger = true`.")]
        public bool ignoreTriggerColliders = false;
        [Tooltip("The amount of time from original headset collision until the rewind to the last good known position takes place.")]
        public float rewindDelay = 0.5f;
        [Tooltip("The additional distance to push the play area back upon rewind to prevent being right next to the wall again.")]
        public float pushbackDistance = 0.5f;
        [Tooltip("The threshold to determine how low the headset has to be before it is considered the user is crouching. The last good position will only be recorded in a non-crouching position.")]
        public float crouchThreshold = 0.5f;
        [Tooltip("The threshold to determind how low the headset can be to perform a position rewind. If the headset Y position is lower than this threshold then a rewind won't occur.")]
        public float crouchRewindThreshold = 0.1f;
        [Tooltip("A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Position Rewind.")]
        public VRTK_PolicyList targetListPolicy;

        [Header("Custom Settings")]

        [Tooltip("The VRTK Body Physics script to use for the collisions and rigidbodies. If this is left blank then the first Body Physics script found in the scene will be used.")]
        public VRTK_BodyPhysics bodyPhysics;
        [Tooltip("The VRTK Headset Collision script to use to determine if the headset is colliding. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_HeadsetCollision headsetCollision;

        /// <summary>
        /// Emitted when the draggable item is successfully dropped.
        /// </summary>
        public event PositionRewindEventHandler PositionRewindToSafe;

        protected Transform headset;
        protected Transform playArea;

        protected Vector3 lastGoodStandingPosition;
        protected Vector3 lastGoodHeadsetPosition;
        protected float highestHeadsetY;
        protected float lastPlayAreaY;
        protected bool lastGoodPositionSet = false;
        protected bool hasCollided = false;
        protected bool isColliding = false;
        protected bool isRewinding = false;
        protected float collideTimer = 0f;

        public virtual void OnPositionRewindToSafe(PositionRewindEventArgs e)
        {
            if (PositionRewindToSafe != null)
            {
                PositionRewindToSafe(this, e);
            }
        }

        /// <summary>
        /// The SetLastGoodPosition method stores the current valid play area and headset position.
        /// </summary>
        public virtual void SetLastGoodPosition()
        {
            if (playArea != null && headset != null)
            {
                lastGoodPositionSet = true;
                lastGoodStandingPosition = playArea.position;
                lastGoodHeadsetPosition = headset.position;
            }
        }

        /// <summary>
        /// The RewindPosition method resets the play area position to the last known good position of the play area.
        /// </summary>
        public virtual void RewindPosition()
        {
            if (headset != null)
            {
                Vector3 storedPosition = playArea.position;
                Vector3 resetVector = lastGoodHeadsetPosition - headset.position;
                Vector3 moveOffset = resetVector.normalized * pushbackDistance;
                playArea.position += resetVector + moveOffset;
                if (bodyPhysics != null)
                {
                    bodyPhysics.ResetVelocities();
                }
                OnPositionRewindToSafe(SetEventPayload(storedPosition));
            }
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            lastGoodPositionSet = false;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            bodyPhysics = (bodyPhysics != null ? bodyPhysics : FindObjectOfType<VRTK_BodyPhysics>());
            headsetCollision = (headsetCollision != null ? headsetCollision : GetComponentInChildren<VRTK_HeadsetCollision>());
            ManageListeners(true);
        }

        protected virtual void OnDisable()
        {
            ManageListeners(false);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            if (isColliding)
            {
                if (collideTimer > 0f)
                {
                    collideTimer -= Time.deltaTime;
                }
                else
                {
                    collideTimer = 0f;
                    isColliding = false;
                    DoPositionRewind();
                }
            }
        }

        protected virtual PositionRewindEventArgs SetEventPayload(Vector3 previousPosition)
        {
            PositionRewindEventArgs e;
            e.collidedPosition = previousPosition;
            e.resetPosition = playArea.position;
            return e;
        }

        protected virtual bool CrouchThresholdReached()
        {
            float floorVariant = 0.005f;
            return (playArea.position.y > (lastPlayAreaY + floorVariant) || playArea.position.y < (lastPlayAreaY - floorVariant));
        }

        protected virtual void SetHighestHeadsetY()
        {
            highestHeadsetY = (CrouchThresholdReached() ? crouchThreshold : (headset.localPosition.y > highestHeadsetY) ? headset.localPosition.y : highestHeadsetY);
        }

        protected virtual void UpdateLastGoodPosition()
        {
            float highestYDiff = highestHeadsetY - crouchThreshold;
            if (headset.localPosition.y > highestYDiff && highestYDiff > crouchThreshold)
            {
                SetLastGoodPosition();
            }
            lastPlayAreaY = playArea.position.y;
        }

        protected virtual void FixedUpdate()
        {
            if (!isColliding && playArea != null)
            {
                SetHighestHeadsetY();
                UpdateLastGoodPosition();
            }
        }

        protected virtual void StartCollision(GameObject target, Collider collider)
        {
            if (ignoreTriggerColliders && collider.isTrigger)
            {
                return;
            }

            if (!VRTK_PolicyList.Check(target, targetListPolicy))
            {
                isColliding = true;
                if (!hasCollided && collideTimer <= 0f)
                {
                    hasCollided = true;
                    collideTimer = rewindDelay;
                }
            }
        }

        protected virtual void EndCollision(Collider collider)
        {
            if (ignoreTriggerColliders && collider != null && collider.isTrigger)
            {
                return;
            }

            isColliding = false;
            hasCollided = false;
            isRewinding = false;
        }

        protected virtual bool BodyCollisionsEnabled()
        {
            return (bodyPhysics == null || bodyPhysics.enableBodyCollisions);
        }

        protected virtual bool CanRewind()
        {
            return (!isRewinding && playArea != null & lastGoodPositionSet && headset.localPosition.y > crouchRewindThreshold && BodyCollisionsEnabled());
        }

        protected virtual void DoPositionRewind()
        {
            if (CanRewind())
            {
                isRewinding = true;
                RewindPosition();
            }
        }

        protected virtual bool HeadsetListen()
        {
            return (collisionDetector == CollisionDetectors.HeadsetAndBody || collisionDetector == CollisionDetectors.HeadsetOnly);
        }

        protected virtual bool BodyListen()
        {
            return (collisionDetector == CollisionDetectors.HeadsetAndBody || collisionDetector == CollisionDetectors.BodyOnly);
        }

        protected virtual void ManageListeners(bool state)
        {
            if (state)
            {
                if (headsetCollision != null && HeadsetListen())
                {
                    headsetCollision.HeadsetCollisionDetect += HeadsetCollisionDetect;
                    headsetCollision.HeadsetCollisionEnded += HeadsetCollisionEnded;
                }
                if (bodyPhysics != null && BodyListen())
                {
                    bodyPhysics.StartColliding += StartColliding;
                    bodyPhysics.StopColliding += StopColliding;
                }
            }
            else
            {
                if (headsetCollision != null && HeadsetListen())
                {
                    headsetCollision.HeadsetCollisionDetect -= HeadsetCollisionDetect;
                    headsetCollision.HeadsetCollisionEnded -= HeadsetCollisionEnded;
                }
                if (bodyPhysics != null && BodyListen())
                {
                    bodyPhysics.StartColliding -= StartColliding;
                    bodyPhysics.StopColliding -= StopColliding;
                }
            }
        }

        private void StartColliding(object sender, BodyPhysicsEventArgs e)
        {
            StartCollision(e.target, e.collider);
        }

        private void StopColliding(object sender, BodyPhysicsEventArgs e)
        {
            EndCollision(e.collider);
        }

        protected virtual void HeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
        {
            StartCollision(e.collider.gameObject, e.collider);
        }

        protected virtual void HeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
        {
            EndCollision(e.collider);
        }
    }
}