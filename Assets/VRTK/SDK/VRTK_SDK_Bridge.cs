namespace VRTK
{
    using UnityEngine;

    public class VRTK_SDK_Bridge
    {
        private static SDK_BaseSystem systemSDK = null;
        private static SDK_BaseHeadset headsetSDK = null;
        private static SDK_BaseController controllerSDK = null;
        private static SDK_BaseBoundaries boundariesSDK = null;

        public static string GetControllerDefaultColliderPath()
        {
            return GetControllerSDK().GetControllerDefaultColliderPath();
        }

        public static string GetControllerElementPath(VRTK_ControllerElements element, VRTK_DeviceFinder.ControllerHand hand = VRTK_DeviceFinder.ControllerHand.Right, bool fullPath = false)
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

        public static Transform GetControllerOrigin(GameObject controller)
        {
            return GetControllerSDK().GetControllerOrigin(controller);
        }

        public static GameObject GetControllerLeftHand(bool actual)
        {
            return GetControllerSDK().GetControllerLeftHand(actual);
        }

        public static GameObject GetControllerRightHand(bool actual)
        {
            return GetControllerSDK().GetControllerRightHand(actual);
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

        public static GameObject GetControllerModel(VRTK_DeviceFinder.ControllerHand hand)
        {
            return GetControllerSDK().GetControllerModel(hand);
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

        public static Vector2 GetGripAxisOnIndex(uint index)
        {
            return GetControllerSDK().GetGripAxisOnIndex(index);
        }

        public static float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return GetControllerSDK().GetTriggerHairlineDeltaOnIndex(index);
        }

        public static float GetGripHairlineDeltaOnIndex(uint index)
        {
            return GetControllerSDK().GetGripHairlineDeltaOnIndex(index);
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

        public static bool IsHairGripDownOnIndex(uint index)
        {
            return GetControllerSDK().IsHairGripDownOnIndex(index);
        }

        public static bool IsHairGripUpOnIndex(uint index)
        {
            return GetControllerSDK().IsHairGripUpOnIndex(index);
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

        //ButtonOne Menu

        public static bool IsButtonOnePressedOnIndex(uint index)
        {
            return GetControllerSDK().IsButtonOnePressedOnIndex(index);
        }

        public static bool IsButtonOnePressedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsButtonOnePressedDownOnIndex(index);
        }

        public static bool IsButtonOnePressedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsButtonOnePressedUpOnIndex(index);
        }

        public static bool IsButtonOneTouchedOnIndex(uint index)
        {
            return GetControllerSDK().IsButtonOneTouchedOnIndex(index);
        }

        public static bool IsButtonOneTouchedDownOnIndex(uint index)
        {
            return GetControllerSDK().IsButtonOneTouchedDownOnIndex(index);
        }

        public static bool IsButtonOneTouchedUpOnIndex(uint index)
        {
            return GetControllerSDK().IsButtonOneTouchedUpOnIndex(index);
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
                systemSDK = ScriptableObject.CreateInstance<SDK_FallbackSystem>();
                if (VRTK_SDKManager.instance)
                {
                    switch (VRTK_SDKManager.instance.systemSDK)
                    {
                        case VRTK_SDKManager.SupportedSDKs.SteamVR:
                            systemSDK = ScriptableObject.CreateInstance<SDK_SteamVRSystem>();
                            break;
                        default:
                            Debug.LogError("No System SDK configured, falling back to generic System SDK.");
                            break;
                    }
                }
            }
            return systemSDK;
        }

        private static SDK_InterfaceHeadset GetHeadsetSDK()
        {
            if (headsetSDK == null)
            {
                headsetSDK = ScriptableObject.CreateInstance<SDK_FallbackHeadset>();
                if (VRTK_SDKManager.instance)
                {
                    switch (VRTK_SDKManager.instance.headsetSDK)
                    {
                        case VRTK_SDKManager.SupportedSDKs.SteamVR:
                            headsetSDK = ScriptableObject.CreateInstance<SDK_SteamVRHeadset>();
                            break;
                        default:
                            Debug.LogError("No Headset SDK configured, falling back to generic Headset SDK.");
                            break;
                    }
                }
            }
            return headsetSDK;
        }

        private static SDK_InterfaceController GetControllerSDK()
        {
            if (controllerSDK == null)
            {
                controllerSDK = ScriptableObject.CreateInstance<SDK_FallbackController>();
                if (VRTK_SDKManager.instance)
                {
                    switch (VRTK_SDKManager.instance.controllerSDK)
                    {
                        case VRTK_SDKManager.SupportedSDKs.SteamVR:
                            controllerSDK = ScriptableObject.CreateInstance<SDK_SteamVRController>();
                            break;
                        default:
                            Debug.LogError("No Controller SDK configured, falling back to generic Controller SDK.");
                            break;
                    }
                }
            }
            return controllerSDK;
        }

        private static SDK_InterfaceBoundaries GetBoundariesSDK()
        {
            if (boundariesSDK == null)
            {
                boundariesSDK = ScriptableObject.CreateInstance<SDK_FallbackBoundaries>();
                if (VRTK_SDKManager.instance)
                {
                    switch (VRTK_SDKManager.instance.boundariesSDK)
                    {
                        case VRTK_SDKManager.SupportedSDKs.SteamVR:
                            boundariesSDK = ScriptableObject.CreateInstance<SDK_SteamVRBoundaries>();
                            break;
                        default:
                            Debug.LogError("No Boundaries SDK configured, falling back to generic Boundaries SDK.");
                            break;
                    }
                }
            }
            return boundariesSDK;
        }
    }
}