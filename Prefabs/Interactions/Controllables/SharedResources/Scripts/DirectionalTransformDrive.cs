namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Extension;
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
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public InteractableFacade Interactable { get; protected set; }
        /// <summary>
        /// The <see cref="Vector3Restrictor"/> to clamp the position of the drive within the drive limits.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public Vector3Restrictor PositionClamper { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPropertyApplier"/> to automatically move the drive to a specific location.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPropertyApplier PropertyApplier { get; protected set; }
        #endregion

        /// <summary>
        /// The position to automatically move the drive to.
        /// </summary>
        private readonly TransformData autoDrivePosition = new TransformData();

        /// <inheritdoc />
        public override void Process()
        {
            ConfigureAutoDrive(Facade.MoveToTargetValue);
            base.Process();
        }

        /// <inheritdoc />
        [RequiresBehaviourState]
        public override Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            PositionClamper.XBounds = driveAxis == DriveAxis.Axis.XAxis ? DriveLimits : default;
            PositionClamper.YBounds = driveAxis == DriveAxis.Axis.YAxis ? DriveLimits : default;
            PositionClamper.ZBounds = driveAxis == DriveAxis.Axis.ZAxis ? DriveLimits : default;

            return -base.CalculateDriveAxis(driveAxis);
        }

        /// <inheritdoc />
        public override void ProcessDriveSpeed(float driveSpeed, bool moveToTargetValue)
        {
            PropertyApplier.TransitionDuration = driveSpeed.ApproxEquals(0f) ? 0f : 1f / driveSpeed;
            PropertyApplier.enabled = moveToTargetValue;
            if (PropertyApplier.enabled)
            {
                if (Interactable.ConsumerRigidbody != null)
                {
                    Interactable.ConsumerRigidbody.velocity = Vector3.zero;
                }
                PropertyApplier.Apply();
            }
        }

        /// <inheritdoc />
        protected override Transform GetDriveTransform()
        {
            return Interactable.transform;
        }

        /// <inheritdoc />
        protected override void SetDriveTargetValue(Vector3 targetValue)
        {
            autoDrivePosition.UseLocalValues = true;
            autoDrivePosition.Transform = GetDriveTransform();
            autoDrivePosition.PositionOverride = targetValue;
            PropertyApplier.Source = autoDrivePosition;
            PropertyApplier.Apply();
        }
    }
}