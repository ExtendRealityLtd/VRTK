// Touchpad Movement|Locomotion|20100
namespace VRTK
{
    using UnityEngine;
    using System;

#pragma warning disable 0618
    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="movementType">The type of movement for the axis.</param>
    /// <param name="direction">The direction of the axis.</param>
    public struct TouchpadMovementAxisEventArgs
    {
        public VRTK_TouchpadMovement.AxisMovementType movementType;
        public VRTK_TouchpadMovement.AxisMovementDirection direction;
    }
#pragma warning restore 0618

#pragma warning disable 0618
    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="TouchpadMovementAxisEventArgs"/></param>
    public delegate void TouchpadMovementAxisEventHandler(VRTK_TouchpadMovement sender, TouchpadMovementAxisEventArgs e);
#pragma warning restore 0618

    /// <summary>
    /// Adds the ability to move and rotate the play area and the player by using the touchpad. 
    /// </summary>
    /// <remarks>
    /// The Touchpad Movement script requires VRTK_BodyPhysics script to be present in one of the scene GameObjects for collision detection. 
    /// 
    /// Vertical axis movement types include:
    /// - regular smooth sliding (walking)
    /// - warping, which instantly moves the player forward at a fixed distance
    /// 
    /// Horizontal axis movement types include:
    /// - smooth sliding (strafing)
    /// - smooth rotation
    /// - snap rotation, which instantly rotates the player at a fixed angle
    /// - warping, which instantly moves the player sideways (instant strafing)
    /// 
    /// Additionally it's possible to enable direction flip feature which allows the user to do an instant 180 degree turn by pressing the touchpad down.
    /// 
    /// It's also possible to define a button to multiply any type of movement (speed, range, angle) when the set button is pressed. Values above one will give a boost effect
    /// and values below one will do the opposite. All movement values are public properties and can be set from other script at runtime.
    /// 
    /// Different movement types can be split across the controllers by having one script per hand side and with the desired options. 
    /// Warp and snap rotate options may provide more comfortable experience for some and blink effect can be used to soften the movement. 
    /// Snap rotate and flip direction options can be useful with teleport scripts for seated experiences and for people using front facing camera setups(Oculus default, PSVR). 
    /// </remarks>
    [RequireComponent(typeof(VRTK_BodyPhysics))]
    [Obsolete("`VRTK_TouchpadMovement` has been replaced with `VRTK_TouchpadControl`. This script will be removed in a future version of VRTK.")]
    public class VRTK_TouchpadMovement : MonoBehaviour
    {
        /// <summary>
        /// Movement types that can be performed by the vertical axis.
        /// </summary>
        /// <param name="None">No movement is performed.</param>
        /// <param name="Slide">Performs smooth movement (walk).</param>
        /// <param name="Warp">Performs an instant warp movement.</param>
        /// <param name="WarpWithBlink">Performs an instant warp movement with a blink effect.</param>
        public enum VerticalAxisMovement
        {
            None,
            Slide,
            Warp,
            WarpWithBlink
        }

        /// <summary>
        /// Movement types that can be performed by the horizontal axis.
        /// </summary>
        /// <param name="None">No movement is performed.</param>
        /// <param name="Slide">Performs smooth movement (strafe).</param>
        /// <param name="Rotate">Performs smooth rotation.</param>
        /// <param name="SnapRotate">Performs fixed angle rotation.</param>
        /// <param name="SnapRotateWithBlink">Performs fixed angle rotation with a blink effect.</param>
        /// <param name="Warp">Performs an instant warp movement.</param>
        /// <param name="WarpWithBlink">Performs an instant warp movement with a blink effect.</param>
        public enum HorizontalAxisMovement
        {
            None,
            Slide,
            Rotate,
            SnapRotate,
            SnapRotateWithBlink,
            Warp,
            WarpWithBlink
        }

        /// <summary>
        /// Which type axis movement did occur.
        /// </summary>
        /// <param name="Warp">User warped.</param>
        /// <param name="FlipDirection">User flipped the direction.</param>
        /// <param name="SnapRotate">User snap rotated.</param>
        public enum AxisMovementType
        {
            Warp,
            FlipDirection,
            SnapRotate
        }

        /// <summary>
        /// Which direction did the axis movement occur.
        /// </summary>
        public enum AxisMovementDirection
        {
            None,
            Left,
            Right,
            Forward,
            Backward
        }

        /// <summary>
        /// Emitted when a warp, a flip direction or a snap rotate movement has successfully completed.
        /// </summary>
        public event TouchpadMovementAxisEventHandler AxisMovement;


        [Header("General settings")]

        [Tooltip("If this is checked then the left controller touchpad will be enabled for the selected movement types.")]
        public bool leftController = true;
        [Tooltip("If this is checked then the right controller touchpad will be enabled for the selected movement types.")]
        public bool rightController = true;
        [Tooltip("If a button is defined then the selected movement will only be performed when the specified button is being held down and the touchpad axis changes.")]
        public VRTK_ControllerEvents.ButtonAlias moveOnButtonPress = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("If the defined movement multiplier button is pressed then the movement will be affected by the axis multiplier value.")]
        public VRTK_ControllerEvents.ButtonAlias movementMultiplierButton = VRTK_ControllerEvents.ButtonAlias.Undefined;

        [Header("Vertical Axis")]
        [Tooltip("Selects the main movement type to be performed when the vertical axis changes occur.")]
        public VerticalAxisMovement verticalAxisMovement = VerticalAxisMovement.Slide;
        [Tooltip("Dead zone for the vertical axis. High value recommended for warp movement.")]
        [Range(0, 1)]
        public float verticalDeadzone = 0.2f;
        [Tooltip("Multiplier for the vertical axis movement when the multiplier button is pressed.")]
        public float verticalMultiplier = 1.5f;
        [Tooltip("The direction that will be moved in is the direction of this device.")]
        public VRTK_DeviceFinder.Devices deviceForDirection = VRTK_DeviceFinder.Devices.Headset;

        [Header("Direction flip")]
        [Tooltip("Enables a secondary action of a direction flip of 180 degrees when the touchpad is pulled downwards.")]
        public bool flipDirectionEnabled;
        [Tooltip("Dead zone for the downwards pull. High value recommended.")]
        [Range(0, 1)]
        public float flipDeadzone = 0.7f;
        [Tooltip("The delay before the next direction flip is allowed to happen.")]
        public float flipDelay = 1;
        [Tooltip("Enables blink on flip.")]
        public bool flipBlink = true;

        [Header("Horizontal Axis")]
        [Tooltip("Selects the movement type to be performed when the horizontal axis changes occur.")]
        public HorizontalAxisMovement horizontalAxisMovement = HorizontalAxisMovement.Slide;
        [Tooltip("Dead zone for the horizontal axis. High value recommended for snap rotate and warp movement.")]
        [Range(0, 1)]
        public float horizontalDeadzone = 0.2f;
        [Tooltip("Multiplier for the horizontal axis movement when the multiplier button is pressed.")]
        public float horizontalMultiplier = 1.25f;
        [Tooltip("The delay before the next snap rotation is allowed to happen.")]
        public float snapRotateDelay = 0.5f;
        [Tooltip("The number of degrees to instantly rotate in to the given direction.")]
        public float snapRotateAngle = 30;
        [Tooltip("The maximum speed the play area will be rotated when the touchpad is being touched at the extremes of the axis. If a lower part of the touchpad axis is touched (nearer the centre) then the rotation speed is slower.")]
        public float rotateMaxSpeed = 3;

        [Header("Shared Axis Settings")]
        [Tooltip("Blink effect duration multiplier for the movement delay, ie. 1.0 means blink transition lasts until the delay has expired and 0.5 means the effect has completed when half of the delay time is done.")]
        [Range(0.1f, 1)]
        public float blinkDurationMultiplier = 0.7f;
        [Tooltip("The maximum speed the play area will be moved by sliding when the touchpad is being touched at the extremes of the axis. If a lower part of the touchpad axis is touched (nearer the centre) then the speed is slower.")]
        public float slideMaxSpeed = 3f;
        [Tooltip("The speed in which the play area slows down to a complete stop when the user is no longer touching the touchpad. This deceleration effect can ease any motion sickness that may be suffered.")]
        public float slideDeceleration = 0.1f;
        [Tooltip("The delay before the next warp is allowed to happen.")]
        public float warpDelay = 0.5f;
        [Tooltip("The distance to warp in to the given direction.")]
        public float warpRange = 1;
        [Tooltip("The maximum altitude change allowed for a warp to happen.")]
        public float warpMaxAltitudeChange = 1;

        private GameObject controllerLeftHand;
        private GameObject controllerRightHand;
        private Transform playArea;
        private Vector2 touchAxis;
        private float movementSpeed;
        private float strafeSpeed;
        private float blinkFadeInTime;
        private float lastWarp;
        private float lastFlip;
        private float lastSnapRotate;
        private bool multiplyMovement;
        private CapsuleCollider bodyCollider;
        private Transform headset;
        private bool leftSubscribed;
        private bool rightSubscribed;
        private ControllerInteractionEventHandler touchpadAxisChanged;
        private ControllerInteractionEventHandler touchpadUntouched;
        private VRTK_ControllerEvents controllerEvents;
        private VRTK_BodyPhysics bodyPhysics;
        private bool wasFalling;
        private bool previousLeftControllerState;
        private bool previousRightControllerState;

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            touchpadAxisChanged = new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            touchpadUntouched = new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

            controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();

            playArea = VRTK_DeviceFinder.PlayAreaTransform();
            if (!playArea)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
            }

            headset = VRTK_DeviceFinder.HeadsetTransform();
            if (!headset)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "HeadsetTransform", "Headset SDK"));
            }

            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);

            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed);
            bodyPhysics = GetComponent<VRTK_BodyPhysics>();

            movementSpeed = 0f;
            strafeSpeed = 0f;
            blinkFadeInTime = 0f;
            lastWarp = 0f;
            lastFlip = 0f;
            lastSnapRotate = 0f;
            multiplyMovement = false;

            bodyCollider = playArea.GetComponentInChildren<CapsuleCollider>();
            if (!bodyCollider)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_TouchpadMovement", "CapsuleCollider", "the PlayArea"));
            }
        }

        protected virtual void OnDisable()
        {
            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed, true);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed, true);
            bodyPhysics = null;
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            multiplyMovement = (controllerEvents && movementMultiplierButton != VRTK_ControllerEvents.ButtonAlias.Undefined && controllerEvents.IsButtonPressed(movementMultiplierButton));
            CheckControllerState(controllerLeftHand, leftController, ref leftSubscribed, ref previousLeftControllerState);
            CheckControllerState(controllerRightHand, rightController, ref rightSubscribed, ref previousRightControllerState);
        }

        protected virtual void FixedUpdate()
        {
            bool moved = false;

            HandleFalling();

            if (horizontalAxisMovement == HorizontalAxisMovement.Slide)
            {
                CalculateSpeed(true, ref strafeSpeed, touchAxis.x);
                moved = true;
            }
            else if (horizontalAxisMovement == HorizontalAxisMovement.Rotate)
            {
                Rotate();
            }
            else if ((horizontalAxisMovement == HorizontalAxisMovement.SnapRotate || horizontalAxisMovement == HorizontalAxisMovement.SnapRotateWithBlink) && Mathf.Abs(touchAxis.x) > horizontalDeadzone && lastSnapRotate < Time.timeSinceLevelLoad)
            {
                SnapRotate(horizontalAxisMovement == HorizontalAxisMovement.SnapRotateWithBlink);
            }
            else if ((horizontalAxisMovement == HorizontalAxisMovement.Warp || horizontalAxisMovement == HorizontalAxisMovement.WarpWithBlink) && Mathf.Abs(touchAxis.x) > horizontalDeadzone && lastWarp < Time.timeSinceLevelLoad)
            {
                Warp(horizontalAxisMovement == HorizontalAxisMovement.WarpWithBlink, true);
            }

            if (flipDirectionEnabled && touchAxis.y < 0)
            {
                if (touchAxis.y < -flipDeadzone && lastFlip < Time.timeSinceLevelLoad)
                {
                    lastFlip = Time.timeSinceLevelLoad + flipDelay;
                    SnapRotate(flipBlink, true);
                }
            }
            else if (verticalAxisMovement == VerticalAxisMovement.Slide)
            {
                CalculateSpeed(false, ref movementSpeed, touchAxis.y);
                moved = true;
            }
            else if ((verticalAxisMovement == VerticalAxisMovement.Warp || verticalAxisMovement == VerticalAxisMovement.WarpWithBlink) && Mathf.Abs(touchAxis.y) > verticalDeadzone && lastWarp < Time.timeSinceLevelLoad)
            {
                Warp(verticalAxisMovement == VerticalAxisMovement.WarpWithBlink);
            }

            if (moved)
            {
                Move();
            }
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

            var axis = e.touchpadAxis;
            var norm = axis.normalized;
            var mag = axis.magnitude;

            if (axis.y < verticalDeadzone && axis.y > -verticalDeadzone)
            {
                axis.y = 0;
            }
            else
            {
                axis.y = (norm * ((mag - verticalDeadzone) / (1 - verticalDeadzone))).y;
            }

            if (axis.x < horizontalDeadzone && axis.x > -horizontalDeadzone)
            {
                axis.x = 0;
            }
            else
            {
                axis.x = (norm * ((mag - horizontalDeadzone) / (1 - horizontalDeadzone))).x;
            }

            touchAxis = axis;
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            touchAxis = Vector2.zero;
            controllerEvents = null;
        }

        private void OnAxisMovement(AxisMovementType givenMovementType, AxisMovementDirection givenDirection)
        {
            if (AxisMovement != null)
            {
                var args = new TouchpadMovementAxisEventArgs();
                args.movementType = givenMovementType;
                args.direction = givenDirection;
                AxisMovement(this, args);
            }
        }

        private void CalculateSpeed(bool horizontal, ref float speed, float inputValue)
        {
            if (inputValue != 0f)
            {
                speed = (slideMaxSpeed * inputValue);
                speed = (multiplyMovement ? speed * (horizontal ? horizontalMultiplier : verticalMultiplier) : speed);
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
                speed -= Mathf.Lerp(slideDeceleration, slideMaxSpeed, 0f);
            }
            else if (speed < 0)
            {
                speed += Mathf.Lerp(slideDeceleration, -slideMaxSpeed, 0f);
            }
            else
            {
                speed = 0;
            }

            if (speed < verticalDeadzone && speed > -verticalDeadzone)
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

        private void Warp(bool blink = false, bool horizontal = false)
        {
            var distance = warpRange * (multiplyMovement ? (horizontal ? horizontalMultiplier : verticalMultiplier) : 1);
            var deviceDirector = VRTK_DeviceFinder.DeviceTransform(deviceForDirection);
            var bodyPos = playArea.TransformPoint(bodyCollider.center);
            var targetPos = bodyPos + ((horizontal ? deviceDirector.right : deviceDirector.forward) * distance * ((horizontal ? touchAxis.x : touchAxis.y) < 0 ? -1 : 1));

            var headMargin = 0.2f;
            RaycastHit rh;

            // direction raycast to stop near obstacles
            Vector3 direction = (horizontal ? (touchAxis.x < 0 ? headset.right : headset.right * -1) : (touchAxis.y > 0 ? headset.forward : headset.forward * -1));
            if (Physics.Raycast(headset.position + (Vector3.up * headMargin), direction, out rh, distance))
            {
                targetPos = rh.point - (direction * bodyCollider.radius);
            }

            // vertical raycast for height position
            if (Physics.Raycast(targetPos + (Vector3.up * (warpMaxAltitudeChange + headMargin)), Vector3.down, out rh, (warpMaxAltitudeChange + headMargin) * 2))
            {
                targetPos.y = rh.point.y + (bodyCollider.height / 2);

                lastWarp = Time.timeSinceLevelLoad + warpDelay;
                playArea.position = targetPos - bodyPos + playArea.position;

                if (blink)
                {
                    blinkFadeInTime = warpDelay * blinkDurationMultiplier;
                    VRTK_SDK_Bridge.HeadsetFade(Color.black, 0);
                    Invoke("ReleaseBlink", 0.01f);
                }

                OnAxisMovement(AxisMovementType.Warp, (horizontal ? (touchAxis.x < 0 ? AxisMovementDirection.Left : AxisMovementDirection.Right) : (touchAxis.y < 0 ? AxisMovementDirection.Backward : AxisMovementDirection.Forward)));
            }
        }

        private void RotateAroundPlayer(float angle)
        {
            var pos = playArea.TransformPoint(bodyCollider.center);
            playArea.Rotate(Vector3.up, angle);
            pos -= playArea.TransformPoint(bodyCollider.center);
            playArea.position += pos;
        }

        private void Rotate()
        {
            var angle = touchAxis.x * rotateMaxSpeed * Time.deltaTime * (multiplyMovement ? horizontalMultiplier : 1) * 10;
            RotateAroundPlayer(angle);
        }

        private void SnapRotate(bool blink = false, bool flipDirection = false)
        {
            lastSnapRotate = Time.timeSinceLevelLoad + snapRotateDelay;

            var angle = (flipDirection ? 180 : (snapRotateAngle * (multiplyMovement ? horizontalMultiplier : 1)) * (touchAxis.x < 0 ? -1 : 1));
            RotateAroundPlayer(angle);

            if (blink)
            {
                blinkFadeInTime = snapRotateDelay * blinkDurationMultiplier;
                VRTK_SDK_Bridge.HeadsetFade(Color.black, 0);
                Invoke("ReleaseBlink", 0.01f);
            }

            OnAxisMovement((flipDirection ? AxisMovementType.FlipDirection : AxisMovementType.SnapRotate), (touchAxis.x < 0 ? AxisMovementDirection.Left : AxisMovementDirection.Right));
        }

        private void ReleaseBlink()
        {
            VRTK_SDK_Bridge.HeadsetFade(Color.clear, blinkFadeInTime);
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