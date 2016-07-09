using UnityEngine;
using VRTK;

public class VRTK_ControllerAppearance_Example : MonoBehaviour
{
    private VRTK_ControllerTooltips tooltips;
    private VRTK_ControllerActions actions;
    private VRTK_ControllerEvents events;

    private void Start()
    {
        if (GetComponent<VRTK_ControllerEvents>() == null)
        {
            Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a SteamVR Controller that has the VRTK_ControllerEvents script attached to it");
            return;
        }

        events = GetComponent<VRTK_ControllerEvents>();
        actions = GetComponent<VRTK_ControllerActions>();
        tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();

        //Setup controller event listeners
        events.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
        events.TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

        events.ApplicationMenuPressed += new ControllerInteractionEventHandler(DoApplicationMenuPressed);
        events.ApplicationMenuReleased += new ControllerInteractionEventHandler(DoApplicationMenuReleased);

        events.GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
        events.GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

        events.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
        events.TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

        tooltips.ShowTips(false);
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        actions.ToggleHighlightTrigger(true, Color.yellow, 0.5f);
        actions.SetControllerOpacity(0.8f);
    }

    private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
        actions.ToggleHighlightTrigger(false);
        if (!events.AnyButtonPressed())
        {
            actions.SetControllerOpacity(1f);
        }
    }

    private void DoApplicationMenuPressed(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(true, VRTK_ControllerTooltips.TooltipButtons.AppMenuTooltip);
        actions.ToggleHighlightApplicationMenu(true, Color.yellow, 0.5f);
        actions.SetControllerOpacity(0.8f);
    }

    private void DoApplicationMenuReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(false, VRTK_ControllerTooltips.TooltipButtons.AppMenuTooltip);
        actions.ToggleHighlightApplicationMenu(false);
        if (!events.AnyButtonPressed())
        {
            actions.SetControllerOpacity(1f);
        }
    }

    private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
        actions.ToggleHighlightGrip(true, Color.yellow, 0.5f);
        actions.SetControllerOpacity(0.8f);
    }

    private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
        actions.ToggleHighlightGrip(false);
        if (!events.AnyButtonPressed())
        {
            actions.SetControllerOpacity(1f);
        }
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
        actions.ToggleHighlightTouchpad(true, Color.yellow, 0.5f);
        actions.SetControllerOpacity(0.8f);
    }

    private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        tooltips.ShowTips(false, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
        actions.ToggleHighlightTouchpad(false);
        if (!events.AnyButtonPressed())
        {
            actions.SetControllerOpacity(1f);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        actions.ToggleHighlightController(true, Color.yellow, 0.4f);
    }

    private void OnTriggerExit(Collider collider)
    {
        actions.ToggleHighlightController(false);
    }
}