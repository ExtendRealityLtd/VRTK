namespace VRTK
{
    using UnityEngine;

    public abstract class SDK_InterfaceController : ScriptableObject
    {
        public enum ControllerElelements
        {
            AttachPoint,
            Trigger,
            GripLeft,
            GripRight,
            Touchpad,
            ApplicationMenu,
            SystemMenu,
            Body
        }

        public enum ButtonPressTypes
        {
            Press,
            PressDown,
            PressUp,
            Touch,
            TouchDown,
            TouchUp
        }

        public abstract string GetControllerDefaultColliderPath();
        public abstract string GetControllerElementPath(ControllerElelements element, VRTK_DeviceFinder.ControllerHand hand, bool fullPath = false);
        public abstract uint GetControllerIndex(GameObject controller);
        public abstract GameObject GetControllerByIndex(uint index, bool actual = false);
        public abstract Transform GetControllerOrigin(GameObject controller);
        public abstract GameObject GetControllerLeftHand(bool actual = false);
        public abstract GameObject GetControllerRightHand(bool actual = false);
        public abstract bool IsControllerLeftHand(GameObject controller, bool actual = false);
        public abstract bool IsControllerRightHand(GameObject controller, bool actual = false);

        public abstract GameObject GetControllerRenderModel(GameObject controller);
        public abstract void SetControllerRenderModelWheel(GameObject renderModel, bool state);

        public abstract void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500);

        public abstract Vector3 GetVelocityOnIndex(uint index);
        public abstract Vector3 GetAngularVelocityOnIndex(uint index);

        public abstract Vector2 GetTouchpadAxisOnIndex(uint index);
        public abstract Vector2 GetTriggerAxisOnIndex(uint index);

        public abstract float GetTriggerHairlineDeltaOnIndex(uint index);
        public abstract bool IsTriggerPressedOnIndex(uint index);
        public abstract bool IsTriggerPressedDownOnIndex(uint index);
        public abstract bool IsTriggerPressedUpOnIndex(uint index);
        public abstract bool IsTriggerTouchedOnIndex(uint index);
        public abstract bool IsTriggerTouchedDownOnIndex(uint index);
        public abstract bool IsTriggerTouchedUpOnIndex(uint index);
        public abstract bool IsHairTriggerDownOnIndex(uint index);
        public abstract bool IsHairTriggerUpOnIndex(uint index);

        public abstract bool IsGripPressedOnIndex(uint index);
        public abstract bool IsGripPressedDownOnIndex(uint index);
        public abstract bool IsGripPressedUpOnIndex(uint index);
        public abstract bool IsGripTouchedOnIndex(uint index);
        public abstract bool IsGripTouchedDownOnIndex(uint index);
        public abstract bool IsGripTouchedUpOnIndex(uint index);

        public abstract bool IsTouchpadPressedOnIndex(uint index);
        public abstract bool IsTouchpadPressedDownOnIndex(uint index);
        public abstract bool IsTouchpadPressedUpOnIndex(uint index);
        public abstract bool IsTouchpadTouchedOnIndex(uint index);
        public abstract bool IsTouchpadTouchedDownOnIndex(uint index);
        public abstract bool IsTouchpadTouchedUpOnIndex(uint index);

        public abstract bool IsApplicationMenuPressedOnIndex(uint index);
        public abstract bool IsApplicationMenuPressedDownOnIndex(uint index);
        public abstract bool IsApplicationMenuPressedUpOnIndex(uint index);
        public abstract bool IsApplicationMenuTouchedOnIndex(uint index);
        public abstract bool IsApplicationMenuTouchedDownOnIndex(uint index);
        public abstract bool IsApplicationMenuTouchedUpOnIndex(uint index);
    }
}