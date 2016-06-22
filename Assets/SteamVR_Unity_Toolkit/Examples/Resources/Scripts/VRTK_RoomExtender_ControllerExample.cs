using UnityEngine;
using System.Collections;
using VRTK;

public class VRTK_RoomExtender_ControllerExample: MonoBehaviour {

    public bool pressToEnable = true;

    protected VRTK_RoomExtender roomExtender;

    // Use this for initialization
    void Start () {
        if (GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("VRTK_RoomExtender_ControllerExample is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
            return;
        }
        if(FindObjectOfType<VRTK_RoomExtender>() == null)
        {
            Debug.LogError("VRTK_RoomExtender is required to be attached to the CameraRig that has the VRTK_RoomExtender script attached to it");
            return;
        }
        roomExtender = FindObjectOfType<VRTK_RoomExtender>();
        //Setup controller event listeners
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
    }

    void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        if (pressToEnable)
        {
            enableAdditionalMovement(e);
        }
        else
        {
            disableAdditionalMovement();
        }
    }

    void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        if (pressToEnable)
        {
            disableAdditionalMovement();
        }
        else
        {
            enableAdditionalMovement(e);
        }
    }

    void enableAdditionalMovement(ControllerInteractionEventArgs e)
    {
        roomExtender.additionalMovementMultiplier = e.touchpadAxis.magnitude * 5 > 1 ? e.touchpadAxis.magnitude * 5 : 1;
        roomExtender.additionalMovementEnabled = true;
    }

    void disableAdditionalMovement()
    {
        roomExtender.additionalMovementEnabled = false;
    }
}
