namespace VRTK.Prefabs.Interactions.InteractorVisibilityModifier
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Provides a mechanism for showing and hiding Interactors.
    /// </summary>
    public class InteractorVisibilityModifierFacade : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the <see cref="InteractorFacade"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractorFacade>
        {
        }

        #region Visibility Events
        /// <summary>
        /// Emitted when the Interactor becomes shown.
        /// </summary>
        [Header("Visibility Events"), DocumentedByXml]
        public UnityEvent Shown = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor becomes hidden.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Hidden = new UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The emitter that deals with hiding the interactor.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public InteractorFacadeEventProxyEmitter HideEmitter { get; protected set; }
        /// <summary>
        /// The emitter that deals with showing the interactor.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public InteractorFacadeEventProxyEmitter ShowEmitter { get; protected set; }
        #endregion

        /// <summary>
        /// Attempts to hide the given <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="interactorFacade">The interactor to hide.</param>
        public virtual void Hide(InteractorFacade interactorFacade)
        {
            if (Emit(HideEmitter, interactorFacade))
            {
                Hidden?.Invoke(interactorFacade);
            }
        }

        /// <summary>
        /// Attempts to show the given <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="interactorFacade">The interactor to show.</param>
        public virtual void Show(InteractorFacade interactorFacade)
        {
            if (Emit(ShowEmitter, interactorFacade))
            {
                Shown?.Invoke(interactorFacade);
            }
        }

        /// <summary>
        /// Attempts to emit the appropriate emitter for the given <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="emitter">The event proxy to emit to.</param>
        /// <param name="interactorFacade">The interactor to emit with.</param>
        /// <returns>Whether the event is emitted.</returns>
        [RequiresBehaviourState]
        protected virtual bool Emit(InteractorFacadeEventProxyEmitter emitter, InteractorFacade interactorFacade)
        {
            if (emitter == null)
            {
                return false;
            }

            emitter.Receive(interactorFacade);
            return true;
        }
    }
}