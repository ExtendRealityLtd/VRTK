namespace VRTK
{
    using UnityEngine;

    public class VRTK_SDK_Bridge
    {
        private static SDK_Base activeSDK = null;

        public static string GetControllerElementPath(SDK_Base.ControllerElelements element, VRTK_DeviceFinder.ControllerHand hand = VRTK_DeviceFinder.ControllerHand.Right)
        {
            return GetActiveSDK().GetControllerElementPath(element, hand);
        }

        public static GameObject GetTrackedObject(GameObject obj, out uint index)
        {
            return GetActiveSDK().GetTrackedObject(obj, out index);
        }

        public static GameObject GetTrackedObjectByIndex(uint index)
        {
            return GetActiveSDK().GetTrackedObjectByIndex(index);
        }

        public static uint GetIndexOfTrackedObject(GameObject trackedObject)
        {
            return GetActiveSDK().GetIndexOfTrackedObject(trackedObject);
        }

        public static Transform GetTrackedObjectOrigin(GameObject obj)
        {
            return GetActiveSDK().GetTrackedObjectOrigin(obj);
        }

        public static bool TrackedIndexIsController(uint index)
        {
            return GetActiveSDK().TrackedIndexIsController(index);
        }

        public static GameObject GetControllerLeftHand()
        {
            return GetActiveSDK().GetControllerLeftHand();
        }

        public static GameObject GetControllerRightHand()
        {
            return GetActiveSDK().GetControllerRightHand();
        }

        public static bool IsControllerLeftHand(GameObject controller)
        {
            return GetActiveSDK().IsControllerLeftHand(controller);
        }

        public static bool IsControllerRightHand(GameObject controller)
        {
            return GetActiveSDK().IsControllerRightHand(controller);
        }

        public static Transform GetHeadset()
        {
            return GetActiveSDK().GetHeadset();
        }

        public static Transform GetHeadsetCamera()
        {
            return GetActiveSDK().GetHeadsetCamera();
        }

        public static GameObject GetHeadsetCamera(GameObject obj)
        {
            return GetActiveSDK().GetHeadsetCamera(obj);
        }

        public static Transform GetPlayArea()
        {
            return GetActiveSDK().GetPlayArea();
        }

        public static Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            return GetActiveSDK().GetPlayAreaVertices(playArea);
        }

        public static float GetPlayAreaBorderThickness(GameObject playArea)
        {
            return GetActiveSDK().GetPlayAreaBorderThickness(playArea);
        }

        public static bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            return GetActiveSDK().IsPlayAreaSizeCalibrated(playArea);
        }

        public static bool IsDisplayOnDesktop()
        {
            return GetActiveSDK().IsDisplayOnDesktop();
        }

        public static bool ShouldAppRenderWithLowResources()
        {
            return GetActiveSDK().ShouldAppRenderWithLowResources();
        }

        public static void ForceInterleavedReprojectionOn(bool force)
        {
            GetActiveSDK().ForceInterleavedReprojectionOn(force);
        }

        public static GameObject GetControllerRenderModel(GameObject controller)
        {
            return GetActiveSDK().GetControllerRenderModel(controller);
        }

        public static void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            GetActiveSDK().SetControllerRenderModelWheel(renderModel, state);
        }

        public static void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            GetActiveSDK().HeadsetFade(color, duration, fadeOverlay);
        }

        public static bool HasHeadsetFade(GameObject obj)
        {
            return GetActiveSDK().HasHeadsetFade(obj);
        }

        public static void AddHeadsetFade(Transform camera)
        {
            GetActiveSDK().AddHeadsetFade(camera);
        }

        public static void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
            GetActiveSDK().HapticPulseOnIndex(index, durationMicroSec);
        }

        public static Vector3 GetVelocityOnIndex(uint index)
        {
            return GetActiveSDK().GetVelocityOnIndex(index);
        }

        public static Vector3 GetAngularVelocityOnIndex(uint index)
        {
            return GetActiveSDK().GetAngularVelocityOnIndex(index);
        }

        public static Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return GetActiveSDK().GetTouchpadAxisOnIndex(index);
        }

        public static Vector2 GetTriggerAxisOnIndex(uint index)
        {
            return GetActiveSDK().GetTriggerAxisOnIndex(index);
        }

        public static float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return GetActiveSDK().GetTriggerHairlineDeltaOnIndex(index);
        }

        //Trigger

        public static bool IsTriggerPressedOnIndex(uint index)
        {
            return GetActiveSDK().IsTriggerPressedOnIndex(index);
        }

        public static bool IsTriggerPressedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsTriggerPressedDownOnIndex(index);
        }

        public static bool IsTriggerPressedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsTriggerPressedUpOnIndex(index);
        }

        public static bool IsTriggerTouchedOnIndex(uint index)
        {
            return GetActiveSDK().IsTriggerTouchedOnIndex(index);
        }

        public static bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsTriggerTouchedDownOnIndex(index);
        }

        public static bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsTriggerTouchedUpOnIndex(index);
        }

        public static bool IsHairTriggerDownOnIndex(uint index)
        {
            return GetActiveSDK().IsHairTriggerDownOnIndex(index);
        }

        public static bool IsHairTriggerUpOnIndex(uint index)
        {
            return GetActiveSDK().IsHairTriggerUpOnIndex(index);
        }

        //Grip

        public static bool IsGripPressedOnIndex(uint index)
        {
            return GetActiveSDK().IsGripPressedOnIndex(index);
        }

        public static bool IsGripPressedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsGripPressedDownOnIndex(index);
        }

        public static bool IsGripPressedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsGripPressedUpOnIndex(index);
        }

        public static bool IsGripTouchedOnIndex(uint index)
        {
            return GetActiveSDK().IsGripTouchedOnIndex(index);
        }

        public static bool IsGripTouchedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsGripTouchedDownOnIndex(index);
        }

        public static bool IsGripTouchedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsGripTouchedUpOnIndex(index);
        }

        //Touchpad

        public static bool IsTouchpadPressedOnIndex(uint index)
        {
            return GetActiveSDK().IsTouchpadPressedOnIndex(index);
        }

        public static bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsTouchpadPressedDownOnIndex(index);
        }

        public static bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsTouchpadPressedUpOnIndex(index);
        }

        public static bool IsTouchpadTouchedOnIndex(uint index)
        {
            return GetActiveSDK().IsTouchpadTouchedOnIndex(index);
        }

        public static bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsTouchpadTouchedDownOnIndex(index);
        }

        public static bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsTouchpadTouchedUpOnIndex(index);
        }

        //Application Menu

        public static bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return GetActiveSDK().IsApplicationMenuPressedOnIndex(index);
        }

        public static bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsApplicationMenuPressedDownOnIndex(index);
        }

        public static bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsApplicationMenuPressedUpOnIndex(index);
        }

        public static bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return GetActiveSDK().IsApplicationMenuTouchedOnIndex(index);
        }

        public static bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return GetActiveSDK().IsApplicationMenuTouchedDownOnIndex(index);
        }

        public static bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
            return GetActiveSDK().IsApplicationMenuTouchedUpOnIndex(index);
        }

        private static SDK_Base GetActiveSDK()
        {
            if (activeSDK == null)
            {
                activeSDK = ScriptableObject.CreateInstance<SDK_SteamVR>();
            }

            return activeSDK;
        }
    }
}