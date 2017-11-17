// Base Controller|SDK_Base|006
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    public struct VRTKSDKBaseControllerEventArgs
    {
        public VRTK_ControllerReference controllerReference;
    }

    public delegate void VRTKSDKBaseControllerEventHandler(object sender, VRTKSDKBaseControllerEventArgs e);

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
        public enum ButtonTypes
        {
            /// <summary>
            /// Button One on the controller.
            /// </summary>
            ButtonOne,
            /// <summary>
            /// Button Two on the controller.
            /// </summary>
            ButtonTwo,
            /// <summary>
            /// Grip on the controller.
            /// </summary>
            Grip,
            /// <summary>
            /// Grip Hairline on the controller.
            /// </summary>
            GripHairline,
            /// <summary>
            /// Start Menu on the controller.
            /// </summary>
            StartMenu,
            /// <summary>
            /// Trigger on the controller.
            /// </summary>
            Trigger,
            /// <summary>
            /// Trigger Hairline on the controller.
            /// </summary>
            TriggerHairline,
            /// <summary>
            /// Touchpad on the controller.
            /// </summary>
            Touchpad,
            /// <summary>
            /// Touchpad Two on the controller.
            /// </summary>
            TouchpadTwo,
            /// <summary>
            /// Middle Finger on the controller.
            /// </summary>
            MiddleFinger,
            /// <summary>
            /// Ring Finger on the controller.
            /// </summary>
            RingFinger,
            /// <summary>
            /// Pinky Finger on the controller.
            /// </summary>
            PinkyFinger
        }

        /// <summary>
        /// Concepts of controller button press
        /// </summary>
        public enum ButtonPressTypes
        {
            /// <summary>
            /// The button is currently being pressed.
            /// </summary>
            Press,
            /// <summary>
            /// The button has just been pressed down.
            /// </summary>
            PressDown,
            /// <summary>
            /// The button has just been released.
            /// </summary>
            PressUp,
            /// <summary>
            /// The button is currently being touched.
            /// </summary>
            Touch,
            /// <summary>
            /// The button has just been touched.
            /// </summary>
            TouchDown,
            /// <summary>
            /// The button is no longer being touched.
            /// </summary>
            TouchUp
        }

        /// <summary>
        /// The elements of a generic controller
        /// </summary>
        public enum ControllerElements
        {
            /// <summary>
            /// The default point on the controller to attach grabbed objects to.
            /// </summary>
            AttachPoint,
            /// <summary>
            /// The trigger button.
            /// </summary>
            Trigger,
            /// <summary>
            /// The left part of the grip button collection.
            /// </summary>
            GripLeft,
            /// <summary>
            /// The right part of the grip button collection.
            /// </summary>
            GripRight,
            /// <summary>
            /// The touch pad/stick.
            /// </summary>
            Touchpad,
            /// <summary>
            /// The first generic button.
            /// </summary>
            ButtonOne,
            /// <summary>
            /// The second generic button.
            /// </summary>
            ButtonTwo,
            /// <summary>
            /// The system menu button.
            /// </summary>
            SystemMenu,
            /// <summary>
            /// The encompassing mesh of the controller body.
            /// </summary>
            Body,
            /// <summary>
            /// The start menu button.
            /// </summary>
            StartMenu
        }

        /// <summary>
        /// Controller hand reference.
        /// </summary>
        public enum ControllerHand
        {
            /// <summary>
            /// No hand is assigned.
            /// </summary>
            None,
            /// <summary>
            /// The left hand is assigned.
            /// </summary>
            Left,
            /// <summary>
            /// The right hand is assigned.
            /// </summary>
            Right
        }

        /// <summary>
        /// SDK Controller types.
        /// </summary>
        public enum ControllerType
        {
            /// <summary>
            /// No controller type.
            /// </summary>
            Undefined,
            /// <summary>
            /// A custom controller type.
            /// </summary>
            Custom,
            /// <summary>
            /// The Simulator default hand controller.
            /// </summary>
            Simulator_Hand,
            /// <summary>
            /// The HTC Vive wand controller for SteamVR.
            /// </summary>
            SteamVR_ViveWand,
            /// <summary>
            /// The Oculus Touch controller for SteamVR.
            /// </summary>
            SteamVR_OculusTouch,
            /// <summary>
            /// The Oculus Touch controller for Oculus Utilities.
            /// </summary>
            Oculus_OculusTouch,
            /// <summary>
            /// The Daydream controller for Google Daydream SDK.
            /// </summary>
            Daydream_Controller,
            /// <summary>
            /// The Flip controller for Ximmerse SDK.
            /// </summary>
            Ximmerse_Flip,
            /// <summary>
            /// The Valve Knuckles controller for SteamVR.
            /// </summary>
            SteamVR_ValveKnuckles,
            /// <summary>
            /// The Oculus Gamepad for Oculus Utilities.
            /// </summary>
            Oculus_OculusGamepad,
            /// <summary>
            /// The Oculus Remote for Oculus Utilities.
            /// </summary>
            Oculus_OculusRemote,
            /// <summary>
            /// The Oculus GearVR HMD controls for Oculus Utilities.
            /// </summary>
            Oculus_GearVRHMD,
            /// <summary>
            /// The Oculus GearVR controller for Oculus Utilities.
            /// </summary>
            Oculus_GearVRController,
            /// <summary>
            /// The Windows Mixed Reality Motion Controller for Windows Mixed Reality.
            /// </summary>
            WindowsMR_MotionController,
            /// <summary>
            /// The Windows Mixed Reality Motion Controller for SteamVR.
            /// </summary>
            SteamVR_WindowsMRController
        }

        public event VRTKSDKBaseControllerEventHandler LeftControllerReady;
        public event VRTKSDKBaseControllerEventHandler RightControllerReady;
        public event VRTKSDKBaseControllerEventHandler LeftControllerModelReady;
        public event VRTKSDKBaseControllerEventHandler RightControllerModelReady;

        protected Transform defaultSDKLeftControllerModel = null;
        protected Transform defaultSDKRightControllerModel = null;

        public virtual void OnControllerReady(ControllerHand hand)
        {
            VRTKSDKBaseControllerEventArgs e;
            e.controllerReference = VRTK_ControllerReference.GetControllerReference(hand);

            switch (hand)
            {
                case ControllerHand.Left:
                    if (LeftControllerReady != null)
                    {
                        LeftControllerReady(this, e);
                    }
                    break;
                case ControllerHand.Right:
                    if (RightControllerReady != null)
                    {
                        RightControllerReady(this, e);
                    }
                    break;
            }
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
        /// <param name="controllerReference">The reference to the controller to get type of.</param>
        /// <returns>The ControllerType based on the SDK and headset being used.</returns>
        public abstract ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null);

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
        [System.Obsolete("GenerateControllerPointerOrigin has been deprecated and will be removed in a future version of VRTK.")]
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
        /// The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.
        /// </summary>
        /// <param name="hand">The hand to determine if the controller model will be ready for.</param>
        /// <returns>Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.</returns>
        public abstract bool WaitForControllerModel(ControllerHand hand);

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
        /// The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the sense axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the sense axis on.</param>
        /// <returns>The current sense axis value.</returns>
        public abstract float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);

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
            if (sdkManager != null && sdkManager.loadedSetup != null)
            {
                return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
            }
            return null;
        }

        protected virtual GameObject GetSDKManagerControllerRightHand(bool actual = false)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.loadedSetup != null)
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
            if (sdkManager != null && sdkManager.loadedSetup != null && controller != null)
            {
                return (actual ? controller == sdkManager.loadedSetup.actualLeftController : controller == sdkManager.scriptAliasLeftController);
            }
            return false;
        }

        protected virtual bool CheckControllerRightHand(GameObject controller, bool actual)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.loadedSetup != null && controller != null)
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
            if (sdkManager != null && sdkManager.loadedSetup != null)
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
            if (sdkManager != null && sdkManager.loadedSetup != null)
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

        protected virtual void OnControllerModelReady(ControllerHand hand, VRTK_ControllerReference controllerReference)
        {
            VRTKSDKBaseControllerEventArgs e;
            e.controllerReference = controllerReference;

            switch (hand)
            {
                case ControllerHand.Left:
                    if (LeftControllerModelReady != null)
                    {
                        LeftControllerModelReady(this, e);
                    }
                    break;
                case ControllerHand.Right:
                    if (RightControllerModelReady != null)
                    {
                        RightControllerModelReady(this, e);
                    }
                    break;
            }
        }

        protected virtual bool ShouldWaitForControllerModel(ControllerHand hand, bool ignoreChildCount)
        {
            //If the default model isn't set or the current controller model isn't the default controller model, then don't bother waiting for the model to stream in.
            switch (hand)
            {
                case ControllerHand.Left:
                    return IsDefaultControllerModel(defaultSDKLeftControllerModel, GetControllerModel(ControllerHand.Left), ignoreChildCount);
                case ControllerHand.Right:
                    return IsDefaultControllerModel(defaultSDKRightControllerModel, GetControllerModel(ControllerHand.Right), ignoreChildCount);
            }
            return false;
        }

        protected virtual bool IsDefaultControllerModel(Transform givenDefault, GameObject givenActual, bool ignoreChildCount)
        {
            return (givenDefault != null && givenActual == givenDefault.gameObject && givenActual != null && (ignoreChildCount || givenActual.transform.childCount == 0));
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