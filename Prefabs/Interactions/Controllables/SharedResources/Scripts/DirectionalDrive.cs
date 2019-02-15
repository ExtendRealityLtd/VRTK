namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Type;

    /// <summary>
    /// The basis to drive a control in a linear direction.
    /// </summary>
    public abstract class DirectionalDrive : Drive<DirectionalDriveFacade, DirectionalDrive>
    {
        /// <summary>
        /// Calculates the limits of the drive.
        /// </summary>
        /// <param name="newLimit">The maximum local space limit the drive can reach.</param>
        /// <returns>The minimum and maximum local space limit the drive can reach.</returns>
        public virtual FloatRange CalculateDriveLimits(float newLimit)
        {
            if (!isActiveAndEnabled)
            {
                return new FloatRange();
            }

            float motionLimit = Mathf.Abs(newLimit * 0.5f);
            return new FloatRange(-motionLimit, motionLimit);
        }

        /// <inheritdoc />
        protected override FloatRange CalculateDriveLimits(DirectionalDriveFacade facade)
        {
            return CalculateDriveLimits(facade.DriveLimit);
        }

        /// <inheritdoc />
        protected override float CalculateValue(DriveAxis.Axis axis, FloatRange limits)
        {
            if (!isActiveAndEnabled)
            {
                return 0f;
            }

            float result = 0f;
            switch (axis)
            {
                case DriveAxis.Axis.XAxis:
                    result = GetDriveTransform().localPosition.x;
                    break;
                case DriveAxis.Axis.YAxis:
                    result = GetDriveTransform().localPosition.y;
                    break;
                case DriveAxis.Axis.ZAxis:
                    result = GetDriveTransform().localPosition.z;
                    break;
            }
            return Mathf.Clamp(result, limits.minimum, limits.maximum);
        }

        /// <inheritdoc />
        public override void ConfigureAutoDrive(bool autoDrive)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            ProcessDriveSpeed(facade.DriveSpeed, facade.MoveToTargetValue);
        }
    }
}