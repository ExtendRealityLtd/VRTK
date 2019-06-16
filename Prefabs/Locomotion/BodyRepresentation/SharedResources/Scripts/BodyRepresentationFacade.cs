namespace VRTK.Prefabs.Locomotion.BodyRepresentation
{
    using UnityEngine;
    using UnityEngine.Events;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactors;
    using VRTK.Prefabs.Interactions.Interactors.Collection;

    /// <summary>
    /// The public interface for the BodyRepresentation prefab.
    /// </summary>
    public class BodyRepresentationFacade : MonoBehaviour
    {
        #region Source Settings
        /// <summary>
        /// The object to follow.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Source Settings"), DocumentedByXml]
        public GameObject Source { get; set; }
        /// <summary>
        /// The thickness of <see cref="Source"/> to be used when resolving body collisions.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float SourceThickness { get; set; } = 0.05f;
        /// <summary>
        /// An optional offset for the <see cref="Source"/> to use.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Offset { get; set; }
        #endregion

        #region Interaction Settings
        /// <summary>
        /// A collection of interactors to exclude from physics collision checks.
        /// </summary>
        [Serialized]
        [field: Header("Interaction Settings"), DocumentedByXml]
        public InteractorFacadeObservableList IgnoredInteractors { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Emitted when the body starts touching ground.
        /// </summary>
        [Header("Events"), DocumentedByXml]
        public UnityEvent BecameGrounded = new UnityEvent();
        /// <summary>
        /// Emitted when the body stops touching ground.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent BecameAirborne = new UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public BodyRepresentationProcessor Processor { get; protected set; }
        #endregion

        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public BodyRepresentationProcessor.MovementInterest Interest
        {
            get { return Processor.Interest; }
            set { Processor.Interest = value; }
        }

        /// <summary>
        /// Whether the body touches ground.
        /// </summary>
        public bool IsCharacterControllerGrounded => Processor.IsCharacterControllerGrounded;

        /// <summary>
        /// The <see cref="Rigidbody"/> that acts as the physical representation of the body.
        /// </summary>
        public Rigidbody PhysicsBody => Processor.PhysicsBody;

        /// <summary>
        /// Sets the source of truth for movement to come from <see cref="BodyRepresentationInternalSetup.rigidbody"/> until <see cref="BodyRepresentationInternalSetup.characterController"/> hits the ground, then <see cref="BodyRepresentationInternalSetup.characterController"/> is the new source of truth.
        /// </summary>
        /// <remarks>
        /// This method needs to be called right before or right after applying any form of movement to the rigidbody while the body is grounded, i.e. in the same frame and before <see cref="BodyRepresentationInternalSetup.Process"/> is called.
        /// </remarks>
        public virtual void ListenToRigidbodyMovement()
        {
            Interest = BodyRepresentationProcessor.MovementInterest.RigidbodyUntilGrounded;
        }

        /// <summary>
        /// Solves body collisions by not moving the body in case it can't go to its current position.
        /// </summary>
        /// <remarks>
        /// If body collisions should be prevented this method needs to be called right before or right after applying any form of movement to the body.
        /// </remarks>
        public virtual void SolveBodyCollisions()
        {
            Processor.SolveBodyCollisions();
        }

        protected virtual void OnEnable()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.AddListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.AddListener(OnIgnoredInteractorRemoved);
        }

        protected virtual void OnDisable()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.RemoveListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.RemoveListener(OnIgnoredInteractorRemoved);
        }

        /// <summary>
        /// Processes when a new <see cref="InteractorFacade"/> is added to the ignored collection.
        /// </summary>
        /// <param name="interactor">The interactor to ignore collisions from.</param>
        protected virtual void OnIgnoredInteractorAdded(InteractorFacade interactor)
        {
            Processor.IgnoreInteractorsCollisions(interactor);
        }

        /// <summary>
        /// Processes when a new <see cref="InteractorFacade"/> is removed from the ignored collection.
        /// </summary>
        /// <param name="interactor">The interactor to resume collisions with.</param>
        protected virtual void OnIgnoredInteractorRemoved(InteractorFacade interactor)
        {
            Processor.ResumeInteractorsCollisions(interactor);
        }

        /// <summary>
        /// Called after <see cref="Source"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Source))]
        protected virtual void OnAfterSourceChange()
        {
            Processor.ConfigureSourceObjectFollower();
        }

        /// <summary>
        /// Called after <see cref="Offset"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Offset))]
        protected virtual void OnAfterOffsetChange()
        {
            Processor.ConfigureOffsetObjectFollower();
        }

        /// <summary>
        /// Called after <see cref="IgnoredInteractors"/> has been changed.
        /// </summary>
        [CalledBeforeChangeOf(nameof(IgnoredInteractors))]
        protected virtual void OnBeforeIgnoredInteractorsChange()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.RemoveListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.RemoveListener(OnIgnoredInteractorRemoved);
        }

        /// <summary>
        /// Called after <see cref="IgnoredInteractors"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(IgnoredInteractors))]
        protected virtual void OnAfterIgnoredInteractorsChange()
        {
            if (IgnoredInteractors == null)
            {
                return;
            }

            IgnoredInteractors.Added.AddListener(OnIgnoredInteractorAdded);
            IgnoredInteractors.Removed.AddListener(OnIgnoredInteractorRemoved);
        }
    }
}