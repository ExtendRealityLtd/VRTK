namespace VRTK.Prefabs.Interactions.Interactables.Grab.Provider
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Event;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactors;
    using Zinnia.Extension;

    /// <summary>
    /// Processes a received grab event and passes it over to the appropriate grab actions.
    /// </summary>
    public abstract class GrabInteractableInteractorProvider : MonoBehaviour
    {
        #region Input Settings
        [Header("Input Settings"), Tooltip("The input GameObjectEventProxyEmitter for the grab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _inputGrabReceived = null;
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        public GameObjectEventProxyEmitter InputGrabReceived => _inputGrabReceived;

        [Tooltip("The input GameObjectEventProxyEmitter for the ungrab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _inputUngrabReceived = null;
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the ungrab action.
        /// </summary>
        public GameObjectEventProxyEmitter InputUngrabReceived => _inputUngrabReceived;
        #endregion

        #region Primary Output Settings
        [Header("Primary Output Settings"), Tooltip("The output GameObjectEventProxyEmitter for the primary grab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputPrimaryGrabAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary grab action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputPrimaryGrabAction => _outputPrimaryGrabAction;

        [Tooltip("The output GameObjectEventProxyEmitter for the primary grab setup on secondary action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputPrimaryGrabSetupOnSecondaryAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary grab setup on secondary action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputPrimaryGrabSetupOnSecondaryAction => _outputPrimaryGrabSetupOnSecondaryAction;

        [Tooltip("The output GameObjectEventProxyEmitter for the primary ungrab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputPrimaryUngrabAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary ungrab action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputPrimaryUngrabAction => _outputPrimaryUngrabAction;

        [Tooltip("The output GameObjectEventProxyEmitter for the primary ungrab reset on secondary action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputPrimaryUngrabResetOnSecondaryAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary ungrab reset on secondary action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputPrimaryUngrabResetOnSecondaryAction => _outputPrimaryUngrabResetOnSecondaryAction;
        #endregion

        #region Secondary Output Settings
        [Header("Secondary Output Settings"), Tooltip("The output GameObjectEventProxyEmitter for the secondary grab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputSecondaryGrabAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the secondary grab action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputSecondaryGrabAction => _outputSecondaryGrabAction;

        [Tooltip("The output GameObjectEventProxyEmitter for the Secondary ungrab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputSecondaryUngrabAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the Secondary ungrab action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputSecondaryUngrabAction => _outputSecondaryUngrabAction;
        #endregion

        /// <summary>
        /// A reusable collection to hold the returned grabbing interactors.
        /// </summary>
        protected readonly List<InteractorFacade> grabbingInteractors = new List<InteractorFacade>();

        /// <summary>
        /// Gets the available grabbing Interactors from the provider.
        /// </summary>
        /// <returns>A collection of Interactors that are currently grabbing the Interactable.</returns>
        public abstract IReadOnlyList<InteractorFacade> GetGrabbingInteractors();

        /// <summary>
        /// Gets the Grabbing Interactors stored in the given collection.
        /// </summary>
        /// <param name="elements">The collection to retrieve the Grabbing Interactors from.</param>
        /// <returns>A collection of Grabbing Interactors.</returns>
        protected virtual IReadOnlyList<InteractorFacade> GetGrabbingInteractors(IEnumerable<GameObject> elements)
        {
            grabbingInteractors.Clear();

            if (elements == null)
            {
                return grabbingInteractors;
            }

            foreach (GameObject element in elements)
            {
                InteractorFacade interactor = element.TryGetComponent<InteractorFacade>(true, true);
                if (interactor != null)
                {
                    grabbingInteractors.Add(interactor);
                }
            }

            return grabbingInteractors;
        }
    }
}