using UnityEngine;
using System.Collections;
using VRTK;

public class RC_Car_Controller : MonoBehaviour {
    public GameObject rcCar;
    private RC_Car rcCarScript;

    void Start()
    {
        rcCarScript = rcCar.GetComponent<RC_Car>();
        GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += new ControllerInteractionEventHandler(DoTriggerAxisChanged);
        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);

        GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

        GetComponent<VRTK_ControllerEvents>().ApplicationMenuPressed += new ControllerInteractionEventHandler(DoApplicationMenuPressed);
    }

    void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        rcCarScript.SetTouchAxis(e.touchpadAxis);
    }

    void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        rcCarScript.SetTriggerAxis(e.buttonPressure);
    }

    void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        rcCarScript.SetTouchAxis(Vector2.zero);
    }

    void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        rcCarScript.SetTriggerAxis(0f);
    }

    void DoApplicationMenuPressed(object sender, ControllerInteractionEventArgs e)
    {
        rcCarScript.Reset();
    }
}
