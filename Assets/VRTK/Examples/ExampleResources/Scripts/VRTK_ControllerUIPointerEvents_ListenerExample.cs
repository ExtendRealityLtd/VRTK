namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTK_ControllerUIPointerEvents_ListenerExample : MonoBehaviour
    {
        public bool togglePointerOnHit = false;

        private void Start()
        {
            if (GetComponent<VRTK_UIPointer>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerUIPointerEvents_ListenerExample", "VRTK_UIPointer", "the Controller Alias"));
                return;
            }

            if (togglePointerOnHit)
            {
                GetComponent<VRTK_UIPointer>().activationMode = VRTK_UIPointer.ActivationMethods.AlwaysOn;
            }

            //Setup controller event listeners
            GetComponent<VRTK_UIPointer>().UIPointerElementEnter += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementEnter;
            GetComponent<VRTK_UIPointer>().UIPointerElementExit += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementExit;
            GetComponent<VRTK_UIPointer>().UIPointerElementClick += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementClick;
            GetComponent<VRTK_UIPointer>().UIPointerElementDragStart += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragStart;
            GetComponent<VRTK_UIPointer>().UIPointerElementDragEnd += VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragEnd;
        }

        private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementEnter(object sender, UIPointerEventArgs e)
        {
            VRTK_Logger.Info("UI Pointer entered " + e.currentTarget.name + " on Controller index [" + e.controllerIndex + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
            if (togglePointerOnHit && GetComponent<VRTK_Pointer>())
            {
                GetComponent<VRTK_Pointer>().Toggle(true);
            }
        }

        private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementExit(object sender, UIPointerEventArgs e)
        {
            VRTK_Logger.Info("UI Pointer exited " + e.previousTarget.name + " on Controller index [" + e.controllerIndex + "] and the state was " + e.isActive);
            if (togglePointerOnHit && GetComponent<VRTK_Pointer>())
            {
                GetComponent<VRTK_Pointer>().Toggle(false);
            }
        }

        private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementClick(object sender, UIPointerEventArgs e)
        {
            VRTK_Logger.Info("UI Pointer clicked " + e.currentTarget.name + " on Controller index [" + e.controllerIndex + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
        }

        private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragStart(object sender, UIPointerEventArgs e)
        {
            VRTK_Logger.Info("UI Pointer started dragging " + e.currentTarget.name + " on Controller index [" + e.controllerIndex + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
        }

        private void VRTK_ControllerUIPointerEvents_ListenerExample_UIPointerElementDragEnd(object sender, UIPointerEventArgs e)
        {
            VRTK_Logger.Info("UI Pointer stopped dragging " + e.currentTarget.name + " on Controller index [" + e.controllerIndex + "] and the state was " + e.isActive + " ### World Position: " + e.raycastResult.worldPosition);
        }
    }
}