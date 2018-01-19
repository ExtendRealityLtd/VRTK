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

        private bool IsForwardPressed()
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

        private bool IsBackPressed()
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

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex;

            if (IsForwardPressed() || Input.GetKeyUp(KeyCode.Space))
            {
                nextSceneIndex++;
                if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
                    nextSceneIndex = 0;
            }
            else if (IsBackPressed() || Input.GetKeyUp(KeyCode.Backspace))
            {
                nextSceneIndex--;
                if (nextSceneIndex < 0)
                    nextSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
            }

            if (nextSceneIndex == currentSceneIndex)
            {
                return;
            }

            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}