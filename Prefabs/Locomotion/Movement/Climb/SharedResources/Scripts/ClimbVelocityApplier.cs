namespace VRTK.Prefabs.Locomotion.Movement.Climb
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection.List;
    using Zinnia.Tracking.Follow;
    using Zinnia.Tracking.Velocity;
    using Zinnia.Data.Operation.Mutation;
    using Zinnia.Data.Type.Transformation.Aggregation;
    using VRTK.Prefabs.Locomotion.BodyRepresentation;

    /// <summary>
    /// Sets up the Climb prefab based on the provided user settings.
    /// </summary>
    public class ClimbVelocityApplier : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public ClimbFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The objects defining the sources of movement.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public GameObjectObservableList Interactors { get; protected set; }
        /// <summary>
        /// The objects defining the offsets of movement.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObjectObservableList Interactables { get; protected set; }
        /// <summary>
        /// The <see cref="ComponentTrackerProxy"/> component for obtaining velocity data.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ComponentTrackerProxy VelocityProxy { get; protected set; }
        /// <summary>
        /// The <see cref="Vector3Multiplier"/> component for multiplying velocity data.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Vector3Multiplier VelocityMultiplier { get; protected set; }
        /// <summary>
        /// The <see cref="ObjectDistanceComparator"/> component for the source.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ObjectDistanceComparator SourceDistanceComparator { get; protected set; }
        /// <summary>
        /// The <see cref="ObjectDistanceComparator"/> component for the offset.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ObjectDistanceComparator OffsetDistanceComparator { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPositionMutator"/> component for the offset.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPositionMutator TargetPositionProperty { get; protected set; }
        /// <summary>
        /// The <see cref="Zinnia.Tracking.Velocity.VelocityEmitter"/> component for emitting velocity data.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public VelocityEmitter VelocityEmitter { get; protected set; }
        #endregion

        /// <summary>
        /// Applies velocity to the <see cref="BodyRepresentation"/>.
        /// </summary>
        public virtual void ApplyVelocity()
        {
            if (!isActiveAndEnabled || Interactors.NonSubscribableElements.Count > 0 || VelocityProxy.ProxySource == null)
            {
                return;
            }

            VelocityEmitter.EmitVelocity();
            Facade.BodyRepresentationFacade.ListenToRigidbodyMovement();
            Facade.BodyRepresentationFacade.PhysicsBody.velocity += VelocityMultiplier.Result;
            VelocityProxy.ProxySource = null;
        }

        /// <summary>
        /// Configures the <see cref="TargetPositionProperty"/> based on the facade settings.
        /// </summary>
        public virtual void ConfigureTargetPositionProperty()
        {
            TargetPositionProperty.Target = Facade.BodyRepresentationFacade.Offset == null ? Facade.BodyRepresentationFacade.Source : Facade.BodyRepresentationFacade.Offset;
        }

        protected virtual void OnEnable()
        {
            Interactors.Populated.AddListener(OnListBecamePopulated);
            Interactors.Added.AddListener(OnInteractorAdded);
            Interactors.Removed.AddListener(OnInteractorRemoved);
            Interactors.Emptied.AddListener(OnListBecameEmpty);

            Interactables.Added.AddListener(OnInteractableAdded);
            Interactables.Removed.AddListener(OnInteractableRemoved);

            SourceDistanceComparator.enabled = false;
            OffsetDistanceComparator.enabled = false;
            ConfigureTargetPositionProperty();
        }

        protected virtual void OnDisable()
        {
            TargetPositionProperty.Target = null;

            OffsetDistanceComparator.enabled = false;
            SourceDistanceComparator.enabled = false;

            Interactables.Removed.RemoveListener(OnInteractableRemoved);
            Interactables.Added.RemoveListener(OnInteractableAdded);

            Interactors.Emptied.RemoveListener(OnListBecameEmpty);
            Interactors.Removed.RemoveListener(OnInteractorRemoved);
            Interactors.Added.RemoveListener(OnInteractorAdded);
            Interactors.Populated.RemoveListener(OnListBecamePopulated);
        }

        /// <summary>
        /// Emits a climb started event when the list becomes populated.
        /// </summary>
        /// <param name="addedElement">The element added.</param>
        protected virtual void OnListBecamePopulated(GameObject addedElement)
        {
            if (Interactors.NonSubscribableElements.Count > 0 || Interactables.NonSubscribableElements.Count > 0)
            {
                Facade.ClimbStarted?.Invoke();
            }
        }

        /// <summary>
        /// Emits a climb stopped event when the list becomes empty.
        /// </summary>
        /// <param name="removedElement">The element removed.</param>
        protected virtual void OnListBecameEmpty(GameObject removedElement)
        {
            if (Interactors.NonSubscribableElements.Count == 0 && Interactables.NonSubscribableElements.Count == 0)
            {
                Facade.ClimbStopped?.Invoke();
            }
        }

        /// <summary>
        /// Configures the <see cref="SourceDistanceComparator"/> when an interactor is added.
        /// </summary>
        /// <param name="interactor">The added interactor.</param>
        protected virtual void OnInteractorAdded(GameObject interactor)
        {
            SourceDistanceComparator.Source = interactor;
            SourceDistanceComparator.Target = interactor;
            SourceDistanceComparator.enabled = interactor != null;
            SourceDistanceComparator.SavePosition();

            if (interactor != null)
            {
                Facade.BodyRepresentationFacade.Interest = BodyRepresentationProcessor.MovementInterest.CharacterController;
            }
        }

        /// <summary>
        /// Configures the <see cref="SourceDistanceComparator"/> when an interactor is removed.
        /// </summary>
        /// <param name="interactor">The removed interactor.</param>
        protected virtual void OnInteractorRemoved(GameObject interactor)
        {
            IReadOnlyList<GameObject> elements = Interactors.NonSubscribableElements;
            OnInteractorAdded(elements.Count == 0 ? null : elements[elements.Count - 1]);
        }

        /// <summary>
        /// Configures the <see cref="OffsetDistanceComparator"/> when an interactable is added.
        /// </summary>
        /// <param name="interactable">The added interactable.</param>
        protected virtual void OnInteractableAdded(GameObject interactable)
        {
            OffsetDistanceComparator.Source = interactable;
            OffsetDistanceComparator.Target = interactable;
            OffsetDistanceComparator.enabled = interactable != null;
            OffsetDistanceComparator.SavePosition();

            if (interactable != null)
            {
                Facade.BodyRepresentationFacade.Interest = BodyRepresentationProcessor.MovementInterest.CharacterController;
            }
        }

        /// <summary>
        /// Configures the <see cref="OffsetDistanceComparator"/> when an interactable is removed.
        /// </summary>
        /// <param name="interactable">The removed interactable.</param>
        protected virtual void OnInteractableRemoved(GameObject interactable)
        {
            IReadOnlyList<GameObject> elements = Interactables.NonSubscribableElements;
            OnInteractableAdded(elements.Count == 0 ? null : elements[elements.Count - 1]);
        }
    }
}