// Move In Place|Locomotion|20110
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Move In Place allows the user to move the play area by calculating the y-movement of the user's headset and/or controllers. The user is propelled forward the more they are moving. This simulates moving in game by moving in real life.
    /// </summary>
    /// <remarks>
    ///   > This locomotion method is based on Immersive Movement, originally created by Highsight.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/042_CameraRig_MoveInPlace` demonstrates how the user can move and traverse colliders by either swinging the controllers in a walking fashion or by running on the spot utilisng the head bob for movement.
    /// </example>
    [RequireComponent(typeof(VRTK_BodyPhysics))]
    public class VRTK_MoveInPlace : MonoBehaviour
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
        /// <param name="ControllerRotation">Player will move in the direction that the controllers are pointing (averaged).</param>
        /// <param name="DumbDecoupling">Player will move in the direction they were first looking when they engaged Move In Place.</param>
        /// <param name="SmartDecoupling">Player will move in the direction they are looking only if their headset point the same direction as their controllers.</param>
        public enum DirectionalMethod
        {
            Gaze,
            ControllerRotation,
            DumbDecoupling,
            SmartDecoupling
        }

        [Tooltip("If this is checked then the left controller touchpad will be enabled to move the play area.")]
        public bool leftController = true;
        [Tooltip("If this is checked then the right controller touchpad will be enabled to move the play area.")]
        public bool rightController = true;
        [Tooltip("Select which button to hold to engage Move In Place.")]
        public VRTK_ControllerEvents.ButtonAlias engageButton = VRTK_ControllerEvents.ButtonAlias.Touchpad_Press;
        [Tooltip("Select which trackables are used to determine movement.")]
        public ControlOptions controlOptions = ControlOptions.HeadsetAndControllers;
        [Tooltip("Lower to decrease speed, raise to increase.")]
        public float speedScale = 1;
        [Tooltip("The max speed the user can move in game units. (If 0 or less, max speed is uncapped)")]
        public float maxSpeed = 4;
        [Tooltip("The speed in which the play area slows down to a complete stop when the user is no longer pressing the engage button. This deceleration effect can ease any motion sickness that may be suffered.")]
        public float deceleration = 0.1f;
        [Tooltip("The speed in which the play area slows down to a complete stop when the user is falling.")]
        public float fallingDeceleration = 0.01f;
        [Tooltip("How the user's movement direction will be determined.  The Gaze method tends to lead to the least motion sickness.  Smart decoupling is still a Work In Progress.")]
        public DirectionalMethod directionMethod = DirectionalMethod.Gaze;
        [Tooltip("The degree threshold that all tracked objects (controllers, headset) must be within to change direction when using the Smart Decoupling Direction Method.")]
        public float smartDecoupleThreshold = 30f;
        // The cap before we stop adding the delta to the movement list. This will help regulate speed.
        [Tooltip("The maximum amount of movement required to register in the virtual world.  Decreasing this will increase acceleration, and vice versa.")]
        public float sensitivity = 0.02f;

        private Transform playArea;
        private GameObject controllerLeftHand;
        private GameObject controllerRightHand;
        private Transform headset;
        private bool leftSubscribed;
        private bool rightSubscribed;
        private bool previousLeftControllerState;
        private bool previousRightControllerState;
        private VRTK_ControllerEvents.ButtonAlias previousEngageButton;
        private VRTK_BodyPhysics bodyPhysics;
        private bool currentlyFalling;

        // The maximum number of updates we should hold to process movements. The higher the number, the slower the acceleration/deceleration & vice versa.
        private int averagePeriod;
        // Which tracked objects to use to determine amount of movement.
        private List<Transform> trackedObjects;
        // List of all the update's movements over the average period.
        private Dictionary<Transform, List<float>> movementList;
        private Dictionary<Transform, float> previousYPositions;
        // Used to determine the direction when using a decoupling method.
        private Vector3 initalGaze;
        // The current move speed of the player. If Move In Place is not active, it will be set to 0.00f.
        private float currentSpeed;
        // The current direction the player is moving. If Move In Place is not active, it will be set to Vector.zero.
        private Vector3 direction;
        // True if Move In Place is currently engaged.
        private bool active;

        /// <summary>
        /// Set the control options and modify the trackables to match.
        /// </summary>
        /// <param name="givenControlOptions">The control options to set the current control options to.</param>
        public void SetControlOptions(ControlOptions givenControlOptions)
        {
            controlOptions = givenControlOptions;
            trackedObjects.Clear();

            if (controllerLeftHand && controllerRightHand && (controlOptions.Equals(ControlOptions.HeadsetAndControllers) || controlOptions.Equals(ControlOptions.ControllersOnly)))
            {
                trackedObjects.Add(VRTK_DeviceFinder.GetActualController(controllerLeftHand).transform);
                trackedObjects.Add(VRTK_DeviceFinder.GetActualController(controllerRightHand).transform);
            }

            if (headset && (controlOptions.Equals(ControlOptions.HeadsetAndControllers) || controlOptions.Equals(ControlOptions.HeadsetOnly)))
            {
                trackedObjects.Add(headset.transform);
            }
        }

        /// <summary>
        /// The GetMovementDirection method will return the direction the player is moving.
        /// </summary>
        /// <returns>Returns a vector representing the player's current movement direction.</returns>
        public Vector3 GetMovementDirection()
        {
            return direction;
        }

        /// <summary>
        /// The GetSpeed method will return the current speed the player is moving at.
        /// </summary>
        /// <returns>Returns a float representing the player's current movement speed.</returns>
        public float GetSpeed()
        {
            return currentSpeed;
        }

        protected virtual void OnEnable()
        {
            trackedObjects = new List<Transform>();
            movementList = new Dictionary<Transform, List<float>>();
            previousYPositions = new Dictionary<Transform, float>();
            initalGaze = Vector3.zero;
            direction = Vector3.zero;
            averagePeriod = 60;
            currentSpeed = 0f;
            active = false;
            previousEngageButton = engageButton;

            bodyPhysics = GetComponent<VRTK_BodyPhysics>();
            controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();

            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed);

            headset = VRTK_DeviceFinder.HeadsetTransform();

            SetControlOptions(controlOptions);

            playArea = VRTK_DeviceFinder.PlayAreaTransform();

            // Initialize the lists.
            foreach (Transform trackedObj in trackedObjects)
            {
                movementList.Add(trackedObj, new List<float>());
                previousYPositions.Add(trackedObj, trackedObj.transform.localPosition.y);
            }
            if (!playArea)
            {
                Debug.LogError("No play area could be found. Have you selected a valid Boundaries SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Boundaries SDK from the dropdown.");
            }
        }

        protected virtual void OnDisable()
        {
            bodyPhysics = null;
            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed, true);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed, true);

            controllerLeftHand = null;
            controllerRightHand = null;
            headset = null;
            playArea = null;
        }

        protected virtual void Update()
        {
            CheckControllerState(controllerLeftHand, leftController, ref leftSubscribed, ref previousLeftControllerState);
            CheckControllerState(controllerRightHand, rightController, ref leftSubscribed, ref previousRightControllerState);
            previousEngageButton = engageButton;
        }

        protected virtual void FixedUpdate()
        {
            HandleFalling();
            // If Move In Place is currently engaged.
            if (active && !currentlyFalling)
            {
                // Initialize the list average.
                float listAverage = 0;

                foreach (Transform trackedObj in trackedObjects)
                {
                    // Get the amount of Y movement that's occured since the last update.
                    float deltaYPostion = Mathf.Abs(previousYPositions[trackedObj] - trackedObj.transform.localPosition.y);

                    // Convenience code.
                    List<float> trackedObjList = movementList[trackedObj];

                    // Cap off the speed.
                    if (deltaYPostion > sensitivity)
                    {
                        trackedObjList.Add(sensitivity);
                    }
                    else
                    {
                        trackedObjList.Add(deltaYPostion);
                    }

                    // Keep our tracking list at m_averagePeriod number of elements.
                    if (trackedObjList.Count > averagePeriod)
                    {
                        trackedObjList.RemoveAt(0);
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
                if (directionMethod == DirectionalMethod.SmartDecoupling || directionMethod == DirectionalMethod.DumbDecoupling)
                {
                    // If we haven't set an inital gaze yet, set it now.
                    // If we're doing dumb decoupling, this is what we'll be sticking with.
                    if (initalGaze.Equals(Vector3.zero))
                    {
                        initalGaze = new Vector3(headset.forward.x, 0, headset.forward.z);
                    }

                    // If we're doing smart decoupling, check to see if we want to reset our distance.
                    if (directionMethod == DirectionalMethod.SmartDecoupling)
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
                // if we're doing controller rotation movement
                else if (directionMethod.Equals(DirectionalMethod.ControllerRotation))
                {
                    direction = DetermineAverageControllerRotation() * Vector3.forward;
                }
                // Otherwise if we're just doing Gaze movement, always set the direction to where we're looking.
                else if (directionMethod.Equals(DirectionalMethod.Gaze))
                {
                    direction = (new Vector3(headset.forward.x, 0, headset.forward.z));
                }

                // Update our current speed.
                currentSpeed = speed;
            }
            else if (currentSpeed > 0f)
            {
                currentSpeed -= (currentlyFalling ? fallingDeceleration : deceleration);
            }
            else
            {
                currentSpeed = 0f;
                direction = Vector3.zero;
            }

            foreach (Transform trackedObj in trackedObjects)
            {
                // Get delta postions and rotations
                previousYPositions[trackedObj] = trackedObj.transform.localPosition.y;
            }

            Vector3 movement = (direction * currentSpeed) * Time.fixedDeltaTime;
            if (playArea)
            {
                playArea.position = new Vector3(movement.x + playArea.position.x, playArea.position.y, movement.z + playArea.position.z);
            }
        }

        protected virtual void CheckControllerState(GameObject controller, bool controllerState, ref bool subscribedState, ref bool previousState)
        {
            if (controllerState != previousState || engageButton != previousEngageButton)
            {
                SetControllerListeners(controller, controllerState, ref subscribedState);
            }
            previousState = controllerState;
        }

        protected virtual void HandleFalling()
        {
            if (bodyPhysics && bodyPhysics.IsFalling())
            {
                currentlyFalling = true;
            }

            if (bodyPhysics && !bodyPhysics.IsFalling() && currentlyFalling)
            {
                currentlyFalling = false;
                currentSpeed = 0f;
            }
        }

        private void EngageButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            active = true;
        }

        private void EngageButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            // If the button is released, clear all the lists.
            foreach (Transform obj in trackedObjects)
            {
                movementList[obj].Clear();
            }
            initalGaze = Vector3.zero;

            active = false;
        }

        private Quaternion DetermineAverageControllerRotation()
        {
            // Build the average rotation of the controller(s)
            Quaternion newRotation;

            // Both controllers are present
            if (controllerLeftHand != null && controllerRightHand != null)
            {
                newRotation = AverageRotation(controllerLeftHand.transform.rotation, controllerRightHand.transform.rotation);
            }
            // Left controller only
            else if (controllerRightHand != null && controllerRightHand == null)
            {
                newRotation = controllerLeftHand.transform.rotation;
            }
            // Right controller only
            else if (controllerRightHand != null && controllerLeftHand == null)
            {
                newRotation = controllerRightHand.transform.rotation;
            }
            // No controllers!
            else
            {
                newRotation = Quaternion.identity;
            }

            return newRotation;
        }

        // Returns the average of two Quaternions
        private Quaternion AverageRotation(Quaternion rot1, Quaternion rot2)
        {
            return Quaternion.Slerp(rot1, rot2, 0.5f);
        }

        // Returns a Vector3 with only the X and Z components (Y is 0'd)
        private static Vector3 Vector3XZOnly(Vector3 vec)
        {
            return new Vector3(vec.x, 0f, vec.z);
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
            if (controllerEvent)
            {
                //If engage button has changed, then unsubscribe the previous engage button from the events
                if (engageButton != previousEngageButton && subscribed)
                {
                    controllerEvent.UnsubscribeToButtonAliasEvent(previousEngageButton, true, EngageButtonPressed);
                    controllerEvent.UnsubscribeToButtonAliasEvent(previousEngageButton, false, EngageButtonReleased);
                    subscribed = false;
                }

                if (toggle && !subscribed)
                {
                    controllerEvent.SubscribeToButtonAliasEvent(engageButton, true, EngageButtonPressed);
                    controllerEvent.SubscribeToButtonAliasEvent(engageButton, false, EngageButtonReleased);
                    subscribed = true;
                }
                else if (!toggle && subscribed)
                {
                    controllerEvent.UnsubscribeToButtonAliasEvent(engageButton, true, EngageButtonPressed);
                    controllerEvent.UnsubscribeToButtonAliasEvent(engageButton, false, EngageButtonReleased);
                    subscribed = false;
                }
            }
        }
    }
}