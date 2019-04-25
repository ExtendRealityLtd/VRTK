namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using Zinnia.Data.Operation.Extraction;

    /// <summary>
    /// Extracts a <see cref="GameObject"/> relevant to the extraction method from an <see cref="InteractorFacade"/>.
    /// </summary>
    public class InteractorExtractor : GameObjectExtractor
    {
        /// <summary>
        /// Extracts the <see cref="GameObject"/> the <see cref="InteractorFacade"/> is residing on.
        /// </summary>
        /// <param name="interactor">The Interactor to extract from.</param>
        /// <returns>The residing <see cref="GameObject"/>.</returns>
        public virtual GameObject Extract(InteractorFacade interactor)
        {
            if (interactor == null)
            {
                Result = null;
                return null;
            }

            Result = interactor.gameObject;
            return base.Extract();
        }

        /// <summary>
        /// Extracts the <see cref="GameObject"/> the <see cref="InteractorFacade"/> is residing on.
        /// </summary>
        /// <param name="interactor">The Interactor to extract from.</param>
        public virtual void DoExtract(InteractorFacade interactor)
        {
            Extract(interactor);
        }

        /// <summary>
        /// Extracts the attach point associated with the grabbing functionality of the Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor to extract from.</param>
        /// <returns>The attach point.</returns>
        public virtual GameObject ExtractAttachPoint(InteractorFacade interactor)
        {
            if (interactor == null || interactor.GrabConfiguration == null)
            {
                Result = null;
                return null;
            }

            Result = interactor.GrabConfiguration.AttachPoint;
            return base.Extract();
        }

        /// <summary>
        /// Extracts the attach point associated with the grabbing functionality of the Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor to extract from.</param>
        public virtual void DoExtractAttachPoint(InteractorFacade interactor)
        {
            ExtractAttachPoint(interactor);
        }
    }
}