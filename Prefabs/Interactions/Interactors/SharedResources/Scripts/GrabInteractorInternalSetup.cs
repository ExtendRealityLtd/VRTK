namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Action;
    using Zinnia.Utility;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection;
    using Zinnia.Tracking.Velocity;
    using Zinnia.Tracking.Collision.Active;
    using VRTK.Prefabs.Interactions.Interactables;

    /// <summary>
    /// Sets up the Interactor Prefab grab settings based on the provided user settings.
    /// </summary>
    public class GrabInteractorInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected InteractorFacade facade;
        #endregion

        #region Reference Settings
        [Header("Reference Settings"), Tooltip("The point in which to attach a grabbed Interactable to the Interactor."), InternalSetting, SerializeField]
        private GameObject _attachPoint;
        /// <summary>
        /// The point in which to attach a grabbed Interactable to the Interactor.
        /// </summary>
        public GameObject AttachPoint
        {
            get { return _attachPoint; }
            set
            {
                _attachPoint = value;
                ConfigurePublishers();
            }
        }

        [Tooltip("The VelocityTrackerProcessor to measure the interactors current velocity for throwing on release."), InternalSetting, SerializeField]
        private VelocityTrackerProcessor _velocityTracker;
        /// <summary>
        /// The <see cref="VelocityTrackerProcessor"/> to measure the interactors current velocity for throwing on release.
        /// </summary>
        public VelocityTrackerProcessor VelocityTracker
        {
            get { return _velocityTracker; }
            set
            {
                _velocityTracker = value;
                ConfigureVelocityTrackers();
            }
        }
        #endregion

        #region Grab Settings
        /// <summary>
        /// The <see cref="BooleanAction"/> that will initiate the Interactor grab mechanism.
        /// </summary>
        [Header("Grab Settings"), Tooltip("The BooleanAction that will initiate the Interactor grab mechanism."), InternalSetting, SerializeField]
        protected BooleanAction grabAction;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid start grabbing action.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid start grabbing action."), InternalSetting, SerializeField]
        protected ActiveCollisionPublisher startGrabbingPublisher;
        /// <summary>
        /// The <see cref="ActiveCollisionPublisher"/> for checking valid stop grabbing action.
        /// </summary>
        [Tooltip("The ActiveCollisionPublisher for checking valid stop grabbing action."), InternalSetting, SerializeField]
        protected ActiveCollisionPublisher stopGrabbingPublisher;
        /// <summary>
        /// The processor for initiating an instant grab.
        /// </summary>
        [Tooltip("The processor for initiating an instant grab."), InternalSetting, SerializeField]
        protected GameObject instantGrabProcessor;
        /// <summary>
        /// The processor for initiating a precognitive grab.
        /// </summary>
        [Tooltip("The processor for initiating a precognitive grab."), InternalSetting, SerializeField]
        protected GameObject precognitionGrabProcessor;
        /// <summary>
        /// The <see cref="CountdownTimer"/> to determine grab precognition.
        /// </summary>
        [Tooltip("The CountdownTimer to determine grab precognition."), InternalSetting, SerializeField]
        protected CountdownTimer precognitionTimer;
        /// <summary>
        /// The minimum timer value for the grab precognition <see cref="CountdownTimer"/>.
        /// </summary>
        [Tooltip("The minimum timer value for the grab precognition CountdownTimer."), InternalSetting, SerializeField]
        protected float minPrecognitionTimer = 0.01f;
        /// <summary>
        /// The <see cref="GameObjectObservableSet"/> containing the currently grabbed objects.
        /// </summary>
        [Tooltip("The GameObjectSet containing the currently grabbed objects."), InternalSetting, SerializeField]
        public GameObjectObservableSet grabbedObjectsCollection;
        #endregion

        /// <summary>
        /// A collection of currently grabbed GameObjects.
        /// </summary>
        public IReadOnlyList<GameObject> GrabbedObjects => GetGrabbedObjects();

        /// <summary>
        /// A reusable collection to hold the returned grabbed objects.
        /// </summary>
        protected readonly List<GameObject> grabbedObjects = new List<GameObject>();
        /// <summary>
        /// A reusable instance of event data.
        /// </summary>
        protected readonly ActiveCollisionsContainer.EventData activeCollisionsEventData = new ActiveCollisionsContainer.EventData();

        /// <summary>
        /// Configures the action used to control grabbing.
        /// </summary>
        public virtual void ConfigureGrabAction()
        {
            if (grabAction != null && facade != null && facade.GrabAction != null)
            {
                grabAction.ClearSources();
                grabAction.AddSource(facade.GrabAction);
            }
        }

        /// <summary>
        /// Configures the velocity tracker used for grabbing.
        /// </summary>
        public virtual void ConfigureVelocityTrackers()
        {
            if (VelocityTracker != null && facade != null && facade.VelocityTracker != null)
            {
                VelocityTracker.velocityTrackers.Clear();
                VelocityTracker.velocityTrackers.Add(facade.VelocityTracker);
            }
        }

        /// <summary>
        /// Configures the <see cref="ActiveCollisionPublisher"/> components for touching and untouching.
        /// </summary>
        public virtual void ConfigurePublishers()
        {
            if (startGrabbingPublisher != null)
            {
                startGrabbingPublisher.payload.sourceContainer = AttachPoint;
            }

            if (stopGrabbingPublisher != null)
            {
                stopGrabbingPublisher.payload.sourceContainer = AttachPoint;
            }
        }

        /// <summary>
        /// Configures the <see cref="CountdownTimer"/> components for grab precognition.
        /// </summary>
        public virtual void ConfigureGrabPrecognition()
        {
            if (facade.GrabPrecognition < minPrecognitionTimer && !facade.GrabPrecognition.ApproxEquals(0f))
            {
                facade.GrabPrecognition = minPrecognitionTimer;
            }
            precognitionTimer.startTime = facade.GrabPrecognition;
            ChooseGrabProcessor();
        }

        /// <summary>
        /// Attempt to grab an Interactable to the current Interactor utilizing custom collision data.
        /// </summary>
        /// <param name="interactable">The Interactable to attempt to grab.</param>
        /// <param name="collision">Custom collision data.</param>
        /// <param name="collider">Custom collider data.</param>
        public virtual void Grab(InteractableFacade interactable, Collision collision, Collider collider)
        {
            if (interactable == null)
            {
                return;
            }

            Ungrab();
            startGrabbingPublisher.SetActiveCollisions(CreateActiveCollisionsEventData(interactable.gameObject, collision, collider));
            ProcessGrabAction(startGrabbingPublisher, true);
            if (interactable.IsGrabTypeToggle())
            {
                ProcessGrabAction(startGrabbingPublisher, false);
            }
        }

        /// <summary>
        /// Attempt to ungrab currently grabbed Interactables to the current Interactor.
        /// </summary>
        public virtual void Ungrab()
        {
            if (GrabbedObjects.Count == 0)
            {
                return;
            }

            InteractableFacade interactable = GrabbedObjects[0].TryGetComponent<InteractableFacade>(true, true);
            if (interactable.IsGrabTypeToggle())
            {
                ProcessGrabAction(startGrabbingPublisher, true);
            }
            ProcessGrabAction(stopGrabbingPublisher, false);
        }

        protected virtual void ProcessGrabAction(ActiveCollisionPublisher publisher, bool actionState)
        {
            instantGrabProcessor.SetActive(false);
            precognitionGrabProcessor.SetActive(false);
            if (grabAction.Value != actionState)
            {
                grabAction.Receive(actionState);
            }
            if (grabAction.Value)
            {
                publisher.Publish();
            }
            ChooseGrabProcessor();
        }

        protected virtual void OnEnable()
        {
            ConfigureGrabAction();
            ConfigureVelocityTrackers();
            ConfigurePublishers();
            ConfigureGrabPrecognition();
        }

        /// <summary>
        /// Chooses which grab processing to perform on the grab action.
        /// </summary>
        protected virtual void ChooseGrabProcessor()
        {
            bool disablePrecognition = precognitionTimer.startTime.ApproxEquals(0f);
            instantGrabProcessor.SetActive(disablePrecognition);
            precognitionGrabProcessor.SetActive(!disablePrecognition);
        }

        /// <summary>
        /// Retrieves a collection of currently grabbed GameObjects.
        /// </summary>
        /// <returns>The currently grabbed GameObjects.</returns>
        protected virtual IReadOnlyList<GameObject> GetGrabbedObjects()
        {
            grabbedObjects.Clear();

            if (grabbedObjectsCollection == null)
            {
                return grabbedObjects;
            }

            grabbedObjects.AddRange(grabbedObjectsCollection.Elements);
            return grabbedObjects;
        }

        protected virtual ActiveCollisionsContainer.EventData CreateActiveCollisionsEventData(GameObject forwardSource, Collision collision = null, Collider collider = null)
        {
            collider = collider == null ? forwardSource.GetComponentInChildren<Collider>() : collider;
            activeCollisionsEventData.activeCollisions[0].Set(forwardSource.TryGetComponent<Component>(), collider.isTrigger, collision, collider);
            return activeCollisionsEventData;
        }
    }
}
