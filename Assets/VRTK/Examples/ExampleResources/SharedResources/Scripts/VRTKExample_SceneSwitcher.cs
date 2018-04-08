namespace VRTK.Examples.Utilities
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class VRTKExample_SceneSwitcher : MonoBehaviour
    {
        public KeyCode backKey = KeyCode.Backspace;
        public KeyCode forwardKey = KeyCode.Space;

        protected int firstSceneIndex = 0;
        protected int lastSceneIndex;

        protected bool pressEnabled;
        protected VRTK_ControllerReference controllerReference;

        protected virtual void Awake()
        {
            DynamicGI.UpdateEnvironment();
        }

        protected virtual void OnEnable()
        {
            lastSceneIndex = SceneManager.sceneCountInBuildSettings - 1;
            pressEnabled = false;
            Invoke("EnablePress", 1f);
        }

        protected virtual void Update()
        {
            GameObject rightHand = VRTK_DeviceFinder.GetControllerRightHand(true);
            controllerReference = VRTK_ControllerReference.GetControllerReference(rightHand);

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex;

            if (ForwardPressed())
            {
                nextSceneIndex++;
                if (nextSceneIndex >= lastSceneIndex)
                {
                    nextSceneIndex = firstSceneIndex;
                }
            }
            else if (BackPressed())
            {
                nextSceneIndex--;
                if (nextSceneIndex < firstSceneIndex)
                {
                    nextSceneIndex = lastSceneIndex - 1;
                }
            }

            if (nextSceneIndex != currentSceneIndex)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }

        protected virtual void EnablePress()
        {
            pressEnabled = true;
        }

        protected virtual bool BackPressed()
        {
            if (Input.GetKeyDown(backKey) || ControllerBackward())
            {
                return true;
            }
            return false;
        }

        protected virtual bool ForwardPressed()
        {
            if (Input.GetKeyDown(forwardKey) || ControllerForward())
            {
                return true;
            }
            return false;
        }

        protected virtual bool ControllerForward()
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }

            return (pressEnabled &&
                    VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                    VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                    VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Press, controllerReference));
        }

        protected virtual bool ControllerBackward()
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return false;
            }

            return (pressEnabled &&
                    VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                    VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.Press, controllerReference) &&
                    VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.Press, controllerReference));
        }
    }
}