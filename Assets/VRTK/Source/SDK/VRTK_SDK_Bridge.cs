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

        public static SDK_BaseController.ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)
        {
            return GetControllerSDK().GetCurrentControllerType(controllerReference);
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

        public static Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetControllerOrigin(controllerReference);
        }

        [System.Obsolete("GenerateControllerPointerOrigin has been deprecated and will be removed in a future version of VRTK.")]
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

        public static bool WaitForControllerModel(SDK_BaseController.ControllerHand hand)
        {
            return GetControllerSDK().WaitForControllerModel(hand);
        }

        public static GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerSDK().GetControllerModel(controller);
        }

        public static GameObject GetControllerModel(SDK_BaseController.ControllerHand hand)
        {
            return GetControllerSDK().GetControllerModel(hand);
        }

        public static SDK_BaseController.ControllerHand GetControllerModelHand(GameObject controllerModel)
        {
            return GetControllerSDK().GetControllerModelHand(controllerModel);
        }

        public static GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetControllerRenderModel(controllerReference);
        }

        public static void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
            GetControllerSDK().SetControllerRenderModelWheel(renderModel, state);
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

        public static Vector3 GetControllerVelocity(VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetVelocity(controllerReference);
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

        public static float GetControllerSenseAxis(SDK_BaseController.ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetButtonSenseAxis(buttonType, controllerReference);
        }

        public static float GetControllerHairlineDelta(SDK_BaseController.ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetButtonHairlineDelta(buttonType, controllerReference);
        }

        public static bool GetControllerButtonState(SDK_BaseController.ButtonTypes buttonType, SDK_BaseController.ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            return GetControllerSDK().GetControllerButtonState(buttonType, pressType, controllerReference);
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

        public static string GetHeadsetType()
        {
            return GetHeadsetSDK().GetHeadsetType();
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