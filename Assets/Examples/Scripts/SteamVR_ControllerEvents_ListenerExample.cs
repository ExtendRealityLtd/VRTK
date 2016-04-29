using UnityEngine;
using System.Collections;

public class SteamVR_ControllerEvents_ListenerExample : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_ControllerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the SteamVR_ControllerEvents script attached to it");
            return;
        }
        
        //Setup controller event listeners
        GetComponent<SteamVR_ControllerEvents>().TriggerClicked += new ControllerClickedEventHandler(DoTriggerClicked);
        GetComponent<SteamVR_ControllerEvents>().TriggerUnclicked += new ControllerClickedEventHandler(DoTriggerUnclicked);

        GetComponent<SteamVR_ControllerEvents>().TriggerAxisChanged += new ControllerClickedEventHandler(DoTriggerAxisChanged);

        GetComponent<SteamVR_ControllerEvents>().ApplicationMenuClicked += new ControllerClickedEventHandler(DoApplicationMenuClicked);
        GetComponent<SteamVR_ControllerEvents>().ApplicationMenuUnclicked += new ControllerClickedEventHandler(DoApplicationMenuUnclicked);

        GetComponent<SteamVR_ControllerEvents>().GripClicked += new ControllerClickedEventHandler(DoGripClicked);
        GetComponent<SteamVR_ControllerEvents>().GripUnclicked += new ControllerClickedEventHandler(DoGripUnclicked);

        GetComponent<SteamVR_ControllerEvents>().TouchpadClicked += new ControllerClickedEventHandler(DoTouchpadClicked);
        GetComponent<SteamVR_ControllerEvents>().TouchpadUnclicked += new ControllerClickedEventHandler(DoTouchpadUnclicked);

        GetComponent<SteamVR_ControllerEvents>().TouchpadTouched += new ControllerClickedEventHandler(DoTouchpadTouched);
        GetComponent<SteamVR_ControllerEvents>().TouchpadUntouched += new ControllerClickedEventHandler(DoTouchpadUntouched);

        GetComponent<SteamVR_ControllerEvents>().TouchpadAxisChanged += new ControllerClickedEventHandler(DoTouchpadAxisChanged);
    }

    void DebugLogger(uint index, string button, string action, float buttonPressure, Vector2 touchpadAxis)
    {
        Debug.Log("Controller on index '" + index + "' " + button + " has been " + action + " with a pressure of " + buttonPressure + " / trackpad axis at: " + touchpadAxis);
    }

    void DoTriggerClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "pressed down", e.buttonPressure, e.touchpadAxis);
    }

    void DoTriggerUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "released", e.buttonPressure, e.touchpadAxis);
    }

    void DoTriggerAxisChanged(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "axis changed", e.buttonPressure, e.touchpadAxis);
    }

    void DoApplicationMenuClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "pressed down", e.buttonPressure, e.touchpadAxis);
    }

    void DoApplicationMenuUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "released", e.buttonPressure, e.touchpadAxis);
    }

    void DoGripClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "pressed down", e.buttonPressure, e.touchpadAxis);
    }

    void DoGripUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "released", e.buttonPressure, e.touchpadAxis);
    }

    void DoTouchpadClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "pressed down", e.buttonPressure, e.touchpadAxis);
    }

    void DoTouchpadUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e.buttonPressure, e.touchpadAxis);
    }

    void DoTouchpadTouched(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e.buttonPressure, e.touchpadAxis);
    }

    void DoTouchpadUntouched(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e.buttonPressure, e.touchpadAxis);
    }

    void DoTouchpadAxisChanged(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "axis changed", e.buttonPressure, e.touchpadAxis);
    }
}
