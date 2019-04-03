namespace VRTK.Prefabs.Locomotion.BodyRepresentation
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using Malimbe.BehaviourStateRequirementMethod;
    using Malimbe.MemberChangeMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Cast;
    using Zinnia.Data.Attribute;
    using Zinnia.Data.Type;
    using Zinnia.Extension;
    using Zinnia.Process;
    using Zinnia.Tracking.Collision;
    using Zinnia.Tracking.Follow;
    using VRTK.Prefabs.Interactions.Interactables;
    using VRTK.Prefabs.Interactions.Interactors;

    /// <summary>
    /// Sets up the BodyRepresentation prefab based on the provided user settings and implements the logic to represent a body.
    /// </summary>
    public class BodyRepresentationProcessor : MonoBehaviour, IProcessable
    {
        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public enum MovementInterest
        {
            /// <summary>
            /// The source of truth for movement comes from <see cref="BodyRepresentationProcessor.Character"/>.
            /// </summary>
            CharacterController,
            /// <summary>
            /// The source of truth for movement comes from <see cref="BodyRepresentationProcessor.Character"/> until <see cref="rigidbody"/> is in the air, then <see cref="rigidbody"/> is the new source of truth.
            /// </summary>
            CharacterControllerUntilAirborne,
            /// <summary>
            /// The source of truth for movement comes from <see cref="rigidbody"/>.
            /// </summary>
            Rigidbody,
            /// <summary>
            /// The source of truth for movement comes from <see cref="rigidbody"/> until <see cref="BodyRepresentationProcessor.Character"/> hits the ground, then <see cref="BodyRepresentationProcessor.Character"/> is the new source of truth.
            /// </summary>
            RigidbodyUntilGrounded
        }

        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public BodyRepresentationFacade Facade { get; protected set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The <see cref="UnityEngine.CharacterController"/> that acts as the main representation of the body.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public CharacterController Character { get; protected set; }
        /// <summary>
        /// The <see cref="Rigidbody"/> that acts as the physical representation of the body.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Rigidbody PhysicsBody { get; protected set; }
        /// <summary>
        /// The <see cref="CapsuleCollider"/> that acts as the physical collider representation of the body.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public CapsuleCollider RigidbodyCollider { get; protected set; }
        /// <summary>
        /// A <see cref="CollisionIgnorer"/> to manage ignoring collisions with the BodyRepresentation colliders.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public CollisionIgnorer CollisionsToIgnore { get; protected set; }
        #endregion

        /// <summary>
        /// The object that defines the main source of truth for movement.
        /// </summary>
        public MovementInterest Interest { get; set; } = MovementInterest.CharacterControllerUntilAirborne;
        /// <summary>
        /// Whether <see cref="Character"/> touches ground.
        /// </summary>
        public bool IsCharacterControllerGrounded => wasCharacterControllerGrounded == true;

        /// <summary>
        /// Movement to apply to <see cref="Character"/> to resolve collisions.
        /// </summary>
        protected static readonly Vector3 collisionResolutionMovement = new Vector3(0.001f, 0f, 0f);
        /// <summary>
        /// The colliders to ignore body collisions with.
        /// </summary>
        protected readonly HashSet<Collider> ignoredColliders = new HashSet<Collider>();
        /// <summary>
        /// The colliders to restore after an ungrab.
        /// </summary>
        protected readonly HashSet<Collider> restoreColliders = new HashSet<Collider>();
        /// <summary>
        /// The previous position of <see cref="PhysicsBody"/>.
        /// </summary>
        protected Vector3 previousRigidbodyPosition;
        /// <summary>
        /// Whether <see cref="Character"/> was grounded previously.
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
        /// Positions, sizes and controls all variables necessary to make a body representation follow the given <see cref="BodyRepresentationFacade.Source"/>.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void Process()
        {
            if (Interest != MovementInterest.CharacterController && Facade.Offset != null)
            {
                Vector3 offsetPosition = Facade.Offset.transform.position;
                Vector3 previousPosition = offsetPosition;

                offsetPosition.y = PhysicsBody.position.y - Character.skinWidth;

                Facade.Offset.transform.position = offsetPosition;
                Facade.Source.transform.position += offsetPosition - previousPosition;
            }

            Vector3 previousCharacterControllerPosition;

            // Handle walking down stairs/slopes and physics affecting the Rigidbody in general.
            Vector3 rigidbodyPhysicsMovement = PhysicsBody.position - previousRigidbodyPosition;
            if (Interest == MovementInterest.Rigidbody || Interest == MovementInterest.RigidbodyUntilGrounded)
            {
                previousCharacterControllerPosition = Character.transform.position;
                Character.Move(rigidbodyPhysicsMovement);

                if (Facade.Offset != null)
                {
                    Vector3 movement = Character.transform.position - previousCharacterControllerPosition;
                    Facade.Offset.transform.position += movement;
                    Facade.Source.transform.position += movement;
                }
            }

            // Position the CharacterController and handle moving the source relative to the offset.
            Vector3 characterControllerPosition = Character.transform.position;
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
            if (isGrounded && Facade.Offset != null && characterControllerSourceMovement.y > 0f)
            {
                Facade.Offset.transform.position += Vector3.up * characterControllerSourceMovement.y;
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
        [RequiresBehaviourState]
        public virtual void SolveBodyCollisions()
        {
            if (Facade.Source == null)
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
            Vector3 characterControllerPosition = Character.transform.position + Character.center;
            Vector3 difference = Facade.Source.transform.position - characterControllerPosition;
            difference.y = 0f;

            float minimumDistanceToColliders = Character.radius - Facade.SourceThickness;
            if (difference.magnitude < minimumDistanceToColliders)
            {
                return;
            }

            float newDistance = difference.magnitude - minimumDistanceToColliders;
            (Facade.Offset == null ? Facade.Source : Facade.Offset).transform.position -= difference.normalized * newDistance;
            Process();
        }

        /// <summary>
        /// Configures the source object follower based on the facade settings.
        /// </summary>
        public virtual void ConfigureSourceObjectFollower()
        {
            if (Facade.Source != null)
            {
                sourceObjectFollower = Facade.Source.GetComponent<ObjectFollower>();
            }
        }

        /// <summary>
        /// Configures the offset object follower based on the facade settings.
        /// </summary>
        public virtual void ConfigureOffsetObjectFollower()
        {
            if (Facade.Offset != null)
            {
                offsetObjectFollower = Facade.Offset.GetComponent<ObjectFollower>();
            }
        }

        /// <summary>
        /// Ignores all of the colliders on the interactor collection.
        /// </summary>
        public virtual void IgnoreInteractorsCollisions(InteractorFacade interactor)
        {
            CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.AddUnique(interactor.gameObject));
            interactor.Grabbed.AddListener(IgnoreInteractorGrabbedCollision);
            interactor.Ungrabbed.AddListener(ResumeInteractorUngrabbedCollision);
        }

        /// <summary>
        /// Resumes all of the colliders on the interactor collection.
        /// </summary>
        public virtual void ResumeInteractorsCollisions(InteractorFacade interactor)
        {
            CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.Remove(interactor.gameObject));
            interactor.Grabbed.RemoveListener(IgnoreInteractorGrabbedCollision);
            interactor.Ungrabbed.RemoveListener(ResumeInteractorUngrabbedCollision);
        }

        protected virtual void Awake()
        {
            Physics.IgnoreCollision(Character, RigidbodyCollider, true);
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

        protected virtual void OnDisable()
        {
            sourceObjectFollower = null;
            offsetObjectFollower = null;
        }

        /// <summary>
        /// Ignores the interactable grabbed by the interactor.
        /// </summary>
        /// <param name="interactable">The interactable to ignore.</param>
        protected virtual void IgnoreInteractorGrabbedCollision(InteractableFacade interactable)
        {
            CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.AddUnique(interactable.gameObject));
        }

        /// <summary>
        /// Resumes the interactable ungrabbed by the interactor.
        /// </summary>
        /// <param name="interactable">The interactable to resume.</param>
        protected virtual void ResumeInteractorUngrabbedCollision(InteractableFacade interactable)
        {
            CollisionsToIgnore.RunWhenActiveAndEnabled(() => CollisionsToIgnore.Targets.Remove(interactable.gameObject));
        }

        /// <summary>
        /// Changes the height and position of <see cref="Character"/> to match <see cref="BodyRepresentationFacade.Source"/>.
        /// </summary>
        /// <param name="setPositionDirectly">Whether to set the position directly or tell <see cref="Character"/> to move to it.</param>
        protected virtual void MatchCharacterControllerWithSource(bool setPositionDirectly)
        {
            Vector3 sourcePosition = Facade.Source.transform.position;
            float height = Facade.Offset == null
                ? sourcePosition.y
                : Facade.Offset.transform.InverseTransformPoint(sourcePosition).y;
            height -= Character.skinWidth;

            // CharacterController enforces a minimum height of twice its radius, so let's match that here.
            height = Mathf.Max(height, 2f * Character.radius);

            Vector3 position = sourcePosition;
            position.y -= height;

            if (Facade.Offset != null)
            {
                // The offset defines the source's "floor".
                position.y = Mathf.Max(position.y, Facade.Offset.transform.position.y + Character.skinWidth);
            }

            if (setPositionDirectly)
            {
                Character.transform.position = position;
            }
            else
            {
                Vector3 movement = position - Character.transform.position;
                // The CharacterController doesn't resolve any potential collisions in case we don't move it.
                Character.Move(movement == Vector3.zero ? movement + collisionResolutionMovement : movement);
                if (movement == Vector3.zero)
                {
                    Character.Move(movement - collisionResolutionMovement);
                }
            }

            Character.height = height;

            Vector3 center = Character.center;
            center.y = height / 2f;
            Character.center = center;
        }

        /// <summary>
        /// Changes <see cref="RigidbodyCollider"/> to match the collider settings of <see cref="Character"/> and moves <see cref="PhysicsBody"/> to match <see cref="Character"/>.
        /// </summary>
        protected virtual void MatchRigidbodyAndColliderWithCharacterController()
        {
            RigidbodyCollider.radius = Character.radius;
            RigidbodyCollider.height = Character.height + Character.skinWidth;

            Vector3 center = Character.center;
            center.y = (Character.height - Character.skinWidth) / 2f;
            RigidbodyCollider.center = center;

            PhysicsBody.position = Character.transform.position;
        }

        /// <summary>
        /// Checks whether <see cref="Character"/> is grounded.
        /// </summary>
        /// <remarks>
        /// <see cref="CharacterController.isGrounded"/> isn't accurate so this method does an additional check using <see cref="Physics"/>.
        /// </remarks>
        /// <returns>Whether <see cref="Character"/> is grounded.</returns>
        protected virtual bool CheckIfCharacterControllerIsGrounded()
        {
            if (Character.isGrounded)
            {
                return true;
            }

            HeapAllocationFreeReadOnlyList<Collider> hitColliders = PhysicsCast.OverlapSphereAll(
                null,
                Character.transform.position + (Vector3.up * (Character.radius - Character.skinWidth - 0.001f)),
                Character.radius,
                1 << Character.gameObject.layer);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider != Character
                    && hitCollider != RigidbodyCollider
                    && !ignoredColliders.Contains(hitCollider)
                    && !Physics.GetIgnoreLayerCollision(
                        hitCollider.gameObject.layer,
                        Character.gameObject.layer)
                    && !Physics.GetIgnoreLayerCollision(hitCollider.gameObject.layer, PhysicsBody.gameObject.layer))
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
            previousRigidbodyPosition = PhysicsBody.position;
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
                Facade.BecameGrounded?.Invoke();
            }
            else
            {
                Facade.BecameAirborne?.Invoke();
            }
        }

        /// <summary>
        /// Called after <see cref="Interest"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Interest))]
        protected virtual void OnAfterInterestChange()
        {
            switch (Interest)
            {
                case MovementInterest.CharacterController:
                case MovementInterest.CharacterControllerUntilAirborne:
                    PhysicsBody.isKinematic = true;
                    rigidbodySetFrameCount = 0;
                    break;
                case MovementInterest.Rigidbody:
                case MovementInterest.RigidbodyUntilGrounded:
                    PhysicsBody.isKinematic = false;
                    rigidbodySetFrameCount = Time.frameCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Interest), Interest, null);
            }
        }
    }
}