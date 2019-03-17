namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
    using Zinnia.Extension;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using Zinnia.Tracking.Velocity;
    using Zinnia.Tracking.Follow.Modifier.Property.Rotation;
    using VRTK.Prefabs.Interactions.Interactables;

    /// <summary>
    /// A rotational drive that directly manipulates a <see cref="Transform.rotation"/> to control the rotational angle without the use of any physics.
    /// </summary>
    public class RotationalTransformDrive : RotationalDrive
    {
        #region Reference Settings
        /// <summary>
        /// The <see cref="InteractableFacade"/> that controls the movement of the drive.
        /// </summary>
        [Serialized]
        [field: Header("Reference Settings"), DocumentedByXml, Restricted]
        public InteractableFacade Interactable { get; protected set; }
        /// <summary>
        /// The <see cref="GameObject"/> that contains the meshes for the control.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject InteractableMesh { get; protected set; }
        /// <summary>
        /// The <see cref="TransformPositionDifferenceRotation"/> to drive the rotation of the control.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public TransformPositionDifferenceRotation RotationModifier { get; protected set; }
        /// <summary>
        /// The <see cref="ArtificialVelocityApplier"/> that applies artificial angular velocity to the control after releasing.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public ArtificialVelocityApplier VelocityApplier { get; protected set; }
        #endregion

        /// <inheritdoc />
        [RequiresBehaviourState]
        public override Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            Vector3 axisDirection = base.CalculateDriveAxis(driveAxis);
            switch (driveAxis)
            {
                case DriveAxis.Axis.XAxis:
                    RotationModifier.FollowOnAxis = Vector3State.XOnly;
                    break;
                case DriveAxis.Axis.YAxis:
                    RotationModifier.FollowOnAxis = Vector3State.YOnly;
                    break;
                case DriveAxis.Axis.ZAxis:
                    RotationModifier.FollowOnAxis = Vector3State.ZOnly;
                    break;
            }

            return axisDirection;
        }

        /// <inheritdoc />
        [RequiresBehaviourState]
        public override void CalculateHingeLocation(Vector3 newHingeLocation)
        {
            transform.localPosition = newHingeLocation;
            InteractableMesh.transform.localPosition = newHingeLocation * -1f;
        }

        /// <inheritdoc />
        public override void ApplyExistingAngularVelocity(float multiplier = 1f)
        {
            VelocityApplier.AngularVelocity = AxisDirection * pseudoAngularVelocity * multiplier;
            VelocityApplier.Apply();
        }

        /// <inheritdoc />
        protected override Transform GetDriveTransform()
        {
            return Interactable.transform;
        }

        /// <inheritdoc />
        protected override void AttemptApplyLimits()
        {
            if (ApplyLimits())
            {
                VelocityApplier.Velocity = Vector3.zero;
                VelocityApplier.AngularVelocity = Vector3.zero;
            }
        }

        /// <inheritdoc />
        protected override void ProcessAutoDrive(float driveSpeed)
        {
            if (Facade.MoveToTargetValue && !driveSpeed.ApproxEquals(0f))
            {
                GetDriveTransform().localRotation *= Quaternion.Euler(-AxisDirection * driveSpeed * Time.deltaTime);
            }
        }
    }
}