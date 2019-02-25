namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Modification;
    using Zinnia.Data.Type.Transformation;
    using VRTK.Prefabs.Interactions.Interactables;

    /// <summary>
    /// A directional drive that directly manipulates a <see cref="Transform.position"/> to control the linear translation movement without the use of any physics.
    /// </summary>
    public class DirectionalTransformDrive : DirectionalDrive
    {
        #region Reference Settings
        /// <summary>
        /// The <see cref="InteractableFacade"/> that controls the movement of the drive.
        /// </summary>
        [Tooltip("The InteractableFacade that controls the movement of the drive."), InternalSetting, SerializeField]
        protected InteractableFacade interactable;
        /// <summary>
        /// The <see cref="Vector3Restrictor"/> to clamp the position of the drive within the drive limits.
        /// </summary>
        [Tooltip("The Vector3Restrictor to clamp the position of the drive within the drive limits."), InternalSetting, SerializeField]
        protected Vector3Restrictor positionClamper;
        /// <summary>
        /// The <see cref="TransformPropertyApplier"/> to automatically move the drive to a specific location.
        /// </summary>
        [Header("Reference Settings"), Tooltip("The TransformPropertyApplier to automatically move the drive to a specific location."), InternalSetting, SerializeField]
        protected TransformPropertyApplier propertyApplier;
        #endregion

        /// <summary>
        /// A float range of 0f, 0f.
        /// </summary>
        private static readonly FloatRange nilLimit = FloatRange.Zero;
        /// <summary>
        /// The position to automatically move the drive to.
        /// </summary>
        private readonly TransformData autoDrivePosition = new TransformData();

        /// <inheritdoc />
        public override void Process()
        {
            ConfigureAutoDrive(facade.MoveToTargetValue);
            base.Process();
        }

        /// <inheritdoc />
        public override Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            if (!isActiveAndEnabled)
            {
                return Vector3.zero;
            }

            positionClamper.xBounds = driveAxis == DriveAxis.Axis.XAxis ? DriveLimits : nilLimit;
            positionClamper.yBounds = driveAxis == DriveAxis.Axis.YAxis ? DriveLimits : nilLimit;
            positionClamper.zBounds = driveAxis == DriveAxis.Axis.ZAxis ? DriveLimits : nilLimit;

            return -base.CalculateDriveAxis(driveAxis);
        }

        /// <inheritdoc />
        public override void ProcessDriveSpeed(float driveSpeed, bool moveToTargetValue)
        {
            propertyApplier.transitionDuration = 1f / driveSpeed;
            propertyApplier.enabled = moveToTargetValue;
            if (propertyApplier.enabled)
            {
                interactable.ConsumerRigidbody.velocity = Vector3.zero;
                propertyApplier.Apply();
            }
        }

        /// <inheritdoc />
        protected override Transform GetDriveTransform()
        {
            return interactable.transform;
        }

        /// <inheritdoc />
        protected override void SetDriveTargetValue(Vector3 targetValue)
        {
            autoDrivePosition.transform = GetDriveTransform();
            autoDrivePosition.positionOverride = autoDrivePosition.transform.TransformPoint(targetValue - autoDrivePosition.transform.localPosition);
            propertyApplier.Source = autoDrivePosition;
            propertyApplier.Apply();
        }
    }
}