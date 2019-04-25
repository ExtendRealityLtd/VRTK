namespace VRTK.Prefabs.Interactions.Interactables.Grab
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Malimbe.BehaviourStateRequirementMethod;

    /// <summary>
    /// Emits an appropriate event based on the state of whether an <see cref="InteractableFacade"/> is currently being grabbed or not.
    /// </summary>
    public class InteractableGrabStateEmitter : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the specified <see cref="InteractableFacade"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractableFacade>
        {
        }

        /// <summary>
        /// Emitted if the <see cref="InteractableFacade"/> is grabbed.
        /// </summary>
        public UnityEvent Grabbed = new UnityEvent();
        /// <summary>
        /// Emitted if the <see cref="InteractableFacade"/> is not grabbed.
        /// </summary>
        public UnityEvent NotGrabbed = new UnityEvent();

        /// <summary>
        /// Determines if the given Interactable is currently grabbed by a valid Interactor.
        /// </summary>
        /// <param name="interactable">The Interactable to check.</param>
        /// <returns>Whether the Interactable is being grabbed.</returns>
        [RequiresBehaviourState]
        public virtual bool IsGrabbed(InteractableFacade interactable)
        {
            if (interactable == null)
            {
                return false;
            }

            if (interactable.IsGrabbed)
            {
                Grabbed?.Invoke(interactable);
                return true;
            }
            else
            {
                NotGrabbed?.Invoke(interactable);
                return false;
            }
        }

        /// <summary>
        /// Determines if the given Interactable is currently grabbed by a valid Interactor.
        /// </summary>
        /// <param name="interactable">The Interactable to check.</param>
        public virtual void DoIsGrabbed(InteractableFacade interactable)
        {
            IsGrabbed(interactable);
        }

        /// <summary>
        /// Determines if the given <see cref="GameObject"/>'s <see cref="InteractableFacade"/> is currently grabbed by a valid Interactor.
        /// </summary>
        /// <param name="interactable">The <see cref="GameObject"/> to check.</param>
        /// <returns>Whether the Interactable is being grabbed.</returns>
        public virtual bool IsGrabbed(GameObject interactable)
        {
            if (interactable == null)
            {
                return false;
            }

            return IsGrabbed(interactable.GetComponent<InteractableFacade>());
        }

        /// <summary>
        /// Determines if the given <see cref="GameObject"/>'s <see cref="InteractableFacade"/> is currently grabbed by a valid Interactor.
        /// </summary>
        /// <param name="interactable">The <see cref="GameObject"/> to check.</param>
        public virtual void DoIsGrabbed(GameObject interactable)
        {
            IsGrabbed(interactable);
        }
    }
}