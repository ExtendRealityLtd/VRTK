using UnityEngine;
using System.Collections;

public class SteamVR_ControllerInteract_ListenerExample : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        if (GetComponent<SteamVR_InteractTouch>() == null || GetComponent<SteamVR_InteractGrab>() == null)
        {
            Debug.LogError("SteamVR_ControllerInteracts_ListenerExample is required to be attached to a SteamVR Controller that has the SteamVR_InteractTouch and SteamVR_InteractGrab script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<SteamVR_InteractTouch>().ControllerTouchInteractableObject += new ObjectInteractEventHandler(DoInteractTouch);
        GetComponent<SteamVR_InteractTouch>().ControllerUntouchInteractableObject += new ObjectInteractEventHandler(DoInteractUntouch);
        GetComponent<SteamVR_InteractGrab>().ControllerGrabInteractableObject += new ObjectInteractEventHandler(DoInteractGrab);
        GetComponent<SteamVR_InteractGrab>().ControllerUngrabInteractableObject += new ObjectInteractEventHandler(DoInteractUngrab);
    }

    void DebugLogger(uint index, string action, GameObject target)
    {
        Debug.Log("Controller on index '" + index + "' is " + action + " an object named " + target.name);
    }

    void DoInteractTouch(object sender, ObjectInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TOUCHING", e.target);
    }

    void DoInteractUntouch(object sender, ObjectInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "NO LONGER TOUCHING", e.target);
    }

    void DoInteractGrab(object sender, ObjectInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "GRABBING", e.target);
    }

    void DoInteractUngrab(object sender, ObjectInteractEventArgs e)
    {
        DebugLogger(e.controllerIndex, "NO LONGER GRABBING", e.target);
    }
}