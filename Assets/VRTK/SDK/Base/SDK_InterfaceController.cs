namespace VRTK
{
    using UnityEngine;

    public enum VRTK_ControllerElements
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

    public interface SDK_InterfaceController
    {
        string GetControllerDefaultColliderPath();
        string GetControllerElementPath(VRTK_ControllerElements element, VRTK_DeviceFinder.ControllerHand hand, bool fullPath = false);
        uint GetControllerIndex(GameObject controller);
        GameObject GetControllerByIndex(uint index, bool actual = false);
        Transform GetControllerOrigin(GameObject controller);
        GameObject GetControllerLeftHand(bool actual = false);
        GameObject GetControllerRightHand(bool actual = false);
        bool IsControllerLeftHand(GameObject controller, bool actual = false);
        bool IsControllerRightHand(GameObject controller, bool actual = false);

        GameObject GetControllerRenderModel(GameObject controller);
        void SetControllerRenderModelWheel(GameObject renderModel, bool state);

        void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500);

        Vector3 GetVelocityOnIndex(uint index);
        Vector3 GetAngularVelocityOnIndex(uint index);

        Vector2 GetTouchpadAxisOnIndex(uint index);
        Vector2 GetTriggerAxisOnIndex(uint index);

        float GetTriggerHairlineDeltaOnIndex(uint index);
        bool IsTriggerPressedOnIndex(uint index);
        bool IsTriggerPressedDownOnIndex(uint index);
        bool IsTriggerPressedUpOnIndex(uint index);
        bool IsTriggerTouchedOnIndex(uint index);
        bool IsTriggerTouchedDownOnIndex(uint index);
        bool IsTriggerTouchedUpOnIndex(uint index);
        bool IsHairTriggerDownOnIndex(uint index);
        bool IsHairTriggerUpOnIndex(uint index);

        bool IsGripPressedOnIndex(uint index);
        bool IsGripPressedDownOnIndex(uint index);
        bool IsGripPressedUpOnIndex(uint index);
        bool IsGripTouchedOnIndex(uint index);
        bool IsGripTouchedDownOnIndex(uint index);
        bool IsGripTouchedUpOnIndex(uint index);

        bool IsTouchpadPressedOnIndex(uint index);
        bool IsTouchpadPressedDownOnIndex(uint index);
        bool IsTouchpadPressedUpOnIndex(uint index);
        bool IsTouchpadTouchedOnIndex(uint index);
        bool IsTouchpadTouchedDownOnIndex(uint index);
        bool IsTouchpadTouchedUpOnIndex(uint index);

        bool IsApplicationMenuPressedOnIndex(uint index);
        bool IsApplicationMenuPressedDownOnIndex(uint index);
        bool IsApplicationMenuPressedUpOnIndex(uint index);
        bool IsApplicationMenuTouchedOnIndex(uint index);
        bool IsApplicationMenuTouchedDownOnIndex(uint index);
        bool IsApplicationMenuTouchedUpOnIndex(uint index);
    }
}