// OculusVR Controller|SDK_OculusVR|004
namespace VRTK
{
#if VRTK_DEFINE_SDK_OCULUSVR
    using UnityEngine;
    using System.Collections.Generic;
#endif

    /// <summary>
    /// The OculusVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_OculusVRSystem))]
    public class SDK_OculusVRController
#if VRTK_DEFINE_SDK_OCULUSVR
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_OCULUSVR
        private SDK_OculusVRBoundaries cachedBoundariesSDK;
        private VRTK_TrackedController cachedLeftController;
        private VRTK_TrackedController cachedRightController;
        private OVRInput.Controller[] touchControllers = new OVRInput.Controller[] { OVRInput.Controller.LTouch, OVRInput.Controller.RTouch };
        private OVRInput.RawAxis2D[] touchpads = new OVRInput.RawAxis2D[] { OVRInput.RawAxis2D.LThumbstick, OVRInput.RawAxis2D.RThumbstick };
        private OVRInput.RawAxis1D[] triggers = new OVRInput.RawAxis1D[] { OVRInput.RawAxis1D.LIndexTrigger, OVRInput.RawAxis1D.RIndexTrigger };
        private OVRInput.RawAxis1D[] grips = new OVRInput.RawAxis1D[] { OVRInput.RawAxis1D.LHandTrigger, OVRInput.RawAxis1D.RHandTrigger };

        private Quaternion[] previousControllerRotations = new Quaternion[2];
        private Quaternion[] currentControllerRotations = new Quaternion[2];

        private bool[] previousHairTriggerState = new bool[2];
        private bool[] currentHairTriggerState = new bool[2];

        private bool[] previousHairGripState = new bool[2];
        private bool[] currentHairGripState = new bool[2];

        private float[] hairTriggerLimit = new float[2];
        private float[] hairGripLimit = new float[2];

        private OVRHapticsClip hapticsProceduralClipLeft = new OVRHapticsClip();
        private OVRHapticsClip hapticsProceduralClipRight = new OVRHapticsClip();

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(uint index, Dictionary<string, object> options)
        {
#if VRTK_DEFINE_OCULUSVR_UTILITIES_1_11_0_OR_OLDER
            CalculateAngularVelocity(index);
#endif
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(uint index, Dictionary<string, object> options)
        {
#if VRTK_DEFINE_OCULUSVR_UTILITIES_1_12_0_OR_NEWER
            CalculateAngularVelocity(index);
#endif
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
            if (GetAvatar())
            {
                var suffix = (fullPath ? "" : "");
                var parent = "controller_" + (hand == ControllerHand.Left ? "left" : "right") + "_renderPart_0";
                var prefix = (hand == ControllerHand.Left ? "l" : "r") + "ctrl:";
                var child = prefix + (hand == ControllerHand.Left ? "left" : "right") + "_touch_controller_world";

                var path = parent + "/" + child + "/" + prefix + "b_";

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
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController != null && cachedLeftController.index == index)
                {
                    return (actual ? sdkManager.actualLeftController : sdkManager.scriptAliasLeftController);
                }

                if (cachedRightController != null && cachedRightController.index == index)
                {
                    return (actual ? sdkManager.actualRightController : sdkManager.scriptAliasRightController);
                }
            }
            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(GameObject controller)
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
            var controller = GetSDKManagerControllerLeftHand(actual);
            if (!controller && actual)
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
            var controller = GetSDKManagerControllerRightHand(actual);
            if (!controller && actual)
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
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(GameObject controller)
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
        /// The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulseOnIndex(uint index, float strength = 0.5f)
        {
            if (index < uint.MaxValue)
            {
                var controller = GetControllerByIndex(index);

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
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            var modifiers = new SDK_ControllerHapticModifiers();
            modifiers.durationModifier = 0.8f;
            modifiers.intervalModifier = 1f;
            return modifiers;
        }

        /// <summary>
        /// The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }
            var device = GetTrackedObject(GetControllerByIndex(index));
            return OVRInput.GetLocalControllerVelocity(touchControllers[device.index]);
        }

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }

            var deltaRotation = currentControllerRotations[index] * Quaternion.Inverse(previousControllerRotations[index]);
            return new Vector3(Mathf.DeltaAngle(0, deltaRotation.eulerAngles.x), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.y), Mathf.DeltaAngle(0, deltaRotation.eulerAngles.z));
        }

        /// <summary>
        /// The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current x,y position of where the touchpad is being touched.</returns>
        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = GetTrackedObject(GetControllerByIndex(index));
            return (device ? OVRInput.Get(touchpads[index], touchControllers[index]) : Vector2.zero);
        }

        /// <summary>
        /// The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the trigger.</returns>
        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = GetTrackedObject(GetControllerByIndex(index));
            if (device)
            {
                var output = OVRInput.Get(triggers[index], touchControllers[index]);
                return new Vector2(output, 0f);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// The GetGripAxisOnIndex method is used to get the current grip position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the grip.</returns>
        public override Vector2 GetGripAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = GetTrackedObject(GetControllerByIndex(index));
            if (device)
            {
                var output = OVRInput.Get(grips[index], touchControllers[index]);
                return new Vector2(output, 0f);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the trigger presses.</returns>
        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return 0f;
            }
            return 0.1f;
        }

        /// <summary>
        /// The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the grip presses.</returns>
        public override float GetGripHairlineDeltaOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return 0f;
            }
            return 0.1f;
        }

        /// <summary>
        /// The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.PrimaryIndexTrigger);
        }

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.PrimaryIndexTrigger);
        }

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.PrimaryIndexTrigger);
        }

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.PrimaryIndexTrigger);
        }

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.PrimaryIndexTrigger);
        }

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.PrimaryIndexTrigger);
        }

        /// <summary>
        /// The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            return (currentHairTriggerState[index] && !previousHairTriggerState[index]);
        }

        /// <summary>
        /// The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            return (!currentHairTriggerState[index] && previousHairTriggerState[index]);
        }

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsGripPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.PrimaryHandTrigger);
        }

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.PrimaryHandTrigger);
        }

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.PrimaryHandTrigger);
        }

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsGripTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Button.PrimaryHandTrigger);
        }

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Button.PrimaryHandTrigger);
        }

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Button.PrimaryHandTrigger);
        }

        /// <summary>
        /// The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairGripDownOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            return (currentHairGripState[index] && !previousHairGripState[index]);
        }

        /// <summary>
        /// The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairGripUpOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            return (!currentHairGripState[index] && previousHairGripState[index]);
        }

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.PrimaryThumbstick);
        }

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.PrimaryThumbstick);
        }

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.PrimaryThumbstick);
        }

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.PrimaryThumbstick);
        }

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.PrimaryThumbstick);
        }

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.PrimaryThumbstick);
        }

        /// <summary>
        /// The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonOnePressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.One);
        }

        /// <summary>
        /// The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonOnePressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.One);
        }

        /// <summary>
        /// The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOnePressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.One);
        }

        /// <summary>
        /// The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonOneTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.One);
        }

        /// <summary>
        /// The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonOneTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.One);
        }

        /// <summary>
        /// The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOneTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.One);
        }

        /// <summary>
        /// The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonTwoPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.Two);
        }

        /// <summary>
        /// The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonTwoPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.Two);
        }

        /// <summary>
        /// The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.Two);
        }

        /// <summary>
        /// The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonTwoTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, OVRInput.Touch.Two);
        }

        /// <summary>
        /// The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonTwoTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, OVRInput.Touch.Two);
        }

        /// <summary>
        /// The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, OVRInput.Touch.Two);
        }

        /// <summary>
        /// The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsStartMenuPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, OVRInput.Button.Start);
        }

        /// <summary>
        /// The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsStartMenuPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, OVRInput.Button.Start);
        }

        /// <summary>
        /// The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsStartMenuPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, OVRInput.Button.Start);
        }

        /// <summary>
        /// The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsStartMenuTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsStartMenuTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsStartMenuTouchedUpOnIndex(uint index)
        {
            return false;
        }

        private void CalculateAngularVelocity(uint index)
        {
            if (index < uint.MaxValue)
            {
                var device = GetTrackedObject(GetControllerByIndex(index));
                previousControllerRotations[index] = currentControllerRotations[index];
                currentControllerRotations[index] = device.transform.rotation;

                UpdateHairValues(index, GetTriggerAxisOnIndex(index).x, GetTriggerHairlineDeltaOnIndex(index), ref previousHairTriggerState[index], ref currentHairTriggerState[index], ref hairTriggerLimit[index]);
                UpdateHairValues(index, GetGripAxisOnIndex(index).x, GetGripHairlineDeltaOnIndex(index), ref previousHairGripState[index], ref currentHairGripState[index], ref hairGripLimit[index]);
            }
        }

        private void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftController = null;
                cachedRightController = null;
            }

            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (cachedLeftController == null && sdkManager.actualLeftController)
                {
                    cachedLeftController = sdkManager.actualLeftController.GetComponent<VRTK_TrackedController>();
                    cachedLeftController.index = 0;
                }
                if (cachedRightController == null && sdkManager.actualRightController)
                {
                    cachedRightController = sdkManager.actualRightController.GetComponent<VRTK_TrackedController>();
                    cachedRightController.index = 1;
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

        private bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Button button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }

            var device = GetTrackedObject(GetControllerByIndex(index));
            if (device)
            {
                var controller = touchControllers[index];
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

        private bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Touch button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }

            var device = GetTrackedObject(GetControllerByIndex(index));
            if (device)
            {
                var controller = touchControllers[index];
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

        private void UpdateHairValues(uint index, float axisValue, float hairDelta, ref bool previousState, ref bool currentState, ref float hairLimit)
        {
            previousState = currentState;
            var value = axisValue;
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

        private SDK_OculusVRBoundaries GetBoundariesSDK()
        {
            if (cachedBoundariesSDK == null)
            {
                cachedBoundariesSDK = (VRTK_SDKManager.instance ? VRTK_SDKManager.instance.GetBoundariesSDK() : CreateInstance<SDK_OculusVRBoundaries>()) as SDK_OculusVRBoundaries;
            }

            return cachedBoundariesSDK;
        }

        private bool HasAvatar(bool controllersAreVisible = true)
        {
            GetBoundariesSDK();
#if VRTK_DEFINE_SDK_OCULUSVR_AVATAR
            if (cachedBoundariesSDK)
            {
                var avatar = cachedBoundariesSDK.GetAvatar();
                return (avatar && controllersAreVisible && avatar.StartWithControllers);
            }
#endif
            return false;
        }

        private GameObject GetAvatar()
        {
            GetBoundariesSDK();
#if VRTK_DEFINE_SDK_OCULUSVR_AVATAR
            if (cachedBoundariesSDK)
            {
                var avatar = cachedBoundariesSDK.GetAvatar();
                if (avatar)
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