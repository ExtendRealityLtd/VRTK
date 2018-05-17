// Artificial Slider|ArtificialControllables|120030
namespace VRTK.Controllables.ArtificialBased
{
    using UnityEngine;
    using System.Collections;
    using VRTK.GrabAttachMechanics;
    using VRTK.SecondaryControllerGrabActions;

    /// <summary>
    /// A artificially simulated slider.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
    ///
    /// **Script Usage:**
    ///  * Create a slider container GameObject and set the GameObject that is to become the slider as a child of the container.
    ///  * Place the `VRTK_ArtificialSlider` script onto the GameObject that is to become the slider.
    ///
    ///   > The slider GameObject must not be at the root level and needs to have it's Transform position set to `0,0,0`. This is the reason for the container GameObject requirement. Any positioning of the slider must be set on the parent GameObject.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactables/Controllables/Artificial/VRTK_ArtificialSlider")]
    public class VRTK_ArtificialSlider : VRTK_BaseControllable
    {
        [Header("Slider Settings")]

        [Tooltip("The maximum length that the slider can be moved from the origin position across the `Operate Axis`. A negative value will allow it to move the opposite way.")]
        public float maximumLength = 0.1f;
        [Tooltip("The normalized position the slider can be within the minimum or maximum slider positions before the minimum or maximum positions are considered reached.")]
        public float minMaxThreshold = 0.01f;
        [Tooltip("The target position to move the slider towards given in a normalized value of `0f` (start point) to `1f` (end point).")]
        [Range(0f, 1f)]
        [SerializeField]
        protected float positionTarget = 0f;
        [Tooltip("The position the slider when it is at the default resting point given in a normalized value of `0f` (start point) to `1f` (end point).")]
        [Range(0f, 1f)]
        public float restingPosition = 0f;
        [Tooltip("The normalized threshold value the slider has to be within the `Resting Position` before the slider is forced back to the `Resting Position` if it is not grabbed.")]
        [Range(0f, 1f)]
        public float forceRestingPositionThreshold = 0f;

        [Header("Value Step Settings")]

        [Tooltip("The minimum and the maximum step values for the slider to register along the `Operate Axis`.")]
        public Limits2D stepValueRange = new Limits2D(0f, 1f);
        [Tooltip("The increments the slider value will change in between the `Step Value Range`.")]
        public float stepSize = 0.1f;
        [Tooltip("If this is checked then the value for the slider will be the step value and not the absolute position of the slider Transform.")]
        public bool useStepAsValue = true;

        [Header("Snap Settings")]

        [Tooltip("If this is checked then the slider will snap to the position of the nearest step along the value range.")]
        public bool snapToStep = false;
        [Tooltip("The speed in which the slider will snap to the relevant point along the `Operate Axis`")]
        public float snapForce = 10f;

        [Header("Interaction Settings")]

        [Tooltip("The speed in which to track the grabbed slider to the interacting object.")]
        public float trackingSpeed = 25f;
        [Tooltip("If this is checked then when the Interact Grab grabs the Interactable Object, it will grab it with precision and pick it up at the particular point on the Interactable Object that the Interact Touch is touching.")]
        public bool precisionGrab = true;
        [Tooltip("The maximum distance the grabbing object is away from the slider before it is automatically released.")]
        public float detachDistance = 1f;
        [Tooltip("The amount of friction to the slider Rigidbody when it is released.")]
        public float releaseFriction = 10f;
        [Tooltip("A collection of GameObjects that will be used as the valid collisions to determine if the door can be interacted with.")]
        public GameObject[] onlyInteractWith = new GameObject[0];

        protected VRTK_InteractableObject controlInteractableObject;
        protected VRTK_MoveTransformGrabAttach controlGrabAttach;
        protected VRTK_SwapControllerGrabAction controlSecondaryGrabAction;
        protected bool createInteractableObject;
        protected Limits2D axisLimits;
        protected Vector3 previousLocalPosition;
        protected Coroutine setPositionTargetAtEndOfFrameRoutine;
        protected bool stillResting;

        /// <summary>
        /// The GetValue method returns the current position value of the slider.
        /// </summary>
        /// <returns>The actual position of the button.</returns>
        public override float GetValue()
        {
            return transform.localPosition[(int)operateAxis];
        }

        /// <summary>
        /// The GetNormalizedValue method returns the current position value of the slider normalized between `0f` and `1f`.
        /// </summary>
        /// <returns>The normalized position of the button.</returns>
        public override float GetNormalizedValue()
        {
            return VRTK_SharedMethods.NormalizeValue(GetValue(), originalLocalPosition[(int)operateAxis], MaximumLength()[(int)operateAxis]);
        }

        /// <summary>
        /// The SetValue method sets the current position value of the slider
        /// </summary>
        /// <param name="value">The new position value</param>
        public override void SetValue(float value)
        {
            Vector3 tempPos = new Vector3();
            tempPos = transform.localPosition;
            tempPos[(int)operateAxis] = value;

            transform.localPosition = tempPos;
        }

        /// <summary>
        /// The GetStepValue method returns the current position of the slider based on the step value range.
        /// </summary>
        /// <param name="currentValue">The current position value of the slider to get the Step Value for.</param>
        /// <returns>The current Step Value based on the slider position.</returns>
        public virtual float GetStepValue(float currentValue)
        {
            return Mathf.Round((stepValueRange.minimum + Mathf.Clamp01(currentValue / maximumLength) * (stepValueRange.maximum - stepValueRange.minimum)) / stepSize) * stepSize;
        }

        /// <summary>
        /// The SetPositionTarget method allows the setting of the `Position Target` parameter at runtime.
        /// </summary>
        /// <param name="newPositionTarget">The new position target value.</param>
        /// <param name="speed">The speed to move to the new position target.</param>
        public virtual void SetPositionTarget(float newPositionTarget, float speed)
        {
            positionTarget = newPositionTarget;
            SetPositionWithNormalizedValue(positionTarget, speed);
        }

        /// <summary>
        /// The SetPositionTargetWithStepValue sets the `Position Target` parameter but uses a value within the `Step Value Range`.
        /// </summary>
        /// <param name="givenStepValue">The step value within the `Step Value Range` to set the `Position Target` parameter to.</param>
        /// <param name="speed">The speed to move to the new position target.</param>
        public virtual void SetPositionTargetWithStepValue(float givenStepValue, float speed)
        {
            SetPositionTarget(VRTK_SharedMethods.NormalizeValue(givenStepValue, stepValueRange.minimum, stepValueRange.maximum), speed);
        }

        /// <summary>
        /// The SetRestingPositionWithStepValue sets the `Resting Position` parameter but uses a value within the `Step Value Range`.
        /// </summary>
        /// <param name="givenStepValue">The step value within the `Step Value Range` to set the `Resting Position` parameter to.</param>
        public virtual void SetRestingPositionWithStepValue(float givenStepValue)
        {
            restingPosition = VRTK_SharedMethods.NormalizeValue(givenStepValue, stepValueRange.minimum, stepValueRange.maximum);
        }

        /// <summary>
        /// The GetPositionFromStepValue returns the position the slider would be at based on the given step value.
        /// </summary>
        /// <param name="givenStepValue">The step value to check the position for.</param>
        /// <returns>The position the slider would be at based on the given step value.</returns>
        public virtual float GetPositionFromStepValue(float givenStepValue)
        {
            float normalizedStepValue = VRTK_SharedMethods.NormalizeValue(givenStepValue, stepValueRange.minimum, stepValueRange.maximum);
            return Mathf.Lerp(axisLimits.minimum, axisLimits.maximum, Mathf.Clamp01(normalizedStepValue));
        }

        /// <summary>
        /// The IsResting method returns whether the slider is at the resting position or within the resting position threshold.
        /// </summary>
        /// <returns>Returns `true` if the slider is at the resting position or within the resting position threshold.</returns>
        public override bool IsResting()
        {
            float currentValue = GetNormalizedValue();
            return (!IsGrabbed() && currentValue <= (restingPosition + forceRestingPositionThreshold) && currentValue >= (restingPosition - forceRestingPositionThreshold));
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
            Vector3 initialPoint = transform.position;
            base.OnDrawGizmosSelected();
            Vector3 destinationPoint = initialPoint + (AxisDirection(true) * maximumLength);
            Gizmos.DrawLine(initialPoint, destinationPoint);
            Gizmos.DrawSphere(initialPoint, 0.01f);
            Gizmos.DrawSphere(destinationPoint, 0.01f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            SetValue(storedValue);

            previousLocalPosition = Vector3.one * float.MaxValue;
            stillResting = false;
            SetupInteractableObject();
            setPositionTargetAtEndOfFrameRoutine = StartCoroutine(SetPositionTargetAtEndOfFrameRoutine());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ManageInteractableListeners(false);
            if (createInteractableObject)
            {
                Destroy(controlInteractableObject);
            }
            if (setPositionTargetAtEndOfFrameRoutine != null)
            {
                StopCoroutine(setPositionTargetAtEndOfFrameRoutine);
            }
            transform.localPosition = Vector3.zero;
        }

        protected override void EmitEvents()
        {
            bool valueChanged = !VRTK_SharedMethods.Vector3ShallowCompare(transform.localPosition, previousLocalPosition, equalityFidelity);

            if (valueChanged)
            {
                float currentPosition = GetNormalizedValue();
                float minThreshold = minMaxThreshold;
                float maxThreshold = 1f - minMaxThreshold;
                stillResting = false;

                ControllableEventArgs payload = EventPayload();
                OnValueChanged(payload);

                if (currentPosition >= maxThreshold && !AtMaxLimit())
                {
                    atMaxLimit = true;
                    OnMaxLimitReached(payload);
                }
                else if (currentPosition <= minThreshold && !AtMinLimit())
                {
                    atMinLimit = true;
                    OnMinLimitReached(payload);
                }
                else if (currentPosition > minThreshold && currentPosition < maxThreshold)
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

            if (!stillResting && IsResting() && !valueChanged)
            {
                OnRestingPointReached(EventPayload());
                stillResting = true;
            }

            previousLocalPosition = transform.localPosition;
        }

        protected override ControllableEventArgs EventPayload()
        {
            ControllableEventArgs e = base.EventPayload();
            e.value = (useStepAsValue ? GetStepValue(GetValue()) : GetValue());
            return e;
        }

        protected virtual IEnumerator SetPositionTargetAtEndOfFrameRoutine()
        {
            yield return new WaitForEndOfFrame();
            SetPositionTarget(positionTarget, 0f);
            if (snapToStep)
            {
                SetPositionTargetWithStepValue(GetStepValue(GetValue()), snapForce);
            }
            EmitEvents();
        }

        protected virtual void SetupInteractableObject()
        {
            controlInteractableObject = GetComponent<VRTK_InteractableObject>();
            if (controlInteractableObject == null)
            {
                controlInteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
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
                controlGrabAttach = controlInteractableObject.gameObject.AddComponent<VRTK_MoveTransformGrabAttach>();
                SetGrabMechanicParameters();
                controlInteractableObject.grabAttachMechanicScript = controlGrabAttach;
                ManageGrabbableListeners(true);
                controlGrabAttach.ResetState();
            }
        }

        protected virtual void SetGrabMechanicParameters()
        {
            if (controlGrabAttach != null)
            {
                controlGrabAttach.precisionGrab = precisionGrab;
                controlGrabAttach.releaseDecelerationDamper = releaseFriction;
                axisLimits = new Limits2D(originalLocalPosition[(int)operateAxis], MaximumLength()[(int)operateAxis]);
                switch (operateAxis)
                {
                    case OperatingAxis.xAxis:
                        controlGrabAttach.xAxisLimits = axisLimits;
                        break;
                    case OperatingAxis.yAxis:
                        controlGrabAttach.yAxisLimits = axisLimits;
                        break;
                    case OperatingAxis.zAxis:
                        controlGrabAttach.zAxisLimits = axisLimits;
                        break;
                }
                controlGrabAttach.trackingSpeed = trackingSpeed;
                controlGrabAttach.detachDistance = detachDistance;
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

        protected virtual Vector3 MaximumLength()
        {
            return originalLocalPosition + (AxisDirection() * maximumLength);
        }

        protected virtual void SetPositionWithNormalizedValue(float givenTargetPosition, float speed)
        {
            float positionOnAxis = Mathf.Lerp(axisLimits.minimum, axisLimits.maximum, Mathf.Clamp01(givenTargetPosition));
            SnapToPosition(positionOnAxis, speed);
        }

        protected virtual void SnapToPosition(float positionOnAxis, float speed)
        {
            if (controlGrabAttach != null)
            {
                controlGrabAttach.SetCurrentPosition((AxisDirection() * Mathf.Sign(maximumLength)) * positionOnAxis, speed);
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
            SetGrabMechanicParameters();
        }

        protected virtual void InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            SetGrabMechanicParameters();
            if (snapToStep)
            {
                SetPositionTargetWithStepValue(GetStepValue(GetValue()), snapForce);
            }

            if (ForceRestingPosition())
            {
                SetPositionWithNormalizedValue(restingPosition, snapForce);
            }
        }

        protected virtual bool ForceRestingPosition()
        {
            return (forceRestingPositionThreshold > 0f && !IsGrabbed() && (Mathf.Abs(restingPosition - GetNormalizedValue()) <= forceRestingPositionThreshold));
        }

        protected virtual bool IsGrabbed()
        {
            return (controlInteractableObject != null && controlInteractableObject.IsGrabbed());
        }

        protected virtual void ManageGrabbableListeners(bool state)
        {
            if (controlGrabAttach != null)
            {
                if (state)
                {
                    controlGrabAttach.TransformPositionChanged += GrabMechanicTransformPositionChanged;
                }
                else
                {
                    controlGrabAttach.TransformPositionChanged -= GrabMechanicTransformPositionChanged;
                }
            }
        }

        protected virtual void GrabMechanicTransformPositionChanged(object sender, MoveTransformGrabAttachEventArgs e)
        {
            EmitEvents();
        }
    }
}