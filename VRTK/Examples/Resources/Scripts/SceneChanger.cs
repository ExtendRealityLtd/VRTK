namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneChanger : MonoBehaviour
    {
        private bool canPress;
        private uint controllerIndex;

        private void Awake()
        {
            canPress = false;
            Invoke("ResetPress", 1f);
            DynamicGI.UpdateEnvironment();
        }

        private bool ForwardPressed()
        {
            if (controllerIndex >= uint.MaxValue)
            {
                return false;
            }
            if (canPress && VRTK_SDK_Bridge.IsTriggerPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsGripPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsTouchpadPressedOnIndex(controllerIndex))
            {
                return true;
            }
            return false;
        }

        private bool BackPressed()
        {
            if (controllerIndex >= uint.MaxValue)
            {
                return false;
            }

            if (canPress && VRTK_SDK_Bridge.IsTriggerPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsGripPressedOnIndex(controllerIndex) && VRTK_SDK_Bridge.IsButtonOnePressedOnIndex(controllerIndex))
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
            var rightHand = VRTK_DeviceFinder.GetControllerRightHand(true);
            controllerIndex = VRTK_DeviceFinder.GetControllerIndex(rightHand);
            if (ForwardPressed() || Input.GetKeyUp(KeyCode.Space))
            {
                var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
                {
                    nextSceneIndex = 0;
                }
                SceneManager.LoadScene(nextSceneIndex);
            }

            if (BackPressed() || Input.GetKeyUp(KeyCode.Backspace))
            {
                var previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
                if (previousSceneIndex < 0)
                {
                    previousSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
                }
                SceneManager.LoadScene(previousSceneIndex);
            }
        }
    }
}