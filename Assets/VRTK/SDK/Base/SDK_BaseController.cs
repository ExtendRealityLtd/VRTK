// Base Controller|SDK_Base|006
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
    public abstract class SDK_BaseController : SDK_Base
    {
        /// <summary>
        /// Types of buttons on a controller
        /// </summary>
        /// <param name="ButtonOne">Button One on the controller.</param>
        /// <param name="ButtonTwo">Button Two on the controller.</param>
        /// <param name="Grip">Grip on the controller.</param>
        /// <param name="GripHairline">Grip Hairline on the controller.</param>
        /// <param name="StartMenu">Start Menu on the controller.</param>
        /// <param name="Trigger">Trigger on the controller.</param>
        /// <param name="TriggerHairline">Trigger Hairline on the controller.</param>
        /// <param name="Touchpad">Touchpad on the controller.</param>
        public enum ButtonTypes
        {
            ButtonOne,
            ButtonTwo,
            Grip,
            GripHairline,
            StartMenu,
            Trigger,
            TriggerHairline,
            Touchpad,
        }

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
        /// <param name="StartMenu">The start menu button.</param>
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
            Body,
            StartMenu
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
        /// SDK Controller types.
        /// </summary>
        /// <param name="Undefined">No controller type.</param>
        /// <param name="Custom">A custom controller type.</param>
        /// <param name="Simulator_Hand">The Simulator default hand controller.</param>
        /// <param name="SteamVR_ViveWand">The HTC Vive wand controller for SteamVR.</param>
        /// <param name="SteamVR_OculusTouch">The Oculus Touch controller for SteamVR.</param>
        /// <param name="Oculus_OculusTouch">The Oculus Touch controller for Oculus Utilities.</param>
        /// <param name="Daydream_Controller">The Daydream controller for Google Daydream SDK.</param>
        /// <param name="Ximmerse_Flip">The Flip controller for Ximmerse SDK.</param>
        public enum ControllerType
        {
            Undefined,
            Custom,
            Simulator_Hand,
            SteamVR_ViveWand,
            SteamVR_OculusTouch,
            Oculus_OculusTouch,
            Daydream_Controller,
            Ximmerse_Flip
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public abstract void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public abstract void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);

        /// <summary>
        /// The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.
        /// </summary>
        /// <returns>The ControllerType based on the SDK and headset being used.</returns>
        public abstract ControllerType GetCurrentControllerType();

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
        /// <returns>The GameObject of the controller</returns>
        public abstract GameObject GetControllerByIndex(uint index, bool actual = false);

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public abstract Transform GetControllerOrigin(VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <param name="parent">The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.</param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        public abstract Transform GenerateControllerPointerOrigin(GameObject parent);

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
        /// The GetControllerModelHand method returns the hand for the given controller model GameObject.
        /// </summary>
        /// <param name="controllerModel">The controller model GameObject to get the hand for.</param>
        /// <returns>The hand enum for which the given controller model is for.</returns>
        public virtual ControllerHand GetControllerModelHand(GameObject controllerModel)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.loadedSetup != null)
            {
                if (controllerModel == sdkManager.loadedSetup.modelAliasLeftController)
                {
                    return ControllerHand.Left;
                }
                else if (controllerModel == sdkManager.loadedSetup.modelAliasRightController)
                {
                    return ControllerHand.Right;
                }
            }
            return ControllerHand.None;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public abstract GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public abstract void SetControllerRenderModelWheel(GameObject renderModel, bool state);

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public abstract void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f);

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public abstract bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip);

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public abstract SDK_ControllerHapticModifiers GetHapticModifiers();

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public abstract Vector3 GetVelocity(VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public abstract Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.
        /// </summary>
        /// <param name="currentAxisValues"></param>
        /// <param name="previousAxisValues"></param>
        /// <param name="compareFidelity"></param>
        /// <returns>Returns true if the touchpad is not currently being touched or moved.</returns>
        public abstract bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity);

        /// <summary>
        /// The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the button axis on.</param>
        /// <returns>A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.</returns>
        public abstract Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.
        /// </summary>
        /// <param name="buttonType">The type of button to get the hairline delta for.</param>
        /// <param name="controllerReference">The reference to the controller to get the hairline delta for.</param>
        /// <returns>The delta between the button presses.</returns>
        public abstract float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);

        /// <summary>
        /// The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the state of.</param>
        /// <param name="pressType">The button state to check for.</param>
        /// <param name="controllerReference">The reference to the controller to check the button state on.</param>
        /// <returns>Returns true if the given button is in the state of the given press type on the given controller reference.</returns>
        public abstract bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference);

        protected virtual GameObject GetSDKManagerControllerLeftHand(bool actual = false)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
            }
            return null;
        }

        protected virtual GameObject GetSDKManagerControllerRightHand(bool actual = false)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
            }
            return null;
        }

        protected virtual bool CheckActualOrScriptAliasControllerIsLeftHand(GameObject controller)
        {
            return (IsControllerLeftHand(controller, true) || IsControllerLeftHand(controller, false));
        }

        protected virtual bool CheckActualOrScriptAliasControllerIsRightHand(GameObject controller)
        {
            return (IsControllerRightHand(controller, true) || IsControllerRightHand(controller, false));
        }

        protected virtual bool CheckControllerLeftHand(GameObject controller, bool actual)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && controller != null)
            {
                return (actual ? controller == sdkManager.loadedSetup.actualLeftController : controller == sdkManager.scriptAliasLeftController);
            }
            return false;
        }

        protected virtual bool CheckControllerRightHand(GameObject controller, bool actual)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && controller != null)
            {
                return (actual ? controller == sdkManager.loadedSetup.actualRightController : controller == sdkManager.scriptAliasRightController);
            }
            return false;
        }

        protected virtual GameObject GetControllerModelFromController(GameObject controller)
        {
            return GetControllerModel(VRTK_DeviceFinder.GetControllerHand(controller));
        }

        protected virtual GameObject GetSDKManagerControllerModelForHand(ControllerHand hand)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                switch (hand)
                {
                    case ControllerHand.Left:
                        return sdkManager.loadedSetup.modelAliasLeftController;
                    case ControllerHand.Right:
                        return sdkManager.loadedSetup.modelAliasRightController;
                }
            }
            return null;
        }

        protected virtual GameObject GetActualController(GameObject controller)
        {
            GameObject returnController = null;
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                if (IsControllerLeftHand(controller))
                {
                    returnController = sdkManager.loadedSetup.actualLeftController;
                }
                else if (IsControllerRightHand(controller))
                {
                    returnController = sdkManager.loadedSetup.actualRightController;
                }
            }
            return returnController;
        }
    }

    public class SDK_ControllerHapticModifiers
    {
        public float durationModifier = 1f;
        public float intervalModifier = 1f;
        public ushort maxHapticVibration = 1;
        public int hapticsBufferSize = 8192;
    }
}