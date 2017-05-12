// Ximmerse Controller|SDK_Ximmerse|004
namespace VRTK
{
#if VRTK_DEFINE_SDK_XIMMERSE
    using UnityEngine;
    using System.Collections.Generic;
    using Ximmerse.InputSystem;
    using Ximmerse.VR;
#endif

    /// <summary>
    /// The Ximmerse Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_XimmerseSystem))]
    [SDK_Description(typeof(SDK_XimmerseSystem), 1)]
    public class SDK_XimmerseController
#if VRTK_DEFINE_SDK_XIMMERSE
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_XIMMERSE
        protected TrackedObject cachedLeftTrackedObject;
        protected TrackedObject cachedRightTrackedObject;

        protected Quaternion[] previousControllerRotations = new Quaternion[2];
        protected Quaternion[] currentControllerRotations = new Quaternion[2];

        protected Vector3[] previousControllerPositions = new Vector3[2];
        protected Vector3[] currentControllerPositions = new Vector3[2];

        protected bool[] previousHairTriggerState = new bool[2];
        protected bool[] currentHairTriggerState = new bool[2];

        protected bool[] previousHairGripState = new bool[2];
        protected bool[] currentHairGripState = new bool[2];

        protected float[] hairTriggerLimit = new float[2];
        protected float[] hairGripLimit = new float[2];

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
            if (controllerReference != null && controllerReference.IsValid())
            {
                uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                TrackedObject device = GetTrackedObject(controllerReference.actual);
                if (device != null)
                {
                    previousControllerRotations[index] = currentControllerRotations[index];
                    currentControllerRotations[index] = device.transform.rotation;

                    previousControllerPositions[index] = currentControllerPositions[index];
                    currentControllerPositions[index] = device.transform.position;
                }
                UpdateHairValues(index, GetButtonAxis(ButtonTypes.Trigger, controllerReference).x, GetButtonHairlineDelta(ButtonTypes.Trigger, controllerReference), ref previousHairTriggerState[index], ref currentHairTriggerState[index], ref hairTriggerLimit[index]);
                UpdateHairValues(index, GetButtonAxis(ButtonTypes.Grip, controllerReference).x, GetButtonHairlineDelta(ButtonTypes.Grip, controllerReference), ref previousHairGripState[index], ref currentHairGripState[index], ref hairGripLimit[index]);
            }
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/XimmerseCobra02";
        }

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            string suffix = (fullPath ? "" : "");
            string handName = (hand == ControllerHand.Left) ? "cobra02-L" : "cobra02-R";
            string path = handName + "/Dummy";
            switch (element)
            {
                case ControllerElements.AttachPoint:
                    return path + "/object_8";
                case ControllerElements.Trigger:
                    return path + "/object_2" + suffix;
                case ControllerElements.GripLeft:
                    return path + "/object_5" + suffix;
                case ControllerElements.GripRight:
                    return path + "/object_4" + suffix;
                case ControllerElements.Touchpad:
                    return path + "/Cylinder001" + suffix;
                case ControllerElements.ButtonOne:
                    return path + "/Cylinder002" + suffix;
                case ControllerElements.ButtonTwo:
                    return path + "/Cylinder003" + suffix;
                case ControllerElements.SystemMenu:
                    return path + "/Cylinder003" + suffix;
                case ControllerElements.Body:
                    return path + "/object_1";
            }
            return null;
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            TrackedObject trackedObject = GetTrackedObject(controller);
            if (trackedObject == null)
            {
                return uint.MaxValue;
            }
            else
            {
                switch (trackedObject.source)
                {
                    case ControllerType.LeftController:
                        return 0;
                    case ControllerType.RightController:
                        return 1;
                }
            }
            return uint.MaxValue;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns></returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject != null && (uint)cachedLeftTrackedObject.controllerInput.handle == index)
                {
                    return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                }

                if (cachedRightTrackedObject != null && (uint)cachedRightTrackedObject.controllerInput.handle == index)
                {
                    return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
                }
            }
            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            TrackedObject trackedObject = GetTrackedObject(controllerReference.actual);
            if (trackedObject != null)
            {
                return trackedObject.transform ? trackedObject.transform : trackedObject.transform.parent;
            }
            return null;
        }

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerLeftHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<VRContext>("TrackingSpace/LeftHandAnchor");
            }
            return controller;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerRightHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<VRContext>("TrackingSpace/RightHandAnchor");
            }
            return controller;
        }

        /// <summary>
        /// The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        /// <summary>
        /// The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        /// <summary>
        /// The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        /// <summary>
        /// The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given GameObject.
        /// </summary>
        /// <param name="controller">The GameObject to get the model alias for.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(ControllerHand hand)
        {
            GameObject model = GetSDKManagerControllerModelForHand(hand);
            if (model == null)
            {
                GameObject controller = null;
                switch (hand)
                {
                    case ControllerHand.Left:
                        controller = GetControllerLeftHand(true);
                        break;
                    case ControllerHand.Right:
                        controller = GetControllerRightHand(true);
                        break;
                }

                if (controller != null)
                {
                    model = controller;
                }
            }
            return model;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            MeshRenderer renderModel = controllerReference.actual.GetComponentInChildren<MeshRenderer>();
            return (renderModel != null ? renderModel.gameObject : null);
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                int convertedStrength = Mathf.RoundToInt(1000f * strength);
                ControllerInput input = null;
                switch (index)
                {
                    case 0:
                        input = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
                        break;
                    case 1:
                        input = ControllerInputManager.instance.GetControllerInput(ControllerType.RightController);
                        break;
                }
                input.StartVibration(convertedStrength, 0.5f);
            }
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            //Ximmerse doesn't support audio haptics so return false to do a fallback.
            return false;
        }

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            SDK_ControllerHapticModifiers modifiers = new SDK_ControllerHapticModifiers();
            modifiers.durationModifier = 0.8f;
            modifiers.intervalModifier = 1f;
            return modifiers;
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return Vector3.zero;
            }

            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            ControllerInput device = GetControllerInputByIndex(index);
            if (device == null)
            {
                return Vector3.zero;
            }

            Vector3 deltaMovement = currentControllerPositions[index] - previousControllerPositions[index];
            deltaMovement /= Time.deltaTime;
            return deltaMovement;
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return Vector3.zero;
            }

            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            Quaternion deltaRotation = currentControllerRotations[index] * Quaternion.Inverse(previousControllerRotations[index]);
            return new Vector3(Mathf.DeltaAngle(0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.z));
        }

        /// <summary>
        /// The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.
        /// </summary>
        /// <param name="currentAxisValues"></param>
        /// <param name="previousAxisValues"></param>
        /// <param name="compareFidelity"></param>
        /// <returns>Returns true if the touchpad is not currently being touched or moved.</returns>
        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            return (!isTouched || VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity));
        }

        /// <summary>
        /// The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the button axis on.</param>
        /// <returns>A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.</returns>
        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return Vector2.zero;
            }

            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            ControllerInput controllerInputDevice = GetControllerInputByIndex(index);
            TrackedObject trackedObjectDevice = GetTrackedObject(controllerReference.actual);

            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    if (controllerInputDevice != null)
                    {
                        Vector2 rawTouchpad = controllerInputDevice.GetTouchPos() - new Vector2(0.5f, 0.5f);
                        return new Vector2(rawTouchpad.x, -rawTouchpad.y);
                    }
                    break;
                case ButtonTypes.Trigger:
                    if (controllerInputDevice != null)
                    {
                        return new Vector2(controllerInputDevice.GetAxis(ControllerRawAxis.LeftTrigger), 0f);
                    }
                    break;
                case ButtonTypes.Grip:
                    if (trackedObjectDevice != null)
                    {
                        return new Vector2((trackedObjectDevice.controllerInput.GetButton(DaydreamButton.Grip) ? 1f : 0f), 0f);
                    }
                    break;
            }

            return Vector2.zero;
        }

        /// <summary>
        /// The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.
        /// </summary>
        /// <param name="buttonType">The type of button to get the hairline delta for.</param>
        /// <param name="controllerReference">The reference to the controller to get the hairline delta for.</param>
        /// <returns>The delta between the button presses.</returns>
        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            //TODO: This doesn't seem correct, surely this should be storing the previous button press value and getting the delta.
            return (VRTK_ControllerReference.IsValid(controllerReference) ? 0.1f : 0f);
        }

        /// <summary>
        /// The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the state of.</param>
        /// <param name="pressType">The button state to check for.</param>
        /// <param name="controllerReference">The reference to the controller to check the button state on.</param>
        /// <returns>Returns true if the given button is in the state of the given press type on the given controller reference.</returns>
        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return IsButtonPressed(index, pressType, ControllerRawButton.LeftTrigger);
                case ButtonTypes.TriggerHairline:
                    if (pressType == ButtonPressTypes.PressDown)
                    {
                        return (currentHairTriggerState[index] && !previousHairTriggerState[index]);
                    }
                    else if (pressType == ButtonPressTypes.PressUp)
                    {
                        return (!currentHairTriggerState[index] && previousHairTriggerState[index]);
                    }
                    break;
                case ButtonTypes.Grip:
                    return IsButtonPressed(index, pressType, ControllerRawButton.LeftShoulder) || IsButtonPressed(index, pressType, ControllerRawButton.RightShoulder);
                case ButtonTypes.GripHairline:
                    if (pressType == ButtonPressTypes.PressDown)
                    {
                        return (currentHairGripState[index] && !previousHairGripState[index]);
                    }
                    else if (pressType == ButtonPressTypes.PressUp)
                    {
                        return (!currentHairGripState[index] && previousHairGripState[index]);
                    }
                    break;
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(index, pressType, ControllerRawButton.LeftThumb);
                case ButtonTypes.ButtonOne:
                    return IsButtonPressed(index, pressType, ControllerRawButton.Back);
                case ButtonTypes.ButtonTwo:
                    return IsButtonPressed(index, pressType, ControllerRawButton.Start);
            }
            return false;
        }

        protected virtual void Awake()
        {
            SetTrackedControllerCaches(true);
        }

        protected virtual void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
        {
            SetTrackedControllerCaches(true);
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftTrackedObject = null;
                cachedRightTrackedObject = null;
            }

            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject == null && sdkManager.loadedSetup.actualLeftController)
                {
                    cachedLeftTrackedObject = sdkManager.loadedSetup.actualLeftController.GetComponent<TrackedObject>();
                }
                if (cachedRightTrackedObject == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightTrackedObject = sdkManager.loadedSetup.actualRightController.GetComponent<TrackedObject>();
                }
            }
        }

        protected virtual TrackedObject GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();
            TrackedObject trackedObject = null;

            if (IsControllerLeftHand(controller))
            {
                trackedObject = cachedLeftTrackedObject;
            }
            else if (IsControllerRightHand(controller))
            {
                trackedObject = cachedRightTrackedObject;
            }
            return trackedObject;
        }

        protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, ControllerRawButton button)
        {
            ControllerInput device = GetControllerInputByIndex(index);
            if (device == null)
            {
                return false;
            }

            switch (type)
            {
                case ButtonPressTypes.Press:
                    return device.GetButton(button);
                case ButtonPressTypes.PressDown:
                    return device.GetButtonDown(button);
                case ButtonPressTypes.PressUp:
                    return device.GetButtonUp(button);
                case ButtonPressTypes.Touch:
                    return device.GetButton(button);
                case ButtonPressTypes.TouchDown:
                    return device.GetButtonDown(button);
                case ButtonPressTypes.TouchUp:
                    return device.GetButtonUp(button);
            }

            return false;
        }

        protected virtual void UpdateHairValues(uint index, float axisValue, float hairDelta, ref bool previousState, ref bool currentState, ref float hairLimit)
        {
            previousState = currentState;
            float value = axisValue;
            if (currentState)
            {
                if (value < (hairLimit - hairDelta) || value <= 0f)
                {
                    currentState = false;
                }
            }
            else
            {
                if (value > (hairLimit + hairDelta) || value >= 1f)
                {
                    currentState = true;
                }
            }

            hairLimit = (currentState ? Mathf.Max(hairLimit, value) : Mathf.Min(hairLimit, value));
        }

        protected virtual ControllerInput GetControllerInputByIndex(uint index)
        {
            ControllerInput result = null;
            switch (index)
            {
                case 0:
                    result = ControllerInputManager.instance.GetControllerInput(ControllerType.LeftController);
                    break;
                case 1:
                    result = ControllerInputManager.instance.GetControllerInput(ControllerType.RightController);
                    break;
            }

            return result;
        }
#endif
    }
}