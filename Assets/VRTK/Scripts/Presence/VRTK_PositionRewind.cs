// Position Rewind|Presence|70070
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Position Rewind script is used to reset the user back to a good known standing position upon receiving a headset collision event.
    /// </summary>
    /// <example>
    /// /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has the position rewind script to reset the user's position if they walk into objects.
    /// </example>
    [RequireComponent(typeof(VRTK_HeadsetCollision))]
    public class VRTK_PositionRewind : MonoBehaviour
    {
        [Tooltip("The amount of time from original headset collision until the rewind to the last good known position takes place.")]
        public float rewindDelay = 0.5f;
        [Tooltip("The additional distance to push the play area back upon rewind to prevent being right next to the wall again.")]
        public float pushbackDistance = 0.5f;
        [Tooltip("The threshold to determine how low the headset has to be before it is considered the user is crouching. The last good position will only be recorded in a non-crouching position.")]
        public float crouchThreshold = 0.5f;
        [Tooltip("The threshold to determind how low the headset can be to perform a position rewind. If the headset Y position is lower than this threshold then a rewind won't occur.")]
        public float crouchRewindThreshold = 0.1f;

        protected Transform headset;
        protected Transform playArea;
        protected Rigidbody playareaRigidbody;
        protected VRTK_BodyPhysics bodyPhysics;
        protected VRTK_HeadsetCollision headsetCollision;

        protected Vector3 lastGoodStandingPosition;
        protected Vector3 lastGoodHeadsetPosition;
        protected float highestHeadsetY;
        protected float lastPlayAreaY;
        protected bool lastGoodPositionSet = false;
        protected bool hasCollided = false;
        protected bool isColliding = false;
        protected float collideTimer = 0f;

        protected virtual void OnEnable()
        {
            lastGoodPositionSet = false;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            bodyPhysics = FindObjectOfType<VRTK_BodyPhysics>();
            playareaRigidbody = playArea.GetComponent<Rigidbody>();
            headsetCollision = GetComponent<VRTK_HeadsetCollision>();
            ManageHeadsetListeners(true);
            if (!playArea)
            {
                Debug.LogError("No play area could be found. Have you selected a valid Boundaries SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Boundaries SDK from the dropdown.");
            }
        }

        protected virtual void OnDisable()
        {
            ManageHeadsetListeners(false);
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
                    RewindPosition();
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!isColliding && playArea)
            {
                var floorVariant = 0.005f;
                if (playArea.position.y > (lastPlayAreaY + floorVariant) || playArea.position.y < (lastPlayAreaY - floorVariant))
                {
                    highestHeadsetY = crouchThreshold;
                }

                if (headset.localPosition.y > highestHeadsetY)
                {
                    highestHeadsetY = headset.localPosition.y;
                }

                float highestYDiff = highestHeadsetY - crouchThreshold;

                if (headset.localPosition.y > highestYDiff && highestYDiff > crouchThreshold)
                {
                    lastGoodPositionSet = true;
                    lastGoodStandingPosition = playArea.position;
                    lastGoodHeadsetPosition = headset.position;
                }

                lastPlayAreaY = playArea.position.y;
            }
        }

        protected virtual void StartCollision()
        {
            isColliding = true;
            if (!hasCollided && collideTimer <= 0f)
            {
                hasCollided = true;
                collideTimer = rewindDelay;
            }
        }

        protected virtual void EndCollision()
        {
            isColliding = false;
            hasCollided = false;
        }

        protected virtual bool BodyCollisionsEnabled()
        {
            return (bodyPhysics == null || bodyPhysics.enableBodyCollisions);
        }

        protected virtual void RewindPosition()
        {
            if (lastGoodPositionSet && headset.localPosition.y > crouchRewindThreshold && BodyCollisionsEnabled())
            {
                var xReset = playArea.position.x - (headset.position.x - lastGoodHeadsetPosition.x);
                var zReset = playArea.position.z - (headset.position.z - lastGoodHeadsetPosition.z);

                var currentPosition = new Vector3(headset.position.x, lastGoodStandingPosition.y, headset.position.z);
                var resetPosition = new Vector3(xReset, lastGoodStandingPosition.y, zReset);
                var finalPosition = currentPosition + (resetPosition - currentPosition).normalized * (Vector3.Distance(resetPosition, currentPosition) + pushbackDistance);

                playArea.position = finalPosition;
                if (playareaRigidbody)
                {
                    playareaRigidbody.velocity = Vector3.zero;
                    playareaRigidbody.angularVelocity = Vector3.zero;
                }
            }
        }

        protected virtual void ManageHeadsetListeners(bool state)
        {
            if (headsetCollision)
            {
                if (state)
                {
                    headsetCollision.HeadsetCollisionDetect += HeadsetCollision_HeadsetCollisionDetect;
                    headsetCollision.HeadsetCollisionEnded += HeadsetCollision_HeadsetCollisionEnded;
                }
                else
                {
                    headsetCollision.HeadsetCollisionDetect -= HeadsetCollision_HeadsetCollisionDetect;
                    headsetCollision.HeadsetCollisionEnded -= HeadsetCollision_HeadsetCollisionEnded;
                }
            }
        }

        protected virtual void HeadsetCollision_HeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
        {
            StartCollision();
        }

        protected virtual void HeadsetCollision_HeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
        {
            EndCollision();
        }
    }
}