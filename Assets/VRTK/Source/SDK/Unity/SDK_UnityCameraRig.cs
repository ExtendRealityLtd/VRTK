namespace VRTK
{
    using UnityEngine;
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
    using XRDevice = UnityEngine.VR.VRDevice;
#endif

    /// <summary>
    /// The `[UnityBase_CameraRig]` prefab is a default camera rig set up for use with the Unity SDK support.
    /// </summary>
    /// <remarks>
    /// The Unity CameraRig also utilises the Unity Controller Tracker and Headset Tracker to enable GameObject tracking of it's position/rotation to the available connected VR device via the `UnityEngine.VR` library.
    /// </remarks>
    public class SDK_UnityCameraRig : MonoBehaviour
    {
        [Tooltip("Automatically set the Unity Physics Fixed Timestep value based on the HMD render frequency.")]
        public bool lockPhysicsUpdateRateToRenderFrequency = true;

        protected virtual void Update()
        {
            if (lockPhysicsUpdateRateToRenderFrequency && Time.timeScale > 0.0f)
            {
                Time.fixedDeltaTime = Time.timeScale / XRDevice.refreshRate;
            }
        }
    }
}