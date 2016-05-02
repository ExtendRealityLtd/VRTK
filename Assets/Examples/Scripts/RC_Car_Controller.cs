using UnityEngine;
using System.Collections;

public class RC_Car_Controller : MonoBehaviour {
    public GameObject rcCar;
    private RC_Car rcCarScript;

    void Start()
    {
        rcCarScript = rcCar.GetComponent<RC_Car>();
        GetComponent<SteamVR_ControllerEvents>().TriggerAxisChanged += new ControllerClickedEventHandler(DoTriggerAxisChanged);
        GetComponent<SteamVR_ControllerEvents>().TouchpadAxisChanged += new ControllerClickedEventHandler(DoTouchpadAxisChanged);

        GetComponent<SteamVR_ControllerEvents>().TriggerUnclicked += new ControllerClickedEventHandler(DoTriggerUnclicked);
        GetComponent<SteamVR_ControllerEvents>().TouchpadUntouched += new ControllerClickedEventHandler(DoTouchpadUntouched);

        GetComponent<SteamVR_ControllerEvents>().ApplicationMenuClicked += new ControllerClickedEventHandler(DoApplicationMenuClicked);
    }

    void DoTouchpadAxisChanged(object sender, ControllerClickedEventArgs e)
    {
        rcCarScript.SetTouchAxis(e.touchpadAxis);
    }

    void DoTriggerAxisChanged(object sender, ControllerClickedEventArgs e)
    {
        rcCarScript.SetTriggerAxis(e.buttonPressure);
    }

    void DoTouchpadUntouched(object sender, ControllerClickedEventArgs e)
    {
        rcCarScript.SetTouchAxis(Vector2.zero);
    }

    void DoTriggerUnclicked(object sender, ControllerClickedEventArgs e)
    {
        rcCarScript.SetTriggerAxis(0f);
    }

    void DoApplicationMenuClicked(object sender, ControllerClickedEventArgs e)
    {
        rcCarScript.Reset();
    }
}
