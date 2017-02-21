// Daydream Controller|SDK_Daydream|003
namespace VRTK
{
#if VRTK_SDK_DAYDREAM
    using UnityEngine;
    using System.Collections.Generic;

    public class SDK_DaydreamController : SDK_FallbackController
    {
        private GameObject controller;

        private Vector3 prevPosition;
        private Vector3 prevRotation;
        private Vector3 velocity = Vector3.zero;
        private Vector3 angularVelocity = Vector3.zero;

        //TODO: where should we check that controller is connected? eg if (GvrController.State != GvrConnectionState.Connected) 

        public override void ProcessUpdate(uint index, Dictionary<string, object> options)
        {
            if (controller == null)
            {
                return;
            }

            velocity = (controller.transform.position - prevPosition) / Time.deltaTime;

            angularVelocity = Quaternion.FromToRotation(prevRotation, controller.transform.eulerAngles).eulerAngles / Time.deltaTime;

            prevPosition = controller.transform.position;
            prevRotation = controller.transform.rotation.eulerAngles;
        }

        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            var returnCollider = "ControllerColliders/Fallback";
            return returnCollider;
        }

        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            //TODO: Use Gvr's tooltips or add an attach object ourselves
            string dd = "Controller/ddcontroller/";
            string pad = dd + "Tooltips/TouchPadOutside";
            string app = dd + "Tooltips/AppButtonOutside";

            switch (element)
            {
                case ControllerElements.AttachPoint:
                    return pad; //TODO: attach point at tip of controller?
                case ControllerElements.Touchpad:
                    return pad;
                case ControllerElements.ButtonOne:
                    return app;
                default:
                    return dd;
            }
        }

        public override uint GetControllerIndex(GameObject controller)
        {
            uint index = 0;

            switch (controller.name)
            {
                case "Camera":
                    index = 0;
                    break;
                case "RightController":
                    index = 1;
                    break;
                case "LeftController":
                    index = uint.MaxValue;
                    break;
            }
            return index;
        }

        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            switch (index)
            {
                case 1:
                    return controller;
                default:
                    return null;
            }
        }

        public override Transform GetControllerOrigin(GameObject controller)
        {
            return controller.transform;
        }

        //public override Transform GenerateControllerPointerOrigin(GameObject parent)
        //public override GameObject GetControllerLeftHand(bool actual = false)

        public override GameObject GetControllerRightHand(bool actual = false)
        {
            controller = GetSDKManagerControllerRightHand(actual);
            if ((controller == null) && actual)
            {
                controller = GameObject.Find("GvrControllerPointer/Controller");
            }
            if (controller != null)
            {
                prevPosition = controller.transform.position;
                prevRotation = controller.transform.rotation.eulerAngles;
            }
            return controller;
        }

        //public override bool IsControllerLeftHand(GameObject controller)
        //public override bool IsControllerLeftHand(GameObject controller, bool actual)

        public override bool IsControllerRightHand(GameObject controller)
        {
            return true;
        }


        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return true;
        }

        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        public override GameObject GetControllerModel(ControllerHand hand)
        {
            var model = GetSDKManagerControllerModelForHand(hand);
            if (!model)
            {
                model = GameObject.Find("DaydreamCameraRig/GvrControllerPointer/Controller"); //TODO: CAMERA_RIG constant at top?
            }
            return model;
        }

        public override GameObject GetControllerRenderModel(GameObject controller)
        {
            return controller;
        }

        //public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        //public override void HapticPulseOnIndex(uint index, float strength = 0.5f)
        //public override SDK_ControllerHapticModifiers GetHapticModifiers()

        public override Vector3 GetVelocityOnIndex(uint index)
        {
            return velocity;
        }

        public override Vector3 GetAngularVelocityOnIndex(uint index)
        {
            return angularVelocity;
        }

        public override Vector2 GetTouchpadAxisOnIndex(uint index)
        {
            return GvrController.TouchPos;
        }

        //public override Vector2 GetTriggerAxisOnIndex(uint index)
        //public override Vector2 GetGripAxisOnIndex(uint index)
        //public override float GetTriggerHairlineDeltaOnIndex(uint index)
        //public override float GetGripHairlineDeltaOnIndex(uint index)
        //public override bool IsTriggerPressedOnIndex(uint index)
        //public override bool IsTriggerPressedDownOnIndex(uint index)
        //public override bool IsTriggerPressedUpOnIndex(uint index)
        //public override bool IsTriggerTouchedOnIndex(uint index)
        //public override bool IsTriggerTouchedDownOnIndex(uint index)
        //public override bool IsTriggerTouchedUpOnIndex(uint index)
        //public override bool IsHairTriggerDownOnIndex(uint index)
        //public override bool IsHairTriggerUpOnIndex(uint index)
        //public override bool IsGripPressedOnIndex(uint index)
        //public override bool IsGripPressedDownOnIndex(uint index)
        //public override bool IsGripPressedUpOnIndex(uint index)
        //public override bool IsGripTouchedOnIndex(uint index)
        //public override bool IsGripTouchedDownOnIndex(uint index)
        //public override bool IsGripTouchedUpOnIndex(uint index)
        //public override bool IsHairGripDownOnIndex(uint index)
        //public override bool IsHairGripUpOnIndex(uint index)

        public override bool IsTouchpadPressedOnIndex(uint index)
        {
            return GvrController.ClickButton;
        }

        public override bool IsTouchpadPressedDownOnIndex(uint index)
        {
            return GvrController.ClickButtonDown;
        }

        public override bool IsTouchpadPressedUpOnIndex(uint index)
        {
            return GvrController.ClickButtonUp;
        }

        public override bool IsTouchpadTouchedOnIndex(uint index)
        {
            return GvrController.IsTouching;
        }

        public override bool IsTouchpadTouchedDownOnIndex(uint index)
        {
            return GvrController.TouchDown;
        }

        public override bool IsTouchpadTouchedUpOnIndex(uint index)
        {
            return GvrController.TouchUp;
        }

        public override bool IsButtonOnePressedOnIndex(uint index)
        {
            return GvrController.AppButton;
        }

        public override bool IsButtonOnePressedDownOnIndex(uint index)
        {
            return GvrController.AppButtonDown;
        }

        public override bool IsButtonOnePressedUpOnIndex(uint index)
        {
            return GvrController.AppButtonUp;
        }

        //public override bool IsButtonOneTouchedOnIndex(uint index)
        //public override bool IsButtonOneTouchedDownOnIndex(uint index)
        //public override bool IsButtonOneTouchedUpOnIndex(uint index)
        //public override bool IsButtonTwoPressedOnIndex(uint index)
        //public override bool IsButtonTwoPressedDownOnIndex(uint index)
        //public override bool IsButtonTwoPressedUpOnIndex(uint index)
        //public override bool IsButtonTwoTouchedOnIndex(uint index)
        //public override bool IsButtonTwoTouchedDownOnIndex(uint index)
        //public override bool IsButtonTwoTouchedUpOnIndex(uint index)
        //public override bool IsStartMenuPressedOnIndex(uint index)
        //public override bool IsStartMenuPressedDownOnIndex(uint index)
        //public override bool IsStartMenuPressedUpOnIndex(uint index)
        //public override bool IsStartMenuTouchedOnIndex(uint index)
        //public override bool IsStartMenuTouchedDownOnIndex(uint index)
        //public override bool IsStartMenuTouchedUpOnIndex(uint index)

        // Quiet the Awake method in SDK_FallbackController
        private void Awake()
        {
        }
    }
#else
    public class SDK_DaydreamController : SDK_FallbackController
    {
    }
#endif
}
