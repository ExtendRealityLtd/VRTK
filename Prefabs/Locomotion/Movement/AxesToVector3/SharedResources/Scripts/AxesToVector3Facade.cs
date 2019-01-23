namespace VRTK.Prefabs.Locomotion.Movement.AxesToVector3
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
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
        [Header("Axis Settings"), Tooltip("The AxisUsage to utilize when applying the axis input."), SerializeField]
        private AxisUsage _axisUsageType = AxisUsage.Incremental;
        /// <summary>
        /// The <see cref="AxisUsage"/> to utilize when applying the axis input.
        /// </summary>
        public AxisUsage AxisUsageType
        {
            get
            {
                return _axisUsageType;
            }
            set
            {
                _axisUsageType = value;
                internalSetup.SetAxisUsageType();
            }
        }

        [Tooltip("The FloatAction to get the lateral (left/right direction) data from."), SerializeField]
        private FloatAction _lateralAxis;
        /// <summary>
        /// The <see cref="FloatAction"/> to get the lateral (left/right direction) data from.
        /// </summary>
        public FloatAction LateralAxis
        {
            get
            {
                return _lateralAxis;
            }
            set
            {
                _lateralAxis = value;
                internalSetup.SetAxisSources();
            }
        }

        [Tooltip("The FloatAction to get the longitudinal (forward/backward direction) data from."), SerializeField]
        private FloatAction _longitudinalAxis;
        /// <summary>
        /// The <see cref="FloatAction"/> to get the longitudinal (forward/backward direction) data from.
        /// </summary>
        public FloatAction LongitudinalAxis
        {
            get
            {
                return _longitudinalAxis;
            }
            set
            {
                _longitudinalAxis = value;
                internalSetup.SetAxisSources();
            }
        }

        [Tooltip("The multiplier to apply to the lateral axis."), SerializeField]
        private float _lateralSpeedMultiplier = 1f;
        /// <summary>
        /// The multiplier to apply to the lateral axis.
        /// </summary>
        public float LateralSpeedMultiplier
        {
            get
            {
                return _lateralSpeedMultiplier;
            }
            set
            {
                _lateralSpeedMultiplier = value;
                internalSetup.SetMultipliers();
            }
        }

        [Tooltip("The multiplier to apply to the longitudinal axis."), SerializeField]
        private float _longitudinalSpeedMultiplier = 1f;
        /// <summary>
        /// The multiplier to apply to the longitudinal axis.
        /// </summary>
        public float LongitudinalSpeedMultiplier
        {
            get
            {
                return _longitudinalSpeedMultiplier;
            }
            set
            {
                _longitudinalSpeedMultiplier = value;
                internalSetup.SetMultipliers();
            }
        }
        #endregion

        #region Reference Settings
        /// <summary>
        /// The source of the forward direction to move towards.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The source of the forward direction to move towards.")]
        public GameObject sourceOfForwardDirection;
        #endregion

        #region Events
        /// <summary>
        /// Emitted when the axes are converted into a <see cref="Vector3"/>.
        /// </summary>
        [Header("Events"), Tooltip("Emitted when the axes are converted into a Vector3.")]
        public UnityEvent Processed = new UnityEvent();
        #endregion

        #region Internal Settings
        /// <summary>
        /// The linked Internal Setup.
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked Internal Setup."), InternalSetting, SerializeField]
        protected AxesToVector3InternalSetup internalSetup;
        #endregion

        protected virtual void OnValidate()
        {
            if (!isActiveAndEnabled || !Application.isPlaying)
            {
                return;
            }

            internalSetup.SetAxisSources();
            internalSetup.SetMultipliers();
            internalSetup.SetAxisUsageType();
        }
    }
}