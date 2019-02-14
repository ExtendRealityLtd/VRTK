namespace VRTK.Prefabs.Interactions.Interactables.Grab.Receiver
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Rule;
    using Zinnia.Event;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection;
    using Zinnia.Tracking.Collision.Active;
    using VRTK.Prefabs.Interactions.Interactors;

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
        [Header("Interactable Settings"), Tooltip("The mechanism of how to keep the grab action active."), SerializeField]
        private ActiveType _grabType = ActiveType.HoldTillRelease;
        /// <summary>
        /// The mechanism of how to keep the grab action active.
        /// </summary>
        public ActiveType GrabType
        {
            get { return _grabType; }
            set
            {
                _grabType = value;
                ConfigureGrabType();
            }
        }
        #endregion

        #region Grab Consumer Settings
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the grab payload.
        /// </summary>
        [Header("Grab Consumer Settings"), Tooltip("The ActiveCollisionConsumer that listens for the grab payload."), InternalSetting, SerializeField]
        protected ActiveCollisionConsumer grabConsumer;
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the ungrab payload.
        /// </summary>
        [Tooltip("The ActiveCollisionConsumer that listens for the ungrab payload."), InternalSetting, SerializeField]
        protected ActiveCollisionConsumer ungrabConsumer;
        #endregion

        #region Grab Action Settings
        /// <summary>
        /// The <see cref="ListContainsRule"/> used to determine the grab validity.
        /// </summary>
        [Tooltip("The ListContainsRule used to determine the grab validity."), InternalSetting, SerializeField]
        protected ListContainsRule grabValidity;
        #endregion

        #region Active Type Settings
        /// <summary>
        /// The <see cref="GameObject"/> containing the logic for starting HoldTillRelease grabbing.
        /// </summary>
        [Header("Active Type Settings"), Tooltip("The GameObject containing the logic for starting HoldTillRelease grabbing."), InternalSetting, SerializeField]
        protected GameObject startStateGrab;
        /// <summary>
        /// The <see cref="GameObject"/> containing the logic for ending HoldTillRelease grabbing.
        /// </summary>
        [Tooltip("The GameObject containing the logic for ending HoldTillRelease grabbing."), InternalSetting, SerializeField]
        protected GameObject stopStateGrab;
        /// <summary>
        /// The <see cref="GameObject"/> containing the logic for starting and ending Toggle grabbing.
        /// </summary>
        [Tooltip("The GameObject containing the logic for starting and ending Toggle grabbing"), InternalSetting, SerializeField]
        protected GameObject toggleGrab;

        [Tooltip("The GameObjectObservableSet containing the logic for starting and ending Toggle grabbing."), InternalSetting, SerializeField]
        private GameObjectObservableSet _toggleSet = null;
        /// <summary>
        /// The <see cref="GameObjectObservableSet"/> containing the logic for starting and ending Toggle grabbing.
        /// </summary>
        public GameObjectObservableSet ToggleSet => _toggleSet;
        #endregion

        #region Output Settings
        [Header("Output Settings"), Tooltip("The output ActiveCollisionConsumerEventProxyEmitter for the grab action."), InternalSetting, SerializeField]
        private ActiveCollisionConsumerEventProxyEmitter _outputActiveCollisionConsumer = null;
        /// <summary>
        /// The output <see cref="ActiveCollisionConsumerEventProxyEmitter"/> for the grab action.
        /// </summary>
        public ActiveCollisionConsumerEventProxyEmitter OutputActiveCollisionConsumer => _outputActiveCollisionConsumer;

        [Tooltip("The output GameObjectEventProxyEmitter for the grab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputGrabAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the grab action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputGrabAction => _outputGrabAction;

        [Tooltip("The output GameObjectEventProxyEmitter for the ungrab action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputUngrabAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the ungrab action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputUngrabAction => _outputUngrabAction;

        [Tooltip("The output GameObjectEventProxyEmitter for the ungrab on untouch action."), InternalSetting, SerializeField]
        private GameObjectEventProxyEmitter _outputUngrabOnUntouchAction = null;
        /// <summary>
        /// The output <see cref="GameObjectEventProxyEmitter"/> for the ungrab on untouch action.
        /// </summary>
        public GameObjectEventProxyEmitter OutputUngrabOnUntouchAction => _outputUngrabOnUntouchAction;
        #endregion

        /// <summary>
        /// Sets the consumer containers to the current active container.
        /// </summary>
        /// <param name="container">The container for the consumer.</param>
        public virtual void ConfigureConsumerContainers(GameObject container)
        {
            grabConsumer.container = container;
            ungrabConsumer.container = container;
        }

        /// <summary>
        /// Configures the interactor grab validity.
        /// </summary>
        /// <param name="interactors">The interactors to add to the validity list.</param>
        public virtual void ConfigureGrabValidity(List<InteractorFacade> interactors)
        {
            grabValidity.objects.Clear();
            foreach (InteractorFacade interactor in interactors.EmptyIfNull())
            {
                if (interactor.GrabInteractorSetup.AttachPoint != null)
                {
                    grabValidity.objects.Add(interactor.GrabInteractorSetup.AttachPoint);
                }
            }
        }

        /// <summary>
        /// Configures the Grab Type to be used.
        /// </summary>
        public virtual void ConfigureGrabType()
        {
            switch (GrabType)
            {
                case ActiveType.HoldTillRelease:
                    startStateGrab.TrySetActive(true);
                    stopStateGrab.TrySetActive(true);
                    toggleGrab.TrySetActive(false);
                    break;
                case ActiveType.Toggle:
                    startStateGrab.TrySetActive(false);
                    stopStateGrab.TrySetActive(false);
                    toggleGrab.TrySetActive(true);
                    break;
            }
        }

        protected virtual void OnEnable()
        {
            ConfigureGrabType();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ConfigureGrabType();
        }
    }
}