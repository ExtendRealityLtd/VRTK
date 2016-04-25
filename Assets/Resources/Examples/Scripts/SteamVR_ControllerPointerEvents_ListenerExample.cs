using UnityEngine;
using System.Collections;

public class SteamVR_ControllerPointerEvents_ListenerExample : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        if (GetComponent<SteamVR_ControllerEvents>() == null)
        {
            Debug.LogError("SteamVR_ControllerPointerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the SteamVR_SimplePointer script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<SteamVR_SimplePointer>().ControllerPointerIn += new ControllerPointerEventHandler(DoPointerIn);
        GetComponent<SteamVR_SimplePointer>().ControllerPointerOut += new ControllerPointerEventHandler(DoPointerOut);
        GetComponent<SteamVR_SimplePointer>().PointerDestinationSet += new ControllerPointerEventHandler(DoPointerDestinationSet);
    }

    void DebugLogger(uint index, string action, Transform target, float distance, Vector3 tipPosition)
    {
        Debug.Log("Controller on index '" + index + "' is " + action + " at a distance of " + distance + " on object named " + target.name + " - the pointer tip position is/was: " +tipPosition);
    }

    void DoPointerIn(object sender, ControllerPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER IN", e.target, e.distance, e.tipPosition);
    }

    void DoPointerOut(object sender, ControllerPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER OUT", e.target, e.distance, e.tipPosition);
    }

    void DoPointerDestinationSet(object sender, ControllerPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER DESTINATION", e.target, e.distance, e.tipPosition);
    }
}
