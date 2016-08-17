using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private SteamVR_TrackedObject controller;
    private bool canPress;

    private void Awake()
    {
        var manager = FindObjectOfType<SteamVR_ControllerManager>();
        controller = manager.right.GetComponent<SteamVR_TrackedObject>();
        canPress = false;
        Invoke("ResetPress", 1f);
        DynamicGI.UpdateEnvironment();
    }

    private bool ForwardPressed()
    {
        var controllerIndex = (uint)controller.index;
        if (controllerIndex >= uint.MaxValue)
        {
            return false;
        }

        var device = SteamVR_Controller.Input((int)controllerIndex);
        if (canPress && device.GetPress(SteamVR_Controller.ButtonMask.Trigger) && device.GetPress(SteamVR_Controller.ButtonMask.Grip) && device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            return true;
        }
        return false;
    }

    private bool BackPressed()
    {
        var controllerIndex = (uint)controller.index;
        if (controllerIndex >= uint.MaxValue)
        {
            return false;
        }

        var device = SteamVR_Controller.Input((int)controllerIndex);
        if (canPress && device.GetPress(SteamVR_Controller.ButtonMask.Trigger) && device.GetPress(SteamVR_Controller.ButtonMask.Grip) && device.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu))
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
