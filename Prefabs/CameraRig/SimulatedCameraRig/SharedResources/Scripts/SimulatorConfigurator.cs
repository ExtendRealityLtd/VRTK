namespace VRTK.Prefabs.CameraRig.SimulatedCameraRig
{
    using UnityEngine;
    using UnityEngine.XR;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Sets up the Simulated CameraRig Prefab based on the provided user settings.
    /// </summary>
    public class SimulatorConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public SimulatorFacade Facade { get; protected set; }
        #endregion

        #region Control Settings
        /// <summary>
        /// The linked <see cref="ActiveObjectController"/> for the PlayArea.
        /// </summary>
        [Serialized]
        [field: Header("Control Settings"), DocumentedByXml, Restricted]
        public ActiveObjectController PersonObjectController { get; protected set; }
        /// <summary>
        /// The linked <see cref="ActiveObjectController"/> for the Left Controller.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ActiveObjectController LeftObjectController { get; protected set; }
        /// <summary>
        /// The linked <see cref="ActiveObjectController"/> for the Right Controller.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ActiveObjectController RightObjectController { get; protected set; }
        #endregion

        /// <summary>
        /// The original value of <see cref="XRSettings.enabled"/>.
        /// </summary>
        protected bool wereXRSettingsEnabled;
        /// <summary>
        /// The original value of <see cref="Time.fixedDeltaTime"/>.
        /// </summary>
        protected float originalFixedDeltaTime;

        /// <summary>
        /// Disables <see cref="XRSettings.enabled"/> or restores it to its original value.
        /// </summary>
        /// <param name="state">Whether to disable <see cref="XRSettings.enabled"/> or to restore it back to its original value.</param>
        public virtual void ConfigureXRSettings(bool state)
        {
            if (state)
            {
                wereXRSettingsEnabled = XRSettings.enabled;
                XRSettings.enabled = false;
            }
            else
            {
                XRSettings.enabled = wereXRSettingsEnabled;
            }
        }

        /// <summary>
        /// Sets <see cref="Time.fixedDeltaTime"/> to the simulated frame rate.
        /// </summary>
        public virtual void ConfigureSimulatedFrameRate()
        {
            Time.fixedDeltaTime = Time.timeScale / Facade.SimulatedFrameRate;
        }

        /// <summary>
        /// Configures the speed in which the objects are controlled at.
        /// </summary>
        public virtual void ConfigureControlSpeed()
        {
            PersonObjectController.RunWhenActiveAndEnabled(() => PersonObjectController.MovementSpeed = Facade.PlayerSpeed);
            LeftObjectController.RunWhenActiveAndEnabled(() => LeftObjectController.MovementSpeed = Facade.ControllerSpeed);
            RightObjectController.RunWhenActiveAndEnabled(() => RightObjectController.MovementSpeed = Facade.ControllerSpeed);
        }

        protected virtual void OnEnable()
        {
            wereXRSettingsEnabled = XRSettings.enabled;
            originalFixedDeltaTime = Time.fixedDeltaTime;
            ConfigureXRSettings(Facade.DisableXRSettings);
            ConfigureControlSpeed();
        }

        protected virtual void OnDisable()
        {
            ConfigureXRSettings(false);
            Time.fixedDeltaTime = originalFixedDeltaTime;
        }
    }
}