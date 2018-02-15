// HyperealVR Controller|SDK_HyperealVR|004
namespace VRTK
{
#if VRTK_DEFINE_SDK_HYPEREALVR
    using UnityEngine;
    using System.Collections.Generic;
    using Hypereal;
    using System;
#endif

    /// <summary>
    /// The HyperealVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_HyperealVRSystem))]
    public class SDK_HyperealVRController
#if VRTK_DEFINE_SDK_HYPEREALVR
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_HYPEREALVR
        private VRTK_TrackedController cachedLeftController;
        private VRTK_TrackedController cachedRightController;
        private VRTK_TrackedController cachedLeftTrackedObject;
        private VRTK_TrackedController cachedRightTrackedObject;

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
            return ControllerType.Custom;
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/HyperealSens";
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
                    return "Base";
                case ControllerElements.Body:
                    return "Base/SM_Prop_HyFeel" + (hand == ControllerHand.Left ? "L" : "R") + "_02";
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
            VRTK_TrackedController trackedObject = GetTrackedObject(controller);
            return (trackedObject ? (uint)trackedObject.index : uint.MaxValue);
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
        /// <param name="controllerReference">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            return controllerReference.actual.transform;
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
            if (actual)
            {
                HyTrackObjRig trackedObjRig = VRTK_SharedMethods.FindEvenInactiveComponent<HyTrackObjRig>(true);
                if (trackedObjRig)
                {
                    return trackedObjRig.leftController;
                }
            }
            else
            {
                VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
                if (sdkManager != null)
                {
                    return sdkManager.scriptAliasLeftController;
                }
            }
            return null;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            if (actual)
            {
                HyTrackObjRig trackedObjRig = VRTK_SharedMethods.FindEvenInactiveComponent<HyTrackObjRig>(true);
                if (trackedObjRig)
                {
                    return trackedObjRig.rightController;
                }
            }
            else
            {
                VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
                if (sdkManager != null)
                {
                    return sdkManager.scriptAliasRightController;
                }
            }
            return null;
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
            GameObject modelGO = GetSDKManagerControllerModelForHand(hand);
            if (!modelGO)
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
                    Transform model = controller.transform.Find("Model");
                    modelGO = (model != null ? model.gameObject : null);
                }
            }
            return modelGO;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controllerReference">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            return controllerReference.actual.GetComponentInChildren<MeshRenderer>().gameObject;
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);

            if (ctrlDevice == HyDevice.Device_Unknown)
            {
                return;
            }

            HyperealVR.Instance.SetHapticFeedback(ctrlDevice, 0.5f, strength);
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            //TODO;
            return false;
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            SetTrackedControllerCaches();
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller0);
                    return controllerState.velocity;
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller1);
                    return controllerState.velocity;
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            SetTrackedControllerCaches();
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller0);
                    return controllerState.angularVelocity;
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    HyTrackingState controllerState = HyperealVR.Instance.GetTrackingState(HyDevice.Device_Controller1);
                    return controllerState.angularVelocity;
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);
            if (ctrlDevice == HyDevice.Device_Unknown)
            {
                return Vector2.zero;
            }

            HyInput input = HyInputManager.Instance.GetInputDevice(ctrlDevice);

            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    return input.GetTouchpadAxis();
                case ButtonTypes.Trigger:
                    return new Vector2(input.GetTriggerAxis(HyInputKey.IndexTrigger), 0f);
                case ButtonTypes.Grip:
                    return new Vector2(input.GetTriggerAxis(HyInputKey.SideTrigger), 0f);
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);
            if (ctrlDevice == HyDevice.Device_Unknown)
            {
                return 0f;
            }

            return (buttonType == ButtonTypes.Trigger ? 0.1f : 0f);
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
            HyDevice ctrlDevice = MappingIndex2HyDevice(index);
            if (ctrlDevice == HyDevice.Device_Unknown)
            {
                return false;
            }

            switch (buttonType)
            {
                case ButtonTypes.ButtonOne:
                    return false;
                case ButtonTypes.ButtonTwo:
                    if (ctrlDevice == HyDevice.Device_Controller0)
                    {
                        return IsButtonPressed(ctrlDevice, pressType, HyInputKey.Menu);
                    }
                    else
                    {
                        return false;
                    }
                case ButtonTypes.Grip:
                    return IsButtonPressed(ctrlDevice, pressType, HyInputKey.SideTrigger);
                case ButtonTypes.GripHairline:
                    return false;
                case ButtonTypes.StartMenu:
                    if (ctrlDevice == HyDevice.Device_Controller0)
                    {
                        return false;
                    }
                    else
                    {
                        return IsButtonPressed(ctrlDevice, pressType, HyInputKey.Menu);
                    }
                case ButtonTypes.Trigger:
                    return IsButtonPressed(ctrlDevice, pressType, HyInputKey.IndexTrigger);
                case ButtonTypes.TriggerHairline:
                    return false;
                case ButtonTypes.Touchpad:
                    return IsButtonPressed(ctrlDevice, pressType, HyInputKey.Touchpad);
            }
            return false;
        }

        private void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
        {
            SetTrackedControllerCaches(true);
        }

        private void SetTrackedControllerCaches(bool forceRefresh = false)
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
                    cachedLeftController = sdkManager.loadedSetup.actualLeftController.GetComponent<VRTK_TrackedController>();
                    if (cachedLeftController != null)
                    {
                        cachedLeftController.index = 0;
                    }
                }
                if (cachedRightTrackedObject == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightController = sdkManager.loadedSetup.actualRightController.GetComponent<VRTK_TrackedController>();
                    if (cachedRightController != null)
                    {
                        cachedRightController.index = 1;
                    }
                }
            }
        }

        private VRTK_TrackedController GetTrackedObject(GameObject controller)
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

        private HyDevice MappingIndex2HyDevice(uint index)
        {
            switch (index)
            {
                case 0:
                    return HyDevice.Device_Controller0;
                case 1:
                    return HyDevice.Device_Controller1;
                default:
                    break;
            }
            return HyDevice.Device_Unknown;
        }

        private bool IsButtonPressed(HyDevice index, ButtonPressTypes type, HyInputKey button)
        {
            //todo use index to input type
            HyInput device = HyInputManager.Instance.GetInputDevice(index);
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

        protected virtual void Awake()
        {
            SetTrackedControllerCaches(true);
        }

#endif
    }
}