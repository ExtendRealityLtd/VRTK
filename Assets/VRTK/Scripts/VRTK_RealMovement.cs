// Real Movement|Scripts|0145
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    [RequireComponent(typeof(VRTK_PlayerPresence))]

    /// <summary>
    /// Real Movement allows you to move the play area by calculating the y-movement of your controllers and propelling you the more they are moving.  This simulates walking in game by walking in real life.
    /// This locomotion method is based on Immersive Movement, originally created by Highsight.
    /// </summary>
    /// <remarks>
    /// It is recommend you combine this with the VRTK_HeightAdjustTeleport
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/042_CameraRig_RealMovement` shows how the user can move and traverse colliders.
    /// </example>
    public class VRTK_RealMovement : MonoBehaviour
    {

        /// <summary>
        /// Options for testing if a play space fall is valid.
        /// </summary>
        /// <param name="HeadsetAndControllers">Track both headset and controllers for movement calculations.</param>
        /// <param name="ControllersOnly">Track only the controllers for movement calculations.</param>
        /// <param name="HeadsetOnly">Track only headset for movement caluclations.</param>
        public enum ControlOptions
        {
            HeadsetAndControllers,
            ControllersOnly,
            HeadsetOnly,
        }

        /// <summary>
        /// Options for which method is used to determine player direction while moving.
        /// </summary>
        /// <param name="Gaze">Player will always move in the direction they are currently looking.</param>
        /// <param name="DumbDecoupling">Player will move in the direction they were first looking when they engaged Real Movement.</param>
        /// <param name="SmartDecoupling">Player will move in the direction they are looking only if their headset point the same direction as their controllers.</param>
        public enum DirectionalMethod
        {
            Gaze,
            DumbDecoupling,
            SmartDecoupling
        }

        /// <summary>
        /// If true, the left controller's trackpad will engage Real Movement.
        /// </summary>
        public bool LeftController
        {
            get { return leftController; }
            set
            {
                leftController = value;
                SetControllerListeners(controllerLeftHand);
            }
        }

        /// <summary>
        /// If true, the right controller's trackpad will engage Real Movement.
        /// </summary>
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

        [Tooltip("Select which trackables are used to determine movement.")]
        [SerializeField]
        public ControlOptions controlOptions = ControlOptions.HeadsetAndControllers;

        [SerializeField]
        [Tooltip("Lower to decrease speed, raise to increase.")]
        public float speedScale = 1;

        [SerializeField]
        [Tooltip("The max speed you can move in game units. (If 0 or less, max speed is uncapped)")]
        public float maxSpeed = 4;

        [SerializeField]
        [Tooltip("How your movement direction will be determined.  The Gaze method tends to lead to the least motion sickness.  Smart decoupling is still a Work In Progress.")]
        public DirectionalMethod dirMethod = DirectionalMethod.Gaze;

        [SerializeField]
        [Tooltip("If we're using Smart Decoupling, all trackables must be within this degree threshold to change direction.")]
        public float smartDecoupleThreshold = 30f;

        // The cap before we stop adding the delta to the movement list.  This will help regulate speed.
        [SerializeField]
        [Tooltip("The max amount of movement we wish to register in the virutal world.  Decreasing this will increase acceleration, and vice versa.")]
        public float sensitivity = 0.02f;

        // The maximum number of updates we should hold to process movements.  The higher the number, the slower the acceleration/deceleration & vice versa.
        private int averagePeriod = 60;
        private float cameraLastYRotation = 0f;
        
        // Which tracked objects to use to determine amount of movement.
        private List<Transform> trackedObjects = new List<Transform>();

        private VRTK_PlayerPresence playerBody;
        private Rigidbody rigidBody;

        private GameObject controllerLeftHand;
        private GameObject controllerRightHand;
        private Transform headset;
        private bool leftSubscribed;
        private bool rightSubscribed;
        private ControllerInteractionEventHandler touchpadPressed;
        private ControllerInteractionEventHandler touchpadUp;

        // List of all the update's movements over the average period.
        private Dictionary<Transform, List<float>> movementList = new Dictionary<Transform, List<float>>();
        private Dictionary<Transform, float> previousYPositions = new Dictionary<Transform, float>();

        // Keeps track of the play area's last Y position.  This number is used to determine the delta position of the play area between updates.
        // the delta y-position is then subtracted by all calculations to discount the movement of the play area from the trackable's movements. 
        private float playAreaPreviousY = 0f;

        // Used to determine the direction when using a decoupling method.
        private Vector3 initalGaze = Vector3.zero;

        // The current move speed of the player.  If Real Movement is not active, it will be set to 0.00f.
        private float curSpeed = 0.00f;

        // The current direction the player is moving.  If Real Movement is not active, it will be set to Vector.zero.
        private Vector3 direction = new Vector3();

        // True if Real Movement is currently engaged.
        private bool active = false;

        /// <summary>
        /// Set your control options and modify the trackables to match.
        /// </summary>
        public void SetControlOptions(ControlOptions controlOptions)
        {
            this.controlOptions = controlOptions;
            trackedObjects.Clear();

            if (this.controlOptions.Equals(ControlOptions.HeadsetAndControllers) || this.controlOptions.Equals(ControlOptions.ControllersOnly))
            {
                trackedObjects.Add(controllerLeftHand.transform);
                trackedObjects.Add(controllerRightHand.transform);
            }

            if (this.controlOptions.Equals(ControlOptions.HeadsetAndControllers) || this.controlOptions.Equals(ControlOptions.HeadsetOnly))
            {
                trackedObjects.Add(headset.transform);
            }
        }
        
        public Vector3 GetMovementDirection()
        {
            return direction;
        }

        public float GetSpeed()
        {
            return curSpeed;
        }

        private void Awake()
        {
            controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
            headset = VRTK_DeviceFinder.HeadsetTransform();

            SetControlOptions(controlOptions);
        }

        private void OnEnable()
        {
            touchpadPressed = new ControllerInteractionEventHandler(DoTouchpadDown);
            touchpadUp = new ControllerInteractionEventHandler(DoTouchpadUp);
        }

        private void OnDisable()
        {
            touchpadPressed -= DoTouchpadDown;
            touchpadUp -= DoTouchpadUp;
        }

        private void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            SetControllerListeners(controllerLeftHand);
            SetControllerListeners(controllerRightHand);

            // Initialize the lists.
            foreach (Transform trackedObj in trackedObjects)
            {
                movementList.Add(trackedObj, new List<float>());
                previousYPositions.Add(trackedObj, trackedObj.transform.localPosition.y);
                cameraLastYRotation = headset.transform.rotation.y;
            }
            playAreaPreviousY = transform.position.y;
        }
        
        private void DoTouchpadDown(object sender, ControllerInteractionEventArgs e)
        {
            var controllerEvents = (VRTK_ControllerEvents)sender;
            active = true;
        }

        private void DoTouchpadUp(object sender, ControllerInteractionEventArgs e)
        {
            var controllerEvents = (VRTK_ControllerEvents)sender;

            // If the button is released, clear all the lists.
            foreach (Transform obj in trackedObjects)
            {
                movementList[obj].Clear();
            }
            initalGaze = Vector3.zero;
            direction = Vector3.zero;
            curSpeed = 0;

            active = false;
        }
        
        private void FixedUpdate()
        { 
            // If Real Movement is currently engaged.
            if (active)
            {
                // Initialze the list average.
                float listAverage = 0;

                // Get the y-position delta of the play area.
                float m_playAreaDeltaY = Mathf.Abs(playAreaPreviousY - transform.position.y);

                foreach (Transform trackedObj in trackedObjects)
                {
                    // Get the amount of Y movement that's occured since the last update.
                    float deltaYPostion = Mathf.Abs(previousYPositions[trackedObj] - trackedObj.transform.localPosition.y);

                    // Convenience code.
                    List<float> trackedObjList = movementList[trackedObj];

                    if ((playerBody != null && (!playerBody.IsFalling())) || playerBody == null)
                    {
                        // Cap off the speed.
                        if (deltaYPostion > sensitivity)
                        {
                            trackedObjList.Add(sensitivity);
                        }
                        else {
                            trackedObjList.Add(deltaYPostion);
                        }

                        // Keep our tracking list at m_averagePeriod number of elements.
                        if (trackedObjList.Count > averagePeriod)
                        {
                            trackedObjList.RemoveAt(0);
                        }
                    }

                    // Average out the current tracker's list.
                    float sum = 0;
                    foreach (float diffrences in trackedObjList)
                    {
                        sum += diffrences;
                    }
                    float avg = sum / averagePeriod;

                    // Add the average to the the list average.
                    listAverage += avg;
                }

                float speed = ((speedScale * 350) * (listAverage / trackedObjects.Count));

                if (speed > maxSpeed && maxSpeed >= 0)
                {
                    speed = maxSpeed;
                }

                direction = Vector3.zero;

                // If we're doing a decoupling method...
                if (dirMethod == DirectionalMethod.SmartDecoupling || dirMethod == DirectionalMethod.DumbDecoupling)
                {
                    // If we haven't set an inital gaze yet, set it now.
                    // If we're doing dumb decoupling, this is what we'll be sticking with.
                    if (initalGaze.Equals(Vector3.zero))
                    {
                        initalGaze = new Vector3(headset.forward.x, 0, headset.forward.z);
                    }

                    // If we're doing smart decoupling, check to see if we want to reset our distance.
                    if (dirMethod == DirectionalMethod.SmartDecoupling)
                    {
                        bool closeEnough = true;
                        float curXDir = headset.rotation.eulerAngles.y;
                        if (curXDir <= smartDecoupleThreshold)
                        {
                            curXDir += 360;
                        }
                    
                        closeEnough = closeEnough && (Mathf.Abs(curXDir - controllerLeftHand.transform.rotation.eulerAngles.y) <= smartDecoupleThreshold);
                        closeEnough = closeEnough && (Mathf.Abs(curXDir - controllerRightHand.transform.rotation.eulerAngles.y) <= smartDecoupleThreshold);

                        // If the controllers and the headset are pointing the same direction (within the threshold) reset the direction the player's moving.
                        if (closeEnough)
                        {
                            initalGaze = new Vector3(headset.forward.x, 0, headset.forward.z);
                        }
                    }
                    direction = initalGaze;
                }
                // Otherwise if we're just doing Gaze movement, always set the direction to where we're looking.
                else if (dirMethod.Equals(DirectionalMethod.Gaze))
                {
                    direction = (new Vector3(headset.forward.x, 0, headset.forward.z));
                }

                // Update our current speed.
                curSpeed = speed;

                cameraLastYRotation = direction.y;
            }

            foreach (Transform trackedObj in trackedObjects)
            {
                // Get delta postions and rotations
                previousYPositions[trackedObj] = trackedObj.transform.localPosition.y;
            }

            Vector3 movement = (direction * curSpeed) * Time.fixedDeltaTime;

            rigidBody.MovePosition(rigidBody.transform.position + movement);

            playAreaPreviousY = transform.position.y;
        }

        private void SetControllerListeners(GameObject controller)
        {
            if (controller && VRTK_SDK_Bridge.IsControllerLeftHand(controller))
            {
                ToggleControllerListeners(controller, leftController, ref leftSubscribed);
            }
            else if (controller && VRTK_SDK_Bridge.IsControllerRightHand(controller))
            {
                ToggleControllerListeners(controller, rightController, ref rightSubscribed);
            }
        }

        private void ToggleControllerListeners(GameObject controller, bool toggle, ref bool subscribed)
        {
            var controllerEvent = controller.GetComponent<VRTK_ControllerEvents>();
            if (controllerEvent)
            {
                if (toggle && !subscribed)
                {
                    controllerEvent.TouchpadPressed += touchpadPressed;
                    controllerEvent.TouchpadReleased += touchpadUp;
                    subscribed = true;
                }
                else if (!toggle && subscribed)
                {
                    controllerEvent.TouchpadPressed -= touchpadPressed;
                    controllerEvent.TouchpadReleased -= touchpadUp;
                    subscribed = false;
                }
            }
        }
    }
}
