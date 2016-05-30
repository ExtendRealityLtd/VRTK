using UnityEngine;
using System.Collections;

public class VRTK_ControllerPointerEvents_ListenerExample : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        if (GetComponent<VRTK_SimplePointer>() == null)
        {
            Debug.LogError("VRTK_ControllerPointerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the VRTK_SimplePointer script attached to it");
            return;
        }

        //Setup controller event listeners
        GetComponent<VRTK_SimplePointer>().WorldPointerIn += new WorldPointerEventHandler(DoPointerIn);
        GetComponent<VRTK_SimplePointer>().WorldPointerOut += new WorldPointerEventHandler(DoPointerOut);
        GetComponent<VRTK_SimplePointer>().WorldPointerDestinationSet += new WorldPointerEventHandler(DoPointerDestinationSet);
    }

    void DebugLogger(uint index, string action, Transform target, float distance, Vector3 tipPosition)
    {
        string targetName = (target ? target.name : "<NO VALID TARGET>");
        Debug.Log("Controller on index '" + index + "' is " + action + " at a distance of " + distance + " on object named " + targetName + " - the pointer tip position is/was: " +tipPosition);
    }

    void DoPointerIn(object sender, WorldPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER IN", e.target, e.distance, e.destinationPosition);
    }

    void DoPointerOut(object sender, WorldPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER OUT", e.target, e.distance, e.destinationPosition);
    }

    void DoPointerDestinationSet(object sender, WorldPointerEventArgs e)
    {
        DebugLogger(e.controllerIndex, "POINTER DESTINATION", e.target, e.distance, e.destinationPosition);
    }
}
