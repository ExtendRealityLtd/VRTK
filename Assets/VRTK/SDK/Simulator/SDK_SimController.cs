// Simulator Controller|SDK_Simulator|003
namespace VRTK
{
#if VRTK_SDK_SIM
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Sim Controller SDK script provides functions to help simulate VR controllers.
    /// </summary>
    public class SDK_SimController : SDK_BaseController
    {
        private SimControllers controllers;
        private Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>()
        {
            {"Trigger", KeyCode.Mouse1 },
            {"Grip", KeyCode.Mouse0 },
            {"TouchpadPress", KeyCode.Q },
            {"ButtonOne", KeyCode.E },
            {"ButtonTwo", KeyCode.R },
            {"StartMenu", KeyCode.F },
            {"TouchModifier", KeyCode.T},
            {"HairTouchModifier", KeyCode.H}
        };

        protected const string RIGHT_HAND_CONTROLLER_NAME = "RightHand";
        protected const string LEFT_HAND_CONTROLLER_NAME = "LeftHand";

        public virtual void SetKeyMappings(Dictionary<string, KeyCode> givenKeyMappings)
        {
            keyMappings = givenKeyMappings;
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(uint index, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/Simulator";
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
            var suffix = (fullPath ? "/attach" : "");
            switch (element)
            {
                case ControllerElements.AttachPoint:
                    return "";
                case ControllerElements.Trigger:
                    return "" + suffix;
                case ControllerElements.GripLeft:
                    return "" + suffix;
                case ControllerElements.GripRight:
                    return "" + suffix;
                case ControllerElements.Touchpad:
                    return "" + suffix;
                case ControllerElements.ButtonOne:
                    return "" + suffix;
                case ControllerElements.SystemMenu:
                    return "" + suffix;
                case ControllerElements.Body:
                    return "";
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
            uint index = 0;

            switch (controller.name)
            {
                case "Camera":
                    index = 0;
                    break;
                case "RightController":
                    index = 1;
                    break;
                case "LeftController":
                    index = 2;
                    break;
            }
            return index;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            switch (index)
            {
                case 1:
                    return controllers.rightHand.gameObject;
                case 2:
                    return controllers.leftHand.gameObject;
                default:
                    return null;
            }
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(GameObject controller)
        {
            return controller.transform;
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
            // use the basic base functionality to find the left hand controller
            var controller = GetSDKManagerControllerLeftHand(actual);
            // if the controller cannot be found with default settings, try finding it below the InputSimulator by name
            if (!controller && actual)
            {
                controller = GetActualController(ControllerHand.Left);
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
            // use the basic base functionality to find the right hand controller
            var controller = GetSDKManagerControllerRightHand(actual);
            // if the controller cannot be found with default settings, try finding it below the InputSimulator by name
            if (!controller && actual)
            {
                controller = GetActualController(ControllerHand.Right);
            }

            return controller;
        }

        /// <summary>
        /// finds the actual controller for the specified hand (identified by name) and returns it
        /// </summary>
        /// <param name="hand">the for which to find the respective controller gameobject</param>
        /// <returns>the gameobject of the actual controller corresponding to the specified hand</returns>
        private static GameObject GetActualController(ControllerHand hand)
        {
            GameObject simPlayer = SDK_InputSimulator.FindInScene();
            GameObject controller = null;

            if (simPlayer != null)
            {
                switch (hand)
                {
                    case ControllerHand.Right:
                        controller = simPlayer.transform.FindChild(RIGHT_HAND_CONTROLLER_NAME).gameObject;
                        break;
                    case ControllerHand.Left:
                        controller = simPlayer.transform.FindChild(LEFT_HAND_CONTROLLER_NAME).gameObject;
                        break;
                    default:
                        break;
                }
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
            GameObject model = null;
            GameObject simPlayer = SDK_InputSimulator.FindInScene();
            if (simPlayer)
            {
                switch (hand)
                {
                    case ControllerHand.Left:
                        model = simPlayer.transform.FindChild(string.Format("{0}/Hand", LEFT_HAND_CONTROLLER_NAME)).gameObject;
                        break;
                    case ControllerHand.Right:
                        model = simPlayer.transform.FindChild(string.Format("{0}/Hand", RIGHT_HAND_CONTROLLER_NAME)).gameObject;
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
            return controller.transform.parent.FindChild("Hand").gameObject;
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
        /// The GetVelocityOnIndex method is used to determine the current velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocityOnIndex(uint index)
        {
            switch (index)
            {
                case 1:
                    return controllers.rightController.GetVelocity();
                case 2:
                    return controllers.leftController.GetVelocity();
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            switch (index)
            {
                case 1:
                    return controllers.rightController.GetAngularVelocity();
                case 2:
                    return controllers.leftController.GetAngularVelocity();
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// The GetTouchpadAxisOnIndex method is used to get the current touch position on the controller touchpad.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current x,y position of where the touchpad is being touched.</returns>
        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        /// <summary>
        /// The GetTriggerAxisOnIndex method is used to get the current trigger position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the trigger.</returns>
        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        /// <summary>
        /// The GetGripAxisOnIndex method is used to get the current grip position on the controller.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector2 containing the current position of the grip.</returns>
        public override Vector2 GetGripAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        /// <summary>
        /// The GetTriggerHairlineDeltaOnIndex method is used to get the difference between the current trigger press and the previous frame trigger press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the trigger presses.</returns>
        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return 0;
        }

        /// <summary>
        /// The GetGripHairlineDeltaOnIndex method is used to get the difference between the current grip press and the previous frame grip press.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>The delta between the grip presses.</returns>
        public override float GetGripHairlineDeltaOnIndex(uint index)
        {
            return 0f;
        }

        /// <summary>
        /// The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTriggerPressedOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsTriggerTouchedOnIndex(index);
        }

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsTriggerTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsTriggerTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["Trigger"]);
        }

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["Trigger"]);
        }

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["Trigger"]);
        }

        /// <summary>
        /// The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            // button hair touches shall be ignored if the only the touch modifier is used
            return !IsButtonHairTouchIgnored() && IsTriggerTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            // button hair touches shall be ignored if the only the touch modifier is used
            return !IsButtonHairTouchIgnored() && IsTriggerTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsGripPressedOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsGripTouchedOnIndex(index);
        }

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsGripPressedDownOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsGripTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripPressedUpOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsGripTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsGripTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["Grip"]);
        }

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["Grip"]);
        }

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["Grip"]);
        }

        /// <summary>
        /// The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairGripDownOnIndex(uint index)
        {
            // button hair touches shall be ignored if the hair touch modifier is used
            return !IsButtonHairTouchIgnored() && IsGripTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairGripUpOnIndex(uint index)
        {
            // button hair touches shall be ignored if the hair touch modifier is used
            return !IsButtonHairTouchIgnored() && IsGripTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsTouchpadTouchedOnIndex(index);
        }

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsTouchpadTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsTouchpadTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["TouchpadPress"]);
        }

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["TouchpadPress"]);
        }

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["TouchpadPress"]);
        }

        /// <summary>
        /// The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonOnePressedOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsButtonOneTouchedOnIndex(index);
        }

        /// <summary>
        /// The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonOnePressedDownOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsButtonOneTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOnePressedUpOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsButtonOneTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonOneTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["ButtonOne"]);
        }

        /// <summary>
        /// The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonOneTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["ButtonOne"]);
        }

        /// <summary>
        /// The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOneTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["ButtonOne"]);
        }

        /// <summary>
        /// The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonTwoPressedOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsButtonTwoTouchedOnIndex(index);
        }

        /// <summary>
        /// The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonTwoPressedDownOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsButtonTwoTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoPressedUpOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsButtonTwoTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonTwoTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["ButtonTwo"]);
        }

        /// <summary>
        /// The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonTwoTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["ButtonTwo"]);
        }

        /// <summary>
        /// The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["ButtonTwo"]);
        }

        /// <summary>
        /// The IsStartMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsStartMenuPressedOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsStartMenuTouchedOnIndex(index);
        }

        /// <summary>
        /// The IsStartMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsStartMenuPressedDownOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsStartMenuTouchedDownOnIndex(index);
        }

        /// <summary>
        /// The IsStartMenuPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsStartMenuPressedUpOnIndex(uint index)
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return !IsButtonPressIgnored() && IsStartMenuTouchedUpOnIndex(index);
        }

        /// <summary>
        /// The IsStartMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsStartMenuTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings["StartMenu"]);
        }

        /// <summary>
        /// The IsStartMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsStartMenuTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings["StartMenu"]);
        }

        /// <summary>
        /// The IsStartMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsStartMenuTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings["StartMenu"]);
        }

        private void OnEnable()
        {
            controllers = new SimControllers();
        }

        /// <summary>
        /// whether or not the touch modifier is currently pressed
        /// if so, pressing a key on the keyboard will only emit touch events,
        /// but not a real press (or hair touch events).
        /// </summary>
        /// <returns>whether or not the TouchModifier is active</returns>
        protected bool IsTouchModifierPressed()
        {
            return Input.GetKey(keyMappings["TouchModifier"]);
        }

        /// <summary>
        /// whether or not the hair touch modifier is currently pressed
        /// if so, pressing a key on the keyboard will only emit touch and hair touch events,
        /// but not a real press.
        /// </summary>
        /// <returns>whether or not the HairTouchModifier is active</returns>
        protected bool IsHairTouchModifierPressed()
        {
            return Input.GetKey(keyMappings["HairTouchModifier"]);
        }

        /// <summary>
        /// whether or not a button press shall be ignored, e.g. because of the
        /// use of the touch or hair touch modifier
        /// </summary>
        /// <returns>Returns true if the button press is ignored.</returns>
        protected bool IsButtonPressIgnored()
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return IsHairTouchModifierPressed() || IsTouchModifierPressed();
        }

        /// <summary>
        /// whether or not a button press shall be ignored, e.g. because of the
        /// use of the touch or hair touch modifier
        /// </summary>
        /// <returns>Returns true if the hair trigger touch should be ignored.</returns>
        protected bool IsButtonHairTouchIgnored()
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return IsTouchModifierPressed() && !IsHairTouchModifierPressed();
        }

        /// <summary>
        /// checks if the given button (KeyCode) is currently in a specific pressed state (ButtonPressTypes) on the keyboard
        /// also asserts that button presses are only handled for the currently active controller by comparing the controller indices
        /// </summary>
        /// <param name="index">unique index of the controller for which the button press is to be checked</param>
        /// <param name="type">the type of press (up, down, hold)</param>
        /// <param name="button">the button on the keyboard</param>
        /// <returns>Returns true if the button is being pressed.</returns>
        private bool IsButtonPressed(uint index, ButtonPressTypes type, KeyCode button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }

            if (index == 1)
            {
                if (!controllers.rightController.Selected)
                {
                    return false;
                }
            }
            else if (index == 2)
            {
                if (!controllers.leftController.Selected)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


            switch (type)
            {
                case ButtonPressTypes.Press:
                    return Input.GetKey(button);
                case ButtonPressTypes.PressDown:
                    return Input.GetKeyDown(button);
                case ButtonPressTypes.PressUp:
                    return Input.GetKeyUp(button);
            }
            return false;
        }

        private class SimControllers
        {
            public Transform rightHand;
            public Transform leftHand;
            public SDK_ControllerSim rightController;
            public SDK_ControllerSim leftController;
            public SimControllers()
            {
                GameObject simPlayer = SDK_InputSimulator.FindInScene();
                if (simPlayer)
                {
                    rightHand = simPlayer.transform.FindChild(RIGHT_HAND_CONTROLLER_NAME);
                    leftHand = simPlayer.transform.FindChild(LEFT_HAND_CONTROLLER_NAME);
                    rightController = rightHand.GetComponent<SDK_ControllerSim>();
                    leftController = leftHand.GetComponent<SDK_ControllerSim>();
                }
            }
        }
    }
#else
    public class SDK_SimController : SDK_FallbackController
    {
    }
#endif
}
