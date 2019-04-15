namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerRigidbody"/>.
    /// </summary>
    public class InteractableConsumerRigidbodyExtractor : GameObjectExtractor
    {
        /// <summary>
        /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerRigidbody"/>.
        /// </summary>
        /// <param name="interactable">The Interactable to extract from.</param>
        /// <returns>The related <see cref="GameObject"/>.</returns>
        public virtual GameObject Extract(InteractableFacade interactable)
        {
            if (interactable == null)
            {
                Result = null;
                return null;
            }

            Result = interactable.ConsumerRigidbody != null ? interactable.ConsumerRigidbody.gameObject : null;
            return base.Extract();
        }

        /// <summary>
        /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerRigidbody"/>.
        /// </summary>
        /// <param name="interactable">The Interactable to extract from.</param>
        public virtual void DoExtract(InteractableFacade interactable)
        {
            Extract(interactable);
        }
    }
}