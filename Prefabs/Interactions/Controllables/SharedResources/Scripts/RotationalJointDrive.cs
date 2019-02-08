namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using Zinnia.Data.Type;
    using Zinnia.Data.Attribute;
    using Zinnia.Extension;

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
        protected JointMotor jointMotor;

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

            float previousActualAngle = previousActualRotation[(int)facade.DriveAxis];
            float currentActualAngle = currentActualRotation[(int)facade.DriveAxis];
            float actualTargetAngle = Mathf.Lerp(DriveLimits.minimum, DriveLimits.maximum, facade.TargetValue);

            if (circleUpperLeftQuadrant.Contains(previousActualAngle) && circleUpperRightQuadrant.Contains(currentActualAngle))
            {
                rotationMultiplier++;
            }
            else if (circleUpperRightQuadrant.Contains(previousActualAngle) && circleUpperLeftQuadrant.Contains(currentActualAngle))
            {
                rotationMultiplier--;
            }

            if (!joint.useLimits && ApplyLimits())
            {
                jointRigidbody.velocity = Vector3.zero;
                jointRigidbody.angularVelocity = Vector3.zero;
            }

            pseudoRotation = currentActualAngle + (circleDegrees * rotationMultiplier);
            jointMotor.targetVelocity = (facade.MoveToTargetValue && !pseudoRotation.ApproxEquals(actualTargetAngle, targetValueReachedThreshold) ? facade.DriveSpeed : 0f) * CalculateDirectionMultiplier();
            joint.motor = jointMotor;

            if (facade.MoveToTargetValue && jointMotor.targetVelocity.ApproxEquals(0f))
            {
                joint.transform.localRotation = Quaternion.Euler(joint.axis * actualTargetAngle);
            }
        }

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
        public override FloatRange CalculateDriveLimits(FloatRange newLimit)
        {
            return newLimit;
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
        public override void ProcessDriveSpeed(float driveSpeed, bool moveToTargetValue)
        {
        }

        /// <inheritdoc />
        public override void SetTargetValue(float normalizedValue)
        {
        }

        /// <inheritdoc />
        public override void ProcessAutoDrive(bool autoDrive)
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
            ProcessAutoDrive(facade.MoveToTargetValue);
            jointRigidbody = joint.GetComponent<Rigidbody>();
            CalculateHingeLocation(facade.HingeLocation);
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

        /// <summary>
        /// Calculates a multiplier based on the direction the rotation is traveling.
        /// </summary>
        /// <returns>The multiplier that represents the direction.</returns>
        protected virtual float CalculateDirectionMultiplier()
        {
            return (Mathf.Lerp(facade.DriveLimit.minimum, facade.DriveLimit.maximum, facade.TargetValue) >= pseudoRotation ? 1f : -1f);
        }

        /// <summary>
        /// Applies the limits on the drive rotation.
        /// </summary>
        /// <returns>Whether the limits have been applied.</returns>
        protected virtual bool ApplyLimits()
        {
            if (pseudoRotation < DriveLimits.minimum)
            {
                joint.transform.localRotation = Quaternion.Euler(joint.axis * DriveLimits.minimum);
                return true;
            }
            else if (pseudoRotation > DriveLimits.maximum)
            {
                joint.transform.localRotation = Quaternion.Euler(joint.axis * DriveLimits.maximum);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to retrieve a simple x, y or z euler angle from the <see cref="Transform.localEulerAngles"/> utilizing any other axis rotation.
        /// </summary>
        /// <returns>The actual axis angle from 0f to 360f.</returns>
        protected virtual Vector3 GetSimpleEulerAngles()
        {
            Vector3 currentEulerAngle = joint.transform.localEulerAngles;
            if (facade.DriveAxis == DriveAxis.Axis.XAxis && !currentEulerAngle.y.ApproxEquals(0f, 1f))
            {
                currentEulerAngle.x = currentEulerAngle.y - (currentEulerAngle.x > (circleDegrees * 0.5f) ? currentEulerAngle.x - circleDegrees : currentEulerAngle.x);
            }
            return currentEulerAngle;
        }
    }
}