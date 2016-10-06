namespace VRTK
{
    using UnityEngine;

    public abstract class SDK_Base : ScriptableObject
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

        protected Transform cachedHeadset;
        protected Transform cachedHeadsetCamera;
        protected Transform cachedPlayArea;

        public abstract string GetControllerElementPath(ControllerElelements element, VRTK_DeviceFinder.ControllerHand hand);
        public abstract GameObject GetTrackedObject(GameObject obj, out uint index);
        public abstract GameObject GetTrackedObjectByIndex(uint index);
        public abstract uint GetIndexOfTrackedObject(GameObject trackedObject);
        public abstract Transform GetTrackedObjectOrigin(GameObject obj);
        public abstract bool TrackedIndexIsController(uint index);
        public abstract GameObject GetControllerLeftHand();
        public abstract GameObject GetControllerRightHand();
        public abstract bool IsControllerLeftHand(GameObject controller);
        public abstract bool IsControllerRightHand(GameObject controller);
        public abstract Transform GetHeadset();
        public abstract Transform GetHeadsetCamera();
        public abstract GameObject GetHeadsetCamera(GameObject obj);
        public abstract Transform GetPlayArea();
        public abstract Vector3[] GetPlayAreaVertices(GameObject playArea);
        public abstract float GetPlayAreaBorderThickness(GameObject playArea);
        public abstract bool IsPlayAreaSizeCalibrated(GameObject playArea);
        public abstract bool IsDisplayOnDesktop();
        public abstract bool ShouldAppRenderWithLowResources();
        public abstract void ForceInterleavedReprojectionOn(bool force);
        public abstract GameObject GetControllerRenderModel(GameObject controller);
        public abstract void SetControllerRenderModelWheel(GameObject renderModel, bool state);
        public abstract void HeadsetFade(Color color, float duration, bool fadeOverlay = false);
        public abstract bool HasHeadsetFade(GameObject obj);
        public abstract void AddHeadsetFade(Transform camera);
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