namespace VRTK.Prefabs.Interactions.Interactables.Climb
{
    using UnityEngine;
    using Zinnia.Event;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Sets up the Interactable.Climbable prefab based on the provided user settings.
    /// </summary>
    public class ClimbInteractableInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected ClimbInteractableFacade facade;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="InteractableFacade"/> component acting as the interactable for climbing.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The Interactable Facade component acting as the interactable for climbing."), InternalSetting, SerializeField]
        protected InteractableFacade interactableFacade;
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> component handling a started climb.
        /// </summary>
        [Tooltip("The Game Object Event Proxy Emitter component handling a started climb."), InternalSetting, SerializeField]
        protected GameObjectEventProxyEmitter startEventProxyEmitter;
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> component handling a stopped climb.
        /// </summary>
        [Tooltip("The Game Object Event Proxy Emitter component handling a stopped climb."), InternalSetting, SerializeField]
        protected GameObjectEventProxyEmitter stopEventProxyEmitter;
        #endregion

        protected virtual void OnEnable()
        {
            startEventProxyEmitter.Emitted.AddListener(OnStartEventProxyEmitted);
            stopEventProxyEmitter.Emitted.AddListener(OnStopEventProxyEmitted);
        }

        protected virtual void OnDisable()
        {
            stopEventProxyEmitter.Emitted.RemoveListener(OnStopEventProxyEmitted);
            startEventProxyEmitter.Emitted.RemoveListener(OnStartEventProxyEmitted);
        }

        /// <summary>
        /// Processes the start climbing functionality.
        /// </summary>
        /// <param name="interactor">The interactor initiating the climb.</param>
        protected virtual void OnStartEventProxyEmitted(GameObject interactor)
        {
            facade.ClimbFacade.AddInteractor(interactor);
            facade.ClimbFacade.AddInteractable(interactableFacade.gameObject);
        }

        /// <summary>
        /// Processes the stop climbing functionality.
        /// </summary>
        /// <param name="interactor">The interactor that is no longer climbing.</param>
        protected virtual void OnStopEventProxyEmitted(GameObject interactor)
        {
            facade.ClimbFacade.SetVelocitySource(interactor);
            facade.ClimbFacade.SetVelocityMultiplier(facade.releaseMultiplier);
            facade.ClimbFacade.RemoveInteractable(interactableFacade.gameObject);
            facade.ClimbFacade.RemoveInteractor(interactor);
        }
    }
}