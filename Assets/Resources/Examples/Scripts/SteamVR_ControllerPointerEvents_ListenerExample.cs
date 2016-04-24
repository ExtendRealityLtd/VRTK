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
    }

    void DebugLogger(uint index, string action, Transform target, float distance)
    {
        Debug.Log("Controller on index '" + index + "' is " + action + " at a distance of " + distance + " on object named " + target.name );
    }

    void DoPointerIn(object sender, ControllerPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER IN", e.target, e.distance);
    }

    void DoPointerOut(object sender, ControllerPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER OUT", e.target, e.distance);
    }
}
