namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
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
        protected float PreviousActualAngle => previousActualRotation[(int)facade.DriveAxis];
        /// <summary>
        /// The current rotation angle of the control.
        /// </summary>
        protected float CurrentActualAngle => currentActualRotation[(int)facade.DriveAxis];
        /// <summary>
        /// The target angle for the control to reach.
        /// </summary>
        protected float ActualTargetAngle => Mathf.Lerp(DriveLimits.minimum, DriveLimits.maximum, facade.TargetValue);

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
        /// The representation of the rotation in meaningful values and not limited to 0f to 360f.
        /// </summary>
        protected float pseudoRotation;
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

        /// <inheritdoc />
        public override void Process()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            base.Process();

            previousActualRotation = currentActualRotation;
            currentActualRotation = GetSimpleEulerAngles();

            CalculateRotationMultiplier();
            AttemptApplyLimits();

            pseudoRotation = CurrentActualAngle + (circleDegrees * rotationMultiplier);

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
            ConfigureAutoDrive(facade.MoveToTargetValue);
            CalculateHingeLocation(facade.HingeLocation);
        }

        /// <inheritdoc />
        protected override FloatRange CalculateDriveLimits(RotationalDriveFacade facade)
        {
            return facade.DriveLimit;
        }

        /// <inheritdoc />
        protected override float CalculateValue(DriveAxis.Axis driveAxis, FloatRange limits)
        {
            if (!isActiveAndEnabled)
            {
                return 0f;
            }

            return Mathf.Clamp(pseudoRotation, limits.minimum, limits.maximum);
        }

        /// <summary>
        /// Attempts to retrieve a simple x, y or z euler angle from the <see cref="Transform.localEulerAngles"/> utilizing any other axis rotation.
        /// </summary>
        /// <returns>The actual axis angle from 0f to 360f.</returns>
        protected virtual Vector3 GetSimpleEulerAngles()
        {
            Vector3 currentEulerAngle = GetDriveTransform().localEulerAngles;
            if (facade.DriveAxis == DriveAxis.Axis.XAxis && !currentEulerAngle.y.ApproxEquals(0f, 1f))
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
            if (actualAngle.ApproxEquals(pseudoRotation, targetValueReachedThreshold))
            {
                return 0f;
            }

            return actualAngle > pseudoRotation ? 1f : -1f;
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
            if (pseudoRotation < DriveLimits.minimum)
            {
                GetDriveTransform().localRotation = Quaternion.Euler(-AxisDirection * DriveLimits.minimum);
                return true;
            }
            else if (pseudoRotation > DriveLimits.maximum)
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
            return (facade.MoveToTargetValue && !pseudoRotation.ApproxEquals(ActualTargetAngle, targetValueReachedThreshold) ? facade.DriveSpeed : 0f) * CalculateDirectionMultiplier();
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
            if (facade.MoveToTargetValue && driveSpeed.ApproxEquals(0f))
            {
                GetDriveTransform().localRotation = Quaternion.Euler(-AxisDirection * ActualTargetAngle);
            }
        }
    }
}