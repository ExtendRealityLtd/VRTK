namespace VRTK.Prefabs.Interactions.Interactables.Grab
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Registers listeners to the initial grab and final ungrab states of an <see cref="InteractableFacade"/> and emits the <see cref="InteractableFacade"/> as the event payload.
    /// </summary>
    public class InteractableGrabStateRegistrar : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the specified <see cref="InteractableFacade"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractableFacade>
        {
        }

        /// <summary>
        /// Determines whether to unsubscribe all registered listeners when the component is disabled.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool UnsubscribeOnDisable { get; set; } = true;

        /// <summary>
        /// Emitted when the <see cref="InteractableFacade"/> is grabbed.
        /// </summary>
        public UnityEvent Grabbed = new UnityEvent();
        /// <summary>
        /// Emitted when the <see cref="InteractableFacade"/> is ungrabbed.
        /// </summary>
        public UnityEvent Ungrabbed = new UnityEvent();

        /// <summary>
        /// <see cref="Action"/>s that unsubscribe the added grab event listeners.
        /// </summary>
        protected readonly Dictionary<InteractableFacade, System.Action> unsubscribeGrabActions = new Dictionary<InteractableFacade, System.Action>();
        /// <summary>
        /// <see cref="Action"/>s that unsubscribe the added ungrab event listeners.
        /// </summary>
        protected readonly Dictionary<InteractableFacade, System.Action> unsubscribeUngrabActions = new Dictionary<InteractableFacade, System.Action>();

        /// <summary>
        /// Registers a listener on the given <see cref="InteractableFacade"/> Ungrabbed event.
        /// </summary>
        /// <param name="ungrabbable">The interactable to register the ungrab event on.</param>
        [RequiresBehaviourState]
        public virtual void RegisterUngrabbed(InteractableFacade ungrabbable)
        {
            if (ungrabbable == null || unsubscribeUngrabActions.ContainsKey(ungrabbable))
            {
                return;
            }

            void OnUngrabbed(InteractorFacade _) => InteractableUngrabbed(ungrabbable);
            ungrabbable.LastUngrabbed.AddListener(OnUngrabbed);
            unsubscribeUngrabActions[ungrabbable] = () => ungrabbable.LastUngrabbed.RemoveListener(OnUngrabbed);
        }

        /// <summary>
        /// Registers a listener on the given <see cref="GameObject"/>'s <see cref="InteractableFacade"/> Ungrabbed event.
        /// </summary>
        /// <param name="ungrabbable">The <see cref="GameObject"/> to get the <see cref="InteractableFacade"/> from to register the ungrab event on.</param>
        [RequiresBehaviourState]
        public virtual void RegisterUngrabbed(GameObject ungrabbable)
        {
            if (ungrabbable == null)
            {
                return;
            }

            RegisterUngrabbed(ungrabbable.GetComponent<InteractableFacade>());
        }

        /// <summary>
        /// Registers a listener on the given <see cref="InteractableFacade"/> Grabbed event.
        /// </summary>
        /// <param name="grabbable">The interactable to register the grab event on.</param>
        [RequiresBehaviourState]
        public virtual void RegisterGrabbed(InteractableFacade grabbable)
        {
            if (grabbable == null || unsubscribeGrabActions.ContainsKey(grabbable))
            {
                return;
            }

            void OnGrabbed(InteractorFacade _) => InteractableGrabbed(grabbable);
            grabbable.FirstGrabbed.AddListener(OnGrabbed);
            unsubscribeGrabActions[grabbable] = () => grabbable.FirstGrabbed.RemoveListener(OnGrabbed);
        }

        /// <summary>
        /// Registers a listener on the given <see cref="GameObject"/>'s <see cref="InteractableFacade"/> Grabbed event.
        /// </summary>
        /// <param name="grabbable">The <see cref="GameObject"/> to get the <see cref="InteractableFacade"/> from to register the grab event on.</param>
        [RequiresBehaviourState]
        public virtual void RegisterGrabbed(GameObject grabbable)
        {
            if (grabbable == null)
            {
                return;
            }

            RegisterGrabbed(grabbable.GetComponent<InteractableFacade>());
        }

        /// <summary>
        /// Unregisters a listener from the given <see cref="InteractableFacade"/> Ungrabbed event.
        /// </summary>
        /// <param name="ungrabbable">The interactable to unregister the ungrab event from.</param>
        public virtual void UnregisterUngrabbed(InteractableFacade ungrabbable)
        {
            if (ungrabbable == null || !unsubscribeUngrabActions.TryGetValue(ungrabbable, out System.Action unsubscribeAction))
            {
                return;
            }

            unsubscribeAction();
            unsubscribeUngrabActions.Remove(ungrabbable);
        }

        /// <summary>
        /// Unregisters a listener from the given <see cref="GameObject"/>'s <see cref="InteractableFacade"/> Ungrabbed event.
        /// </summary>
        /// <param name="ungrabbable">The <see cref="GameObject"/> to get the <see cref="InteractableFacade"/> from to unregister the ungrab event from.</param>
        public virtual void UnregisterUngrabbed(GameObject ungrabbable)
        {
            if (ungrabbable == null)
            {
                return;
            }

            UnregisterUngrabbed(ungrabbable.GetComponent<InteractableFacade>());
        }

        /// <summary>
        /// Unregisters a listener from the given <see cref="InteractableFacade"/> Grabbed event.
        /// </summary>
        /// <param name="grabbable">The interactable to unregister the grab event from.</param>
        public virtual void UnregisterGrabbed(InteractableFacade grabbable)
        {
            if (grabbable == null || !unsubscribeGrabActions.TryGetValue(grabbable, out System.Action unsubscribeAction))
            {
                return;
            }

            unsubscribeAction();
            unsubscribeGrabActions.Remove(grabbable);
        }

        /// <summary>
        /// Unregisters a listener from the given <see cref="GameObject"/>'s <see cref="InteractableFacade"/> Grabbed event.
        /// </summary>
        /// <param name="grabbable">The <see cref="GameObject"/> to get the <see cref="InteractableFacade"/> from to unregister the grab event from.</param>
        public virtual void UnregisterGrabbed(GameObject grabbable)
        {
            if (grabbable == null)
            {
                return;
            }

            UnregisterGrabbed(grabbable.GetComponent<InteractableFacade>());
        }

        /// <summary>
        /// Unregisters all listeners for all ungrabbed events.
        /// </summary>
        public virtual void UnregisterAllUngrabbed()
        {
            foreach (InteractableFacade entry in unsubscribeUngrabActions.Keys.ToArray())
            {
                UnregisterUngrabbed(entry);
            }
            unsubscribeUngrabActions.Clear();
        }

        /// <summary>
        /// Unregisters all listeners for all grabbed events.
        /// </summary>
        public virtual void UnregisterAllGrabbed()
        {
            foreach (InteractableFacade entry in unsubscribeGrabActions.Keys.ToArray())
            {
                UnregisterGrabbed(entry);
            }
            unsubscribeGrabActions.Clear();
        }

        /// <summary>
        /// Unregisters all listeners for all events.
        /// </summary>
        public virtual void UnregisterAll()
        {
            UnregisterAllUngrabbed();
            UnregisterAllGrabbed();
        }

        protected virtual void OnDisable()
        {
            if (UnsubscribeOnDisable)
            {
                UnregisterAll();
            }
        }

        /// <summary>
        /// Processes the grabbed event on the given Interactable.
        /// </summary>
        /// <param name="interactable">The Interactable to process the grab event for.</param>
        protected virtual void InteractableGrabbed(InteractableFacade interactable)
        {
            Grabbed?.Invoke(interactable);
        }

        /// <summary>
        /// Processes the ungrabbed event on the given Interactable.
        /// </summary>
        /// <param name="interactable">The Interactable to process the ungrab event for.</param>
        protected virtual void InteractableUngrabbed(InteractableFacade interactable)
        {
            Ungrabbed?.Invoke(interactable);
        }
    }
}