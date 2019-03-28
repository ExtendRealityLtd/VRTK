namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Extension;
    using Zinnia.Data.Type;

    /// <summary>
    /// The basis to drive a control in a rotational angle.
    /// </summary>
    public abstract class RotationalDrive : Drive<RotationalDriveFacade, RotationalDrive>
    {
        /// <summary>
        /// The previous rotation angle of the control.
        /// </summary>
        protected float PreviousActualAngle => previousActualRotation[(int)Facade.DriveAxis];
        /// <summary>
        /// The current rotation angle of the control.
        /// </summary>
        protected float CurrentActualAngle => currentActualRotation[(int)Facade.DriveAxis];
        /// <summary>
        /// The target angle for the control to reach.
        /// </summary>
        protected float ActualTargetAngle => Mathf.Lerp(DriveLimits.minimum, DriveLimits.maximum, Facade.TargetValue);

        /// <summary>
        /// The total number of degrees in a circle.
        /// </summary>
        protected const float circleDegrees = 360f;
        /// <summary>

        /// The angle range that defines the upper right quadrant of a circle.
        /// </summary>
        protected static readonly FloatRange circleUpperRightQuadrant = new FloatRange(-1f, 90f);
        /// <summary>
        /// The angle range that defines the upper left quadrant of a circle.
        /// </summary>
        protected static readonly FloatRange circleUpperLeftQuadrant = new FloatRange(270f, 360f);

        /// <summary>
        /// The representation of the previous frame rotation in meaningful values and not limited to 0f to 360f.
        /// </summary>
        protected float previousPseudoRotation;
        /// <summary>
        /// The representation of the current frame rotation in meaningful values and not limited to 0f to 360f.
        /// </summary>
        protected float currentPseudoRotation;
        /// <summary>
        /// The representation of the rotational velocity.
        /// </summary>
        protected float pseudoAngularVelocity;
        /// <summary>
        /// The multiplier used to determine how many complete revolutions the drive has performed.
        /// </summary>
        protected float rotationMultiplier;
        /// <summary>
        /// The previous actual rotational value of the drive.
        /// </summary>
        protected Vector3 previousActualRotation;
        /// <summary>
        /// The current actual rotational value of the drive.
        /// </summary>
        protected Vector3 currentActualRotation;

        /// <summary>
        /// Calculates the location of the rotational hinge for the drive.
        /// </summary>
        /// <param name="newHingeLocation">The new local space for the hinge point.</param>
        public abstract void CalculateHingeLocation(Vector3 newHingeLocation);
        /// <summary>
        /// Attempts to apply the existing angular velocity back on to the rotation of the drive.
        /// </summary>
        /// <param name="multiplier">The amount to multiply the angular velocity to be applied by.</param>
        public abstract void ApplyExistingAngularVelocity(float multiplier = 1f);

        /// <inheritdoc />
        [RequiresBehaviourState]
        public override void Process()
        {
            base.Process();

            previousActualRotation = currentActualRotation;
            currentActualRotation = GetSimpleEulerAngles();

            CalculateRotationMultiplier();
            AttemptApplyLimits();

            currentPseudoRotation = CurrentActualAngle + (circleDegrees * rotationMultiplier);
            pseudoAngularVelocity = !currentPseudoRotation.ApproxEquals(previousPseudoRotation) ? previousPseudoRotation - currentPseudoRotation : pseudoAngularVelocity;
            previousPseudoRotation = currentPseudoRotation;

            float autoDriveTargetVelocity = CalculateAutoDriveVelocity();
            ProcessAutoDrive(autoDriveTargetVelocity);
            MatchActualTargetAngle(autoDriveTargetVelocity);
        }

        /// <summary>
        /// Automatically controls the drive to the target rotation at the given speed.
        /// </summary>
        /// <param name="driveSpeed">The speed to automatically rotate the drive.</param>
        protected abstract void ProcessAutoDrive(float driveSpeed);

        /// <inheritdoc />
        protected override void SetUpInternals()
        {
            ConfigureAutoDrive(Facade.MoveToTargetValue);
            CalculateHingeLocation(Facade.HingeLocation);
        }

        /// <inheritdoc />
        protected override FloatRange CalculateDriveLimits(RotationalDriveFacade facade)
        {
            return facade.DriveLimit;
        }

        /// <inheritdoc />
        [RequiresBehaviourState]
        protected override float CalculateValue(DriveAxis.Axis driveAxis, FloatRange limits)
        {
            return Mathf.Clamp(currentPseudoRotation, limits.minimum, limits.maximum);
        }

        /// <summary>
        /// Attempts to retrieve a simple x, y or z euler angle from the <see cref="Transform.localEulerAngles"/> utilizing any other axis rotation.
        /// </summary>
        /// <returns>The actual axis angle from 0f to 360f.</returns>
        protected virtual Vector3 GetSimpleEulerAngles()
        {
            Vector3 currentEulerAngle = GetDriveTransform().localEulerAngles;
            if (Facade.DriveAxis == DriveAxis.Axis.XAxis && !currentEulerAngle.y.ApproxEquals(0f, 1f))
            {
                currentEulerAngle.x = currentEulerAngle.y - (currentEulerAngle.x > (circleDegrees * 0.5f) ? currentEulerAngle.x - circleDegrees : currentEulerAngle.x);
            }
            return currentEulerAngle;
        }

        /// <summary>
        /// Calculates a multiplier based on the direction the rotation is traveling.
        /// </summary>
        /// <returns>The multiplier that represents the direction.</returns>
        protected virtual float CalculateDirectionMultiplier()
        {
            float actualAngle = ActualTargetAngle;
            if (actualAngle.ApproxEquals(currentPseudoRotation, TargetValueReachedThreshold))
            {
                return 0f;
            }

            return actualAngle > currentPseudoRotation ? 1f : -1f;
        }

        /// <summary>
        /// Attempts to apply the limits on the drive.
        /// </summary>
        protected virtual void AttemptApplyLimits()
        {
            ApplyLimits();
        }

        /// <summary>
        /// Applies the limits on the drive rotation.
        /// </summary>
        /// <returns>Whether the limits have been applied.</returns>
        protected virtual bool ApplyLimits()
        {
            if (currentPseudoRotation < DriveLimits.minimum - TargetValueReachedThreshold)
            {
                GetDriveTransform().localRotation = Quaternion.Euler(-AxisDirection * DriveLimits.minimum);
                return true;
            }
            else if (currentPseudoRotation > DriveLimits.maximum + TargetValueReachedThreshold)
            {
                GetDriveTransform().localRotation = Quaternion.Euler(-AxisDirection * DriveLimits.maximum);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates the velocity that the drive should automatically rotate the control with.
        /// </summary>
        /// <returns>The velocity to drive the control automatically with.</returns>
        protected virtual float CalculateAutoDriveVelocity()
        {
            return (Facade.MoveToTargetValue && !currentPseudoRotation.ApproxEquals(ActualTargetAngle, TargetValueReachedThreshold) ? Facade.DriveSpeed : 0f) * CalculateDirectionMultiplier();
        }

        /// <summary>
        /// Calculates the current rotation the control is at.
        /// </summary>
        protected virtual void CalculateRotationMultiplier()
        {
            if (circleUpperLeftQuadrant.Contains(PreviousActualAngle) && circleUpperRightQuadrant.Contains(CurrentActualAngle))
            {
                rotationMultiplier++;
            }
            else if (circleUpperRightQuadrant.Contains(PreviousActualAngle) && circleUpperLeftQuadrant.Contains(CurrentActualAngle))
            {
                rotationMultiplier--;
            }
        }

        /// <summary>
        /// Attempts to match the target angle to set the control at the correct angle if the drive is no longer moving.
        /// </summary>
        /// <param name="driveSpeed">The speed the drive is automatically rotating at.</param>
        protected virtual void MatchActualTargetAngle(float driveSpeed)
        {
            if (Facade.MoveToTargetValue && driveSpeed.ApproxEquals(0f))
            {
                GetDriveTransform().localRotation = Quaternion.Euler(-AxisDirection * ActualTargetAngle);
            }
        }
    }
}