namespace VRTK
{
    using UnityEngine;
    using Valve.VR;

    public class SDK_SteamVR : SDK_Base
    {
        private SteamVR_ControllerManager cachedControllerManager;

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

        public override uint GetIndexOfTrackedObject(GameObject trackedObject)
        {
            uint index = 0;
            GetTrackedObject(trackedObject, out index);
            return index;
        }

        public override Transform GetTrackedObjectOrigin(GameObject obj)
        {
            var trackedObject = obj.GetComponent<SteamVR_TrackedObject>();
            if (trackedObject)
            {
                return trackedObject.origin ? trackedObject.origin : trackedObject.transform.parent;
            }
            return null;
        }

        public override bool TrackedIndexIsController(uint index)
        {
            var system = OpenVR.System;
            if (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
            {
                return true;
            }
            return false;
        }

        public override GameObject GetControllerLeftHand()
        {
            var controllerManager = GetControllerManager();
            if (controllerManager)
            {
                return controllerManager.left;
            }
            return null;
        }

        public override GameObject GetControllerRightHand()
        {
            var controllerManager = GetControllerManager();
            if (controllerManager)
            {
                return controllerManager.right;
            }
            return null;
        }

        public override bool IsControllerLeftHand(GameObject controller)
        {
            var controllerManager = GetControllerManager();
            if (controllerManager && controller == controllerManager.left)
            {
                return true;
            }
            return false;
        }

        public override bool IsControllerRightHand(GameObject controller)
        {
            var controllerManager = GetControllerManager();
            if (controllerManager && controller == controllerManager.right)
            {
                return true;
            }
            return false;
        }

        public override Transform GetHeadset()
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

        public override Transform GetHeadsetCamera()
        {
            if (cachedHeadsetCamera == null)
            {
                cachedHeadsetCamera = FindObjectOfType<SteamVR_Camera>().transform;
            }
            return cachedHeadsetCamera;
        }

        public override GameObject GetHeadsetCamera(GameObject obj)
        {
            return obj.GetComponent<SteamVR_Camera>().gameObject;
        }

        public override Transform GetPlayArea()
        {
            if (cachedPlayArea == null)
            {
                cachedPlayArea = FindObjectOfType<SteamVR_PlayArea>().transform;
            }
            return cachedPlayArea;
        }

        public override Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            if (area)
            {
                return area.vertices;
            }
            return null;
        }

        public override float GetPlayAreaBorderThickness(GameObject playArea)
        {
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            if (area)
            {
                return area.borderThickness;
            }
            return 0f;
        }

        public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            var area = playArea.GetComponent<SteamVR_PlayArea>();
            return (area.size == SteamVR_PlayArea.Size.Calibrated);
        }

        public override bool IsDisplayOnDesktop()
        {
            return (OpenVR.System == null || OpenVR.System.IsDisplayOnDesktop());
        }

        public override bool ShouldAppRenderWithLowResources()
        {
            return (OpenVR.Compositor != null && OpenVR.Compositor.ShouldAppRenderWithLowResources());
        }

        public override void ForceInterleavedReprojectionOn(bool force)
        {
            if (OpenVR.Compositor != null)
            {
                OpenVR.Compositor.ForceInterleavedReprojectionOn(force);
            }
        }

        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            var renderModel = (controller.GetComponent<SteamVR_RenderModel>() ? controller.GetComponent<SteamVR_RenderModel>() : controller.GetComponentInChildren<SteamVR_RenderModel>());
            return (renderModel ? renderModel.gameObject : null);
        }

        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            var model = renderModel.GetComponent<SteamVR_RenderModel>();
            if (model)
            {
                model.controllerModeState.bScrollWheelVisible = state;
            }
        }

        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            SteamVR_Fade.Start(color, duration, fadeOverlay);
        }

        public override bool HasHeadsetFade(GameObject obj)
        {
            if (obj.GetComponentInChildren<SteamVR_Fade>())
            {
                return true;
            }
            return false;
        }

        public override void AddHeadsetFade(Transform camera)
        {
            if (camera && !camera.gameObject.GetComponent<SteamVR_Fade>())
            {
                camera.gameObject.AddComponent<SteamVR_Fade>();
            }
        }

        public override void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
            if (index < uint.MaxValue)
            {
                var device = SteamVR_Controller.Input((int)index);
                device.TriggerHapticPulse(durationMicroSec, EVRButtonId.k_EButton_Axis0);
            }
        }

        public override Vector3 GetVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.velocity;
        }

        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector3.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.angularVelocity;
        }

        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis();
        }

        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return Vector2.zero;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetAxis(EVRButtonId.k_EButton_SteamVR_Trigger);
        }

        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return 0f;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.hairTriggerDelta;
        }

        //Trigger

        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Trigger);
        }

        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Trigger);
        }

        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Trigger);
        }

        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Trigger);
        }

        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Trigger);
        }

        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerDown();
        }

        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            if (index >= uint.MaxValue)
            {
                return false;
            }
            var device = SteamVR_Controller.Input((int)index);
            return device.GetHairTriggerUp();
        }

        //Grip

        public override bool IsGripPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Grip);
        }

        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Grip);
        }

        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Grip);
        }

        public override bool IsGripTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Grip);
        }

        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Grip);
        }

        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Grip);
        }

        //Touchpad

        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.Touchpad);
        }

        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchUp, SteamVR_Controller.ButtonMask.Touchpad);
        }

        //Application Menu

        public override bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Press, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public override bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public override bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.PressUp, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public override bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.Touch, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

        public override bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return IsButtonPressed(index, ButtonPressTypes.TouchDown, SteamVR_Controller.ButtonMask.ApplicationMenu);
        }

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
