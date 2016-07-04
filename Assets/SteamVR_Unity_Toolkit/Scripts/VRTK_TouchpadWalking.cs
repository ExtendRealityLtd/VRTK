namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_TouchpadWalking : MonoBehaviour
    {
        [SerializeField]
        private bool leftController = true;
        public bool LeftController
        {
            get { return leftController; }
            set
            {
                leftController = value;
                SetControllerListeners(controllerManager.left);
            }
        }

        [SerializeField]
        private bool rightController = true;
        public bool RightController
        {
            get { return rightController; }
            set
            {
                rightController = value;
                SetControllerListeners(controllerManager.right);
            }
        }

        public float maxWalkSpeed = 3f;
        public float deceleration = 0.1f;

        private SteamVR_ControllerManager controllerManager;
        private Vector2 touchAxis;
        private float movementSpeed = 0f;
        private float strafeSpeed = 0f;

        private bool leftSubscribed;
        private bool rightSubscribed;

        private ControllerInteractionEventHandler touchpadAxisChanged;
        private ControllerInteractionEventHandler touchpadUntouched;

        private VRTK_PlayerPresence playerPresence;

        private void Awake()
        {
            if (this.GetComponent<VRTK_PlayerPresence>())
            {
                playerPresence = this.GetComponent<VRTK_PlayerPresence>();
            }
            else
            {
                Debug.LogError("The VRTK_TouchpadWalking script requires the VRTK_PlayerPresence script to be attached to the [CameraRig]");
            }

            touchpadAxisChanged = new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            touchpadUntouched = new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

            controllerManager = this.GetComponent<SteamVR_ControllerManager>();
        }

        private void Start()
        {
            Utilities.SetPlayerObject(this.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);

            var controllerManager = GameObject.FindObjectOfType<SteamVR_ControllerManager>();

            SetControllerListeners(controllerManager.left);
            SetControllerListeners(controllerManager.right);
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
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
            var movement = playerPresence.GetHeadset().forward * movementSpeed * Time.deltaTime;
            var strafe = playerPresence.GetHeadset().right * strafeSpeed * Time.deltaTime;
            float fixY = this.transform.position.y;
            this.transform.position += (movement + strafe);
            this.transform.position = new Vector3(this.transform.position.x, fixY, this.transform.position.z);
        }

        private void FixedUpdate()
        {
            CalculateSpeed(ref movementSpeed, touchAxis.y);
            CalculateSpeed(ref strafeSpeed, touchAxis.x);
            Move();
        }

        private void SetControllerListeners(GameObject controller)
        {
            if (controller && controller == controllerManager.left)
            {
                ToggleControllerListeners(controller, leftController, ref leftSubscribed);
            }
            else if (controller && controller == controllerManager.right)
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