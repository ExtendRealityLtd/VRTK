// Move In Place|Locomotion|20110
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Moves the SDK Camera Rig based on the motion of the headset and/or the controllers. Attempts to recreate the concept of physically walking on the spot to create scene movement.
    /// </summary>
    /// <remarks>
    ///   > This locomotion method is based on Immersive Movement, originally created by Highsight. Thanks to KJack (author of Arm Swinger) for additional work.
    ///
    /// **Optional Components:**
    ///  * `VRTK_BodyPhysics` - A Body Physics script to help determine potential collisions in the moving direction and prevent collision tunnelling.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_MoveInPlace` script on any active scene GameObject.
    ///
    /// **Script Dependencies:**
    ///  * The Controller Events script on the controller Script Alias to determine when the engage button is pressed.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/042_CameraRig_MoveInPlace` demonstrates how the user can move and traverse colliders by either swinging the controllers in a walking fashion or by running on the spot utilisng the head bob for movement.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_MoveInPlace")]
    public class VRTK_MoveInPlace : MonoBehaviour
    {
        /// <summary>
        /// Valid control options
        /// </summary>
        public enum ControlOptions
        {
            /// <summary>
            /// Track both headset and controllers for movement calculations.
            /// </summary>
            HeadsetAndControllers,
            /// <summary>
            /// Track only the controllers for movement calculations.
            /// </summary>
            ControllersOnly,
            /// <summary>
            /// Track only headset for movement caluclations.
            /// </summary>
            HeadsetOnly,
        }

        /// <summary>
        /// Options for which method is used to determine direction while moving.
        /// </summary>
        public enum DirectionalMethod
        {
            /// <summary>
            /// Will always move in the direction they are currently looking.
            /// </summary>
            Gaze,
            /// <summary>
            /// Will move in the direction that the controllers are pointing (averaged).
            /// </summary>
            ControllerRotation,
            /// <summary>
            /// Will move in the direction they were first looking when they engaged Move In Place.
            /// </summary>
            DumbDecoupling,
            /// <summary>
            /// Will move in the direction they are looking only if their headset point the same direction as their controllers.
            /// </summary>
            SmartDecoupling,
            /// <summary>
            /// Will move in the direction that the controller with the engage button pressed is pointing.
            /// </summary>
            EngageControllerRotationOnly,
            /// <summary>
            /// Will move in the direction that the left controller is pointing.
            /// </summary>
            LeftControllerRotationOnly,
            /// <summary>
            /// Will move in the direction that the right controller is pointing.
            /// </summary>
            RightControllerRotationOnly
        }

        [Header("Control Settings")]

        [Tooltip("If this is checked then the left controller engage button will be enabled to move the play area.")]
        public bool leftController = true;
        [Tooltip("If this is checked then the right controller engage button will be enabled to move the play area.")]
        public bool rightController = true;
        [Tooltip("The button to press to activate the movement.")]
        public VRTK_ControllerEvents.ButtonAlias engageButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;
        [Tooltip("The device to determine the movement paramters from.")]
        public ControlOptions controlOptions = ControlOptions.HeadsetAndControllers;
        [Tooltip("The method in which to determine the direction of forward movement.")]
        public DirectionalMethod directionMethod = DirectionalMethod.Gaze;

        [Header("Speed Settings")]

        [Tooltip("The speed in which to move the play area.")]
        public float speedScale = 1;
        [Tooltip("The maximun speed in game units. (If 0 or less, max speed is uncapped)")]
        public float maxSpeed = 4;
        [Tooltip("The speed in which the play area slows down to a complete stop when the engage button is released. This deceleration effect can ease any motion sickness that may be suffered.")]
        public float deceleration = 0.1f;
        [Tooltip("The speed in which the play area slows down to a complete stop when falling is occuring.")]
        public float fallingDeceleration = 0.01f;

        [Header("Advanced Settings")]

        [Tooltip("The degree threshold that all tracked objects (controllers, headset) must be within to change direction when using the Smart Decoupling Direction Method.")]
        public float smartDecoupleThreshold = 30f;
        // The cap before we stop adding the delta to the movement list. This will help regulate speed.
        [Tooltip("The maximum amount of movement required to register in the virtual world.  Decreasing this will increase acceleration, and vice versa.")]
        public float sensitivity = 0.02f;

        [Header("Custom Settings")]

        [Tooltip("An optional Body Physics script to check for potential collisions in the moving direction. If any potential collision is found then the move will not take place. This can help reduce collision tunnelling.")]
        public VRTK_BodyPhysics bodyPhysics;

        protected Transform playArea;
        protected GameObject controllerLeftHand;
        protected GameObject controllerRightHand;
        protected VRTK_ControllerReference engagedController;
        protected Transform headset;
        protected bool leftSubscribed;
        protected bool rightSubscribed;
        protected bool previousLeftControllerState;
        protected bool previousRightControllerState;
        protected VRTK_ControllerEvents.ButtonAlias previousEngageButton;
        protected bool currentlyFalling;

        protected int averagePeriod;
        protected List<Transform> trackedObjects = new List<Transform>();
        protected Dictionary<Transform, List<float>> movementList = new Dictionary<Transform, List<float>>();
        protected Dictionary<Transform, float> previousYPositions = new Dictionary<Transform, float>();
        protected Vector3 initialGaze;
        protected float currentSpeed;
        protected Vector3 currentDirection;
        protected Vector3 previousDirection;
        protected bool movementEngaged;

        /// <summary>
        /// Set the control options and modify the trackables to match.
        /// </summary>
        /// <param name="givenControlOptions">The control options to set the current control options to.</param>
        public virtual void SetControlOptions(ControlOptions givenControlOptions)
        {
            controlOptions = givenControlOptions;
            trackedObjects.Clear();

            if (controllerLeftHand != null && controllerRightHand != null && (controlOptions == ControlOptions.HeadsetAndControllers || controlOptions == ControlOptions.ControllersOnly))
            {
                VRTK_SharedMethods.AddListValue(trackedObjects, VRTK_DeviceFinder.GetActualController(controllerLeftHand).transform, true);
                VRTK_SharedMethods.AddListValue(trackedObjects, VRTK_DeviceFinder.GetActualController(controllerRightHand).transform, true);
            }

            if (headset != null && (controlOptions == ControlOptions.HeadsetAndControllers || controlOptions == ControlOptions.HeadsetOnly))
            {
                VRTK_SharedMethods.AddListValue(trackedObjects, headset.transform, true);
            }
        }

        /// <summary>
        /// The GetMovementDirection method will return the direction the play area is currently moving in.
        /// </summary>
        /// <returns>Returns a Vector3 representing the current movement direction.</returns>
        public virtual Vector3 GetMovementDirection()
        {
            return currentDirection;
        }

        /// <summary>
        /// The GetSpeed method will return the current speed the play area is moving at.
        /// </summary>
        /// <returns>Returns a float representing the current movement speed.</returns>
        public virtual float GetSpeed()
        {
            return currentSpeed;
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void OnEnable()
        {
            trackedObjects.Clear();
            movementList.Clear();
            previousYPositions.Clear();
            initialGaze = Vector3.zero;
            currentDirection = Vector3.zero;
            previousDirection = Vector3.zero;
            averagePeriod = 60;
            currentSpeed = 0f;
            movementEngaged = false;
            previousEngageButton = engageButton;

            bodyPhysics = (bodyPhysics != null ? bodyPhysics : FindObjectOfType<VRTK_BodyPhysics>());
            controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
            controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();

            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed);

            headset = VRTK_DeviceFinder.HeadsetTransform();

            SetControlOptions(controlOptions);

            playArea = VRTK_DeviceFinder.PlayAreaTransform();

            // Initialize the lists.
            for (int i = 0; i < trackedObjects.Count; i++)
            {
                Transform trackedObj = trackedObjects[i];
                VRTK_SharedMethods.AddDictionaryValue(movementList, trackedObj, new List<float>(), true);
                VRTK_SharedMethods.AddDictionaryValue(previousYPositions, trackedObj, trackedObj.transform.localPosition.y, true);
            }
            if (playArea == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
            }
        }

        protected virtual void OnDisable()
        {
            SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed, true);
            SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed, true);

            controllerLeftHand = null;
            controllerRightHand = null;
            headset = null;
            playArea = null;
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void Update()
        {
            CheckControllerState(controllerLeftHand, leftController, ref leftSubscribed, ref previousLeftControllerState);
            CheckControllerState(controllerRightHand, rightController, ref rightSubscribed, ref previousRightControllerState);
            previousEngageButton = engageButton;
        }

        protected virtual void FixedUpdate()
        {
            HandleFalling();
            // If Move In Place is currently engaged.
            if (MovementActivated() && !currentlyFalling)
            {
                // Initialize the list average.
                float speed = Mathf.Clamp(((speedScale * 350) * (CalculateListAverage() / trackedObjects.Count)), 0f, maxSpeed);
                previousDirection = currentDirection;
                currentDirection = SetDirection();
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
                currentDirection = Vector3.zero;
                previousDirection = Vector3.zero;
            }

            SetDeltaTransformData();
            MovePlayArea(currentDirection, currentSpeed);
        }

        protected virtual bool MovementActivated()
        {
            return (movementEngaged || engageButton == VRTK_ControllerEvents.ButtonAlias.Undefined);
        }

        protected virtual void CheckControllerState(GameObject controller, bool controllerState, ref bool subscribedState, ref bool previousState)
        {
            if (controllerState != previousState || engageButton != previousEngageButton)
            {
                SetControllerListeners(controller, controllerState, ref subscribedState);
            }
            previousState = controllerState;
        }

        protected virtual float CalculateListAverage()
        {
            float listAverage = 0;

            for (int i = 0; i < trackedObjects.Count; i++)
            {
                Transform trackedObj = trackedObjects[i];
                // Get the amount of Y movement that's occured since the last update.
                float previousYPosition = VRTK_SharedMethods.GetDictionaryValue(previousYPositions, trackedObj);
                float deltaYPostion = Mathf.Abs(previousYPosition - trackedObj.transform.localPosition.y);

                // Convenience code.
                List<float> trackedObjList = VRTK_SharedMethods.GetDictionaryValue(movementList, trackedObj, new List<float>(), true);

                // Cap off the speed.
                if (deltaYPostion > sensitivity)
                {
                    VRTK_SharedMethods.AddListValue(trackedObjList, sensitivity);
                }
                else
                {
                    VRTK_SharedMethods.AddListValue(trackedObjList, deltaYPostion);
                }

                // Keep our tracking list at m_averagePeriod number of elements.
                if (trackedObjList.Count > averagePeriod)
                {
                    trackedObjList.RemoveAt(0);
                }

                // Average out the current tracker's list.
                float sum = 0;
                for (int j = 0; j < trackedObjList.Count; j++)
                {
                    float diffrences = trackedObjList[j];
                    sum += diffrences;
                }
                float avg = sum / averagePeriod;

                // Add the average to the the list average.
                listAverage += avg;
            }

            return listAverage;
        }

        protected virtual Vector3 SetDirection()
        {
            switch (directionMethod)
            {
                case DirectionalMethod.SmartDecoupling:
                case DirectionalMethod.DumbDecoupling:
                    return CalculateCouplingDirection();
                case DirectionalMethod.ControllerRotation:
                    return CalculateControllerRotationDirection(DetermineAverageControllerRotation() * Vector3.forward);
                case DirectionalMethod.LeftControllerRotationOnly:
                    return CalculateControllerRotationDirection((controllerLeftHand != null ? controllerLeftHand.transform.rotation : Quaternion.identity) * Vector3.forward);
                case DirectionalMethod.RightControllerRotationOnly:
                    return CalculateControllerRotationDirection((controllerRightHand != null ? controllerRightHand.transform.rotation : Quaternion.identity) * Vector3.forward);
                case DirectionalMethod.EngageControllerRotationOnly:
                    return CalculateControllerRotationDirection((engagedController != null ? engagedController.scriptAlias.transform.rotation : Quaternion.identity) * Vector3.forward);
                case DirectionalMethod.Gaze:
                    return new Vector3(headset.forward.x, 0, headset.forward.z);
            }

            return Vector2.zero;
        }

        protected virtual Vector3 CalculateCouplingDirection()
        {
            // If we haven't set an inital gaze yet, set it now.
            // If we're doing dumb decoupling, this is what we'll be sticking with.
            if (initialGaze == Vector3.zero)
            {
                initialGaze = new Vector3(headset.forward.x, 0, headset.forward.z);
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
                    initialGaze = new Vector3(headset.forward.x, 0, headset.forward.z);
                }
            }
            return initialGaze;
        }

        protected virtual Vector3 CalculateControllerRotationDirection(Vector3 calculatedControllerDirection)
        {
            return (Vector3.Angle(previousDirection, calculatedControllerDirection) <= 90f ? calculatedControllerDirection : previousDirection);
        }

        protected virtual void SetDeltaTransformData()
        {
            for (int i = 0; i < trackedObjects.Count; i++)
            {
                Transform trackedObj = trackedObjects[i];
                // Get delta postions and rotations
                VRTK_SharedMethods.AddDictionaryValue(previousYPositions, trackedObj, trackedObj.transform.localPosition.y, true);
            }
        }

        protected virtual void MovePlayArea(Vector3 moveDirection, float moveSpeed)
        {
            Vector3 movement = (moveDirection * moveSpeed) * Time.fixedDeltaTime;
            Vector3 finalPosition = new Vector3(movement.x + playArea.position.x, playArea.position.y, movement.z + playArea.position.z);
            if (playArea != null && CanMove(bodyPhysics, playArea.position, finalPosition))
            {
                playArea.position = finalPosition;
            }
        }

        protected virtual bool CanMove(VRTK_BodyPhysics givenBodyPhysics, Vector3 currentPosition, Vector3 proposedPosition)
        {
            if (givenBodyPhysics == null)
            {
                return true;
            }

            Vector3 proposedDirection = (proposedPosition - currentPosition).normalized;
            float distance = Vector3.Distance(currentPosition, proposedPosition);
            return !givenBodyPhysics.SweepCollision(proposedDirection, distance);
        }

        protected virtual void HandleFalling()
        {
            if (bodyPhysics != null && bodyPhysics.IsFalling())
            {
                currentlyFalling = true;
            }

            if (bodyPhysics != null && !bodyPhysics.IsFalling() && currentlyFalling)
            {
                currentlyFalling = false;
                currentSpeed = 0f;
            }
        }

        protected virtual void EngageButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            engagedController = e.controllerReference;
            movementEngaged = true;
        }

        protected virtual void EngageButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            // If the button is released, clear all the lists.
            for (int i = 0; i < trackedObjects.Count; i++)
            {
                Transform trackedObj = trackedObjects[i];
                VRTK_SharedMethods.GetDictionaryValue(movementList, trackedObj, new List<float>()).Clear();
            }
            initialGaze = Vector3.zero;

            movementEngaged = false;
            engagedController = null;
        }

        protected virtual Quaternion DetermineAverageControllerRotation()
        {
            // Build the average rotation of the controller(s)
            Quaternion newRotation;

            // Both controllers are present
            if (controllerLeftHand != null && controllerRightHand != null)
            {
                newRotation = AverageRotation(controllerLeftHand.transform.rotation, controllerRightHand.transform.rotation);
            }
            // Left controller only
            else if (controllerLeftHand != null && controllerRightHand == null)
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
        protected virtual Quaternion AverageRotation(Quaternion rot1, Quaternion rot2)
        {
            return Quaternion.Slerp(rot1, rot2, 0.5f);
        }

        protected virtual void SetControllerListeners(GameObject controller, bool controllerState, ref bool subscribedState, bool forceDisabled = false)
        {
            if (controller != null)
            {
                bool toggleState = (forceDisabled ? false : controllerState);
                ToggleControllerListeners(controller, toggleState, ref subscribedState);
            }
        }

        protected virtual void ToggleControllerListeners(GameObject controller, bool toggle, ref bool subscribed)
        {
            VRTK_ControllerEvents controllerEvents = controller.GetComponentInChildren<VRTK_ControllerEvents>();
            if (controllerEvents != null)
            {
                //If engage button has changed, then unsubscribe the previous engage button from the events
                if (engageButton != previousEngageButton && subscribed)
                {
                    controllerEvents.UnsubscribeToButtonAliasEvent(previousEngageButton, true, EngageButtonPressed);
                    controllerEvents.UnsubscribeToButtonAliasEvent(previousEngageButton, false, EngageButtonReleased);
                    subscribed = false;
                }

                if (toggle && !subscribed)
                {
                    controllerEvents.SubscribeToButtonAliasEvent(engageButton, true, EngageButtonPressed);
                    controllerEvents.SubscribeToButtonAliasEvent(engageButton, false, EngageButtonReleased);
                    subscribed = true;
                }
                else if (!toggle && subscribed)
                {
                    controllerEvents.UnsubscribeToButtonAliasEvent(engageButton, true, EngageButtonPressed);
                    controllerEvents.UnsubscribeToButtonAliasEvent(engageButton, false, EngageButtonReleased);
                    subscribed = false;
                }
            }
        }
    }
}