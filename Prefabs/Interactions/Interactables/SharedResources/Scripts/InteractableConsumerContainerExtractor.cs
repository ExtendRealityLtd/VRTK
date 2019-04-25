namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerContainer"/>.
    /// </summary>
    public class InteractableConsumerContainerExtractor : GameObjectExtractor
    {
        /// <summary>
        /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerContainer"/>.
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

            Result = interactable.ConsumerContainer;
            return base.Extract();
        }

        /// <summary>
        /// Extracts the <see cref="GameObject"/> of the <see cref="InteractableFacade.ConsumerContainer"/>.
        /// </summary>
        /// <param name="interactable">The Interactable to extract from.</param>
        public virtual void DoExtract(InteractableFacade interactable)
        {
            Extract(interactable);
        }
    }
}