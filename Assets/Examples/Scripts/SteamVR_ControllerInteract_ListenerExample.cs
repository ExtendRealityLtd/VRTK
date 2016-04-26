using UnityEngine;
using System.Collections;

public class SteamVR_ControllerInteract_ListenerExample : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        if (GetComponent<SteamVR_ControllerInteract>() == null)
        {
            Debug.LogError("SteamVR_ControllerInteracts_ListenerExample is required to be attached to a SteamVR Controller that has the SteamVR_ControllerInteract script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<SteamVR_ControllerInteract>().ControllerTouchInteractableObject += new ControllerInteractEventHandler(DoInteractTouch);
        GetComponent<SteamVR_ControllerInteract>().ControllerUntouchInteractableObject += new ControllerInteractEventHandler(DoInteractUntouch);
        GetComponent<SteamVR_ControllerInteract>().ControllerGrabInteractableObject += new ControllerInteractEventHandler(DoInteractGrab);
        GetComponent<SteamVR_ControllerInteract>().ControllerUngrabInteractableObject += new ControllerInteractEventHandler(DoInteractUngrab);
    }

    void DebugLogger(uint index, string action, GameObject target)
    {
        Debug.Log("Controller on index '" + index + "' is " + action + " an object named " + target.name);
    }

    void DoInteractTouch(object sender, ControllerInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHING", e.target);
    }

    void DoInteractUntouch(object sender, ControllerInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "NO LONGER TOUCHING", e.target);
    }

    void DoInteractGrab(object sender, ControllerInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRABBING", e.target);
    }

    void DoInteractUngrab(object sender, ControllerInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "NO LONGER GRABBING", e.target);
    }
}
