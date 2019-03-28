namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Process;
    using Zinnia.Extension;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// The basis for a mechanism to drive motion on a control.
    /// </summary>
    /// <typeparam name="TFacade">The <see cref="DriveFacade{TDrive, TSelf}"/> to be used with the drive.</typeparam>
    /// <typeparam name="TSelf">The actual concrete implementation of the drive being used.</typeparam>
    public abstract class Drive<TFacade, TSelf> : MonoBehaviour, IProcessable where TFacade : DriveFacade<TSelf, TFacade> where TSelf : Drive<TFacade, TSelf>
    {
        #region Facade Settings
        /// <summary>
        /// The public interface facade.
        /// </summary>
        [Serialized]
        [field: Header("Facade Settings"), DocumentedByXml, Restricted]
        public TFacade Facade { get; protected set; }
        #endregion

        #region Threshold Settings
        /// <summary>
        /// The threshold that the current normalized value of the control can be within to consider the target value has been reached.
        /// </summary>
        [Serialized]
        [field: Header("Threshold Settings"), DocumentedByXml]
        public float TargetValueReachedThreshold { get; set; } = 0.025f;
        #endregion

        /// <summary>
        /// The current raw value for the drive control.
        /// </summary>
        public float Value => CalculateValue(Facade.DriveAxis, DriveLimits);
        /// <summary>
        /// The current normalized value for the drive control between the set limits.
        /// </summary>
        public float NormalizedValue => Mathf.InverseLerp(DriveLimits.minimum, DriveLimits.maximum, Value);
        /// <summary>
        /// The current step value for the drive control.
        /// </summary>
        public float StepValue => CalculateStepValue(Facade);
        /// <summary>
        /// The current normalized step value for the drive control between the set step range.
        /// </summary>
        public float NormalizedStepValue => Mathf.InverseLerp(Facade.StepRange.minimum, Facade.StepRange.maximum, StepValue);
        /// <summary>
        /// The calculated direction for the drive axis.
        /// </summary>
        public Vector3 AxisDirection { get; protected set; }
        /// <summary>
        /// The calculated limits for the drive.
        /// </summary>
        public FloatRange DriveLimits { get; protected set; }

        /// <summary>
        /// The previous state of <see cref="Value"/>.
        /// </summary>
        protected float previousValue = float.MaxValue;
        /// <summary>
        /// The previous state of <see cref="StepValue"/>.
        /// </summary>
        protected float previousStepValue = float.MaxValue;
        /// <summary>
        /// The previous state of whether the target value has been reached.
        /// </summary>
        protected bool previousTargetValueReached;
        /// <summary>
        /// Whether the control is moving or not.
        /// </summary>
        protected bool isMoving;

        /// <summary>
        /// Sets up the drive mechanism.
        /// </summary>
        public virtual void SetUp()
        {
            SetUpInternals();
            DriveLimits = CalculateDriveLimits(Facade);
            AxisDirection = CalculateDriveAxis(Facade.DriveAxis);
            ProcessDriveSpeed(Facade.DriveSpeed, Facade.MoveToTargetValue);
            SetTargetValue(Facade.TargetValue);
        }

        /// <summary>
        /// Processes the value changes and emits the appropriate events.
        /// </summary>
        [RequiresBehaviourState]
        public virtual void Process()
        {
            if (!Value.ApproxEquals(previousValue))
            {
                if (!isMoving && previousValue < float.MaxValue)
                {
                    EmitStartedMoving();
                    isMoving = true;
                }
                previousValue = Value;
                EmitValueChanged();
                EmitNormalizedValueChanged();
            }
            else
            {
                if (isMoving)
                {
                    EmitStoppedMoving();
                    isMoving = false;
                }
            }

            if (!StepValue.ApproxEquals(previousStepValue))
            {
                previousStepValue = StepValue;
                EmitStepValueChanged();
            }

            float targetValue = GetTargetValue();
            bool targetValueReached = NormalizedValue.ApproxEquals(targetValue, TargetValueReachedThreshold);
            bool shouldEmitEvent = !previousTargetValueReached && targetValueReached;
            previousTargetValueReached = targetValueReached;

            if (CanMoveToTargetValue() && shouldEmitEvent)
            {
                EmitTargetValueReached();
            }
        }

        /// <summary>
        /// Processes the speed in which the drive can affect the control.
        /// </summary>
        /// <param name="driveSpeed">The speed to drive the control at.</param>
        /// <param name="moveToTargetValue">Whether to allow the drive to automatically move the control to the desired target value.</param>
        public virtual void ProcessDriveSpeed(float driveSpeed, bool moveToTargetValue) { }

        /// <summary>
        /// Sets the target value of the drive to the given normalized value.
        /// </summary>
        /// <param name="normalizedValue">The normalized value to set the Target Value to.</param>
        [RequiresBehaviourState]
        public virtual void SetTargetValue(float normalizedValue)
        {
            SetDriveTargetValue(AxisDirection * Mathf.Lerp(DriveLimits.minimum, DriveLimits.maximum, Mathf.Clamp01(normalizedValue)));
        }

        /// <summary>
        /// Calculates the axis to drive the control on.
        /// </summary>
        /// <param name="driveAxis">The desired world axis.</param>
        /// <returns>The direction of the drive axis.</returns>
        public virtual Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            return driveAxis.GetAxisDirection(true);
        }

        /// <summary>
        /// Configures the ability to automatically drive the control.
        /// </summary>
        /// <param name="autoDrive">Whether the drive can automatically drive the control.</param>
        public virtual void ConfigureAutoDrive(bool autoDrive) { }

        /// <summary>
        /// Calculates the current value of the control.
        /// </summary>
        /// <param name="axis">The axis the drive is operating on.</param>
        /// <param name="limits">The limits of the drive.</param>
        /// <returns>The calculated value.</returns>
        protected abstract float CalculateValue(DriveAxis.Axis axis, FloatRange limits);
        /// <summary>
        /// Calculates the limits of the drive.
        /// </summary>
        /// <param name="facade">The facade containing the data for the calculation.</param>
        /// <returns>The minimum and maximum local space limit the drive can reach.</returns>
        protected abstract FloatRange CalculateDriveLimits(TFacade facade);
        /// <summary>
        /// Gets the <see cref="Transform"/> that the drive is operating on.
        /// </summary>
        /// <returns>The drive <see cref="Transform"/>.</returns>
        protected abstract Transform GetDriveTransform();

        protected virtual void OnEnable()
        {
            SetUp();
        }

        /// <summary>
        /// Performs any required internal setup.
        /// </summary>
        protected virtual void SetUpInternals() { }

        /// <summary>
        /// Sets the target value of the drive.
        /// </summary>
        /// <param name="targetValue">The value to set the drive target to.</param>
        protected virtual void SetDriveTargetValue(Vector3 targetValue) { }

        /// <summary>
        /// Gets the drive control target value.
        /// </summary>
        /// <returns>The target value specified in the facade.</returns>
        protected virtual float GetTargetValue()
        {
            return Facade.TargetValue;
        }

        /// <summary>
        /// Determines whether the drive can move the control to the target value.
        /// </summary>
        /// <returns>Whether the drive can automatically move to the target value specified in the facade.</returns>
        protected virtual bool CanMoveToTargetValue()
        {
            return Facade.MoveToTargetValue;
        }

        /// <summary>
        /// Calculates the current step value of the control.
        /// </summary>
        /// <param name="facade">The facade containing the data for the calculation.</param>
        /// <returns>The calculated step value.</returns>
        protected virtual float CalculateStepValue(TFacade facade)
        {
            return Mathf.Round(Mathf.Lerp(facade.StepRange.minimum / facade.StepIncrement, facade.StepRange.maximum / facade.StepIncrement, NormalizedValue));
        }

        /// <summary>
        /// Emits the ValueChanged event.
        /// </summary>
        protected virtual void EmitValueChanged()
        {
            Facade.ValueChanged?.Invoke(Value);
        }

        /// <summary>
        /// Emits the NormalizedValueChanged event.
        /// </summary>
        protected virtual void EmitNormalizedValueChanged()
        {
            Facade.NormalizedValueChanged?.Invoke(NormalizedValue);
        }

        /// <summary>
        /// Emits the StepValueChanged event.
        /// </summary>
        protected virtual void EmitStepValueChanged()
        {
            Facade.StepValueChanged?.Invoke(StepValue);
        }

        /// <summary>
        /// Emits the TargetValueReached event.
        /// </summary>
        protected virtual void EmitTargetValueReached()
        {
            Facade.TargetValueReached?.Invoke(NormalizedValue);
        }

        /// <summary>
        /// Emits the StartedMoving event.
        /// </summary>
        protected virtual void EmitStartedMoving()
        {
            Facade.StartedMoving?.Invoke(0f);
        }

        /// <summary>
        /// Emits the StoppedMoving event.
        /// </summary>
        protected virtual void EmitStoppedMoving()
        {
            Facade.StoppedMoving?.Invoke(0f);
        }
    }
}