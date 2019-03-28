namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Malimbe.BehaviourStateRequirementMethod;
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
                return FloatRange.MinMax;
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
        [RequiresBehaviourState]
        protected override float CalculateValue(DriveAxis.Axis axis, FloatRange limits)
        {
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
        [RequiresBehaviourState]
        public override void ConfigureAutoDrive(bool autoDrive)
        {
            ProcessDriveSpeed(Facade.DriveSpeed, Facade.MoveToTargetValue);
        }
    }
}