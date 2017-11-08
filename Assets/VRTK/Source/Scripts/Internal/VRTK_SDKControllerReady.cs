namespace VRTK
{
    using UnityEngine;

    public abstract class VRTK_SDKControllerReady : MonoBehaviour
    {
        protected VRTK_SDKManager sdkManager;
        protected SDK_BaseController previousControllerSDK;

        protected virtual void OnEnable()
        {
            sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                sdkManager.LoadedSetupChanged += LoadedSetupChanged;
            }
            CheckControllersReady();
        }

        protected virtual void OnDisable()
        {
            if (sdkManager != null)
            {
                sdkManager.LoadedSetupChanged -= LoadedSetupChanged;
                UnregisterPreviousLeftController();
                UnregisterPreviousRightController();
            }
        }

        protected virtual void LoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            CheckControllersReady();
            previousControllerSDK = VRTK_SDK_Bridge.GetControllerSDK();
        }

        protected virtual void CheckControllersReady()
        {
            RegisterLeftControllerReady();
            RegisterRightControllerReady();

            VRTK_ControllerReference leftRef = VRTK_DeviceFinder.GetControllerReferenceLeftHand();
            VRTK_ControllerReference rightRef = VRTK_DeviceFinder.GetControllerReferenceRightHand();

            if (VRTK_ControllerReference.IsValid(leftRef))
            {
                ControllerReady(leftRef);
            }

            if (VRTK_ControllerReference.IsValid(rightRef))
            {
                ControllerReady(rightRef);
            }
        }

        protected virtual void UnregisterPreviousLeftController()
        {
            try
            {
                previousControllerSDK.LeftControllerReady -= LeftControllerReady;
            }
            catch (System.Exception)
            {
            }
        }

        protected virtual void UnregisterPreviousRightController()
        {
            try
            {
                previousControllerSDK.RightControllerReady -= RightControllerReady;
            }
            catch (System.Exception)
            {
            }
        }

        protected virtual void RegisterLeftControllerReady()
        {
            UnregisterPreviousLeftController();
            try
            {
                VRTK_SDK_Bridge.GetControllerSDK().LeftControllerReady -= LeftControllerReady;
                VRTK_SDK_Bridge.GetControllerSDK().LeftControllerReady += LeftControllerReady;
            }
            catch (System.Exception)
            {
                VRTK_SDK_Bridge.GetControllerSDK().LeftControllerReady += LeftControllerReady;
            }
        }

        protected virtual void RegisterRightControllerReady()
        {
            UnregisterPreviousRightController();

            try
            {
                VRTK_SDK_Bridge.GetControllerSDK().RightControllerReady -= RightControllerReady;
                VRTK_SDK_Bridge.GetControllerSDK().RightControllerReady += RightControllerReady;
            }
            catch (System.Exception)
            {
                VRTK_SDK_Bridge.GetControllerSDK().RightControllerReady += RightControllerReady;
            }
        }

        protected virtual void RightControllerReady(object sender, VRTKSDKBaseControllerEventArgs e)
        {
            ControllerReady(e.controllerReference);
        }

        protected virtual void LeftControllerReady(object sender, VRTKSDKBaseControllerEventArgs e)
        {
            ControllerReady(e.controllerReference);
        }

        protected virtual void ControllerReady(VRTK_ControllerReference controllerReference)
        {
        }
    }
}