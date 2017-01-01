// Touchpad Walking|Locomotion|20060
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The ability to move the play area around the game world by sliding a finger over the touchpad is achieved using this script.
    /// </summary>
    /// <remarks>
    /// The Touchpad Walking script adds a rigidbody and a box collider to the user's position to prevent them from walking through other collidable game objects.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/017_CameraRig_TouchpadWalking` has a collection of walls and slopes that can be traversed by the user with the touchpad. There is also an area that can only be traversed if the user is crouching.
    /// </example>
    public class VRTK_TouchpadWalking : MonoBehaviour
    {
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

        [Tooltip("If this is checked then the left controller touchpad will be enabled to move the play area. It can also be toggled at runtime.")]
        [SerializeField]
        private bool leftController = true;
        [Tooltip("If this is checked then the right controller touchpad will be enabled to move the play area. It can also be toggled at runtime.")]
        [SerializeField]
        private bool rightController = true;

        [Tooltip("The maximum speed the play area will be moved when the touchpad is being touched at the extremes of the axis. If a lower part of the touchpad axis is touched (nearer the centre) then the walk speed is slower.")]
        public float maxWalkSpeed = 3f;
        [Tooltip("The speed in which the play area slows down to a complete stop when the user is no longer touching the touchpad. This deceleration effect can ease any motion sickness that may be suffered.")]
        public float deceleration = 0.1f;
        [Tooltip("If a button is defined then movement will only occur when the specified button is being held down and the touchpad axis changes.")]
        public VRTK_ControllerEvents.ButtonAlias moveOnButtonPress = VRTK_ControllerEvents.ButtonAlias.Undefined;
        [Tooltip("The direction that will be moved in is the direction of this device.")]
        public VRTK_DeviceFinder.Devices deviceForDirection = VRTK_DeviceFinder.Devices.Headset;

        private GameObject controllerLeftHand;
        private GameObject controllerRightHand;
        private Transform playArea;
        private Vector2 touchAxis;
        private float movementSpeed = 0f;
        private float strafeSpeed = 0f;
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
        }

        private void Start()
        {
            VRTK_PlayerObject.SetPlayerObject(gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
            SetControllerListeners(controllerLeftHand);
            SetControllerListeners(controllerRightHand);
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            var controllerEvents = (VRTK_ControllerEvents)sender;
            if (moveOnButtonPress != VRTK_ControllerEvents.ButtonAlias.Undefined && !controllerEvents.IsButtonPressed(moveOnButtonPress))
            {
                touchAxis = Vector2.zero;
                return;
            }
            touchAxis = e.touchpadAxis;
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            touchAxis = Vector2.zero;
        }

        private void CalculateSpeed(ref float speed, float inputValue)
        {
            if (inputValue != 0f)
            {
                speed = (maxWalkSpeed * inputValue);
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

        private void FixedUpdate()
        {
            CalculateSpeed(ref movementSpeed, touchAxis.y);
            CalculateSpeed(ref strafeSpeed, touchAxis.x);
            Move();
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