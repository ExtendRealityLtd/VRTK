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

        GetComponent<SteamVR_ControllerEvents>().ApplicationMenuClicked += new ControllerClickedEventHandler(DoApplicationMenuClicked);
        GetComponent<SteamVR_ControllerEvents>().ApplicationMenuUnclicked += new ControllerClickedEventHandler(DoApplicationMenuUnclicked);

        GetComponent<SteamVR_ControllerEvents>().GripClicked += new ControllerClickedEventHandler(DoGripClicked);
        GetComponent<SteamVR_ControllerEvents>().GripUnclicked += new ControllerClickedEventHandler(DoGripUnclicked);

        GetComponent<SteamVR_ControllerEvents>().TouchpadClicked += new ControllerClickedEventHandler(DoTouchpadClicked);
        GetComponent<SteamVR_ControllerEvents>().TouchpadUnclicked += new ControllerClickedEventHandler(DoTouchpadUnclicked);

        GetComponent<SteamVR_ControllerEvents>().TouchpadTouched += new ControllerClickedEventHandler(DoTouchpadTouched);
        GetComponent<SteamVR_ControllerEvents>().TouchpadUntouched += new ControllerClickedEventHandler(DoTouchpadUntouched);
    }

    void DebugLogger(uint index, string button, string action, Vector2 touchpadAxis)
    {
        Debug.Log("Controller on index '" + index + "' " + button + " has been " + action + " / trackpad axis at: " + touchpadAxis);
    }

    void DoTriggerClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "pressed down", e.touchpadAxis);
    }

    void DoTriggerUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "released", e.touchpadAxis);
    }

    void DoApplicationMenuClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "pressed down", e.touchpadAxis);
    }

    void DoApplicationMenuUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "released", e.touchpadAxis);
    }

    void DoGripClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "pressed down", e.touchpadAxis);
    }

    void DoGripUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "released", e.touchpadAxis);
    }

    void DoTouchpadClicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "pressed down", e.touchpadAxis);
    }

    void DoTouchpadUnclicked(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e.touchpadAxis);
    }

    void DoTouchpadTouched(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e.touchpadAxis);
    }

    void DoTouchpadUntouched(object sender, ControllerClickedEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e.touchpadAxis);
    }
}
