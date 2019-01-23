namespace VRTK.Prefabs.Interactions.Interactables.Grab.Action
{
    using UnityEngine;
    using System.Collections.Generic;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Modification;

    /// <summary>
    /// Describes an action that allows the Interactable to point in the direction of a given Interactor.
    /// </summary>
    public class GrabInteractableControlDirectionAction : GrabInteractableAction
    {
        #region Interactable Settings
        /// <summary>
        /// A <see cref="GameObject"/> collection to enable/disabled as part of the direction control process.
        /// </summary>
        [Header("Interactable Settings"), Tooltip("A GameObject collection to enable/disabled as part of the direction control process."), SerializeField]
        protected List<GameObject> linkedObjects = new List<GameObject>();
        /// <summary>
        /// The <see cref="DirectionModifier"/> to process the direction control.
        /// </summary>
        [Tooltip("The DirectionModifier to process the direction control."), InternalSetting, SerializeField]
        protected DirectionModifier directionModifier;
        #endregion

        /// <inheritdoc />
        public override void SetUp(GrabInteractableInternalSetup grabSetup)
        {
            base.SetUp(grabSetup);
            directionModifier.SetTarget(grabSetup.Facade.ConsumerContainer);
        }

        /// <summary>
        /// Enables the <see cref="GameObject"/> state of each of the items in the <see cref="linkedObjects"/> collection.
        /// </summary>
        public virtual void EnableLinkedObjects()
        {
            ToggleLinkedObjectState(true);
        }

        /// <summary>
        /// Disables the <see cref="GameObject"/> state of each of the items in the <see cref="linkedObjects"/> collection.
        /// </summary>
        public virtual void DisableLinkedObjects()
        {
            ToggleLinkedObjectState(false);
        }

        /// <summary>
        /// Toggles the <see cref="GameObject"/> state of each of the items in the <see cref="linkedObjects"/> collection.
        /// </summary>
        /// <param name="state">The state to set the <see cref="GameObject"/> active state to.</param>
        protected virtual void ToggleLinkedObjectState(bool state)
        {
            foreach (GameObject linkedObject in linkedObjects)
            {
                linkedObject.SetActive(state);
            }
        }
    }
}