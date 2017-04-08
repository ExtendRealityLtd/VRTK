namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTK_RoomExtender_ControllerExample : MonoBehaviour
    {
        protected VRTK_RoomExtender roomExtender;

        // Use this for initialization
        private void Start()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_RoomExtender_ControllerExample", "VRTK_ControllerEvents", "the Controller Alias"));
                return;
            }
            if (FindObjectOfType<VRTK_RoomExtender>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_SCENE, "VRTK_RoomExtender_ControllerExample", "VRTK_RoomExtender"));
                return;
            }
            roomExtender = FindObjectOfType<VRTK_RoomExtender>();
            //Setup controller event listeners
            GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
            GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
            GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += new ControllerInteractionEventHandler(DoSwitchMovementFunction);
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

        private void DoSwitchMovementFunction(object sender, ControllerInteractionEventArgs e)
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
}