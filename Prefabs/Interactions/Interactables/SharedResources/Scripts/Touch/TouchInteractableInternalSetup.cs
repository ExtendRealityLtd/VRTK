namespace VRTK.Prefabs.Interactions.Interactables.Touch
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Rule;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection;
    using Zinnia.Tracking.Collision;
    using Zinnia.Tracking.Collision.Active;
    using Zinnia.Tracking.Collision.Active.Operation;
    using VRTK.Prefabs.Interactions.Interactors;

    public class TouchInteractableInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        [Header("Facade Settings"), Tooltip("The public interface facade."), SerializeField]
        private InteractableFacade _facade = null;
        /// <summary>
        /// The public interface facade.
        /// </summary>
        public InteractableFacade Facade => _facade;
        #endregion

        #region Touch Consumer Settings
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the touch payload.
        /// </summary>
        [Header("Touch Consumer Settings"), Tooltip("The ActiveCollisionConsumer that listens for the touch payload."), InternalSetting, SerializeField]
        protected ActiveCollisionConsumer touchConsumer;
        /// <summary>
        /// The <see cref="ActiveCollisionConsumer"/> that listens for the untouch payload.
        /// </summary>
        [Tooltip("The ActiveCollisionConsumer that listens for the untouch payload."), InternalSetting, SerializeField]
        protected ActiveCollisionConsumer untouchConsumer;
        #endregion

        #region Touch Settings
        /// <summary>
        /// The <see cref="GameObjectObservableSet"/> that holds the current touching objects data.
        /// </summary>
        [Header("Touch Settings"), Tooltip("The GameObjectSet that holds the current touching objects data."), InternalSetting, SerializeField]
        protected GameObjectObservableSet currentTouchingObjects;
        /// <summary>
        /// The <see cref="ListContainsRule"/> used to determine the touch validity.
        /// </summary>
        [Tooltip("The ListContainsRule used to determine the touch validity."), InternalSetting, SerializeField]
        protected ListContainsRule touchValidity;
        #endregion

        #region Interactor Settings
        /// <summary>
        /// The <see cref="ActiveCollisionsContainer"/> for potential interactors.
        /// </summary>
        [Header("Interactor Settings"), Tooltip("The ActiveCollisionsContainer for potential interactors."), InternalSetting, SerializeField]
        protected ActiveCollisionsContainer potentialInteractors;
        /// <summary>
        /// The <see cref="NotifierContainerExtractor"/> for adding active interactors.
        /// </summary>
        [Tooltip("The NotifierContainerExtractor for adding active interactors."), InternalSetting, SerializeField]
        protected NotifierContainerExtractor addActiveInteractor;
        /// <summary>
        /// The <see cref="NotifierContainerExtractor"/> for removing active interactors.
        /// </summary>
        [Tooltip("The NotifierContainerExtractor for removing active interactors."), InternalSetting, SerializeField]
        protected NotifierContainerExtractor removeActiveInteractor;
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
                interactor.Touched?.Invoke(Facade);
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
                interactor.Untouched?.Invoke(Facade);
                if (Facade.TouchingInteractors.Count == 0)
                {
                    Facade.LastUntouched?.Invoke(interactor);
                }
            }
        }

        /// <summary>
        /// Configures the interactor touch validity.
        /// </summary>
        /// <param name="interactors">The interactors to add to the validity list.</param>
        public virtual void ConfigureTouchValidity(List<InteractorFacade> interactors)
        {
            touchValidity.objects.Clear();
            foreach (InteractorFacade interactor in interactors)
            {
                touchValidity.objects.Add(interactor.gameObject);
            }
        }

        /// <summary>
        /// Sets the consumer containers to the current active container.
        /// </summary>
        public virtual void ConfigureContainer()
        {
            touchConsumer.container = Facade.ConsumerContainer;
            untouchConsumer.container = Facade.ConsumerContainer;
        }

        /// <summary>
        /// Retrieves a collection of Interactors that are touching the Interactable.
        /// </summary>
        /// <returns>The touching Interactors.</returns>
        protected virtual IReadOnlyList<InteractorFacade> GetTouchingInteractors()
        {
            touchingInteractors.Clear();

            if (currentTouchingObjects == null)
            {
                return touchingInteractors;
            }

            foreach (GameObject element in currentTouchingObjects.Elements)
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
            ConfigureTouchValidity(Facade.disallowedTouchInteractors);
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
            Facade.CollisionNotifier.CollisionStarted.AddListener(potentialInteractors.Add);
            Facade.CollisionNotifier.CollisionStarted.AddListener(addActiveInteractor.DoExtract);
            Facade.CollisionNotifier.CollisionChanged.AddListener(ProcessPotentialInteractorContentChange);
            Facade.CollisionNotifier.CollisionStopped.AddListener(potentialInteractors.Remove);
            Facade.CollisionNotifier.CollisionStopped.AddListener(removeActiveInteractor.DoExtract);
        }

        /// <summary>
        /// Unlinks the <see cref="CollisionNotifier"/> to the potential and active interactor logic.
        /// </summary>
        protected virtual void UnlinkActiveInteractorCollisions()
        {
            Facade.CollisionNotifier.CollisionStarted.RemoveListener(potentialInteractors.Add);
            Facade.CollisionNotifier.CollisionStarted.RemoveListener(addActiveInteractor.DoExtract);
            Facade.CollisionNotifier.CollisionChanged.RemoveListener(ProcessPotentialInteractorContentChange);
            Facade.CollisionNotifier.CollisionStopped.RemoveListener(potentialInteractors.Remove);
            Facade.CollisionNotifier.CollisionStopped.RemoveListener(removeActiveInteractor.DoExtract);
        }

        /// <summary>
        /// Handles any change of collision contents.
        /// </summary>
        /// <param name="data">The changed collision data.</param>
        protected virtual void ProcessPotentialInteractorContentChange(CollisionNotifier.EventData data)
        {
            potentialInteractors.ProcessContentsChanged();
        }
    }
}