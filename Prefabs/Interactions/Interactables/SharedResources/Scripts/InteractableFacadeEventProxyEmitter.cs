namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine.Events;
    using System;
    using Zinnia.Event.Proxy;

    /// <summary>
    /// Emits a UnityEvent with an <see cref="InteractableFacade"/> payload whenever the Receive method is called.
    /// </summary>
    public class InteractableFacadeEventProxyEmitter : RestrictableSingleEventProxyEmitter<InteractableFacade, InteractableFacadeEventProxyEmitter.UnityEvent>
    {
        /// <summary>
        /// Defines the event with the specified state.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractableFacade>
        {
        }

        /// <inheritdoc />
        protected override object GetTargetToCheck()
        {
            return Payload.gameObject;
        }
    }
}