// Controller Tooltips|Prefabs|0030
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="element">The tooltip element being affected.</param>
    public struct ControllerTooltipsEventArgs
    {
        public VRTK_ControllerTooltips.TooltipButtons element;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllerTooltipsEventArgs"/></param>
    public delegate void ControllerTooltipsEventHandler(object sender, ControllerTooltipsEventArgs e);

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

        [Header("Button Text Settings")]

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

        [Header("Tooltip Colour Settings")]

        [Tooltip("The colour to use for the tooltip background container.")]
        public Color tipBackgroundColor = Color.black;
        [Tooltip("The colour to use for the text within the tooltip.")]
        public Color tipTextColor = Color.white;
        [Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
        public Color tipLineColor = Color.black;

        [Header("Button Transform Settings")]

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

        [Header("Custom Settings")]

        [Tooltip("The controller to read the controller events from. If this is blank then it will attempt to get a controller events script from the same or parent GameObject.")]
        public VRTK_ControllerEvents controllerEvents;
        [Tooltip("The headset controller aware script to use to see if the headset is looking at the controller. If this is blank then it will attempt to get a controller events script from the same or parent GameObject.")]
        public VRTK_HeadsetControllerAware headsetControllerAware;
        [Tooltip("If this is checked then the tooltips will be hidden when the headset is not looking at the controller.")]
        public bool hideWhenNotInView = true;
        [Tooltip("The total number of initialisation attempts to make when waiting for the button transforms to initialise.")]
        public int retryInitMaxTries = 10;
        [Tooltip("The amount of seconds to wait before re-attempting to initialise the controller tooltips if the button transforms have not been initialised yet.")]
        public float retryInitCounter = 0.1f;

        /// <summary>
        /// Emitted when the controller tooltip is turned on.
        /// </summary>
        public event ControllerTooltipsEventHandler ControllerTooltipOn;
        /// <summary>
        /// Emitted when the controller tooltip is turned off.
        /// </summary>
        public event ControllerTooltipsEventHandler ControllerTooltipOff;

        protected TooltipButtons[] availableButtons;
        protected VRTK_ObjectTooltip[] buttonTooltips;
        protected bool[] tooltipStates;

        protected int retryInitCurrentTries = 0;

        public virtual void OnControllerTooltipOn(ControllerTooltipsEventArgs e)
        {
            if (ControllerTooltipOn != null)
            {
                ControllerTooltipOn(this, e);
            }
        }

        public virtual void OnControllerTooltipOff(ControllerTooltipsEventArgs e)
        {
            if (ControllerTooltipOff != null)
            {
                ControllerTooltipOff(this, e);
            }
        }

        /// <summary>
        /// The Reset method reinitalises the tooltips on all of the controller elements.
        /// </summary>
        public virtual void ResetTooltip()
        {
            InitialiseTips();
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
            EmitEvent(state, element);
        }

        protected virtual void Awake()
        {
            VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
            InitButtonsArray();
        }

        protected virtual void OnEnable()
        {
            controllerEvents = (controllerEvents != null ? controllerEvents : GetComponentInParent<VRTK_ControllerEvents>());
            InitButtonsArray();
            InitListeners();
            ResetTooltip();
        }

        protected virtual void OnDisable()
        {
            if (controllerEvents != null)
            {
                controllerEvents.ControllerEnabled -= DoControllerEnabled;
                controllerEvents.ControllerVisible -= DoControllerVisible;
                controllerEvents.ControllerHidden -= DoControllerInvisible;
            }

            if (headsetControllerAware != null)
            {
                headsetControllerAware.ControllerGlanceEnter -= DoGlanceEnterController;
                headsetControllerAware.ControllerGlanceExit -= DoGlanceExitController;
            }
        }

        protected virtual void OnDestroy()
        {
            VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
        }

        protected virtual void EmitEvent(bool state, TooltipButtons element)
        {
            ControllerTooltipsEventArgs e;
            e.element = element;
            if (state)
            {
                OnControllerTooltipOn(e);
            }
            else
            {
                OnControllerTooltipOff(e);
            }
        }

        protected virtual void InitButtonsArray()
        {
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
                buttonTooltips[i] = transform.Find(availableButtons[i].ToString()).GetComponent<VRTK_ObjectTooltip>();
            }

            retryInitCurrentTries = retryInitMaxTries;
        }

        protected virtual void InitListeners()
        {
            if (controllerEvents != null)
            {
                controllerEvents.ControllerEnabled += DoControllerEnabled;
                controllerEvents.ControllerVisible += DoControllerVisible;
                controllerEvents.ControllerHidden += DoControllerInvisible;
            }

            headsetControllerAware = (headsetControllerAware != null ? headsetControllerAware : FindObjectOfType<VRTK_HeadsetControllerAware>());
            if (headsetControllerAware != null)
            {
                headsetControllerAware.ControllerGlanceEnter += DoGlanceEnterController;
                headsetControllerAware.ControllerGlanceExit += DoGlanceExitController;
                ToggleTips(false);
            }
        }

        protected virtual void DoControllerEnabled(object sender, ControllerInteractionEventArgs e)
        {
            if (controllerEvents != null)
            {
                GameObject actualController = VRTK_DeviceFinder.GetActualController(controllerEvents.gameObject);
                if (actualController != null && actualController.activeInHierarchy)
                {
                    ResetTooltip();
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
            if (controllerEvents != null && hideWhenNotInView)
            {
                VRTK_ControllerReference checkControllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvents.gameObject);
                if (checkControllerReference == e.controllerReference)
                {
                    ToggleTips(true);
                }
            }
        }

        protected virtual void DoGlanceExitController(object sender, HeadsetControllerAwareEventArgs e)
        {
            if (controllerEvents != null && hideWhenNotInView)
            {
                VRTK_ControllerReference checkControllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvents.gameObject);
                if (checkControllerReference == e.controllerReference)
                {
                    ToggleTips(false);
                }
            }
        }

        protected virtual void InitialiseTips()
        {
            bool initComplete = false;
            VRTK_ObjectTooltip[] tooltips = GetComponentsInChildren<VRTK_ObjectTooltip>(true);
            for (int i = 0; i < tooltips.Length; i++)
            {
                VRTK_ObjectTooltip tooltip = tooltips[i];
                string tipText = "";
                Transform tipTransform = null;

                switch (tooltip.name.Replace("Tooltip", "").ToLower())
                {
                    case "trigger":
                        tipText = triggerText;
                        tipTransform = GetTransform(trigger, SDK_BaseController.ControllerElements.Trigger);
                        break;
                    case "grip":
                        tipText = gripText;
                        tipTransform = GetTransform(grip, SDK_BaseController.ControllerElements.GripLeft);
                        break;
                    case "touchpad":
                        tipText = touchpadText;
                        tipTransform = GetTransform(touchpad, SDK_BaseController.ControllerElements.Touchpad);
                        break;
                    case "buttonone":
                        tipText = buttonOneText;
                        tipTransform = GetTransform(buttonOne, SDK_BaseController.ControllerElements.ButtonOne);
                        break;
                    case "buttontwo":
                        tipText = buttonTwoText;
                        tipTransform = GetTransform(buttonTwo, SDK_BaseController.ControllerElements.ButtonTwo);
                        break;
                    case "startmenu":
                        tipText = startMenuText;
                        tipTransform = GetTransform(startMenu, SDK_BaseController.ControllerElements.StartMenu);
                        break;
                }

                initComplete = (tipTransform != null);

                tooltip.displayText = tipText;
                tooltip.drawLineTo = tipTransform;

                tooltip.containerColor = tipBackgroundColor;
                tooltip.fontColor = tipTextColor;
                tooltip.lineColor = tipLineColor;

                tooltip.ResetTooltip();

                if (tipTransform == null || tipText.Trim().Length == 0)
                {
                    tooltip.gameObject.SetActive(false);
                }
            }

            if (!initComplete && retryInitCurrentTries > 0)
            {
                retryInitCurrentTries--;
                Invoke("ResetTooltip", retryInitCounter);
                return;
            }

            if (headsetControllerAware == null || !hideWhenNotInView)
            {
                ToggleTips(true);
            }
        }

        protected virtual Transform GetTransform(Transform setTransform, SDK_BaseController.ControllerElements findElement)
        {
            Transform returnTransform = null;
            if (setTransform != null)
            {
                returnTransform = setTransform;
            }
            else if (controllerEvents != null)
            {
                GameObject modelController = VRTK_DeviceFinder.GetModelAliasController(controllerEvents.gameObject);

                if (modelController != null && modelController.activeInHierarchy)
                {
                    SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerEvents.gameObject);
                    string elementPath = VRTK_SDK_Bridge.GetControllerElementPath(findElement, controllerHand, true);
                    returnTransform = (elementPath != null ? modelController.transform.Find(elementPath) : null);
                }
            }

            return returnTransform;
        }
    }
}