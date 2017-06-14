namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;

    public static class VRTK_SDK_Bridge
    {
        private static SDK_BaseSystem systemSDK = null;
        private static SDK_BaseHeadset headsetSDK = null;
        private static SDK_BaseController controllerSDK = null;
        private static SDK_BaseBoundaries boundariesSDK = null;

        #region Controller Methods

        public static void ControllerProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options = null)
        {
            GetControllerSDK().ProcessUpdate(controllerReference, options);
        }

        public static void ControllerProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options = null)
        {
            GetControllerSDK().ProcessFixedUpdate(controllerReference, options);
        }

        public static SDK_BaseController.ControllerType GetCurrentControllerType()
        {
            return GetControllerSDK().GetCurrentControllerType();
        }

        public static string GetControllerDefaultColliderPath(SDK_BaseController.ControllerHand hand)
        {
            return GetControllerSDK().GetControllerDefaultColliderPath(hand);
        }

        public static string GetControllerElementPath(SDK_BaseController.ControllerElements element, SDK_BaseController.ControllerHand hand, bool fullPath = false)
        {
            return GetControllerSDK().GetControllerElementPath(element, hand, fullPath);
        }

        public static uint GetControllerIndex(GameObject controller)
        {
            return GetControllerSDK().GetControllerIndex(controller);
        }

        public static GameObject GetControllerByIndex(uint index, bool actual)
        {
            return GetControllerSDK().GetControllerByIndex(index, actual);
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetControllerOrigin(controller)` has been replaced with `VRTK_SDK_Bridge.GetControllerOrigin(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static Transform GetControllerOrigin(GameObject controller)
        {
            return GetControllerOrigin(VRTK_ControllerReference.GetControllerReference(controller));
        }

        public static Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetControllerOrigin(controllerReference);
        }

        public static Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return GetControllerSDK().GenerateControllerPointerOrigin(parent);
        }

        public static GameObject GetControllerLeftHand(bool actual)
        {
            return GetControllerSDK().GetControllerLeftHand(actual);
        }

        public static GameObject GetControllerRightHand(bool actual)
        {
            return GetControllerSDK().GetControllerRightHand(actual);
        }

        public static GameObject GetControllerByHand(SDK_BaseController.ControllerHand hand, bool actual)
        {
            switch (hand)
            {
                case SDK_BaseController.ControllerHand.Left:
                    return GetControllerLeftHand(actual);
                case SDK_BaseController.ControllerHand.Right:
                    return GetControllerRightHand(actual);
            }
            return null;
        }

        public static bool IsControllerLeftHand(GameObject controller)
        {
            return GetControllerSDK().IsControllerLeftHand(controller);
        }

        public static bool IsControllerRightHand(GameObject controller)
        {
            return GetControllerSDK().IsControllerRightHand(controller);
        }

        public static bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return GetControllerSDK().IsControllerLeftHand(controller, actual);
        }

        public static bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return GetControllerSDK().IsControllerRightHand(controller, actual);
        }

        public static GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerSDK().GetControllerModel(controller);
        }

        public static SDK_BaseController.ControllerHand GetControllerModelHand(GameObject controllerModel)
        {
            return GetControllerSDK().GetControllerModelHand(controllerModel);
        }

        public static GameObject GetControllerModel(SDK_BaseController.ControllerHand hand)
        {
            return GetControllerSDK().GetControllerModel(hand);
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetControllerRenderModel(controller)` has been replaced with `VRTK_SDK_Bridge.GetControllerRenderModel(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static GameObject GetControllerRenderModel(GameObject controller)
        {
            return GetControllerRenderModel(VRTK_ControllerReference.GetControllerReference(controller));
        }

        public static GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetControllerRenderModel(controllerReference);
        }

        public static void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            GetControllerSDK().SetControllerRenderModelWheel(renderModel, state);
        }

        [System.Obsolete("`VRTK_SDK_Bridge.HapticPulseOnIndex(index, strength)` has been replaced with `VRTK_SDK_Bridge.HapticPulse(controllerReference, strength)`. This method will be removed in a future version of VRTK.")]
        public static void HapticPulseOnIndex(uint index, float strength = 0.5f)
        {
            HapticPulse(VRTK_ControllerReference.GetControllerReference(index), strength);
        }

        public static void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)
        {
            GetControllerSDK().HapticPulse(controllerReference, strength);
        }

        public static bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            return GetControllerSDK().HapticPulse(controllerReference, clip);
        }

        public static SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            return GetControllerSDK().GetHapticModifiers();
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetVelocityOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerVelocity(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static Vector3 GetVelocityOnIndex(uint index)
        {
            return GetControllerVelocity(VRTK_ControllerReference.GetControllerReference(index));
        }

        public static Vector3 GetControllerVelocity(VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetVelocity(controllerReference);
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetAngularVelocityOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerAngularVelocity(controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static Vector3 GetAngularVelocityOnIndex(uint index)
        {
            return GetControllerAngularVelocity(VRTK_ControllerReference.GetControllerReference(index));
        }

        public static Vector3 GetControllerAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetAngularVelocity(controllerReference);
        }

        public static bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            return GetControllerSDK().IsTouchpadStatic(isTouched, currentAxisValues, previousAxisValues, compareFidelity);
        }

        public static Vector2 GetControllerAxis(SDK_BaseController.ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetButtonAxis(buttonType, controllerReference);
        }

        public static float GetControllerHairlineDelta(SDK_BaseController.ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetButtonHairlineDelta(buttonType, controllerReference);
        }

        public static bool GetControllerButtonState(SDK_BaseController.ButtonTypes buttonType, SDK_BaseController.ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetControllerButtonState(buttonType, pressType, controllerReference);
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetTouchpadAxisOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerAxis(buttonType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetTriggerAxisOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerAxis(buttonType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static Vector2 GetTriggerAxisOnIndex(uint index)
        {
            return GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetGripAxisOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerAxis(buttonType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static Vector2 GetGripAxisOnIndex(uint index)
        {
            return GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetTriggerHairlineDeltaOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerHairlineDelta(buttonType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.TriggerHairline, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.GetGripHairlineDeltaOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerHairlineDelta(buttonType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static float GetGripHairlineDeltaOnIndex(uint index)
        {
            return GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.GripHairline, VRTK_ControllerReference.GetControllerReference(index));
        }

        //Trigger
        [System.Obsolete("`VRTK_SDK_Bridge.IsTriggerPressedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTriggerPressedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Press, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTriggerPressedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTriggerPressedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTriggerPressedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTriggerPressedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTriggerTouchedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTriggerTouchedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Touch, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTriggerTouchedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTriggerTouchedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsHairTriggerDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsHairTriggerDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsHairTriggerUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsHairTriggerUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        //Grip
        [System.Obsolete("`VRTK_SDK_Bridge.IsGripPressedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsGripPressedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.Press, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsGripPressedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsGripPressedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsGripPressedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsGripPressedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsGripTouchedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsGripTouchedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.Touch, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsGripTouchedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsGripTouchedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsGripTouchedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsGripTouchedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsHairGripDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsHairGripDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsHairGripUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsHairGripUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        //Touchpad

        [System.Obsolete("`VRTK_SDK_Bridge.IsTouchpadPressedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTouchpadPressedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.Press, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTouchpadPressedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTouchpadPressedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTouchpadTouchedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTouchpadTouchedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.Touch, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTouchpadTouchedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsTouchpadTouchedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        //ButtonOne

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonOnePressedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonOnePressedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.Press, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonOnePressedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonOnePressedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonOnePressedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonOnePressedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonOneTouchedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonOneTouchedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.Touch, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonOneTouchedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonOneTouchedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonOneTouchedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonOneTouchedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        //ButtonTwo

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonTwoPressedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonTwoPressedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.Press, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonTwoPressedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonTwoPressedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonTwoPressedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonTwoPressedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonTwoTouchedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonTwoTouchedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.Touch, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonTwoTouchedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonTwoTouchedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsButtonTwoTouchedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsButtonTwoTouchedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        //StartMenu

        [System.Obsolete("`VRTK_SDK_Bridge.IsStartMenuPressedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsStartMenuPressedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.Press, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsStartMenuPressedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsStartMenuPressedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsStartMenuPressedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsStartMenuPressedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsStartMenuTouchedOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsStartMenuTouchedOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.Touch, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsStartMenuTouchedDownOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsStartMenuTouchedDownOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.TouchDown, VRTK_ControllerReference.GetControllerReference(index));
        }

        [System.Obsolete("`VRTK_SDK_Bridge.IsStartMenuTouchedUpOnIndex(index)` has been replaced with `VRTK_SDK_Bridge.GetControllerButtonState(buttonType, pressType, controllerReference)`. This method will be removed in a future version of VRTK.")]
        public static bool IsStartMenuTouchedUpOnIndex(uint index)
        {
            return GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.TouchUp, VRTK_ControllerReference.GetControllerReference(index));
        }

        #endregion

        #region Headset Methods

        public static void HeadsetProcessUpdate(Dictionary<string, object> options = null)
        {
            GetHeadsetSDK().ProcessUpdate(options);
        }

        public static void HeadsetProcessFixedUpdate(Dictionary<string, object> options = null)
        {
            GetHeadsetSDK().ProcessFixedUpdate(options);
        }

        public static Transform GetHeadset()
        {
            return GetHeadsetSDK().GetHeadset();
        }

        public static Transform GetHeadsetCamera()
        {
            return GetHeadsetSDK().GetHeadsetCamera();
        }

        public static Vector3 GetHeadsetVelocity()
        {
            return GetHeadsetSDK().GetHeadsetVelocity();
        }

        public static Vector3 GetHeadsetAngularVelocity()
        {
            return GetHeadsetSDK().GetHeadsetAngularVelocity();
        }

        public static void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            GetHeadsetSDK().HeadsetFade(color, duration, fadeOverlay);
        }

        public static bool HasHeadsetFade(Transform obj)
        {
            return GetHeadsetSDK().HasHeadsetFade(obj);
        }

        public static void AddHeadsetFade(Transform camera)
        {
            GetHeadsetSDK().AddHeadsetFade(camera);
        }

        #endregion

        #region Boundaries Methods

        public static Transform GetPlayArea()
        {
            return GetBoundariesSDK().GetPlayArea();
        }

        public static Vector3[] GetPlayAreaVertices()
        {
            return GetBoundariesSDK().GetPlayAreaVertices();
        }

        public static float GetPlayAreaBorderThickness()
        {
            return GetBoundariesSDK().GetPlayAreaBorderThickness();
        }

        public static bool IsPlayAreaSizeCalibrated()
        {
            return GetBoundariesSDK().IsPlayAreaSizeCalibrated();
        }

        public static bool GetDrawAtRuntime()
        {
            return GetBoundariesSDK().GetDrawAtRuntime();
        }

        public static void SetDrawAtRuntime(bool value)
        {
            GetBoundariesSDK().SetDrawAtRuntime(value);
        }

        #endregion

        #region System Methods

        public static bool IsDisplayOnDesktop()
        {
            return GetSystemSDK().IsDisplayOnDesktop();
        }

        public static bool ShouldAppRenderWithLowResources()
        {
            return GetSystemSDK().ShouldAppRenderWithLowResources();
        }

        public static void ForceInterleavedReprojectionOn(bool force)
        {
            GetSystemSDK().ForceInterleavedReprojectionOn(force);
        }

        #endregion

        public static SDK_BaseSystem GetSystemSDK()
        {
            if (VRTK_SDKManager.instance != null && VRTK_SDKManager.instance.loadedSetup != null)
            {
                return VRTK_SDKManager.instance.loadedSetup.systemSDK;
            }
            if (systemSDK == null)
            {
                systemSDK = ScriptableObject.CreateInstance<SDK_FallbackSystem>();
            }
            return systemSDK;
        }

        public static SDK_BaseHeadset GetHeadsetSDK()
        {
            if (VRTK_SDKManager.instance != null && VRTK_SDKManager.instance.loadedSetup != null)
            {
                return VRTK_SDKManager.instance.loadedSetup.headsetSDK;
            }
            if (headsetSDK == null)
            {
                headsetSDK = ScriptableObject.CreateInstance<SDK_FallbackHeadset>();
            }
            return headsetSDK;
        }

        public static SDK_BaseController GetControllerSDK()
        {
            if (VRTK_SDKManager.instance != null && VRTK_SDKManager.instance.loadedSetup != null)
            {
                return VRTK_SDKManager.instance.loadedSetup.controllerSDK;
            }
            if (controllerSDK == null)
            {
                controllerSDK = ScriptableObject.CreateInstance<SDK_FallbackController>();
            }
            return controllerSDK;
        }

        public static SDK_BaseBoundaries GetBoundariesSDK()
        {
            if (VRTK_SDKManager.instance != null && VRTK_SDKManager.instance.loadedSetup != null)
            {
                return VRTK_SDKManager.instance.loadedSetup.boundariesSDK;
            }
            if (boundariesSDK == null)
            {
                boundariesSDK = ScriptableObject.CreateInstance<SDK_FallbackBoundaries>();
            }
            return boundariesSDK;
        }

        public static void InvalidateCaches()
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(systemSDK);
            Object.DestroyImmediate(headsetSDK);
            Object.DestroyImmediate(controllerSDK);
            Object.DestroyImmediate(boundariesSDK);
#else
            Object.Destroy(systemSDK);
            Object.Destroy(headsetSDK);
            Object.Destroy(controllerSDK);
            Object.Destroy(boundariesSDK);
#endif

            systemSDK = null;
            headsetSDK = null;
            controllerSDK = null;
            boundariesSDK = null;
        }
    }
}