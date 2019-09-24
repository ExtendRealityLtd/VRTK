namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Extension;

    /// <summary>
    /// Extracts the <see cref="InteractableFacade"/> if one exists in the hierarchy of the given <see cref="GameObject"/>.
    /// </summary>
    public class InteractableFacadeExtractor : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the specified <see cref="InteractableFacade"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractableFacade>
        {
        }

        /// <summary>
        /// Emitted when the <see cref="InteractableFacade"/> is extracted.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Extracted = new UnityEvent();

        /// <summary>
        /// The extracted <see cref="InteractableFacade"/>.
        /// </summary>
        public InteractableFacade Result { get; protected set; }

        /// <summary>
        /// Extracts the <see cref="InteractableFacade"/>/
        /// </summary>
        /// <returns>The extracted <see cref="InteractableFacade"/>.</returns>
        public virtual InteractableFacade Extract(GameObject container)
        {
            if (container == null)
            {
                Result = null;
                return null;
            }

            Result = container != null ? container.TryGetComponent<InteractableFacade>(true, true) : null;

            if (!isActiveAndEnabled || Result == null)
            {
                Result = null;
                return null;
            }

            Extracted?.Invoke(Result);
            return Result;
        }

        /// <summary>
        /// Extracts the <see cref="InteractableFacade"/>.
        /// </summary>
        public virtual void DoExtract(GameObject container)
        {
            Extract(container);
        }
    }
}