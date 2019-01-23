namespace VRTK.Prefabs.Interactions.Interactables
{
    using UnityEngine;
    using Zinnia.Data.Attribute;
    using VRTK.Prefabs.Locomotion.Movement.Climb;
    using VRTK.Prefabs.Interactions.Interactables.Climb;

    /// <summary>
    /// The public interface for the Interactable.Climbable prefab.
    /// </summary>
    public class ClimbInteractableFacade : MonoBehaviour
    {
        #region Climb Settings
        [Header("Climb Settings"), Tooltip("The Climb Facade to use."), SerializeField]
        private ClimbFacade _climbFacade = null;
        /// <summary>
        /// The <see cref="ClimbFacade"/> to use.
        /// </summary>
        public ClimbFacade ClimbFacade => _climbFacade;
        /// <summary>
        /// The multiplier to apply to the velocity of the interactor when the interactable is released and climbing stops.
        /// </summary>
        [Tooltip("The multiplier to apply to the velocity of the interactor when the interactable is released and climbing stops.")]
        public Vector3 releaseMultiplier = Vector3.one;
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected ClimbInteractableInternalSetup internalSetup;
        #endregion
    }
}