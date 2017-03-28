// Controller Tooltips|Prefabs|0030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// This adds a collection of Object Tooltips to the Controller that give information on what the main controller buttons may do. To add the prefab, it just needs to be added as a child of the relevant alias controller GameObject.
    /// </summary>
    /// <remarks>
    /// If the transforms for the buttons are not provided, then the script will attempt to find the attach transforms on the default controller model.
    ///
    /// If no text is provided for one of the elements then the tooltip for that element will be set to disabled.
    ///
    /// There are a number of parameters that can be set on the Prefab which are provided by the `VRTK_ControllerTooltips` script which is applied to the prefab.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/029_Controller_Tooltips` displays two cubes that have an object tooltip added to them along with tooltips that have been added to the controllers.
    /// </example>
    public class VRTK_ControllerTooltips : MonoBehaviour
    {
        public enum TooltipButtons
        {
            None,
            TriggerTooltip,
            GripTooltip,
            TouchpadTooltip,
            ButtonOneTooltip,
            ButtonTwoTooltip,
            StartMenuTooltip
        }

        [Tooltip("The text to display for the trigger button action.")]
        public string triggerText;
        [Tooltip("The text to display for the grip button action.")]
        public string gripText;
        [Tooltip("The text to display for the touchpad action.")]
        public string touchpadText;
        [Tooltip("The text to display for button one action.")]
        public string buttonOneText;
        [Tooltip("The text to display for button two action.")]
        public string buttonTwoText;
        [Tooltip("The text to display for the start menu action.")]
        public string startMenuText;
        [Tooltip("The colour to use for the tooltip background container.")]
        public Color tipBackgroundColor = Color.black;
        [Tooltip("The colour to use for the text within the tooltip.")]
        public Color tipTextColor = Color.white;
        [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
        public Color tipLineColor = Color.black;
        [Tooltip("The transform for the position of the trigger button on the controller.")]
        public Transform trigger;
        [Tooltip("The transform for the position of the grip button on the controller.")]
        public Transform grip;
        [Tooltip("The transform for the position of the touchpad button on the controller.")]
        public Transform touchpad;
        [Tooltip("The transform for the position of button one on the controller.")]
        public Transform buttonOne;
        [Tooltip("The transform for the position of button two on the controller.")]
        public Transform buttonTwo;
        [Tooltip("The transform for the position of the start menu on the controller.")]
        public Transform startMenu;

        protected bool triggerInitialised = false;
        protected bool gripInitialised = false;
        protected bool touchpadInitialised = false;
        protected bool buttonOneInitialised = false;
        protected bool buttonTwoInitialised = false;
        protected bool startMenuInitialised = false;
        protected TooltipButtons[] availableButtons;
        protected VRTK_ObjectTooltip[] buttonTooltips;
        protected bool[] tooltipStates;
        protected VRTK_ControllerEvents controllerEvents;
        protected VRTK_HeadsetControllerAware headsetControllerAware;

        /// <summary>
        /// The Reset method reinitalises the tooltips on all of the controller elements.
        /// </summary>
        public virtual void ResetTooltip()
        {
            triggerInitialised = false;
            gripInitialised = false;
            touchpadInitialised = false;
            buttonOneInitialised = false;
            buttonTwoInitialised = false;
            startMenuInitialised = false;
        }

        /// <summary>
        /// The UpdateText method allows the tooltip text on a specific controller element to be updated at runtime.
        /// </summary>
        /// <param name="element">The specific controller element to change the tooltip text on.</param>
        /// <param name="newText">A string containing the text to update the tooltip to display.</param>
        public virtual void UpdateText(TooltipButtons element, string newText)
        {
            switch (element)
            {
                case TooltipButtons.ButtonOneTooltip:
                    buttonOneText = newText;
                    break;
                case TooltipButtons.ButtonTwoTooltip:
                    buttonTwoText = newText;
                    break;
                case TooltipButtons.StartMenuTooltip:
                    startMenuText = newText;
                    break;
                case TooltipButtons.GripTooltip:
                    gripText = newText;
                    break;
                case TooltipButtons.TouchpadTooltip:
                    touchpadText = newText;
                    break;
                case TooltipButtons.TriggerTooltip:
                    triggerText = newText;
                    break;
            }
            ResetTooltip();
        }

        /// <summary>
        /// The ToggleTips method will display the controller tooltips if the state is `true` and will hide the controller tooltips if the state is `false`. An optional `element` can be passed to target a specific controller tooltip to toggle otherwise all tooltips are toggled.
        /// </summary>
        /// <param name="state">The state of whether to display or hide the controller tooltips, true will display and false will hide.</param>
        /// <param name="element">The specific element to hide the tooltip on, if it is `TooltipButtons.None` then it will hide all tooltips. Optional parameter defaults to `TooltipButtons.None`</param>
        public virtual void ToggleTips(bool state, TooltipButtons element = TooltipButtons.None)
        {
            if (element == TooltipButtons.None)
            {
                for (int i = 1; i < buttonTooltips.Length; i++)
                {
                    if (buttonTooltips[i].displayText.Length > 0)
                    {
                        buttonTooltips[i].gameObject.SetActive(state);
                    }
                }
            }
            else
            {
                if (buttonTooltips[(int)element].displayText.Length > 0)
                {
                    buttonTooltips[(int)element].gameObject.SetActive(state);
                }
            }
        }

        protected virtual void Awake()
        {
            controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();
            triggerInitialised = false;
            gripInitialised = false;
            touchpadInitialised = false;
            buttonOneInitialised = false;
            buttonTwoInitialised = false;
            startMenuInitialised = false;

            availableButtons = new TooltipButtons[]
            {
                TooltipButtons.None,
                TooltipButtons.TriggerTooltip,
                TooltipButtons.GripTooltip,
                TooltipButtons.TouchpadTooltip,
                TooltipButtons.ButtonOneTooltip,
                TooltipButtons.ButtonTwoTooltip,
                TooltipButtons.StartMenuTooltip
            };

            buttonTooltips = new VRTK_ObjectTooltip[availableButtons.Length];
            tooltipStates = new bool[availableButtons.Length];

            for (int i = 1; i < availableButtons.Length; i++)
            {
                buttonTooltips[i] = transform.FindChild(availableButtons[i].ToString()).GetComponent<VRTK_ObjectTooltip>();
            }

            InitialiseTips();
        }

        protected virtual void OnEnable()
        {
            if (controllerEvents != null)
            {
                controllerEvents.ControllerVisible += DoControllerVisible;
                controllerEvents.ControllerHidden += DoControllerInvisible;
            }

            headsetControllerAware = FindObjectOfType<VRTK_HeadsetControllerAware>();
            if (headsetControllerAware)
            {
                headsetControllerAware.ControllerGlanceEnter += DoGlanceEnterController;
                headsetControllerAware.ControllerGlanceExit += DoGlanceExitController;
                ToggleTips(false);
            }
        }

        protected virtual void OnDisable()
        {
            if (controllerEvents != null)
            {
                controllerEvents.ControllerVisible -= DoControllerVisible;
                controllerEvents.ControllerHidden -= DoControllerInvisible;
            }

            if (headsetControllerAware)
            {
                headsetControllerAware.ControllerGlanceEnter -= DoGlanceEnterController;
                headsetControllerAware.ControllerGlanceExit -= DoGlanceExitController;
            }
        }

        protected virtual void Update()
        {
            if (!TipsInitialised() && controllerEvents != null)
            {
                GameObject actualController = VRTK_DeviceFinder.GetActualController(controllerEvents.gameObject);
                if (actualController && actualController.activeInHierarchy)
                {
                    InitialiseTips();
                }
            }
        }

        protected virtual void DoControllerVisible(object sender, ControllerInteractionEventArgs e)
        {
            for (int i = 0; i < availableButtons.Length; i++)
            {
                ToggleTips(tooltipStates[i], availableButtons[i]);
            }
        }

        protected virtual void DoControllerInvisible(object sender, ControllerInteractionEventArgs e)
        {
            for (int i = 1; i < buttonTooltips.Length; i++)
            {
                tooltipStates[i] = buttonTooltips[i].gameObject.activeSelf;
            }
            ToggleTips(false);
        }


        protected virtual void DoGlanceEnterController(object sender, HeadsetControllerAwareEventArgs e)
        {
            if (controllerEvents != null)
            {
                uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controllerEvents.gameObject);
                if (controllerIndex == e.controllerIndex)
                {
                    ToggleTips(true);
                }
            }
        }

        protected virtual void DoGlanceExitController(object sender, HeadsetControllerAwareEventArgs e)
        {
            if (controllerEvents != null)
            {
                uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(controllerEvents.gameObject);
                if (controllerIndex == e.controllerIndex)
                {
                    ToggleTips(false);
                }
            }
        }

        protected virtual void InitialiseTips()
        {
            foreach (VRTK_ObjectTooltip tooltip in GetComponentsInChildren<VRTK_ObjectTooltip>(true))
            {
                string tipText = "";
                Transform tipTransform = null;

                switch (tooltip.name.Replace("Tooltip", "").ToLower())
                {
                    case "trigger":
                        tipText = triggerText;
                        tipTransform = GetTransform(trigger, SDK_BaseController.ControllerElements.Trigger);
                        if (tipTransform != null)
                        {
                            triggerInitialised = true;
                        }
                        break;
                    case "grip":
                        tipText = gripText;
                        tipTransform = GetTransform(grip, SDK_BaseController.ControllerElements.GripLeft);
                        if (tipTransform != null)
                        {
                            gripInitialised = true;
                        }
                        break;
                    case "touchpad":
                        tipText = touchpadText;
                        tipTransform = GetTransform(touchpad, SDK_BaseController.ControllerElements.Touchpad);
                        if (tipTransform != null)
                        {
                            touchpadInitialised = true;
                        }
                        break;
                    case "buttonone":
                        tipText = buttonOneText;
                        tipTransform = GetTransform(buttonOne, SDK_BaseController.ControllerElements.ButtonOne);
                        if (tipTransform != null)
                        {
                            buttonOneInitialised = true;
                        }
                        break;
                    case "buttontwo":
                        tipText = buttonTwoText;
                        tipTransform = GetTransform(buttonTwo, SDK_BaseController.ControllerElements.ButtonTwo);
                        if (tipTransform != null)
                        {
                            buttonTwoInitialised = true;
                        }
                        break;
                    case "startmenu":
                        tipText = startMenuText;
                        tipTransform = GetTransform(startMenu, SDK_BaseController.ControllerElements.StartMenu);
                        if (tipTransform != null)
                        {
                            startMenuInitialised = true;
                        }
                        break;
                }

                tooltip.displayText = tipText;
                tooltip.drawLineTo = tipTransform;

                tooltip.containerColor = tipBackgroundColor;
                tooltip.fontColor = tipTextColor;
                tooltip.lineColor = tipLineColor;

                tooltip.ResetTooltip();

                if (tipText.Trim().Length == 0)
                {
                    tooltip.gameObject.SetActive(false);
                }
            }
        }

        protected virtual bool TipsInitialised()
        {
            return (triggerInitialised && gripInitialised && touchpadInitialised && (buttonOneInitialised || buttonTwoInitialised || startMenuInitialised));
        }

        protected virtual Transform GetTransform(Transform setTransform, SDK_BaseController.ControllerElements findElement)
        {
            Transform returnTransform = null;
            if (setTransform)
            {
                returnTransform = setTransform;
            }
            else if (controllerEvents != null)
            {
                GameObject modelController = VRTK_DeviceFinder.GetModelAliasController(controllerEvents.gameObject);

                if (modelController && modelController.activeInHierarchy)
                {
                    SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerEvents.gameObject);
                    string elementPath = VRTK_SDK_Bridge.GetControllerElementPath(findElement, controllerHand, true);
                    returnTransform = modelController.transform.FindChild(elementPath);
                }
            }

            return returnTransform;
        }
    }
}