﻿// Position Rewind|Presence|70070
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Position Rewind script is used to reset the user back to a good known standing position upon receiving a headset collision event.
    /// </summary>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has the position rewind script to reset the user's position if they walk into objects.
    /// </example>
    [RequireComponent(typeof(VRTK_HeadsetCollision))]
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_PositionRewind")]
    public class VRTK_PositionRewind : MonoBehaviour
    {
        [Header("Rewind Settings")]

        [Tooltip("The amount of time from original headset collision until the rewind to the last good known position takes place.")]
        public float rewindDelay = 0.5f;
        [Tooltip("The additional distance to push the play area back upon rewind to prevent being right next to the wall again.")]
        public float pushbackDistance = 0.5f;
        [Tooltip("The threshold to determine how low the headset has to be before it is considered the user is crouching. The last good position will only be recorded in a non-crouching position.")]
        public float crouchThreshold = 0.5f;
        [Tooltip("The threshold to determind how low the headset can be to perform a position rewind. If the headset Y position is lower than this threshold then a rewind won't occur.")]
        public float crouchRewindThreshold = 0.1f;

        [Header("Custom Settings")]

        [Tooltip("The VRTK Body Physics script to use for the collisions and rigidbodies. If this is left blank then the first Body Physics script found in the scene will be used.")]
        public VRTK_BodyPhysics bodyPhysics;
        [Tooltip("The VRTK Headset Collision script to use to determine if the headset is colliding. If this is left blank then the script will need to be applied to the same GameObject.")]
        public VRTK_HeadsetCollision headsetCollision;

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

        protected virtual void OnEnable()
        {
            lastGoodPositionSet = false;
            headset = VRTK_DeviceFinder.HeadsetTransform();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            if (playArea == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
            }

            bodyPhysics = (bodyPhysics != null ? bodyPhysics : FindObjectOfType<VRTK_BodyPhysics>());
            headsetCollision = (headsetCollision != null ? headsetCollision : GetComponentInChildren<VRTK_HeadsetCollision>());
            ManageHeadsetListeners(true);
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
            if (!isColliding && playArea != null)
            {
                float floorVariant = 0.005f;
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
            isRewinding = false;
        }

        protected virtual bool BodyCollisionsEnabled()
        {
            return (bodyPhysics == null || bodyPhysics.enableBodyCollisions);
        }

        protected virtual void RewindPosition()
        {
            if (!isRewinding && playArea != null & lastGoodPositionSet && headset.localPosition.y > crouchRewindThreshold && BodyCollisionsEnabled())
            {
                isRewinding = true;
                Vector3 rewindDirection = lastGoodHeadsetPosition - headset.position;
                float rewindDistance = Vector2.Distance(new Vector2(headset.position.x, headset.position.z), new Vector2(lastGoodHeadsetPosition.x, lastGoodHeadsetPosition.z));
                playArea.Translate(rewindDirection.normalized * (rewindDistance + pushbackDistance));
                playArea.position = new Vector3(playArea.position.x, lastGoodStandingPosition.y, playArea.position.z);
                if (bodyPhysics != null)
                {
                    bodyPhysics.ResetVelocities();
                }
            }
        }

        protected virtual void ManageHeadsetListeners(bool state)
        {
            if (headsetCollision != null)
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