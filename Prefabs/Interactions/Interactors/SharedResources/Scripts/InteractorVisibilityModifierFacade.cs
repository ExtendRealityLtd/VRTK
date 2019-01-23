namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using Zinnia.Data.Attribute;

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
        [Header("Visibility Events")]
        public UnityEvent Shown = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor becomes hidden.
        /// </summary>
        public UnityEvent Hidden = new UnityEvent();
        #endregion

        #region Internal Settings
        /// <summary>
        /// The emitter that deals with hiding the interactor.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The emitter that deals with hiding the interactor."), InternalSetting, SerializeField]
        protected InteractorFacadeEventProxyEmitter hideEmitter;
        /// <summary>
        /// The emitter that deals with showing the interactor.
        /// </summary>
        [Tooltip("The emitter that deals with showing the interactor."), InternalSetting, SerializeField]
        protected InteractorFacadeEventProxyEmitter showEmitter;
        #endregion

        /// <summary>
        /// Attempts to hide the given <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="interactorFacade">The interactor to hide.</param>
        public virtual void Hide(InteractorFacade interactorFacade)
        {
            if (Emit(hideEmitter, interactorFacade))
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
            if (Emit(showEmitter, interactorFacade))
            {
                Shown?.Invoke(interactorFacade);
            }
        }

        /// <summary>
        /// Attempts to emit the appropriate emitter for the given <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="emitter">The event proxy to emit to.</param>
        /// <param name="interactorFacade">The interactor to emit with.</param>
        /// <returns><see langword="true"/> if the event is emitted.</returns>
        protected virtual bool Emit(InteractorFacadeEventProxyEmitter emitter, InteractorFacade interactorFacade)
        {
            if (!isActiveAndEnabled || emitter == null)
            {
                return false;
            }

            emitter.Receive(interactorFacade);
            return true;
        }
    }
}