namespace VRTK.Prefabs.Locomotion.Movement.AxesToVector3
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using Malimbe.MemberChangeMethod;
    using Malimbe.MemberClearanceMethod;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Zinnia.Action;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The public interface for the AxisSlide prefab.
    /// </summary>
    public class AxesToVector3Facade : MonoBehaviour
    {
        /// <summary>
        /// Defines the event with the multiplied <see cref="Vector3"/> value.
        /// </summary>
        [Serializable]
        public class UnityEvent : UnityEvent<Vector3>
        {
        }

        /// <summary>
        /// The way to use the given axis data.
        /// </summary>
        public enum AxisUsage
        {
            /// <summary>
            /// The processed output is a continuous incremental value based on the axis input.
            /// </summary>
            Incremental,
            /// <summary>
            /// The processed output is a specific direction value only outputted once per axis change.
            /// </summary>
            Directional
        }

        #region Axis Settings
        /// <summary>
        /// The <see cref="AxisUsage"/> to utilize when applying the axis input.
        /// </summary>
        [Serialized]
        [field: Header("Axis Settings"), DocumentedByXml]
        public AxisUsage AxisUsageType { get; set; }
        /// <summary>
        /// The <see cref="FloatAction"/> to get the lateral (left/right direction) data from.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public FloatAction LateralAxis { get; set; }
        /// <summary>
        /// The <see cref="FloatAction"/> to get the longitudinal (forward/backward direction) data from.
        /// </summary>
        [Serialized, Cleared]
        [field: DocumentedByXml]
        public FloatAction LongitudinalAxis { get; set; }
        /// <summary>
        /// The multiplier to apply to the lateral axis.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float LateralSpeedMultiplier { get; set; } = 1f;
        /// <summary>
        /// The multiplier to apply to the longitudinal axis.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float LongitudinalSpeedMultiplier { get; set; } = 1f;
        #endregion

        #region Direction Settings
        /// <summary>
        /// The source of the forward direction to move towards.
        /// </summary>
        [Serialized, Cleared]
        [field: Header("Direction Settings"), DocumentedByXml]
        public GameObject SourceOfForwardDirection { get; set; }
        #endregion

        #region Events
        /// <summary>
        /// Emitted when the axes are converted into a <see cref="Vector3"/>.
        /// </summary>
        [Header("Events"), DocumentedByXml]
        public UnityEvent Processed = new UnityEvent();
        #endregion

        #region Reference Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public AxesToVector3Processor Processor { get; protected set; }
        #endregion

        /// <summary>
        /// Called after <see cref="AxisUsageType"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(AxisUsageType))]
        protected virtual void OnAfterAxisUsageTypeChange()
        {
            Processor.ConfigureAxisUsageType();
        }

        /// <summary>
        /// Called after <see cref="LateralAxis"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LateralAxis))]
        protected virtual void OnAfterLateralAxisChange()
        {
            Processor.ConfigureAxisSources();
        }

        /// <summary>
        /// Called after <see cref="LongitudinalAxis"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LongitudinalAxis))]
        protected virtual void OnAfterLongitudinalAxisChange()
        {
            Processor.ConfigureAxisSources();
        }

        /// <summary>
        /// Called after <see cref="LateralSpeedMultiplier"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LateralSpeedMultiplier))]
        protected virtual void OnAfterLateralSpeedMultiplierChange()
        {
            Processor.ConfigureMultipliers();
        }

        /// <summary>
        /// Called after <see cref="LongitudinalSpeedMultiplier"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(LongitudinalSpeedMultiplier))]
        protected virtual void OnAfterLongitudinalSpeedMultiplierChange()
        {
            Processor.ConfigureMultipliers();
        }
    }
}