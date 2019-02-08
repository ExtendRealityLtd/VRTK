namespace VRTK.Prefabs.Locomotion.Movement.Climb
{
    using UnityEngine;
    using System.Linq;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection;
    using Zinnia.Data.Operation;
    using Zinnia.Data.Type.Transformation;
    using Zinnia.Tracking.Follow;
    using Zinnia.Tracking.Velocity;
    using VRTK.Prefabs.Locomotion.BodyRepresentation;

    /// <summary>
    /// Sets up the Climb prefab based on the provided user settings.
    /// </summary>
    public class ClimbInternalSetup : MonoBehaviour
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected ClimbFacade facade;
        #endregion

        #region Reference Settings
        [Header("Reference Settings"), Tooltip("The objects defining the sources of movement."), InternalSetting, SerializeField]
        protected GameObjectObservableList interactors;
        /// <summary>
        /// The objects defining the sources of movement.
        /// </summary>
        public GameObjectObservableList Interactors => interactors;

        [Tooltip("The objects defining the offsets of movement."), InternalSetting, SerializeField]
        protected GameObjectObservableList interactables;
        /// <summary>
        /// The objects defining the offsets of movement.
        /// </summary>
        public GameObjectObservableList Interactables => interactables;

        [Tooltip("The ComponentTrackerProxy component for obtaining velocity data."), InternalSetting, SerializeField]
        protected ComponentTrackerProxy velocityProxy;
        /// <summary>
        /// The <see cref="ComponentTrackerProxy"/> component for obtaining velocity data.
        /// </summary>
        public ComponentTrackerProxy VelocityProxy => velocityProxy;

        [Tooltip("The Vector3Multiplier component for multiplying velocity data."), InternalSetting, SerializeField]
        protected Vector3Multiplier velocityMultiplier;
        /// <summary>
        /// The <see cref="Vector3Multiplier"/> component for multiplying velocity data.
        /// </summary>
        public Vector3Multiplier VelocityMultiplier => velocityMultiplier;

        /// <summary>
        /// The <see cref="ObjectDistanceComparator"/> component for the source.
        /// </summary>
        [Tooltip("The ObjectDistanceComparator component for the source."), InternalSetting, SerializeField]
        protected ObjectDistanceComparator sourceDistanceComparator;
        /// <summary>
        /// The <see cref="ObjectDistanceComparator"/> component for the offset.
        /// </summary>
        [Tooltip("The ObjectDistanceComparator component for the offset."), InternalSetting, SerializeField]
        protected ObjectDistanceComparator offsetDistanceComparator;
        /// <summary>
        /// The <see cref="TransformPositionMutator"/> component for the offset.
        /// </summary>
        [Tooltip("The TransformPositionMutator component for the target."), InternalSetting, SerializeField]
        protected TransformPositionMutator targetPositionProperty;
        /// <summary>
        /// The <see cref="VelocityEmitter"/> component for emitting velocity data.
        /// </summary>
        [Tooltip("The VelocityEmitter component for emitting velocity data."), InternalSetting, SerializeField]
        protected VelocityEmitter velocityEmitter;
        #endregion

        /// <summary>
        /// Applies velocity to the <see cref="BodyRepresentation"/>.
        /// </summary>
        public virtual void ApplyVelocity()
        {
            if (!isActiveAndEnabled || interactors.Elements.Any() || velocityProxy.ProxySource == null)
            {
                return;
            }

            velocityEmitter.EmitVelocity();
            facade.BodyRepresentationFacade.ListenToRigidbodyMovement();
            facade.BodyRepresentationFacade.PhysicsBody.velocity += velocityMultiplier.Result;
            velocityProxy.ClearProxySource();
        }

        /// <summary>
        /// Configures the <see cref="targetPositionProperty"/> based on the facade settings.
        /// </summary>
        public virtual void ConfigureTargetPositionProperty()
        {
            targetPositionProperty.Target = facade.BodyRepresentationFacade.Offset == null ? facade.BodyRepresentationFacade.Source : facade.BodyRepresentationFacade.Offset;
        }

        protected virtual void OnEnable()
        {
            interactors.BecamePopulated.AddListener(OnListBecamePopulated);
            interactors.ElementAdded.AddListener(OnInteractorAdded);
            interactors.ElementRemoved.AddListener(OnInteractorRemoved);
            interactors.BecameEmpty.AddListener(OnListBecameEmpty);

            interactables.ElementAdded.AddListener(OnInteractableAdded);
            interactables.ElementRemoved.AddListener(OnInteractableRemoved);

            sourceDistanceComparator.enabled = false;
            offsetDistanceComparator.enabled = false;
            ConfigureTargetPositionProperty();
        }

        protected virtual void OnDisable()
        {
            targetPositionProperty.Target = null;

            offsetDistanceComparator.enabled = false;
            sourceDistanceComparator.enabled = false;

            interactables.ElementRemoved.RemoveListener(OnInteractableRemoved);
            interactables.ElementAdded.RemoveListener(OnInteractableAdded);

            interactors.BecameEmpty.RemoveListener(OnListBecameEmpty);
            interactors.ElementRemoved.RemoveListener(OnInteractorRemoved);
            interactors.ElementAdded.RemoveListener(OnInteractorAdded);
            interactors.BecamePopulated.RemoveListener(OnListBecamePopulated);
        }

        /// <summary>
        /// Emits a climb started event when the list becomes populated.
        /// </summary>
        /// <param name="addedElement">The element added.</param>
        protected virtual void OnListBecamePopulated(GameObject addedElement)
        {
            if (interactors.Elements.Any() || interactables.Elements.Any())
            {
                facade.ClimbStarted?.Invoke();
            }
        }

        /// <summary>
        /// Emits a climb stopped event when the list becomes empty.
        /// </summary>
        /// <param name="addedElement">The element removed.</param>
        protected virtual void OnListBecameEmpty(GameObject removedElement)
        {
            if (!interactors.Elements.Any() && !interactables.Elements.Any())
            {
                facade.ClimbStopped?.Invoke();
            }
        }

        /// <summary>
        /// Configures the <see cref="sourceDistanceComparator"/> when an interactor is added.
        /// </summary>
        /// <param name="interactor">The added interactor.</param>
        protected virtual void OnInteractorAdded(GameObject interactor)
        {
            sourceDistanceComparator.Source = interactor;
            sourceDistanceComparator.Target = interactor;
            sourceDistanceComparator.enabled = interactor != null;
            sourceDistanceComparator.SavePosition();

            if (interactor != null)
            {
                facade.BodyRepresentationFacade.Interest = BodyRepresentationInternalSetup.MovementInterest.CharacterController;
            }
        }

        /// <summary>
        /// Configures the <see cref="sourceDistanceComparator"/> when an interactor is removed.
        /// </summary>
        /// <param name="interactor">The removed interactor.</param>
        protected virtual void OnInteractorRemoved(GameObject interactor)
        {
            OnInteractorAdded(interactors.Elements.LastOrDefault());
        }

        /// <summary>
        /// Configures the <see cref="offsetDistanceComparator"/> when an interactable is added.
        /// </summary>
        /// <param name="interactable">The added interactable.</param>
        protected virtual void OnInteractableAdded(GameObject interactable)
        {
            offsetDistanceComparator.Source = interactable;
            offsetDistanceComparator.Target = interactable;
            offsetDistanceComparator.enabled = interactable != null;
            offsetDistanceComparator.SavePosition();

            if (interactable != null)
            {
                facade.BodyRepresentationFacade.Interest = BodyRepresentationInternalSetup.MovementInterest.CharacterController;
            }
        }

        /// <summary>
        /// Configures the <see cref="offsetDistanceComparator"/> when an interactable is removed.
        /// </summary>
        /// <param name="interactable">The removed interactable.</param>
        protected virtual void OnInteractableRemoved(GameObject interactable)
        {
            OnInteractableAdded(interactables.Elements.LastOrDefault());
        }
    }
}