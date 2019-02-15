namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Attribute;

    /// <summary>
    /// A rotational drive that utilizes a physics joint to control the rotational angle.
    /// </summary>
    public class RotationalJointDrive : RotationalDrive
    {
        #region Joint Settings
        /// <summary>
        /// The joint being used to drive the rotation.
        /// </summary>
        [Header("Joint Settings"), Tooltip("The joint being used to drive the rotation."), InternalSetting, SerializeField]
        protected HingeJoint joint;
        /// <summary>
        /// The parent <see cref="GameObject"/> of the joint.
        /// </summary>
        [Tooltip("The parent GameObject of the joint."), InternalSetting, SerializeField]
        protected GameObject jointContainer;
        #endregion

        /// <summary>
        /// The <see cref="Rigidbody"/> that the joint is using.
        /// </summary>
        protected Rigidbody jointRigidbody;
        /// <summary>
        /// The motor data to set on the joint.
        /// </summary>
        protected JointMotor jointMotor = new JointMotor();

        /// <inheritdoc />
        public override Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            if (!isActiveAndEnabled)
            {
                return Vector3.zero;
            }

            Vector3 axisDirection = base.CalculateDriveAxis(driveAxis);
            joint.axis = axisDirection * -1f;
            return axisDirection;
        }

        /// <inheritdoc />
        public override void CalculateHingeLocation(Vector3 newHingeLocation)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            joint.anchor = newHingeLocation;
            SetJointContainerPosition();
        }

        /// <inheritdoc />
        public override void ConfigureAutoDrive(bool autoDrive)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            joint.useMotor = autoDrive;
        }

        /// <inheritdoc />
        protected override void SetUpInternals()
        {
            jointMotor.force = joint.motor.force;
            jointRigidbody = joint.GetComponent<Rigidbody>();
            ConfigureAutoDrive(facade.MoveToTargetValue);
            base.SetUpInternals();
        }

        /// <inheritdoc />
        protected override Transform GetDriveTransform()
        {
            return joint.transform;
        }

        /// <inheritdoc />
        protected override void ProcessAutoDrive(float driveSpeed)
        {
            jointMotor.targetVelocity = driveSpeed;
            joint.motor = jointMotor;
        }

        /// <inheritdoc />
        protected override void AttemptApplyLimits()
        {
            if (!joint.useLimits && ApplyLimits())
            {
                jointRigidbody.velocity = Vector3.zero;
                jointRigidbody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Sets the <see cref="jointContainer"/> position based on the joint anchor.
        /// </summary>
        protected virtual void SetJointContainerPosition()
        {
            // Disable any child rigidbody GameObjects of the joint to prevent the anchor position updating their position.
            Rigidbody[] rigidbodyChildren = joint.GetComponentsInChildren<Rigidbody>();
            bool[] childrenStates = new bool[rigidbodyChildren.Length];
            for (int index = 0; index < rigidbodyChildren.Length; index++)
            {
                if (rigidbodyChildren[index].gameObject == jointContainer || rigidbodyChildren[index] == jointRigidbody)
                {
                    continue;
                }

                childrenStates[index] = rigidbodyChildren[index].gameObject.activeSelf;
                rigidbodyChildren[index].gameObject.SetActive(false);
            }

            // Set the current joint container to match the joint anchor to provide the correct offset.
            jointContainer.transform.localPosition = joint.anchor;
            jointContainer.transform.localRotation = Quaternion.identity;

            // Restore the state of child rigidbody GameObjects now the anchor position has been set.
            for (int index = 0; index < rigidbodyChildren.Length; index++)
            {
                if (rigidbodyChildren[index].gameObject == jointContainer || rigidbodyChildren[index] == jointRigidbody)
                {
                    continue;
                }

                rigidbodyChildren[index].gameObject.SetActive(childrenStates[index]);
            }
        }
    }
}