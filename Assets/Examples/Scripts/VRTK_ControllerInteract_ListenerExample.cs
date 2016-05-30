using UnityEngine;
using System.Collections;

public class VRTK_ControllerInteract_ListenerExample : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        if (GetComponent<VRTK_InteractTouch>() == null || GetComponent<VRTK_InteractGrab>() == null)
        {
            Debug.LogError("VRTK_ControllerInteracts_ListenerExample is required to be attached to a SteamVR Controller that has the VRTK_InteractTouch and VRTK_InteractGrab script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<VRTK_InteractTouch>().ControllerTouchInteractableObject += new ObjectInteractEventHandler(DoInteractTouch);
        GetComponent<VRTK_InteractTouch>().ControllerUntouchInteractableObject += new ObjectInteractEventHandler(DoInteractUntouch);
        GetComponent<VRTK_InteractGrab>().ControllerGrabInteractableObject += new ObjectInteractEventHandler(DoInteractGrab);
        GetComponent<VRTK_InteractGrab>().ControllerUngrabInteractableObject += new ObjectInteractEventHandler(DoInteractUngrab);
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