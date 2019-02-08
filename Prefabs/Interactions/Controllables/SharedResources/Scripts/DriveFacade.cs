namespace VRTK.Prefabs.Interactions.Controllables
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// Defines the event with the drive <see cref="float"/> value.
    /// </summary>
    [Serializable]
    public class DriveUnityEvent : UnityEvent<float>
    {
    }

    /// <summary>
    /// The basis of the public interface that will drive a control in relation to a specified world axis.
    /// </summary>
    /// <typeparam name="TDrive">The <see cref="Drive{TFacade, TSelf}"/> to operate with the facade.</typeparam>
    /// <typeparam name="TSelf">The actual concrete implementation of the drive facade being used.</typeparam>
    public abstract class DriveFacade<TDrive, TSelf> : MonoBehaviour
         where TDrive : Drive<TSelf, TDrive> where TSelf : DriveFacade<TDrive, TSelf>
    {
        #region Internal Settings
        /// <summary>
        /// The linked <see cref="TDrive"/>
        /// </summary>
        [Header("Internal Settings"), Tooltip("The linked TDrive."), InternalSetting, SerializeField]
        protected TDrive drive;
        #endregion

        #region Events
        /// <summary>
        /// Emitted when the raw value changes with the raw value data.
        /// </summary>
        [Header("Events")]
        public DriveUnityEvent ValueChanged = new DriveUnityEvent();
        /// <summary>
        /// Emitted when the step value changes with the step value data.
        /// </summary>
        public DriveUnityEvent StepValueChanged = new DriveUnityEvent();
        /// <summary>
        /// Emitted when the normalized value changes with the normalized value data.
        /// </summary>
        public DriveUnityEvent NormalizedValueChanged = new DriveUnityEvent();
        /// <summary>
        /// Emitted when <see cref="TargetValue"/> has been reached by the control.
        /// </summary>
        public DriveUnityEvent TargetValueReached = new DriveUnityEvent();
        #endregion

        #region Drive Settings
        [Header("Drive Settings"), Tooltip("The axis to operate the drive motion on."), SerializeField]
        private DriveAxis.Axis _driveAxis = Controllables.DriveAxis.Axis.XAxis;
        /// <summary>
        /// The axis to operate the drive motion on.
        /// </summary>
        public DriveAxis.Axis DriveAxis
        {
            get { return _driveAxis; }
            set
            {
                _driveAxis = value;
                CalculateDriveAxis(_driveAxis);
            }
        }

        [Tooltip("Determines if the drive should move the control to the set Target Value."), SerializeField]
        private bool _moveToTargetValue = true;
        /// <summary>
        /// Determines if the drive should move the element to the set <see cref="TargetValue"/>.
        /// </summary>
        public bool MoveToTargetValue
        {
            get { return _moveToTargetValue; }
            set
            {
                _moveToTargetValue = value;
                ProcessAutoDrive(_moveToTargetValue);
            }
        }

        [Tooltip("The normalized value to attempt to drive the control to if the Move To Target Value is set to true."), SerializeField, Range(0f, 1f)]
        private float _targetValue = 0.5f;
        /// <summary>
        /// The normalized value to attempt to drive the control to if the <see cref="MoveToTargetValue"/> is set to <see langword="true"/>.
        /// </summary>
        public float TargetValue
        {
            get { return _targetValue; }
            set
            {
                _targetValue = value;
                SetTargetValue(_targetValue);
            }
        }

        [Tooltip("The speed in which the drive will attempt to move the control to the desired value."), SerializeField]
        private float _driveSpeed = 10f;
        /// <summary>
        /// The speed in which the drive will attempt to move the control to the desired value.
        /// </summary>
        public float DriveSpeed
        {
            get { return _driveSpeed; }
            set
            {
                _driveSpeed = value;
                ProcessDriveSpeed(_driveSpeed, MoveToTargetValue);
            }
        }
        #endregion

        #region Step Settings
        /// <summary>
        /// The range of step values to use.
        /// </summary>
        [Header("Step Settings"), Tooltip("The range of step values to use.")]
        public FloatRange stepRange = new FloatRange(0f, 10f);
        /// <summary>
        /// The increment to increase the steps in value by.
        /// </summary>
        [Tooltip("The increment to increase the steps in value by.")]
        public float stepIncrement = 1f;
        #endregion

        /// <summary>
        /// Sets the <see cref="TargetValue"/> to the position that the current step value represents.
        /// </summary>
        public virtual void SetTargetValueByStepValue()
        {
            SetTargetValueByStepValue(drive.StepValue);
        }

        /// <summary>
        /// Sets the <see cref="TargetValue"/> to the position that the given step value represents.
        /// </summary>
        /// <param name="stepValue">The step value that represents the new target value.</param>
        public virtual void SetTargetValueByStepValue(float stepValue)
        {
            float normalizedStepValue = Mathf.InverseLerp(stepRange.minimum, stepRange.maximum, Mathf.Clamp(stepValue, stepRange.minimum, stepRange.maximum));
            TargetValue = normalizedStepValue;
        }

        /// <summary>
        /// Forces the drive to move to the current step value at the given speed.
        /// </summary>
        /// <param name="driveSpeed">The speed the drive will move the control at.</param>
        public virtual void ForceSnapToStepValue(float driveSpeed)
        {
            MoveToTargetValue = true;
            DriveSpeed = driveSpeed;
            SetTargetValueByStepValue();
        }

        /// <summary>
        /// Calculates the axis to use for the given <see cref="Controllables.DriveAxis.Axis"/>.
        /// </summary>
        /// <param name="driveAxis">The new value.</param>
        protected virtual void CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            drive.CalculateDriveAxis(driveAxis);
        }

        /// <summary>
        /// Processes the drive's ability to automatically drive the control.
        /// </summary>
        /// <param name="autoDrive">Whether the drive can automatically drive the control.</param>
        protected virtual void ProcessAutoDrive(bool autoDrive)
        {
            drive.ProcessAutoDrive(autoDrive);
        }

        /// <summary>
        /// Sets the new <see cref="TargetValue"/>.
        /// </summary>
        /// <param name="targetValue">The new value.</param>
        protected virtual void SetTargetValue(float targetValue)
        {
            drive.SetTargetValue(targetValue);
        }

        /// <summary>
        /// Processes the changes to the <see cref="DriveSpeed"/>.
        /// </summary>
        /// <param name="driveSpeed">The new value.</param>
        /// <param name="moveToTargetValue">Whether the new value should be processed.</param>
        protected virtual void ProcessDriveSpeed(float driveSpeed, bool moveToTargetValue)
        {
            drive.ProcessDriveSpeed(driveSpeed, moveToTargetValue);
        }

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            drive.SetUp();
        }
    }
}