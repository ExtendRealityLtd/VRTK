namespace VRTK.Prefabs.Locomotion.Movement.Climb
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Locomotion.BodyRepresentation;
    using Malimbe.BehaviourStateRequirementMethod;

    /// <summary>
    /// The public interface for the Climb prefab.
    /// </summary>
    public class ClimbFacade : MonoBehaviour
    {
        #region Control Settings
        /// <summary>
        /// The body representation to control.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Control Settings"), DocumentedByXml]
        public BodyRepresentationFacade BodyRepresentationFacade { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Emitted when a climb starts.
        /// </summary>
        [Header("Events"), DocumentedByXml]
        public UnityEvent ClimbStarted = new UnityEvent();
        /// <summary>
        /// Emitted when the climb stops.
        /// </summary>
        [DocumentedByXml]
        public UnityEvent ClimbStopped = new UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// Applies velocity when releasing from climb.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public ClimbVelocityApplier VelocityApplier { get; protected set; }
        #endregion

        /// <summary>
        /// The current source of the movement. The body will be moved in reverse direction in case this object moves.
        /// </summary>
        public GameObject CurrentInteractor => Interactors.Count == 0 ? null : Interactors[Interactors.Count - 1];
        /// <summary>
        /// The current optional offset of the movement. The body will be moved in case this object moves.
        /// </summary>
        public GameObject CurrentInteractable => Interactables.Count == 0 ? null : Interactables[Interactors.Count - 1];
        /// <summary>
        /// Whether a climb is happening right now.
        /// </summary>
        public bool IsClimbing => Interactors.Count > 0 || Interactables.Count > 0;

        /// <summary>
        /// The objects that define the source of movement in order they should be used. The last object defines <see cref="CurrentInteractor"/>.
        /// </summary>
        public IReadOnlyList<GameObject> Interactors => VelocityApplier.Interactors.NonSubscribableElements;
        /// <summary>
        /// The objects that define the optional offsets of movement in order they should be used. The last object defines <see cref="CurrentInteractable"/>.
        /// </summary>
        public IReadOnlyList<GameObject> Interactables => VelocityApplier.Interactables.NonSubscribableElements;

        /// <summary>
        /// Adds a source of movement for the body.
        /// </summary>
        /// <param name="interactor">The object to use as a source of the movement.</param>
        [RequiresBehaviourState]
        public virtual void AddInteractor(GameObject interactor)
        {
            VelocityApplier.Interactors.Add(interactor);
        }

        /// <summary>
        /// Removes a source of movement for the body.
        /// </summary>
        /// <param name="interactor">The object used as a source of the movement.</param>
        [RequiresBehaviourState]
        public virtual void RemoveInteractor(GameObject interactor)
        {
            if (!VelocityApplier.Interactors.Contains(interactor))
            {
                return;
            }

            VelocityApplier.Interactors.RemoveLastOccurrence(interactor);
            VelocityApplier.ApplyVelocity();
        }

        /// <summary>
        /// Clears the sources of the movement.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void ClearInteractors()
        {
            VelocityApplier.Interactors.Clear();
        }

        /// <summary>
        /// Adds an optional offset of movement for the body.
        /// </summary>
        /// <param name="interactable">The object to use as an optional offset of the movement.</param>
        [RequiresBehaviourState]
        public virtual void AddInteractable(GameObject interactable)
        {
            VelocityApplier.Interactables.Add(interactable);
        }

        /// <summary>
        /// Removes an optional offset of movement for the body.
        /// </summary>
        /// <param name="interactable">The object used as an optional offset of the movement.</param>
        [RequiresBehaviourState]
        public virtual void RemoveInteractable(GameObject interactable)
        {
            VelocityApplier.Interactables.RemoveLastOccurrence(interactable);
        }

        /// <summary>
        /// Clears the optional offsets of the movement.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void ClearInteractables()
        {
            VelocityApplier.Interactables.Clear();
        }

        /// <summary>
        /// Sets a source to track the velocity from.
        /// </summary>
        /// <param name="source">The tracked velocity source.</param>
        public virtual void SetVelocitySource(GameObject source)
        {
            VelocityApplier.VelocityProxy.ProxySource = source;
        }

        /// <summary>
        /// Sets the multiplier to apply to any tracked velocity.
        /// </summary>
        /// <param name="multiplier">The multiplier to apply to tracked velocity.</param>
        public virtual void SetVelocityMultiplier(Vector3 multiplier)
        {
            VelocityApplier.VelocityMultiplier.Collection.SetAt(multiplier, 1);
            VelocityApplier.VelocityMultiplier.Collection.CurrentIndex = 0;
        }

        /// <summary>
        /// Called after <see cref="BodyRepresentationFacade"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(BodyRepresentationFacade))]
        protected virtual void OnAfterBodyRepresentationFacadeChange()
        {
            VelocityApplier.ConfigureTargetPositionProperty();
        }
    }
}