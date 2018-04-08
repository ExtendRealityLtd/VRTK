// Drag World|Locomotion|20150
namespace VRTK
{
    using UnityEngine;
    /// <summary>
    /// Provides the ability to move, rotate and scale the PlayArea by dragging the world with the controllers.
    /// </summary>
    /// <remarks>
    /// **Script Usage:**
    ///  * Place the `VRTK_DragWorld` script on any active scene GameObject.
    ///
    ///   > If only one controller is being used to track the rotation mechanism, then the rotation will be based on the perpendicual (yaw) axis angular velocity of the tracking controller.
    ///   > If both controllers are being used to track the rotation mechanism, then the rotation will be based on pushing one controller forward, whilst pulling the other controller backwards.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_DragWorld")]
    public class VRTK_DragWorld : MonoBehaviour
    {
        /// <summary>
        /// The controller on which to determine as the activation requirement for the control mechanism.
        /// </summary>
        public enum ActivationRequirement
        {
            /// <summary>
            /// Only pressing the activation button on the left controller will activate the mechanism, if the right button is held down then the mechanism will not be activated.
            /// </summary>
            LeftControllerOnly,
            /// <summary>
            /// Only pressing the activation button on the right controller will activate the mechanism, if the left button is held down then the mechanism will not be activated.
            /// </summary>
            RightControllerOnly,
            /// <summary>
            /// Pressing the activation button on the left controller is all that is required to activate the mechanism.
            /// </summary>
            LeftController,
            /// <summary>
            /// Pressing the activation button on the right controller is all that is required to activate the mechanism.
            /// </summary>
            RightController,
            /// <summary>
            /// Pressing the activation button on the either controller is all that is required to activate the mechanism.
            /// </summary>
            EitherController,
            /// <summary>
            /// Pressing the activation button on both controllers is required to activate the mechanism.
            /// </summary>
            BothControllers
        }

        /// <summary>
        /// The controllers which to track when performing the mechanism.
        /// </summary>
        public enum TrackingController
        {
            /// <summary>
            /// Only track the left controller.
            /// </summary>
            LeftController,
            /// <summary>
            /// Only track the right controller.
            /// </summary>
            RightController,
            /// <summary>
            /// Track either the left or the right controller.
            /// </summary>
            EitherController,
            /// <summary>
            /// Only track both controllers at the same time.
            /// </summary>
            BothControllers
        }

        [Header("Movement Settings")]

        [Tooltip("The controller button to press to activate the movement mechanism.")]
        public VRTK_ControllerEvents.ButtonAlias movementActivationButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
        [Tooltip("The controller(s) on which the activation button is to be pressed to consider the movement mechanism active.")]
        public ActivationRequirement movementActivationRequirement = ActivationRequirement.EitherController;
        [Tooltip("The controller(s) on which to track position of to determine if a valid move has taken place.")]
        public TrackingController movementTrackingController = TrackingController.BothControllers;
        [Tooltip("The amount to multply the movement by.")]
        public float movementMultiplier = 3f;
        [Tooltip("The axes to lock to prevent movement across.")]
        public Vector3State movementPositionLock = new Vector3State(false, true, false);

        [Header("Rotation Settings")]

        [Tooltip("The controller button to press to activate the rotation mechanism.")]
        public VRTK_ControllerEvents.ButtonAlias rotationActivationButton = VRTK_ControllerEvents.ButtonAlias.GripPress;
        [Tooltip("The controller(s) on which the activation button is to be pressed to consider the rotation mechanism active.")]
        public ActivationRequirement rotationActivationRequirement = ActivationRequirement.BothControllers;
        [Tooltip("The controller(s) on which to determine how rotation should occur. `BothControllers` requires both controllers to be pushed/pulled to rotate, whereas any other setting will base rotation on the rotation of the activating controller.")]
        public TrackingController rotationTrackingController = TrackingController.BothControllers;
        [Tooltip("The amount to multply the rotation angle by.")]
        public float rotationMultiplier = 0.75f;
        [Tooltip("The threshold the rotation angle has to be above to consider a valid rotation amount.")]
        public float rotationActivationThreshold = 0.1f;

        [Header("Scale Settings")]

        [Tooltip("The controller button to press to activate the scale mechanism.")]
        public VRTK_ControllerEvents.ButtonAlias scaleActivationButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;
        [Tooltip("The controller(s) on which the activation button is to be pressed to consider the scale mechanism active.")]
        public ActivationRequirement scaleActivationRequirement = ActivationRequirement.BothControllers;
        [Tooltip("The controller(s) on which to determine how scaling should occur.")]
        public TrackingController scaleTrackingController = TrackingController.BothControllers;
        [Tooltip("The amount to multply the scale factor by.")]
        public float scaleMultiplier = 3f;
        [Tooltip("The threshold the distance between the scale objects has to be above to consider a valid scale operation.")]
        public float scaleActivationThreshold = 0.002f;
        [Tooltip("the minimum scale amount that can be applied.")]
        public Vector3 minimumScale = Vector3.one;
        [Tooltip("the maximum scale amount that can be applied.")]
        public Vector3 maximumScale = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

        [Header("Custom Settings")]

        [Tooltip("The transform to apply the control mechanisms to. If this is left blank then the PlayArea will be controlled.")]
        public Transform controllingTransform;
        [Tooltip("Uses the specified `Offset Transform` when dealing with rotational offsets.")]
        public bool useOffsetTransform = true;
        [Tooltip("The transform to use when dealing with rotational offsets. If this is left blank then the Headset will be used as the offset.")]
        public Transform offsetTransform;

        protected VRTK_ControllerReference leftControllerReference;
        protected VRTK_ControllerReference rightControllerReference;
        protected VRTK_ControllerEvents leftControllerEvents;
        protected VRTK_ControllerEvents rightControllerEvents;
        protected Transform playArea;
        protected Transform headset;

        protected VRTK_ControllerEvents.ButtonAlias subscribedMovementActivationButton;
        protected Vector3 previousLeftControllerPosition = Vector3.zero;
        protected Vector3 previousRightControllerPosition = Vector3.zero;
        protected bool movementLeftControllerActivated;
        protected bool movementRightControllerActivated;
        protected bool movementActivated;

        protected VRTK_ControllerEvents.ButtonAlias subscribedRotationActivationButton;
        protected Vector2 previousRotationAngle = Vector2.zero;
        protected bool rotationLeftControllerActivated;
        protected bool rotationRightControllerActivated;
        protected bool rotationActivated;

        protected VRTK_ControllerEvents.ButtonAlias subscribedScaleActivationButton;
        protected float previousControllerDistance;
        protected bool scaleLeftControllerActivated;
        protected bool scaleRightControllerActivated;
        protected bool scaleActivated;

        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            headset = VRTK_DeviceFinder.HeadsetTransform();
            controllingTransform = (controllingTransform != null ? controllingTransform : playArea);
            offsetTransform = (offsetTransform != null ? offsetTransform : headset);
            leftControllerEvents = GetControllerEvents(VRTK_DeviceFinder.GetControllerLeftHand());
            rightControllerEvents = GetControllerEvents(VRTK_DeviceFinder.GetControllerRightHand());
            movementActivated = false;
            rotationActivated = false;
            scaleActivated = false;
            ManageActivationListeners(true);
            SetControllerReferences();
        }

        protected virtual void OnDisable()
        {
            ManageActivationListeners(false);
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }


        protected virtual void FixedUpdate()
        {
            Scale();
            Rotate();
            Move();
            ManageActivationListeners(true);
        }

        protected virtual VRTK_ControllerEvents GetControllerEvents(GameObject controllerObject)
        {
            return (controllerObject != null ? controllerObject.GetComponentInChildren<VRTK_ControllerEvents>() : null);
        }


        protected virtual void ManageActivationListeners(bool state)
        {
            ManageActivationListener(state, ref movementActivationButton, ref subscribedMovementActivationButton, MovementActivationButtonPressed, MovementActivationButtonReleased);
            ManageActivationListener(state, ref rotationActivationButton, ref subscribedRotationActivationButton, RotationActivationButtonPressed, RotationActivationButtonReleased);
            ManageActivationListener(state, ref scaleActivationButton, ref subscribedScaleActivationButton, ScaleActivationButtonPressed, ScaleActivationButtonReleased);
        }

        protected virtual void ManageActivationListener(bool state, ref VRTK_ControllerEvents.ButtonAlias activationButton, ref VRTK_ControllerEvents.ButtonAlias subscribedActivationButton, ControllerInteractionEventHandler buttonPressedCallback, ControllerInteractionEventHandler buttonReleasedCallback)
        {
            if (subscribedActivationButton == VRTK_ControllerEvents.ButtonAlias.Undefined && (!state || activationButton != subscribedActivationButton))
            {
                if (leftControllerEvents != null)
                {
                    leftControllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, true, buttonPressedCallback);
                    leftControllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, false, buttonReleasedCallback);
                    leftControllerEvents.ControllerModelAvailable -= ControllerModelAvailable;
                }
                if (rightControllerEvents != null)
                {
                    rightControllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, true, buttonPressedCallback);
                    rightControllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, false, buttonReleasedCallback);
                    rightControllerEvents.ControllerModelAvailable -= ControllerModelAvailable;
                }
                subscribedActivationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            }

            if (state && subscribedActivationButton == VRTK_ControllerEvents.ButtonAlias.Undefined && activationButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                bool subscribed = false;
                if (leftControllerEvents != null)
                {
                    leftControllerEvents.SubscribeToButtonAliasEvent(activationButton, true, buttonPressedCallback);
                    leftControllerEvents.SubscribeToButtonAliasEvent(activationButton, false, buttonReleasedCallback);
                    leftControllerEvents.ControllerModelAvailable += ControllerModelAvailable;
                    subscribed = true;
                }

                if (rightControllerEvents != null)
                {
                    rightControllerEvents.SubscribeToButtonAliasEvent(activationButton, true, buttonPressedCallback);
                    rightControllerEvents.SubscribeToButtonAliasEvent(activationButton, false, buttonReleasedCallback);
                    rightControllerEvents.ControllerModelAvailable += ControllerModelAvailable;
                    subscribed = true;
                }

                if (subscribed)
                {
                    subscribedActivationButton = activationButton;
                }
            }
        }

        protected virtual void ControllerModelAvailable(object sender, ControllerInteractionEventArgs e)
        {
            SetControllerReferences();
        }

        protected virtual void SetControllerReferences()
        {
            leftControllerReference = VRTK_DeviceFinder.GetControllerReferenceLeftHand();
            rightControllerReference = VRTK_DeviceFinder.GetControllerReferenceRightHand();
        }

        protected virtual void ManageActivationState(SDK_BaseController.ControllerHand hand, ActivationRequirement activationRequirement, bool pressedState, ref bool leftActivationState, ref bool rightActivationState, ref bool activated)
        {
            switch (hand)
            {
                case SDK_BaseController.ControllerHand.Left:
                    leftActivationState = pressedState; ;
                    break;
                case SDK_BaseController.ControllerHand.Right:
                    rightActivationState = pressedState; ;
                    break;
            }

            switch (activationRequirement)
            {
                case ActivationRequirement.LeftControllerOnly:
                    activated = (rightActivationState ? false : leftActivationState);
                    break;
                case ActivationRequirement.RightControllerOnly:
                    activated = (leftActivationState ? false : rightActivationState);
                    break;
                case ActivationRequirement.LeftController:
                    activated = leftActivationState;
                    break;
                case ActivationRequirement.RightController:
                    activated = rightActivationState;
                    break;
                case ActivationRequirement.EitherController:
                    activated = (leftActivationState || rightActivationState);
                    break;
                case ActivationRequirement.BothControllers:
                    activated = (leftActivationState && rightActivationState);
                    break;
            }
        }

        protected virtual void MovementActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            ManageActivationState(e.controllerReference.hand, movementActivationRequirement, true, ref movementLeftControllerActivated, ref movementRightControllerActivated, ref movementActivated);
            SetControllerPositions();
        }

        protected virtual void MovementActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            ManageActivationState(e.controllerReference.hand, movementActivationRequirement, false, ref movementLeftControllerActivated, ref movementRightControllerActivated, ref movementActivated);
        }

        protected virtual void RotationActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            ManageActivationState(e.controllerReference.hand, rotationActivationRequirement, true, ref rotationLeftControllerActivated, ref rotationRightControllerActivated, ref rotationActivated);
            previousRotationAngle = GetControllerRotation();
        }

        protected virtual void RotationActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            ManageActivationState(e.controllerReference.hand, rotationActivationRequirement, false, ref rotationLeftControllerActivated, ref rotationRightControllerActivated, ref rotationActivated);
        }


        protected virtual void ScaleActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            ManageActivationState(e.controllerReference.hand, scaleActivationRequirement, true, ref scaleLeftControllerActivated, ref scaleRightControllerActivated, ref scaleActivated);
            previousControllerDistance = GetControllerDistance();
        }

        protected virtual void ScaleActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            ManageActivationState(e.controllerReference.hand, scaleActivationRequirement, false, ref scaleLeftControllerActivated, ref scaleRightControllerActivated, ref scaleActivated);
        }

        protected virtual Vector3 GetLeftControllerPosition()
        {
            return (VRTK_ControllerReference.IsValid(leftControllerReference) ? leftControllerReference.actual.transform.localPosition : Vector3.zero);
        }

        protected virtual Vector3 GetRightControllerPosition()
        {
            return (VRTK_ControllerReference.IsValid(rightControllerReference) ? rightControllerReference.actual.transform.localPosition : Vector3.zero);
        }

        protected virtual void SetControllerPositions()
        {
            previousLeftControllerPosition = GetLeftControllerPosition();
            previousRightControllerPosition = GetRightControllerPosition();
        }

        protected virtual Vector2 GetControllerRotation()
        {
            return new Vector2((GetLeftControllerPosition() - GetRightControllerPosition()).x, (GetLeftControllerPosition() - GetRightControllerPosition()).z);
        }

        protected virtual float GetControllerDistance()
        {
            switch (scaleTrackingController)
            {
                case TrackingController.BothControllers:
                    return Vector3.Distance(GetLeftControllerPosition(), GetRightControllerPosition());
                case TrackingController.LeftController:
                    return Vector3.Distance(GetLeftControllerPosition(), offsetTransform.localPosition);
                case TrackingController.RightController:
                    return Vector3.Distance(GetRightControllerPosition(), offsetTransform.localPosition);
                case TrackingController.EitherController:
                    return Vector3.Distance(GetLeftControllerPosition(), offsetTransform.localPosition) + Vector3.Distance(GetRightControllerPosition(), offsetTransform.localPosition);
            }

            return 0f;
        }

        protected virtual bool TrackingControllerEnabled(TrackingController trackingController, TrackingController hand, bool handActivated)
        {
            return (trackingController == TrackingController.BothControllers || trackingController == hand || (trackingController == TrackingController.EitherController && handActivated));
        }

        protected virtual void Move()
        {
            if (!movementActivated)
            {
                return;
            }

            Vector3 leftMovementOffset = (TrackingControllerEnabled(movementTrackingController, TrackingController.LeftController, movementLeftControllerActivated) ? GetLeftControllerPosition() - previousLeftControllerPosition : Vector3.zero);
            Vector3 rightMovementOffset = (TrackingControllerEnabled(movementTrackingController, TrackingController.RightController, movementRightControllerActivated) ? GetRightControllerPosition() - previousRightControllerPosition : Vector3.zero);
            Vector3 movementOffset = controllingTransform.localRotation * (leftMovementOffset + rightMovementOffset);
            Vector3 newPosition = controllingTransform.localPosition - Vector3.Scale((movementOffset * movementMultiplier), controllingTransform.localScale);
            controllingTransform.localPosition = new Vector3((movementPositionLock.xState ? controllingTransform.localPosition.x : newPosition.x), (movementPositionLock.yState ? controllingTransform.localPosition.y : newPosition.y), (movementPositionLock.zState ? controllingTransform.localPosition.z : newPosition.z));
            SetControllerPositions();
        }

        protected virtual void Rotate()
        {
            if (!rotationActivated)
            {
                return;
            }

            if (rotationTrackingController == TrackingController.BothControllers && VRTK_ControllerReference.IsValid(leftControllerReference) && VRTK_ControllerReference.IsValid(rightControllerReference))
            {
                Vector2 currentRotationAngle = GetControllerRotation();
                float newAngle = Vector2.Angle(currentRotationAngle, previousRotationAngle)*Mathf.Sign(Vector3.Cross(currentRotationAngle, previousRotationAngle).z);
                RotateByAngle(newAngle);
                previousRotationAngle = currentRotationAngle;
            }
            else
            {
                float leftControllerAngle = (TrackingControllerEnabled(rotationTrackingController, TrackingController.LeftController, rotationLeftControllerActivated) ? VRTK_DeviceFinder.GetControllerAngularVelocity(leftControllerReference).y : 0f);
                float rightControllerAngle = (TrackingControllerEnabled(rotationTrackingController, TrackingController.RightController, rotationRightControllerActivated) ? VRTK_DeviceFinder.GetControllerAngularVelocity(rightControllerReference).y : 0f);
                RotateByAngle(leftControllerAngle + rightControllerAngle);
            }
        }

        protected virtual void RotateByAngle(float angle)
        {
            if (Mathf.Abs(angle) >= rotationActivationThreshold)
            {
                if (useOffsetTransform)
                {
                    controllingTransform.RotateAround(offsetTransform.position, Vector3.up, angle * rotationMultiplier);
                }
                else
                {
                    controllingTransform.Rotate(Vector3.up * (angle * rotationMultiplier));
                }
            }
        }

        protected virtual void Scale()
        {
            if (!scaleActivated)
            {
                return;
            }

            float currentDistance = GetControllerDistance();
            float distanceDelta = currentDistance - previousControllerDistance;
            if (Mathf.Abs(distanceDelta) >= scaleActivationThreshold)
            {
                controllingTransform.localScale += (Vector3.one * Time.deltaTime * Mathf.Sign(distanceDelta) * scaleMultiplier);
                controllingTransform.localScale = new Vector3(Mathf.Clamp(controllingTransform.localScale.x, minimumScale.x, maximumScale.x), Mathf.Clamp(controllingTransform.localScale.y, minimumScale.y, maximumScale.y), Mathf.Clamp(controllingTransform.localScale.z, minimumScale.z, maximumScale.z));
            }
            previousControllerDistance = currentDistance;
        }
    }
}
