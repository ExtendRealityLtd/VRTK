namespace VRTK.Prefabs.Locomotion.BodyRepresentation
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// The public interface for the BodyRepresentation prefab.
    /// </summary>
    public class BodyRepresentationFacade : MonoBehaviour
    {
        #region Source Settings
        [Header("Source Settings"), Tooltip("The object to follow."), SerializeField]
        private GameObject _source;
        /// <summary>
        /// The object to follow.
        /// </summary>
        public GameObject Source
        {
            get { return _source; }
            set
            {
                _source = value;
                internalSetup.ConfigureSourceObjectFollower();
            }
        }
        /// <summary>
        /// The thickness of <see cref="Source"/> to be used when resolving body collisions.
        /// </summary>
        [Tooltip("The thickness of source to be used when resolving body collisions.")]
        public float sourceThickness = 0.05f;

        [Tooltip("An optional offset for the source to use."), SerializeField]
        private GameObject _offset;
        /// <summary>
        /// An optional offset for the <see cref="Source"/> to use.
        /// </summary>
        public GameObject Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                internalSetup.ConfigureOffsetObjectFollower();
            }
        }
        #endregion

        #region Interaction Settings
        /// <summary>
        /// A collection of interactors to exclude from physics collision checks.
        /// </summary>
        [Header("Interaction Settings"), Tooltip("A collection of interactors to exclude from physics collision checks.")]
        public List<InteractorFacade> ignoredInteractors = new List<InteractorFacade>();
        #endregion

        #region Events
        /// <summary>
        /// Emitted when the body starts touching ground.
        /// </summary>
        [Header("Events"), Tooltip("Emitted when the body starts touching ground.")]
        public UnityEvent BecameGrounded = new UnityEvent();
        /// <summary>
        /// Emitted when the body stops touching ground.
        /// </summary>
        [Tooltip("Emitted when the body stops touching ground.")]
        public UnityEvent BecameAirborne = new UnityEvent();
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected BodyRepresentationInternalSetup internalSetup;
        #endregion

        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public BodyRepresentationInternalSetup.MovementInterest Interest
        {
            get
            {
                return internalSetup.Interest;
            }
            set
            {
                internalSetup.Interest = value;
            }
        }

        /// <summary>
        /// Whether the body touches ground.
        /// </summary>
        public bool IsCharacterControllerGrounded => internalSetup.IsCharacterControllerGrounded;

        /// <summary>
        /// The <see cref="Rigidbody"/> that acts as the physical representation of the body.
        /// </summary>
        public Rigidbody PhysicsBody => internalSetup.PhysicsBody;

        /// <summary>
        /// Sets the source of truth for movement to come from <see cref="BodyRepresentationInternalSetup.rigidbody"/> until <see cref="BodyRepresentationInternalSetup.characterController"/> hits the ground, then <see cref="BodyRepresentationInternalSetup.characterController"/> is the new source of truth.
        /// </summary>
        /// <remarks>
        /// This method needs to be called right before or right after applying any form of movement to the rigidbody while the body is grounded, i.e. in the same frame and before <see cref="BodyRepresentationInternalSetup.Process"/> is called.
        /// </remarks>
        public virtual void ListenToRigidbodyMovement()
        {
            Interest = BodyRepresentationInternalSetup.MovementInterest.RigidbodyUntilGrounded;
        }

        /// <summary>
        /// Solves body collisions by not moving the body in case it can't go to its current position.
        /// </summary>
        /// <remarks>
        /// If body collisions should be prevented this method needs to be called right before or right after applying any form of movement to the body.
        /// </remarks>
        public virtual void SolveBodyCollisions()
        {
            internalSetup.SolveBodyCollisions();
        }

        /// <summary>
        /// Refreshes the ignored interactors.
        /// </summary>
        public virtual void RefreshIgnoredInteractors()
        {
            internalSetup.ConfigureIgnoreInteractorsCollisions();
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            internalSetup.ConfigureSourceObjectFollower();
            internalSetup.ConfigureOffsetObjectFollower();
            RefreshIgnoredInteractors();
        }
    }
}