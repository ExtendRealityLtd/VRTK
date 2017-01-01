// Fallback Controller|SDK_Fallback|003
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// The Base Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    /// <remarks>
    /// This is the fallback class that will just return default values.
    /// </remarks>
    public class SDK_FallbackController : SDK_BaseController
    {
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
            return "";
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
            return "";
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controller)
        {
            return uint.MaxValue;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns></returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controller">The controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(GameObject controller)
        {
            return null;
        }

        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        public override Transform GenerateControllerPointerOrigin()
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
            return null;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            return null;
        }

        /// <summary>
        /// The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            return false;
        }

        /// <summary>
        /// The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            return false;
        }

        /// <summary>
        /// The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return false;
        }

        /// <summary>
        /// The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual)
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
            return null;
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(ControllerHand hand)
        {
            return null;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(GameObject controller)
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
            return Vector3.zero;
        }

        /// <summary>
        /// The GetAngularVelocityOnIndex method is used to determine the current angular velocity of the tracked object on the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            return Vector3.zero;
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
            return 0f;
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
            return false;
        }

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairTriggerDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairTriggerUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsGripPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsGripTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairGripDownOnIndex method is used to determine if the controller button has passed it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has passed it's press threshold.</returns>
        public override bool IsHairGripDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsHairGripUpOnIndex method is used to determine if the controller button has been released from it's press threshold.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released from it's press threshold.</returns>
        public override bool IsHairGripUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOnePressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonOnePressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOnePressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonOnePressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOnePressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOnePressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOneTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonOneTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOneTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonOneTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonOneTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonOneTouchedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsButtonTwoPressedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsButtonTwoPressedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoPressedUpOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsButtonTwoTouchedOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsButtonTwoTouchedDownOnIndex(uint index)
        {
            return false;
        }

        /// <summary>
        /// The IsButtonTwoTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsButtonTwoTouchedUpOnIndex(uint index)
        {
            return false;
        }

        private void Awake()
        {
            Debug.LogError("Fallback Controller SDK is being used. Have you selected a valid Controller SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Controller SDK from the dropdown.");
        }
    }
}