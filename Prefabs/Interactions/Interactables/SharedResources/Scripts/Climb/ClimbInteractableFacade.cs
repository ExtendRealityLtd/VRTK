namespace VRTK.Prefabs.Interactions.Interactables.Climb
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using VRTK.Prefabs.Locomotion.Movement.Climb;

    /// <summary>
    /// The public interface for the Interactable.Climbable prefab.
    /// </summary>
    public class ClimbInteractableFacade : MonoBehaviour
    {
        #region Climb Settings
        /// <summary>
        /// The <see cref="ClimbFacade"/> to use.
        /// </summary>
        [Serialized]
        [field: Header("Climb Settings"), DocumentedByXml]
        public ClimbFacade ClimbFacade { get; set; }
        /// <summary>
        /// The multiplier to apply to the velocity of the interactor when the interactable is released and climbing stops.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public Vector3 ReleaseMultiplier { get; set; } = Vector3.one;
        #endregion
    }
}