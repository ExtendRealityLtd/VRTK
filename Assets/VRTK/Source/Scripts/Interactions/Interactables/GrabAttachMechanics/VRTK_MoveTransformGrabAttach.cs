// Move Transform Grab Attach|GrabAttachMechanics|50110
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingObject">The GameObject that is performing the interaction (e.g. a controller).</param>
    /// <param name="position">The current position in relation to the axis limits from the origin position.</param>
    /// <param name="normalizedPosition">The normalized position (between `0f` and `1f`) of the Interactable Object in relation to the axis limits.</param>
    /// <param name="currentDirection">The direction vector that the Interactable Object is currently moving across the axes in.</param>
    /// <param name="originDirection">The direction vector that the Interactable Object is currently moving across the axes in in relation to the origin position.</param>
    public struct MoveTransformGrabAttachEventArgs
    {
        public GameObject interactingObject;
        public Vector3 position;
        public Vector3 normalizedPosition;
        public Vector3 currentDirection;
        public Vector3 originDirection;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="MoveTransformGrabAttachEventArgs"/></param>
    public delegate void MoveTransformGrabAttachEventHandler(object sender, MoveTransformGrabAttachEventArgs e);

    /// <summary>
    /// Moves the Transform of the Interactable Object towards the interacting object within specified limits.
    /// </summary>
    /// <remarks>
    ///   > To allow unrestricted movement, set the axis limit minimum to `-infinity` and the axis limit maximum to `infinity`.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_MoveTransformGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_MoveTransformGrabAttach")]
    public class VRTK_MoveTransformGrabAttach : VRTK_BaseGrabAttach
    {
        [Tooltip("The maximum distance the grabbing object is away from the Interactable Object before it is automatically released.")]
        public float detachDistance = 1f;

        [Header("Movement Settings")]

        [Tooltip("The speed in which to track the grabbed Interactable Object to the interacting object.")]
        public float trackingSpeed = 10f;
        [Tooltip("If this is checked then it will force the rigidbody on the Interactable Object to be `Kinematic` when the grab occurs.")]
        public bool forceKinematicOnGrab = true;
        [Tooltip("The damper in which to slow the Interactable Object down when released to simulate continued momentum. The higher the number, the faster the Interactable Object will come to a complete stop on release.")]
        public float releaseDecelerationDamper = 5f;
        [Tooltip("The speed in which the Interactable Object returns to it's origin position when released. If the `Reset To Orign On Release Speed` is `0f` then the position will not be reset.")]
        public float resetToOrignOnReleaseSpeed = 0f;

        [Header("Position Limit Settings")]

        [Tooltip("The minimum and maximum limits the Interactable Object can be moved along the x axis.")]
        public Limits2D xAxisLimits = Limits2D.zero;
        [Tooltip("The minimum and maximum limits the Interactable Object can be moved along the y axis.")]
        public Limits2D yAxisLimits = Limits2D.zero;
        [Tooltip("The minimum and maximum limits the Interactable Object can be moved along the z axis.")]
        public Limits2D zAxisLimits = Limits2D.zero;
        [Tooltip("The threshold the position value needs to be within to register a min or max position value.")]
        public float minMaxThreshold = 0.01f;
        [Tooltip("The threshold the normalized position value needs to be within to register a min or max normalized position value.")]
        [Range(0f, 0.99f)]
        public float minMaxNormalizedThreshold = 0.01f;

        /// <summary>
        /// The default local position of the Interactable Object.
        /// </summary>
        [HideInInspector]
        public Vector3 localOrigin;

        /// <summary>
        /// Emitted when the Transform position has changed.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler TransformPositionChanged;
        /// <summary>
        /// Emitted when the Transform position has reached the X Axis Min Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler XAxisMinLimitReached;
        /// <summary>
        /// Emitted when the Transform position has exited the X Axis Min Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler XAxisMinLimitExited;
        /// <summary>
        /// Emitted when the Transform position has reached the X Axis Max Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler XAxisMaxLimitReached;
        /// <summary>
        /// Emitted when the Transform position has exited the X Axis Max Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler XAxisMaxLimitExited;
        /// <summary>
        /// Emitted when the Transform position has reached the Y Axis Min Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler YAxisMinLimitReached;
        /// <summary>
        /// Emitted when the Transform position has exited the Y Axis Min Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler YAxisMinLimitExited;
        /// <summary>
        /// Emitted when the Transform position has reached the Y Axis Max Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler YAxisMaxLimitReached;
        /// <summary>
        /// Emitted when the Transform position has exited the Y Axis Max Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler YAxisMaxLimitExited;
        /// <summary>
        /// Emitted when the Transform position has reached the Z Axis Min Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler ZAxisMinLimitReached;
        /// <summary>
        /// Emitted when the Transform position has exited the Z Axis Min Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler ZAxisMinLimitExited;
        /// <summary>
        /// Emitted when the Transform position has reached the Z Axis Max Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler ZAxisMaxLimitReached;
        /// <summary>
        /// Emitted when the Transform position has exited the Z Axis Max Limit.
        /// </summary>
        public event MoveTransformGrabAttachEventHandler ZAxisMaxLimitExited;

        protected bool previousKinematicState;
        protected bool[] limitsReached = new bool[6];
        protected Limits2D xOriginLimits;
        protected Limits2D yOriginLimits;
        protected Limits2D zOriginLimits;
        protected Vector3 previousPosition;
        protected Vector3 movementVelocity;
        protected Coroutine resetPositionRoutine;
        protected Coroutine deceleratePositionRoutine;

        public virtual void OnTransformPositionChanged(MoveTransformGrabAttachEventArgs e)
        {
            if (TransformPositionChanged != null)
            {
                TransformPositionChanged(this, e);
            }
        }

        public virtual void OnXAxisMinLimitReached(MoveTransformGrabAttachEventArgs e)
        {
            if (XAxisMinLimitReached != null)
            {
                XAxisMinLimitReached(this, e);
            }
        }

        public virtual void OnXAxisMinLimitExited(MoveTransformGrabAttachEventArgs e)
        {
            if (XAxisMinLimitExited != null)
            {
                XAxisMinLimitExited(this, e);
            }
        }

        public virtual void OnXAxisMaxLimitReached(MoveTransformGrabAttachEventArgs e)
        {
            if (XAxisMaxLimitReached != null)
            {
                XAxisMaxLimitReached(this, e);
            }
        }

        public virtual void OnXAxisMaxLimitExited(MoveTransformGrabAttachEventArgs e)
        {
            if (XAxisMaxLimitExited != null)
            {
                XAxisMaxLimitExited(this, e);
            }
        }

        public virtual void OnYAxisMinLimitReached(MoveTransformGrabAttachEventArgs e)
        {
            if (YAxisMinLimitReached != null)
            {
                YAxisMinLimitReached(this, e);
            }
        }

        public virtual void OnYAxisMinLimitExited(MoveTransformGrabAttachEventArgs e)
        {
            if (YAxisMinLimitExited != null)
            {
                YAxisMinLimitExited(this, e);
            }
        }

        public virtual void OnYAxisMaxLimitReached(MoveTransformGrabAttachEventArgs e)
        {
            if (YAxisMaxLimitReached != null)
            {
                YAxisMaxLimitReached(this, e);
            }
        }

        public virtual void OnYAxisMaxLimitExited(MoveTransformGrabAttachEventArgs e)
        {
            if (YAxisMaxLimitExited != null)
            {
                YAxisMaxLimitExited(this, e);
            }
        }

        public virtual void OnZAxisMinLimitReached(MoveTransformGrabAttachEventArgs e)
        {
            if (ZAxisMinLimitReached != null)
            {
                ZAxisMinLimitReached(this, e);
            }
        }

        public virtual void OnZAxisMinLimitExited(MoveTransformGrabAttachEventArgs e)
        {
            if (ZAxisMinLimitExited != null)
            {
                ZAxisMinLimitExited(this, e);
            }
        }

        public virtual void OnZAxisMaxLimitReached(MoveTransformGrabAttachEventArgs e)
        {
            if (ZAxisMaxLimitReached != null)
            {
                ZAxisMaxLimitReached(this, e);
            }
        }

        public virtual void OnZAxisMaxLimitExited(MoveTransformGrabAttachEventArgs e)
        {
            if (ZAxisMaxLimitExited != null)
            {
                ZAxisMaxLimitExited(this, e);
            }
        }

        /// <summary>
        /// The StartGrab method sets up the grab attach mechanic as soon as an Interactable Object is grabbed.
        /// </summary>
        /// <param name="grabbingObject">The GameObject that is doing the grabbing.</param>
        /// <param name="givenGrabbedObject">The GameObject that is being grabbed.</param>
        /// <param name="givenControllerAttachPoint">The point on the grabbing object that the grabbed object should be attached to after grab occurs.</param>
        /// <returns>Returns `true` if the grab is successful, `false` if the grab is unsuccessful.</returns>
        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            CancelResetPosition();
            CancelDeceleratePosition();
            bool grabResult = base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
            if (grabbedObjectRigidBody != null)
            {
                previousKinematicState = grabbedObjectRigidBody.isKinematic;
                grabbedObjectRigidBody.isKinematic = (forceKinematicOnGrab ? true : previousKinematicState);
            }
            limitsReached = new bool[6];
            return grabResult;
        }

        /// <summary>
        /// The StopGrab method ends the grab of the current Interactable Object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If `true` will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            base.StopGrab(applyGrabbingObjectVelocity);
            if (grabbedObjectRigidBody != null)
            {
                grabbedObjectRigidBody.isKinematic = previousKinematicState;
            }
            if (resetToOrignOnReleaseSpeed > 0f)
            {
                ResetPosition();
            }
            else if (releaseDecelerationDamper > 0f)
            {
                CancelDeceleratePosition();
                deceleratePositionRoutine = StartCoroutine(DeceleratePosition());
            }
        }

        /// <summary>
        /// The CreateTrackPoint method sets up the point of grab to track on the grabbed object.
        /// </summary>
        /// <param name="controllerPoint">The point on the controller where the grab was initiated.</param>
        /// <param name="currentGrabbedObject">The GameObject that is currently being grabbed.</param>
        /// <param name="currentGrabbingObject">The GameObject that is currently doing the grabbing.</param>
        /// <param name="customTrackPoint">A reference to whether the created track point is an auto generated custom object.</param>
        /// <returns>The Transform of the created track point.</returns>
        public override Transform CreateTrackPoint(Transform controllerPoint, GameObject currentGrabbedObject, GameObject currentGrabbingObject, ref bool customTrackPoint)
        {
            Transform returnTrackpoint = null;
            returnTrackpoint = base.CreateTrackPoint(controllerPoint, currentGrabbedObject, currentGrabbingObject, ref customTrackPoint);
            if (!precisionGrab)
            {
                returnTrackpoint.position = currentGrabbedObject.transform.position;
            }
            returnTrackpoint.rotation = Quaternion.identity;
            returnTrackpoint.localScale = Vector3.one;
            return returnTrackpoint;
        }

        /// <summary>
        /// The ProcessUpdate method is run in every Update method on the Interactable Object.
        /// </summary>
        public override void ProcessUpdate()
        {
            if (trackPoint != null)
            {
                float distance = Vector3.Distance(trackPoint.position, initialAttachPoint.position);
                if (distance > detachDistance && grabbedObjectScript.IsDroppable())
                {
                    ForceReleaseGrab();
                }
                else
                {
                    Vector3 currentPosition = transform.localPosition;
                    Vector3 moveTowards = currentPosition + Vector3.Scale((transform.InverseTransformPoint(controllerAttachPoint.transform.position) - transform.InverseTransformPoint(grabbedObjectScript.GetPrimaryAttachPoint().position)), transform.localScale);
                    Vector3 targetPosition = Vector3.Lerp(currentPosition, moveTowards, trackingSpeed * Time.deltaTime);
                    previousPosition = transform.localPosition;
                    UpdatePosition(targetPosition, false);
                    movementVelocity = transform.localPosition - previousPosition;
                }
            }
        }

        /// <summary>
        /// The GetPosition method returns a Vector3 of the Transform position in relation to the axis limits.
        /// </summary>
        /// <returns>A Vector3 containing the current Transform position in relation to the axis limits.</returns>
        public virtual Vector3 GetPosition()
        {
            return VRTK_SharedMethods.VectorHeading(localOrigin, transform.localPosition);
        }

        /// <summary>
        /// The GetNormalizedPosition method returns a Vector3 of the Transform position normalized between `0f` and `1f` in relation to the axis limits.;
        /// </summary>
        /// <returns>A normalized Vector3 of the Transform position in relation to the axis limits.</returns>
        public virtual Vector3 GetNormalizedPosition()
        {
            return NormalizePosition(GetPosition());
        }

        /// <summary>
        /// The GetCurrentDirection method returns a Vector3 of the current positive/negative axis direction that the Transform is moving in.
        /// </summary>
        /// <returns>A Vector3 of the direction the Transform is moving across the relevant axis in.</returns>
        public virtual Vector3 GetCurrentDirection()
        {
            return VRTK_SharedMethods.VectorDirection(previousPosition, transform.localPosition);
        }

        /// <summary>
        /// The GetDirectionFromOrigin method returns a Vector3 of the direction across the axis from the original position.
        /// </summary>
        /// <returns>A Vector3 of the direction the Transform is moving across the relevant axis in relation to the original position.</returns>
        public virtual Vector3 GetDirectionFromOrigin()
        {
            return VRTK_SharedMethods.VectorDirection(localOrigin, transform.localPosition);
        }

        /// <summary>
        /// The SetCurrentPosition method sets the position of the Interactable Object to the given new position at the appropriate speed.
        /// </summary>
        /// <param name="newPosition">The position to move the Interactable Object to.</param>
        /// <param name="speed">The speed in which to move the Interactable Object.</param>
        public virtual void SetCurrentPosition(Vector3 newPosition, float speed)
        {
            if(speed > 0f)
            {
                CancelResetPosition();
                resetPositionRoutine = StartCoroutine(MoveToPosition(newPosition, speed));
            }
            else
            {
                UpdatePosition(newPosition, false);
            }
        }

        /// <summary>
        /// The ResetPosition method will move the Interactable Object back to the origin position.
        /// </summary>
        public virtual void ResetPosition()
        {
            SetCurrentPosition(localOrigin, resetToOrignOnReleaseSpeed);
        }

        /// <summary>
        /// The GetWorldLimits method returns an array of minimum and maximum axis limits for the Interactable Object in world space.
        /// </summary>
        /// <returns>An array of axis limits in world space.</returns>
        public virtual Limits2D[] GetWorldLimits()
        {
            return new Limits2D[] { xOriginLimits, yOriginLimits, zOriginLimits };
        }

        protected virtual void OnEnable()
        {
            ResetState();
        }

        protected virtual void OnDisable()
        {
            CancelResetPosition();
            CancelDeceleratePosition();
        }

        protected override void Initialise()
        {
            tracked = false;
            climbable = false;
            kinematic = true;
            SetupOrigin();
        }

        protected virtual void SetupOrigin()
        {
            CheckAxisLimits();
            localOrigin = transform.localPosition;
            xOriginLimits = new Limits2D(localOrigin.x + xAxisLimits.minimum, localOrigin.x + xAxisLimits.maximum);
            yOriginLimits = new Limits2D(localOrigin.y + yAxisLimits.minimum, localOrigin.y + yAxisLimits.maximum);
            zOriginLimits = new Limits2D(localOrigin.z + zAxisLimits.minimum, localOrigin.z + zAxisLimits.maximum);
            previousPosition = localOrigin;
        }

        protected virtual float ClampAxis(Limits2D limits, float axisValue)
        {
            axisValue = (axisValue < limits.minimum + minMaxThreshold ? limits.minimum : axisValue);
            axisValue = (axisValue > limits.maximum - minMaxThreshold ? limits.maximum : axisValue);
            return Mathf.Clamp(axisValue, limits.minimum, limits.maximum);
        }

        protected virtual void ClampPosition()
        {
            transform.localPosition = new Vector3(ClampAxis(xOriginLimits, transform.localPosition.x), ClampAxis(yOriginLimits, transform.localPosition.y), ClampAxis(zOriginLimits, transform.localPosition.z));
        }

        protected virtual Vector3 NormalizePosition(Vector3 givenHeading)
        {
            return new Vector3(
                VRTK_SharedMethods.NormalizeValue(givenHeading.x, xAxisLimits.minimum, xAxisLimits.maximum, minMaxNormalizedThreshold),
                VRTK_SharedMethods.NormalizeValue(givenHeading.y, yAxisLimits.minimum, yAxisLimits.maximum, minMaxNormalizedThreshold),
                VRTK_SharedMethods.NormalizeValue(givenHeading.z, zAxisLimits.minimum, zAxisLimits.maximum, minMaxNormalizedThreshold)
            );
        }

        protected virtual void CancelResetPosition()
        {
            if (resetPositionRoutine != null)
            {
                StopCoroutine(resetPositionRoutine);
            }
        }

        protected virtual void CancelDeceleratePosition()
        {
            if (deceleratePositionRoutine != null)
            {
                StopCoroutine(deceleratePositionRoutine);
            }
        }

        protected virtual void UpdatePosition(Vector3 newPosition, bool additive, bool forceClamp = true)
        {
            transform.localPosition = (additive ? transform.localPosition + newPosition : newPosition);
            if (forceClamp)
            {
                ClampPosition();
            }
            EmitEvents();
        }

        protected virtual IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
        {
            while (transform.localPosition != targetPosition)
            {
                UpdatePosition(Vector3.Lerp(transform.localPosition, targetPosition, speed * Time.deltaTime), false, false);
                yield return null;
            }
            UpdatePosition(targetPosition, false);
        }

        protected virtual IEnumerator DeceleratePosition()
        {
            while (movementVelocity != Vector3.zero)
            {
                movementVelocity = Vector3.Slerp(movementVelocity, Vector3.zero, releaseDecelerationDamper * Time.deltaTime);
                UpdatePosition(movementVelocity, true);
                yield return null;
            }
            movementVelocity = Vector3.zero;
        }

        protected virtual void CheckAxisLimits()
        {
            xAxisLimits = FixAxisLimits(xAxisLimits);
            yAxisLimits = FixAxisLimits(yAxisLimits);
            zAxisLimits = FixAxisLimits(zAxisLimits);
        }

        protected virtual Limits2D FixAxisLimits(Limits2D givenLimits)
        {
            givenLimits.minimum = (givenLimits.minimum > 0f ? givenLimits.minimum * -1f : givenLimits.minimum);
            givenLimits.maximum = (givenLimits.maximum < 0f ? givenLimits.maximum * -1f : givenLimits.maximum);
            return givenLimits;
        }

        protected virtual void EmitEvents()
        {
            MoveTransformGrabAttachEventArgs payload = SetEventPayload();
            if (transform.localPosition != previousPosition)
            {
                OnTransformPositionChanged(payload);
            }

            Vector3 positions = GetNormalizedPosition();

            if (positions.x == 0f && !limitsReached[0])
            {
                OnXAxisMinLimitReached(payload);
                limitsReached[0] = true;
            }
            else if (positions.x == 1f && !limitsReached[1])
            {
                OnXAxisMaxLimitReached(payload);
                limitsReached[1] = true;
            }
            else if (positions.x > 0f && positions.x < 1f)
            {
                if (limitsReached[0])
                {
                    OnXAxisMinLimitExited(payload);
                }
                if (limitsReached[1])
                {
                    OnXAxisMaxLimitExited(payload);
                }
                limitsReached[0] = false;
                limitsReached[1] = false;
            }

            if (positions.y == 0f && !limitsReached[2])
            {
                OnYAxisMinLimitReached(payload);
                limitsReached[2] = true;
            }
            else if (positions.y == 1f && !limitsReached[3])
            {
                OnYAxisMaxLimitReached(payload);
                limitsReached[3] = true;
            }
            else if (positions.y > 0f && positions.y < 1f)
            {
                if (limitsReached[2])
                {
                    OnYAxisMinLimitExited(payload);
                }
                if (limitsReached[3])
                {
                    OnYAxisMaxLimitExited(payload);
                }
                limitsReached[2] = false;
                limitsReached[3] = false;
            }

            if (positions.z == 0f && !limitsReached[4])
            {
                OnZAxisMinLimitReached(payload);
                limitsReached[4] = true;
            }
            else if (positions.z == 1f && !limitsReached[5])
            {
                OnZAxisMaxLimitReached(payload);
                limitsReached[5] = true;
            }
            else if (positions.z > 0f && positions.z < 1f)
            {
                if (limitsReached[4])
                {
                    OnZAxisMinLimitExited(payload);
                }
                if (limitsReached[5])
                {
                    OnZAxisMaxLimitExited(payload);
                }
                limitsReached[4] = false;
                limitsReached[5] = false;
            }
        }

        protected virtual MoveTransformGrabAttachEventArgs SetEventPayload()
        {
            MoveTransformGrabAttachEventArgs e;
            e.interactingObject = (grabbedObjectScript != null ? grabbedObjectScript.GetGrabbingObject() : null);
            e.position = GetPosition();
            e.normalizedPosition = GetNormalizedPosition();
            e.currentDirection = GetCurrentDirection();
            e.originDirection = GetDirectionFromOrigin();
            return e;
        }
    }
}