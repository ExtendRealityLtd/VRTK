using UnityEngine;
using System.Collections;

public struct ControllerClickedEventArgs
{
    public uint controllerIndex;
    public Vector2 touchpadAxis;
}

public delegate void ControllerClickedEventHandler(object sender, ControllerClickedEventArgs e);

public class SteamVR_ControllerEvents : MonoBehaviour {
    public bool triggerPressed = false;
    public bool applicationMenuPressed = false;
    public bool touchpadPressed = false;
    public bool touchpadTouched = false;
    public bool gripPressed = false;

    public event ControllerClickedEventHandler TriggerClicked;
    public event ControllerClickedEventHandler TriggerUnclicked;

    public event ControllerClickedEventHandler ApplicationMenuClicked;
    public event ControllerClickedEventHandler ApplicationMenuUnclicked;

    public event ControllerClickedEventHandler GripClicked;
    public event ControllerClickedEventHandler GripUnclicked;

    public event ControllerClickedEventHandler TouchpadClicked;
    public event ControllerClickedEventHandler TouchpadUnclicked;

    public event ControllerClickedEventHandler TouchpadTouched;
    public event ControllerClickedEventHandler TouchpadUntouched;

    private uint controllerIndex;
    private SteamVR_TrackedObject trackedController;
    private SteamVR_Controller.Device device;

    void Awake()
    {
        trackedController = GetComponent<SteamVR_TrackedObject>();        
    }

    public virtual void OnTriggerClicked(ControllerClickedEventArgs e)
    {
        if (TriggerClicked != null)
            TriggerClicked(this, e);
    }

    public virtual void OnTriggerUnclicked(ControllerClickedEventArgs e)
    {
        if (TriggerUnclicked != null)
            TriggerUnclicked(this, e);
    }

    public virtual void OnApplicationMenuClicked(ControllerClickedEventArgs e)
    {
        if (ApplicationMenuClicked != null)
            ApplicationMenuClicked(this, e);
    }

    public virtual void OnApplicationMenuUnclicked(ControllerClickedEventArgs e)
    {
        if (ApplicationMenuUnclicked != null)
            ApplicationMenuUnclicked(this, e);
    }

    public virtual void OnGripClicked(ControllerClickedEventArgs e)
    {
        if (GripClicked != null)
            GripClicked(this, e);
    }

    public virtual void OnGripUnclicked(ControllerClickedEventArgs e)
    {
        if (GripUnclicked != null)
            GripUnclicked(this, e);
    }

    public virtual void OnTouchpadClicked(ControllerClickedEventArgs e)
    {
        if (TouchpadClicked != null)
            TouchpadClicked(this, e);
    }

    public virtual void OnTouchpadUnclicked(ControllerClickedEventArgs e)
    {
        if (TouchpadUnclicked != null)
            TouchpadUnclicked(this, e);
    }

    public virtual void OnTouchpadTouched(ControllerClickedEventArgs e)
    {
        if (TouchpadTouched != null)
            TouchpadTouched(this, e);
    }

    public virtual void OnTouchpadUntouched(ControllerClickedEventArgs e)
    {
        if (TouchpadUntouched != null)
            TouchpadUntouched(this, e);
    }

    ControllerClickedEventArgs SetButtonEvent(ref bool buttonBool, bool value)
    {
        buttonBool = value;
        ControllerClickedEventArgs e;
        e.controllerIndex = controllerIndex;
        e.touchpadAxis = device.GetAxis();
        return e;
    }

    void Update()
    {
        controllerIndex = (uint)trackedController.index;
        device = SteamVR_Controller.Input((int)controllerIndex);

        //Trigger
        if(device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            OnTriggerClicked(SetButtonEvent(ref triggerPressed, true));
        } else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            OnTriggerUnclicked(SetButtonEvent(ref triggerPressed, false));
        }

        //ApplicationMenu
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            OnApplicationMenuClicked(SetButtonEvent(ref applicationMenuPressed, true));
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            OnApplicationMenuUnclicked(SetButtonEvent(ref applicationMenuPressed, false));
        }

        //Grip
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            OnGripClicked(SetButtonEvent(ref gripPressed, true));
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
        {
            OnGripUnclicked(SetButtonEvent(ref gripPressed, false));
        }

        //Touchpad Clicked
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadClicked(SetButtonEvent(ref touchpadPressed, true));
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadUnclicked(SetButtonEvent(ref touchpadPressed, false));
        }

        //Touchpad Touched
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadTouched(SetButtonEvent(ref touchpadTouched, true));
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadUntouched(SetButtonEvent(ref touchpadTouched, false));
        }
    }
}
