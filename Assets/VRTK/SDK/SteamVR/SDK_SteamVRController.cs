// SteamVR Controller|SDK|003
namespace VRTK
{
    using UnityEngine;
#if VRTK_SDK_CONTROLLER_STEAMVR
    using Valve.VR;
#endif

    /// <summary>
    /// The SteamVR Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    public class SDK_SteamVRController : SDK_BaseController
    {
#if VRTK_SDK_CONTROLLER_STEAMVR
        private SteamVR_ControllerManager cachedControllerManager;

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(ControllerElelements element, VRTK_DeviceFinder.ControllerHand hand)
        {
            switch (element)
            {
                case ControllerElelements.AttachPoint:
                    return "Model/tip/attach";
                case ControllerElelements.Trigger:
                    return "Model/trigger";
                case ControllerElelements.GripLeft:
                    return "Model/lgrip";
                case ControllerElelements.GripRight:
                    return "Model/rgrip";
                case ControllerElelements.Touchpad:
                    return "Model/trackpad";
                case ControllerElelements.ApplicationMenu:
                    return "Model/button";
                case ControllerElelements.SystemMenu:
                    return "Model/sys_button";
                case ControllerElelements.Body:
                    return "Model/body";
            }
            return null;
        }

        /// <summary>
        /// The GetTrackedObject method checks to see if the given game object is a tracked object and returns the game object back if it is valid along with it's tracked index.
        /// </summary>
        /// <param name="obj">The GameObject to check on.</param>
        /// <param name="index">The variable to store the tracked index in if one is found.</param>
        /// <returns>A GameObject of the tracked object if one is found.</returns>
        public override GameObject GetTrackedObject(GameObject obj, out uint index)
        {
            var trackedObject = obj.GetComponent<SteamVR_TrackedObject>();
            index = 0;
            if (trackedObject)
            {
                index = (uint)trackedObject.index;
                return trackedObject.gameObject;
            }
            return null;
        }

        /// <summary>
        /// The GetTrackedObjectByIndex method retrieves a tracked object game object by it's index.
        /// </summary>
        /// <param name="index">The index of the tracked object to look up.</param>
        /// <returns>The GameObject of the found tracked object.</returns>
        public override GameObject GetTrackedObjectByIndex(uint index)
        {
            //attempt to get from cache first
            if (VRTK_ObjectCache.trackedControllers.ContainsKey(index))
            {
                return VRTK_ObjectCache.trackedControllers[index];
            }

            //if not found in cache then brute force check
            foreach (SteamVR_TrackedObject trackedObject in FindObjectsOfType<SteamVR_TrackedObject>())
            {
                if ((uint)trackedObject.index == index)
                {
                    return trackedObject.gameObject;
                }
            }

            return null;
        }

        /// <summary>
        /// The GetIndexOfTrackedObject method returns the tracked index of the given tracked object.
        /// </summary>
        /// <param name="trackedObject">The GameObject containing the tracked object.</param>
        /// <returns>The tracked index of the given tracked object.</returns>
        public override uint GetIndexOfTrackedObject(GameObject trackedObject)
        {
            uint index = uint.MaxValue;
            GetTrackedObject(trackedObject, out index);
            return index;
        }

        /// <summary>
        /// The GetTrackedObjectOrigin method returns the origin of the given game object if it is a tracked object.
        /// </summary>
        /// <param name="obj">The GameObject to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the tracked object.</returns>
        public override Transform GetTrackedObjectOrigin(GameObject obj)
        {
            var trackedObject = obj.GetComponent<SteamVR_TrackedObject>();
            if (trackedObject)
            {
                return trackedObject.origin ? trackedObject.origin : trackedObject.transform.parent;
            }
            return null;
        }

        /// <summary>
        /// The TrackedIndexIsController method is used to determine if the given tracked index is a controller.
        /// </summary>
        /// <param name="index">The tracked index to check for.</param>
        /// <returns>Returns true if the tracked object found is a controller.</returns>
        public override bool TrackedIndexIsController(uint index)
        {
            var system = OpenVR.System;
            if (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The GetControllerLeftHand method returns the game object containing the representation of the left hand controller.
        /// </summary>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand()
        {
            var controllerManager = GetControllerManager();
            if (controllerManager)
            {
                return controllerManager.left;
            }
            return null;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the game object containing the representation of the right hand controller.
        /// </summary>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand()
        {
            var controllerManager = GetControllerManager();
            if (controllerManager)
            {
                return controllerManager.right;
            }
            return null;
        }

        /// <summary>
        /// The IsControllerLeftHand method is used to check if the given game object is the game object containing the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given game object is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            var controllerManager = GetControllerManager();
            if (controllerManager && controller == controllerManager.left)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The IsControllerRightHand method is used to check if the given game object is the game object containing the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given game object is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            var controllerManager = GetControllerManager();
            if (controllerManager && controller == controllerManager.right)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            var renderModel = (controller.GetComponent<SteamVR_RenderModel>() ? controller.GetComponent<SteamVR_RenderModel>() : controller.GetComponentInChildren<SteamVR_RenderModel>());
            return (renderModel ? renderModel.gameObject : null);
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            var model = renderModel.GetComponent<SteamVR_RenderModel>();
            if (model)
            {
                model.controllerModeState.bScrollWheelVisible = state;
            }
        }

        /// <summary>
        /// The HapticPulseOnIndex method is used to initiate a simple haptic pulse on the tracked object of the given index.
        /// </summary>
        /// <param name="index">The index of the tracked object to initiate the haptic pulse on.</param>
        /// <param name="durationMicroSec">The amount of microseconds to run the haptic pulse for.</param>
        public override void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
            if (index < uint.MaxValue)
            {
                var device = SteamVR_Controller.Input((int)index);
                device.TriggerHapticPulse(durationMicroSec, EVRButtonId.k_EButton_Axis0);
            }
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
            var device = SteamVR_Controller.Input((int)index);
            return device.velocity;
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
            var device = SteamVR_Controller.Input((int)index);
            return device.angularVelocity;
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
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis();
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
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
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
            var device = SteamVR_Controller.Input((int)index);
            return device.hairTriggerDelta;
        }

        /// <summary>
        /// The IsTriggerPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        /// <summary>
        /// The IsTriggerTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Trigger);
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
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerDown();
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
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerUp();
        }

        /// <summary>
        /// The IsGripPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsGripPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsGripTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsGripTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Grip);
        }

        /// <summary>
        /// The IsTouchpadPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsTouchpadTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        /// <summary>
        /// The IsApplicationMenuPressedOnIndex method is used to determine if the controller button is being pressed down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being pressed.</returns>
        public override bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuPressedDownOnIndex method is used to determine if the controller button has just been pressed down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been pressed down.</returns>
        public override bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuPressedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuTouchedOnIndex method is used to determine if the controller button is being touched down continually.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button is continually being touched.</returns>
        public override bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuTouchedDownOnIndex method is used to determine if the controller button has just been touched down.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been touched down.</returns>
        public override bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        /// <summary>
        /// The IsApplicationMenuTouchedUpOnIndex method is used to determine if the controller button has just been released.
        /// </summary>
        /// <param name="index">The index of the tracked object to check for.</param>
        /// <returns>Returns true if the button has just been released.</returns>
        public override bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        [RuntimeInitializeOnLoadMethod]
        private void Initialise()
        {
            SteamVR_Utils.Event.Listen("TrackedDeviceRoleChanged", OnTrackedDeviceRoleChanged);
        }

        private void OnTrackedDeviceRoleChanged(params object[] args)
        {
            cachedControllerManager = null;
            VRTK_ObjectCache.trackedControllers.Clear();
        }

        private SteamVR_ControllerManager GetControllerManager()
        {
            if (cachedControllerManager == null || !cachedControllerManager.isActiveAndEnabled)
            {
                foreach (var manager in FindObjectsOfType<SteamVR_ControllerManager>())
                {
                    if (manager.left && manager.right)
                    {
                        cachedControllerManager = manager;
                    }
                }
            }
            return cachedControllerManager;
        }

        private static bool IsButtonPressed(uint index, ButtonPressTypes type, ulong button)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);

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
#endif
    }
}