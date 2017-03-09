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
                Debug.LogError("VRTK_RoomExtender_ControllerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }
            if (FindObjectOfType<VRTK_RoomExtender>() == null)
            {
                Debug.LogError("VRTK_RoomExtender script is required.");
                return;
            }
            roomExtender = FindObjectOfType<VRTK_RoomExtender>();
            //Setup controller event listeners
            GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
            GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);
            GetComponent<VRTK_ControllerEvents>().AliasMenuOn += new ControllerInteractionEventHandler(DoSwitchMovementFunction);
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