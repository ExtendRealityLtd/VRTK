namespace VRTK.Prefabs.Locomotion.Movement.SpatialManipulation
{
    using UnityEngine;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Zinnia.Data.Attribute;
    using Malimbe.MemberChangeMethod;
    using Zinnia.Action;

    /// <summary>
    /// The public interface for the Drag.Rotate.Scale Spatial Manipulator prefab.
    /// </summary>
    public class SpatialManipulatorFacade : MonoBehaviour
    {
        #region Object Settings
        /// <summary>
        /// The primary source to track positional and rotational data on to apply to the manipulator.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Object Settings"), DocumentedByXml]
        public GameObject PrimarySource { get; set; }
        /// <summary>
        /// The secondary source to track positional and rotational data on to apply to the manipulator.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject SecondarySource { get; set; }
        /// <summary>
        /// The target to apply the spatial manipulation to.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Target { get; set; }
        /// <summary>
        /// An optional offset to take into consideration when manipulating the target.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public GameObject Offset { get; set; }
        #endregion

        #region Activation Settings
        /// <summary>
        /// The Action to engage the position manipulation.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Activation Settings"), DocumentedByXml]
        public BooleanAction ActivatePositionManipulation { get; set; }
        /// <summary>
        /// The Action to engage the rotation manipulation.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public BooleanAction ActivateRotationManipulation { get; set; }
        /// <summary>
        /// The Action to engage the scale manipulation.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public BooleanAction ActivateScaleManipulation { get; set; }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The manipulator that handles position.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public SpatialManipulator PositionManipulator { get; protected set; }
        /// <summary>
        /// The manipulator that handles rotation.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public SpatialManipulator RotationManipulator { get; protected set; }
        /// <summary>
        /// The manipulator that handles scale.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public SpatialManipulator ScaleManipulator { get; protected set; }
        #endregion

        protected virtual void OnEnable()
        {
            SetReferenceSettings();
            ScaleManipulator.ActivationAction = ActivateScaleManipulation;
            RotationManipulator.ActivationAction = ActivateRotationManipulation;
            PositionManipulator.ActivationAction = ActivatePositionManipulation;
        }

        protected virtual void SetReferenceSettings()
        {
            SetObjectSettings(ScaleManipulator);
            SetObjectSettings(RotationManipulator);
            SetObjectSettings(PositionManipulator);
        }

        protected virtual void SetObjectSettings(SpatialManipulator manipulator)
        {
            manipulator.PrimarySource = PrimarySource;
            manipulator.SecondarySource = SecondarySource;
            manipulator.Target = Target;
            manipulator.Offset = Offset;
        }

        /// <summary>
        /// Called after <see cref="PrimarySource"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(PrimarySource))]
        protected virtual void OnAfterPrimarySourceChange()
        {
            SetReferenceSettings();
        }

        /// <summary>
        /// Called after <see cref="SecondarySource"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(SecondarySource))]
        protected virtual void OnAfterSecondarySourceChange()
        {
            SetReferenceSettings();
        }

        /// <summary>
        /// Called after <see cref="Target"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Target))]
        protected virtual void OnAfterTargetChange()
        {
            SetReferenceSettings();
        }

        /// <summary>
        /// Called after <see cref="Offset"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(Offset))]
        protected virtual void OnAfterOffsetChange()
        {
            SetReferenceSettings();
        }

        /// <summary>
        /// Called after <see cref="ActivateScaleManipulation"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ActivateScaleManipulation))]
        protected virtual void OnAfterActivateScaleManipulationChange()
        {
            ScaleManipulator.ActivationAction = ActivateScaleManipulation;
        }

        /// <summary>
        /// Called after <see cref="ActivateRotationManipulation"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ActivateRotationManipulation))]
        protected virtual void OnAfterActivateRotationManipulationChange()
        {
            RotationManipulator.ActivationAction = ActivateRotationManipulation;
        }

        /// <summary>
        /// Called after <see cref="ActivatePositionManipulation"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(ActivatePositionManipulation))]
        protected virtual void OnAfterActivatePositionManipulationChange()
        {
            PositionManipulator.ActivationAction = ActivatePositionManipulation;
        }
    }
}