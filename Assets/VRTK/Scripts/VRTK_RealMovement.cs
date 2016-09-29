// Touchpad Walking|Scripts|0140
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    [RequireComponent(typeof(VRTK_PlayerPresence))]

    public class VRTK_RealMovement : MonoBehaviour
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

        // Amount of frames to sample to smooth out the movement.  Higher frames = higher stability?
        [SerializeField]
        [Tooltip("How many samples should be used to determine average movement speed. (60's good bro, don't change unless you know what you're doing)")]
        private int m_averagePeriod = 60;

        // How quick you should move.
        [SerializeField]
        [Tooltip("Lower to decrease speed, raise to increase.")]
        private float m_speedScale = 1;

        [SerializeField]
        [Tooltip("The max speed you can move in game units. (If 0 or less, max speed is uncapped)")]
        private float m_maxSpeed = 4;

        [SerializeField]
        [Tooltip("How your movement direction will be determined.  The Gaze method tends to lead to the least motion sickness.  Smart decoupling is still a Work In Progress.  Using the trackpad will override all other Immersive Movement functions.")]
        private DirectionalMethod m_dirMethod = DirectionalMethod.Gaze;

        [SerializeField]
        [Tooltip("If we're using Smart Decoupling, .")]
        private float m_smartDecoupleThreshold = 30f;

        // Which tracked objects to use to determine amount of movement.
        private List<Transform> m_trackedObjects = new List<Transform>();

        // The cap before we stop adding the delta to the movement list.  This will help regulate speed.
        [SerializeField]
        [Tooltip("The max amount of movement we wish to register in the virutal world.  Decreasing this will increase acceleration, and vice versa.")]
        private float m_sensitivity = 0.02f;

        private float m_cameraLastYRotation = 0f;

        public VRTK_PlayerPresence m_playerBody;
        private Rigidbody m_rigidBody;
        private GameObject controllerLeftHand;
        private GameObject controllerRightHand;
        private Transform headset;
        private bool leftSubscribed;
        private bool rightSubscribed;
        private ControllerInteractionEventHandler touchpadPressed;
        private ControllerInteractionEventHandler touchpadUp;

        // List of all the frames movements over the average period.
        private Dictionary<Transform, List<float>> m_movementList = new Dictionary<Transform, List<float>>();
        private Dictionary<Transform, float> m_previousYPositions = new Dictionary<Transform, float>();

        private float m_playAreaPreviousY = 0f;

        private Vector3 m_initalGaze = Vector3.zero;

        private Dictionary<Transform, Vector3> m_initialTrackableDirs = new Dictionary<Transform, Vector3>();

        private float m_curSpeed = 0.00f;

        private Vector3 m_direction = new Vector3();

        private bool active = false;

        public VRTK_RealMovement()
        {
            // Constructor population no longer works in 5.4 due to new restrictions.
#if (!UNITY_5_4)
        m_trackedObjects = new List<Transform>();

        foreach (Camera obj in GetComponentsInChildren<Camera>())
        {
            // If we're not using 5.4, we want to track the head.
            if (obj.name.ToLower().Contains("head"))
        // If we're using 5.4, we want to track the eye, not the head.
        if (obj.name.ToLower().Contains("eye"))
            {
                m_camera = obj.transform;
                m_trackedObjects.Add(obj.transform);
            }
        }

        foreach (SteamVR_TrackedObject obj in GetComponentsInChildren<SteamVR_TrackedObject>())
        {
            if (obj.name.ToLower().Contains("controller"))
            {
                if (obj.name.ToLower().Contains("right"))
                {
                    m_controller = obj.GetComponent<SteamVR_TrackedObject>();
                }
                m_trackedObjects.Add(obj.transform);
            }
        }
#endif
        }

        // Use this for initialization
        void Start()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            SetControllerListeners(controllerLeftHand);
            SetControllerListeners(controllerRightHand);
            // Initialize the lists.
            foreach (Transform trackedObj in m_trackedObjects)
            {
                m_movementList.Add(trackedObj, new List<float>());
                m_previousYPositions.Add(trackedObj, trackedObj.transform.localPosition.y);
                m_cameraLastYRotation = headset.transform.rotation.y;
            }
            m_playAreaPreviousY = transform.position.y;
        }

        // If we're in Unity 5.4, we need to make sure we are using the eyes, not the head.
        void Awake()
        {
            controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
            headset = VRTK_DeviceFinder.HeadsetTransform();

            m_trackedObjects.Add(controllerLeftHand.transform);
            m_trackedObjects.Add(controllerRightHand.transform);
            m_trackedObjects.Add(headset.transform);

            touchpadPressed = new ControllerInteractionEventHandler(DoTouchpadDown);
            touchpadUp = new ControllerInteractionEventHandler(DoTouchpadUp);
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
            foreach (Transform obj in m_trackedObjects)
            {
                m_movementList[obj].Clear();
            }
            m_initalGaze = Vector3.zero;
            m_initialTrackableDirs = new Dictionary<Transform, Vector3>();
            m_direction = Vector3.zero;
            m_curSpeed = 0;

            active = false;
        }

        // FixedUpdate is called at a regular rate.
        void FixedUpdate()
        { 
            // If the button is pressed...
            if (active)
            {
                // Initialze the average and crosscount.
                float listAverage = 0;

                float m_playAreaDeltaY = Mathf.Abs(m_playAreaPreviousY - transform.position.y);

                foreach (Transform trackedObj in m_trackedObjects)
                {
                    // Get the amount of Y movement that's occured since the last update.
                    float deltaYPostion = Mathf.Abs(m_previousYPositions[trackedObj] - trackedObj.transform.localPosition.y);

                    //deltaYPostion = Mathf.Max(0, deltaYPostion - (m_playAreaDeltaY));

                    // Convenience code.
                    List<float> trackedObjList = m_movementList[trackedObj];

                    if ((m_playerBody != null && (!m_playerBody.IsFalling() /*|| m_playerBody.IsClimbing()*/)) || m_playerBody == null)
                    {
                        // Cap off the speed.
                        if (deltaYPostion > m_sensitivity)
                        {
                            trackedObjList.Add(m_sensitivity);
                        }
                        else {
                            trackedObjList.Add(deltaYPostion);
                        }

                        // Keep our tracking list at m_averagePeriod number of elements.
                        if (trackedObjList.Count > m_averagePeriod)
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
                    float avg = sum / m_averagePeriod;

                    // Add the average to the the list average.
                    listAverage += avg;
                }

                float speed = ((m_speedScale * 350) * (listAverage / m_trackedObjects.Count));

                if (speed > m_maxSpeed && m_maxSpeed >= 0)
                    speed = m_maxSpeed;

                m_direction = Vector3.zero;

                if (m_dirMethod == DirectionalMethod.SmartDecoupling || m_dirMethod == DirectionalMethod.DumbDecoupling)
                {
                    if (m_initalGaze.Equals(Vector3.zero))
                    {
                        m_initalGaze = new Vector3(headset.forward.x, 0, headset.forward.z);
                    }

                    if (m_dirMethod == DirectionalMethod.SmartDecoupling)
                    {
                        bool closeEnough = true;
                        float curXDir = headset.rotation.eulerAngles.y;
                        if (curXDir <= m_smartDecoupleThreshold)
                        {
                            curXDir += 360;
                        }
                    
                        closeEnough = closeEnough && (Mathf.Abs(curXDir - controllerLeftHand.transform.rotation.eulerAngles.y) <= m_smartDecoupleThreshold);
                        closeEnough = closeEnough && (Mathf.Abs(curXDir - controllerRightHand.transform.rotation.eulerAngles.y) <= m_smartDecoupleThreshold);

                        if (closeEnough)
                            m_initalGaze = new Vector3(headset.forward.x, 0, headset.forward.z);
                    }
                    m_direction = m_initalGaze;
                }
                else if (m_dirMethod.Equals(DirectionalMethod.Gaze))
                {
                    m_direction = (new Vector3(headset.forward.x, 0, headset.forward.z));
                }

                m_curSpeed = speed;

                m_cameraLastYRotation = m_direction.y;
            }

            foreach (Transform trackedObj in m_trackedObjects)
            {
                // Get delta postions and rotations
                m_previousYPositions[trackedObj] = trackedObj.transform.localPosition.y;
            }

            Vector3 movement = (m_direction * m_curSpeed) * Time.fixedDeltaTime;

            /*if (m_playerBody != null)
            {
                m_playerBody.updatePlayerBodyPosition();
                Vector3 cameraDistance = m_playerBody.getCameraDistance();
                movement = movement + m_playerBody.getImmersiveMovementModifier() - (cameraDistance);
                m_playerBody.resetImmersiveMovementModifier();
            }*/

            m_rigidBody.MovePosition(m_rigidBody.transform.position + movement);

            m_playAreaPreviousY = transform.position.y;
        }

        public Vector3 getMovementDirection()
        {
            return m_direction;
        }

        public float getSpeed()
        {
            return m_curSpeed;
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

    [System.Serializable]

    public enum DirectionalMethod : int
    {
        Gaze,
        DumbDecoupling,
        SmartDecoupling
    }
}