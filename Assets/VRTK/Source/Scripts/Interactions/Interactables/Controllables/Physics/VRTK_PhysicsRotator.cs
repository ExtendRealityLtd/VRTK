// Physics Rotator|PhysicsControllables|110030
namespace VRTK.Controllables.PhysicsBased
{
    using UnityEngine;
    using VRTK.GrabAttachMechanics;
    using VRTK.SecondaryControllerGrabActions;

    /// <summary>
    /// A physics based rotatable object.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
    ///  * `Rigidbody` - A Unity Rigidbody to allow the GameObject to be affected by the Unity Physics System. Will be automatically added at runtime.
    ///
    /// **Optional Components:**
    ///  * `VRTK_ControllerRigidbodyActivator` - A Controller Rigidbody Activator to automatically enable the controller rigidbody when near the rotator.
    /// 
    /// **Script Usage:**
    ///  * Create a rotator container GameObject and set the GameObject that is to become the rotator as a child of the newly created container GameObject.
    ///  * Place the `VRTK_PhysicsRotator` script onto the GameObject that is to become the rotatable object and ensure the Transform rotation is `0, 0, 0`.
    ///  * Create a nested GameObject under the rotator GameObject and position it where the hinge should operate.
    ///  * Apply the nested hinge GameObject to the `Hinge Point` parameter on the Physics Rotator script.
    ///
    ///   > The rotator GameObject must not be at the root level and needs to have the Transform rotation set to `0,0,0`. This is the reason for the container GameObject requirement. Any positioning of the rotator must be set on the parent container GameObject.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactables/Controllables/Physics/VRTK_PhysicsRotator")]
    public class VRTK_PhysicsRotator : VRTK_BasePhysicsControllable
    {
        /// <summary>
        /// Type of Grab Mechanic
        /// </summary>
        public enum GrabMechanic
        {
            /// <summary>
            /// The Track Object Grab Mechanic
            /// </summary>
            TrackObject,
            /// <summary>
            /// The Rotator Track Grab Mechanic
            /// </summary>
            RotatorTrack
        }

        [Header("Hinge Settings")]

        [Tooltip("A Transform that denotes the position where the rotator hinge will be created.")]
        public Transform hingePoint;
        [Tooltip("The minimum and maximum angle the rotator can rotate to.")]
        [MinMaxRange(-180f, 180f)]
        public Limits2D angleLimits = new Limits2D(-180f, 180);
        [Tooltip("The angle at which the rotator rotation can be within the minimum or maximum angle before the minimum or maximum angles are considered reached.")]
        public float minMaxThresholdAngle = 1f;
        [Tooltip("The angle at which will be considered as the resting position of the rotator.")]
        public float restingAngle = 0f;
        [Tooltip("The threshold angle from the `Resting Angle` that the current angle of the rotator needs to be within to snap the rotator back to the `Resting Angle`.")]
        public float forceRestingAngleThreshold = 1f;
        [Tooltip("The target angle to rotate the rotator to.")]
        public float angleTarget = 0f;
        [Tooltip("If this is checked then the rotator Rigidbody will have all rotations frozen.")]
        public bool isLocked = false;

        [Header("Value Step Settings")]

        [Tooltip("The minimum and the maximum step values for the rotator to register along the `Operate Axis`.")]
        public Limits2D stepValueRange = new Limits2D(0f, 1f);
        [Tooltip("The increments the rotator value will change in between the `Step Value Range`.")]
        public float stepSize = 0.1f;
        [Tooltip("If this is checked then the value for the rotator will be the step value and not the absolute rotation of the rotator Transform.")]
        public bool useStepAsValue = true;

        [Header("Snap Settings")]

        [Tooltip("If this is checked then the rotator will snap to the angle of the nearest step along the value range.")]
        public bool snapToStep = false;
        [Tooltip("The speed in which the rotator will snap to the relevant angle along the `Operate Axis`")]
        public float snapForce = 10f;

        [Header("Interaction Settings")]

        [Tooltip("The type of Interactable Object grab mechanic to use when operating the rotator.")]
        public GrabMechanic grabMechanic = GrabMechanic.RotatorTrack;
        [Tooltip("If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.")]
        public bool precisionGrab = true;
        [Tooltip("The maximum distance the grabbing object is away from the rotator before it is automatically released.")]
        public float detachDistance = 1f;
        [Tooltip("If this is checked then the `Grabbed Friction` value will be used as the Rigidbody drag value when the rotator is grabbed and the `Released Friction` value will be used as the Rigidbody drag value when the door is released.")]
        public bool useFrictionOverrides = false;
        [Tooltip("The Rigidbody drag value when the rotator is grabbed.")]
        public float grabbedFriction = 0f;
        [Tooltip("The Rigidbody drag value when the rotator is released.")]
        public float releasedFriction = 0f;
        [Tooltip("A collection of GameObjects that will be used as the valid collisions to determine if the rotator can be interacted with.")]
        public GameObject[] onlyInteractWith = new GameObject[0];

        protected VRTK_InteractableObject controlInteractableObject;
        protected VRTK_TrackObjectGrabAttach controlGrabAttach;
        protected VRTK_SwapControllerGrabAction controlSecondaryGrabAction;
        protected bool createControlInteractableObject;
        protected HingeJoint controlJoint;
        protected JointSpring controlJointSpring = new JointSpring();
        protected JointLimits controlJointLimits = new JointLimits();
        protected bool createControlJoint;
        protected RigidbodyConstraints savedConstraints;
        protected bool stillLocked;
        protected bool stillResting;
        protected float previousValue;
        protected float previousAngleTarget;

        /// <summary>
        /// The GetValue method returns the current rotation value of the rotator.
        /// </summary>
        /// <returns>The actual rotation of the rotator.</returns>
        public override float GetValue()
        {
            float currentValue = transform.localEulerAngles[(int)operateAxis];
            return Quaternion.Angle(transform.localRotation, originalLocalRotation) * Mathf.Sign((currentValue > 180f ? currentValue - 360f : currentValue));
        }

        /// <summary>
        /// The GetNormalizedValue method returns the current rotation value of the rotator normalized between `0f` and `1f`.
        /// </summary>
        /// <returns>The normalized rotation of the rotator.</returns>
        public override float GetNormalizedValue()
        {
            return VRTK_SharedMethods.NormalizeValue(GetValue(), angleLimits.minimum, angleLimits.maximum);
        }

        /// <summary>
        /// The SetValue method sets the current Angle of the rotator
        /// </summary>
        /// <param name="value">The new rotation value</param>
        public override void SetValue(float value)
        {
            UpdateToAngle(value);
        }

        /// <summary>
        /// The GetStepValue method returns the current angle of the rotator based on the step value range.
        /// </summary>
        /// <param name="currentValue">The current angle value of the rotator to get the Step Value for.</param>
        /// <returns>The current Step Value based on the rotator angle.</returns>
        public virtual float GetStepValue(float currentValue)
        {
            float step = Mathf.Lerp(stepValueRange.minimum, stepValueRange.maximum, VRTK_SharedMethods.NormalizeValue(currentValue, angleLimits.minimum, angleLimits.maximum));
            return Mathf.Round(step / stepSize) * stepSize;
        }

        /// <summary>
        /// The SetAngleTargetWithStepValue sets the `Angle Target` parameter but uses a value within the `Step Value Range`.
        /// </summary>
        /// <param name="givenStepValue">The step value within the `Step Value Range` to set the `Angle Target` parameter to.</param>
        public virtual void SetAngleTargetWithStepValue(float givenStepValue)
        {
            angleTarget = VRTK_SharedMethods.NormalizeValue(givenStepValue, stepValueRange.minimum, stepValueRange.maximum);
            SetAngleWithNormalizedValue(angleTarget);
            previousAngleTarget = angleTarget;
        }

        /// <summary>
        /// The SetRestingAngleWithStepValue sets the `Resting Angle` parameter but uses a value within the `Step Value Range`.
        /// </summary>
        /// <param name="givenStepValue">The step value within the `Step Value Range` to set the `Resting Angle` parameter to.</param>
        public virtual void SetRestingAngleWithStepValue(float givenStepValue)
        {
            restingAngle = VRTK_SharedMethods.NormalizeValue(givenStepValue, stepValueRange.minimum, stepValueRange.maximum);
        }

        /// <summary>
        /// The GetAngleFromStepValue returns the angle the rotator would be at based on the given step value.
        /// </summary>
        /// <param name="givenStepValue">The step value to check the angle for.</param>
        /// <returns>The angle the rotator would be at based on the given step value.</returns>
        public virtual float GetAngleFromStepValue(float givenStepValue)
        {
            float normalizedStepValue = VRTK_SharedMethods.NormalizeValue(givenStepValue, stepValueRange.minimum, stepValueRange.maximum);
            return (controlJoint != null ? Mathf.Lerp(controlJoint.limits.min, controlJoint.limits.max, Mathf.Clamp01(normalizedStepValue)) : 0f);
        }

        /// <summary>
        /// The IsResting method returns whether the rotator is at the resting angle or within the resting angle threshold.
        /// </summary>
        /// <returns>Returns `true` if the rotator is at the resting angle or within the resting angle threshold.</returns>
        public override bool IsResting()
        {
            float currentValue = GetValue();
            return (!IsGrabbed() && (currentValue <= restingAngle + minMaxThresholdAngle && currentValue >= restingAngle - minMaxThresholdAngle));
        }

        /// <summary>
        /// The GetControlJoint method returns the joint associated with the control.
        /// </summary>
        /// <returns>The joint associated with the control.</returns>
        public virtual HingeJoint GetControlJoint()
        {
            return controlJoint;
        }

        /// <summary>
        /// The GetControlInteractableObject method returns the Interactable Object associated with the control.
        /// </summary>
        /// <returns>The Interactable Object associated with the control.</returns>
        public virtual VRTK_InteractableObject GetControlInteractableObject()
        {
            return controlInteractableObject;
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (hingePoint != null)
            {
                Bounds rotatorBounds = VRTK_SharedMethods.GetBounds(transform, transform);
                Vector3 limits = transform.rotation * ((AxisDirection() * rotatorBounds.size[(int)operateAxis]) * 0.53f);
                Vector3 hingeStart = hingePoint.transform.position - limits;
                Vector3 hingeEnd = hingePoint.transform.position + limits;
                Gizmos.DrawLine(hingeStart, hingeEnd);
                Gizmos.DrawSphere(hingeStart, 0.01f);
                Gizmos.DrawSphere(hingeEnd, 0.01f);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            stillLocked = false;
            stillResting = false;
            previousAngleTarget = float.MaxValue;
            previousValue = float.MaxValue;
            savedConstraints = controlRigidbody.constraints;
            SetupInteractableObject();
            SetupJoint();
            SetFrictions(releasedFriction);
            CheckLock();

            SetValue(storedValue);
        }

        protected override void OnDisable()
        {
            storedValue = GetValue();

            if (createControlJoint)
            {
                Destroy(controlJoint);
            }
            base.OnDisable();
            if (createControlInteractableObject)
            {
                ManageInteractableObjectListeners(false);
                Destroy(controlSecondaryGrabAction);
                Destroy(controlGrabAttach);
                Destroy(controlInteractableObject);
            }
            else
            {
                ManageInteractableObjectListeners(false);
            }
        }

        protected virtual void Update()
        {
            ForceRestingPosition();
            ForceAngleTarget();
            ForceSnapToStep();
            SetJointLimits();
            EmitEvents();
        }

        protected override void EmitEvents()
        {
            bool valueChanged = Mathf.Abs(GetValue() - previousValue) >= equalityFidelity;

            if (valueChanged)
            {
                ControllableEventArgs payload = EventPayload();
                float currentAngle = GetValue();
                float minThreshold = angleLimits.minimum + minMaxThresholdAngle;
                float maxThreshold = angleLimits.maximum - minMaxThresholdAngle;
                stillResting = false;

                OnValueChanged(payload);

                if (currentAngle >= maxThreshold && !AtMaxLimit())
                {
                    atMaxLimit = true;
                    OnMaxLimitReached(payload);
                }
                else if (currentAngle <= (angleLimits.minimum + minMaxThresholdAngle) && !AtMinLimit())
                {
                    atMinLimit = true;
                    OnMinLimitReached(payload);
                }
                else if (currentAngle > minThreshold && currentAngle < maxThreshold)
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

                previousValue = GetValue();
            }

            if (!stillResting && IsResting() && !valueChanged)
            {
                OnRestingPointReached(EventPayload());
                stillResting = true;
            }
        }

        protected override ControllableEventArgs EventPayload()
        {
            ControllableEventArgs e = base.EventPayload();
            e.value = (useStepAsValue ? GetStepValue(GetValue()) : GetValue());
            return e;
        }

        protected virtual void SetupJoint()
        {
            createControlJoint = false;
            controlJoint = GetComponent<HingeJoint>();
            if (controlJoint == null && hingePoint != null)
            {
                controlJoint = gameObject.AddComponent<HingeJoint>();
                createControlJoint = true;
                controlJoint.axis = AxisDirection();
                controlJoint.connectedBody = connectedTo;
                hingePoint.SetParent(transform);
                controlJoint.anchor = (hingePoint != null ? hingePoint.localPosition : Vector3.zero);
                controlJoint.useLimits = true;
                SetJointLimits();
            }
        }

        protected virtual void SetJointLimits()
        {
            if (controlJoint != null)
            {
                controlJointLimits.min = angleLimits.minimum;
                controlJointLimits.max = angleLimits.maximum;
                controlJoint.limits = controlJointLimits;
            }
        }

        protected virtual void ManageSpring(bool activate, float springTarget)
        {
            if (controlJoint != null)
            {
                controlJoint.useSpring = activate;
                controlJointSpring.spring = 100f;
                controlJointSpring.damper = 10f;
                controlJointSpring.targetPosition = springTarget;
                controlJoint.spring = controlJointSpring;
            }
        }

        protected virtual void SetupInteractableObject()
        {
            createControlInteractableObject = false;
            controlInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (controlInteractableObject == null)
            {
                controlInteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
                createControlInteractableObject = true;
                controlInteractableObject.isGrabbable = true;
                controlInteractableObject.ignoredColliders = (onlyInteractWith.Length > 0 ? VRTK_SharedMethods.ColliderExclude(GetComponentsInChildren<Collider>(true), VRTK_SharedMethods.GetCollidersInGameObjects(onlyInteractWith, true, true)) : new Collider[0]);

                SetupGrabMechanic();
                SetupSecondaryAction();
                ManageInteractableObjectListeners(true);
            }
        }

        protected virtual void SetupGrabMechanic()
        {
            switch (grabMechanic)
            {
                case GrabMechanic.TrackObject:
                    controlGrabAttach = controlInteractableObject.gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
                    break;
                case GrabMechanic.RotatorTrack:
                    controlGrabAttach = controlInteractableObject.gameObject.AddComponent<VRTK_RotatorTrackGrabAttach>();
                    break;
            }
            SetGrabMechanicParameters();
            controlInteractableObject.grabAttachMechanicScript = controlGrabAttach;
        }

        protected virtual void SetGrabMechanicParameters()
        {
            if (controlGrabAttach != null)
            {
                controlGrabAttach.precisionGrab = precisionGrab;
                controlGrabAttach.detachDistance = detachDistance;
            }
        }

        protected virtual void SetupSecondaryAction()
        {
            controlSecondaryGrabAction = controlInteractableObject.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
            controlInteractableObject.secondaryGrabActionScript = controlSecondaryGrabAction;
        }

        protected virtual void ManageInteractableObjectListeners(bool state)
        {
            if (controlInteractableObject != null)
            {
                if (state)
                {
                    controlInteractableObject.InteractableObjectTouched += InteractableObjectTouched;
                    controlInteractableObject.InteractableObjectUntouched += InteractableObjectUntouched;
                    controlInteractableObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
                    controlInteractableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
                }
                else
                {
                    controlInteractableObject.InteractableObjectTouched -= InteractableObjectTouched;
                    controlInteractableObject.InteractableObjectUntouched -= InteractableObjectUntouched;
                    controlInteractableObject.InteractableObjectGrabbed -= InteractableObjectGrabbed;
                    controlInteractableObject.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
                }
            }
        }

        protected virtual void InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
        {
            CheckLock();
            if (GetControlActivatorContainer() != null)
            {
                AttemptMove();
            }
        }

        protected virtual void InteractableObjectUntouched(object sender, InteractableObjectEventArgs e)
        {
            CheckLock();
            if (GetControlActivatorContainer() != null)
            {
                AttemptRelease();
            }
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            SetGrabMechanicParameters();
            AttemptMove();
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            AttemptRelease();
        }

        protected virtual void AttemptMove()
        {
            SetFrictions(grabbedFriction);
            ManageSpring(false, restingAngle);
        }

        protected virtual void AttemptRelease()
        {
            SetFrictions(releasedFriction);
        }

        protected virtual void SetFrictions(float frictionValue)
        {
            if (useFrictionOverrides)
            {
                SetRigidbodyDrag(frictionValue);
                SetRigidbodyAngularDrag(frictionValue);
            }
        }

        protected virtual void CheckLock()
        {
            if (controlRigidbody != null)
            {
                if (isLocked && !stillLocked)
                {
                    savedConstraints = controlRigidbody.constraints;
                    SetRigidbodyConstraints(RigidbodyConstraints.FreezeRotation);
                    stillLocked = true;
                }
                else if (!isLocked && stillLocked)
                {
                    SetRigidbodyConstraints(savedConstraints);
                    stillLocked = false;
                }
            }
        }

        protected virtual void SetAngleWithNormalizedValue(float normalizedTargetAngle)
        {
            if (controlJoint != null)
            {
                float positionOnAxis = Mathf.Lerp(controlJoint.limits.min, controlJoint.limits.max, Mathf.Clamp01(normalizedTargetAngle));
                UpdateToAngle(positionOnAxis);
            }
        }

        protected virtual void UpdateToAngle(float givenTargetAngle)
        {
            bool activateSpring = (Mathf.Abs(GetValue() - givenTargetAngle) >= equalityFidelity);
            ManageSpring(activateSpring, givenTargetAngle);
        }

        protected virtual void ForceRestingPosition()
        {
            bool validReset = (controlJoint != null && !controlJoint.useSpring && !IsGrabbed() && (GetControlActivatorContainer() == null || !controlInteractableObject.IsTouched()));
            float currentValue = GetValue();
            float restingAngleMin = restingAngle - forceRestingAngleThreshold;
            float restingAngleMax = restingAngle + forceRestingAngleThreshold;
            if (validReset && currentValue > restingAngleMin && currentValue < restingAngleMax)
            {
                ManageSpring(true, restingAngle);
            }
        }

        protected virtual void ForceAngleTarget()
        {
            if (!IsGrabbed() && previousAngleTarget != angleTarget)
            {
                UpdateToAngle(angleTarget);
            }
            previousAngleTarget = angleTarget;
        }

        protected virtual void ForceSnapToStep()
        {
            bool validSnap = (snapToStep && controlJoint != null && !IsGrabbed() && !controlJoint.useSpring);
            bool notAtSnapValue = (Mathf.Abs(GetValue() - GetAngleFromStepValue(GetStepValue(GetValue()))) >= equalityFidelity);
            if (validSnap && notAtSnapValue)
            {
                SetAngleTargetWithStepValue(GetStepValue(GetValue()));
            }
        }

        protected virtual bool IsGrabbed()
        {
            return (controlInteractableObject != null && controlInteractableObject.IsGrabbed());
        }
    }
}