namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneChanger : MonoBehaviour
    {
        private bool canPress;
        private VRTK_ControllerReference controllerReference;

        private void Awake()
        {
            canPress = false;
            Invoke("ResetPress", 1f);
            DynamicGI.UpdateEnvironment();
        }

        private bool ForwardPressed()
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }
            if (canPress &&
                VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.Press, controllerReference))
            {
                return true;
            }
            return false;
        }

        private bool BackPressed()
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }

            if (canPress &&
                VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.Press, controllerReference))
            {
                return true;
            }
            return false;
        }

        private void ResetPress()
        {
            canPress = true;
        }

        private void Update()
        {
            GameObject rightHand = VRTK_DeviceFinder.GetControllerRightHand(true);
            controllerReference = VRTK_ControllerReference.GetControllerReference(rightHand);
            if (ForwardPressed() || Input.GetKeyUp(KeyCode.Space))
            {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
                {
                    nextSceneIndex = 0;
                }
                VRTK_SDKManager.instance.UnloadSDKSetup();
                SceneManager.LoadScene(nextSceneIndex);
            }

            if (BackPressed() || Input.GetKeyUp(KeyCode.Backspace))
            {
                int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
                if (previousSceneIndex < 0)
                {
                    previousSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
                }
                VRTK_SDKManager.instance.UnloadSDKSetup();
                SceneManager.LoadScene(previousSceneIndex);
            }
        }
    }
}