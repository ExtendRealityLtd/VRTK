namespace VRTK.Examples
{
    using UnityEngine;

    public class RC_Car_Controller : MonoBehaviour
    {
        public GameObject rcCar;
        private RC_Car rcCarScript;

        private void Start()
        {
            rcCarScript = rcCar.GetComponent<RC_Car>();
            GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += new ControllerInteractionEventHandler(DoTriggerAxisChanged);
            GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);

            GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);
            GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

            GetComponent<VRTK_ControllerEvents>().ButtonOnePressed += new ControllerInteractionEventHandler(DoCarReset);
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            rcCarScript.SetTouchAxis(e.touchpadAxis);
        }

        private void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            rcCarScript.SetTriggerAxis(e.buttonPressure);
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            rcCarScript.SetTouchAxis(Vector2.zero);
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            rcCarScript.SetTriggerAxis(0f);
        }

        private void DoCarReset(object sender, ControllerInteractionEventArgs e)
        {
            rcCarScript.ResetCar();
        }
    }
}