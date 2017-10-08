// Physics Button|PhysicsControllables|110020
namespace VRTK.Controllables.PhysicsBased
{
    using UnityEngine;

    /// <summary>
    /// A physics based pushable button.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
    ///  * `Rigidbody` - A Unity Rigidbody to allow the GameObject to be affected by the Unity Physics System. Will be automatically added at runtime.
    ///
    /// **Optional Components:**
    ///  * `ConstantForce` - A Unity Constant Force to push the button back to it's origin position. Will be automatically created if the `Reset Force` is not `0f`.
    ///  * `VRTK_ControllerRigidbodyActivator` - A Controller Rigidbody Activator to automatically enable the controller rigidbody upon touching the button. Will be automatically created if the `Auto Interaction` paramter is checked.
    /// 
    /// **Script Usage:**
    ///  * Place the `VRTK_Button` script onto the GameObject that is to become the button.
    /// </remarks>
    public class VRTK_Button : VRTK_BasePhysicsControllable
    {
        [Header("Button Settings")]

        [Tooltip("The distance along the `Operate Axis` until the button reaches the pressed position.")]
        public float pressedDistance = 0.1f;
        [Tooltip("The threshold in which the button's current position along the `Operate Axis` has to be within the pressed position for the button to be considered pressed.")]
        public float pressedThreshold = 0f;
        [Tooltip("If this is checked then the button will stay in the pressed position when it reaches the pressed position.")]
        public bool stayPressed = false;
        [Tooltip("The position of the button between the original position and the pressed position. `0f` will set the button position to the original position, `1f` will set the button position to the pressed position.")]
        [Range(0f, 1f)]
        public float positionTarget = 0f;
        [Tooltip("The amount of force to apply to push the Button towards the `Target Position`")]
        public float targetForce = 10f;

        protected ConfigurableJoint controlJoint;
        protected bool createCustomJoint;

        protected Vector3 targetLocalPosition;
        protected Vector3 previousLocalPosition;
        protected bool pressedDown;

        /// <summary>
        /// The GetValue method returns the current position value of the button.
        /// </summary>
        /// <returns>The actual position of the button.</returns>
        public override float GetValue()
        {
            return transform.localPosition[(int)operateAxis];
        }

        /// <summary>
        /// The GetNormalizedValue method returns the current position value of the button normalized between `0f` and `1f`.
        /// </summary>
        /// <returns>The normalized position of the button.</returns>
        public override float GetNormalizedValue()
        {
            return VRTK_SharedMethods.NormalizeValue(GetValue(), originalLocalPosition[(int)operateAxis], PressedPosition()[(int)operateAxis]);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetupJoint();
            targetLocalPosition = Vector3.zero;
            previousLocalPosition = transform.localPosition;
            pressedDown = false;
        }

        protected override void OnDisable()
        {
            if (createCustomJoint)
            {
                Destroy(controlJoint);
            }
            base.OnDisable();
        }

        protected virtual void FixedUpdate()
        {
            if (controlRigidbody != null)
            {
                controlRigidbody.velocity = Vector3.zero;
                ForceLocalPosition();
            }
        }

        protected virtual void Update()
        {
            if (transform.localPosition != previousLocalPosition)
            {
                OnValueChanged(EventPayload());
                CheckPressEvents();
                previousLocalPosition = transform.localPosition;
            }

            if (!stayPressed && pressedDown)
            {
                controlRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            SetTargetPosition();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Vector3 objectHalf = AxisDirection(true) * (transform.localScale[(int)operateAxis] * 0.5f);
            Vector3 initialPoint = transform.position + (objectHalf * PressedDirection());
            Vector3 destinationPoint = initialPoint + (AxisDirection(true) * pressedDistance);
            Gizmos.DrawLine(initialPoint, destinationPoint);
            Gizmos.DrawSphere(destinationPoint, 0.01f);
        }

        protected override void ConfigueRigidbody()
        {
            controlRigidbody.useGravity = false;
            controlRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            controlRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        protected virtual void ForceLocalPosition()
        {
            float xPos = (operateAxis == OperatingAxis.xAxis ? transform.localPosition.x : originalLocalPosition.x);
            float yPos = (operateAxis == OperatingAxis.yAxis ? transform.localPosition.y : originalLocalPosition.y);
            float zPos = (operateAxis == OperatingAxis.zAxis ? transform.localPosition.z : originalLocalPosition.z);
            transform.localPosition = new Vector3(xPos, yPos, zPos);
        }

        protected virtual void SetTargetPosition()
        {
            if (controlJoint != null)
            {
                controlJoint.targetPosition = (AxisDirection() * PressedDirection()) * Mathf.Lerp(controlJoint.linearLimit.limit, -controlJoint.linearLimit.limit, positionTarget);
            }
        }

        protected virtual Vector3 PressedPosition()
        {
            return originalLocalPosition + (AxisDirection() * pressedDistance);
        }

        protected virtual void SetupJoint()
        {
            //move transform towards activation distance
            transform.localPosition += AxisDirection() * (pressedDistance * 0.5f);

            controlJoint = GetComponent<ConfigurableJoint>();
            createCustomJoint = false;
            if (controlJoint == null)
            {
                controlJoint = gameObject.AddComponent<ConfigurableJoint>();
                createCustomJoint = true;

                controlJoint.angularXMotion = ConfigurableJointMotion.Locked;
                controlJoint.angularYMotion = ConfigurableJointMotion.Locked;
                controlJoint.angularZMotion = ConfigurableJointMotion.Locked;

                controlJoint.xMotion = (operateAxis == OperatingAxis.xAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
                controlJoint.yMotion = (operateAxis == OperatingAxis.yAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);
                controlJoint.zMotion = (operateAxis == OperatingAxis.zAxis ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Locked);

                JointDrive snapDriver = new JointDrive();
                snapDriver.positionSpring = 1000f;
                snapDriver.positionDamper = 10f;
                snapDriver.maximumForce = targetForce;

                controlJoint.xDrive = snapDriver;
                controlJoint.yDrive = snapDriver;
                controlJoint.zDrive = snapDriver;

                SoftJointLimit linearLimit = new SoftJointLimit();
                linearLimit.limit = Mathf.Abs(pressedDistance * 0.5f);
                controlJoint.linearLimit = linearLimit;
                controlJoint.connectedBody = connectedTo;
            }
        }

        protected virtual float PressedDirection()
        {
            return (pressedDistance > 0f ? 1f : -1f);
        }

        protected virtual void CheckPressEvents()
        {
            float currentPosition = GetNormalizedValue();
            ControllableEventArgs payload = EventPayload();
            if (currentPosition >= (1f - pressedThreshold) && !AtMaxLimit())
            {
                atMaxLimit = true;
                OnMaxLimitReached(payload);
                StickOnPressed();
            }
            else if (currentPosition <= (0f + pressedThreshold) && !AtMinLimit())
            {
                atMinLimit = true;
                OnMinLimitReached(payload);
            }
            else if (currentPosition > pressedThreshold && currentPosition < (1f - pressedThreshold))
            {
                if (AtMinLimit())
                {
                    OnMinLimitExited(payload);
                }
                if (AtMaxLimit())
                {
                    OnMaxLimitExited(payload);
                }

                atMinLimit = false;
                atMaxLimit = false;
            }
        }

        protected virtual void StickOnPressed()
        {
            if (stayPressed && controlRigidbody != null)
            {
                controlRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                pressedDown = true;
            }
        }
    }
}