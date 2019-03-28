namespace VRTK.Prefabs.PlayAreaRepresentation
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface into the PlayAreaRepresentation Prefab.
    /// </summary>
    public class PlayAreaRepresentationFacade : MonoBehaviour
    {
        #region Target Settings
        /// <summary>
        /// The target to represent the PlayArea.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Target Settings"), DocumentedByXml]
        public GameObject Target { get; set; }

        /// <summary>
        /// An optional origin to use in a position offset calculation.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject OffsetOrigin { get; set; }

        /// <summary>
        /// An optional destination to use in a position offset calculation.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject OffsetDestination { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public PlayAreaRepresentationConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Recalculates the PlayArea dimensions.
        /// </summary>
        public virtual void Recalculate()
        {
            Configuration.RecalculateDimensions();
        }

        /// <summary>
        /// Called after <see cref="Target"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Target))]
        protected virtual void OnAfterTargetChange()
        {
            Configuration.ConfigureTarget();
        }

        /// <summary>
        /// Called after <see cref="OffsetOrigin"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(OffsetOrigin))]
        protected virtual void OnAfterOffsetOriginChange()
        {
            Configuration.ConfigureOffsetOrigin();
        }

        /// <summary>
        /// Called after <see cref="OffsetDestination"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(OffsetDestination))]
        protected virtual void OnAfterOffsetDestinationChange()
        {
            Configuration.ConfigureOffsetDestination();
        }
    }
}