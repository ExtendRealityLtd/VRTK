namespace VRTK.Prefabs.Interactions.Interactables.Grab.Action
{
    using UnityEngine;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Modification;

    /// <summary>
    /// Describes an action that allows the Interactable to be scaled in size between the points of two specified Interactors.
    /// </summary>
    public class GrabInteractableScaleAction : GrabInteractableAction
    {
        #region Interactable Settings
        /// <summary>
        /// The <see cref="PinchScaler"/> to process the scale control.
        /// </summary>
        [Header("Interactable Settings"), Tooltip("The PinchScaler to process the scale control."), InternalSetting, SerializeField]
        protected PinchScaler pinchScaler;
        #endregion

        /// <inheritdoc />
        public override void SetUp(GrabInteractableInternalSetup grabSetup)
        {
            base.SetUp(grabSetup);
            pinchScaler.SetTarget(grabSetup.Facade.ConsumerContainer);
        }
    }
}