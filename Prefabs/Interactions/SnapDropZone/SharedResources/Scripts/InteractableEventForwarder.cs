namespace VRTK.Prefabs.Interactions.SnapDropZone
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Malimbe.BehaviourStateRequirementMethod;
    using Malimbe.XmlDocumentationAttribute;
    using VRTK.Prefabs.Interactions.Interactables;
    using VRTK.Prefabs.Interactions.Interactors;
    using Zinnia.Extension;

    /// <summary>
    /// Emits events when the same event on an observed <see cref="InteractableFacade"/> is raised.
    /// </summary>
    public class InteractableEventForwarder : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the specified <see cref="GameObject"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<GameObject>
        {
        }

        /// <summary>
        /// Emitted when an added object has no <see cref="InteractableFacade"/> on it.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent NotInteractable = new UnityEvent();
        /// <summary>
        /// Emitted when an <see cref="InteractableFacade"/> is grabbed.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Grabbed = new UnityEvent();
        /// <summary>
        /// Emitted when an <see cref="InteractableFacade"/> is ungrabbed.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Ungrabbed = new UnityEvent();

        /// <summary>
        /// <see cref="Action"/>s that unsubscribe the added event listeners.
        /// </summary>
        protected readonly Dictionary<InteractableFacade, Action> unsubscribeActions = new Dictionary<InteractableFacade, Action>();

        /// <summary>
        /// Starts observing events on the given <see cref="InteractableFacade"/> or immediately raises <see cref="NotInteractable"/>.
        /// </summary>
        /// <param name="gameObject">The object potentially having a <see cref="InteractableFacade"/>.</param>
        [RequiresBehaviourState]
        public virtual void Add(GameObject gameObject)
        {
            InteractableFacade interactableFacade = gameObject.TryGetComponent<InteractableFacade>(true, true);
            if (interactableFacade != null)
            {
                gameObject = interactableFacade.gameObject;
            }

            if (interactableFacade == null || interactableFacade.GrabbingInteractors.Count == 0)
            {
                NotInteractable?.Invoke(gameObject);
                return;
            }

            void OnGrabbed(InteractorFacade _) =>
                Grabbed?.Invoke(gameObject);

            void OnUngrabbed(InteractorFacade _) =>
                Ungrabbed?.Invoke(gameObject);

            interactableFacade.Grabbed.AddListener(OnGrabbed);
            interactableFacade.Ungrabbed.AddListener(OnUngrabbed);

            unsubscribeActions[interactableFacade] = () =>
            {
                interactableFacade.Ungrabbed.RemoveListener(OnUngrabbed);
                interactableFacade.Grabbed.RemoveListener(OnGrabbed);
            };
        }

        /// <summary>
        /// Stops observing events on the given <see cref="GameObject"/>.
        /// </summary>
        /// <param name="gameObject">The object potentially having a <see cref="InteractableFacade"/>.</param>
        [RequiresBehaviourState]
        public virtual void Remove(GameObject gameObject)
        {
            InteractableFacade interactableFacade = gameObject.TryGetComponent<InteractableFacade>(true, true);
            if (interactableFacade == null
                || !unsubscribeActions.TryGetValue(interactableFacade, out Action unsubscribeAction))
            {
                return;
            }

            unsubscribeAction();
        }
    }
}
