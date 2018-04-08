namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTK_ControllerAppearance_Example : MonoBehaviour
    {
        public bool highlightBodyOnlyOnCollision = false;
        public bool pulseTriggerHighlightColor = false;

        private VRTK_ControllerTooltips tooltips;
        private VRTK_ControllerHighlighter highligher;
        private VRTK_ControllerEvents events;
        private Color highlightColor = Color.yellow;
        private Color pulseColor = Color.black;
        private Color currentPulseColor;
        private float highlightTimer = 0.5f;
        private float pulseTimer = 0.75f;
        private float dimOpacity = 0.8f;
        private float defaultOpacity = 1f;
        private bool highlighted;

        private void OnEnable()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerAppearance_Example", "VRTK_ControllerEvents", "the same"));
                return;
            }

            events = GetComponent<VRTK_ControllerEvents>();
            highligher = GetComponent<VRTK_ControllerHighlighter>();
            tooltips = GetComponentInChildren<VRTK_ControllerTooltips>();
            currentPulseColor = pulseColor;
            highlighted = false;

            //Setup controller event listeners
            events.TriggerPressed += DoTriggerPressed;
            events.TriggerReleased += DoTriggerReleased;
            events.ButtonOnePressed += DoButtonOnePressed;
            events.ButtonOneReleased += DoButtonOneReleased;
            events.ButtonTwoPressed += DoButtonTwoPressed;
            events.ButtonTwoReleased += DoButtonTwoReleased;
            events.StartMenuPressed += DoStartMenuPressed;
            events.StartMenuReleased += DoStartMenuReleased;
            events.GripPressed += DoGripPressed;
            events.GripReleased += DoGripReleased;
            events.TouchpadPressed += DoTouchpadPressed;
            events.TouchpadReleased += DoTouchpadReleased;

            tooltips.ToggleTips(false);
        }

        private void OnDisable()
        {
            events.TriggerPressed -= DoTriggerPressed;
            events.TriggerReleased -= DoTriggerReleased;
            events.ButtonOnePressed -= DoButtonOnePressed;
            events.ButtonOneReleased -= DoButtonOneReleased;
            events.ButtonTwoPressed -= DoButtonTwoPressed;
            events.ButtonTwoReleased -= DoButtonTwoReleased;
            events.StartMenuPressed -= DoStartMenuPressed;
            events.StartMenuReleased -= DoStartMenuReleased;
            events.GripPressed -= DoGripPressed;
            events.GripReleased -= DoGripReleased;
            events.TouchpadPressed -= DoTouchpadPressed;
            events.TouchpadReleased -= DoTouchpadReleased;
        }

        private void PulseTrigger()
        {
            highligher.HighlightElement(SDK_BaseController.ControllerElements.Trigger, currentPulseColor, pulseTimer);
            currentPulseColor = (currentPulseColor == pulseColor ? highlightColor : pulseColor);
        }

        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.Trigger, highlightColor, (pulseTriggerHighlightColor ? pulseTimer : highlightTimer));
            if (pulseTriggerHighlightColor)
            {
                InvokeRepeating("PulseTrigger", pulseTimer, pulseTimer);
            }
            VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), dimOpacity);
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TriggerTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.Trigger);
            if (pulseTriggerHighlightColor)
            {
                CancelInvoke("PulseTrigger");
            }
            if (!events.AnyButtonPressed())
            {
                VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), defaultOpacity);
            }
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.ButtonOne, highlightColor, highlightTimer);
            VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), dimOpacity);
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonOneTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.ButtonOne);
            if (!events.AnyButtonPressed())
            {
                VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), defaultOpacity);
            }
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.ButtonTwo, highlightColor, highlightTimer);
            VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), dimOpacity);
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.ButtonTwoTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.ButtonTwo);
            if (!events.AnyButtonPressed())
            {
                VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), defaultOpacity);
            }
        }

        private void DoStartMenuPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.StartMenu, highlightColor, highlightTimer);
            VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), dimOpacity);
        }

        private void DoStartMenuReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.StartMenuTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.StartMenu);
            if (!events.AnyButtonPressed())
            {
                VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), defaultOpacity);
            }
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.GripLeft, highlightColor, highlightTimer);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.GripRight, highlightColor, highlightTimer);
            VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), dimOpacity);
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.GripTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.GripLeft);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.GripRight);
            if (!events.AnyButtonPressed())
            {
                VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), defaultOpacity);
            }
        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(true, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
            highligher.HighlightElement(SDK_BaseController.ControllerElements.Touchpad, highlightColor, highlightTimer);
            VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), dimOpacity);
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            tooltips.ToggleTips(false, VRTK_ControllerTooltips.TooltipButtons.TouchpadTooltip);
            highligher.UnhighlightElement(SDK_BaseController.ControllerElements.Touchpad);
            if (!events.AnyButtonPressed())
            {
                VRTK_ObjectAppearance.SetOpacity(VRTK_DeviceFinder.GetModelAliasController(events.gameObject), defaultOpacity);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            OnTriggerStay(collider);
        }

        private void OnTriggerStay(Collider collider)
        {
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject) && !highlighted)
            {
                if (highlightBodyOnlyOnCollision)
                {
                    highligher.HighlightElement(SDK_BaseController.ControllerElements.Body, highlightColor, highlightTimer);
                }
                else
                {
                    highligher.HighlightController(highlightColor, highlightTimer);
                }
                highlighted = true;
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!VRTK_PlayerObject.IsPlayerObject(collider.gameObject))
            {
                if (highlightBodyOnlyOnCollision)
                {
                    highligher.UnhighlightElement(SDK_BaseController.ControllerElements.Body);
                }
                else
                {
                    highligher.UnhighlightController();
                }
                highlighted = false;
            }
        }
    }
}