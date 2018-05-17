// Artificial Rotator|ArtificialControllables|120020
namespace VRTK.Controllables.ArtificialBased
{
    using UnityEngine;
    using VRTK.GrabAttachMechanics;
    using VRTK.SecondaryControllerGrabActions;

    /// <summary>
    /// A artificially simulated openable rotator.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
    /// 
    /// **Script Usage:**
    ///  * Create a rotator container GameObject and set the GameObject that is to become the rotator as a child of the newly created container GameObject.
    ///  * Place the `VRTK_ArtificialRotator` script onto the GameObject that is to become the rotatable object and ensure the Transform rotation is `0, 0, 0`.
    ///  * Create a nested GameObject under the rotator GameObject and position it where the hinge should operate.
    ///  * Apply the nested hinge GameObject to the `Hinge Point` parameter on the Artificial Rotator script.
    ///
    ///   > The rotator GameObject must not be at the root level and needs to have the Transform rotation set to `0,0,0`. This is the reason for the container GameObject requirement. Any positioning of the rotator must be set on the parent container GameObject.
    ///   > The Artificial Rotator script GameObject will become the child of a runtime created GameObject that determines the rotational offset for the rotator.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactables/Controllables/Artificial/VRTK_ArtificialRotator")]
    public class VRTK_ArtificialRotator : VRTK_BaseControllable
    {
        [Header("Hinge Settings")]

        [Tooltip("A Transform that denotes the position where the rotator will rotate around.")]
        public Transform hingePoint;
        [Tooltip("The minimum and maximum angle the rotator can rotate to.")]
        public Limits2D angleLimits = new Limits2D(-180f, 180f);
        [Tooltip("The angle at which the rotator rotation can be within the minimum or maximum angle before the minimum or maximum angles are considered reached.")]
        public float minMaxThresholdAngle = 1f;
        [Tooltip("The angle at which will be considered as the resting position of the rotator.")]
        public float restingAngle = 0f;
        [Tooltip("The threshold angle from the `Resting Angle` that the current angle of the rotator needs to be within to snap the rotator back to the `Resting Angle`.")]
        public float forceRestingAngleThreshold = 1f;
        [Tooltip("The target angle to rotate the rotator to.")]
        [SerializeField]
        protected float angleTarget = 0f;
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

        [Tooltip("If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.")]
        public bool precisionGrab = true;
        [Tooltip("The maximum distance the grabbing object is away from the rotator before it is automatically released.")]
        public float detachDistance = 1f;
        [Tooltip("Determines how the rotation of the object is calculated based on the action of the grabbing object.")]
        public VRTK_RotateTransformGrabAttach.RotationType rotationAction = VRTK_RotateTransformGrabAttach.RotationType.FollowAttachPoint;
        [Tooltip("The simulated friction when the rotator is grabbed.")]
        public float grabbedFriction = 1f;
        [Tooltip("The simulated friction when the rotator is released.")]
        public float releasedFriction = 1f;
        [Tooltip("A collection of GameObjects that will be used as the valid collisions to determine if the rotator can be interacted with.")]
        public GameObject[] onlyInteractWith = new GameObject[0];

        protected VRTK_InteractableObject controlInteractableObject;
        protected VRTK_RotateTransformGrabAttach controlGrabAttach;
        protected VRTK_SwapControllerGrabAction controlSecondaryGrabAction;
        protected bool createInteractableObject;
        protected GameObject rotatorContainer;
        protected bool rotationReset;
        protected bool stillResting;
        protected float previousValue;
        protected float previousAngleTarget;
        protected Transform savedParent;

        /// <summary>
        /// The GetValue method returns the current rotation value of the rotator.
        /// </summary>
        /// <returns>The actual rotation of the rotator.</returns>
        public override float GetValue()
        {
            return (controlGrabAttach != null ? controlGrabAttach.GetAngle() : 0f);
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
            SetAngleTarget(value);
        }

        /// <summary>
        /// The GetContainer method returns the GameObject that is generated to hold the rotator control.
        /// </summary>
        /// <returns>The GameObject container of the rotator control.</returns>
        public virtual GameObject GetContainer()
        {
            return rotatorContainer;
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
            angleTarget = SetAngleWithNormalizedValue(VRTK_SharedMethods.NormalizeValue(givenStepValue, stepValueRange.minimum, stepValueRange.maximum));
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
            return (controlGrabAttach != null ? Mathf.Lerp(controlGrabAttach.angleLimits.minimum, controlGrabAttach.angleLimits.maximum, Mathf.Clamp01(normalizedStepValue)) : 0f);
        }

        /// <summary>
        /// The SetAngleTarget method sets a target angle to rotate the rotator to.
        /// </summary>
        /// <param name="newAngle">The angle in which to rotate the rotator to.</param>
        public virtual void SetAngleTarget(float newAngle)
        {
            if (controlGrabAttach != null)
            {
                newAngle = Mathf.Clamp(newAngle, angleLimits.minimum, angleLimits.maximum);
                angleTarget = newAngle;
                SetRotation(angleTarget, 0f);
            }
        }

        /// <summary>
        /// The IsResting method returns whether the rotator is at the resting angle or within the resting angle threshold.
        /// </summary>
        /// <returns>Returns `true` if the rotator is at the resting angle or within the resting angle threshold.</returns>
        public override bool IsResting()
        {
            float currentValue = GetValue();
            return (!IsGrabbed() && (currentValue <= (restingAngle + minMaxThresholdAngle) && currentValue >= (restingAngle - minMaxThresholdAngle)));
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
            SetValue(storedValue);

            ResetParentContainer();
            base.OnEnable();
            rotatorContainer = gameObject;
            rotationReset = false;
            previousValue = float.MaxValue;
            SetupParentContainer();
            SetupInteractableObject();
            SetAngleTarget(angleTarget);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ManageInteractableListeners(false);
            ManageGrabbableListeners(false);
            if (createInteractableObject)
            {
                Destroy(controlInteractableObject);
            }
            RemoveParentContainer();
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

        protected virtual void SetupParentContainer()
        {
            if (hingePoint != null)
            {
                hingePoint.transform.SetParent(transform.parent);
                Vector3 storedScale = transform.localScale;
                rotatorContainer = new GameObject(VRTK_SharedMethods.GenerateVRTKObjectName(true, name, "Controllable", "ArtificialBased", "RotatorContainer"));
                rotatorContainer.transform.SetParent(transform.parent);
                rotatorContainer.transform.localPosition = transform.localPosition;
                rotatorContainer.transform.localRotation = transform.localRotation;
                rotatorContainer.transform.localScale = Vector3.one;
                transform.SetParent(rotatorContainer.transform);
                rotatorContainer.transform.localPosition = hingePoint.localPosition;
                transform.localPosition = -hingePoint.localPosition;
                transform.localScale = storedScale;
                hingePoint.transform.SetParent(transform);
            }
        }

        protected virtual void RemoveParentContainer()
        {
            if (rotatorContainer != null)
            {
                savedParent = rotatorContainer.transform.parent;
            }
        }

        protected virtual void ResetParentContainer()
        {
            if (savedParent != null)
            {
                transform.SetParent(savedParent);
                Destroy(rotatorContainer);
            }
        }

        protected virtual void SetupInteractableObject()
        {
            controlInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (controlInteractableObject == null)
            {
                controlInteractableObject = rotatorContainer.AddComponent<VRTK_InteractableObject>();
                controlInteractableObject.isGrabbable = true;
                controlInteractableObject.ignoredColliders = (onlyInteractWith.Length > 0 ? VRTK_SharedMethods.ColliderExclude(GetComponentsInChildren<Collider>(true), VRTK_SharedMethods.GetCollidersInGameObjects(onlyInteractWith, true, true)) : new Collider[0]);
                SetupGrabMechanic();
                SetupSecondaryAction();
            }
            ManageInteractableListeners(true);
        }

        protected virtual void SetupGrabMechanic()
        {
            if (controlInteractableObject != null)
            {
                controlGrabAttach = controlInteractableObject.gameObject.AddComponent<VRTK_RotateTransformGrabAttach>();
                SetGrabMechanicParameters();
                controlInteractableObject.grabAttachMechanicScript = controlGrabAttach;
                ManageGrabbableListeners(true);
            }
        }

        protected virtual void SetGrabMechanicParameters()
        {
            if (controlGrabAttach != null)
            {
                controlGrabAttach.precisionGrab = precisionGrab;
                controlGrabAttach.detachDistance = detachDistance;
                controlGrabAttach.rotationAction = rotationAction;
                controlGrabAttach.rotateAround = (VRTK_RotateTransformGrabAttach.RotationAxis)operateAxis;
                controlGrabAttach.rotationFriction = grabbedFriction;
                controlGrabAttach.releaseDecelerationDamper = releasedFriction;
                controlGrabAttach.angleLimits = angleLimits;
            }
        }

        protected virtual void SetupSecondaryAction()
        {
            if (controlInteractableObject != null)
            {
                controlSecondaryGrabAction = controlInteractableObject.gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
                controlInteractableObject.secondaryGrabActionScript = controlSecondaryGrabAction;
            }
        }

        protected virtual void ManageInteractableListeners(bool state)
        {
            if (controlInteractableObject != null)
            {
                if (state)
                {
                    controlInteractableObject.InteractableObjectGrabbed += InteractableObjectGrabbed;
                    controlInteractableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbed;
                }
                else
                {
                    controlInteractableObject.InteractableObjectGrabbed -= InteractableObjectGrabbed;
                    controlInteractableObject.InteractableObjectUngrabbed -= InteractableObjectUngrabbed;
                }
            }
        }

        protected virtual void InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            CheckLock();
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            rotationReset = false;
            ForceRestingPosition();
            ForceSnapToStep();
        }

        protected virtual void CheckLock()
        {
            if (controlGrabAttach != null)
            {
                SetGrabMechanicParameters();
                controlGrabAttach.angleLimits = (isLocked ? Limits2D.zero : angleLimits);
            }
        }

        protected virtual void ManageGrabbableListeners(bool state)
        {
            if (controlGrabAttach != null)
            {
                if (state)
                {
                    controlGrabAttach.AngleChanged += GrabMechanicAngleChanged;
                }
                else
                {
                    controlGrabAttach.AngleChanged -= GrabMechanicAngleChanged;
                }
            }
        }

        protected virtual void GrabMechanicAngleChanged(object sender, RotateTransformGrabAttachEventArgs e)
        {
            if (controlInteractableObject != null && !controlInteractableObject.IsGrabbed())
            {
                ForceRestingPosition();
                ForceSnapToStep();
            }
            if (processAtEndOfFrame == null)
            {
                EmitEvents();
            }
        }

        protected virtual float SetAngleWithNormalizedValue(float normalizedTargetAngle)
        {
            if (controlGrabAttach != null)
            {
                float positionOnAxis = Mathf.Lerp(controlGrabAttach.angleLimits.minimum, controlGrabAttach.angleLimits.maximum, Mathf.Clamp01(normalizedTargetAngle));
                SetRotation(positionOnAxis, releasedFriction * 0.1f);
                return positionOnAxis;
            }
            return 0f;
        }

        protected virtual void ForceRestingPosition()
        {
            if (!rotationReset && controlGrabAttach != null)
            {
                float currentValue = GetValue();
                if (currentValue <= (restingAngle + forceRestingAngleThreshold) && currentValue >= (restingAngle - forceRestingAngleThreshold))
                {

                    SetRotation(restingAngle, releasedFriction * 0.1f);
                }
            }
        }

        protected virtual void ForceSnapToStep()
        {
            bool validSnap = (snapToStep && controlGrabAttach != null && !IsGrabbed() && !rotationReset);
            bool notAtSnapValue = (Mathf.Abs(GetValue() - GetAngleFromStepValue(GetStepValue(GetValue()))) >= equalityFidelity);
            if (validSnap && notAtSnapValue)
            {
                SetAngleTargetWithStepValue(GetStepValue(GetValue()));
            }
        }

        protected virtual void SetRotation(float newAngle, float speed)
        {
            rotationReset = true;
            controlGrabAttach.SetRotation(newAngle, speed);
        }

        protected virtual bool IsGrabbed()
        {
            return (controlInteractableObject != null && controlInteractableObject.IsGrabbed());
        }
    }
}