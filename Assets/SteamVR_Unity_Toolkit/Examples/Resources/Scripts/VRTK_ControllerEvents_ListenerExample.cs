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
        GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

        GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += new ControllerInteractionEventHandler(DoTriggerAxisChanged);

        GetComponent<VRTK_ControllerEvents>().ApplicationMenuPressed += new ControllerInteractionEventHandler(DoApplicationMenuPressed);
        GetComponent<VRTK_ControllerEvents>().ApplicationMenuReleased += new ControllerInteractionEventHandler(DoApplicationMenuReleased);

        GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
        GetComponent<VRTK_ControllerEvents>().GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

        GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
    }

    void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
    {
        Debug.Log("Controller on index '" + index + "' " + button + " has been " + action 
                + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
    }

    void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "pressed down", e);
    }

    void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "released", e);
    }

    void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "axis changed", e);
    }

    void DoApplicationMenuPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "pressed down", e);
    }

    void DoApplicationMenuReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "APPLICATION MENU", "released", e);
    }

    void DoGripPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "pressed down", e);
    }

    void DoGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRIP", "released", e);
    }

    void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "pressed down", e);
    }

    void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e);
    }

    void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e);
    }

    void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e);
    }

    void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHPAD", "axis changed", e);
    }
}
