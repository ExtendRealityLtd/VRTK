// Rotate Transform Grab Attach|GrabAttachMechanics|50120
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;
    using System.Collections;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="interactingObject">The GameObject that is performing the interaction (e.g. a controller).</param>
    /// <param name="currentAngle">The current angle the Interactable Object is rotated to.</param>
    /// <param name="normalizedAngle">The normalized angle (between `0f` and `1f`) the Interactable Object is rotated to.</param>
    /// <param name="rotationSpeed">The speed in which the rotation is occuring.</param>
    public struct RotateTransformGrabAttachEventArgs
    {
        public GameObject interactingObject;
        public float currentAngle;
        public float normalizedAngle;
        public Vector3 rotationSpeed;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="RotateTransformGrabAttachEventArgs"/></param>
    public delegate void RotateTransformGrabAttachEventHandler(object sender, RotateTransformGrabAttachEventArgs e);

    /// <summary>
    /// Rotates the Transform of the Interactable Object around a specified transform local axis within the given limits.
    /// </summary>
    /// <remarks>
    ///   > To allow unrestricted movement, set the angle limits minimum to `-infinity` and the angle limits maximum to `infinity`.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_RotateTransformGrabAttach` script on either:
    ///    * The GameObject of the Interactable Object to detect interactions on.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Grab Attach Mechanic Script` parameter to denote use of the grab mechanic.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Interactables/Grab Attach Mechanics/VRTK_RotateTransformGrabAttach")]
    public class VRTK_RotateTransformGrabAttach : VRTK_BaseGrabAttach
    {
        /// <summary>
        /// The local axis for rotation.
        /// </summary>
        public enum RotationAxis
        {
            /// <summary>
            /// The local X Axis of the transform.
            /// </summary>
            xAxis,
            /// <summary>
            /// The local Y Axis of the transform.
            /// </summary>
            yAxis,
            /// <summary>
            /// The local Z Axis of the transform.
            /// </summary>
            zAxis
        }

        /// <summary>
        /// The way in which rotation from the grabbing object is applied.
        /// </summary>
        public enum RotationType
        {
            /// <summary>
            /// The angle between the Interactable Object origin and the grabbing object attach point.
            /// </summary>
            FollowAttachPoint,
            /// <summary>
            /// The angular velocity across the grabbing object's longitudinal axis (the roll axis).
            /// </summary>
            FollowLongitudinalAxis,
            /// <summary>
            /// The angular velocity across the grabbing object's lateral axis (the pitch axis).
            /// </summary>
            FollowLateralAxis,
            /// <summary>
            /// The angular velocity across the grabbing object's perpendicular axis (the yaw axis).
            /// </summary>
            FollowPerpendicularAxis
        }

        [Header("Detach Settings")]

        [Tooltip("The maximum distance the grabbing object is away from the Interactable Object before it is automatically dropped.")]
        public float detachDistance = 1f;
        [Tooltip("The distance between grabbing object and the centre of Interactable Object that is considered to be non grabbable. If the grabbing object is within the `Origin Deadzone` distance then it will be automatically ungrabbed.")]
        public float originDeadzone = 0f;

        [Header("Rotation Settings")]

        [Tooltip("The local axis in which to rotate the object around.")]
        public RotationAxis rotateAround = RotationAxis.xAxis;
        [Tooltip("Determines how the rotation of the object is calculated based on the action of the grabbing object.")]
        public RotationType rotationAction = RotationType.FollowAttachPoint;
        [Tooltip("The amount of friction to apply when rotating, simulates a tougher rotation.")]
        [Range(1f, 32f)]
        public float rotationFriction = 1f;
        [Tooltip("The damper in which to slow the Interactable Object's rotation down when released to simulate continued momentum. The higher the number, the faster the Interactable Object will come to a complete stop on release.")]
        public float releaseDecelerationDamper = 1f;
        [Tooltip("The speed in which the Interactable Object returns to it's origin rotation when released. If the `Reset To Orign On Release Speed` is `0f` then the rotation will not be reset.")]
        public float resetToOrignOnReleaseSpeed = 0f;

        [Header("Rotation Limits")]

        [Tooltip("The negative and positive limits the axis can be rotated to.")]
        public Limits2D angleLimits = new Limits2D(-180f, 180f);
        [Tooltip("The threshold the rotation value needs to be within to register a min or max rotation value.")]
        public float minMaxThreshold = 1f;
        [Tooltip("The threshold the normalized rotation value needs to be within to register a min or max normalized rotation value.")]
        [Range(0f, 0.99f)]
        public float minMaxNormalizedThreshold = 0.01f;

        /// <summary>
        /// The default local rotation of the Interactable Object.
        /// </summary>
        [HideInInspector]
        public Quaternion originRotation;

        /// <summary>
        /// Emitted when the angle changes.
        /// </summary>
        public event RotateTransformGrabAttachEventHandler AngleChanged;
        /// <summary>
        /// Emitted when the angle reaches the minimum angle.
        /// </summary>
        public event RotateTransformGrabAttachEventHandler MinAngleReached;
        /// <summary>
        /// Emitted when the angle exits the minimum angle state.
        /// </summary>
        public event RotateTransformGrabAttachEventHandler MinAngleExited;
        /// <summary>
        /// Emitted when the angle reaches the maximum angle.
        /// </summary>
        public event RotateTransformGrabAttachEventHandler MaxAngleReached;
        /// <summary>
        /// Emitted when the angle exits the maximum angle state.
        /// </summary>
        public event RotateTransformGrabAttachEventHandler MaxAngleExited;

        protected Vector3 previousAttachPointPosition;
        protected Vector3 currentRotation;
        protected Bounds grabbedObjectBounds;
        protected Vector3 currentRotationSpeed;
        protected Coroutine updateRotationRoutine;
        protected Coroutine decelerateRotationRoutine;
        protected bool[] limitsReached = new bool[2];
        protected VRTK_ControllerReference grabbingObjectReference;

        public virtual void OnAngleChanged(RotateTransformGrabAttachEventArgs e)
        {
            if (AngleChanged != null)
            {
                AngleChanged(this, e);
            }
        }

        public virtual void OnMinAngleReached(RotateTransformGrabAttachEventArgs e)
        {
            if (MinAngleReached != null)
            {
                MinAngleReached(this, e);
            }
        }

        public virtual void OnMinAngleExited(RotateTransformGrabAttachEventArgs e)
        {
            if (MinAngleExited != null)
            {
                MinAngleExited(this, e);
            }
        }

        public virtual void OnMaxAngleReached(RotateTransformGrabAttachEventArgs e)
        {
            if (MaxAngleReached != null)
            {
                MaxAngleReached(this, e);
            }
        }

        public virtual void OnMaxAngleExited(RotateTransformGrabAttachEventArgs e)
        {
            if (MaxAngleExited != null)
            {
                MaxAngleExited(this, e);
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
            CancelUpdateRotation();
            CancelDecelerateRotation();
            bool grabResult = base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
            previousAttachPointPosition = controllerAttachPoint.transform.position;
            grabbedObjectBounds = VRTK_SharedMethods.GetBounds(givenGrabbedObject.transform);
            limitsReached = new bool[2];
            CheckAngleLimits();
            grabbingObjectReference = VRTK_ControllerReference.GetControllerReference(grabbingObject);
            return grabResult;
        }

        /// <summary>
        /// The StopGrab method ends the grab of the current Interactable Object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If `true` will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            base.StopGrab(applyGrabbingObjectVelocity);
            if (resetToOrignOnReleaseSpeed > 0f)
            {
                ResetRotation();
            }
            else if (releaseDecelerationDamper > 0f)
            {
                CancelDecelerateRotation();
                decelerateRotationRoutine = StartCoroutine(DecelerateRotation());
            }
        }

        /// <summary>
        /// The ProcessUpdate method is run in every Update method on the Interactable Object.
        /// </summary>
        public override void ProcessUpdate()
        {
            if (trackPoint != null)
            {
                float distance = Vector3.Distance(transform.position, controllerAttachPoint.transform.position);
                if (StillTouching() && distance >= originDeadzone)
                {
                    Vector3 newRotation = GetNewRotation();
                    previousAttachPointPosition = controllerAttachPoint.transform.position;
                    currentRotationSpeed = newRotation;
                    UpdateRotation(newRotation, true, true);
                }
                else if (grabbedObjectScript.IsDroppable())
                {
                    ForceReleaseGrab();
                }
            }
        }

        /// <summary>
        /// The SetRotation method sets the rotation on the Interactable Object to the given angle over the desired time.
        /// </summary>
        /// <param name="newAngle">The angle to rotate to through the current rotation axis.</param>
        /// <param name="transitionTime">The time in which the entire rotation operation will take place.</param>
        public virtual void SetRotation(float newAngle, float transitionTime = 0f)
        {
            newAngle = Mathf.Clamp(newAngle, angleLimits.minimum, angleLimits.maximum);
            Vector3 newCurrentRotation = currentRotation;
            switch (rotateAround)
            {
                case RotationAxis.xAxis:
                    newCurrentRotation = new Vector3(newAngle, currentRotation.y, currentRotation.z);
                    break;
                case RotationAxis.yAxis:
                    newCurrentRotation = new Vector3(currentRotation.x, newAngle, currentRotation.z);
                    break;
                case RotationAxis.zAxis:
                    newCurrentRotation = new Vector3(currentRotation.x, currentRotation.y, newAngle);
                    break;
            }

            if (transitionTime > 0f)
            {
                CancelUpdateRotation();
                updateRotationRoutine = StartCoroutine(RotateToAngle(newCurrentRotation, VRTK_SharedMethods.DividerToMultiplier(transitionTime)));
            }
            else
            {
                UpdateRotation(newCurrentRotation, false, false);
                currentRotation = newCurrentRotation;
            }
        }

        /// <summary>
        /// The ResetRotation method will rotate the Interactable Object back to the origin rotation.
        /// </summary>
        /// <param name="ignoreTransition">If this is `true` then the `Reset To Origin On Release Speed` will be ignored and the reset will occur instantly.</param>
        public virtual void ResetRotation(bool ignoreTransition = false)
        {
            CancelDecelerateRotation();
            if (resetToOrignOnReleaseSpeed > 0 && !ignoreTransition)
            {
                CancelUpdateRotation();
                updateRotationRoutine = StartCoroutine(RotateToAngle(Vector3.zero, resetToOrignOnReleaseSpeed));
            }
            else
            {
                UpdateRotation(originRotation.eulerAngles, false, false);
                currentRotation = Vector3.zero;
                currentRotationSpeed = Vector3.zero;
            }
        }

        /// <summary>
        /// The GetAngle method returns the current angle the Interactable Object is rotated to.
        /// </summary>
        /// <returns>The current rotated angle.</returns>
        public virtual float GetAngle()
        {
            switch (rotateAround)
            {
                case RotationAxis.xAxis:
                    return currentRotation.x;
                case RotationAxis.yAxis:
                    return currentRotation.y;
                case RotationAxis.zAxis:
                    return currentRotation.z;
            }
            return -0f;
        }

        /// <summary>
        /// The GetNormalizedAngle returns the normalized current angle between the minimum and maximum angle limits.
        /// </summary>
        /// <returns>The normalized rotated angle. Will return `0f` if either limit is set to `infinity`.</returns>
        public virtual float GetNormalizedAngle()
        {
            return (angleLimits.minimum > float.MinValue && angleLimits.maximum < float.MaxValue ? VRTK_SharedMethods.NormalizeValue(GetAngle(), angleLimits.minimum, angleLimits.maximum, minMaxNormalizedThreshold) : 0f);
        }

        /// <summary>
        /// The GetRotationSpeed returns the current speed in which the Interactable Object is rotating.
        /// </summary>
        /// <returns>A Vector3 containing the speed each axis is rotating in.</returns>
        public virtual Vector3 GetRotationSpeed()
        {
            return currentRotationSpeed;
        }

        protected virtual void OnDisable()
        {
            CancelUpdateRotation();
            CancelDecelerateRotation();
        }

        protected override void Initialise()
        {
            tracked = false;
            climbable = false;
            kinematic = true;
            precisionGrab = true;
            originRotation = transform.localRotation;
            currentRotation = Vector3.zero;
        }

        protected virtual Vector3 GetNewRotation()
        {
            Vector3 grabbingObjectAngularVelocity = Vector3.zero;
            if (VRTK_ControllerReference.IsValid(grabbingObjectReference))
            {
                grabbingObjectAngularVelocity = VRTK_DeviceFinder.GetControllerAngularVelocity(grabbingObjectReference) * VRTK_SharedMethods.DividerToMultiplier(rotationFriction);
            }

            switch (rotationAction)
            {
                case RotationType.FollowAttachPoint:
                    return CalculateAngle(transform.position, previousAttachPointPosition, controllerAttachPoint.transform.position);
                case RotationType.FollowLongitudinalAxis:
                    return BuildFollowAxisVector(grabbingObjectAngularVelocity.x);
                case RotationType.FollowPerpendicularAxis:
                    return BuildFollowAxisVector(grabbingObjectAngularVelocity.y);
                case RotationType.FollowLateralAxis:
                    return BuildFollowAxisVector(grabbingObjectAngularVelocity.z);
            }

            return Vector3.zero;
        }

        protected virtual Vector3 BuildFollowAxisVector(float givenAngle)
        {
            float xAngle = (rotateAround == RotationAxis.xAxis ? givenAngle : 0f);
            float yAngle = (rotateAround == RotationAxis.yAxis ? givenAngle : 0f);
            float zAngle = (rotateAround == RotationAxis.zAxis ? givenAngle : 0f);

            return new Vector3(xAngle, yAngle, zAngle);
        }

        protected virtual Vector3 CalculateAngle(Vector3 originPoint, Vector3 originalGrabPoint, Vector3 currentGrabPoint)
        {
            float xRotated = (rotateAround == RotationAxis.xAxis ? CalculateAngle(originPoint, originalGrabPoint, currentGrabPoint, transform.right) : 0f);
            float yRotated = (rotateAround == RotationAxis.yAxis ? CalculateAngle(originPoint, originalGrabPoint, currentGrabPoint, transform.up) : 0f);
            float zRotated = (rotateAround == RotationAxis.zAxis ? CalculateAngle(originPoint, originalGrabPoint, currentGrabPoint, transform.forward) : 0f);

            float frictionMultiplier = VRTK_SharedMethods.DividerToMultiplier(rotationFriction);
            return new Vector3(xRotated * frictionMultiplier, yRotated * frictionMultiplier, zRotated * frictionMultiplier);
        }

        protected virtual float CalculateAngle(Vector3 originPoint, Vector3 previousPoint, Vector3 currentPoint, Vector3 direction)
        {
            Vector3 sideA = previousPoint - originPoint;
            Vector3 sideB = VRTK_SharedMethods.VectorDirection(originPoint, currentPoint);
            return AngleSigned(sideA, sideB, direction);
        }

        protected virtual void UpdateRotation(Vector3 newRotation, bool additive, bool updateCurrentRotation)
        {
            if (WithinRotationLimit(currentRotation + newRotation))
            {
                if (updateCurrentRotation)
                {
                    currentRotation += newRotation;
                }
                transform.localRotation = (additive ? transform.localRotation * Quaternion.Euler(newRotation) : Quaternion.Euler(newRotation));
                EmitEvents();
            }
        }

        protected virtual bool WithinRotationLimit(Vector3 rotationCheck)
        {
            switch (rotateAround)
            {
                case RotationAxis.xAxis:
                    return angleLimits.WithinLimits(rotationCheck.x);
                case RotationAxis.yAxis:
                    return angleLimits.WithinLimits(rotationCheck.y);
                case RotationAxis.zAxis:
                    return angleLimits.WithinLimits(rotationCheck.z);
            }
            return false;
        }

        protected virtual float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        protected virtual bool StillTouching()
        {
            float distance = Vector3.Distance(controllerAttachPoint.transform.position, initialAttachPoint.position);
            return (grabbedObjectBounds.Contains(controllerAttachPoint.transform.position) || distance <= detachDistance);
        }

        protected virtual void CancelUpdateRotation()
        {
            if (updateRotationRoutine != null)
            {
                StopCoroutine(updateRotationRoutine);
            }
        }

        protected virtual void CancelDecelerateRotation()
        {
            if (decelerateRotationRoutine != null)
            {
                StopCoroutine(decelerateRotationRoutine);
            }
        }

        protected virtual IEnumerator RotateToAngle(Vector3 targetAngle, float rotationSpeed)
        {
            Vector3 previousRotation = currentRotation;
            currentRotationSpeed = Vector3.zero;
            while (currentRotation != targetAngle)
            {
                currentRotation = Vector3.Lerp(currentRotation, targetAngle, rotationSpeed * Time.deltaTime);
                UpdateRotation(currentRotation - previousRotation, true, false);
                previousRotation = currentRotation;
                yield return null;
            }
            UpdateRotation(targetAngle, false, false);
            currentRotation = targetAngle;
        }

        protected virtual IEnumerator DecelerateRotation()
        {
            while (currentRotationSpeed != Vector3.zero)
            {
                currentRotationSpeed = Vector3.Slerp(currentRotationSpeed, Vector3.zero, releaseDecelerationDamper * Time.deltaTime);
                UpdateRotation(currentRotationSpeed, true, true);
                yield return null;
            }
        }

        protected virtual float GetLimitedAngle(float angle)
        {
            return (angle > 180f ? angle - 360f : angle);
        }

        protected virtual void CheckAngleLimits()
        {
            angleLimits.minimum = (angleLimits.minimum > 0f ? angleLimits.minimum * -1f : angleLimits.minimum);
            angleLimits.maximum = (angleLimits.maximum < 0f ? angleLimits.maximum * -1f : angleLimits.maximum);
        }

        protected virtual void EmitEvents()
        {
            OnAngleChanged(SetEventPayload());
            float angle = GetAngle();
            float minAngle = angleLimits.minimum + minMaxThreshold;
            float maxAngle = angleLimits.maximum - minMaxThreshold;
            if (angle <= minAngle && !limitsReached[0])
            {
                limitsReached[0] = true;
                OnMinAngleReached(SetEventPayload());
            }
            else if (angle >= maxAngle && !limitsReached[1])
            {
                limitsReached[1] = true;
                OnMaxAngleReached(SetEventPayload());
            }
            else if (angle > minAngle && angle < maxAngle)
            {
                if (limitsReached[0])
                {
                    OnMinAngleExited(SetEventPayload());
                }
                if (limitsReached[1])
                {
                    OnMaxAngleExited(SetEventPayload());
                }
                limitsReached[0] = false;
                limitsReached[1] = false;
            }
        }

        protected virtual RotateTransformGrabAttachEventArgs SetEventPayload()
        {
            RotateTransformGrabAttachEventArgs e;
            e.interactingObject = (grabbedObjectScript != null ? grabbedObjectScript.GetGrabbingObject() : null);
            e.currentAngle = GetAngle();
            e.normalizedAngle = GetNormalizedAngle();
            e.rotationSpeed = currentRotationSpeed;
            return e;
        }
    }
}