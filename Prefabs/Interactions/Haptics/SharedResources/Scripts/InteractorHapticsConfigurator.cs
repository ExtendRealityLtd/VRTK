namespace VRTK.Prefabs.Interactions.Haptics
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Rule;
    using Zinnia.Haptics;
    using Zinnia.Extension;
    using Zinnia.Event.Proxy;
    using Zinnia.Data.Attribute;

    public class InteractorHapticsConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public InteractorHapticsFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="RulesMatcher"/> to determine which controller to rumble.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public RulesMatcher RulesToMatch { get; protected set; }
        /// <summary>
        /// The <see cref="XRNodeHapticPulser"/> that rumbles the left controller.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public XRNodeHapticPulser LeftHapicPuliser { get; protected set; }
        /// <summary>
        /// The <see cref="XRNodeHapticPulser"/> that rumbles the right controller.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public XRNodeHapticPulser RightHapicPuliser { get; protected set; }
        /// <summary>
        /// The <see cref="EmptyEventProxyEmitter"/> that initiates the haptics process.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public EmptyEventProxyEmitter BeginHaptics { get; protected set; }
        /// <summary>
        /// The <see cref="EmptyEventProxyEmitter"/> that cancels the haptics process.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public EmptyEventProxyEmitter CancelHaptics { get; protected set; }
        #endregion

        protected virtual void OnEnable()
        {
            LeftHapicPuliser.RunWhenActiveAndEnabled(() => LeftHapicPuliser.Intensity = Facade.Intensity);
            RightHapicPuliser.RunWhenActiveAndEnabled(() => RightHapicPuliser.Intensity = Facade.Intensity);
            RulesToMatch.gameObject.SetActive(Facade.OnlyRumbleActiveInteractor);
        }
    }
}