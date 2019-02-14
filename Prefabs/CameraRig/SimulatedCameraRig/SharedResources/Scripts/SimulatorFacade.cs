namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using UnityEngine.XR;
    using Zinnia.Data.Operation;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.CameraRig.TrackedAlias;

    /// <summary>
    /// Provides configuration for the Simulated CameraRig.
    /// </summary>
    public class SimulatorFacade : MonoBehaviour
    {
        #region Simulator Settings
        [Header("Simulator Settings"), Tooltip("The optional Tracked Alias prefab, must be provided if one is used in the scene."), SerializeField]
        private TrackedAliasFacade _trackedAlias;
        /// <summary>
        /// The optional Tracked Alias prefab, must be provided if one is used in the scene.
        /// </summary>
        public TrackedAliasFacade TrackedAlias
        {
            get { return _trackedAlias; }
            set
            {
                _trackedAlias = value;
                ConfigureTrackedAlias();
            }
        }

        [Tooltip("Determines whether to disable the XRSettings."), SerializeField]
        private bool _disableXRSettings = true;
        /// <summary>
        /// Determines whether to disable the XRSettings.
        /// </summary>
        public bool DisableXRSettings
        {
            get
            {
                return _disableXRSettings;
            }
            set
            {
                _disableXRSettings = value;
                ConfigureXRSettings(_disableXRSettings);
            }
        }

        [Tooltip("The frame rate to simulate with fixedDeltaTime."), SerializeField]
        private float _simulatedFrameRate = 90f;
        /// <summary>
        /// The frame rate to simulate with fixedDeltaTime.
        /// </summary>
        public float SimulatedFrameRate
        {
            get
            {
                return _simulatedFrameRate;
            }
            set
            {
                _simulatedFrameRate = value;
                ConfigureSimulatedFrameRate();
            }
        }
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked TransformPositionMutator.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked TransformPositionMutator."), InternalSetting, SerializeField]
        protected TransformPositionMutator playAreaPosition;
        /// <summary>
        /// The linked TransformPropertyResetter.
        /// </summary>
        [Tooltip("The linked TransformPropertyResetter."), InternalSetting, SerializeField]
        protected TransformPropertyResetter playAreaResetter;
        #endregion

        /// <summary>
        /// The original configuration of XRSettings.
        /// </summary>
        protected bool originalXRSettings;
        /// <summary>
        /// The original configuration of FixedDeltaTime.
        /// </summary>
        protected float originalFixedDeltaTime;

        protected virtual void OnEnable()
        {
            originalXRSettings = XRSettings.enabled;
            originalFixedDeltaTime = Time.fixedDeltaTime;
            ConfigureTrackedAlias();
        }

        protected virtual void OnDisable()
        {
            ConfigureXRSettings(false);
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }

        protected virtual void OnValidate()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            ConfigureXRSettings(DisableXRSettings);
            ConfigureSimulatedFrameRate();

            if (Application.isPlaying)
            {
                ConfigureTrackedAlias();
            }
        }

        /// <summary>
        /// Configures the provided <see cref="TrackedAlias"/> onto the simulator CameraRig.
        /// </summary>
        protected virtual void ConfigureTrackedAlias()
        {
            if (TrackedAlias != null)
            {
                playAreaPosition.Target = playAreaPosition != null ? TrackedAlias.PlayAreaAlias.gameObject : null;
                playAreaResetter.source = playAreaResetter != null ? TrackedAlias.PlayAreaAlias.transform : null;
            }
        }

        /// <summary>
        /// Configures the XRSettings.
        /// </summary>
        /// <param name="state">The new value for the setting.</param>
        protected virtual void ConfigureXRSettings(bool state)
        {
            if (state)
            {
                originalXRSettings = XRSettings.enabled;
                XRSettings.enabled = false;
            }
            else
            {
                XRSettings.enabled = originalXRSettings;
            }
        }

        /// <summary>
        /// Configures the simulated frame rate.
        /// </summary>
        protected virtual void ConfigureSimulatedFrameRate()
        {
            Time.fixedDeltaTime = Time.timeScale / SimulatedFrameRate;
        }
    }
}