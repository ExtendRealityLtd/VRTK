namespace VRTK
{
    using UnityEngine;

    public class VRTK_SDK_Bridge
    {
        public SDK_BaseSystem currentSystemSDK = null;

        private static SDK_BaseSystem systemSDK = null;
        private static SDK_BaseHeadset headsetSDK = null;
        private static SDK_BaseController controllerSDK = null;
        private static SDK_BaseBoundaries boundariesSDK = null;

        public static string GetControllerElementPath(SDK_InterfaceController.ControllerElelements element, VRTK_DeviceFinder.ControllerHand hand = VRTK_DeviceFinder.ControllerHand.Right)
        {
            return GetControllerSDK().GetControllerElementPath(element, hand);
        }

        public static GameObject GetTrackedObject(GameObject obj, out uint index)
        {
            return GetControllerSDK().GetTrackedObject(obj, out index);
        }

        public static GameObject GetTrackedObjectByIndex(uint index)
        {
            return GetControllerSDK().GetTrackedObjectByIndex(index);
        }

        public static uint GetIndexOfTrackedObject(GameObject trackedObject)
        {
            return GetControllerSDK().GetIndexOfTrackedObject(trackedObject);
        }

        public static Transform GetTrackedObjectOrigin(GameObject obj)
        {
            return GetControllerSDK().GetTrackedObjectOrigin(obj);
        }

        public static bool TrackedIndexIsController(uint index)
        {
            return GetControllerSDK().TrackedIndexIsController(index);
        }

        public static GameObject GetControllerLeftHand()
        {
            return GetControllerSDK().GetControllerLeftHand();
        }

        public static GameObject GetControllerRightHand()
        {
            return GetControllerSDK().GetControllerRightHand();
        }

        public static bool IsControllerLeftHand(GameObject controller)
        {
            return GetControllerSDK().IsControllerLeftHand(controller);
        }

        public static bool IsControllerRightHand(GameObject controller)
        {
            return GetControllerSDK().IsControllerRightHand(controller);
        }

        public static GameObject GetControllerRenderModel(GameObject controller)
        {
            return GetControllerSDK().GetControllerRenderModel(controller);
        }

        public static void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            GetControllerSDK().SetControllerRenderModelWheel(renderModel, state);
        }

        public static void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
            GetControllerSDK().HapticPulseOnIndex(index, durationMicroSec);
        }

        public static Vector3 GetVelocityOnIndex(uint index)
        {
            return GetControllerSDK().GetVelocityOnIndex(index);
        }

        public static Vector3 GetAngularVelocityOnIndex(uint index)
        {
            return GetControllerSDK().GetAngularVelocityOnIndex(index);
        }

        public static Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return GetControllerSDK().GetTouchpadAxisOnIndex(index);
        }

        public static Vector2 GetTriggerAxisOnIndex(uint index)
        {
            return GetControllerSDK().GetTriggerAxisOnIndex(index);
        }

        public static float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return GetControllerSDK().GetTriggerHairlineDeltaOnIndex(index);
        }

        //Trigger

        public static bool IsTriggerPressedOnIndex(uint index)
        {
            return GetControllerSDK().IsTriggerPressedOnIndex(index);
        }

        public static bool IsTriggerPressedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsTriggerPressedDownOnIndex(index);
        }

        public static bool IsTriggerPressedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsTriggerPressedUpOnIndex(index);
        }

        public static bool IsTriggerTouchedOnIndex(uint index)
        {
            return GetControllerSDK().IsTriggerTouchedOnIndex(index);
        }

        public static bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsTriggerTouchedDownOnIndex(index);
        }

        public static bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsTriggerTouchedUpOnIndex(index);
        }

        public static bool IsHairTriggerDownOnIndex(uint index)
        {
            return GetControllerSDK().IsHairTriggerDownOnIndex(index);
        }

        public static bool IsHairTriggerUpOnIndex(uint index)
        {
            return GetControllerSDK().IsHairTriggerUpOnIndex(index);
        }

        //Grip

        public static bool IsGripPressedOnIndex(uint index)
        {
            return GetControllerSDK().IsGripPressedOnIndex(index);
        }

        public static bool IsGripPressedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsGripPressedDownOnIndex(index);
        }

        public static bool IsGripPressedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsGripPressedUpOnIndex(index);
        }

        public static bool IsGripTouchedOnIndex(uint index)
        {
            return GetControllerSDK().IsGripTouchedOnIndex(index);
        }

        public static bool IsGripTouchedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsGripTouchedDownOnIndex(index);
        }

        public static bool IsGripTouchedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsGripTouchedUpOnIndex(index);
        }

        //Touchpad

        public static bool IsTouchpadPressedOnIndex(uint index)
        {
            return GetControllerSDK().IsTouchpadPressedOnIndex(index);
        }

        public static bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsTouchpadPressedDownOnIndex(index);
        }

        public static bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsTouchpadPressedUpOnIndex(index);
        }

        public static bool IsTouchpadTouchedOnIndex(uint index)
        {
            return GetControllerSDK().IsTouchpadTouchedOnIndex(index);
        }

        public static bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsTouchpadTouchedDownOnIndex(index);
        }

        public static bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsTouchpadTouchedUpOnIndex(index);
        }

        //Application Menu

        public static bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return GetControllerSDK().IsApplicationMenuPressedOnIndex(index);
        }

        public static bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsApplicationMenuPressedDownOnIndex(index);
        }

        public static bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsApplicationMenuPressedUpOnIndex(index);
        }

        public static bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return GetControllerSDK().IsApplicationMenuTouchedOnIndex(index);
        }

        public static bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsApplicationMenuTouchedDownOnIndex(index);
        }

        public static bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsApplicationMenuTouchedUpOnIndex(index);
        }

        public static Transform GetHeadset()
        {
            return GetHeadsetSDK().GetHeadset();
        }

        public static Transform GetHeadsetCamera()
        {
            return GetHeadsetSDK().GetHeadsetCamera();
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

        public static Transform GetPlayArea()
        {
            return GetBoundariesSDK().GetPlayArea();
        }

        public static Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            return GetBoundariesSDK().GetPlayAreaVertices(playArea);
        }

        public static float GetPlayAreaBorderThickness(GameObject playArea)
        {
            return GetBoundariesSDK().GetPlayAreaBorderThickness(playArea);
        }

        public static bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            return GetBoundariesSDK().IsPlayAreaSizeCalibrated(playArea);
        }

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

        private static SDK_InterfaceSystem GetSystemSDK()
        {
            if (systemSDK == null)
            {
#if VRTK_SDK_SYSTEM_STEAMVR
                systemSDK = ScriptableObject.CreateInstance<SDK_SteamVRSystem>();
#endif
                if(systemSDK == null)
                {
                    systemSDK = ScriptableObject.CreateInstance<SDK_BaseSystem>();
                    Debug.LogError("No System SDK configured, falling back to Base System SDK.");
                }
            }
            return systemSDK;
        }

        private static SDK_InterfaceHeadset GetHeadsetSDK()
        {
            if (headsetSDK == null)
            {
#if VRTK_SDK_HEADSET_STEAMVR
                headsetSDK = ScriptableObject.CreateInstance<SDK_SteamVRHeadset>();
#endif
                if (headsetSDK == null)
                {
                    headsetSDK = ScriptableObject.CreateInstance<SDK_BaseHeadset>();
                    Debug.LogError("No Headset SDK configured, falling back to Base Headset SDK.");
                }
            }
            return headsetSDK;
        }

        private static SDK_InterfaceController GetControllerSDK()
        {
            if (controllerSDK == null)
            {
#if VRTK_SDK_CONTROLLER_STEAMVR
                controllerSDK = ScriptableObject.CreateInstance<SDK_SteamVRController>();
#endif
                if (controllerSDK == null)
                {
                    controllerSDK = ScriptableObject.CreateInstance<SDK_BaseController>();
                    Debug.LogError("No Controller SDK configured, falling back to Base Controller SDK.");
                }
            }
            return controllerSDK;
        }

        private static SDK_InterfaceBoundaries GetBoundariesSDK()
        {
            if (boundariesSDK == null)
            {
#if VRTK_SDK_BOUNDARIES_STEAMVR
                boundariesSDK = ScriptableObject.CreateInstance<SDK_SteamVRBoundaries>();
#endif
                if (boundariesSDK == null)
                {
                    boundariesSDK = ScriptableObject.CreateInstance<SDK_BaseBoundaries>();
                    Debug.LogError("No Boundaries SDK configured, falling back to Base Boundaries SDK.");
                }
            }
            return boundariesSDK;
        }
    }
}