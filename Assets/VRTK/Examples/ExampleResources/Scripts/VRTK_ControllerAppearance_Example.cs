namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTK_ControllerAppearance_Example : MonoBehaviour
    {
        public bool highlightBodyOnlyOnCollision = false;

        private VRTK_ControllerTooltips tooltips;
        private VRTK_ControllerActions actions;
        private VRTK_ControllerEvents events;

        private void Start()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            events = GetComponent<VRTK_ControllerEvents>();
            actions = GetComponent<VRTK_ControllerActions>();
            tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();

            //Setup controller event listeners
            events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
            events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

            events.ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
            events.ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

            events.ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
            events.ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

            events.StartMenuPressed += new ControllerInteractionEventHandler(DoStartMenuPressed);
            events.StartMenuReleased += new ControllerInteractionEventHandler(DoStartMenuReleased);

            events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
            events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

            events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
            events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

            tooltips.ToggleTips(false);
        }

        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
            actions.ToggleHighlightTrigger(true, Color.yellow, 0.5f);
            actions.SetControllerOpacity(0.8f);
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
            actions.ToggleHighlightTrigger(false);
            if (!events.AnyButtonPressed())
            {
                actions.SetControllerOpacity(1f);
            }
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
            actions.ToggleHighlightButtonOne(true, Color.yellow, 0.5f);
            actions.SetControllerOpacity(0.8f);
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
            actions.ToggleHighlightButtonOne(false);
            if (!events.AnyButtonPressed())
            {
                actions.SetControllerOpacity(1f);
            }
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
            actions.ToggleHighlightButtonTwo(true, Color.yellow, 0.5f);
            actions.SetControllerOpacity(0.8f);
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
            actions.ToggleHighlightButtonTwo(false);
            if (!events.AnyButtonPressed())
            {
                actions.SetControllerOpacity(1f);
            }
        }

        private void DoStartMenuPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
            actions.ToggleHighlightStartMenu(true, Color.yellow, 0.5f);
            actions.SetControllerOpacity(0.8f);
        }

        private void DoStartMenuReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
            actions.ToggleHighlightStartMenu(false);
            if (!events.AnyButtonPressed())
            {
                actions.SetControllerOpacity(1f);
            }
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
            actions.ToggleHighlightGrip(true, Color.yellow, 0.5f);
            actions.SetControllerOpacity(0.8f);
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
            actions.ToggleHighlightGrip(false);
            if (!events.AnyButtonPressed())
            {
                actions.SetControllerOpacity(1f);
            }
        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
            actions.ToggleHighlightTouchpad(true, Color.yellow, 0.5f);
            actions.SetControllerOpacity(0.8f);
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
            actions.ToggleHighlightTouchpad(false);
            if (!events.AnyButtonPressed())
            {
                actions.SetControllerOpacity(1f);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
            {
                if (highlightBodyOnlyOnCollision)
                {
                    actions.ToggleHighlighBody(true, Color.yellow, 0.4f);
                }
                else
                {
                    actions.ToggleHighlightController(true, Color.yellow, 0.4f);
                }
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
            {
                if (highlightBodyOnlyOnCollision)
                {
                    actions.ToggleHighlighBody(false);
                }
                else
                {
                    actions.ToggleHighlightController(false);
                }
            }
        }
    }
}