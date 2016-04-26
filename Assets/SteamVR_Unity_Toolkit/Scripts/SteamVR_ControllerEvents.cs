using UnityEngine;
using System.Collections;

public struct ControllerClickedEventArgs
{
    public uint controllerIndex;
    public Vector2 touchpadAxis;
}

public delegate void ControllerClickedEventHandler(object sender, ControllerClickedEventArgs e);

public class SteamVR_ControllerEvents : MonoBehaviour {
    public enum ButtonAlias
    {
        Trigger,
        Grip,
        Touchpad_Touch,
        Touchpad_Press,
        Application_Menu
    }

    public ButtonAlias pointerToggleButton = ButtonAlias.Grip;
    public ButtonAlias interactToggleButton = ButtonAlias.Trigger;
    public ButtonAlias menuToggleButton = ButtonAlias.Application_Menu;

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

    public event ControllerClickedEventHandler AliasPointerOn;
    public event ControllerClickedEventHandler AliasPointerOff;

    public event ControllerClickedEventHandler AliasInteractOn;
    public event ControllerClickedEventHandler AliasInteractOff;

    public event ControllerClickedEventHandler AliasMenuOn;
    public event ControllerClickedEventHandler AliasMenuOff;

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

    public virtual void OnAliasPointerOn(ControllerClickedEventArgs e)
    {
        if (AliasPointerOn != null)
            AliasPointerOn(this, e);
    }

    public virtual void OnAliasPointerOff(ControllerClickedEventArgs e)
    {
        if (AliasPointerOff != null)
            AliasPointerOff(this, e);
    }

    public virtual void OnAliasInteractOn(ControllerClickedEventArgs e)
    {
        if (AliasInteractOn != null)
            AliasInteractOn(this, e);
    }

    public virtual void OnAliasInteractOff(ControllerClickedEventArgs e)
    {
        if (AliasInteractOff != null)
            AliasInteractOff(this, e);
    }

    public virtual void OnAliasMenuOn(ControllerClickedEventArgs e)
    {
        if (AliasMenuOn != null)
            AliasMenuOn(this, e);
    }

    public virtual void OnAliasMenuOff(ControllerClickedEventArgs e)
    {
        if (AliasMenuOff != null)
            AliasMenuOff(this, e);
    }

    ControllerClickedEventArgs SetButtonEvent(ref bool buttonBool, bool value)
    {
        buttonBool = value;
        ControllerClickedEventArgs e;
        e.controllerIndex = controllerIndex;
        e.touchpadAxis = device.GetAxis();
        return e;
    }

    void EmitAlias(ButtonAlias type, bool touchDown, ref bool buttonBool)
    {
        if (pointerToggleButton == type)
        {
            if (touchDown)
            {
                OnAliasPointerOn(SetButtonEvent(ref buttonBool, true));
            } else
            {
                OnAliasPointerOff(SetButtonEvent(ref buttonBool, false));
            }
        }

        if (interactToggleButton == type)
        {
            if (touchDown)
            {
                OnAliasInteractOn(SetButtonEvent(ref buttonBool, true));
            }
            else
            {
                OnAliasInteractOff(SetButtonEvent(ref buttonBool, false));
            }
        }

        if (menuToggleButton == type)
        {
            if (touchDown)
            {
                OnAliasMenuOn(SetButtonEvent(ref buttonBool, true));
            }
            else
            {
                OnAliasMenuOff(SetButtonEvent(ref buttonBool, false));
            }
        }
    }

    void Update()
    {
        controllerIndex = (uint)trackedController.index;
        device = SteamVR_Controller.Input((int)controllerIndex);

        //Trigger
        if(device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            OnTriggerClicked(SetButtonEvent(ref triggerPressed, true));
            EmitAlias(ButtonAlias.Trigger, true, ref triggerPressed);
        } else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            OnTriggerUnclicked(SetButtonEvent(ref triggerPressed, false));
            EmitAlias(ButtonAlias.Trigger, false, ref triggerPressed);
        }

        //ApplicationMenu
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            OnApplicationMenuClicked(SetButtonEvent(ref applicationMenuPressed, true));
            EmitAlias(ButtonAlias.Application_Menu, true, ref applicationMenuPressed);
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {

            OnApplicationMenuUnclicked(SetButtonEvent(ref applicationMenuPressed, false));
            EmitAlias(ButtonAlias.Application_Menu, false, ref applicationMenuPressed);
        }

        //Grip
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            OnGripClicked(SetButtonEvent(ref gripPressed, true));
            EmitAlias(ButtonAlias.Grip, true, ref gripPressed);
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
        {
            OnGripUnclicked(SetButtonEvent(ref gripPressed, false));
            EmitAlias(ButtonAlias.Grip, false, ref gripPressed);
        }

        //Touchpad Clicked
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadClicked(SetButtonEvent(ref touchpadPressed, true));
            EmitAlias(ButtonAlias.Touchpad_Press, true, ref touchpadPressed);
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadUnclicked(SetButtonEvent(ref touchpadPressed, false));
            EmitAlias(ButtonAlias.Touchpad_Press, false, ref touchpadPressed);
        }

        //Touchpad Touched
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadTouched(SetButtonEvent(ref touchpadTouched, true));
            EmitAlias(ButtonAlias.Touchpad_Touch, true, ref touchpadTouched);
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadUntouched(SetButtonEvent(ref touchpadTouched, false));
            EmitAlias(ButtonAlias.Touchpad_Touch, false, ref touchpadTouched);
        }
    }
}
