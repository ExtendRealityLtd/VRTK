using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using VRTK;

public class UISceneChanger : MonoBehaviour {
    private SteamVR_TrackedObject controller;

    public Event SceneChangeButtons;
    public GameObject Rig;
    public GameObject Head;
    public GameObject SceneCanvas;
    public float OffSet;
    public GameObject ControllerRight;

    private bool HadSimplePointer;
    private bool HadBenzer;
    private bool HadUIPointer;
    private Vector3 OffSetVector;

    private void Awake()
    {
        SteamVR_ControllerManager manager = FindObjectOfType<SteamVR_ControllerManager>();
        controller = manager.right.GetComponent<SteamVR_TrackedObject>();
        DynamicGI.UpdateEnvironment();

        Rig = FindObjectOfType<SteamVR_PlayArea>().gameObject;
        Head = FindObjectOfType<SteamVR_Camera>().gameObject;
        SceneCanvas = transform.GetChild(0).gameObject;

        SteamVR_ControllerManager Manager = Rig.GetComponent<SteamVR_ControllerManager>();
        ControllerRight = Manager.right;
        SceneCanvas.SetActive(false);
    }

    public void ShowUISceneChanger()
    {
        //3 things
        SetTransformToRig();
        SetControllerPointer();
        SceneCanvas.SetActive(!SceneCanvas.activeSelf);
    }

    public void ChangeScene(int numberOfScene)
    {
        Debug.Log("Change to scene: " + numberOfScene);

        var SceneIndex = numberOfScene - 1;

        SceneManager.LoadScene(SceneIndex);
        
    }

    private bool UISceneChangerButtonPressed()
    {
        var controllerIndex = (uint)controller.index;
        if (controllerIndex >= uint.MaxValue)
        {
            return false;
        }

        var device = SteamVR_Controller.Input((int)controllerIndex);
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && device.GetPress(SteamVR_Controller.ButtonMask.Grip)
            && device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)
            || device.GetPress(SteamVR_Controller.ButtonMask.Trigger) && device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)
            && device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)
            || device.GetPress(SteamVR_Controller.ButtonMask.Trigger) && device.GetPress(SteamVR_Controller.ButtonMask.Grip)
            && device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Debug.Log("Scene Changer Button Pressed");
            return true;
        }
        return false;
    }

    public void SetTransformToRig()
    {
        transform.SetParent(Head.transform);
        transform.localPosition = new Vector3(0, 0, OffSet);
        transform.parent = null;
        transform.LookAt(VRTK_DeviceFinder.HeadsetCamera().gameObject.transform);
    }

    public void SetControllerPointer()
    {
        if (!SceneCanvas.activeSelf)
        {
            if (ControllerRight.GetComponent<VRTK_SimplePointer>() == null)
            {
                HadSimplePointer = false;
                ControllerRight.AddComponent<VRTK_ControllerEvents>();
                ControllerRight.AddComponent<VRTK_SimplePointer>();
                
            }
            else
                HadSimplePointer = true;
            if (ControllerRight.GetComponent<VRTK_UIPointer>() == null)
            {
                HadUIPointer = false;
                ControllerRight.AddComponent<VRTK_UIPointer>();
            }
            else
                HadUIPointer = true;
            if (ControllerRight.GetComponent<VRTK_BezierPointer>())
            {
                HadBenzer = true;
                ControllerRight.GetComponent<VRTK_BezierPointer>().enabled = false;
            }
            else
                HadBenzer = false;
        }
        else
        {
            if(!HadSimplePointer)
            {
                Destroy(ControllerRight.GetComponent<VRTK_SimplePointer>());
                Destroy(gameObject.GetComponent<VRTK_EventSystemVRInput>());
            }
            if(HadBenzer)
            {
                ControllerRight.GetComponent<VRTK_BezierPointer>().enabled = true;
            }
            if(!HadUIPointer)
            {
                Destroy(ControllerRight.GetComponent<VRTK_UIPointer>());
            }
        }

        
    }

    private void Update()
    {
        if (UISceneChangerButtonPressed() || Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Toggle UISceneChanger");
            ShowUISceneChanger();
        }
        
    }
}
