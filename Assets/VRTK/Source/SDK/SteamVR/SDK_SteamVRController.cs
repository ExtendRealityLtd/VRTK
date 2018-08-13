// SteamVR Controller|SDK_SteamVR|004
namespace VRTK
{
#if VRTK_DEFINE_SDK_STEAMVR
    using UnityEngine;
    using System.Collections.Generic;
    using System.Text;
    using Valve.VR;
    using System;
#if !VRTK_DEFINE_STEAMVR_PLUGIN_1_2_2_OR_NEWER
    using System;
    using System.Reflection;
#endif
#endif

    /// <summary>
    /// The SteamVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_SteamVRSystem))]
    public class SDK_SteamVRController
#if VRTK_DEFINE_SDK_STEAMVR
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_STEAMVR
        protected SteamVR_TrackedObject cachedLeftTrackedObject;
        protected SteamVR_TrackedObject cachedRightTrackedObject;
        protected Dictionary<GameObject, SteamVR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, SteamVR_TrackedObject>();
        protected Dictionary<uint, SteamVR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, SteamVR_TrackedObject>();
        protected Dictionary<EVRButtonId, bool> axisTouchStates = new Dictionary<EVRButtonId, bool>();
        protected Dictionary<EVRButtonId, float> axisTouchFidelity = new Dictionary<EVRButtonId, float>() { { EVRButtonId.k_EButton_SteamVR_Touchpad, 0f }, { EVRButtonId.k_EButton_Axis2, 0.25f } };
        protected ushort maxHapticVibration = 3999;

#if !VRTK_DEFINE_STEAMVR_PLUGIN_1_2_2_OR_NEWER
        /// <summary>
        /// This method is called just after unloading the VRTK_SDKSetup that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public override void OnAfterSetupUnload(VRTK_SDKSetup setup)
        {
            base.OnAfterSetupUnload(setup);

            SteamVR_ControllerManager controllerManager = setup.actualLeftController.transform.parent.GetComponent<SteamVR_ControllerManager>();
            FieldInfo connectedField = typeof(SteamVR_ControllerManager).GetField(
                "connected",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (connectedField == null)
            {
                return;
            }

            bool[] connected = (bool[])connectedField.GetValue(controllerManager);
            Array.Clear(connected, 0, connected.Length);
            connectedField.SetValue(controllerManager, connected);
        }
#endif

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
            uint checkIndex = uint.MaxValue;
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                checkIndex = controllerReference.index;
            }
            else
            {
                VRTK_ControllerReference leftHand = VRTK_ControllerReference.GetControllerReference(GetControllerLeftHand());
                VRTK_ControllerReference rightHand = VRTK_ControllerReference.GetControllerReference(GetControllerRightHand());

                if (!VRTK_ControllerReference.IsValid(leftHand) && !VRTK_ControllerReference.IsValid(rightHand))
                {
                    return ControllerType.Undefined;
                }
                checkIndex = (VRTK_ControllerReference.IsValid(rightHand) ? rightHand.index : leftHand.index);
            }

            ControllerType returnType = ControllerType.Undefined;
            if (checkIndex < uint.MaxValue)
            {
                string controllerModelNumber = GetModelNumber(checkIndex);
                returnType = MatchControllerTypeByString(controllerModelNumber);
            }
            return returnType;
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_ViveWand:
                    return "ControllerColliders/HTCVive";
                case ControllerType.SteamVR_OculusTouch:
                    return (hand == ControllerHand.Left ? "ControllerColliders/SteamVROculusTouch_Left" : "ControllerColliders/SteamVROculusTouch_Right");
                case ControllerType.SteamVR_ValveKnuckles:
                    return (hand == ControllerHand.Left ? "ControllerColliders/ValveKnuckles_Left" : "ControllerColliders/ValveKnuckles_Right");
                case ControllerType.SteamVR_WindowsMRController:
                    return (hand == ControllerHand.Left ? "ControllerColliders/SteamVRWindowsMRController_Left" : "ControllerColliders/SteamVRWindowsMRController_Right");
                default:
                    return "ControllerColliders/Fallback";
            }
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
            string suffix = (fullPath ? "/attach" : "");
            switch (element)
            {
                case ControllerElements.AttachPoint:
                    return "tip/attach";
                case ControllerElements.Trigger:
                    return "trigger" + suffix;
                case ControllerElements.GripLeft:
                    return GetControllerGripPath(hand, suffix, ControllerHand.Left);
                case ControllerElements.GripRight:
                    return GetControllerGripPath(hand, suffix, ControllerHand.Right);
                case ControllerElements.Touchpad:
                    return GetControllerTouchpadPath(hand, suffix);
                case ControllerElements.ButtonOne:
                    return GetControllerButtonOnePath(hand, suffix);
                case ControllerElements.ButtonTwo:
                    return GetControllerButtonTwoPath(hand, suffix);
                case ControllerElements.SystemMenu:
                    return GetControllerSystemMenuPath(hand, suffix);
                case ControllerElements.StartMenu:
                    return GetControllerStartMenuPath(hand, suffix);
                case ControllerElements.Body:
                    return "body";
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
            SteamVR_TrackedObject trackedObject = GetTrackedObject(controller);
            return (trackedObject != null ? (uint)trackedObject.index : uint.MaxValue);
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
            if (index < uint.MaxValue)
            {
                VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
                if (sdkManager != null)
                {
                    if (cachedLeftTrackedObject != null && (uint)cachedLeftTrackedObject.index == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                    }

                    if (cachedRightTrackedObject != null && (uint)cachedRightTrackedObject.index == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
                    }
                }

                SteamVR_TrackedObject currentTrackedObjectByIndex = VRTK_SharedMethods.GetDictionaryValue(cachedTrackedObjectsByIndex, index);
                if (currentTrackedObjectByIndex != null)
                {
                    return currentTrackedObjectByIndex.gameObject;
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
            SteamVR_TrackedObject trackedObject = GetTrackedObject(controllerReference.actual);
            if (trackedObject != null)
            {
                return (trackedObject.origin != null ? trackedObject.origin : trackedObject.transform.parent);
            }

            return null;
        }

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <param name="parent">The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.</param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        [System.Obsolete("GenerateControllerPointerOrigin has been deprecated and will be removed in a future version of VRTK.")]
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_OculusTouch:
                    if (IsControllerLeftHand(parent) || IsControllerRightHand(parent))
                    {
                        GameObject generatedOrigin = new GameObject(parent.name + " _CustomPointerOrigin");
                        generatedOrigin.transform.SetParent(parent.transform);
                        generatedOrigin.transform.localEulerAngles = new Vector3(40f, 0f, 0f);
                        generatedOrigin.transform.localPosition = new Vector3((IsControllerLeftHand(parent) ? 0.0081f : -0.0081f), -0.0273f, -0.0311f);
                        return generatedOrigin.transform;
                    }
                    break;
            }
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
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<SteamVR_ControllerManager>("Controller (left)", true);
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
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<SteamVR_ControllerManager>("Controller (right)", true);
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
            return ShouldWaitForControllerModel(hand, false);
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
                switch (hand)
                {
                    case ControllerHand.Left:
                        model = (defaultSDKLeftControllerModel != null ? defaultSDKLeftControllerModel.gameObject : null);
                        break;
                    case ControllerHand.Right:
                        model = (defaultSDKRightControllerModel != null ? defaultSDKRightControllerModel.gameObject : null);
                        break;
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
            SteamVR_RenderModel renderModel = controllerReference.actual.GetComponentInChildren<SteamVR_RenderModel>();
            return (renderModel != null ? renderModel.gameObject : null);
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            SteamVR_RenderModel model = renderModel.GetComponent<SteamVR_RenderModel>();
            if (model != null)
            {
                model.controllerModeState.bScrollWheelVisible = state;
            }
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            if (index < OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                float convertedStrength = maxHapticVibration * strength;
                SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
                device.TriggerHapticPulse((ushort)convertedStrength, EVRButtonId.k_EButton_Axis0);
            }
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            //SteamVR doesn't support audio haptics so return false to do a fallback.
            return false;
        }

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            SDK_ControllerHapticModifiers modifiers = new SDK_ControllerHapticModifiers();
            modifiers.maxHapticVibration = maxHapticVibration;
            return modifiers;
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            if (index <= (uint)SteamVR_TrackedObject.EIndex.Hmd || index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return Vector3.zero;
            }
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
            return device.velocity;
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            if (index <= (uint)SteamVR_TrackedObject.EIndex.Hmd || index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return Vector3.zero;
            }
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
            return device.angularVelocity;
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            if (index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return Vector2.zero;
            }

            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    return device.GetAxis();
                case ButtonTypes.TouchpadTwo:
                    return (VRTK_DeviceFinder.GetCurrentControllerType() == ControllerType.SteamVR_WindowsMRController ? device.GetAxis(EVRButtonId.k_EButton_Axis2) : Vector2.zero);
                case ButtonTypes.Trigger:
                    return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
                case ButtonTypes.Grip:
                    switch (GetCurrentControllerType())
                    {
                        case ControllerType.SteamVR_OculusTouch:
                        case ControllerType.SteamVR_ValveKnuckles:
                            return device.GetAxis(EVRButtonId.k_EButton_Axis2);
                        default:
                            return new Vector2((GetControllerButtonState(buttonType, ButtonPressTypes.Press, controllerReference) ? 1f : 0f), 0f);
                    }
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            if (index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return 0f;
            }

            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return device.GetAxis(EVRButtonId.k_EButton_Axis3).x;
                case ButtonTypes.Grip:
                    return device.GetAxis(EVRButtonId.k_EButton_Axis2).x;
                case ButtonTypes.MiddleFinger:
                    return device.GetAxis(EVRButtonId.k_EButton_Axis3).y;
                case ButtonTypes.RingFinger:
                    return device.GetAxis(EVRButtonId.k_EButton_Axis4).x;
                case ButtonTypes.PinkyFinger:
                    return device.GetAxis(EVRButtonId.k_EButton_Axis4).y;
            }
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            if (index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return 0f;
            }
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);

            return (buttonType == ButtonTypes.Trigger || buttonType == ButtonTypes.TriggerHairline ? device.hairTriggerDelta : 0f);
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            if (index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return false;
            }

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return IsButtonPressed(index, pressType, SteamVR_Controller.ButtonMask.Trigger);
                case ButtonTypes.TriggerHairline:
                    if (pressType == ButtonPressTypes.PressDown)
                    {
                        return SteamVR_Controller.Input((int)index).GetHairTriggerDown();
                    }
                    else if (pressType == ButtonPressTypes.PressUp)
                    {
                        return SteamVR_Controller.Input((int)index).GetHairTriggerUp();
                    }
                    break;
                case ButtonTypes.Grip:
                    return IsButtonPressed(index, pressType, SteamVR_Controller.ButtonMask.Grip);
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(index, pressType, SteamVR_Controller.ButtonMask.Touchpad);
                case ButtonTypes.ButtonOne:
                    return IsButtonPressed(index, pressType, (1ul << (int)EVRButtonId.k_EButton_A));
                case ButtonTypes.ButtonTwo:
                    return IsButtonPressed(index, pressType, SteamVR_Controller.ButtonMask.ApplicationMenu);
                case ButtonTypes.StartMenu:
                    return IsButtonPressed(index, pressType, SteamVR_Controller.ButtonMask.System);
                case ButtonTypes.TouchpadTwo:
                    return (VRTK_DeviceFinder.GetCurrentControllerType() == ControllerType.SteamVR_WindowsMRController ? CheckAxisTouch(index, pressType, EVRButtonId.k_EButton_Axis2) : false);
            }
            return false;
        }

        protected virtual void Awake()
        {
            defaultSDKLeftControllerModel = (GetControllerLeftHand(true) != null ? GetControllerLeftHand(true).transform.Find("Model") : null);
            defaultSDKRightControllerModel = (GetControllerRightHand(true) != null ? GetControllerRightHand(true).transform.Find("Model") : null);

#if VRTK_DEFINE_STEAMVR_PLUGIN_1_1_1_OR_OLDER
            SteamVR_Utils.Event.Listen("TrackedDeviceRoleChanged", OnTrackedDeviceRoleChanged);
            SteamVR_Utils.Event.Listen("render_model_loaded", OnRenderModelLoaded);
#elif VRTK_DEFINE_STEAMVR_PLUGIN_1_2_0
            SteamVR_Events.System("TrackedDeviceRoleChanged").Listen(OnTrackedDeviceRoleChanged);
            SteamVR_Events.RenderModelLoaded.Listen(OnRenderModelLoaded);
#elif VRTK_DEFINE_STEAMVR_PLUGIN_1_2_1_OR_NEWER
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Listen(OnTrackedDeviceRoleChanged);
            SteamVR_Events.RenderModelLoaded.Listen(OnRenderModelLoaded);
#endif

            SetTrackedControllerCaches(true);
        }

        protected virtual void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
        {
            SetTrackedControllerCaches(true);
        }

        protected virtual void OnRenderModelLoaded(SteamVR_RenderModel givenControllerRenderModel, bool successfullyLoaded)
        {
            if (successfullyLoaded)
            {
                SteamVR_RenderModel leftControllerRenderModel = (GetControllerLeftHand(true) != null ? GetControllerLeftHand(true).GetComponentInChildren<SteamVR_RenderModel>() : null);
                SteamVR_RenderModel rightControllerRenderModel = (GetControllerRightHand(true) != null ? GetControllerRightHand(true).GetComponentInChildren<SteamVR_RenderModel>() : null);
                ControllerHand selectedHand = ControllerHand.None;
                if (givenControllerRenderModel == leftControllerRenderModel)
                {
                    selectedHand = ControllerHand.Left;
                }
                else if (givenControllerRenderModel == rightControllerRenderModel)
                {
                    selectedHand = ControllerHand.Right;
                }
                OnControllerModelReady(selectedHand, VRTK_ControllerReference.GetControllerReference((uint)givenControllerRenderModel.index));
            }
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftTrackedObject = null;
                cachedRightTrackedObject = null;
                cachedTrackedObjectsByGameObject.Clear();
                cachedTrackedObjectsByIndex.Clear();
            }

            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftTrackedObject == null && sdkManager.loadedSetup.actualLeftController)
                {
                    cachedLeftTrackedObject = sdkManager.loadedSetup.actualLeftController.GetComponent<SteamVR_TrackedObject>();
                }
                if (cachedRightTrackedObject == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightTrackedObject = sdkManager.loadedSetup.actualRightController.GetComponent<SteamVR_TrackedObject>();
                }
            }
        }

        protected virtual SteamVR_TrackedObject GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();

            if (IsControllerLeftHand(controller))
            {
                return cachedLeftTrackedObject;
            }
            else if (IsControllerRightHand(controller))
            {
                return cachedRightTrackedObject;
            }

            if (controller == null)
            {
                return null;
            }

            SteamVR_TrackedObject currentTrackedObjectByGameObject = VRTK_SharedMethods.GetDictionaryValue(cachedTrackedObjectsByGameObject, controller);

            if (currentTrackedObjectByGameObject != null)
            {
                return currentTrackedObjectByGameObject;
            }
            else
            {
                SteamVR_TrackedObject trackedObject = controller.GetComponent<SteamVR_TrackedObject>();
                if (trackedObject != null)
                {
                    VRTK_SharedMethods.AddDictionaryValue(cachedTrackedObjectsByGameObject, controller, trackedObject, true);
                    VRTK_SharedMethods.AddDictionaryValue(cachedTrackedObjectsByIndex, (uint)trackedObject.index, trackedObject, true);
                }
                return trackedObject;
            }
        }

        protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, ulong button)
        {
            if (index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return false;
            }
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);

            switch (type)
            {
                case ButtonPressTypes.Press:
                    return device.GetPress(button);
                case ButtonPressTypes.PressDown:
                    return device.GetPressDown(button);
                case ButtonPressTypes.PressUp:
                    return device.GetPressUp(button);
                case ButtonPressTypes.Touch:
                    return device.GetTouch(button);
                case ButtonPressTypes.TouchDown:
                    return device.GetTouchDown(button);
                case ButtonPressTypes.TouchUp:
                    return device.GetTouchUp(button);
            }

            return false;
        }

        protected virtual bool CheckAxisTouch(uint index, ButtonPressTypes type, EVRButtonId axisId)
        {
            if (index >= OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                return false;
            }
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
            Vector2 axisValue = device.GetAxis(axisId);

            bool currentAxisPressState = VRTK_SharedMethods.GetDictionaryValue(axisTouchStates, axisId, false, true);
            float axisFidelity = VRTK_SharedMethods.GetDictionaryValue(axisTouchFidelity, axisId);

            switch (type)
            {
                case ButtonPressTypes.Touch:
                    return (!VRTK_SharedMethods.Vector3ShallowCompare(axisValue, Vector2.zero, axisFidelity));
                case ButtonPressTypes.TouchDown:
                    if (!currentAxisPressState && !VRTK_SharedMethods.Vector3ShallowCompare(axisValue, Vector2.zero, axisFidelity))
                    {
                        VRTK_SharedMethods.AddDictionaryValue(axisTouchStates, axisId, true, true);
                        return true;
                    }
                    return false;
                case ButtonPressTypes.TouchUp:
                    if (currentAxisPressState && VRTK_SharedMethods.Vector3ShallowCompare(axisValue, Vector2.zero, axisFidelity))
                    {
                        VRTK_SharedMethods.AddDictionaryValue(axisTouchStates, axisId, false, true);
                        return true;
                    }
                    return false;
            }
            return false;
        }

        protected virtual string GetControllerGripPath(ControllerHand hand, string suffix, ControllerHand forceHand)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_ViveWand:
                    return (forceHand == ControllerHand.Left ? "lgrip" : "rgrip") + suffix;
                case ControllerType.SteamVR_ValveKnuckles:
                    return "button_b" + suffix;
                case ControllerType.SteamVR_OculusTouch:
                    return "grip" + suffix;
                case ControllerType.SteamVR_WindowsMRController:
                    return "handgrip" + suffix;
            }
            return null;
        }

        protected virtual string GetControllerTouchpadPath(ControllerHand hand, string suffix)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_ViveWand:
                case ControllerType.SteamVR_ValveKnuckles:
                case ControllerType.SteamVR_WindowsMRController:
                    return "trackpad" + suffix;
                case ControllerType.SteamVR_OculusTouch:
                    return "thumbstick" + suffix;
            }
            return null;
        }

        protected virtual string GetControllerButtonOnePath(ControllerHand hand, string suffix)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_OculusTouch:
                    return (hand == ControllerHand.Left ? "x_button" : "a_button") + suffix;
            }
            return null;
        }

        protected virtual string GetControllerButtonTwoPath(ControllerHand hand, string suffix)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_ViveWand:
                case ControllerType.SteamVR_ValveKnuckles:
                case ControllerType.SteamVR_WindowsMRController:
                    return "button" + suffix;
                case ControllerType.SteamVR_OculusTouch:
                    return (hand == ControllerHand.Left ? "y_button" : "b_button") + suffix;
            }
            return null;
        }

        protected virtual string GetControllerSystemMenuPath(ControllerHand hand, string suffix)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_ViveWand:
                case ControllerType.SteamVR_ValveKnuckles:
                    return "sys_button" + suffix;
                case ControllerType.SteamVR_OculusTouch:
                    return (hand == ControllerHand.Left ? "enter_button" : "home_button") + suffix;
            }
            return null;
        }

        protected virtual string GetControllerStartMenuPath(ControllerHand hand, string suffix)
        {
            switch (GetCurrentControllerType())
            {
                case ControllerType.SteamVR_OculusTouch:
                    return (hand == ControllerHand.Left ? "enter_button" : "home_button") + suffix;
            }
            return null;
        }

        protected virtual ControllerType MatchControllerTypeByString(string controllerModelNumber)
        {
            //Direct string matches for speed
            switch (controllerModelNumber)
            {
                case "vive controller mv":
                case "vive controller dvt":
                    return ControllerType.SteamVR_ViveWand;
                case "knuckles ev1.3":
                    return ControllerType.SteamVR_ValveKnuckles;
                case "oculus rift cv1 (right controller)":
                case "oculus rift cv1 (left controller)":
                    return ControllerType.SteamVR_OculusTouch;
                case "windowsmr: 0x045e/0x065b/0/2":
                    return ControllerType.SteamVR_WindowsMRController;
            }
            return FuzzyMatchControllerTypeByString(controllerModelNumber);
        }

        protected virtual ControllerType FuzzyMatchControllerTypeByString(string controllerModelNumber)
        {
            //Fallback to fuzzy matching
            if (controllerModelNumber.Contains("knuckles"))
            {
                return ControllerType.SteamVR_ValveKnuckles;
            }
            else if (controllerModelNumber.Contains("vive"))
            {
                return ControllerType.SteamVR_ViveWand;
            }
            else if (controllerModelNumber.Contains("oculus rift"))
            {
                return ControllerType.SteamVR_OculusTouch;
            }
            else if (controllerModelNumber.Contains("windowsmr"))
            {
                return ControllerType.SteamVR_WindowsMRController;
            }

            return ControllerType.Undefined;
        }

        protected virtual string GetModelNumber(uint index)
        {
            return (SteamVR.instance != null ? SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, index) : "").ToLower();
        }
#endif
    }
}