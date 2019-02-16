namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
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
        [Tooltip("The InteractableFacade that controls the movement of the drive."), InternalSetting, SerializeField]
        protected InteractableFacade interactable;
        /// <summary>
        /// The <see cref="GameObject"/> that contains the meshes for the control.
        /// </summary>
        [Tooltip("The GameObject that contains the meshes for the control."), InternalSetting, SerializeField]
        protected GameObject interactableMesh;
        /// <summary>
        /// The <see cref="TransformPositionDifferenceRotation"/> to drive the rotation of the control.
        /// </summary>
        [Tooltip("The TransformPositionDifferenceRotation to drive the rotation of the control."), InternalSetting, SerializeField]
        protected TransformPositionDifferenceRotation rotationModifier;
        /// <summary>
        /// The <see cref="ArtificialVelocityApplier"/> that applies artificial angular velocity to the control after releasing.
        /// </summary>
        [Tooltip("The ArtificialVelocityApplier that applies artificial angular velocity to the control after releasing."), InternalSetting, SerializeField]
        protected ArtificialVelocityApplier velocityApplier;
        #endregion

        /// <inheritdoc />
        public override Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            if (!isActiveAndEnabled)
            {
                return Vector3.zero;
            }

            Vector3 axisDirection = base.CalculateDriveAxis(driveAxis);
            switch (driveAxis)
            {
                case DriveAxis.Axis.XAxis:
                    rotationModifier.FollowOnAxis = Vector3State.XOnly;
                    break;
                case DriveAxis.Axis.YAxis:
                    rotationModifier.FollowOnAxis = Vector3State.YOnly;
                    break;
                case DriveAxis.Axis.ZAxis:
                    rotationModifier.FollowOnAxis = Vector3State.ZOnly;
                    break;
            }

            return axisDirection;
        }

        /// <inheritdoc />
        public override void CalculateHingeLocation(Vector3 newHingeLocation)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            transform.localPosition = newHingeLocation;
            interactableMesh.transform.localPosition = newHingeLocation * -1f;
        }

        /// <inheritdoc />
        public override void ApplyExistingAngularVelocity(float multiplier = 1f)
        {
            velocityApplier.AngularVelocity = AxisDirection * pseudoAngularVelocity * multiplier;
            velocityApplier.Apply();
        }

        /// <inheritdoc />
        protected override Transform GetDriveTransform()
        {
            return interactable.transform;
        }

        /// <inheritdoc />
        protected override void AttemptApplyLimits()
        {
            if (ApplyLimits())
            {
                velocityApplier.Velocity = Vector3.zero;
                velocityApplier.AngularVelocity = Vector3.zero;
            }
        }

        /// <inheritdoc />
        protected override void ProcessAutoDrive(float driveSpeed)
        {
            if (facade.MoveToTargetValue && !driveSpeed.ApproxEquals(0f))
            {
                GetDriveTransform().localRotation *= Quaternion.Euler(-AxisDirection * driveSpeed * Time.deltaTime);
            }
        }
    }
}