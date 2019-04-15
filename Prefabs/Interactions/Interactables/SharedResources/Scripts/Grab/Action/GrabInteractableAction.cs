namespace VRTK.Prefabs.Interactions.Interactables.Grab.Action
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Event.Proxy;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Collision.Active.Event.Proxy;

    /// <summary>
    /// Describes an action to perform when a Grab Process is executed.
    /// </summary>
    public class GrabInteractableAction : MonoBehaviour
    {
        #region Input Settings
        /// <summary>
        /// The input <see cref="ActiveCollisionConsumerEventProxyEmitter"/> for the grab action.
        /// </summary>
        [Serialized]
        [field: Header("Input Settings"), DocumentedByXml, Restricted]
        public ActiveCollisionConsumerEventProxyEmitter InputActiveCollisionConsumer { get; protected set; }
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter InputGrabReceived { get; protected set; }
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter InputUngrabReceived { get; protected set; }
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for any pre setup on grab.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter InputGrabSetup { get; protected set; }
        /// <summary>
        /// The input <see cref="GameObjectEventProxyEmitter"/> for any post reset on ungrab.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter InputUngrabReset { get; protected set; }
        #endregion

        /// <summary>
        /// The internal setup for the grab action.
        /// </summary>
        public GrabInteractableConfigurator GrabSetup { get; set; }

        /// <summary>
        /// Notifies that the Interactable is being grabbed.
        /// </summary>
        /// <param name="data">The grabbing object.</param>
        public virtual void NotifyGrab(GameObject data)
        {
            GrabSetup.NotifyGrab(data);
        }

        /// <summary>
        /// Notifies that the Interactable is no longer being grabbed.
        /// </summary>
        /// <param name="data">The previous grabbing object.</param>
        public virtual void NotifyUngrab(GameObject data)
        {
            GrabSetup.NotifyUngrab(data);
        }

        /// <summary>
        /// Called after <see cref="GrabSetup"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(GrabSetup))]
        protected virtual void OnAfterGrabSetupChange() { }
    }
}