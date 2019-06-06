namespace VRTK.Prefabs.Interactions.Interactors
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using Malimbe.MemberChangeMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;
    using Zinnia.Extension;
    using Zinnia.Data.Type;
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
        public class UnityEvent : UnityEvent<InteractableFacade> { }

        #region Interactor Settings
        /// <summary>
        /// The <see cref="BooleanAction"/> that will initiate the Interactor grab mechanism.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Interactor Settings"), DocumentedByXml]
        public BooleanAction GrabAction { get; set; }
        /// <summary>
        /// The <see cref="VelocityTrackerProcessor"/> to measure the interactors current velocity.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public VelocityTrackerProcessor VelocityTracker { get; set; }
        /// <summary>
        /// The time between initiating the <see cref="GrabAction"/> and touching an Interactable to be considered a valid grab.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float GrabPrecognition { get; set; } = 0.1f;
        #endregion

        #region Interactor Events
        /// <summary>
        /// Emitted when the Interactor starts touching a valid Interactable.
        /// </summary>
        [Header("Interactor Events"), DocumentedByXml]
        public UnityEvent Touched = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor stops touching a valid Interactable.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Untouched = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor starts grabbing a valid Interactable.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Grabbed = new UnityEvent();
        /// <summary>
        /// Emitted when the Interactor stops grabbing a valid Interactable.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent Ungrabbed = new UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Touch Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public TouchInteractorConfigurator TouchConfiguration { get; protected set; }
        /// <summary>
        /// The linked Grab Internal Setup.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GrabInteractorConfigurator GrabConfiguration { get; protected set; }
        #endregion

        /// <summary>
        /// A collection of currently touched GameObjects.
        /// </summary>
        public IReadOnlyList<GameObject> TouchedObjects => TouchConfiguration.TouchedObjects;
        /// <summary>
        /// The currently active touched GameObject.
        /// </summary>
        public GameObject ActiveTouchedObject => TouchConfiguration.ActiveTouchedObject;
        /// <summary>
        /// A collection of currently grabbed GameObjects.
        /// </summary>
        public IReadOnlyList<GameObject> GrabbedObjects => GrabConfiguration.GrabbedObjects;

        /// <summary>
        /// Attempt to attach a <see cref="GameObject"/> that contains an <see cref="InteractableFacade"/> to this <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="interactable">The GameObject that the Interactable is on.</param>
        public virtual void Grab(GameObject interactable)
        {
            Grab(interactable.TryGetComponent<InteractableFacade>(true, true));
        }

        /// <summary>
        /// Attempt to attach an <see cref="InteractableFacade"/> to this <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="interactable">The Interactable to attempt to grab.</param>
        public virtual void Grab(InteractableFacade interactable)
        {
            Grab(interactable, null, null);
        }

        /// <summary>
        /// Attempt to attach an <see cref="InteractableFacade"/> found in the given <see cref="SurfaceData"/> to this <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="data">The collision data containing a valid Interactable.</param>
        public virtual void Grab(SurfaceData data)
        {
            if (data == null || data.CollisionData.transform == null)
            {
                return;
            }

            Grab(data.CollisionData.transform.gameObject.TryGetComponent<InteractableFacade>(true, true), null, null);
        }

        /// <summary>
        /// Attempt to attach an <see cref="InteractableFacade"/> to this <see cref="InteractorFacade"/> utilizing custom collision data.
        /// </summary>
        /// <param name="interactable">The Interactable to attempt to grab.</param>
        /// <param name="collision">Custom collision data.</param>
        /// <param name="collider">Custom collider data.</param>
        public virtual void Grab(InteractableFacade interactable, Collision collision, Collider collider)
        {
            GrabConfiguration.Grab(interactable, collision, collider);
        }

        /// <summary>
        /// Attempt to ungrab currently grabbed Interactables to the current Interactor.
        /// </summary>
        public virtual void Ungrab()
        {
            GrabConfiguration.Ungrab();
        }

        /// <summary>
        /// Notifies the interactor that it is touching an interactable.
        /// </summary>
        /// <param name="interactable">The interactable being touched.</param>
        public virtual void NotifyOfTouch(InteractableFacade interactable)
        {
            Touched?.Invoke(interactable);
        }

        /// <summary>
        /// Notifies the interactor that it is no longer touching an interactable.
        /// </summary>
        /// <param name="interactable">The interactable being untouched.</param>
        public virtual void NotifyOfUntouch(InteractableFacade interactable)
        {
            Untouched?.Invoke(interactable);
        }

        /// <summary>
        /// Notifies the interactor that it is grabbing an interactable.
        /// </summary>
        /// <param name="interactable">The interactable being grabbed.</param>
        public virtual void NotifyOfGrab(InteractableFacade interactable)
        {
            Grabbed?.Invoke(interactable);
            GrabConfiguration.GrabbedObjectsCollection.AddUnique(interactable.gameObject);
        }

        /// <summary>
        /// Notifies the interactor that it is no longer grabbing an interactable.
        /// </summary>
        /// <param name="interactable">The interactable being ungrabbed.</param>
        public virtual void NotifyOfUngrab(InteractableFacade interactable)
        {
            Ungrabbed?.Invoke(interactable);
            GrabConfiguration.GrabbedObjectsCollection.Remove(interactable.gameObject);
            GrabConfiguration.StopGrabbingPublisher.ClearActiveCollisions();
        }

        /// <summary>
        /// Called after <see cref="GrabAction"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(GrabAction))]
        protected virtual void OnAfterGrabActionChange()
        {
            GrabConfiguration.ConfigureGrabAction();
        }

        /// <summary>
        /// Called after <see cref="VelocityTracker"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(VelocityTracker))]
        protected virtual void OnAfterVelocityTrackerChange()
        {
            GrabConfiguration.ConfigureVelocityTrackers();
        }

        /// <summary>
        /// Called after <see cref="GrabPrecognition"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(GrabPrecognition))]
        protected virtual void OnAfterGrabPrecognitionChange()
        {
            GrabConfiguration.ConfigureGrabPrecognition();
        }
    }
}