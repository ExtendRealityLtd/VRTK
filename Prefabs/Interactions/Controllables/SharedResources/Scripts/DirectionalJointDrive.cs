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

            joint.xMotion = (driveAxis == DriveAxis.Axis.XAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
            joint.yMotion = (driveAxis == DriveAxis.Axis.YAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
            joint.zMotion = (driveAxis == DriveAxis.Axis.ZAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
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
                return new FloatRange();
            }

            SoftJointLimit softJointLimit = new SoftJointLimit();
            float motionLimit = Mathf.Abs(newLimit * 0.5f);
            softJointLimit.limit = motionLimit;
            joint.linearLimit = softJointLimit;
            return new FloatRange(-motionLimit, motionLimit);
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
            snapDriver.maximumForce = (moveToTargetValue ? 1f : 0f);

            joint.xDrive = snapDriver;
            joint.yDrive = snapDriver;
            joint.zDrive = snapDriver;
        }

        /// <inheritdoc />
        public override void SetTargetValue(float normalizedValue)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            joint.targetPosition = AxisDirection * Mathf.Lerp(DriveLimits.minimum, DriveLimits.maximum, Mathf.Clamp01(normalizedValue));
        }

        /// <inheritdoc />
        public override void ProcessAutoDrive(bool autoDrive)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            ProcessDriveSpeed(facade.DriveSpeed, facade.MoveToTargetValue);
        }

        /// <inheritdoc />
        protected override void SetUpInternals()
        {
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
                    result = joint.transform.localPosition.x;
                    break;
                case DriveAxis.Axis.YAxis:
                    result = joint.transform.localPosition.y;
                    break;
                case DriveAxis.Axis.ZAxis:
                    result = joint.transform.localPosition.z;
                    break;
            }
            return Mathf.Clamp(result, limits.minimum, limits.maximum);
        }
    }
}