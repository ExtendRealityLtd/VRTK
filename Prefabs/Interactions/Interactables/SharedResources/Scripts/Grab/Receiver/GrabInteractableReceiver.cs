namespace VRTK.Prefabs.Interactions.Interactables.Grab.Receiver
{
    using UnityEngine;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Event.Proxy;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using Zinnia.Tracking.Collision.Active;
    using Zinnia.Tracking.Collision.Active.Event.Proxy;

    /// <summary>
    /// Handles the way in which a grab event from an Interactor is received and processed by the Interactable.
    /// </summary>
    public class GrabInteractableReceiver : MonoBehaviour
    {
        /// <summary>
        /// The way in which the grab is kept active.
        /// </summary>
        public enum ActiveType
        {
            /// <summary>
            /// The grab will occur when the button is held down and will ungrab when the button is released.
            /// </summary>
            HoldTillRelease,
            /// <summary>
            /// The grab will occur on the first press of the button and stay grabbed until a second press of the button.
            /// </summary>
            Toggle
        }

        #region Interactable Settings
        /// <summary>
        /// The mechanism of how to keep the grab action active.
        /// </summary>
        [Serialized]
        [field: Header("Interactable Settings"), DocumentedByXml]
        public ActiveType GrabType { get; set; } = ActiveType.HoldTillRelease;
        #endregion

        #region Grab Consumer Settings
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the grab payload.
        /// </summary>
        [Serialized]
        [field: Header("Grab Consumer Settings"), DocumentedByXml, Restricted]
        public ActiveCollisionConsumer GrabConsumer { get; protected set; }
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the ungrab payload.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ActiveCollisionConsumer UngrabConsumer { get; protected set; }
        #endregion

        #region Grab Action Settings
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> used to determine the grab validity.
        /// </summary>
        [Serialized]
        [field: Header("Grab Action Settings"), DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter GrabValidity { get; set; }
        #endregion

        #region Active Type Settings
        /// <summary>
        /// The <see cref="GameObject"/> containing the logic for starting HoldTillRelease grabbing.
        /// </summary>
        [Serialized]
        [field: Header("Active Type Settings"), DocumentedByXml, Restricted]
        public GameObject StartStateGrab { get; protected set; }
        /// <summary>
        /// The <see cref="GameObject"/> containing the logic for ending HoldTillRelease grabbing.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject StopStateGrab { get; protected set; }
        /// <summary>
        /// The <see cref="GameObject"/> containing the logic for starting and ending Toggle grabbing.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject ToggleGrab { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectObservableSet"/> containing the logic for starting and ending Toggle grabbing.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectObservableList ToggleList { get; protected set; }
        #endregion

        #region Output Settings
        /// <summary>
        /// The output <see cref="ActiveCollisionConsumerEventProxyEmitter"/> for the grab action.
        /// </summary>
        [Serialized]
        [field: Header("Output Settings"), DocumentedByXml, Restricted]
        public ActiveCollisionConsumerEventProxyEmitter OutputActiveCollisionConsumer { get; protected set; }
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputGrabAction { get; protected set; }
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the ungrab action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputUngrabAction { get; protected set; }
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the ungrab on untouch action.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter OutputUngrabOnUntouchAction { get; protected set; }
        #endregion

        /// <summary>
        /// Sets the consumer containers to the current active container.
        /// </summary>
        /// <param name="container">The container for the consumer.</param>
        public virtual void ConfigureConsumerContainers(GameObject container)
        {
            GrabConsumer.Container = container;
            UngrabConsumer.Container = container;
        }

        /// <summary>
        /// Configures the Grab Type to be used.
        /// </summary>
        public virtual void ConfigureGrabType()
        {
            switch (GrabType)
            {
                case ActiveType.HoldTillRelease:
                    StartStateGrab.TrySetActive(true);
                    StopStateGrab.TrySetActive(true);
                    ToggleGrab.TrySetActive(false);
                    break;
                case ActiveType.Toggle:
                    StartStateGrab.TrySetActive(false);
                    StopStateGrab.TrySetActive(false);
                    ToggleGrab.TrySetActive(true);
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            ConfigureGrabType();
        }

        /// <summary>
        /// Called after <see cref="GrabType"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(GrabType))]
        protected virtual void OnAfterGrabTypeChange()
        {
            ConfigureGrabType();
        }
    }
}