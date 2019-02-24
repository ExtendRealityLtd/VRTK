namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Zinnia.Extension;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Collision;
    using VRTK.Prefabs.Interactions.Interactors;
    using VRTK.Prefabs.Interactions.Interactables.Touch;
    using VRTK.Prefabs.Interactions.Interactables.Grab;

    /// <summary>
    /// The public interface into the Interactable Prefab.
    /// </summary>
    public class InteractableFacade : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the <see cref="InteractorFacade"/>.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<InteractorFacade>
        {
        }

        #region Touch Events
        /// <summary>
        /// Emitted when the Interactable is touched for the first time by an Interactor.
        /// </summary>
        [Header("Touch Events")]
        public UnityEvent FirstTouched = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor touches the Interactable.
        /// </summary>
        public UnityEvent Touched = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor stops touching the Interactable.
        /// </summary>
        public UnityEvent Untouched = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactable is untouched for the last time by an Interactor.
        /// </summary>
        public UnityEvent LastUntouched = new UnityEvent();
        #endregion

        #region Grab Events
        /// <summary>
        /// Emitted when the Interactable is grabbed for the first time by an Interactor.
        /// </summary>
        [Header("Grab Events")]
        public UnityEvent FirstGrabbed = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor grabs the Interactable.
        /// </summary>
        public UnityEvent Grabbed = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor ungrabs the Interactable.
        /// </summary>
        public UnityEvent Ungrabbed = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactable is ungrabbed for the last time by an Interactor.
        /// </summary>
        public UnityEvent LastUngrabbed = new UnityEvent();
        #endregion

        #region Restriction Settings
        /// <summary>
        /// A collection of interactors that are not allowed to touch this interactable.
        /// </summary>
        [Header("Restriction Settings"), Tooltip("A collection of interactors that are not allowed to touch this interactable.")]
        public List<InteractorFacade> disallowedTouchInteractors = new List<InteractorFacade>();
        /// <summary>
        /// A collection of interactors that are not allowed to grab this interactable.
        /// </summary>
        [Tooltip("A collection of interactors that are not allowed to grab this interactable.")]
        public List<InteractorFacade> disallowedGrabInteractors = new List<InteractorFacade>();
        #endregion

        #region Container Settings
        [Header("Container Settings"), Tooltip("The overall container for the touch consumers."), SerializeField]
        private GameObject _consumerContainer;
        /// <summary>
        /// The overall container for the interactable consumers.
        /// </summary>
        public GameObject ConsumerContainer
        {
            get { return _consumerContainer; }
            set
            {
                _consumerContainer = value;
                ConfigureContainer();
            }
        }
        [Tooltip("The rigidbody for the overall collisions."), SerializeField]
        private Rigidbody _consumerRigidbody;
        /// <summary>
        /// The <see cref="Rigidbody"/> for the overall collisions.
        /// </summary>
        public Rigidbody ConsumerRigidbody
        {
            get { return _consumerRigidbody; }
            set
            {
                _consumerRigidbody = value;
                ConfigureContainer();
            }
        }
        #endregion

        #region Internal Settings
        [Header("Internal Settings"), Tooltip("The linked CollisionNotifier."), SerializeField, InternalSetting]
        private CollisionNotifier _collisionNotifier = null;
        /// <summary>
        /// The linked <see cref="CollisionNotifier"/>.
        /// </summary>
        public CollisionNotifier CollisionNotifier => _collisionNotifier;

        /// <summary>
        /// The linked Touch Internal Setup.
        /// </summary>
        [Tooltip("The linked Touch Internal Setup."), InternalSetting, SerializeField]
        protected TouchInteractableInternalSetup touchInteractableSetup;
        /// <summary>
        /// The linked Grab Internal Setup.
        /// </summary>
        [Tooltip("The linked Grab Internal Setup."), InternalSetting, SerializeField]
        protected GrabInteractableInternalSetup grabInteractableSetup;
        #endregion

        /// <summary>
        /// A collection of Interactors that are currently touching the Interactable.
        /// </summary>
        public IReadOnlyList<InteractorFacade> TouchingInteractors => touchInteractableSetup.TouchingInteractors;
        /// <summary>
        /// A collection of Interactors that are currently grabbing the Interactable.
        /// </summary>
        public IReadOnlyList<InteractorFacade> GrabbingInteractors => grabInteractableSetup.GrabbingInteractors;

        /// <summary>
        /// Attempt to grab the Interactable to the given <see cref="GameObject"/> that contains an Interactor.
        /// </summary>
        /// <param name="interactor">The GameObject that the Interactor is on.</param>
        public virtual void Grab(GameObject interactor)
        {
            Grab(interactor.TryGetComponent<InteractorFacade>(true, true));
        }

        /// <summary>
        /// Attempt to grab the Interactable to the given Interactor.
        /// </summary>
        /// <param name="interactor">The Interactor to attach the Interactable to.</param>
        public virtual void Grab(InteractorFacade interactor)
        {
            grabInteractableSetup.Grab(interactor);
        }

        /// <summary>
        /// Attempt to ungrab the Interactable to the given <see cref="GameObject"/> that contains an Interactor.
        /// </summary>
        /// <param name="interactor">The GameObject that the Interactor is on.</param>
        public virtual void Ungrab(GameObject interactor)
        {
            Ungrab(interactor.TryGetComponent<InteractorFacade>(true, true));
        }

        /// <summary>
        /// Attempt to ungrab the Interactable.
        /// </summary>
        /// <param name="interactor">The Interactor to ungrab from.</param>
        public virtual void Ungrab(InteractorFacade interactor)
        {
            grabInteractableSetup.Ungrab(interactor);
        }

        /// <summary>
        /// Attempt to ungrab the Interactable at a specific grabbing index.
        /// </summary>
        /// <param name="sequenceIndex">The Interactor sequence index to ungrab from.</param>
        public virtual void Ungrab(int sequenceIndex = 0)
        {
            grabInteractableSetup.Ungrab(sequenceIndex);
        }

        /// <summary>
        /// Refreshes the interactor restrictions.
        /// </summary>
        public virtual void RefreshInteractorRestrictions()
        {
            touchInteractableSetup.ConfigureTouchValidity(disallowedTouchInteractors);
            grabInteractableSetup.ConfigureGrabValidity(disallowedGrabInteractors);
        }

        /// <summary>
        /// Determines if the grab type is set to toggle.
        /// </summary>
        /// <returns>Whether the grab type is of type toggle.</returns>
        public virtual bool IsGrabTypeToggle()
        {
            return grabInteractableSetup.IsGrabTypeToggle();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ConfigureContainer();
        }

        /// <summary>
        /// Configures any container data to the sub setup components.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            touchInteractableSetup.ConfigureContainer();
            grabInteractableSetup.ConfigureContainer();
        }
    }
}