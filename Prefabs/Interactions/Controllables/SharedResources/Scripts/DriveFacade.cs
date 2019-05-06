namespace VRTK.Prefabs.Interactions.Controllables
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using Malimbe.MemberChangeMethod;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
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
    public abstract class DriveFacade<TDrive, TSelf> : MonoBehaviour where TDrive : Drive<TSelf, TDrive> where TSelf : DriveFacade<TDrive, TSelf>
    {
        #region Reference Settings
        /// <summary>
        /// The linked <see cref="TDrive"/>
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public TDrive Drive { get; protected set; }
        #endregion

        #region Events
        /// <summary>
        /// Emitted when the raw value changes with the raw value data.
        /// </summary>
        [Header("Events"), DocumentedByXml]
        public DriveUnityEvent ValueChanged = new DriveUnityEvent();
        /// <summary>
        /// Emitted when the step value changes with the step value data.
        /// </summary>
        [DocumentedByXml]
        public DriveUnityEvent StepValueChanged = new DriveUnityEvent();
        /// <summary>
        /// Emitted when the normalized value changes with the normalized value data.
        /// </summary>
        [DocumentedByXml]
        public DriveUnityEvent NormalizedValueChanged = new DriveUnityEvent();
        /// <summary>
        /// Emitted when <see cref="TargetValue"/> has been reached by the control.
        /// </summary>
        [DocumentedByXml]
        public DriveUnityEvent TargetValueReached = new DriveUnityEvent();
        /// <summary>
        /// Emitted when the drive starts moving the control.
        /// </summary>
        [DocumentedByXml]
        public DriveUnityEvent StartedMoving = new DriveUnityEvent();
        /// <summary>
        /// Emitted when the drive is no longer moving the control and it is stationary.
        /// </summary>
        [DocumentedByXml]
        public DriveUnityEvent StoppedMoving = new DriveUnityEvent();
        #endregion

        #region Drive Settings
        /// <summary>
        /// The axis to operate the drive motion on.
        /// </summary>
        [Serialized]
        [field: Header("Drive Settings"), DocumentedByXml]
        public DriveAxis.Axis DriveAxis { get; set; } = Controllables.DriveAxis.Axis.XAxis;
        /// <summary>
        /// Determines if the drive should move the element to the set <see cref="TargetValue"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public bool MoveToTargetValue { get; set; } = true;
        /// <summary>
        /// The normalized value to attempt to drive the control to if the <see cref="MoveToTargetValue"/> is set to <see langword="true"/>.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Range(0f, 1f)]
        public float TargetValue { get; set; } = 0.5f;
        /// <summary>
        /// The speed in which the drive will attempt to move the control to the desired value.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float DriveSpeed { get; set; } = 10f;
        #endregion

        #region Step Settings
        /// <summary>
        /// The range of step values to use.
        /// </summary>
        [Serialized]
        [field: Header("Step Settings"), DocumentedByXml]
        public FloatRange StepRange { get; set; } = new FloatRange(0f, 10f);
        /// <summary>
        /// The increment to increase the steps in value by.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml]
        public float StepIncrement { get; set; } = 1f;
        #endregion

        /// <summary>
        /// Sets the <see cref="TargetValue"/> to the position that the current step value represents.
        /// </summary>
        public virtual void SetTargetValueByStepValue()
        {
            SetTargetValueByStepValue(Drive.StepValue);
        }

        /// <summary>
        /// Sets the <see cref="TargetValue"/> to the position that the given step value represents.
        /// </summary>
        /// <param name="stepValue">The step value that represents the new target value.</param>
        public virtual void SetTargetValueByStepValue(float stepValue)
        {
            float normalizedStepValue = Mathf.InverseLerp(StepRange.minimum, StepRange.maximum, Mathf.Clamp(stepValue, StepRange.minimum, StepRange.maximum));
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
        /// Calculates the axis to use for the given <see cref="DriveAxis.Axis"/>.
        /// </summary>
        /// <param name="driveAxis">The new value.</param>
        protected virtual void CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            Drive.CalculateDriveAxis(driveAxis);
        }

        /// <summary>
        /// Processes the drive's ability to automatically drive the control.
        /// </summary>
        /// <param name="autoDrive">Whether the drive can automatically drive the control.</param>
        protected virtual void ProcessAutoDrive(bool autoDrive)
        {
            Drive.ConfigureAutoDrive(autoDrive);
        }

        /// <summary>
        /// Sets the new <see cref="TargetValue"/>.
        /// </summary>
        /// <param name="targetValue">The new value.</param>
        protected virtual void SetTargetValue(float targetValue)
        {
            Drive.SetTargetValue(targetValue);
        }

        /// <summary>
        /// Processes the changes to the <see cref="DriveSpeed"/>.
        /// </summary>
        /// <param name="driveSpeed">The new value.</param>
        /// <param name="moveToTargetValue">Whether the new value should be processed.</param>
        protected virtual void ProcessDriveSpeed(float driveSpeed, bool moveToTargetValue)
        {
            Drive.ProcessDriveSpeed(driveSpeed, moveToTargetValue);
        }

        /// <summary>
        /// Called after <see cref="DriveAxis"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(DriveAxis))]
        protected virtual void OnAfterDriveAxisChange()
        {
            Drive.SetUp();
        }

        /// <summary>
        /// Called after <see cref="MoveToTargetValue"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(MoveToTargetValue))]
        protected virtual void OnAfterMoveToTargetValueChange()
        {
            Drive.SetUp();
        }

        /// <summary>
        /// Called after <see cref="TargetValue"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(TargetValue))]
        protected virtual void OnAfterTargetValueChange()
        {
            Drive.SetUp();
        }

        /// <summary>
        /// Called after <see cref="DriveSpeed"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(DriveSpeed))]
        protected virtual void OnAfterDriveSpeedChange()
        {
            Drive.SetUp();
        }

        /// <summary>
        /// Called after <see cref="StepRange"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(StepRange))]
        protected virtual void OnAfterStepRangeChange()
        {
            Drive.SetUp();
        }

        /// <summary>
        /// Called after <see cref="StepIncrement"/> has been changed.
        /// </summary>
        [CalledAfterChangeOf(nameof(StepIncrement))]
        protected virtual void OnAfterStepIncrementChange()
        {
            Drive.SetUp();
        }
    }
}