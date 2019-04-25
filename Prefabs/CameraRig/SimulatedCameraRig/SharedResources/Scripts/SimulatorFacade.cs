namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using UnityEngine.XR;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Operation.Mutation;
    using VRTK.Prefabs.CameraRig.TrackedAlias;

    /// <summary>
    /// The public interface into the SimulatedCameraRig Prefab.
    /// </summary>
    public class SimulatorFacade : MonoBehaviour
    {
        #region Simulator Settings
        /// <summary>
        /// The optional Tracked Alias prefab, must be provided if one is used in the scene.
        /// </summary>
        [Serialized]
        [field: Header("Simulator Settings"), DocumentedByXml]
        public TrackedAliasFacade TrackedAlias { get; set; }
        /// <summary>
        /// Determines whether to disable the XRSettings.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool DisableXRSettings { get; set; } = true;
        /// <summary>
        /// The frame rate to simulate with fixedDeltaTime.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float SimulatedFrameRate { get; set; } = 90f;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked TransformPositionMutator.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public TransformPositionMutator PlayAreaPosition { get; protected set; }
        /// <summary>
        /// The linked TransformPropertyResetter.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPropertyResetter PlayAreaResetter { get; protected set; }
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
            ConfigureXRSettings(DisableXRSettings);
        }

        protected virtual void OnDisable()
        {
            ConfigureXRSettings(false);
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }

        /// <summary>
        /// Configures the provided <see cref="TrackedAlias"/> onto the simulator CameraRig.
        /// </summary>
        protected virtual void ConfigureTrackedAlias()
        {
            if (TrackedAlias == null)
            {
                return;
            }

            GameObject playAreaObject = TrackedAlias.PlayAreaAlias.gameObject;
            PlayAreaPosition.Target = PlayAreaPosition != null ? playAreaObject : null;
            PlayAreaResetter.Source = PlayAreaResetter != null ? playAreaObject : null;
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

        /// <summary>
        /// Called after <see cref="TrackedAlias"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(TrackedAlias))]
        protected virtual void OnAfterTrackedAliasChange()
        {
            ConfigureTrackedAlias();
        }

        /// <summary>
        /// Called after <see cref="DisableXRSettings"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(DisableXRSettings))]
        protected virtual void OnAfterDisableXRSettingsChange()
        {
            ConfigureXRSettings(DisableXRSettings);
        }

        /// <summary>
        /// Called after <see cref="SimulatedFrameRate"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SimulatedFrameRate))]
        protected virtual void OnAfterSimulatedFrameRateChange()
        {
            ConfigureSimulatedFrameRate();
        }
    }
}