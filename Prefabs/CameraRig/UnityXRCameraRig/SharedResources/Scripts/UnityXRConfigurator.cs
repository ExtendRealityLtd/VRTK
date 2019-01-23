namespace VRTK.Prefabs.CameraRig.UnityXRCameraRig
{
    using UnityEngine;
    using UnityEngine.XR;

    /// <summary>
    /// Provides configuration for the Unity Engine in XR.
    /// </summary>
    public class UnityXRConfigurator : MonoBehaviour
    {
        [Tooltip("Represents the type of physical space available for XR."), SerializeField]
        private TrackingSpaceType _trackingSpaceType = TrackingSpaceType.RoomScale;
        /// <summary>
        /// Represents the type of physical space available for XR.
        /// </summary>
        public TrackingSpaceType TrackingSpaceType
        {
            get
            {
                return _trackingSpaceType;
            }
            set
            {
                _trackingSpaceType = value;
                UpdateTrackingSpaceType();
            }
        }

        [Tooltip("Automatically set the Unity Physics Fixed Timestep value based on the headset render frequency."), SerializeField]
        private bool _lockPhysicsUpdateRateToRenderFrequency = true;
        /// <summary>
        /// Automatically set the Unity Physics Fixed Timestep value based on the headset render frequency.
        /// </summary>
        public bool LockPhysicsUpdateRateToRenderFrequency
        {
            get
            {
                return _lockPhysicsUpdateRateToRenderFrequency;
            }
            set
            {
                _lockPhysicsUpdateRateToRenderFrequency = value;
                UpdateFixedDeltaTime();
            }
        }

        protected virtual void OnEnable()
        {
            UpdateTrackingSpaceType();
        }

        protected virtual void OnValidate()
        {
            if (!isActiveAndEnabled || !Application.isPlaying)
            {
                return;
            }

            UpdateTrackingSpaceType();
            UpdateFixedDeltaTime();
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
            if (LockPhysicsUpdateRateToRenderFrequency && Time.timeScale > 0.0f)
            {
                Time.fixedDeltaTime = Time.timeScale / XRDevice.refreshRate;
            }
        }
    }
}