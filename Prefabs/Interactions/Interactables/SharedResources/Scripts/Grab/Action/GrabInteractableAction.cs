namespace VRTK.Prefabs.Interactions.Interactables.Grab.Action
{
    using UnityEngine;
    using Zinnia.Event;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Describes an action to perform when a Grab Process is executed.
    /// </summary>
    public class GrabInteractableAction : MonoBehaviour
    {
        #region Input Settings
        [Header("Input Settings"), Tooltip("The input ActiveCollisionConsumerEventProxyEmitter for the grab action."), InternalSetting, SerializeField]
        private ActiveCollisionConsumerEventProxyEmitter _inputActiveCollisionConsumer = null;
        /// <summary>
        /// The input <see cref="ActiveCollisionConsumerEventProxyEmitter"/> for the grab action.
        /// </summary>
        public ActiveCollisionConsumerEventProxyEmitter InputActiveCollisionConsumer => _inputActiveCollisionConsumer;

        [Tooltip("The input GameObjectEventProxyEmitter for the grab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _inputGrabReceived = null;
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        public GameObjectEventProxyEmitter InputGrabReceived => _inputGrabReceived;

        [Tooltip("The input GameObjectEventProxyEmitter for the ungrab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _inputUngrabReceived = null;
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        public GameObjectEventProxyEmitter InputUngrabReceived => _inputUngrabReceived;

        [Tooltip("The input GameObjectEventProxyEmitter for any pre setup on grab."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _inputGrabSetup = null;
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for any pre setup on grab.
        /// </summary>
        public GameObjectEventProxyEmitter InputGrabSetup => _inputGrabSetup;

        [Tooltip("The input GameObjectEventProxyEmitter for any post reset on ungrab."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _inputUngrabReset = null;
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for any post reset on ungrab.
        /// </summary>
        public GameObjectEventProxyEmitter InputUngrabReset => _inputUngrabReset;
        #endregion

        /// <summary>
        /// The internal setup for the grab action.
        /// </summary>
        protected GrabInteractableInternalSetup grabSetup;

        /// <summary>
        /// Sets up the Action with required core data.
        /// </summary>
        /// <param name="grabSetup">The grab internal setup for the action.</param>
        public virtual void SetUp(GrabInteractableInternalSetup grabSetup)
        {
            this.grabSetup = grabSetup;
        }

        /// <summary>
        /// Notifies that the Interactable is being grabbed.
        /// </summary>
        /// <param name="data">The grabbing object.</param>
        public virtual void NotifyGrab(GameObject data)
        {
            grabSetup.NotifyGrab(data);
        }

        /// <summary>
        /// Notifies that the Interactable is no longer being grabbed.
        /// </summary>
        /// <param name="data">The previous grabbing object.</param>
        public virtual void NotifyUngrab(GameObject data)
        {
            grabSetup.NotifyUngrab(data);
        }
    }
}