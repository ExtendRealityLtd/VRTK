namespace VRTK
{
    using UnityEngine;
    using Valve.VR;

    public class VRTK_SDK_Bridge : MonoBehaviour
    {
        private static SteamVR_ControllerManager cachedControllerManager;
        private static Transform cachedHeadset;
        private static Transform cachedHeadsetCamera;
        private static Transform cachedPlayArea;

        public static string defaultAttachPointPath = "Model/tip/attach";
        public static string defaultTriggerModelPath = "Model/trigger";
        public static string defaultGripLeftModelPath = "Model/lgrip";
        public static string defaultGripRightModelPath = "Model/rgrip";
        public static string defaultTouchpadModelPath = "Model/trackpad";
        public static string defaultApplicationMenuModelPath = "Model/button";
        public static string defaultSystemModelPath = "Model/sys_button";
        public static string defaultBodyModelPath = "Model/body";

        public static GameObject GetTrackedObject(GameObject obj, out uint index)
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

        public static GameObject GetTrackedObjectByIndex(uint index)
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

        public static uint GetIndexOfTrackedObject(GameObject trackedObject)
        {
            uint index = 0;
            GetTrackedObject(trackedObject, out index);
            return index;
        }

        public static Transform GetTrackedObjectOrigin(GameObject obj)
        {
            var trackedObject = obj.GetComponent<SteamVR_TrackedObject>();
            if (trackedObject)
            {
                return trackedObject.origin ? trackedObject.origin : trackedObject.transform.parent;
            }
            return null;
        }

        public static bool TrackedIndexIsController(uint index)
        {
            var system = OpenVR.System;
            if (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
            {
                return true;
            }
            return false;
        }

        public static GameObject GetControllerLeftHand()
        {
            var controllerManager = GetControllerManager();
            if (controllerManager)
            {
                return controllerManager.left;
            }
            return null;
        }

        public static GameObject GetControllerRightHand()
        {
            var controllerManager = GetControllerManager();
            if (controllerManager)
            {
                return controllerManager.right;
            }
            return null;
        }

        public static bool IsControllerLeftHand(GameObject controller)
        {
            var controllerManager = GetControllerManager();
            if (controllerManager && controller == controllerManager.left)
            {
                return true;
            }
            return false;
        }

        public static bool IsControllerRightHand(GameObject controller)
        {
            var controllerManager = GetControllerManager();
            if (controllerManager && controller == controllerManager.right)
            {
                return true;
            }
            return false;
        }

        public static Transform GetHeadset()
        {
            if (cachedHeadset == null)
            {
#if (UNITY_5_4_OR_NEWER)
                cachedHeadset = FindObjectOfType<SteamVR_Camera>().transform;
#else
                cachedHeadset = FindObjectOfType<SteamVR_GameView>().transform;
#endif
            }
            return cachedHeadset;
        }

        public static Transform GetHeadsetCamera()
        {
            if (cachedHeadsetCamera == null)
            {
                cachedHeadsetCamera = FindObjectOfType<SteamVR_Camera>().transform;
            }
            return cachedHeadsetCamera;
        }

        public static Transform GetPlayArea()
        {
            if (cachedPlayArea == null)
            {
                cachedPlayArea = FindObjectOfType<SteamVR_PlayArea>().transform;
            }
            return cachedPlayArea;
        }

        public static Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            if (area)
            {
                return area.vertices;
            }
            return null;
        }

        public static float GetPlayAreaBorderThickness(GameObject playArea)
        {
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            if (area)
            {
                return area.borderThickness;
            }
            return 0f;
        }

        public static bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            return (area.size == SteamVR_PlayArea.Size.Calibrated);
        }

        public static bool IsDisplayOnDesktop()
        {
            return (OpenVR.System == null || OpenVR.System.IsDisplayOnDesktop());
        }

        public static bool ShouldAppRenderWithLowResources()
        {
            return (OpenVR.Compositor != null && OpenVR.Compositor.ShouldAppRenderWithLowResources());
        }

        public static void ForceInterleavedReprojectionOn(bool force)
        {
            if (OpenVR.Compositor != null)
            {
                OpenVR.Compositor.ForceInterleavedReprojectionOn(force);
            }
        }

        public static GameObject GetControllerRenderModel(GameObject controller)
        {
            var renderModel = (controller.GetComponent<SteamVR_RenderModel>() ? controller.GetComponent<SteamVR_RenderModel>() : controller.GetComponentInChildren<SteamVR_RenderModel>());
            return renderModel.gameObject;
        }

        public static void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            var model = renderModel.GetComponent<SteamVR_RenderModel>();
            if (model)
            {
                model.controllerModeState.bScrollWheelVisible = state;
            }
        }

        public static void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            SteamVR_Fade.Start(color, duration, fadeOverlay);
        }

        public static bool HasHeadsetFade(GameObject obj)
        {
            if (obj.GetComponentInChildren<SteamVR_Fade>())
            {
                return true;
            }
            return false;
        }

        public static void AddHeadsetFade(Transform camera)
        {
            if (camera && !camera.gameObject.GetComponent<SteamVR_Fade>())
            {
                camera.gameObject.AddComponent<SteamVR_Fade>();
            }
        }

        public static void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
            if (index < uint.MaxValue)
            {
                var device = SteamVR_Controller.Input((int)index);
                device.TriggerHapticPulse(durationMicroSec, EVRButtonId.k_EButton_Axis0);
            }
        }

        public static Vector3 GetVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.velocity;
        }

        public static Vector3 GetAngularVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.angularVelocity;
        }

        public static Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis();
        }

        public static Vector2 GetTriggerAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
        }

        public static float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return 0f;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.hairTriggerDelta;
        }

        //Trigger

        public static bool IsTriggerPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Trigger);
        }

        public static bool IsTriggerPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        public static bool IsTriggerPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Trigger);
        }

        public static bool IsTriggerTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Trigger);
        }

        public static bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        public static bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Trigger);
        }

        public static bool IsHairTriggerDownOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerDown();
        }

        public static bool IsHairTriggerUpOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerUp();
        }

        //Grip

        public static bool IsGripPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Grip);
        }

        public static bool IsGripPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Grip);
        }

        public static bool IsGripPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Grip);
        }

        public static bool IsGripTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Grip);
        }

        public static bool IsGripTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Grip);
        }

        public static bool IsGripTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Grip);
        }

        //Touchpad

        public static bool IsTouchpadPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public static bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public static bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public static bool IsTouchpadTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public static bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public static bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        //Application Menu

        public static bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public static bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public static bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public static bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public static bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public static bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        private static SteamVR_ControllerManager GetControllerManager()
        {
            if (cachedControllerManager == null)
            {
                cachedControllerManager = FindObjectOfType<SteamVR_ControllerManager>();
            }
            return cachedControllerManager;
        }

        private enum ButtonPressTypes
        {
            Press,
            PressDown,
            PressUp,
            Touch,
            TouchDown,
            TouchUp
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
    }
}
