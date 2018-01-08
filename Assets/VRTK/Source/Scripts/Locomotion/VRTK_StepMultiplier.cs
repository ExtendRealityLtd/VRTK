// Step Multiplier|Locomotion|20130
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Multiplies each real world step within the play area to enable further distances to be travelled in the virtual world.
    /// </summary>
    /// <remarks>
    /// **Optional Components:**
    ///  * `VRTK_ControllerEvents` - The events component to listen for the button presses on. This must be applied on the same GameObject as this script if one is not provided via the `Controller Events` parameter.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_StepMultiplier` script on either:
    ///    * Any GameObject in the scene if no activation button is required.
    ///    * The GameObject with the Controller Events scripts if an activation button is required.
    ///    * Any other scene GameObject and provide a valid `VRTK_ControllerEvents` component to the `Controller Events` parameter of this script if an activation button is required.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/028_CameraRig_RoomExtender` shows how the Step Multiplier can be used to move around the scene with multiplied steps.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_StepMultiplier")]
    public class VRTK_StepMultiplier : MonoBehaviour
    {
        /// <summary>
        /// Movement methods.
        /// </summary>
        public enum MovementFunction
        {
            /// <summary>
            /// Moves the head with a non-linear drift movement.
            /// </summary>
            Nonlinear,
            /// <summary>
            /// Moves the headset in a direct linear movement.
            /// </summary>
            LinearDirect
        }

        [Header("Step Multiplier Settings")]

        [Tooltip("The controller button to activate the step multiplier effect. If it is `Undefined` then the step multiplier will always be active.")]
        public VRTK_ControllerEvents.ButtonAlias activationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("This determines the type of movement used by the extender.")]
        public MovementFunction movementFunction = MovementFunction.LinearDirect;
        [Tooltip("This is the factor by which movement at the edge of the circle is amplified. `0` is no movement of the play area. Higher values simulate a bigger play area but may be too uncomfortable.")]
        [Range(0, 10)]
        public float additionalMovementMultiplier = 1.0f;
        [Tooltip("This is the size of the circle in which the play area is not moved and everything is normal. If it is to low it becomes uncomfortable when crouching.")]
        [Range(0, 5)]
        public float headZoneRadius = 0.25f;

        [Header("Custom Settings")]

        [Tooltip("The Controller Events to listen for the events on. If the script is being applied onto a controller then this parameter can be left blank as it will be auto populated by the controller the script is on at runtime.")]
        public VRTK_ControllerEvents controllerEvents;

        protected Vector3 relativeMovementOfCameraRig = new Vector3();
        protected Transform movementTransform;
        protected Transform playArea;
        protected Vector3 headCirclePosition;
        protected Vector3 lastPosition;
        protected Vector3 lastMovement;
        protected bool activationEnabled;
        protected VRTK_ControllerEvents.ButtonAlias subscribedActivationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        protected bool buttonSubscribed;

        protected virtual void Awake()
        {
            VRTK_SDKManager.AttemptAddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            movementTransform = VRTK_DeviceFinder.HeadsetTransform();
            if (movementTransform == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_RoomExtender", "Headset Transform"));
            }
            activationEnabled = false;
            buttonSubscribed = false;
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            MoveHeadCircleNonLinearDrift();
            lastPosition = movementTransform.localPosition;
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.AttemptRemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            ManageButtonSubscription();
            switch (movementFunction)
            {
                case MovementFunction.Nonlinear:
                    MoveHeadCircleNonLinearDrift();
                    break;
                case MovementFunction.LinearDirect:
                    MoveHeadCircle();
                    break;
                default:
                    break;
            }
        }

        protected virtual void ManageButtonSubscription()
        {
            controllerEvents = (controllerEvents != null ? controllerEvents : GetComponentInParent<VRTK_ControllerEvents>());

            if (controllerEvents != null && buttonSubscribed && subscribedActivationButton != VRTK_ControllerEvents.ButtonAlias.Undefined && activationButton != subscribedActivationButton)
            {
                buttonSubscribed = false;
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, true, ActivationButtonPressed);
                controllerEvents.UnsubscribeToButtonAliasEvent(subscribedActivationButton, false, ActivationButtonReleased);
                subscribedActivationButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            }

            if (controllerEvents != null && !buttonSubscribed && activationButton != VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                controllerEvents.SubscribeToButtonAliasEvent(activationButton, true, ActivationButtonPressed);
                controllerEvents.SubscribeToButtonAliasEvent(activationButton, false, ActivationButtonReleased);
                buttonSubscribed = true;
                subscribedActivationButton = activationButton;
            }
        }

        protected virtual void ActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            activationEnabled = true;
        }

        protected virtual void ActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            activationEnabled = false;
        }

        protected virtual void Move(Vector3 movement)
        {
            headCirclePosition += movement;
            if (activationEnabled || activationButton == VRTK_ControllerEvents.ButtonAlias.Undefined)
            {
                playArea.localPosition += movement * additionalMovementMultiplier;
                relativeMovementOfCameraRig += movement * additionalMovementMultiplier;
            }
        }

        protected virtual void MoveHeadCircle()
        {
            //Get the movement of the head relative to the headCircle.
            Vector3 circleCenterToHead = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);

            //Get the direction of the head movement.
            UpdateLastMovement();

            //Checks if the head is outside of the head cirlce and moves the head circle and play area in the movementDirection.
            if (circleCenterToHead.sqrMagnitude > headZoneRadius * headZoneRadius && lastMovement != Vector3.zero)
            {
                //Just move like the headset moved
                Move(lastMovement);
            }
        }

        protected virtual void MoveHeadCircleNonLinearDrift()
        {
            Vector3 movement = new Vector3(movementTransform.localPosition.x - headCirclePosition.x, 0, movementTransform.localPosition.z - headCirclePosition.z);
            if (movement.sqrMagnitude > headZoneRadius * headZoneRadius)
            {
                Vector3 deltaMovement = movement.normalized * (movement.magnitude - headZoneRadius);
                Move(deltaMovement);
            }
        }

        protected virtual void UpdateLastMovement()
        {
            //Save the last movement
            lastMovement = movementTransform.localPosition - lastPosition;
            lastMovement.y = 0;
            lastPosition = movementTransform.localPosition;
        }
    }
}