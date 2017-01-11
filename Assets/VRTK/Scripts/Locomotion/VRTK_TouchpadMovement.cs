// Touchpad Movement|Locomotion|20062
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Adds the ability to move and rotate play area by using the touchpad. 
    /// </summary>
    /// <remarks>
    /// The Touchpad Movement script requires VRTK_BodyPhysics script to be present in one of the scene GameObjects for collision detection. 
    /// Different movement types can be split across the controllers by having one script per side and with the desired options. 
    /// Warp and snap rotate options may provide more comfortable experience for some and blink can be used to soften the effects. 
    /// Snap rotate and flip direction options can be useful with teleport scripts for seated experiences and for people using front facing camera setups (Oculus default, PSVR). 
    /// </remarks>
    public class VRTK_TouchpadMovement : MonoBehaviour
    {
        /// <summary>
        /// Movement types that can be performed by the vertical axis.
        /// </summary>
        public enum VerticalAxisMovement
        {
            /// <summary>
            /// No main movement is performed.
            /// </summary>
            None,
            /// <summary>
            /// Performs smooth movement (walk).
            /// </summary>
            Slide,
            /// <summary>
            /// Performs an instant warp movement.
            /// </summary>
            Warp,
            /// <summary>
            /// Performs an instant warp movement with a blink effect.
            /// </summary>
            WarpWithBlink
        }

        /// <summary>
        /// Movement types that can be performed by the horizontal axis.
        /// </summary>
        public enum HorizontalAxisMovement
        {
            /// <summary>
            /// No movement is performed.
            /// </summary>
            None,
            /// <summary>
            /// Performs smooth movement (strafe).
            /// </summary>
            Slide,
            /// <summary>
            /// Performs smooth rotation.
            /// </summary>
            Rotate,
            /// <summary>
            /// Performs fixed angle rotation.
            /// </summary>
            SnapRotate,
            /// <summary>
            /// Performs fixed angle rotation with a blink effect.
            /// </summary>
            SnapRotateWithBlink,
            /// <summary>
            /// Performs an instant warp movement.
            /// </summary>
            Warp,
            /// <summary>
            /// Performs an instant warp movement with a blink effect.
            /// </summary>
            WarpWithBlink
        }   

        /// <summary>
        /// Which type axis movement did occur.
        /// </summary>
        public enum AxisMovementType
        {
            /// <summary>
            /// User warped.
            /// </summary>
            Warp,
            /// <summary>
            /// User flipped the direction.
            /// </summary>
            FlipDirection,
            /// <summary>
            /// User snap rotated.
            /// </summary>
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

        public delegate void AxisMovementEventHandler(VRTK_TouchpadMovement sender, AxisMovementType movementType, AxisMovementDirection direction);

        /// <summary>
        /// Emitted when a warp, a flip direction or a snap rotate movement has successfully completed.
        /// </summary>
        public event AxisMovementEventHandler AxisMovement;

        public bool LeftController
        {
            get { return leftController; }
            set
            {
                leftController = value;
                SetControllerListeners(controllerLeftHand);
            }
        }

        public bool RightController
        {
            get { return rightController; }
            set
            {
                rightController = value;
                SetControllerListeners(controllerRightHand);
            }
        }

        [Header("General settings")]
        [Tooltip("If this is checked then the left controller touchpad will be enabled for the selected movement types. It can also be toggled at runtime.")]
        [SerializeField]
        private bool leftController = true;
        [Tooltip("If this is checked then the right controller touchpad will be enabled for the selected movement types. It can also be toggled at runtime.")]
        [SerializeField]
        private bool rightController = true;
        [Tooltip("If a button is defined then the selected movement will only be performed when the specified button is being held down and the touchpad axis changes.")]
        public VRTK_ControllerEvents.ButtonAlias moveOnButtonPress = VRTK_ControllerEvents.ButtonAlias.Undefined;

        [Header("Vertical Axis")]
        [Tooltip("Selects the main movement type to be performed when the vertical axis changes occur.")]
        public VerticalAxisMovement verticalAxisMovement = VerticalAxisMovement.Slide;
        [Tooltip("Dead zone for the vertical axis. High value recommended for warp movement.")]
        [Range(0, 1)]
        public float verticalDeadzone = 0.2f;
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
        private float movementSpeed = 0f;
        private float strafeSpeed = 0f;
        private float blinkFadeInTime = 0;
        private float lastWarp = 0;
        private float lastFlip = 0;
        private float lastSnapRotate = 0;
        private CapsuleCollider bodyCollider;
        private Transform headset;
        private bool leftSubscribed;
        private bool rightSubscribed;
        private ControllerInteractionEventHandler touchpadAxisChanged;
        private ControllerInteractionEventHandler touchpadUntouched;

        private void Awake()
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

            headset = VRTK_DeviceFinder.HeadsetTransform();
            if (!headset)
            {
                Debug.LogError("No headset could be found. Have you selected a valid Headset SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Boundaries SDK from the dropdown.");
            }
        }

        private void Start()
        {
            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
            SetControllerListeners(controllerLeftHand);
            SetControllerListeners(controllerRightHand);

            bodyCollider = playArea.GetComponent<CapsuleCollider>();
            if (!bodyCollider)
            {
                Debug.LogError("No body collider could be found in the play area. VRTK_BodyPhysics script is required in one of the scene GameObjects.");
            }
        }

        protected void OnAxisMovement(AxisMovementType movementType, AxisMovementDirection direction)
        {
            if (AxisMovement != null)
            {
                AxisMovement(this, movementType, direction);
            }
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            var controllerEvents = (VRTK_ControllerEvents)sender;
            if (moveOnButtonPress != VRTK_ControllerEvents.ButtonAlias.Undefined && !controllerEvents.IsButtonPressed(moveOnButtonPress))
            {
                touchAxis = Vector2.zero;
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
        }

        private void CalculateSpeed(ref float speed, float inputValue)
        {
            if (inputValue != 0f)
            {
                speed = (slideMaxSpeed * inputValue);
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

        private void Warp(bool blink = false, bool sideways = false)
        {
            var deviceDirector = VRTK_DeviceFinder.DeviceTransform(deviceForDirection);
            var bodyPos = playArea.TransformPoint(bodyCollider.center);
            var targetPos = bodyPos + ((sideways ? deviceDirector.right : deviceDirector.forward) * warpRange * ((sideways ? touchAxis.x : touchAxis.y) < 0 ? -1 : 1));

            var headMargin = 0.2f;
            RaycastHit rh;

            // direction raycast to stop near obstacles
            Vector3 direction = (sideways ? (touchAxis.x < 0 ? headset.right : headset.right * -1) : (touchAxis.y > 0 ? headset.forward : headset.forward * -1));
            if (Physics.Raycast(headset.position + (Vector3.up * headMargin), direction, out rh, warpRange))
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

                OnAxisMovement(AxisMovementType.Warp, (sideways ? (touchAxis.x < 0 ? AxisMovementDirection.Left : AxisMovementDirection.Right) : (touchAxis.y < 0 ? AxisMovementDirection.Backward : AxisMovementDirection.Forward)));
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
            var angle = touchAxis.x * rotateMaxSpeed * Time.deltaTime * 10;
            RotateAroundPlayer(angle);
        }

        private void SnapRotate(bool blink = false, bool flipDirection = false)
        {
            lastSnapRotate = Time.timeSinceLevelLoad + snapRotateDelay;

            var angle = (flipDirection ? 180 : snapRotateAngle * (touchAxis.x < 0 ? -1 : 1));
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

        private void FixedUpdate()
        {
            bool moved = false;

            if (horizontalAxisMovement == HorizontalAxisMovement.Slide)
            {
                CalculateSpeed(ref strafeSpeed, touchAxis.x);
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
                CalculateSpeed(ref movementSpeed, touchAxis.y);
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

        private void SetControllerListeners(GameObject controller)
        {
            if (controller && VRTK_DeviceFinder.IsControllerLeftHand(controller))
            {
                ToggleControllerListeners(controller, leftController, ref leftSubscribed);
            }
            else if (controller && VRTK_DeviceFinder.IsControllerRightHand(controller))
            {
                ToggleControllerListeners(controller, rightController, ref rightSubscribed);
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