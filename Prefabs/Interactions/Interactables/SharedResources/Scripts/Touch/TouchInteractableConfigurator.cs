namespace VRTK.Prefabs.Interactions.Interactables.Touch
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Extension;
    using Zinnia.Event.Proxy;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using Zinnia.Tracking.Collision;
    using Zinnia.Tracking.Collision.Active;
    using Zinnia.Tracking.Collision.Active.Operation.Extraction;
    using VRTK.Prefabs.Interactions.Interactors;

    public class TouchInteractableConfigurator : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public InteractableFacade Facade { get; protected set; }
        #endregion

        #region Touch Consumer Settings
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the touch payload.
        /// </summary>
        [Serialized]
        [field: Header("Touch Consumer Settings"), DocumentedByXml, Restricted]
        public ActiveCollisionConsumer TouchConsumer { get; protected set; }
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the untouch payload.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ActiveCollisionConsumer UntouchConsumer { get; protected set; }
        #endregion

        #region Touch Settings
        /// <summary>
        /// The <see cref="GameObjectObservableList"/> that holds the current touching objects data.
        /// </summary>
        [Serialized]
        [field: Header("Touch Settings"), DocumentedByXml, Restricted]
        public GameObjectObservableList CurrentTouchingObjects { get; protected set; }
        /// <summary>
        /// The <see cref="GameObjectEventProxyEmitter"/> used to determine the touch validity.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectEventProxyEmitter TouchValidity { get; set; }
        #endregion

        #region Interactor Settings
        /// <summary>
        /// The <see cref="ActiveCollisionsContainer"/> for potential interactors.
        /// </summary>
        [Serialized]
        [field: Header("Interactor Settings"), DocumentedByXml, Restricted]
        public ActiveCollisionsContainer PotentialInteractors { get; protected set; }
        /// <summary>
        /// The <see cref="NotifierContainerExtractor"/> for adding active interactors.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public NotifierContainerExtractor AddActiveInteractor { get; protected set; }
        /// <summary>
        /// The <see cref="NotifierContainerExtractor"/> for removing active interactors.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public NotifierContainerExtractor RemoveActiveInteractor { get; protected set; }
        #endregion

        /// <summary>
        /// A collection of Interactors that are currently touching the Interactable.
        /// </summary>
        public IReadOnlyList<InteractorFacade> TouchingInteractors => GetTouchingInteractors();

        /// <summary>
        /// A reusable collection to hold the returned touching interactors.
        /// </summary>
        protected readonly List<InteractorFacade> touchingInteractors = new List<InteractorFacade>();

        /// <summary>
        /// Notifies that the Interactable is being touched.
        /// </summary>
        /// <param name="data">The touching object.</param>
        public virtual void NotifyTouch(GameObject data)
        {
            InteractorFacade interactor = data.TryGetComponent<InteractorFacade>(true, true);
            if (interactor != null)
            {
                if (Facade.TouchingInteractors.Count == 1)
                {
                    Facade.FirstTouched?.Invoke(interactor);
                }
                Facade.Touched?.Invoke(interactor);
                interactor.NotifyOfTouch(Facade);
            }
        }

        /// <summary>
        /// Notifies that the Interactable is being no longer touched.
        /// </summary>
        /// <param name="data">The previous touching object.</param>
        public virtual void NotifyUntouch(GameObject data)
        {
            InteractorFacade interactor = data.TryGetComponent<InteractorFacade>(true, true);
            if (interactor != null)
            {
                Facade.Untouched?.Invoke(interactor);
                interactor.NotifyOfUntouch(Facade);
                if (Facade.TouchingInteractors.Count == 0)
                {
                    Facade.LastUntouched?.Invoke(interactor);
                }
            }
        }

        /// <summary>
        /// Sets the consumer containers to the current active container.
        /// </summary>
        public virtual void ConfigureContainer()
        {
            TouchConsumer.Container = Facade.ConsumerContainer;
            UntouchConsumer.Container = Facade.ConsumerContainer;
        }

        /// <summary>
        /// Retrieves a collection of Interactors that are touching the Interactable.
        /// </summary>
        /// <returns>The touching Interactors.</returns>
        protected virtual IReadOnlyList<InteractorFacade> GetTouchingInteractors()
        {
            touchingInteractors.Clear();

            if (CurrentTouchingObjects == null)
            {
                return touchingInteractors;
            }

            foreach (GameObject element in CurrentTouchingObjects.NonSubscribableElements)
            {
                InteractorFacade interactor = element.TryGetComponent<InteractorFacade>(true, true);
                if (interactor != null)
                {
                    touchingInteractors.Add(interactor);
                }
            }

            return touchingInteractors;
        }

        protected virtual void OnEnable()
        {
            ConfigureContainer();
            TouchValidity.ReceiveValidity = Facade.DisallowedTouchInteractors;
            LinkActiveInteractorCollisions();
        }

        protected virtual void OnDisable()
        {
            UnlinkActiveInteractorCollisions();
        }

        /// <summary>
        /// Links the <see cref="CollisionNotifier"/> to the potential and active interactor logic.
        /// </summary>
        protected virtual void LinkActiveInteractorCollisions()
        {
            Facade.CollisionNotifier.CollisionStarted.AddListener(PotentialInteractors.Add);
            Facade.CollisionNotifier.CollisionStarted.AddListener(AddActiveInteractor.DoExtract);
            Facade.CollisionNotifier.CollisionChanged.AddListener(ProcessPotentialInteractorContentChange);
            Facade.CollisionNotifier.CollisionStopped.AddListener(PotentialInteractors.Remove);
            Facade.CollisionNotifier.CollisionStopped.AddListener(RemoveActiveInteractor.DoExtract);
        }

        /// <summary>
        /// Unlinks the <see cref="CollisionNotifier"/> to the potential and active interactor logic.
        /// </summary>
        protected virtual void UnlinkActiveInteractorCollisions()
        {
            Facade.CollisionNotifier.CollisionStarted.RemoveListener(PotentialInteractors.Add);
            Facade.CollisionNotifier.CollisionStarted.RemoveListener(AddActiveInteractor.DoExtract);
            Facade.CollisionNotifier.CollisionChanged.RemoveListener(ProcessPotentialInteractorContentChange);
            Facade.CollisionNotifier.CollisionStopped.RemoveListener(PotentialInteractors.Remove);
            Facade.CollisionNotifier.CollisionStopped.RemoveListener(RemoveActiveInteractor.DoExtract);
        }

        /// <summary>
        /// Handles any change of collision contents.
        /// </summary>
        /// <param name="data">The changed collision data.</param>
        protected virtual void ProcessPotentialInteractorContentChange(CollisionNotifier.EventData data)
        {
            PotentialInteractors.ProcessContentsChanged();
        }
    }
}