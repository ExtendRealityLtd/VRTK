namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into the SimulatedCameraRig Prefab.
    /// </summary>
    public class SimulatorFacade : MonoBehaviour
    {
        #region Simulator Settings
        /// <summary>
        /// The speed at which to move the player around via the default movement keys.
        /// </summary>
        [Serialized]
        [field: Header("Simulator Settings"), DocumentedByXml]
        public float PlayerSpeed { get; set; } = 0.05f;
        /// <summary>
        /// The speed at which to move the controllers around via the default movement keys.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float ControllerSpeed { get; set; } = 0.025f;
        #endregion

        #region System Settings
        /// <summary>
        /// Determines whether to disable <see cref="XRSettings.enabled"/>.
        /// </summary>
        [Serialized]
        [field: Header("System Settings"), DocumentedByXml]
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
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public SimulatorConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Called after <see cref="PlayerSpeed"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(PlayerSpeed))]
        protected virtual void OnAfterPlayerSpeedChange()
        {
            Configuration.ConfigureControlSpeed();
        }

        /// <summary>
        /// Called after <see cref="ControllerSpeed"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ControllerSpeed))]
        protected virtual void OnAfterControllerSpeedChange()
        {
            Configuration.ConfigureControlSpeed();
        }

        /// <summary>
        /// Called after <see cref="DisableXRSettings"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(DisableXRSettings))]
        protected virtual void OnAfterDisableXRSettingsChange()
        {
            Configuration.ConfigureXRSettings(DisableXRSettings);
        }

        /// <summary>
        /// Called after <see cref="SimulatedFrameRate"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SimulatedFrameRate))]
        protected virtual void OnAfterSimulatedFrameRateChange()
        {
            Configuration.ConfigureSimulatedFrameRate();
        }
    }
}