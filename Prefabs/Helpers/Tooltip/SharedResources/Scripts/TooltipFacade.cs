namespace VRTK.Prefabs.Helpers.Tooltip
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the TooltipFacade prefab.
    /// </summary>
    public class TooltipFacade : MonoBehaviour
    {
        #region Tooltip Settings
        /// <summary>
        /// The object that the tooltip will face towards.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Tooltip Settings"), DocumentedByXml]
        public GameObject FacingSource { get; set; }

        /// <summary>
        /// The target to draw the tooltip line to.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject LineTarget { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public TooltipConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Called after <see cref="LineTarget"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LineTarget))]
        protected virtual void OnAfterLineTargetChange()
        {
            Configuration.SetLine(LineTarget);
        }
    }
}