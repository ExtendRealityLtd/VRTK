namespace VRTK.Prefabs.Locomotion.BodyRepresentation
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using Zinnia.Process;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Collection;
    using Zinnia.Tracking.Follow;
    using VRTK.Prefabs.Interactions.Interactors;
    using VRTK.Prefabs.Interactions.Interactables;

    /// <summary>
    /// Sets up the BodyRepresentation prefab based on the provided user settings and implements the logic to represent a body.
    /// </summary>
    public class BodyRepresentationInternalSetup : MonoBehaviour, IProcessable
    {
        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public enum MovementInterest
        {
            /// <summary>
            /// The source of truth for movement comes from <see cref="characterController"/>.
            /// </summary>
            CharacterController,
            /// <summary>
            /// The source of truth for movement comes from <see cref="characterController"/> until <see cref="rigidbody"/> is in the air, then <see cref="rigidbody"/> is the new source of truth.
            /// </summary>
            CharacterControllerUntilAirborne,
            /// <summary>
            /// The source of truth for movement comes from <see cref="rigidbody"/>.
            /// </summary>
            Rigidbody,
            /// <summary>
            /// The source of truth for movement comes from <see cref="rigidbody"/> until <see cref="characterController"/> hits the ground, then <see cref="characterController"/> is the new source of truth.
            /// </summary>
            RigidbodyUntilGrounded
        }

        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Header("Facade Settings"), Tooltip("The public interface facade."), InternalSetting, SerializeField]
        protected BodyRepresentationFacade facade;
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="CharacterController"/> that acts as the main representation of the body.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The Character Controller that acts as the main representation of the body."), InternalSetting, SerializeField]
        protected CharacterController characterController;

        [Tooltip("The Rigidbody that acts as the physical representation of the body."), InternalSetting, SerializeField]
        protected Rigidbody physicsBody;
        /// <summary>
        /// The <see cref="Rigidbody"/> that acts as the physical representation of the body.
        /// </summary>
        public Rigidbody PhysicsBody => physicsBody;

        /// <summary>
        /// The <see cref="CapsuleCollider"/> that acts as the physical collider representation of the body.
        /// </summary>
        [Tooltip("The Rigidbody that acts as the physical collider representation of the body."), InternalSetting, SerializeField]
        protected CapsuleCollider rigidbodyCollider;
        /// <summary>
        /// An observable list of GameObjects to ignore collisions on.
        /// </summary>
        [Tooltip("An observable list of GameObjects to ignore collisions on."), InternalSetting, SerializeField]
        protected GameObjectObservableList ignoredGameObjectCollisions;
        #endregion

        private MovementInterest _interest = MovementInterest.CharacterControllerUntilAirborne;
        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public MovementInterest Interest
        {
            get
            {
                return _interest;
            }
            set
            {
                _interest = value;

                switch (value)
                {
                    case MovementInterest.CharacterController:
                    case MovementInterest.CharacterControllerUntilAirborne:
                        physicsBody.isKinematic = true;
                        rigidbodySetFrameCount = 0;
                        break;
                    case MovementInterest.Rigidbody:
                    case MovementInterest.RigidbodyUntilGrounded:
                        physicsBody.isKinematic = false;
                        rigidbodySetFrameCount = Time.frameCount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        /// <summary>
        /// Whether <see cref="characterController"/> touches ground.
        /// </summary>
        public bool IsCharacterControllerGrounded => wasCharacterControllerGrounded == true;

        /// <summary>
        /// Movement to apply to <see cref="characterController"/> to resolve collisions.
        /// </summary>
        protected static readonly Vector3 collisionResolutionMovement = new Vector3(0.001f, 0f, 0f);
        /// <summary>
        /// The colliders to ignore body collisions with.
        /// </summary>
        protected HashSet<Collider> ignoredColliders = new HashSet<Collider>();
        /// <summary>
        /// The colliders to restore after an ungrab.
        /// </summary>
        protected HashSet<Collider> RestoreColliders = new HashSet<Collider>();
        /// <summary>
        /// The previous position of <see cref="PhysicsBody"/>.
        /// </summary>
        protected Vector3 previousRigidbodyPosition;
        /// <summary>
        /// Whether <see cref="characterController"/> was grounded previously.
        /// </summary>
        protected bool? wasCharacterControllerGrounded;
        /// <summary>
        /// The frame count of the last time <see cref="Interest"/> was set to <see cref="MovementInterest.Rigidbody"/> or <see cref="MovementInterest.RigidbodyUntilGrounded"/>.
        /// </summary>
        protected int rigidbodySetFrameCount;
        /// <summary>
        /// Stores the routine for ignoring interactor collisions.
        /// </summary>
        protected Coroutine ignoreInteractorCollisions;
        /// <summary>
        /// An optional follower of <see cref="BodyRepresentationFacade.Offset"/>.
        /// </summary>
        protected ObjectFollower offsetObjectFollower;
        /// <summary>
        /// An optional follower of <see cref="BodyRepresentationFacade.Source"/>.
        /// </summary>
        protected ObjectFollower sourceObjectFollower;

        /// <summary>
        /// Ignores collisions from the given <see cref="GameObject"/> with the <see cref="rigidbodyCollider"/> and <see cref="characterController"/>.
        /// </summary>
        /// <param name="toIgnore">The object to ignore collisions from.</param>
        public virtual void IgnoreCollisionsWith(GameObject toIgnore)
        {
            if (toIgnore == null)
            {
                return;
            }

            foreach (Collider foundCollider in toIgnore.GetComponentsInChildren<Collider>(true))
            {
                IgnoreCollisionsWith(foundCollider);
            }
        }

        /// <summary>
        /// Ignores collisions from the given <see cref="Collider"/> with the <see cref="rigidbodyCollider"/> and <see cref="characterController"/>.
        /// </summary>
        /// <param name="toIgnore">The collider to ignore collisions from.</param>
        public virtual void IgnoreCollisionsWith(Collider toIgnore)
        {
            if (toIgnore == null)
            {
                return;
            }

            if (!ignoredColliders.Contains(toIgnore))
            {
                Physics.IgnoreCollision(toIgnore, rigidbodyCollider, true);
                Physics.IgnoreCollision(toIgnore, characterController, true);
                ignoredColliders.Add(toIgnore);
            }
        }

        /// <summary>
        /// Resumes collisions with the given <see cref="GameObject"/> with the <see cref="rigidbodyCollider"/> and <see cref="characterController"/>.
        /// </summary>
        /// <param name="toResume">The object to resume collisions with.</param>
        public virtual void ResumeCollisionsWith(GameObject toResume)
        {
            if (toResume == null)
            {
                return;
            }

            foreach (Collider foundCollider in toResume.GetComponentsInChildren<Collider>(true))
            {
                ResumeCollisionsWith(foundCollider);
            }
        }

        /// <summary>
        /// Resumes collisions with the given <see cref="Collider"/> with the <see cref="rigidbodyCollider"/> and <see cref="characterController"/>.
        /// </summary>
        /// <param name="toResume">The collider to resume collisions with.</param>
        public virtual void ResumeCollisionsWith(Collider toResume)
        {
            if (toResume == null)
            {
                return;
            }

            if (ignoredColliders.Remove(toResume))
            {
                Physics.IgnoreCollision(toResume, rigidbodyCollider, false);
                Physics.IgnoreCollision(toResume, characterController, false);
            }
        }

        /// <summary>
        /// Positions, sizes and controls all variables necessary to make a body representation follow the given <see cref="BodyRepresentationFacade.Source"/>.
        /// </summary>
        public virtual void Process()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (Interest != MovementInterest.CharacterController && facade.Offset != null)
            {
                Vector3 offsetPosition = facade.Offset.transform.position;
                Vector3 previousPosition = offsetPosition;

                offsetPosition.y = physicsBody.position.y - characterController.skinWidth;

                facade.Offset.transform.position = offsetPosition;
                facade.Source.transform.position += offsetPosition - previousPosition;
            }

            Vector3 previousCharacterControllerPosition;

            // Handle walking down stairs/slopes and physics affecting the Rigidbody in general.
            Vector3 rigidbodyPhysicsMovement = physicsBody.position - previousRigidbodyPosition;
            if (Interest == MovementInterest.Rigidbody || Interest == MovementInterest.RigidbodyUntilGrounded)
            {
                previousCharacterControllerPosition = characterController.transform.position;
                characterController.Move(rigidbodyPhysicsMovement);

                if (facade.Offset != null)
                {
                    Vector3 movement = characterController.transform.position - previousCharacterControllerPosition;
                    facade.Offset.transform.position += movement;
                    facade.Source.transform.position += movement;
                }
            }

            // Position the CharacterController and handle moving the source relative to the offset.
            Vector3 characterControllerPosition = characterController.transform.position;
            previousCharacterControllerPosition = characterControllerPosition;
            MatchCharacterControllerWithSource(false);
            Vector3 characterControllerSourceMovement = characterControllerPosition - previousCharacterControllerPosition;

            bool isGrounded = CheckIfCharacterControllerIsGrounded();

            // Allow moving the Rigidbody via physics.
            if (Interest == MovementInterest.CharacterControllerUntilAirborne && !isGrounded)
            {
                Interest = MovementInterest.RigidbodyUntilGrounded;
            }
            else if (Interest == MovementInterest.RigidbodyUntilGrounded
                && isGrounded
                && rigidbodyPhysicsMovement.sqrMagnitude <= 1E-06F
                && rigidbodySetFrameCount > 0
                && rigidbodySetFrameCount + 1 < Time.frameCount)
            {
                Interest = MovementInterest.CharacterControllerUntilAirborne;
            }

            // Handle walking up stairs/slopes via the CharacterController.
            if (isGrounded && facade.Offset != null && characterControllerSourceMovement.y > 0f)
            {
                facade.Offset.transform.position += Vector3.up * characterControllerSourceMovement.y;
            }

            MatchRigidbodyAndColliderWithCharacterController();

            RememberCurrentPositions();
            EmitIsGroundedChangedEvent(isGrounded);
        }

        /// <summary>
        /// Solves body collisions by not moving the body in case it can't go to its current position.
        /// </summary>
        /// <remarks>
        /// If body collisions should be prevented this method needs to be called right before or right after applying any form of movement to the body.
        /// </remarks>
        public virtual void SolveBodyCollisions()
        {
            if (!isActiveAndEnabled || facade.Source == null)
            {
                return;
            }

            if (offsetObjectFollower != null)
            {
                offsetObjectFollower.Process();
            }

            if (sourceObjectFollower != null)
            {
                sourceObjectFollower.Process();
            }

            Process();
            Vector3 characterControllerPosition = characterController.transform.position + characterController.center;
            Vector3 difference = facade.Source.transform.position - characterControllerPosition;
            difference.y = 0f;

            float minimumDistanceToColliders = characterController.radius - facade.sourceThickness;
            if (difference.magnitude < minimumDistanceToColliders)
            {
                return;
            }

            float newDistance = difference.magnitude - minimumDistanceToColliders;
            (facade.Offset == null ? facade.Source : facade.Offset).transform.position -= difference.normalized * newDistance;
            Process();
        }

        /// <summary>
        /// Configures the source object follower based on the facade settings.
        /// </summary>
        public virtual void ConfigureSourceObjectFollower()
        {
            if (facade.Source != null)
            {
                sourceObjectFollower = facade.Source.GetComponent<ObjectFollower>();
            }
        }

        /// <summary>
        /// Configures the offset object follower based on the facade settings.
        /// </summary>
        public virtual void ConfigureOffsetObjectFollower()
        {
            if (facade.Offset != null)
            {
                offsetObjectFollower = facade.Offset.GetComponent<ObjectFollower>();
            }
        }

        /// <summary>
        /// Ignores all of the colliders on the interactor collection.
        /// </summary>
        public virtual void ConfigureIgnoreInteractorsCollisions()
        {
            foreach (InteractorFacade interactor in facade.ignoredInteractors)
            {
                IgnoreInteractorCollision(interactor);
            }
        }

        protected virtual void Awake()
        {
            Physics.IgnoreCollision(characterController, rigidbodyCollider, true);
        }

        protected virtual void OnEnable()
        {
            ConfigureSourceObjectFollower();
            ConfigureOffsetObjectFollower();
            Interest = MovementInterest.CharacterControllerUntilAirborne;
            MatchCharacterControllerWithSource(true);
            MatchRigidbodyAndColliderWithCharacterController();
            RememberCurrentPositions();
        }

        protected virtual void Start()
        {
            ConfigureIgnoreInteractorsCollisions();
        }

        protected virtual void OnDisable()
        {
            sourceObjectFollower = null;
            offsetObjectFollower = null;
        }

        /// <summary>
        /// Ignores all of the colliders on the given <see cref="InteractorFacade"/>.
        /// </summary>
        /// <param name="interactor">The interactor to ignore.</param>
        protected virtual void IgnoreInteractorCollision(InteractorFacade interactor)
        {
            ignoredGameObjectCollisions.AddToEnd(interactor.gameObject);
            interactor.Grabbed.AddListener(IgnoreInteractorGrabbedCollision);
            interactor.Ungrabbed.AddListener(ResumeInteractorUngrabbedCollision);
        }

        /// <summary>
        /// Ignores the interactable grabbed by the interactor.
        /// </summary>
        /// <param name="interactable">The interactable to ignore.</param>
        protected virtual void IgnoreInteractorGrabbedCollision(InteractableFacade interactable)
        {
            Collider[] interactableColliders = interactable.ConsumerRigidbody.GetComponentsInChildren<Collider>(true);
            foreach (Collider toRestore in interactableColliders)
            {
                if (!ignoredColliders.Contains(toRestore))
                {
                    RestoreColliders.Add(toRestore);
                }
            }
            IgnoreCollisionsWith(interactable.ConsumerRigidbody.gameObject);
        }

        /// <summary>
        /// Resumes the interactable ungrabbed by the interactor.
        /// </summary>
        /// <param name="interactable">The interactable to resume.</param>
        protected virtual void ResumeInteractorUngrabbedCollision(InteractableFacade interactable)
        {
            Collider[] interactableColliders = interactable.ConsumerRigidbody.GetComponentsInChildren<Collider>(true);
            foreach (Collider resumeCollider in interactableColliders)
            {
                if (!RestoreColliders.Contains(resumeCollider))
                {
                    continue;
                }

                ResumeCollisionsWith(resumeCollider);
                RestoreColliders.Remove(resumeCollider);
            }
        }

        /// <summary>
        /// Changes the height and position of <see cref="characterController"/> to match <see cref="BodyRepresentationFacade.Source"/>.
        /// </summary>
        /// <param name="setPositionDirectly">Whether to set the position directly or tell <see cref="characterController"/> to move to it.</param>
        protected virtual void MatchCharacterControllerWithSource(bool setPositionDirectly)
        {
            Vector3 sourcePosition = facade.Source.transform.position;
            float height = facade.Offset == null
                ? sourcePosition.y
                : facade.Offset.transform.InverseTransformPoint(sourcePosition).y;
            height -= characterController.skinWidth;

            // CharacterController enforces a minimum height of twice its radius, so let's match that here.
            height = Mathf.Max(height, 2f * characterController.radius);

            Vector3 position = sourcePosition;
            position.y -= height;

            if (facade.Offset != null)
            {
                // The offset defines the source's "floor".
                position.y = Mathf.Max(position.y, facade.Offset.transform.position.y + characterController.skinWidth);
            }

            if (setPositionDirectly)
            {
                characterController.transform.position = position;
            }
            else
            {
                Vector3 movement = position - characterController.transform.position;
                // The CharacterController doesn't resolve any potential collisions in case we don't move it.
                characterController.Move(movement == Vector3.zero ? movement + collisionResolutionMovement : movement);
                if (movement == Vector3.zero)
                {
                    characterController.Move(movement - collisionResolutionMovement);
                }
            }

            characterController.height = height;

            Vector3 center = characterController.center;
            center.y = height / 2f;
            characterController.center = center;
        }

        /// <summary>
        /// Changes <see cref="rigidbodyCollider"/> to match the collider settings of <see cref="characterController"/> and moves <see cref="PhysicsBody"/> to match <see cref="characterController"/>.
        /// </summary>
        protected virtual void MatchRigidbodyAndColliderWithCharacterController()
        {
            rigidbodyCollider.radius = characterController.radius;
            rigidbodyCollider.height = characterController.height + characterController.skinWidth;

            Vector3 center = characterController.center;
            center.y = (characterController.height - characterController.skinWidth) / 2f;
            rigidbodyCollider.center = center;

            physicsBody.position = characterController.transform.position;
        }

        /// <summary>
        /// Checks whether <see cref="characterController"/> is grounded.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterController.isGrounded"/> isn't accurate so this method does an additional check using <see cref="Physics"/>.
        /// </remarks>
        /// <returns>Whether <see cref="characterController"/> is grounded.</returns>
        protected virtual bool CheckIfCharacterControllerIsGrounded()
        {
            if (characterController.isGrounded)
            {
                return true;
            }

            Collider[] hitColliders = Physics.OverlapSphere(
                characterController.transform.position
                + (Vector3.up * (characterController.radius - characterController.skinWidth - 0.001f)),
                characterController.radius,
                1 << characterController.gameObject.layer);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider != characterController
                    && hitCollider != rigidbodyCollider
                    && !ignoredColliders.Contains(hitCollider)
                    && !Physics.GetIgnoreLayerCollision(
                        hitCollider.gameObject.layer,
                        characterController.gameObject.layer)
                    && !Physics.GetIgnoreLayerCollision(hitCollider.gameObject.layer, physicsBody.gameObject.layer))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the previous position variables to remember the current state.
        /// </summary>
        protected virtual void RememberCurrentPositions()
        {
            previousRigidbodyPosition = physicsBody.position;
        }

        /// <summary>
        /// Emits <see cref="BodyRepresentationFacade.BecameGrounded"/> or <see cref="BodyRepresentationFacade.BecameAirborne"/>.
        /// </summary>
        /// <param name="isCharacterControllerGrounded">The current state.</param>
        protected virtual void EmitIsGroundedChangedEvent(bool isCharacterControllerGrounded)
        {
            if (wasCharacterControllerGrounded == isCharacterControllerGrounded)
            {
                return;
            }

            wasCharacterControllerGrounded = isCharacterControllerGrounded;

            if (isCharacterControllerGrounded)
            {
                facade.BecameGrounded?.Invoke();
            }
            else
            {
                facade.BecameAirborne?.Invoke();
            }
        }
    }
}