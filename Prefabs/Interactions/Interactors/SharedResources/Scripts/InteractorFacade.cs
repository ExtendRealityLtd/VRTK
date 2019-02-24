namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Zinnia.Action;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Velocity;
    using VRTK.Prefabs.Interactions.Interactables;

    /// <summary>
    /// The public interface into the Interactor Prefab.
    /// </summary>
    public class InteractorFacade : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the <see cref="InteractableFacade"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractableFacade>
        {
        }

        #region Interactor Settings
        [Header("Interactor Settings"), Tooltip("The BooleanAction that will initiate the Interactor grab mechanism."), SerializeField]
        private BooleanAction _grabAction;
        /// <summary>
        /// The <see cref="BooleanAction"/> that will initiate the Interactor grab mechanism.
        /// </summary>
        public BooleanAction GrabAction
        {
            get { return _grabAction; }
            set
            {
                _grabAction = value;
                grabInteractorSetup.ConfigureGrabAction();
            }
        }

        [Tooltip("The VelocityTrackerProcessor to measure the interactors current velocity."), SerializeField]
        private VelocityTrackerProcessor _velocityTracker;
        /// <summary>
        /// The <see cref="VelocityTrackerProcessor"/> to measure the interactors current velocity.
        /// </summary>
        public VelocityTrackerProcessor VelocityTracker
        {
            get { return _velocityTracker; }
            set
            {
                _velocityTracker = value;
                grabInteractorSetup.ConfigureVelocityTrackers();
            }
        }

        [Tooltip("The time between initiating the grabAction and touching an Interactable to be considered a valid grab."), SerializeField]
        private float _grabPrecognition = 0.1f;
        /// <summary>
        /// The time between initiating the <see cref="GrabAction"/> and touching an Interactable to be considered a valid grab.
        /// </summary>
        public float GrabPrecognition
        {
            get
            {
                return _grabPrecognition;
            }
            set
            {
                _grabPrecognition = value;
                grabInteractorSetup.ConfigureGrabPrecognition();
            }
        }
        #endregion

        #region Interactor Events
        /// <summary>
        /// Emitted when the Interactor starts touching a valid Interactable.
        /// </summary>
        [Header("Interactor Events")]
        public UnityEvent Touched = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor stops touching a valid Interactable.
        /// </summary>
        public UnityEvent Untouched = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor starts grabbing a valid Interactable.
        /// </summary>
        public UnityEvent Grabbed = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor stops grabbing a valid Interactable.
        /// </summary>
        public UnityEvent Ungrabbed = new UnityEvent();
        #endregion

        #region Internal Settings
        [Header("Internal Settings"), Tooltip("The linked Touch Internal Setup."), InternalSetting, SerializeField]
        protected TouchInteractorInternalSetup touchInteractorSetup;
        /// <summary>
        /// The linked Touch Internal Setup.
        /// </summary>
        public TouchInteractorInternalSetup TouchInteractorSetup => touchInteractorSetup;

        [Tooltip("The linked Grab Internal Setup."), InternalSetting, SerializeField]
        protected GrabInteractorInternalSetup grabInteractorSetup;
        /// <summary>
        /// The linked Grab Internal Setup.
        /// </summary>
        public GrabInteractorInternalSetup GrabInteractorSetup => grabInteractorSetup;
        #endregion

        /// <summary>
        /// A collection of currently touched GameObjects.
        /// </summary>
        public IReadOnlyList<GameObject> TouchedObjects => touchInteractorSetup.TouchedObjects;
        /// <summary>
        /// The currently active touched GameObject.
        /// </summary>
        public GameObject ActiveTouchedObject => touchInteractorSetup.ActiveTouchedObject;
        /// <summary>
        /// A collection of currently grabbed GameObjects.
        /// </summary>
        public IReadOnlyList<GameObject> GrabbedObjects => grabInteractorSetup.GrabbedObjects;

        /// <summary>
        /// Attempt to grab a <see cref="GameObject"/> that contains an Interactable to the current Interactor.
        /// </summary>
        /// <param name="interactable">The GameObject that the Interactable is on.</param>
        public virtual void Grab(GameObject interactable)
        {
            Grab(interactable.TryGetComponent<InteractableFacade>(true, true));
        }

        /// <summary>
        /// Attempt to grab an Interactable to the current Interactor.
        /// </summary>
        /// <param name="interactable">The Interactable to attempt to grab.</param>
        public virtual void Grab(InteractableFacade interactable)
        {
            Grab(interactable, null, null);
        }

        /// <summary>
        /// Attempt to grab an Interactable to the current Interactor utilizing custom collision data.
        /// </summary>
        /// <param name="interactable">The Interactable to attempt to grab.</param>
        /// <param name="collision">Custom collision data.</param>
        /// <param name="collider">Custom collider data.</param>
        public virtual void Grab(InteractableFacade interactable, Collision collision, Collider collider)
        {
            grabInteractorSetup.Grab(interactable, collision, collider);
        }

        /// <summary>
        /// Attempt to ungrab currently grabbed Interactables to the current Interactor.
        /// </summary>
        public virtual void Ungrab()
        {
            grabInteractorSetup.Ungrab();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            grabInteractorSetup.ConfigureGrabAction();
            grabInteractorSetup.ConfigureVelocityTrackers();
            grabInteractorSetup.ConfigureGrabPrecognition();
        }
    }
}