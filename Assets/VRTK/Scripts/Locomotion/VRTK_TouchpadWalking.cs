// Touchpad Walking|Locomotion|20090
namespace VRTK
{
    using UnityEngine;
    using System;

    /// <summary>
    /// The ability to move the play area around the game world by sliding a finger over the touchpad is achieved using this script.
    /// </summary>
    /// <remarks>
    /// The Touchpad Walking script adds a rigidbody and a box collider to the user's position to prevent them from walking through other collidable game objects.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    /// </example>
    [Obsolete("`VRTK_TouchpadWalking` has been replaced with `VRTK_TouchpadControl`. This script will be removed in a future version of VRTK.")]
    public class VRTK_TouchpadWalking : MonoBehaviour
    {
        [Tooltip("If this is checked then the left controller touchpad will be enabled to move the play area.")]
        public bool leftController = true;
        [Tooltip("If this is checked then the right controller touchpad will be enabled to move the play area.")]
        public bool rightController = true;

        [Tooltip("The maximum speed the play area will be moved when the touchpad is being touched at the extremes of the axis. If a lower part of the touchpad axis is touched (nearer the centre) then the walk speed is slower.")]
        public float maxWalkSpeed = 3f;
        [Tooltip("The speed in which the play area slows down to a complete stop when the user is no longer touching the touchpad. This deceleration effect can ease any motion sickness that may be suffered.")]
        public float deceleration = 0.1f;
        [Tooltip("If a button is defined then movement will only occur when the specified button is being held down and the touchpad axis changes.")]
        public VRTK_ControllerEvents.ButtonAlias moveOnButtonPress = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The direction that will be moved in is the direction of this device.")]
        public VRTK_DeviceFinder.Devices deviceForDirection = VRTK_DeviceFinder.Devices.Headset;
        [Tooltip("If the defined speed multiplier button is pressed then the current movement speed will be multiplied by the `Speed Multiplier` value.")]
        public VRTK_ControllerEvents.ButtonAlias speedMultiplierButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The amount to mmultiply the movement speed by if the `Speed Multiplier Button` is pressed.")]
        public float speedMultiplier = 1f;

        private GameObject controllerLeftHand;
        private GameObject controllerRightHand;
        private Transform playArea;
        private Vector2 touchAxis;
        private float movementSpeed;
        private float strafeSpeed;
        private bool leftSubscribed;
        private bool rightSubscribed;
        private ControllerInteractionEventHandler touchpadAxisChanged;
        private ControllerInteractionEventHandler touchpadUntouched;
        private bool multiplySpeed;
        private VRTK_ControllerEvents controllerEvents;
        private VRTK_BodyPhysics bodyPhysics;
        private bool wasFalling;
        private bool previousLeftControllerState;
        private bool previousRightControllerState;

        protected virtual void Awake()
        {
            touchpadAxisChanged = new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            touchpadUntouched = new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
            if (!playArea)
            {
                Debug.LogError("No play area could be found. Have you selected a valid Boundaries SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Boundaries SDK from the dropdown.");
            }

            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
        }

        protected virtual void OnEnable()
        {
            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed);
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
            movementSpeed = 0f;
            strafeSpeed = 0f;
            multiplySpeed = false;
        }

        protected virtual void OnDisable()
        {
            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed, true);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed, true);
            bodyPhysics = null;
        }

        protected virtual void Update()
        {
            multiplySpeed = (controllerEvents && speedMultiplierButton != VRTK_ControllerEvents.ButtonAlias.Undefined && controllerEvents.IsButtonPressed(speedMultiplierButton));
            CheckControllerState(controllerLeftHand, leftController, ref leftSubscribed, ref previousLeftControllerState);
            CheckControllerState(controllerRightHand, rightController, ref rightSubscribed, ref previousRightControllerState);
        }

        protected virtual void FixedUpdate()
        {
            HandleFalling();
            CalculateSpeed(ref movementSpeed, touchAxis.y);
            CalculateSpeed(ref strafeSpeed, touchAxis.x);
            Move();
        }

        protected virtual void HandleFalling()
        {
            if (bodyPhysics && bodyPhysics.IsFalling())
            {
                touchAxis = Vector2.zero;
                wasFalling = true;
            }

            if (bodyPhysics && !bodyPhysics.IsFalling() && wasFalling)
            {
                touchAxis = Vector2.zero;
                wasFalling = false;
                strafeSpeed = 0f;
                movementSpeed = 0f;
            }
        }

        protected virtual void CheckControllerState(GameObject controller, bool controllerState, ref bool subscribedState, ref bool previousState)
        {
            if (controllerState != previousState)
            {
                SetControllerListeners(controller, controllerState, ref subscribedState);
            }
            previousState = controllerState;
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            controllerEvents = (VRTK_ControllerEvents)sender;
            if (moveOnButtonPress != VRTK_ControllerEvents.ButtonAlias.Undefined && !controllerEvents.IsButtonPressed(moveOnButtonPress))
            {
                touchAxis = Vector2.zero;
                controllerEvents = null;
                return;
            }

            touchAxis = e.touchpadAxis;
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            touchAxis = Vector2.zero;
            controllerEvents = null;
        }

        private void CalculateSpeed(ref float speed, float inputValue)
        {
            if (inputValue != 0f)
            {
                speed = (maxWalkSpeed * inputValue);
                speed = (multiplySpeed ? speed * speedMultiplier : speed);
            }
            else
            {
                Decelerate(ref speed);
            }
        }

        private void Decelerate(ref float speed)
        {
            if (speed > 0)
            {
                speed -= Mathf.Lerp(deceleration, maxWalkSpeed, 0f);
            }
            else if (speed < 0)
            {
                speed += Mathf.Lerp(deceleration, -maxWalkSpeed, 0f);
            }
            else
            {
                speed = 0;
            }

            float deadzone = 0.1f;
            if (speed < deadzone && speed > -deadzone)
            {
                speed = 0;
            }
        }

        private void Move()
        {
            var deviceDirector = VRTK_DeviceFinder.DeviceTransform(deviceForDirection);
            if (deviceDirector)
            {
                var movement = deviceDirector.forward * movementSpeed * Time.deltaTime;
                var strafe = deviceDirector.right * strafeSpeed * Time.deltaTime;
                float fixY = playArea.position.y;
                playArea.position += (movement + strafe);
                playArea.position = new Vector3(playArea.position.x, fixY, playArea.position.z);
            }
        }

        private void SetControllerListeners(GameObject controller, bool controllerState, ref bool subscribedState, bool forceDisabled = false)
        {
            if (controller)
            {
                bool toggleState = (forceDisabled ? false : controllerState);
                ToggleControllerListeners(controller, toggleState, ref subscribedState);
            }
        }

        private void ToggleControllerListeners(GameObject controller, bool toggle, ref bool subscribed)
        {
            var controllerEvent = controller.GetComponent<VRTK_ControllerEvents>();
            if (controllerEvent && toggle && !subscribed)
            {
                controllerEvent.TouchpadAxisChanged += touchpadAxisChanged;
                controllerEvent.TouchpadTouchEnd += touchpadUntouched;
                subscribed = true;
            }
            else if (controllerEvent && !toggle && subscribed)
            {
                controllerEvent.TouchpadAxisChanged -= touchpadAxisChanged;
                controllerEvent.TouchpadTouchEnd -= touchpadUntouched;
                touchAxis = Vector2.zero;
                subscribed = false;
            }
        }
    }
}