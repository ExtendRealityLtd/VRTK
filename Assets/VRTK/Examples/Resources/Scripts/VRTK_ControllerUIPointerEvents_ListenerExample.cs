namespace VRTK.Examples
{
    using UnityEngine;
    using System.Collections;

    public class VRTK_ControllerUIPointerEvents_ListenerExample : MonoBehaviour
    {
        public bool togglePointerOnHit = false;

        private void Start()
        {
            if (GetComponent<VRTK_UIPointer>() == null)
            {
                Debug.LogError("VRTK_ControllerUIPointerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_UIPointer script attached to it");
                return;
            }

            if (togglePointerOnHit)
            {
                GetComponent<VRTK_UIPointer>().activationMode = VRTK_UIPointer.ActivationMethods.Always_On;
            }

            //Setup controller event listeners
            GetComponent<VRTK_UIPointer>().UIPointerElementEnter += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementEnter;
            GetComponent<VRTK_UIPointer>().UIPointerElementExit += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementExit;
        }

        private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementEnter(object sender, UIPointerEventArgs e)
        {
            Debug.Log("UI Pointer entered " + e.currentTarget.name + " on Controller index [" + e.controllerIndex + "] and the state was " + e.isActive);
            if (togglePointerOnHit && GetComponent<VRTK_SimplePointer>())
            {
                GetComponent<VRTK_SimplePointer>().ToggleBeam(true);
            }
        }

        private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementExit(object sender, UIPointerEventArgs e)
        {
            Debug.Log("UI Pointer exited " + e.previousTarget.name + " on Controller index [" + e.controllerIndex + "] and the state was " + e.isActive);
            if (togglePointerOnHit && GetComponent<VRTK_SimplePointer>())
            {
                GetComponent<VRTK_SimplePointer>().ToggleBeam(false);
            }
        }
    }
}