﻿using UnityEngine;
using VRTK;

public class VRTK_RoomExtender_ControllerExample : MonoBehaviour
{
    protected VRTK_RoomExtender roomExtender;

    // Use this for initialization
    private void Start()
    {
        if (GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("VRTK_RoomExtender_ControllerExample is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
            return;
        }
        if (FindObjectOfType<VRTK_RoomExtender>() == null)
        {
            Debug.LogError("VRTK_RoomExtender is required to be attached to the CameraRig that has the VRTK_RoomExtender script attached to it");
            return;
        }
        roomExtender = FindObjectOfType<VRTK_RoomExtender>();
        //Setup controller event listeners
        GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
        GetComponent<VRTK_ControllerEvents>().ApplicationMenuPressed += new ControllerInteractionEventHandler(DoApplicationMenuPressed);
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        roomExtender.additionalMovementMultiplier = e.touchpadAxis.magnitude * 5 > 1 ? e.touchpadAxis.magnitude * 5 : 1;
        if (roomExtender.additionalMovementEnabledOnButtonPress)
        {
            EnableAdditionalMovement();
        }
        else
        {
            DisableAdditionalMovement();
        }
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (roomExtender.additionalMovementEnabledOnButtonPress)
        {
            DisableAdditionalMovement();
        }
        else
        {
            EnableAdditionalMovement();
        }
    }

    private void DoApplicationMenuPressed(object sender, ControllerInteractionEventArgs e)
    {
        switch (roomExtender.movementFunction)
        {
            case VRTK_RoomExtender.MovementFunction.Nonlinear:
                roomExtender.movementFunction = VRTK_RoomExtender.MovementFunction.LinearDirect;
                break;
            case VRTK_RoomExtender.MovementFunction.LinearDirect:
                roomExtender.movementFunction = VRTK_RoomExtender.MovementFunction.Nonlinear;
                break;
            default:
                break;
        }
    }

    private void EnableAdditionalMovement()
    {
        roomExtender.additionalMovementEnabled = true;
    }

    private void DisableAdditionalMovement()
    {
        roomExtender.additionalMovementEnabled = false;
    }
}