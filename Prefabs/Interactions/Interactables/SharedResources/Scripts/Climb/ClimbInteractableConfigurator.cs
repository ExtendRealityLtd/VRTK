namespace VRTK.Prefabs.Interactions.Interactables.Climb
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Event.Proxy;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Sets up the Interactable.Climbable prefab based on the provided user settings.
    /// </summary>
    public class ClimbInteractableConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public ClimbInteractableFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="Interactables.InteractableFacade"/> component acting as the interactable for climbing.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public InteractableFacade InteractableFacade { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> component handling a started climb.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter StartEventProxyEmitter { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> component handling a stopped climb.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter StopEventProxyEmitter { get; protected set; }
        #endregion

        protected virtual void OnEnable()
        {
            StartEventProxyEmitter.Emitted.AddListener(OnStartEventProxyEmitted);
            StopEventProxyEmitter.Emitted.AddListener(OnStopEventProxyEmitted);
        }

        protected virtual void OnDisable()
        {
            StopEventProxyEmitter.Emitted.RemoveListener(OnStopEventProxyEmitted);
            StartEventProxyEmitter.Emitted.RemoveListener(OnStartEventProxyEmitted);
        }

        /// <summary>
        /// Processes the start climbing functionality.
        /// </summary>
        /// <param name="interactor">The interactor initiating the climb.</param>
        protected virtual void OnStartEventProxyEmitted(GameObject interactor)
        {
            Facade.ClimbFacade.AddInteractor(interactor);
            Facade.ClimbFacade.AddInteractable(InteractableFacade.gameObject);
        }

        /// <summary>
        /// Processes the stop climbing functionality.
        /// </summary>
        /// <param name="interactor">The interactor that is no longer climbing.</param>
        protected virtual void OnStopEventProxyEmitted(GameObject interactor)
        {
            Facade.ClimbFacade.SetVelocitySource(interactor);
            Facade.ClimbFacade.SetVelocityMultiplier(Facade.ReleaseMultiplier);
            Facade.ClimbFacade.RemoveInteractable(InteractableFacade.gameObject);
            Facade.ClimbFacade.RemoveInteractor(interactor);
        }
    }
}