// Simulator Controller|SDK_Simulator|003
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Sim Controller SDK script provides functions to help simulate VR controllers.
    /// </summary>
    [SDK_Description(typeof(SDK_SimSystem))]
    public class SDK_SimController : SDK_BaseController
    {
        protected SDK_ControllerSim rightController;
        protected SDK_ControllerSim leftController;
        protected Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>()
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
            string suffix = (fullPath ? "/attach" : "");
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
            if (CheckActualOrScriptAliasControllerIsRightHand(controller))
            {
                return 1;
            }
            if (CheckActualOrScriptAliasControllerIsLeftHand(controller))
            {
                return 2;
            }
            return uint.MaxValue;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            switch (index)
            {
                case 1:
                    return (sdkManager != null && !actual ? sdkManager.scriptAliasRightController : rightController.gameObject);
                case 2:
                    return (sdkManager != null && !actual ? sdkManager.scriptAliasLeftController : leftController.gameObject);
                default:
                    return null;
            }
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to retrieve the origin from.</param>
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
            GameObject controller = GetSDKManagerControllerLeftHand(actual);
            // if the controller cannot be found with default settings, try finding it below the InputSimulator by name
            if (controller == null && actual)
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
            GameObject controller = GetSDKManagerControllerRightHand(actual);
            // if the controller cannot be found with default settings, try finding it below the InputSimulator by name
            if (controller == null && actual)
            {
                controller = GetActualController(ControllerHand.Right);
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
            if (simPlayer != null)
            {
                switch (hand)
                {
                    case ControllerHand.Left:
                        model = simPlayer.transform.Find(string.Format("{0}/Hand", LEFT_HAND_CONTROLLER_NAME)).gameObject;
                        break;
                    case ControllerHand.Right:
                        model = simPlayer.transform.Find(string.Format("{0}/Hand", RIGHT_HAND_CONTROLLER_NAME)).gameObject;
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
            return controllerReference.scriptAlias.transform.parent.Find("Hand").gameObject;
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            switch (index)
            {
                case 1:
                    return rightController.GetVelocity();
                case 2:
                    return leftController.GetVelocity();
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            switch (index)
            {
                case 1:
                    return rightController.GetAngularVelocity();
                case 2:
                    return leftController.GetAngularVelocity();
                default:
                    return Vector3.zero;
            }
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
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                case ButtonTypes.TriggerHairline:
                    return GetControllerButtonState(index, "Trigger", pressType);
                case ButtonTypes.Grip:
                case ButtonTypes.GripHairline:
                    return GetControllerButtonState(index, "Grip", pressType);
                case ButtonTypes.Touchpad:
                    return GetControllerButtonState(index, "TouchpadPress", pressType);
                case ButtonTypes.ButtonOne:
                    return GetControllerButtonState(index, "ButtonOne", pressType);
                case ButtonTypes.ButtonTwo:
                    return GetControllerButtonState(index, "ButtonTwo", pressType);
                case ButtonTypes.StartMenu:
                    return GetControllerButtonState(index, "StartMenu", pressType);
            }
            return false;
        }

        protected virtual void OnEnable()
        {
            GameObject simPlayer = SDK_InputSimulator.FindInScene();
            if (simPlayer != null)
            {
                rightController = simPlayer.transform.Find(RIGHT_HAND_CONTROLLER_NAME).GetComponent<SDK_ControllerSim>();
                leftController = simPlayer.transform.Find(LEFT_HAND_CONTROLLER_NAME).GetComponent<SDK_ControllerSim>();
            }
        }

        /// <summary>
        /// whether or not the touch modifier is currently pressed
        /// if so, pressing a key on the keyboard will only emit touch events,
        /// but not a real press (or hair touch events).
        /// </summary>
        /// <returns>whether or not the TouchModifier is active</returns>
        protected virtual bool IsTouchModifierPressed()
        {
            return Input.GetKey(keyMappings["TouchModifier"]);
        }

        /// <summary>
        /// whether or not the hair touch modifier is currently pressed
        /// if so, pressing a key on the keyboard will only emit touch and hair touch events,
        /// but not a real press.
        /// </summary>
        /// <returns>whether or not the HairTouchModifier is active</returns>
        protected virtual bool IsHairTouchModifierPressed()
        {
            return Input.GetKey(keyMappings["HairTouchModifier"]);
        }

        /// <summary>
        /// whether or not a button press shall be ignored, e.g. because of the
        /// use of the touch or hair touch modifier
        /// </summary>
        /// <returns>Returns true if the button press is ignored.</returns>
        protected virtual bool IsButtonPressIgnored()
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return IsHairTouchModifierPressed() || IsTouchModifierPressed();
        }

        /// <summary>
        /// whether or not a button press shall be ignored, e.g. because of the
        /// use of the touch or hair touch modifier
        /// </summary>
        /// <returns>Returns true if the hair trigger touch should be ignored.</returns>
        protected virtual bool IsButtonHairTouchIgnored()
        {
            // button presses shall be ignored if the hair touch or touch modifiers are used
            return IsTouchModifierPressed() && !IsHairTouchModifierPressed();
        }

        /// <summary>
        /// Gets the state of the given button key mapping for the press type on the controller index.
        /// </summary>
        /// <param name="index">The index of the controller.</param>
        /// <param name="keyMapping">The key mapping key to check.</param>
        /// <param name="pressType">The type of button press to check.</param>
        /// <returns>Returns true if the button state matches the given data.</returns>
        protected virtual bool GetControllerButtonState(uint index, string keyMapping, ButtonPressTypes pressType)
        {
            if (pressType == ButtonPressTypes.Touch)
            {
                return IsButtonPressed(index, ButtonPressTypes.Press, keyMappings[keyMapping]);
            }
            else if (pressType == ButtonPressTypes.TouchDown)
            {
                return IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings[keyMapping]);
            }
            else if (pressType == ButtonPressTypes.TouchUp)
            {
                return IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings[keyMapping]);
            }
            else if (pressType == ButtonPressTypes.Press)
            {
                return !IsButtonPressIgnored() && IsButtonPressed(index, ButtonPressTypes.Press, keyMappings[keyMapping]);
            }
            else if (pressType == ButtonPressTypes.PressDown)
            {
                return !IsButtonPressIgnored() && IsButtonPressed(index, ButtonPressTypes.PressDown, keyMappings[keyMapping]);
            }
            else if (pressType == ButtonPressTypes.PressUp)
            {
                return !IsButtonPressIgnored() && IsButtonPressed(index, ButtonPressTypes.PressUp, keyMappings[keyMapping]);
            }
            return false;
        }

        /// <summary>
        /// checks if the given button (KeyCode) is currently in a specific pressed state (ButtonPressTypes) on the keyboard
        /// also asserts that button presses are only handled for the currently active controller by comparing the controller indices
        /// </summary>
        /// <param name="index">unique index of the controller for which the button press is to be checked</param>
        /// <param name="type">the type of press (up, down, hold)</param>
        /// <param name="button">the button on the keyboard</param>
        /// <returns>Returns true if the button is being pressed.</returns>
        protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, KeyCode button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }

            if (index == 1)
            {
                if (!rightController.Selected)
                {
                    return false;
                }
            }
            else if (index == 2)
            {
                if (!leftController.Selected)
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

        /// <summary>
        /// finds the actual controller for the specified hand (identified by name) and returns it
        /// </summary>
        /// <param name="hand">the for which to find the respective controller gameobject</param>
        /// <returns>the gameobject of the actual controller corresponding to the specified hand</returns>
        protected virtual GameObject GetActualController(ControllerHand hand)
        {
            GameObject simPlayer = SDK_InputSimulator.FindInScene();
            GameObject controller = null;

            if (simPlayer != null)
            {
                switch (hand)
                {
                    case ControllerHand.Right:
                        controller = simPlayer.transform.Find(RIGHT_HAND_CONTROLLER_NAME).gameObject;
                        break;
                    case ControllerHand.Left:
                        controller = simPlayer.transform.Find(LEFT_HAND_CONTROLLER_NAME).gameObject;
                        break;
                    default:
                        break;
                }
            }

            return controller;
        }
    }
}