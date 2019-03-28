namespace VRTK.Prefabs.Interactions.Controllables
{
    using UnityEngine;
    using System.Collections.Generic;
    using Malimbe.XmlDocumentationAttribute;
    using Malimbe.PropertySerializationAttribute;
    using Malimbe.BehaviourStateRequirementMethod;
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
        [Serialized]
        [field: Header("Joint Settings"), DocumentedByXml, Restricted]
        public HingeJoint Joint { get; protected set; }
        /// <summary>
        /// The parent <see cref="GameObject"/> of the joint.
        /// </summary>
        [Serialized]
        [field: DocumentedByXml, Restricted]
        public GameObject JointContainer { get; protected set; }
        #endregion

        /// <summary>
        /// The <see cref="Rigidbody"/> that the joint is using.
        /// </summary>
        protected Rigidbody jointRigidbody;
        /// <summary>
        /// The motor data to set on the joint.
        /// </summary>
        protected JointMotor jointMotor;
        /// <summary>
        /// A reusable collection to hold the active state of children of the <see cref="Rigidbody"/>s that are adjusted.
        /// </summary>
        protected readonly List<bool> rigidbodyChildrenActiveStates = new List<bool>();

        /// <inheritdoc />
        [RequiresBehaviourState]
        public override Vector3 CalculateDriveAxis(DriveAxis.Axis driveAxis)
        {
            Vector3 axisDirection = base.CalculateDriveAxis(driveAxis);
            Joint.axis = axisDirection * -1f;
            return axisDirection;
        }

        /// <inheritdoc />
        [RequiresBehaviourState]
        public override void CalculateHingeLocation(Vector3 newHingeLocation)
        {
            Joint.anchor = newHingeLocation;
            SetJointContainerPosition();
        }

        /// <inheritdoc />
        [RequiresBehaviourState]
        public override void ConfigureAutoDrive(bool autoDrive)
        {
            Joint.useMotor = autoDrive;
        }

        /// <inheritdoc />
        public override void ApplyExistingAngularVelocity(float multiplier = 1f)
        {
            jointRigidbody.angularVelocity = AxisDirection * pseudoAngularVelocity * multiplier;
        }

        /// <inheritdoc />
        protected override void SetUpInternals()
        {
            jointMotor.force = Joint.motor.force;
            jointRigidbody = Joint.GetComponent<Rigidbody>();
            ConfigureAutoDrive(Facade.MoveToTargetValue);
            base.SetUpInternals();
        }

        /// <inheritdoc />
        protected override Transform GetDriveTransform()
        {
            return Joint.transform;
        }

        /// <inheritdoc />
        protected override void ProcessAutoDrive(float driveSpeed)
        {
            jointMotor.targetVelocity = driveSpeed;
            Joint.motor = jointMotor;
        }

        /// <inheritdoc />
        protected override void AttemptApplyLimits()
        {
            if (!Joint.useLimits && ApplyLimits())
            {
                jointRigidbody.velocity = Vector3.zero;
                jointRigidbody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Sets the <see cref="JointContainer"/> position based on the joint anchor.
        /// </summary>
        protected virtual void SetJointContainerPosition()
        {
            // Disable any child rigidbody GameObjects of the joint to prevent the anchor position updating their position.
            Rigidbody[] rigidbodyChildren = Joint.GetComponentsInChildren<Rigidbody>();
            rigidbodyChildrenActiveStates.Clear();
            for (int index = 0; index < rigidbodyChildren.Length; index++)
            {
                if (rigidbodyChildren[index].gameObject == JointContainer || rigidbodyChildren[index] == jointRigidbody)
                {
                    rigidbodyChildrenActiveStates.Add(false);
                    continue;
                }

                rigidbodyChildrenActiveStates.Add(rigidbodyChildren[index].gameObject.activeSelf);
                rigidbodyChildren[index].gameObject.SetActive(false);
            }

            // Set the current joint container to match the joint anchor to provide the correct offset.
            JointContainer.transform.localPosition = Joint.anchor;
            JointContainer.transform.localRotation = Quaternion.identity;

            // Restore the state of child rigidbody GameObjects now the anchor position has been set.
            for (int index = 0; index < rigidbodyChildren.Length; index++)
            {
                if (rigidbodyChildren[index].gameObject == JointContainer || rigidbodyChildren[index] == jointRigidbody)
                {
                    continue;
                }

                rigidbodyChildren[index].gameObject.SetActive(rigidbodyChildrenActiveStates[index]);
            }
        }
    }
}