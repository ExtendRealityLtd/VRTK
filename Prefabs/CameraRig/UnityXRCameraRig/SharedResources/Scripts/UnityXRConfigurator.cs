namespace VRTK.Prefabs.CameraRig.UnityXRCameraRig
{
    using UnityEngine;
    using UnityEngine.XR;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.MemberChangeMethod;

    /// <summary>
    /// Provides configuration for the Unity Engine in XR.
    /// </summary>
    public class UnityXRConfigurator : MonoBehaviour
    {
        /// <summary>
        /// Represents the type of physical space available for XR.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public TrackingSpaceType TrackingSpaceType { get; set; } = TrackingSpaceType.RoomScale;
        /// <summary>
        /// Automatically set the Unity Physics Fixed Timestep value based on the headset render frequency.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool LockPhysicsUpdateRateToRenderFrequency { get; set; } = true;

        protected virtual void OnEnable()
        {
            UpdateTrackingSpaceType();
        }

        protected virtual void Update()
        {
            UpdateFixedDeltaTime();
        }

        /// <summary>
        /// Updates the tracking space type.
        /// </summary>
        protected virtual void UpdateTrackingSpaceType()
        {
            XRDevice.SetTrackingSpaceType(TrackingSpaceType);
        }

        /// <summary>
        /// Updates the fixed delta time to the appropriate value.
        /// </summary>
        protected virtual void UpdateFixedDeltaTime()
        {
            if (LockPhysicsUpdateRateToRenderFrequency
                && Time.timeScale > 0.0f
                && !string.IsNullOrEmpty(XRSettings.loadedDeviceName))
            {
                Time.fixedDeltaTime = Time.timeScale / XRDevice.refreshRate;
            }
        }

        /// <summary>
        /// Called after <see cref="TrackingSpaceType"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(TrackingSpaceType))]
        protected virtual void OnAfterTrackingSpaceTypeChange()
        {
            UpdateTrackingSpaceType();
        }

        /// <summary>
        /// Called after <see cref="LockPhysicsUpdateRateToRenderFrequency"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LockPhysicsUpdateRateToRenderFrequency))]
        protected virtual void OnAfterLockPhysicsUpdateRateToRenderFrequencyChange()
        {
            UpdateFixedDeltaTime();
        }
    }
}