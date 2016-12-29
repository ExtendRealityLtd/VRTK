// Base Controller|SDK_Base|003
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Base Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    /// <remarks>
    /// This is an abstract class to implement the interface required by all implemented SDKs.
    /// </remarks>
    public abstract class SDK_BaseController : ScriptableObject
    {
        /// <summary>
        /// Concepts of controller button press
        /// </summary>
        /// <param name="Press">The button is currently being pressed.</param>
        /// <param name="PressDown">The button has just been pressed down.</param>
        /// <param name="PressUp">The button has just been released.</param>
        /// <param name="Touch">The button is currently being touched.</param>
        /// <param name="TouchDown">The button has just been touched.</param>
        /// <param name="TouchUp">The button is no longer being touched.</param>
        public enum ButtonPressTypes
        {
            Press,
            PressDown,
            PressUp,
            Touch,
            TouchDown,
            TouchUp
        }

        /// <summary>
        /// The elements of a generic controller
        /// </summary>
        /// <param name="AttachPoint">The default point on the controller to attach grabbed objects to.</param>
        /// <param name="Trigger">The trigger button.</param>
        /// <param name="GripLeft">The left part of the grip button collection.</param>
        /// <param name="GripRight">The right part of the grip button collection.</param>
        /// <param name="Touchpad">The touch pad/stick.</param>
        /// <param name="ButtonOne">The first generic button.</param>
        /// <param name="ButtonTwo">The second generic button.</param>
        /// <param name="SystemMenu">The system menu button.</param>
        /// <param name="Body">The encompassing mesh of the controller body.</param>
        public enum ControllerElements
        {
            AttachPoint,
            Trigger,
            GripLeft,
            GripRight,
            Touchpad,
            ButtonOne,
            ButtonTwo,
            SystemMenu,
            Body
        }

        /// <summary>
        /// Controller hand reference.
        /// </summary>
        /// <param name="None">No hand is assigned.</param>
        /// <param name="Left">The left hand is assigned.</param>
        /// <param name="Right">The right hand is assigned.</param>
        public enum ControllerHand
        {
            None,
            Left,
            Right
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public abstract void ProcessUpdate(uint index, Dictionary<string, object> options);

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public abstract string GetControllerDefaultColliderPath(ControllerHand hand);

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public abstract string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false);

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public abstract uint GetControllerIndex(GameObject controller);

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns></returns>
        public abstract GameObject GetControllerByIndex(uint index, bool actual = false);

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public abstract Transform GetControllerOrigin(GameObject controller);

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        public abstract Transform GenerateControllerPointerOrigin();

        /// <summary>
        /// The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public abstract GameObject GetControllerLeftHand(bool actual = false);

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public abstract GameObject GetControllerRightHand(bool actual = false);

        /// <summary>
        /// The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public abstract bool IsControllerLeftHand(GameObject controller);

        /// <summary>
        /// The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public abstract bool IsControllerRightHand(GameObject controller);

        /// <summary>
        /// The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public abstract bool IsControllerLeftHand(GameObject controller, bool actual);

        /// <summary>
        /// The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public abstract bool IsControllerRightHand(GameObject controller, bool actual);

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given GameObject.
        /// </summary>
        /// <param name="controller">The GameObject to get the model alias for.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public abstract GameObject GetControllerModel(GameObject controller);


        /// <summary>
        /// The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public abstract GameObject GetControllerModel(ControllerHand hand);

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public abstract GameObject GetControllerRenderModel(GameObject controller);

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public abstract void SetControllerRenderModelWheel(GameObject renderModel, bool state);

        /// <summary>
        /// The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public abstract void HapticPulseOnIndex(uint index, float strength = 0.5f);

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public abstract SDK_ControllerHapticModifiers GetHapticModifiers();

        /// <summary>
        /// The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public abstract Vector3 GetVelocityOnIndex(uint index);

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public abstract Vector3 GetAngularVelocityOnIndex(uint index);

        /// <summary>
        /// The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current x,y position of where the touchpad is being touched.</returns>
        public abstract Vector2 GetTouchpadAxisOnIndex(uint index);

        /// <summary>
        /// The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the trigger.</returns>
        public abstract Vector2 GetTriggerAxisOnIndex(uint index);

        /// <summary>
        /// The GetGripAxisOnIndex method is used to get the current grip position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the grip.</returns>
        public abstract Vector2 GetGripAxisOnIndex(uint index);

        /// <summary>
        /// The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the trigger presses.</returns>
        public abstract float GetTriggerHairlineDeltaOnIndex(uint index);

        /// <summary>
        /// The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the grip presses.</returns>
        public abstract float GetGripHairlineDeltaOnIndex(uint index);

        /// <summary>
        /// The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public abstract bool IsTriggerPressedOnIndex(uint index);

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public abstract bool IsTriggerPressedDownOnIndex(uint index);

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsTriggerPressedUpOnIndex(uint index);

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public abstract bool IsTriggerTouchedOnIndex(uint index);

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public abstract bool IsTriggerTouchedDownOnIndex(uint index);

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsTriggerTouchedUpOnIndex(uint index);

        /// <summary>
        /// The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public abstract bool IsHairTriggerDownOnIndex(uint index);

        /// <summary>
        /// The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public abstract bool IsHairTriggerUpOnIndex(uint index);

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public abstract bool IsGripPressedOnIndex(uint index);

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public abstract bool IsGripPressedDownOnIndex(uint index);

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsGripPressedUpOnIndex(uint index);

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public abstract bool IsGripTouchedOnIndex(uint index);

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public abstract bool IsGripTouchedDownOnIndex(uint index);

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsGripTouchedUpOnIndex(uint index);

        /// <summary>
        /// The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public abstract bool IsHairGripDownOnIndex(uint index);

        /// <summary>
        /// The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public abstract bool IsHairGripUpOnIndex(uint index);

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public abstract bool IsTouchpadPressedOnIndex(uint index);

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public abstract bool IsTouchpadPressedDownOnIndex(uint index);

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsTouchpadPressedUpOnIndex(uint index);

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public abstract bool IsTouchpadTouchedOnIndex(uint index);

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public abstract bool IsTouchpadTouchedDownOnIndex(uint index);

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsTouchpadTouchedUpOnIndex(uint index);

        /// <summary>
        /// The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public abstract bool IsButtonOnePressedOnIndex(uint index);

        /// <summary>
        /// The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public abstract bool IsButtonOnePressedDownOnIndex(uint index);

        /// <summary>
        /// The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsButtonOnePressedUpOnIndex(uint index);

        /// <summary>
        /// The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public abstract bool IsButtonOneTouchedOnIndex(uint index);

        /// <summary>
        /// The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public abstract bool IsButtonOneTouchedDownOnIndex(uint index);

        /// <summary>
        /// The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsButtonOneTouchedUpOnIndex(uint index);

        /// <summary>
        /// The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public abstract bool IsButtonTwoPressedOnIndex(uint index);

        /// <summary>
        /// The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public abstract bool IsButtonTwoPressedDownOnIndex(uint index);

        /// <summary>
        /// The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsButtonTwoPressedUpOnIndex(uint index);

        /// <summary>
        /// The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public abstract bool IsButtonTwoTouchedOnIndex(uint index);

        /// <summary>
        /// The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public abstract bool IsButtonTwoTouchedDownOnIndex(uint index);

        /// <summary>
        /// The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public abstract bool IsButtonTwoTouchedUpOnIndex(uint index);

        protected GameObject GetSDKManagerControllerLeftHand(bool actual = false)
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                return (actual ? sdkManager.actualLeftController : sdkManager.scriptAliasLeftController);
            }
            return null;
        }

        protected GameObject GetSDKManagerControllerRightHand(bool actual = false)
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                return (actual ? sdkManager.actualRightController : sdkManager.scriptAliasRightController);
            }
            return null;
        }

        protected bool CheckActualOrScriptAliasControllerIsLeftHand(GameObject controller)
        {
            return (IsControllerLeftHand(controller, true) || IsControllerLeftHand(controller, false));
        }

        protected bool CheckActualOrScriptAliasControllerIsRightHand(GameObject controller)
        {
            return (IsControllerRightHand(controller, true) || IsControllerRightHand(controller, false));
        }

        protected bool CheckControllerLeftHand(GameObject controller, bool actual)
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                return (actual ? controller.Equals(sdkManager.actualLeftController) : controller.Equals(sdkManager.scriptAliasLeftController));
            }
            return false;
        }

        protected bool CheckControllerRightHand(GameObject controller, bool actual)
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                return (actual ? controller.Equals(sdkManager.actualRightController) : controller.Equals(sdkManager.scriptAliasRightController));
            }
            return false;
        }

        protected GameObject GetControllerModelFromController(GameObject controller)
        {
            return GetControllerModel(VRTK_DeviceFinder.GetControllerHand(controller));
        }

        protected GameObject GetSDKManagerControllerModelForHand(ControllerHand hand)
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                switch (hand)
                {
                    case ControllerHand.Left:
                        return sdkManager.modelAliasLeftController;
                    case ControllerHand.Right:
                        return sdkManager.modelAliasRightController;
                }
            }
            return null;
        }

        protected GameObject GetActualController(GameObject controller)
        {
            GameObject returnController = null;
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (IsControllerLeftHand(controller))
                {
                    returnController = sdkManager.actualLeftController;
                }
                else if (IsControllerRightHand(controller))
                {
                    returnController = sdkManager.actualRightController;
                }
            }
            return returnController;
        }
    }

    public class SDK_ControllerHapticModifiers
    {
        public float durationModifier = 1f;
        public float intervalModifier = 1f;
    }
}