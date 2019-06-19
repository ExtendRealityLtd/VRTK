namespace VRTK.Prefabs.Interactions.Haptics
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using VRTK.Prefabs.Interactions.Interactors;

    public class InteractorHapticsFacade : MonoBehaviour
    {
        #region Haptics Settings
        /// <summary>
        /// The intensity of the haptic rumble.
        /// </summary>
        [Serialized]
        [field: Header("Haptics Settings"), DocumentedByXml]
        public float Intensity { get; set; } = 1f;
        /// <summary>
        /// Whether to only apply haptics on the active interacting <see cref="InteractorFacade"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool OnlyRumbleActiveInteractor { get; set; } = true;
        #endregion

        #region Interactor Settings
        /// <summary>
        /// The interactors that are considered part of the left controller.
        /// </summary>
        [Serialized]
        [field: Header("Interactor Settings"), DocumentedByXml]
        public UnityObjectObservableList LeftInteractors { get; set; }
        /// <summary>
        /// The interactors that are considered part of the right controller.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public UnityObjectObservableList RightInteractors { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Interactor Haptics Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public InteractorHapticsConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Applies the defined rules.
        /// </summary>
        /// <param name="source">The source to match the rule against.</param>
        public virtual void ApplyRules(object source)
        {
            Configuration.RulesToMatch.Match(source);
        }

        /// <summary>
        /// Starts the haptics process.
        /// </summary>
        public virtual void Begin()
        {
            Configuration.BeginHaptics.Receive();
        }

        /// <summary>
        /// Cancels the haptics process.
        /// </summary>
        public virtual void Cancel()
        {
            Configuration.CancelHaptics.Receive();
        }

        /// <summary>
        /// Called after <see cref="Intensity"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Intensity))]
        protected virtual void OnAfterIntensityChange()
        {
            Configuration.LeftHapicPuliser.Intensity = Intensity;
            Configuration.RightHapicPuliser.Intensity = Intensity;
        }

        /// <summary>
        /// Called after <see cref="OnlyRumbleActiveInteractor"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(OnlyRumbleActiveInteractor))]
        protected virtual void OnAfterOnlyRumbleActiveInteractorChange()
        {
            Configuration.RulesToMatch.gameObject.SetActive(OnlyRumbleActiveInteractor);
        }
    }
}