using UnityEngine;
using System.Collections;
using VRTK;

public class VRTK_ControllerEvents_ListenerExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
            return;
        }
        
        //Setup controller event listeners
        GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerClickedEventHandler(DoTriggerClicked);
        GetComponent<VRTK_ControllerEvents>().TriggerUnclicked += new ControllerClickedEventHandler(DoTriggerUnclicked);

        GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += new ControllerClickedEventHandler(DoTriggerAxisChanged);

        GetComponent<VRTK_ControllerEvents>().ApplicationMenuClicked += new ControllerClickedEventHandler(DoApplicationMenuClicked);
        GetComponent<VRTK_ControllerEvents>().ApplicationMenuUnclicked += new ControllerClickedEventHandler(DoApplicationMenuUnclicked);

        GetComponent<VRTK_ControllerEvents>().GripClicked += new ControllerClickedEventHandler(DoGripClicked);
        GetComponent<VRTK_ControllerEvents>().GripUnclicked += new ControllerClickedEventHandler(DoGripUnclicked);

        GetComponent<VRTK_ControllerEvents>().TouchpadClicked += new ControllerClickedEventHandler(DoTouchpadClicked);
        GetComponent<VRTK_ControllerEvents>().TouchpadUnclicked += new ControllerClickedEventHandler(DoTouchpadUnclicked);

        GetComponent<VRTK_ControllerEvents>().TouchpadTouched += new ControllerClickedEventHandler(DoTouchpadTouched);
        GetComponent<VRTK_ControllerEvents>().TouchpadUntouched += new ControllerClickedEventHandler(DoTouchpadUntouched);

        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerClickedEventHandler(DoTouchpadAxisChanged);
    }

    void DebugLogger(uint index, string button, string action, ControllerClickedEventArgs e)
    {
        Debug.Log("Controller on index '" + index + "' " + button + " has been " + action 
                + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
    }

    void DoTriggerClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "pressed down", e);
    }

    void DoTriggerUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "released", e);
    }

    void DoTriggerAxisChanged(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "axis changed", e);
    }

    void DoApplicationMenuClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "pressed down", e);
    }

    void DoApplicationMenuUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "released", e);
    }

    void DoGripClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "pressed down", e);
    }

    void DoGripUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "released", e);
    }

    void DoTouchpadClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "pressed down", e);
    }

    void DoTouchpadUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e);
    }

    void DoTouchpadTouched(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e);
    }

    void DoTouchpadUntouched(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e);
    }

    void DoTouchpadAxisChanged(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "axis changed", e);
    }
}
