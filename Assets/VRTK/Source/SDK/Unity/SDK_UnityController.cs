// Unity Controller|SDK_Unity|003
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Unity Controller SDK script provides a bridge  to the base Unity input device support.
    /// </summary>
    [SDK_Description(typeof(SDK_UnitySystem))]
    [SDK_Description(typeof(SDK_UnitySystem), 1)]
    [SDK_Description(typeof(SDK_UnitySystem), 2)]
    [SDK_Description(typeof(SDK_UnitySystem), 3)]
    [SDK_Description(typeof(SDK_UnitySystem), 4)]
    [SDK_Description(typeof(SDK_UnitySystem), 5)]
    public class SDK_UnityController : SDK_BaseController
    {
        protected VRTK_TrackedController cachedLeftController;
        protected VRTK_TrackedController cachedRightController;
        protected SDK_UnityControllerTracker cachedLeftTracker;
        protected SDK_UnityControllerTracker cachedRightTracker;
        protected VRTK_VelocityEstimator cachedLeftVelocityEstimator;
        protected VRTK_VelocityEstimator cachedRightVelocityEstimator;
        protected Vector2 buttonPressThreshold = new Vector2(0.2f, 0.5f);
        protected Dictionary<ButtonTypes, bool> rightAxisButtonPressState = new Dictionary<ButtonTypes, bool>()
        {
            { ButtonTypes.Trigger, false },
            { ButtonTypes.Grip, false },
        };

        protected Dictionary<ButtonTypes, bool> leftAxisButtonPressState = new Dictionary<ButtonTypes, bool>()
        {
            { ButtonTypes.Trigger, false },
            { ButtonTypes.Grip, false },
        };

        protected List<string> validRightHands = new List<string>()
        {
            "OpenVR Controller - Right",
            "OpenVR Controller(Vive. Controller MV) - Right",
            "OpenVR Controller(VIVE Controller Pro MV) - Right",
            "Oculus Touch - Right",
            "Oculus Remote"
        };
        protected List<string> validLeftHands = new List<string>()
        {
            "OpenVR Controller - Left",
            "OpenVR Controller(Vive. Controller MV) - Left",
            "OpenVR Controller(VIVE Controller Pro MV) - Left",
            "Oculus Touch - Left"
        };

        /*
        Axis Codes
            Left Trackpad Horizontal 1
            Left Trackpad Vertical 2

            Right Trackpad Horizontal 4
            Right Trackpad Vertical 5

            Left Trigger 9
            Right Trigger 10

            Left Grip 11
            Right Grip 12

            Left Trigger NearTouch 13
            Right Trigger NearTouch 14

            Left Trackpad NearTouch 15
            Right Trackpad NearTouch 16
        */

        protected int[] rightControllerTouchCodes = new int[] { 15, 17, 10, 11 };
        protected int[] rightControllerPressCodes = new int[] { 9, 1, 0, 7 };
        protected int[] rightOculusRemotePressCodes = new int[] { 9, 0, 1, 7 };

        protected int[] leftControllerTouchCodes = new int[] { 14, 16, 12, 13 };
        protected int[] leftControllerPressCodes = new int[] { 8, 3, 2, 7 };

        protected ControllerType cachedControllerType = ControllerType.Custom;

        protected Dictionary<ButtonTypes, KeyCode?> rightControllerTouchKeyCodes = new Dictionary<ButtonTypes, KeyCode?>()
        {
            { ButtonTypes.Trigger, KeyCode.JoystickButton15 },
            { ButtonTypes.TriggerHairline, null },
            { ButtonTypes.Grip, null },
            { ButtonTypes.GripHairline, null },
            { ButtonTypes.Touchpad, KeyCode.JoystickButton17 },
            { ButtonTypes.ButtonOne, KeyCode.JoystickButton10 },
            { ButtonTypes.ButtonTwo, KeyCode.JoystickButton11 },
            { ButtonTypes.StartMenu, null }
        };

        protected Dictionary<ButtonTypes, KeyCode?> rightControllerPressKeyCodes = new Dictionary<ButtonTypes, KeyCode?>()
        {
            { ButtonTypes.Trigger, null },
            { ButtonTypes.TriggerHairline, null },
            { ButtonTypes.Grip, null },
            { ButtonTypes.GripHairline, null },
            { ButtonTypes.Touchpad, KeyCode.JoystickButton9 },
            { ButtonTypes.ButtonOne, KeyCode.JoystickButton1 },
            { ButtonTypes.ButtonTwo, KeyCode.JoystickButton0 },
            { ButtonTypes.StartMenu, KeyCode.JoystickButton7 }
        };

        protected Dictionary<ButtonTypes, KeyCode?> leftControllerTouchKeyCodes = new Dictionary<ButtonTypes, KeyCode?>()
        {
            { ButtonTypes.Trigger, KeyCode.JoystickButton14 },
            { ButtonTypes.TriggerHairline, null },
            { ButtonTypes.Grip, null },
            { ButtonTypes.GripHairline, null },
            { ButtonTypes.Touchpad, KeyCode.JoystickButton16 },
            { ButtonTypes.ButtonOne, KeyCode.JoystickButton12 },
            { ButtonTypes.ButtonTwo, KeyCode.JoystickButton13 },
            { ButtonTypes.StartMenu, null }
        };

        protected Dictionary<ButtonTypes, KeyCode?> leftControllerPressKeyCodes = new Dictionary<ButtonTypes, KeyCode?>()
        {
            { ButtonTypes.Trigger, null },
            { ButtonTypes.TriggerHairline, null },
            { ButtonTypes.Grip, null },
            { ButtonTypes.GripHairline, null },
            { ButtonTypes.Touchpad, KeyCode.JoystickButton8 },
            { ButtonTypes.ButtonOne, KeyCode.JoystickButton3 },
            { ButtonTypes.ButtonTwo, KeyCode.JoystickButton2 },
            { ButtonTypes.StartMenu, KeyCode.JoystickButton7 }
        };

        private bool settingCaches = false;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
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
        /// The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to get type of.</param>
        /// <returns>The ControllerType based on the SDK and headset being used.</returns>
        public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)
        {
            SetTrackedControllerCaches();
            return cachedControllerType;
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/Fallback";
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
            switch (element)
            {
                case ControllerElements.AttachPoint:
                    return "AttachPoint";
            }
            return "";
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            VRTK_TrackedController trackedObject = GetTrackedObject(controller);
            return (trackedObject != null ? trackedObject.index : uint.MaxValue);
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                }

                if (cachedRightController != null && cachedRightController.index == index)
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
            return VRTK_SDK_Bridge.GetPlayArea();
        }

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <param name="parent">The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.</param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        [System.Obsolete("GenerateControllerPointerOrigin has been deprecated and will be removed in a future version of VRTK.")]
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
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_UnityCameraRig>("LeftHandAnchor", true);
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
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_UnityCameraRig>("RightHandAnchor", true);
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
        /// The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.
        /// </summary>
        /// <param name="hand">The hand to determine if the controller model will be ready for.</param>
        /// <returns>Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.</returns>
        public override bool WaitForControllerModel(ControllerHand hand)
        {
            return false;
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
                    model = controller.transform.Find("Model").gameObject;
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
            return null;
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
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            //Return true so it just always prevents doing a fallback routine.
            return true;
        }

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            return new SDK_ControllerHapticModifiers();
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                if (controllerReference.hand == ControllerHand.Left && cachedLeftVelocityEstimator != null)
                {
                    return cachedLeftVelocityEstimator.GetVelocityEstimate();
                }
                else if (controllerReference.hand == ControllerHand.Right && cachedRightVelocityEstimator != null)
                {
                    return cachedRightVelocityEstimator.GetVelocityEstimate();
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                if (controllerReference.hand == ControllerHand.Left && cachedLeftVelocityEstimator != null)
                {
                    return cachedLeftVelocityEstimator.GetAngularVelocityEstimate();
                }
                else if (controllerReference.hand == ControllerHand.Right && cachedRightVelocityEstimator != null)
                {
                    return cachedRightVelocityEstimator.GetAngularVelocityEstimate();
                }
            }
            return Vector3.zero;
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

            bool isRightController = (controllerReference.hand == ControllerHand.Right);

            if ((isRightController && cachedRightTracker == null) || (!isRightController && cachedLeftTracker == null))
            {
                return Vector2.zero;
            }

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return new Vector2(GetAxisValue((isRightController ? cachedRightTracker.triggerAxisName : cachedLeftTracker.triggerAxisName)), 0f);
                case ButtonTypes.Grip:
                    return new Vector2(GetAxisValue((isRightController ? cachedRightTracker.gripAxisName : cachedLeftTracker.gripAxisName)), 0f);
                case ButtonTypes.Touchpad:
                    return new Vector2(GetAxisValue((isRightController ? cachedRightTracker.touchpadHorizontalAxisName : cachedLeftTracker.touchpadHorizontalAxisName)), GetAxisValue((isRightController ? cachedRightTracker.touchpadVerticalAxisName : cachedLeftTracker.touchpadVerticalAxisName)));
            }
            return Vector2.zero;
        }

        /// <summary>
        /// The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the sense axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the sense axis on.</param>
        /// <returns>The current sense axis value.</returns>
        public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            return 0f;
        }

        /// <summary>
        /// The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.
        /// </summary>
        /// <param name="buttonType">The type of button to get the hairline delta for.</param>
        /// <param name="controllerReference">The reference to the controller to get the hairline delta for.</param>
        /// <returns>The delta between the button presses.</returns>
        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            return 0f;
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

            bool isRightController = (controllerReference.hand == ControllerHand.Right);

            KeyCode? touchButton = VRTK_SharedMethods.GetDictionaryValue((isRightController ? rightControllerTouchKeyCodes : leftControllerTouchKeyCodes), buttonType);
            KeyCode? pressButton = VRTK_SharedMethods.GetDictionaryValue((isRightController ? rightControllerPressKeyCodes : leftControllerPressKeyCodes), buttonType);

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(pressType, touchButton, pressButton);
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return (IsMouseAliasPress(isRightController, buttonType, pressType) || IsAxisButtonPress(controllerReference, buttonType, pressType));
                    }
                    break;
                case ButtonTypes.Grip:
                    return (IsMouseAliasPress(isRightController, buttonType, pressType) || IsAxisButtonPress(controllerReference, buttonType, pressType));
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(pressType, touchButton, pressButton);
                case ButtonTypes.ButtonOne:
                    return IsButtonPressed(pressType, touchButton, pressButton);
                case ButtonTypes.ButtonTwo:
                    return IsButtonPressed(pressType, touchButton, pressButton);
                case ButtonTypes.StartMenu:
                    return IsButtonPressed(pressType, touchButton, pressButton);
            }
            return false;
        }

        protected virtual bool IsMouseAliasPress(bool validController, ButtonTypes buttonType, ButtonPressTypes pressType)
        {
            if (validController)
            {
                switch (buttonType)
                {
                    case ButtonTypes.Trigger:
                        return MousePressType(pressType, 0);
                    case ButtonTypes.Grip:
                        return MousePressType(pressType, 1);
                }
            }
            return false;
        }

        protected virtual bool MousePressType(ButtonPressTypes pressType, int buttonIndex)
        {
            switch (pressType)
            {
                case ButtonPressTypes.Press:
                    return Input.GetMouseButton(buttonIndex);
                case ButtonPressTypes.PressDown:
                    return Input.GetMouseButtonDown(buttonIndex);
                case ButtonPressTypes.PressUp:
                    return Input.GetMouseButtonUp(buttonIndex);
            }
            return false;
        }

        protected virtual float GetAxisValue(string axisName)
        {
            try
            {
                return Input.GetAxis(axisName);
            }
            catch (ArgumentException)
            {
                //Don't do any logging here because it will spam the console
            }
            return 0f;
        }

        protected virtual bool IsAxisOnHandButtonPress(Dictionary<ButtonTypes, bool> axisHandState, ButtonTypes buttonType, ButtonPressTypes pressType, Vector2 axisValue)
        {
            bool previousAxisState = VRTK_SharedMethods.GetDictionaryValue(axisHandState, buttonType);
            if (pressType == ButtonPressTypes.PressDown && !previousAxisState)
            {
                bool currentAxisState = GetAxisPressState(previousAxisState, axisValue.x);
                VRTK_SharedMethods.AddDictionaryValue(axisHandState, buttonType, currentAxisState, true);
                return currentAxisState;
            }
            if (pressType == ButtonPressTypes.PressUp && previousAxisState)
            {
                bool currentAxisState = GetAxisPressState(previousAxisState, axisValue.x);
                VRTK_SharedMethods.AddDictionaryValue(axisHandState, buttonType, currentAxisState, true);
                return !currentAxisState;
            }
            return false;
        }

        protected virtual bool IsAxisButtonPress(VRTK_ControllerReference controllerReference, ButtonTypes buttonType, ButtonPressTypes pressType)
        {
            bool isRightController = (controllerReference.hand == ControllerHand.Right);
            Vector2 axisValue = GetButtonAxis(buttonType, controllerReference);
            return IsAxisOnHandButtonPress((isRightController ? rightAxisButtonPressState : leftAxisButtonPressState), buttonType, pressType, axisValue);
        }

        protected virtual bool GetAxisPressState(bool currentState, float axisValue)
        {
            if (currentState && axisValue <= buttonPressThreshold.x)
            {
                currentState = false;
            }
            else if (!currentState && axisValue >= buttonPressThreshold.y)
            {
                currentState = true;
            }
            return currentState;
        }

        protected virtual bool IsButtonPressed(ButtonPressTypes pressType, KeyCode? touchKey, KeyCode? pressKey)
        {
            switch (pressType)
            {
                case ButtonPressTypes.Touch:
                    return (touchKey != null && Input.GetKey((KeyCode)touchKey));
                case ButtonPressTypes.TouchDown:
                    return (touchKey != null && Input.GetKeyDown((KeyCode)touchKey));
                case ButtonPressTypes.TouchUp:
                    return (touchKey != null && Input.GetKeyUp((KeyCode)touchKey));
                case ButtonPressTypes.Press:
                    return (pressKey != null && Input.GetKey((KeyCode)pressKey));
                case ButtonPressTypes.PressDown:
                    return (pressKey != null && Input.GetKeyDown((KeyCode)pressKey));
                case ButtonPressTypes.PressUp:
                    return (pressKey != null && Input.GetKeyUp((KeyCode)pressKey));
            }
            return false;
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (settingCaches)
            {
                return;
            }

            settingCaches = true;

            if (forceRefresh)
            {
                cachedLeftController = null;
                cachedRightController = null;
            }

            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.loadedSetup != null)
            {
                if (cachedLeftController == null && sdkManager.loadedSetup.actualLeftController != null)
                {
                    cachedLeftController = sdkManager.loadedSetup.actualLeftController.GetComponent<VRTK_TrackedController>();
                    SetControllerIndex(ref cachedLeftController);
                    if (cachedLeftController != null)
                    {
                        cachedLeftTracker = cachedLeftController.GetComponent<SDK_UnityControllerTracker>();
                        cachedLeftVelocityEstimator = cachedLeftController.GetComponent<VRTK_VelocityEstimator>();
                        SetControllerButtons(ControllerHand.Left);
                    }
                }
                if (cachedRightController == null && sdkManager.loadedSetup.actualRightController != null)
                {
                    cachedRightController = sdkManager.loadedSetup.actualRightController.GetComponent<VRTK_TrackedController>();
                    SetControllerIndex(ref cachedRightController);
                    if (cachedRightController != null)
                    {
                        cachedRightTracker = cachedRightController.GetComponent<SDK_UnityControllerTracker>();
                        cachedRightVelocityEstimator = cachedRightController.GetComponent<VRTK_VelocityEstimator>();
                        SetControllerButtons(ControllerHand.Right);
                    }
                }
            }

            settingCaches = false;
        }

        protected virtual void SetControllerButtons(ControllerHand hand)
        {
            List<string> checkhands = (hand == ControllerHand.Right ? validRightHands : validLeftHands);

            bool joystickFound = false;
            int validJoystickIndex = 0;
            string[] availableJoysticks = Input.GetJoystickNames();
            for (int i = 0; i < availableJoysticks.Length; i++)
            {
                if (checkhands.Contains(availableJoysticks[i]))
                {
                    SetCachedControllerType(availableJoysticks[i]);
                    joystickFound = true;
                    validJoystickIndex = i + 1;
                }
            }

            //If the joystick isn't found then try and match on headset type
            if (!joystickFound)
            {
                switch (VRTK_DeviceFinder.GetHeadsetType())
                {
                    case SDK_BaseHeadset.HeadsetType.GoogleDaydream:
                        SetCachedControllerType("googledaydream");
                        joystickFound = true;
                        validJoystickIndex = 1;
                        break;
                }
            }

            if (joystickFound)
            {
                if (hand == ControllerHand.Right)
                {
                    var pressCodes = cachedControllerType == ControllerType.Oculus_OculusRemote ? rightOculusRemotePressCodes : rightControllerPressCodes;
                    SetControllerButtonValues(ref rightControllerTouchKeyCodes, ref rightControllerPressKeyCodes, validJoystickIndex, rightControllerTouchCodes, pressCodes);
                }
                else
                {
                    SetControllerButtonValues(ref leftControllerTouchKeyCodes, ref leftControllerPressKeyCodes, validJoystickIndex, leftControllerTouchCodes, leftControllerPressCodes);
                }
            }
            else if (availableJoysticks.Length > 0 && VRTK_ControllerReference.GetControllerReference(hand) != null && VRTK_ControllerReference.GetControllerReference(hand).actual.gameObject.activeInHierarchy)
            {
                VRTK_Logger.Warn("Failed setting controller buttons on [" + hand + "] due to no valid joystick type found in `GetJoyStickNames` -> " + string.Join(", ", availableJoysticks));
            }
        }

        protected virtual void SetCachedControllerType(string givenType)
        {
            givenType = givenType.ToLower();
            //try direct matching
            switch (givenType)
            {
                case "googledaydream":
                    cachedControllerType = ControllerType.Daydream_Controller;
                    return;
                case "oculus remote":
                    cachedControllerType = ControllerType.Oculus_OculusRemote;
                    return;
            }

            //fallback to fuzzy matching
            if (givenType.Contains("openvr controller"))
            {
                switch (VRTK_DeviceFinder.GetHeadsetType())
                {
                    case SDK_BaseHeadset.HeadsetType.HTCVive:
                        cachedControllerType = ControllerType.SteamVR_ViveWand;
                        break;
                    case SDK_BaseHeadset.HeadsetType.OculusRift:
                        cachedControllerType = ControllerType.SteamVR_OculusTouch;
                        break;
                }
            }
            else if (givenType.Contains("oculus touch"))
            {
                cachedControllerType = ControllerType.Oculus_OculusTouch;
            }
        }

        protected virtual void SetControllerButtonValues(ref Dictionary<ButtonTypes, KeyCode?> touchKeyCodes, ref Dictionary<ButtonTypes, KeyCode?> pressKeyCodes, int joystickIndex, int[] touchCodes, int[] pressCodes)
        {
            VRTK_SharedMethods.AddDictionaryValue(touchKeyCodes, ButtonTypes.Trigger, StringToKeyCode(joystickIndex, touchCodes[0]), true);
            VRTK_SharedMethods.AddDictionaryValue(touchKeyCodes, ButtonTypes.Touchpad, StringToKeyCode(joystickIndex, touchCodes[1]), true);
            VRTK_SharedMethods.AddDictionaryValue(touchKeyCodes, ButtonTypes.ButtonOne, StringToKeyCode(joystickIndex, touchCodes[2]), true);
            VRTK_SharedMethods.AddDictionaryValue(touchKeyCodes, ButtonTypes.ButtonTwo, StringToKeyCode(joystickIndex, touchCodes[3]), true);

            VRTK_SharedMethods.AddDictionaryValue(pressKeyCodes, ButtonTypes.Touchpad, StringToKeyCode(joystickIndex, pressCodes[0]), true);
            VRTK_SharedMethods.AddDictionaryValue(pressKeyCodes, ButtonTypes.ButtonOne, StringToKeyCode(joystickIndex, pressCodes[1]), true);
            VRTK_SharedMethods.AddDictionaryValue(pressKeyCodes, ButtonTypes.ButtonTwo, StringToKeyCode(joystickIndex, pressCodes[2]), true);
            VRTK_SharedMethods.AddDictionaryValue(pressKeyCodes, ButtonTypes.StartMenu, StringToKeyCode(joystickIndex, pressCodes[3]), true);
        }

        protected virtual KeyCode StringToKeyCode(int index, int code)
        {
            return (KeyCode)Enum.Parse(typeof(KeyCode), "Joystick" + index + "Button" + code);
        }

        protected virtual void SetControllerIndex(ref VRTK_TrackedController trackedController)
        {
            if (trackedController != null)
            {
                SDK_UnityControllerTracker tracker = trackedController.GetComponent<SDK_UnityControllerTracker>();
                if (tracker != null)
                {
                    trackedController.index = tracker.index;
                }
            }
        }

        protected virtual VRTK_TrackedController GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();
            VRTK_TrackedController trackedObject = null;

            if (IsControllerLeftHand(controller))
            {
                trackedObject = cachedLeftController;
            }
            else if (IsControllerRightHand(controller))
            {
                trackedObject = cachedRightController;
            }
            return trackedObject;
        }
    }
}