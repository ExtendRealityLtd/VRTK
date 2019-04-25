namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Rule;
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
        [Header("Touch Events"), DocumentedByXml]
        public UnityEvent FirstTouched = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor touches the Interactable.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Touched = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor stops touching the Interactable.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Untouched = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactable is untouched for the last time by an Interactor.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent LastUntouched = new UnityEvent();
        #endregion

        #region Grab Events
        /// <summary>
        /// Emitted when the Interactable is grabbed for the first time by an Interactor.
        /// </summary>
        [Header("Grab Events"), DocumentedByXml]
        public UnityEvent FirstGrabbed = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor grabs the Interactable.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Grabbed = new UnityEvent();
        /// <summary>
        /// Emitted when an Interactor ungrabs the Interactable.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Ungrabbed = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactable is ungrabbed for the last time by an Interactor.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent LastUngrabbed = new UnityEvent();
        #endregion

        #region Restriction Settings
        /// <summary>
        /// The rule to determine what is not allowed to touch this interactable.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Restriction Settings"), DocumentedByXml]
        public RuleContainer DisallowedTouchInteractors { get; set; }
        /// <summary>
        /// The rule to determine what is not allowed to grab this interactable.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public RuleContainer DisallowedGrabInteractors { get; set; }
        #endregion

        #region Container Settings
        /// <summary>
        /// The overall container for the interactable consumers.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Container Settings"), DocumentedByXml]
        public GameObject ConsumerContainer { get; set; }
        [Serialized, Cleared]
        [field: DocumentedByXml]
        /// <summary>
        /// The <see cref="Rigidbody"/> for the overall collisions.
        /// </summary>
        public Rigidbody ConsumerRigidbody { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked <see cref="CollisionNotifier"/>.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public CollisionNotifier CollisionNotifier { get; protected set; }
        /// <summary>
        /// The linked Touch Internal Setup.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TouchInteractableConfigurator TouchConfiguration { get; protected set; }
        /// <summary>
        /// The linked Grab Internal Setup.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GrabInteractableConfigurator GrabConfiguration { get; protected set; }
        #endregion

        /// <summary>
        /// A collection of Interactors that are currently touching the Interactable.
        /// </summary>
        public IReadOnlyList<InteractorFacade> TouchingInteractors => TouchConfiguration.TouchingInteractors;
        /// <summary>
        /// A collection of Interactors that are currently grabbing the Interactable.
        /// </summary>
        public IReadOnlyList<InteractorFacade> GrabbingInteractors => GrabConfiguration.GrabbingInteractors;
        /// <summary>
        /// Determines if the grab type is set to toggle.
        /// </summary>
        public bool IsGrabTypeToggle => GrabConfiguration.IsGrabTypeToggle;
        /// <summary>
        /// Whether the Interactable is currently being touched by any valid Interactor.
        /// </summary>
        public bool IsTouched => TouchingInteractors.Count > 0;
        /// <summary>
        /// Whether the Interactable is currently being grabbed by any valid Interactor.
        /// </summary>
        public bool IsGrabbed => GrabbingInteractors.Count > 0;

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
            GrabConfiguration.Grab(interactor);
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
            GrabConfiguration.Ungrab(interactor);
        }

        /// <summary>
        /// Attempt to ungrab the Interactable at a specific grabbing index.
        /// </summary>
        /// <param name="sequenceIndex">The Interactor sequence index to ungrab from.</param>
        public virtual void Ungrab(int sequenceIndex = 0)
        {
            GrabConfiguration.Ungrab(sequenceIndex);
        }

        /// <summary>
        /// Configures any container data to the sub setup components.
        /// </summary>
        protected virtual void ConfigureContainer()
        {
            TouchConfiguration.ConfigureContainer();
            GrabConfiguration.ConfigureContainer();
        }

        /// <summary>
        /// Called after <see cref="DisallowedTouchInteractors"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(DisallowedTouchInteractors))]
        protected virtual void OnAfterDisallowedTouchInteractorsChange()
        {
            TouchConfiguration.TouchValidity.ReceiveValidity = DisallowedTouchInteractors;
        }

        /// <summary>
        /// Called after <see cref="DisallowedGrabInteractors"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(DisallowedGrabInteractors))]
        protected virtual void OnAfterDisallowedGrabInteractorsChange()
        {
            GrabConfiguration.GrabReceiver.GrabValidity.ReceiveValidity = DisallowedGrabInteractors;
        }

        /// <summary>
        /// Called after <see cref="ConsumerContainer"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ConsumerContainer))]
        protected virtual void OnAfterConsumerContainerChange()
        {
            ConfigureContainer();
        }

        /// <summary>
        /// Called after <see cref="ConsumerRigidbody"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ConsumerRigidbody))]
        protected virtual void OnAfterConsumerRigidbodyChange()
        {
            ConfigureContainer();
        }
    }
}