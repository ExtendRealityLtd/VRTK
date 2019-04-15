namespace VRTK.Prefabs.Interactions.Interactables.Grab.Provider
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Event.Proxy;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Processes a received grab event and passes it over to the appropriate grab actions.
    /// </summary>
    public abstract class GrabInteractableInteractorProvider : MonoBehaviour
    {
        #region Input Settings
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        [Serialized]
        [field: Header("Input Settings"), DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter InputGrabReceived { get; protected set; }

        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the ungrab action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter InputUngrabReceived { get; protected set; }
        #endregion

        #region Primary Output Settings
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary grab action.
        /// </summary>
        [Serialized]
        [field: Header("Primary Output Settings"), DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputPrimaryGrabAction { get; protected set; }
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary grab setup on secondary action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputPrimaryGrabSetupOnSecondaryAction { get; protected set; }
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary ungrab action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputPrimaryUngrabAction { get; protected set; }
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the primary ungrab reset on secondary action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputPrimaryUngrabResetOnSecondaryAction { get; protected set; }
        #endregion

        #region Secondary Output Settings
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the secondary grab action.
        /// </summary>
        [Serialized]
        [field: Header("Secondary Output Settings"), DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputSecondaryGrabAction { get; protected set; }
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the Secondary ungrab action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputSecondaryUngrabAction { get; protected set; }
        #endregion

        /// <summary>
        /// Gets the available grabbing Interactors from the provider.
        /// </summary>
        /// <returns>A collection of Interactors that are currently grabbing the Interactable.</returns>
        public abstract IReadOnlyList<InteractorFacade> GrabbingInteractors { get; }

        /// <summary>
        /// A reusable collection to hold the returned grabbing interactors.
        /// </summary>
        protected readonly List<InteractorFacade> grabbingInteractors = new List<InteractorFacade>();

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