using UnityEngine;
using System.Collections;

public struct ControllerClickedEventArgs
{
    public uint controllerIndex;
    public float buttonPressure;
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
    public ButtonAlias grabToggleButton = ButtonAlias.Trigger;
    public ButtonAlias useToggleButton = ButtonAlias.Trigger;
    public ButtonAlias menuToggleButton = ButtonAlias.Application_Menu;

    public int axisFidelity = 1;

    public bool triggerPressed = false;
    public bool triggerAxisChanged = false;
    public bool applicationMenuPressed = false;
    public bool touchpadPressed = false;
    public bool touchpadTouched = false;
    public bool touchpadAxisChanged = false;
    public bool gripPressed = false;

    public event ControllerClickedEventHandler TriggerClicked;
    public event ControllerClickedEventHandler TriggerUnclicked;

    public event ControllerClickedEventHandler TriggerAxisChanged;

    public event ControllerClickedEventHandler ApplicationMenuClicked;
    public event ControllerClickedEventHandler ApplicationMenuUnclicked;

    public event ControllerClickedEventHandler GripClicked;
    public event ControllerClickedEventHandler GripUnclicked;

    public event ControllerClickedEventHandler TouchpadClicked;
    public event ControllerClickedEventHandler TouchpadUnclicked;

    public event ControllerClickedEventHandler TouchpadTouched;
    public event ControllerClickedEventHandler TouchpadUntouched;

    public event ControllerClickedEventHandler TouchpadAxisChanged;

    public event ControllerClickedEventHandler AliasPointerOn;
    public event ControllerClickedEventHandler AliasPointerOff;

    public event ControllerClickedEventHandler AliasGrabOn;
    public event ControllerClickedEventHandler AliasGrabOff;

    public event ControllerClickedEventHandler AliasUseOn;
    public event ControllerClickedEventHandler AliasUseOff;

    public event ControllerClickedEventHandler AliasMenuOn;
    public event ControllerClickedEventHandler AliasMenuOff;

    private uint controllerIndex;
    private SteamVR_TrackedObject trackedController;
    private SteamVR_Controller.Device device;

    private Vector2 touchpadAxis = Vector2.zero;
    private Vector2 triggerAxis = Vector2.zero;

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

    public virtual void OnTriggerAxisChanged(ControllerClickedEventArgs e)
    {
        if (TriggerAxisChanged != null)
            TriggerAxisChanged(this, e);
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

    public virtual void OnTouchpadAxisChanged(ControllerClickedEventArgs e)
    {
        if (TouchpadAxisChanged != null)
            TouchpadAxisChanged(this, e);
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

    public virtual void OnAliasGrabOn(ControllerClickedEventArgs e)
    {
        if (AliasGrabOn != null)
            AliasGrabOn(this, e);
    }

    public virtual void OnAliasGrabOff(ControllerClickedEventArgs e)
    {
        if (AliasGrabOff != null)
            AliasGrabOff(this, e);
    }

    public virtual void OnAliasUseOn(ControllerClickedEventArgs e)
    {
        if (AliasUseOn != null)
            AliasUseOn(this, e);
    }

    public virtual void OnAliasUseOff(ControllerClickedEventArgs e)
    {
        if (AliasUseOff != null)
            AliasUseOff(this, e);
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

    ControllerClickedEventArgs SetButtonEvent(ref bool buttonBool, bool value, float buttonPressure)
    {
        buttonBool = value;
        ControllerClickedEventArgs e;
        e.controllerIndex = controllerIndex;
        e.buttonPressure = buttonPressure;
        e.touchpadAxis = device.GetAxis();
        return e;
    }

    void Awake()
    {
        trackedController = GetComponent<SteamVR_TrackedObject>();
    }

    void EmitAlias(ButtonAlias type, bool touchDown, float buttonPressure, ref bool buttonBool)
    {
        if (pointerToggleButton == type)
        {
            if (touchDown)
            {
                OnAliasPointerOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
            } else
            {
                OnAliasPointerOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
            }
        }

        if (grabToggleButton == type)
        {
            if (touchDown)
            {
                OnAliasGrabOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
            }
            else
            {
                OnAliasGrabOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
            }
        }

        if (useToggleButton == type)
        {
            if (touchDown)
            {
                OnAliasUseOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
            }
            else
            {
                OnAliasUseOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
            }
        }

        if (menuToggleButton == type)
        {
            if (touchDown)
            {
                OnAliasMenuOn(SetButtonEvent(ref buttonBool, true, buttonPressure));
            }
            else
            {
                OnAliasMenuOff(SetButtonEvent(ref buttonBool, false, buttonPressure));
            }
        }
    }

    bool Vector2ShallowEquals(Vector2 vectorA, Vector2 vectorB)
    {
        return (vectorA.x.ToString("F" + axisFidelity) == vectorB.x.ToString("F" + axisFidelity) &&
                vectorA.y.ToString("F" + axisFidelity) == vectorB.y.ToString("F" + axisFidelity));
    }

    void Update()
    {
        controllerIndex = (uint)trackedController.index;
        device = SteamVR_Controller.Input((int)controllerIndex);

        Vector2 currentTriggerAxis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
        Vector2 currentTouchpadAxis = device.GetAxis();

        if (Vector2ShallowEquals(triggerAxis, currentTriggerAxis))
        {
            triggerAxisChanged = false;
        } else
        {
            OnTriggerAxisChanged(SetButtonEvent(ref triggerPressed, true, currentTriggerAxis.x));
            triggerAxisChanged = true;
        }

        if(Vector2ShallowEquals(touchpadAxis, currentTouchpadAxis))
        {
            touchpadAxisChanged = false;
        } else
        {
            OnTouchpadAxisChanged(SetButtonEvent(ref touchpadTouched, true, 1f));
            touchpadAxisChanged = true;
        }

        touchpadAxis = new Vector2(currentTouchpadAxis.x, currentTouchpadAxis.y);
        triggerAxis = new Vector2(currentTriggerAxis.x, currentTriggerAxis.y);

        //Trigger
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            OnTriggerClicked(SetButtonEvent(ref triggerPressed, true, currentTriggerAxis.x));
            EmitAlias(ButtonAlias.Trigger, true, currentTriggerAxis.x, ref triggerPressed);
        } else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            OnTriggerUnclicked(SetButtonEvent(ref triggerPressed, false, 0f));
            EmitAlias(ButtonAlias.Trigger, false, 0f, ref triggerPressed);
        }

        //ApplicationMenu
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            OnApplicationMenuClicked(SetButtonEvent(ref applicationMenuPressed, true, 1f));
            EmitAlias(ButtonAlias.Application_Menu, true, 1f, ref applicationMenuPressed);
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {

            OnApplicationMenuUnclicked(SetButtonEvent(ref applicationMenuPressed, false, 0f));
            EmitAlias(ButtonAlias.Application_Menu, false, 0f, ref applicationMenuPressed);
        }

        //Grip
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
        {
            OnGripClicked(SetButtonEvent(ref gripPressed, true, 1f));
            EmitAlias(ButtonAlias.Grip, true, 1f, ref gripPressed);
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
        {
            OnGripUnclicked(SetButtonEvent(ref gripPressed, false, 0f));
            EmitAlias(ButtonAlias.Grip, false, 0f, ref gripPressed);
        }

        //Touchpad Clicked
        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadClicked(SetButtonEvent(ref touchpadPressed, true, 1f));
            EmitAlias(ButtonAlias.Touchpad_Press, true, 1f, ref touchpadPressed);
        }
        else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadUnclicked(SetButtonEvent(ref touchpadPressed, false, 0f));
            EmitAlias(ButtonAlias.Touchpad_Press, false, 0f, ref touchpadPressed);
        }

        //Touchpad Touched
        if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadTouched(SetButtonEvent(ref touchpadTouched, true, 1f));
            EmitAlias(ButtonAlias.Touchpad_Touch, true, 1f, ref touchpadTouched);
        }
        else if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            OnTouchpadUntouched(SetButtonEvent(ref touchpadTouched, false, 0f));
            EmitAlias(ButtonAlias.Touchpad_Touch, false, 0f, ref touchpadTouched);
        }
    }
}
