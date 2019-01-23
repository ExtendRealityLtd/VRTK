namespace VRTK.Prefabs.CameraRig.CameraRigSwitcher
{
    using UnityEngine;
    using UnityEngine.XR;

    /// <summary>
    /// Provides configuration for the XRSettings.
    /// </summary>
    public class XRSettingsConfigurator : MonoBehaviour
    {
        /// <summary>
        /// Enables XR in the Unity Software.
        /// </summary>
        public virtual void EnableXR()
        {
            XRSettings.enabled = true;
        }

        /// <summary>
        /// Disables XR in the Unity Software.
        /// </summary>
        public virtual void DisableXR()
        {
            XRSettings.enabled = false;
        }
    }
}