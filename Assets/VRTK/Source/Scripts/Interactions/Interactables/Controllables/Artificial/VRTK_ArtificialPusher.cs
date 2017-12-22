// Artificial Pusher|ArtificialControllables|120010
namespace VRTK.Controllables.ArtificialBased
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// An artificially simulated pushable pusher.
    /// </summary>
    /// <remarks>
    /// **Required Components:**
    ///  * `Collider` - A Unity Collider to determine when an interaction has occured. Can be a compound collider set in child GameObjects. Will be automatically added at runtime.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_ArtificialPusher` script onto the GameObject that is to become the pusher.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactables/Controllables/Artificial/VRTK_ArtificialPusher")]
    public class VRTK_ArtificialPusher : VRTK_BaseControllable
    {
        [Header("Pusher Settings")]

        [Tooltip("The distance along the `Operate Axis` until the pusher reaches the pressed position.")]
        public float pressedDistance = 0.1f;
        [Tooltip("If this is checked then the pusher will stay in the pressed position when it reaches the pressed position.")]
        [SerializeField]
        protected bool stayPressed = false;
        [Tooltip("The threshold in which the pusher's current normalized position along the `Operate Axis` has to be within the minimum and maximum limits of the pusher.")]
        [Range(0f, 1f)]
        public float minMaxLimitThreshold = 0.01f;
        [Tooltip("The normalized position of the pusher between the original position and the pressed position that will be considered as the resting position for the pusher.")]
        [Range(0f, 1f)]
        public float restingPosition = 0f;
        [Tooltip("The normalized value that the pusher can be from the `Resting Position` before the pusher is considered to be resting when not being interacted with.")]
        [Range(0f, 1f)]
        public float restingPositionThreshold = 0.01f;
        [Tooltip("The normalized position of the pusher between the original position and the pressed position. `0f` will set the pusher position to the original position, `1f` will set the pusher position to the pressed position.")]
        [Range(0f, 1f)]
        [SerializeField]
        protected float positionTarget = 0f;
        [Tooltip("The speed in which the pusher moves towards to the `Pressed Distance` position.")]
        public float pressSpeed = 10f;
        [Tooltip("The speed in which the pusher will return to the `Target Position` of the pusher.")]
        public float returnSpeed = 10f;

        protected Coroutine positionLerpRoutine;
        protected Coroutine setTargetPositionRoutine;
        protected float vectorEqualityThreshold = 0.001f;
        protected bool isPressed = false;
        protected bool isMoving = false;
        protected bool isTouched = false;

        /// <summary>
        /// The GetValue method returns the current position value of the pusher.
        /// </summary>
        /// <returns>The actual position of the pusher.</returns>
        public override float GetValue()
        {
            return transform.localPosition[(int)operateAxis];
        }

        /// <summary>
        /// The GetNormalizedValue method returns the current position value of the pusher normalized between `0f` and `1f`.
        /// </summary>
        /// <returns>The normalized position of the pusher.</returns>
        public override float GetNormalizedValue()
        {
            return VRTK_SharedMethods.NormalizeValue(GetValue(), originalLocalPosition[(int)operateAxis], PressedPosition()[(int)operateAxis]);
        }

        /// <summary>
        /// The IsResting method returns whether the pusher is currently at it's resting position.
        /// </summary>
        /// <returns>Returns `true` if the pusher is currently at the resting position.</returns>
        public override bool IsResting()
        {
            float normalizedValue = GetNormalizedValue();
            return (interactingCollider == null && (normalizedValue < (restingPosition + restingPositionThreshold) && normalizedValue > (restingPosition - restingPositionThreshold)));
        }

        /// <summary>
        /// The SetStayPressed method sets the `Stay Pressed` parameter to the given state and if the state is false and the pusher is currently pressed then it is reset to the original position.
        /// </summary>
        /// <param name="state">The state to set the `Stay Pressed` parameter to.</param>
        public virtual void SetStayPressed(bool state)
        {
            stayPressed = state;
            if (!stayPressed && AtPressedPosition())
            {
                SetToRestingPosition();
            }
        }

        /// <summary>
        /// The SetPositionTarget method sets the `Position Target` parameter to the given normalized value.
        /// </summary>
        /// <param name="normalizedTarget">The `Position Target` to set the pusher to between `0f` and `1f`.</param>
        public virtual void SetPositionTarget(float normalizedTarget)
        {
            positionTarget = Mathf.Clamp01(normalizedTarget);
            SetTargetPosition();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Vector3 objectHalf = AxisDirection(true) * (transform.lossyScale[(int)operateAxis] * 0.5f);
            Vector3 initialPoint = actualTransformPosition + (objectHalf * Mathf.Sign(pressedDistance));
            Vector3 destinationPoint = initialPoint + (AxisDirection(true) * pressedDistance);
            Gizmos.DrawLine(initialPoint, destinationPoint);
            Gizmos.DrawSphere(destinationPoint, 0.01f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            isPressed = false;
            isMoving = false;
            isTouched = false;
            setTargetPositionRoutine = StartCoroutine(SetTargetPositionAtEndOfFrame());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelPositionLerp();
            CancelSetTargetPosition();
        }

        protected override void EmitEvents()
        {
            float currentPosition = GetNormalizedValue();
            ControllableEventArgs payload = EventPayload();
            OnValueChanged(payload);

            float minThreshold = minMaxLimitThreshold;
            float maxThreshold = 1f - minMaxLimitThreshold;

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

            if (IsResting())
            {
                OnRestingPointReached(payload);
            }
        }

        protected override void OnTouched(Collider collider)
        {
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject) || VRTK_PlayerObject.IsPlayerObject(collider.gameObject, VRTK_PlayerObject.ObjectTypes.Controller))
            {
                base.OnTouched(collider);

                if (!isMoving)
                {
                    Vector3 targetPosition = (!stayPressed && AtPressedPosition() ? originalLocalPosition : PressedPosition());
                    float targetSpeed = (!stayPressed && AtPressedPosition() ? returnSpeed : pressSpeed);
                    if (!AtTargetPosition(targetPosition))
                    {
                        positionLerpRoutine = StartCoroutine(PositionLerp(targetPosition, targetSpeed));
                    }
                }
                isTouched = true;
            }
        }

        protected override void OnUntouched(Collider collider)
        {
            isTouched = false;
        }

        protected virtual void SetTargetPosition()
        {
            transform.localPosition = Vector3.Lerp(originalLocalPosition, PressedPosition(), (stayPressed && AtPressedPosition() ? 1f : positionTarget));
            EmitEvents();
        }

        protected virtual Vector3 PressedPosition()
        {
            return originalLocalPosition + (AxisDirection() * pressedDistance);
        }

        protected virtual void CancelPositionLerp()
        {
            if (positionLerpRoutine != null)
            {
                StopCoroutine(positionLerpRoutine);
            }
            positionLerpRoutine = null;
        }

        protected virtual void CancelSetTargetPosition()
        {
            if (setTargetPositionRoutine != null)
            {
                StopCoroutine(setTargetPositionRoutine);
            }
            setTargetPositionRoutine = null;
        }

        protected virtual IEnumerator PositionLerp(Vector3 targetPosition, float moveSpeed)
        {
            while (!VRTK_SharedMethods.Vector3ShallowCompare(transform.localPosition, targetPosition, vectorEqualityThreshold))
            {
                yield return null;
                isMoving = true;
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, moveSpeed * Time.deltaTime);
                EmitEvents();
            }
            transform.localPosition = targetPosition;
            isMoving = false;
            EmitEvents();

            ManageAtPressedPosition();
            ManageAtOriginPosition();
        }

        protected virtual IEnumerator SetTargetPositionAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            SetTargetPosition();
        }

        protected virtual void ManageAtPressedPosition()
        {
            if (AtPressedPosition())
            {
                if (stayPressed)
                {
                    ResetInteractor();
                }
                else
                {
                    SetToRestingPosition();
                }
            }
        }

        protected virtual void ManageAtOriginPosition()
        {
            if (AtOriginPosition() && isTouched == false)
            {
                ResetInteractor();
            }
        }

        protected virtual bool AtOriginPosition()
        {
            return VRTK_SharedMethods.Vector3ShallowCompare(transform.localPosition, originalLocalPosition, vectorEqualityThreshold);
        }

        protected virtual bool AtPressedPosition()
        {
            return VRTK_SharedMethods.Vector3ShallowCompare(transform.localPosition, PressedPosition(), vectorEqualityThreshold);
        }

        public virtual bool AtTargetPosition(Vector3 targetPosition)
        {
            return VRTK_SharedMethods.Vector3ShallowCompare(transform.localPosition, targetPosition, vectorEqualityThreshold);
        }

        protected virtual void ResetInteractor()
        {
            interactingCollider = null;
            interactingTouchScript = null;
        }

        protected virtual void SetToRestingPosition()
        {
            positionLerpRoutine = StartCoroutine(PositionLerp(Vector3.Lerp(originalLocalPosition, PressedPosition(), restingPosition), returnSpeed));
        }
    }
}
