using UnityEngine;
using System.Collections;
using VRTK;

public class Remote_Beam_Controller : MonoBehaviour {
    public GameObject remoteBeam;
    private Remote_Beam remoteBeamScript;

    void Start()
    {
        remoteBeamScript = remoteBeam.GetComponent<Remote_Beam>();

        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
    }

    void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        remoteBeamScript.SetTouchAxis(e.touchpadAxis);
    }

    void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        remoteBeamScript.SetTouchAxis(Vector2.zero);
    }
}
