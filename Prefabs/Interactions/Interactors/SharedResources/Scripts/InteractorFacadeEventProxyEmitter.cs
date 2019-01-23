namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine.Events;
    using System;
    using Zinnia.Event;

    /// <summary>
    /// Emits a UnityEvent with an <see cref="InteractorFacade"/> payload whenever <see cref="Receive"/> is called.
    /// </summary>
    public class InteractorFacadeEventProxyEmitter : RestrictableSingleEventProxyEmitter<InteractorFacade, InteractorFacadeEventProxyEmitter.UnityEvent>
    {
        /// <summary>
        /// Defines the event with the specified state.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractorFacade>
        {
        }

        /// <inheritdoc />
        protected override object GetTargetToCheck()
        {
            return Payload.gameObject;
        }
    }
}