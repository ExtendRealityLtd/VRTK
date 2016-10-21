namespace VRTK
{
    using UnityEngine;

    public class SDK_Default : SDK_Base
    {
        public override string GetControllerElementPath(ControllerElelements element, VRTK_DeviceFinder.ControllerHand hand)
        {
            return null;
        }

        public override GameObject GetTrackedObject(GameObject obj, out uint index)
        {
            index = 0;
            return null;
        }

        public override GameObject GetTrackedObjectByIndex(uint index)
        {
            return null;
        }

        public override uint GetIndexOfTrackedObject(GameObject trackedObject)
        {
            return 0;
        }

        public override Transform GetTrackedObjectOrigin(GameObject obj)
        {
            return null;
        }

        public override bool TrackedIndexIsController(uint index)
        {
            return false;
        }

        public override GameObject GetControllerLeftHand()
        {
            return null;
        }

        public override GameObject GetControllerRightHand()
        {
            return null;
        }

        public override bool IsControllerLeftHand(GameObject controller)
        {
            return false;
        }

        public override bool IsControllerRightHand(GameObject controller)
        {
            return false;
        }

        public override Transform GetHeadset()
        {
            if (cachedHeadset == null)
            {
                cachedHeadset = FindObjectOfType<Camera>().transform;
            }
            return cachedHeadset;
        }

        public override Transform GetHeadsetCamera()
        {
            if (cachedHeadsetCamera == null)
            {
                cachedHeadsetCamera = Camera.main.transform;
            }
            return cachedHeadsetCamera;
        }

        public override GameObject GetHeadsetCamera(GameObject obj)
        {
            return obj.GetComponent<Camera>().gameObject;
        }

        public override Transform GetPlayArea()
        {
            return null;
        }

        public override Vector3[] GetPlayAreaVertices(GameObject playArea)
        {
            return null;
        }

        public override float GetPlayAreaBorderThickness(GameObject playArea)
        {
            return 0f;
        }

        public override bool IsPlayAreaSizeCalibrated(GameObject playArea)
        {
            return false;
        }

        public override bool IsDisplayOnDesktop()
        {
            return false;
        }

        public override bool ShouldAppRenderWithLowResources()
        {
            return false;
        }

        public override void ForceInterleavedReprojectionOn(bool force)
        {
        }

        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            return null;
        }

        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
        }

        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
        }

        public override bool HasHeadsetFade(GameObject obj)
        {
            return false;
        }

        public override void AddHeadsetFade(Transform camera)
        {
        }

        public override void HapticPulseOnIndex(uint index, ushort durationMicroSec = 500)
        {
        }

        public override Vector3 GetVelocityOnIndex(uint index)
        {
            return Vector3.zero;
        }

        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            return Vector3.zero;
        }

        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        public override Vector2 GetTriggerAxisOnIndex(uint index)
        {
            return Vector2.zero;
        }

        public override float GetTriggerHairlineDeltaOnIndex(uint index)
        {
            return 0f;
        }

        //Trigger

        public override bool IsTriggerPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTriggerTouchedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsHairTriggerDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsHairTriggerUpOnIndex(uint index)
        {
            return false;
        }

        //Grip

        public override bool IsGripPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsGripTouchedUpOnIndex(uint index)
        {
            return false;
        }

        //Touchpad

        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return false;
        }

        //Application Menu

        public override bool IsApplicationMenuPressedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsApplicationMenuPressedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsApplicationMenuPressedUpOnIndex(uint index)
        {
            return false;
        }

        public override bool IsApplicationMenuTouchedOnIndex(uint index)
        {
            return false;
        }

        public override bool IsApplicationMenuTouchedDownOnIndex(uint index)
        {
            return false;
        }

        public override bool IsApplicationMenuTouchedUpOnIndex(uint index)
        {
            return false;
        }
    }
}