// Oculus Controller|SDK_Oculus|004
namespace VRTK
{
#if VRTK_DEFINE_SDK_OCULUS
    using UnityEngine;
    using System.Collections.Generic;
#endif

    /// <summary>
    /// The Oculus Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_OculusSystem))]
    public class SDK_OculusController
#if VRTK_DEFINE_SDK_OCULUS
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_OCULUS
        protected SDK_OculusBoundaries cachedBoundariesSDK;
        protected VRTK_TrackedController cachedLeftController;
        protected VRTK_TrackedController cachedRightController;
        protected OVRInput.Controller[] touchControllers = new OVRInput.Controller[] { OVRInput.Controller.LTouch, OVRInput.Controller.RTouch };
        protected OVRInput.RawAxis2D[] touchpads = new OVRInput.RawAxis2D[] { OVRInput.RawAxis2D.LThumbstick, OVRInput.RawAxis2D.RThumbstick };
        protected OVRInput.RawAxis1D[] triggers = new OVRInput.RawAxis1D[] { OVRInput.RawAxis1D.LIndexTrigger, OVRInput.RawAxis1D.RIndexTrigger };
        protected OVRInput.RawAxis1D[] grips = new OVRInput.RawAxis1D[] { OVRInput.RawAxis1D.LHandTrigger, OVRInput.RawAxis1D.RHandTrigger };

        protected Quaternion[] previousControllerRotations = new Quaternion[2];
        protected Quaternion[] currentControllerRotations = new Quaternion[2];

        protected bool[] previousHairTriggerState = new bool[2];
        protected bool[] currentHairTriggerState = new bool[2];

        protected bool[] previousHairGripState = new bool[2];
        protected bool[] currentHairGripState = new bool[2];

        protected float[] hairTriggerLimit = new float[2];
        protected float[] hairGripLimit = new float[2];

        protected OVRHapticsClip hapticsProceduralClipLeft;
        protected OVRHapticsClip hapticsProceduralClipRight;

        /// <summary>
        /// This method is called just after loading the <see cref="VRTK_SDKSetup"/> that's using this SDK.
        /// </summary>
        /// <param name="setup">The SDK Setup which is using this SDK.</param>
        public override void OnAfterSetupLoad(VRTK_SDKSetup setup)
        {
            base.OnAfterSetupLoad(setup);

            if (hapticsProceduralClipLeft != null || hapticsProceduralClipRight != null)
            {
                return;
            }

            OVRHaptics.Config.Load();
            hapticsProceduralClipLeft = new OVRHapticsClip();
            hapticsProceduralClipRight = new OVRHapticsClip();
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
#if VRTK_DEFINE_OCULUS_UTILITIES_1_11_0_OR_OLDER
            CalculateAngularVelocity(controllerReference);
#endif
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
#if VRTK_DEFINE_OCULUS_UTILITIES_1_12_0_OR_NEWER
            CalculateAngularVelocity(controllerReference);
#endif
        }

        /// <summary>
        /// The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.
        /// </summary>
        /// <returns>The ControllerType based on the SDK and headset being used.</returns>
        public override ControllerType GetCurrentControllerType()
        {
            return ControllerType.Oculus_OculusTouch;
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            if (HasAvatar())
            {
                return "ControllerColliders/OculusTouch_" + hand.ToString();
            }

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
            if (GetAvatar() != null)
            {
                string suffix = (fullPath ? "" : "");
                string parent = "controller_" + (hand == ControllerHand.Left ? "left" : "right") + "_renderPart_0";
                string prefix = (hand == ControllerHand.Left ? "l" : "r") + "ctrl:";
                string child = prefix + (hand == ControllerHand.Left ? "left" : "right") + "_touch_controller_world";

                string path = parent + "/" + child + "/" + prefix + "b_";

                switch (element)
                {
                    case ControllerElements.AttachPoint:
                        return null;
                    case ControllerElements.Trigger:
                        return path + "trigger" + suffix;
                    case ControllerElements.GripLeft:
                        return path + "hold" + suffix;
                    case ControllerElements.GripRight:
                        return path + "hold" + suffix;
                    case ControllerElements.Touchpad:
                        return path + "stick/" + prefix + "b_stick_IGNORE" + suffix;
                    case ControllerElements.ButtonOne:
                        return path + "button01" + suffix;
                    case ControllerElements.ButtonTwo:
                        return path + "button02" + suffix;
                    case ControllerElements.SystemMenu:
                        return path + "button03" + suffix;
                    case ControllerElements.StartMenu:
                        return path + "button03" + suffix;
                    case ControllerElements.Body:
                        return parent;
                }
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
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/LeftHandAnchor");
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
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/RightHandAnchor");
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
                GameObject avatarObject = GetAvatar();
                switch (hand)
                {
                    case ControllerHand.Left:
                        if (avatarObject != null)
                        {
                            model = avatarObject.transform.Find("controller_left").gameObject;
                        }
                        else
                        {
                            model = GetControllerLeftHand(true);
                            model = (model != null && model.transform.childCount > 0 ? model.transform.GetChild(0).gameObject : null);
                        }
                        break;
                    case ControllerHand.Right:
                        if (avatarObject != null)
                        {
                            model = avatarObject.transform.Find("controller_right").gameObject;
                        }
                        else
                        {
                            model = GetControllerRightHand(true);
                            model = (model != null && model.transform.childCount > 0 ? model.transform.GetChild(0).gameObject : null);
                        }
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
            //TODO: NOT IMPLEMENTED
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
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                GameObject controller = GetControllerByIndex(index);

                if (IsControllerLeftHand(controller))
                {
                    hapticsProceduralClipLeft.Reset();
                    hapticsProceduralClipLeft.WriteSample((byte)(strength * byte.MaxValue));
                    OVRHaptics.LeftChannel.Preempt(hapticsProceduralClipLeft);
                }
                else if (IsControllerRightHand(controller))
                {
                    hapticsProceduralClipRight.Reset();
                    hapticsProceduralClipRight.WriteSample((byte)(strength * byte.MaxValue));
                    OVRHaptics.RightChannel.Preempt(hapticsProceduralClipRight);
                }
            }
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                GameObject controller = GetControllerByIndex(index);

                if (IsControllerLeftHand(controller))
                {
                    OVRHaptics.LeftChannel.Preempt(new OVRHapticsClip(clip));
                }
                else if (IsControllerRightHand(controller))
                {
                    OVRHaptics.RightChannel.Preempt(new OVRHapticsClip(clip));
                }
            }
            return true;
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
            VRTK_TrackedController device = GetTrackedObject(controllerReference.actual);
            return OVRInput.GetLocalControllerVelocity(touchControllers[device.index]);
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
            return (VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity));
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
            VRTK_TrackedController device = GetTrackedObject(controllerReference.actual);
            if (device != null)
            {
                switch (buttonType)
                {
                    case ButtonTypes.Touchpad:
                        return OVRInput.Get(touchpads[index], touchControllers[index]);
                    case ButtonTypes.Trigger:
                        return new Vector2(OVRInput.Get(triggers[index], touchControllers[index]), 0f);
                    case ButtonTypes.Grip:
                        return new Vector2(OVRInput.Get(grips[index], touchControllers[index]), 0f);
                }
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
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(index, pressType, OVRInput.Button.PrimaryIndexTrigger);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(index, pressType, OVRInput.Touch.PrimaryIndexTrigger);
                    }
                    break;
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
                    return IsButtonPressed(index, pressType, OVRInput.Button.PrimaryHandTrigger);
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
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(index, pressType, OVRInput.Button.PrimaryThumbstick);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(index, pressType, OVRInput.Touch.PrimaryThumbstick);
                    }
                    break;
                case ButtonTypes.ButtonOne:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(index, pressType, OVRInput.Button.One);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(index, pressType, OVRInput.Touch.One);
                    }
                    break;
                case ButtonTypes.ButtonTwo:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(index, pressType, OVRInput.Button.Two);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(index, pressType, OVRInput.Touch.Two);
                    }
                    break;
                case ButtonTypes.StartMenu:
                    return IsButtonPressed(index, pressType, OVRInput.Button.Start);
            }
            return false;
        }

        protected virtual void CalculateAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                VRTK_TrackedController device = GetTrackedObject(controllerReference.actual);
                if (device == null)
                {
                    return;
                }

                previousControllerRotations[index] = currentControllerRotations[index];
                currentControllerRotations[index] = device.transform.rotation;

                UpdateHairValues(index, GetButtonAxis(ButtonTypes.Trigger, controllerReference).x, GetButtonHairlineDelta(ButtonTypes.Trigger, controllerReference), ref previousHairTriggerState[index], ref currentHairTriggerState[index], ref hairTriggerLimit[index]);
                UpdateHairValues(index, GetButtonAxis(ButtonTypes.Grip, controllerReference).x, GetButtonHairlineDelta(ButtonTypes.Grip, controllerReference), ref previousHairGripState[index], ref currentHairGripState[index], ref hairGripLimit[index]);
            }
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftController = null;
                cachedRightController = null;
            }

            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController == null && sdkManager.loadedSetup.actualLeftController)
                {
                    cachedLeftController = sdkManager.loadedSetup.actualLeftController.GetComponent<VRTK_TrackedController>();
                    if (cachedLeftController != null)
                    {
                        cachedLeftController.index = 0;
                    }
                }
                if (cachedRightController == null && sdkManager.loadedSetup.actualRightController)
                {
                    cachedRightController = sdkManager.loadedSetup.actualRightController.GetComponent<VRTK_TrackedController>();
                    if (cachedRightController != null)
                    {
                        cachedRightController.index = 1;
                    }
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

        protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Button button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }

            VRTK_TrackedController device = GetTrackedObject(GetControllerByIndex(index));
            if (device != null)
            {
                OVRInput.Controller controller = touchControllers[index];
                switch (type)
                {
                    case ButtonPressTypes.Press:
                        return OVRInput.Get(button, controller);
                    case ButtonPressTypes.PressDown:
                        return OVRInput.GetDown(button, controller);
                    case ButtonPressTypes.PressUp:
                        return OVRInput.GetUp(button, controller);
                }
            }

            return false;
        }

        protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Touch button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }

            VRTK_TrackedController device = GetTrackedObject(GetControllerByIndex(index));
            if (device != null)
            {
                OVRInput.Controller controller = touchControllers[index];
                switch (type)
                {
                    case ButtonPressTypes.Touch:
                        return OVRInput.Get(button, controller);
                    case ButtonPressTypes.TouchDown:
                        return OVRInput.GetDown(button, controller);
                    case ButtonPressTypes.TouchUp:
                        return OVRInput.GetUp(button, controller);
                }
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

        protected virtual SDK_OculusBoundaries GetBoundariesSDK()
        {
            if (cachedBoundariesSDK == null)
            {
                cachedBoundariesSDK = (VRTK_SDKManager.instance ? VRTK_SDKManager.instance.loadedSetup.boundariesSDK : CreateInstance<SDK_OculusBoundaries>()) as SDK_OculusBoundaries;
            }

            return cachedBoundariesSDK;
        }

        protected virtual bool HasAvatar(bool controllersAreVisible = true)
        {
            GetBoundariesSDK();
#if VRTK_DEFINE_SDK_OCULUS_AVATAR
            if (cachedBoundariesSDK != null)
            {
                OvrAvatar avatar = cachedBoundariesSDK.GetAvatar();
                return (avatar && controllersAreVisible && avatar.StartWithControllers);
            }
#endif
            return false;
        }

        protected virtual GameObject GetAvatar()
        {
            GetBoundariesSDK();
#if VRTK_DEFINE_SDK_OCULUS_AVATAR
            if (cachedBoundariesSDK != null)
            {
                OvrAvatar avatar = cachedBoundariesSDK.GetAvatar();
                if (avatar != null)
                {
                    return avatar.gameObject;
                }
            }
#endif
            return null;
        }
#endif
    }
}