﻿// Hip Tracking|Presence|70050
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Attempts to provide the relative position of a hip without the need for additional hardware sensors.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_HipTracking` script on any active scene GameObject and this GameObject will then track to the estimated hip position.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_HipTracking")]
    public class VRTK_HipTracking : MonoBehaviour
    {
        [Tooltip("Distance underneath Player Head for hips to reside.")]
        public float HeadOffset = -0.35f;

        [Header("Optional")]
        [Tooltip("Optional Transform to use as the Head Object for calculating hip position. If none is given one will try to be found in the scene.")]
        public Transform headOverride;
        [Tooltip("Optional Transform to use for calculating which way is 'Up' relative to the player for hip positioning.")]
        public Transform ReferenceUp;

        protected Transform playerHead;

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            playerHead = (headOverride != null ? headOverride : VRTK_DeviceFinder.HeadsetTransform());
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void LateUpdate()
        {
            if (playerHead == null)
            {
                return;
            }
            Vector3 up = Vector3.up;
            if (ReferenceUp != null)
            {
                up = ReferenceUp.up;
            }

            transform.position = playerHead.position + (HeadOffset * up);

            Vector3 forward = playerHead.forward;
            Vector3 forwardLeveld1 = forward;
            forwardLeveld1.y = 0;
            forwardLeveld1.Normalize();
            Vector3 mixedInLocalForward = playerHead.up;
            if (forward.y > 0)
            {
                mixedInLocalForward = -playerHead.up;
            }
            mixedInLocalForward.y = 0;
            mixedInLocalForward.Normalize();

            float dot = Mathf.Clamp(Vector3.Dot(forwardLeveld1, forward), 0f, 1f);
            Vector3 finalForward = Vector3.Lerp(mixedInLocalForward, forwardLeveld1, dot * dot);
            transform.rotation = Quaternion.LookRotation(finalForward, up);
        }
    }
}