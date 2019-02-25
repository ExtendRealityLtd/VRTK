namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// A directional drive that utilizes a physics joint to control the linear translation movement.
    /// </summary>
    public class DirectionalJointDrive : DirectionalDrive
    {
        #region Joint Settings
        /// <summary>
        /// The joint being used to drive the movement.
        /// </summary>
        [Header("Joint Settings"), Tooltip("The joint being used to drive the movement."), InternalSetting, SerializeField]
        protected ConfigurableJoint joint;
        #endregion

        /// <inheritdoc />
        public override Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            if (!isActiveAndEnabled)
            {
                return Vector3.zero;
            }

            joint.xMotion = driveAxis == DriveAxis.Axis.XAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
            joint.yMotion = driveAxis == DriveAxis.Axis.YAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
            joint.zMotion = driveAxis == DriveAxis.Axis.ZAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
            return base.CalculateDriveAxis(driveAxis);
        }

        /// <inheritdoc />
        public override FloatRange CalculateDriveLimits(float newLimit)
        {
            if (!isActiveAndEnabled)
            {
                return FloatRange.MinMax;
            }

            FloatRange jointLimit = base.CalculateDriveLimits(newLimit);
            SoftJointLimit softJointLimit = new SoftJointLimit();
            softJointLimit.limit = jointLimit.maximum;
            joint.linearLimit = softJointLimit;
            return jointLimit;
        }

        /// <inheritdoc />
        public override void ProcessDriveSpeed(float driveSpeed, bool moveToTargetValue)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            JointDrive snapDriver = new JointDrive();
            snapDriver.positionSpring = driveSpeed;
            snapDriver.positionDamper = 1f;
            snapDriver.maximumForce = moveToTargetValue ? 1f : 0f;

            joint.xDrive = snapDriver;
            joint.yDrive = snapDriver;
            joint.zDrive = snapDriver;
        }

        /// <inheritdoc />
        protected override Transform GetDriveTransform()
        {
            return joint.transform;
        }

        /// <inheritdoc />
        protected override void SetDriveTargetValue(Vector3 targetValue)
        {
            joint.targetPosition = targetValue;
        }
    }
}