namespace VRTK.Prefabs.Helpers.AxisRotator
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the AxisRotator prefab.
    /// </summary>
    public class AxisRotatorFacade : MonoBehaviour
    {
        #region Axis Settings
        /// <summary>
        /// The <see cref="FloatAction"/> to get the lateral (left/right direction) data from.
        /// </summary>
        [Serialized]
        [field: Header("Axis Settings"), DocumentedByXml]
        public FloatAction LateralAxis { get; set; }
        /// <summary>
        /// The <see cref="FloatAction"/> to get the longitudinal (forward/backward direction) data from.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public FloatAction LongitudinalAxis { get; set; }
        #endregion

        #region Target Settings
        /// <summary>
        /// The target to rotate.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Target Settings"), DocumentedByXml]
        public GameObject Target { get; set; }
        /// <summary>
        /// The direction offset to use when considering the rotation origin.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject DirectionOffset { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public AxisRotatorConfigurator Configuration { get; protected set; }
        #endregion

        /// <summary>
        /// Called after <see cref="LateralAxis"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LateralAxis))]
        protected virtual void OnAfterLateralAxisChange()
        {
            Configuration.SetAxisSources();
        }

        /// <summary>
        /// Called after <see cref="LongitudinalAxis"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LongitudinalAxis))]
        protected virtual void OnAfterLongitudinalAxisChange()
        {
            Configuration.SetAxisSources();
        }

        /// <summary>
        /// Called after <see cref="Target"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Target))]
        protected virtual void OnAfterTargetChange()
        {
            Configuration.SetMutator();
        }

        /// <summary>
        /// Called after <see cref="DirectionOffset"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(DirectionOffset))]
        protected virtual void OnAfterDirectionOffsetChange()
        {
            Configuration.SetExtractor();
        }
    }
}