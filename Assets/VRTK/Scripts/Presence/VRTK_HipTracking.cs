// Hip Tracking|Presence|70050
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Hip Tracking attempts to reasonably track hip position in the absence of a hip position sensor.
    /// </summary>
    /// <remarks>
    /// The Hip Tracking script is placed on an empty GameObject which will be positioned at the estimated hip position.
    /// </remarks>

    public class VRTK_HipTracking : MonoBehaviour
    {
        [Tooltip("Distance underneath Player Head for hips to reside.")]
        public float HeadOffset = -0.35f;

        [Header("Optional")]
        [Tooltip("Optional Transform to use as the Head Object for calculating hip position. If none is given one will try to be found in the scene.")]
        public Transform headOverride;
        [Tooltip("Optional Transform to use for calculating which way is 'Up' relative to the player for hip positioning.")]
        public Transform ReferenceUp;

        private Transform playerHead;

        private void Awake()
        {
            if (headOverride != null)
            {
                playerHead = headOverride;
            }
            else
            {
                playerHead = VRTK_DeviceFinder.HeadsetTransform();
            }
        }

        private void Update()
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