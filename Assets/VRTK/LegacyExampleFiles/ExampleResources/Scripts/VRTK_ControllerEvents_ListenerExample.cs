namespace VRTK.Examples
{
    using UnityEngine;

    public class VRTK_ControllerEvents_ListenerExample : MonoBehaviour
    {
        public enum EventQuickSelect
        {
            Custom,
            None,
            All,
            ButtonOnly,
            AxisOnly,
            SenseAxisOnly
        }

        [Header("Quick Select")]

        public EventQuickSelect quickSelect = EventQuickSelect.All;

        [Header("Button Events Debug")]

        public bool triggerButtonEvents = true;
        public bool gripButtonEvents = true;
        public bool touchpadButtonEvents = true;
        public bool buttonOneButtonEvents = true;
        public bool buttonTwoButtonEvents = true;
        public bool startMenuButtonEvents = true;

        [Header("Axis Events Debug")]

        public bool triggerAxisEvents = true;
        public bool gripAxisEvents = true;
        public bool touchpadAxisEvents = true;
        public bool touchpadTwoAxisEvents = true;

        [Header("Sense Axis Events Debug")]

        public bool triggerSenseAxisEvents = true;
        public bool touchpadSenseAxisEvents = true;
        public bool middleFingerSenseAxisEvents = true;
        public bool ringFingerSenseAxisEvents = true;
        public bool pinkyFingerSenseAxisEvents = true;

        private VRTK_ControllerEvents controllerEvents;

        private void OnEnable()
        {
            controllerEvents = GetComponent<VRTK_ControllerEvents>();
            if (controllerEvents == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
                return;
            }

            //Setup controller event listeners
            controllerEvents.TriggerPressed += DoTriggerPressed;
            controllerEvents.TriggerReleased += DoTriggerReleased;
            controllerEvents.TriggerTouchStart += DoTriggerTouchStart;
            controllerEvents.TriggerTouchEnd += DoTriggerTouchEnd;
            controllerEvents.TriggerHairlineStart += DoTriggerHairlineStart;
            controllerEvents.TriggerHairlineEnd += DoTriggerHairlineEnd;
            controllerEvents.TriggerClicked += DoTriggerClicked;
            controllerEvents.TriggerUnclicked += DoTriggerUnclicked;
            controllerEvents.TriggerAxisChanged += DoTriggerAxisChanged;
            controllerEvents.TriggerSenseAxisChanged += DoTriggerSenseAxisChanged;

            controllerEvents.GripPressed += DoGripPressed;
            controllerEvents.GripReleased += DoGripReleased;
            controllerEvents.GripTouchStart += DoGripTouchStart;
            controllerEvents.GripTouchEnd += DoGripTouchEnd;
            controllerEvents.GripHairlineStart += DoGripHairlineStart;
            controllerEvents.GripHairlineEnd += DoGripHairlineEnd;
            controllerEvents.GripClicked += DoGripClicked;
            controllerEvents.GripUnclicked += DoGripUnclicked;
            controllerEvents.GripAxisChanged += DoGripAxisChanged;

            controllerEvents.TouchpadPressed += DoTouchpadPressed;
            controllerEvents.TouchpadReleased += DoTouchpadReleased;
            controllerEvents.TouchpadTouchStart += DoTouchpadTouchStart;
            controllerEvents.TouchpadTouchEnd += DoTouchpadTouchEnd;
            controllerEvents.TouchpadAxisChanged += DoTouchpadAxisChanged;
            controllerEvents.TouchpadTwoAxisChanged += DoTouchpadTwoAxisChanged;
            controllerEvents.TouchpadSenseAxisChanged += DoTouchpadSenseAxisChanged;

            controllerEvents.ButtonOnePressed += DoButtonOnePressed;
            controllerEvents.ButtonOneReleased += DoButtonOneReleased;
            controllerEvents.ButtonOneTouchStart += DoButtonOneTouchStart;
            controllerEvents.ButtonOneTouchEnd += DoButtonOneTouchEnd;

            controllerEvents.ButtonTwoPressed += DoButtonTwoPressed;
            controllerEvents.ButtonTwoReleased += DoButtonTwoReleased;
            controllerEvents.ButtonTwoTouchStart += DoButtonTwoTouchStart;
            controllerEvents.ButtonTwoTouchEnd += DoButtonTwoTouchEnd;

            controllerEvents.StartMenuPressed += DoStartMenuPressed;
            controllerEvents.StartMenuReleased += DoStartMenuReleased;

            controllerEvents.ControllerEnabled += DoControllerEnabled;
            controllerEvents.ControllerDisabled += DoControllerDisabled;
            controllerEvents.ControllerIndexChanged += DoControllerIndexChanged;

            controllerEvents.MiddleFingerSenseAxisChanged += DoMiddleFingerSenseAxisChanged;
            controllerEvents.RingFingerSenseAxisChanged += DoRingFingerSenseAxisChanged;
            controllerEvents.PinkyFingerSenseAxisChanged += DoPinkyFingerSenseAxisChanged;
        }

        private void OnDisable()
        {
            if (controllerEvents != null)
            {
                controllerEvents.TriggerPressed -= DoTriggerPressed;
                controllerEvents.TriggerReleased -= DoTriggerReleased;
                controllerEvents.TriggerTouchStart -= DoTriggerTouchStart;
                controllerEvents.TriggerTouchEnd -= DoTriggerTouchEnd;
                controllerEvents.TriggerHairlineStart -= DoTriggerHairlineStart;
                controllerEvents.TriggerHairlineEnd -= DoTriggerHairlineEnd;
                controllerEvents.TriggerClicked -= DoTriggerClicked;
                controllerEvents.TriggerUnclicked -= DoTriggerUnclicked;
                controllerEvents.TriggerAxisChanged -= DoTriggerAxisChanged;
                controllerEvents.TriggerSenseAxisChanged -= DoTriggerSenseAxisChanged;

                controllerEvents.GripPressed -= DoGripPressed;
                controllerEvents.GripReleased -= DoGripReleased;
                controllerEvents.GripTouchStart -= DoGripTouchStart;
                controllerEvents.GripTouchEnd -= DoGripTouchEnd;
                controllerEvents.GripHairlineStart -= DoGripHairlineStart;
                controllerEvents.GripHairlineEnd -= DoGripHairlineEnd;
                controllerEvents.GripClicked -= DoGripClicked;
                controllerEvents.GripUnclicked -= DoGripUnclicked;
                controllerEvents.GripAxisChanged -= DoGripAxisChanged;

                controllerEvents.TouchpadPressed -= DoTouchpadPressed;
                controllerEvents.TouchpadReleased -= DoTouchpadReleased;
                controllerEvents.TouchpadTouchStart -= DoTouchpadTouchStart;
                controllerEvents.TouchpadTouchEnd -= DoTouchpadTouchEnd;
                controllerEvents.TouchpadAxisChanged -= DoTouchpadAxisChanged;
                controllerEvents.TouchpadTwoAxisChanged -= DoTouchpadTwoAxisChanged;
                controllerEvents.TouchpadSenseAxisChanged -= DoTouchpadSenseAxisChanged;

                controllerEvents.ButtonOnePressed -= DoButtonOnePressed;
                controllerEvents.ButtonOneReleased -= DoButtonOneReleased;
                controllerEvents.ButtonOneTouchStart -= DoButtonOneTouchStart;
                controllerEvents.ButtonOneTouchEnd -= DoButtonOneTouchEnd;

                controllerEvents.ButtonTwoPressed -= DoButtonTwoPressed;
                controllerEvents.ButtonTwoReleased -= DoButtonTwoReleased;
                controllerEvents.ButtonTwoTouchStart -= DoButtonTwoTouchStart;
                controllerEvents.ButtonTwoTouchEnd -= DoButtonTwoTouchEnd;

                controllerEvents.StartMenuPressed -= DoStartMenuPressed;
                controllerEvents.StartMenuReleased -= DoStartMenuReleased;

                controllerEvents.ControllerEnabled -= DoControllerEnabled;
                controllerEvents.ControllerDisabled -= DoControllerDisabled;
                controllerEvents.ControllerIndexChanged -= DoControllerIndexChanged;

                controllerEvents.MiddleFingerSenseAxisChanged -= DoMiddleFingerSenseAxisChanged;
                controllerEvents.RingFingerSenseAxisChanged -= DoRingFingerSenseAxisChanged;
                controllerEvents.PinkyFingerSenseAxisChanged -= DoPinkyFingerSenseAxisChanged;
            }
        }

        private void LateUpdate()
        {
            switch (quickSelect)
            {
                case EventQuickSelect.None:
                    triggerButtonEvents = false;
                    gripButtonEvents = false;
                    touchpadButtonEvents = false;
                    buttonOneButtonEvents = false;
                    buttonTwoButtonEvents = false;
                    startMenuButtonEvents = false;

                    triggerAxisEvents = false;
                    gripAxisEvents = false;
                    touchpadAxisEvents = false;
                    touchpadTwoAxisEvents = false;

                    triggerSenseAxisEvents = false;
                    touchpadSenseAxisEvents = false;
                    middleFingerSenseAxisEvents = false;
                    ringFingerSenseAxisEvents = false;
                    pinkyFingerSenseAxisEvents = false;
                    break;
                case EventQuickSelect.All:
                    triggerButtonEvents = true;
                    gripButtonEvents = true;
                    touchpadButtonEvents = true;
                    buttonOneButtonEvents = true;
                    buttonTwoButtonEvents = true;
                    startMenuButtonEvents = true;

                    triggerAxisEvents = true;
                    gripAxisEvents = true;
                    touchpadAxisEvents = true;
                    touchpadTwoAxisEvents = true;

                    triggerSenseAxisEvents = true;
                    touchpadSenseAxisEvents = true;
                    middleFingerSenseAxisEvents = true;
                    ringFingerSenseAxisEvents = true;
                    pinkyFingerSenseAxisEvents = true;
                    break;
                case EventQuickSelect.ButtonOnly:
                    triggerButtonEvents = true;
                    gripButtonEvents = true;
                    touchpadButtonEvents = true;
                    buttonOneButtonEvents = true;
                    buttonTwoButtonEvents = true;
                    startMenuButtonEvents = true;

                    triggerAxisEvents = false;
                    gripAxisEvents = false;
                    touchpadAxisEvents = false;
                    touchpadTwoAxisEvents = false;

                    triggerSenseAxisEvents = false;
                    touchpadSenseAxisEvents = false;
                    middleFingerSenseAxisEvents = false;
                    ringFingerSenseAxisEvents = false;
                    pinkyFingerSenseAxisEvents = false;
                    break;
                case EventQuickSelect.AxisOnly:
                    triggerButtonEvents = false;
                    gripButtonEvents = false;
                    touchpadButtonEvents = false;
                    buttonOneButtonEvents = false;
                    buttonTwoButtonEvents = false;
                    startMenuButtonEvents = false;

                    triggerAxisEvents = true;
                    gripAxisEvents = true;
                    touchpadAxisEvents = true;
                    touchpadTwoAxisEvents = true;

                    triggerSenseAxisEvents = false;
                    touchpadSenseAxisEvents = false;
                    middleFingerSenseAxisEvents = false;
                    ringFingerSenseAxisEvents = false;
                    pinkyFingerSenseAxisEvents = false;
                    break;
                case EventQuickSelect.SenseAxisOnly:
                    triggerButtonEvents = false;
                    gripButtonEvents = false;
                    touchpadButtonEvents = false;
                    buttonOneButtonEvents = false;
                    buttonTwoButtonEvents = false;
                    startMenuButtonEvents = false;

                    triggerAxisEvents = false;
                    gripAxisEvents = false;
                    touchpadAxisEvents = false;
                    touchpadTwoAxisEvents = false;

                    triggerSenseAxisEvents = true;
                    touchpadSenseAxisEvents = true;
                    middleFingerSenseAxisEvents = true;
                    ringFingerSenseAxisEvents = true;
                    pinkyFingerSenseAxisEvents = true;
                    break;
            }
        }

        private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
        {
            string debugString = "Controller on index '" + index + "' " + button + " has been " + action
                                 + " with a pressure of " + e.buttonPressure + " / Primary Touchpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)" + " / Secondary Touchpad axis at: " + e.touchpadTwoAxis + " (" + e.touchpadTwoAngle + " degrees)";
            VRTK_Logger.Info(debugString);
        }

        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "pressed", e);
            }
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "released", e);
            }
        }

        private void DoTriggerTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "touched", e);
            }
        }

        private void DoTriggerTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "untouched", e);
            }
        }

        private void DoTriggerHairlineStart(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "hairline start", e);
            }
        }

        private void DoTriggerHairlineEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "hairline end", e);
            }
        }

        private void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "clicked", e);
            }
        }

        private void DoTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "unclicked", e);
            }
        }

        private void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "axis changed", e);
            }
        }

        private void DoTriggerSenseAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (triggerSenseAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TRIGGER", "sense axis changed", e);
            }
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "pressed", e);
            }
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "released", e);
            }
        }

        private void DoGripTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "touched", e);
            }
        }

        private void DoGripTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "untouched", e);
            }
        }

        private void DoGripHairlineStart(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "hairline start", e);
            }
        }

        private void DoGripHairlineEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "hairline end", e);
            }
        }

        private void DoGripClicked(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "clicked", e);
            }
        }

        private void DoGripUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            if (gripButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "unclicked", e);
            }
        }

        private void DoGripAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (gripAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "GRIP", "axis changed", e);
            }
        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "pressed down", e);
            }
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "released", e);
            }
        }

        private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "touched", e);
            }
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "untouched", e);
            }
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "axis changed", e);
            }
        }

        private void DoTouchpadTwoAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadTwoAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPADTWO", "axis changed", e);
            }
        }

        private void DoTouchpadSenseAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (touchpadSenseAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "TOUCHPAD", "sense axis changed", e);
            }
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonOneButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON ONE", "pressed down", e);
            }
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonOneButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON ONE", "released", e);
            }
        }

        private void DoButtonOneTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonOneButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON ONE", "touched", e);
            }
        }

        private void DoButtonOneTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonOneButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON ONE", "untouched", e);
            }
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonTwoButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON TWO", "pressed down", e);
            }
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonTwoButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON TWO", "released", e);
            }
        }

        private void DoButtonTwoTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonTwoButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON TWO", "touched", e);
            }
        }

        private void DoButtonTwoTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            if (buttonTwoButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "BUTTON TWO", "untouched", e);
            }
        }

        private void DoStartMenuPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (startMenuButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "START MENU", "pressed down", e);
            }
        }

        private void DoStartMenuReleased(object sender, ControllerInteractionEventArgs e)
        {
            if (startMenuButtonEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "START MENU", "released", e);
            }
        }

        private void DoControllerEnabled(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "CONTROLLER STATE", "ENABLED", e);
        }

        private void DoControllerDisabled(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "CONTROLLER STATE", "DISABLED", e);
        }

        private void DoControllerIndexChanged(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "CONTROLLER STATE", "INDEX CHANGED", e);
        }

        private void DoMiddleFingerSenseAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (middleFingerSenseAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "MIDDLE FINGER", "sense axis changed", e);
            }
        }

        private void DoRingFingerSenseAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (ringFingerSenseAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "RING FINGER", "sense axis changed", e);
            }
        }

        private void DoPinkyFingerSenseAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (pinkyFingerSenseAxisEvents)
            {
                DebugLogger(VRTK_ControllerReference.GetRealIndex(e.controllerReference), "PINKY FINGER", "sense axis changed", e);
            }
        }
    }
}