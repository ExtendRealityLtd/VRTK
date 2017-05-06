// Controller Highlighter|Interactions|30021
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;

    /// <summary>
    /// The Controller Highlighter script provides methods to deal with highlighting controller elements.
    /// </summary>
    /// <remarks>
    /// The highlighting of the controller is defaulted to use the `VRTK_MaterialColorSwapHighlighter` if no other highlighter is applied to the Object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the ability to change the opacity of a controller model and to highlight specific elements of a controller such as the buttons or even the entire controller model.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/VRTK_ControllerHighlighter")]
    public class VRTK_ControllerHighlighter : MonoBehaviour
    {
        [Header("General Settings")]

        [Tooltip("The amount of time to take to transition to the set highlight colour.")]
        public float transitionDuration = 0f;

        [Header("Controller Highlight")]

        [Tooltip("The colour to set the entire controller highlight colour to.")]
        public Color highlightController = Color.clear;

        [Header("Element Highlights")]

        [Tooltip("The colour to set the body highlight colour to.")]
        public Color highlightBody = Color.clear;
        [Tooltip("The colour to set the trigger highlight colour to.")]
        public Color highlightTrigger = Color.clear;
        [Tooltip("The colour to set the grip highlight colour to.")]
        public Color highlightGrip = Color.clear;
        [Tooltip("The colour to set the touchpad highlight colour to.")]
        public Color highlightTouchpad = Color.clear;
        [Tooltip("The colour to set the button one highlight colour to.")]
        public Color highlightButtonOne = Color.clear;
        [Tooltip("The colour to set the button two highlight colour to.")]
        public Color highlightButtonTwo = Color.clear;
        [Tooltip("The colour to set the system menu highlight colour to.")]
        public Color highlightSystemMenu = Color.clear;
        [Tooltip("The colour to set the start menu highlight colour to.")]
        public Color highlightStartMenu = Color.clear;

        [Header("Custom Settings")]

        [Tooltip("A collection of strings that determine the path to the controller model sub elements for identifying the model parts at runtime. If the paths are left empty they will default to the model element paths of the selected SDK Bridge.")]
        public VRTK_ControllerModelElementPaths modelElementPaths = new VRTK_ControllerModelElementPaths();
        [Tooltip("A collection of highlighter overrides for each controller model sub element. If no highlighter override is given then highlighter on the Controller game object is used.")]
        public VRTK_ControllerElementHighlighters elementHighlighterOverrides = new VRTK_ControllerElementHighlighters();
        [Tooltip("An optional GameObject to specify which controller to apply the script methods to. If this is left blank then this script is required to be placed on a Controller Alias GameObject.")]
        public GameObject controllerAlias;
        [Tooltip("An optional GameObject to specifiy where the controller models are. If this is left blank then the Model Alias object will be used.")]
        public GameObject modelContainer;

        protected bool controllerHighlighted = false;
        protected Dictionary<string, Transform> cachedElements;
        protected Dictionary<string, object> highlighterOptions;
        protected Coroutine initHighlightersRoutine;

        protected Color lastHighlightController;
        protected Color lastHighlightBody;
        protected Color lastHighlightTrigger;
        protected Color lastHighlightGrip;
        protected Color lastHighlightTouchpad;
        protected Color lastHighlightButtonOne;
        protected Color lastHighlightButtonTwo;
        protected Color lastHighlightSystemMenu;
        protected Color lastHighlightStartMenu;

        protected SDK_BaseController.ControllerElements[] bodyElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.Body };
        protected SDK_BaseController.ControllerElements[] triggerElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.Trigger };
        protected SDK_BaseController.ControllerElements[] gripElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.GripLeft, SDK_BaseController.ControllerElements.GripRight };
        protected SDK_BaseController.ControllerElements[] touchpadElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.Touchpad };
        protected SDK_BaseController.ControllerElements[] buttonOneElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.ButtonOne };
        protected SDK_BaseController.ControllerElements[] buttonTwoElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.ButtonTwo };
        protected SDK_BaseController.ControllerElements[] systemMenuElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.SystemMenu };
        protected SDK_BaseController.ControllerElements[] startMenuElements = new SDK_BaseController.ControllerElements[] { SDK_BaseController.ControllerElements.StartMenu };

        /// <summary>
        /// The ConfigureControllerPaths method is used to set up the model element paths.
        /// </summary>
        public virtual void ConfigureControllerPaths()
        {
            cachedElements = new Dictionary<string, Transform>();
            modelElementPaths.bodyModelPath = GetElementPath(modelElementPaths.bodyModelPath, SDK_BaseController.ControllerElements.Body);
            modelElementPaths.triggerModelPath = GetElementPath(modelElementPaths.triggerModelPath, SDK_BaseController.ControllerElements.Trigger);
            modelElementPaths.leftGripModelPath = GetElementPath(modelElementPaths.leftGripModelPath, SDK_BaseController.ControllerElements.GripLeft);
            modelElementPaths.rightGripModelPath = GetElementPath(modelElementPaths.rightGripModelPath, SDK_BaseController.ControllerElements.GripRight);
            modelElementPaths.touchpadModelPath = GetElementPath(modelElementPaths.touchpadModelPath, SDK_BaseController.ControllerElements.Touchpad);
            modelElementPaths.buttonOneModelPath = GetElementPath(modelElementPaths.buttonOneModelPath, SDK_BaseController.ControllerElements.ButtonOne);
            modelElementPaths.buttonTwoModelPath = GetElementPath(modelElementPaths.buttonTwoModelPath, SDK_BaseController.ControllerElements.ButtonTwo);
            modelElementPaths.systemMenuModelPath = GetElementPath(modelElementPaths.systemMenuModelPath, SDK_BaseController.ControllerElements.SystemMenu);
            modelElementPaths.startMenuModelPath = GetElementPath(modelElementPaths.systemMenuModelPath, SDK_BaseController.ControllerElements.StartMenu);
        }

        /// <summary>
        /// The PopulateHighlighters method sets up the highlighters on the controller model.
        /// </summary>
        public virtual void PopulateHighlighters()
        {
            highlighterOptions = new Dictionary<string, object>();
            highlighterOptions.Add("resetMainTexture", true);
            VRTK_BaseHighlighter objectHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(controllerAlias);

            if (objectHighlighter == null)
            {
                objectHighlighter = controllerAlias.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }

            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerAlias);

            objectHighlighter.Initialise(null, highlighterOptions);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.ButtonOne, controllerHand)), objectHighlighter, elementHighlighterOverrides.buttonOne);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.ButtonTwo, controllerHand)), objectHighlighter, elementHighlighterOverrides.buttonTwo);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Body, controllerHand)), objectHighlighter, elementHighlighterOverrides.body);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.GripLeft, controllerHand)), objectHighlighter, elementHighlighterOverrides.gripLeft);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.GripRight, controllerHand)), objectHighlighter, elementHighlighterOverrides.gripRight);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.StartMenu, controllerHand)), objectHighlighter, elementHighlighterOverrides.startMenu);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.SystemMenu, controllerHand)), objectHighlighter, elementHighlighterOverrides.systemMenu);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Touchpad, controllerHand)), objectHighlighter, elementHighlighterOverrides.touchpad);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Trigger, controllerHand)), objectHighlighter, elementHighlighterOverrides.trigger);
        }

        /// <summary>
        /// The HighlightController method attempts to highlight all sub models of the controller.
        /// </summary>
        /// <param name="color">The colour to highlight the controller to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        public virtual void HighlightController(Color color, float fadeDuration = 0f)
        {
            HighlightElement(SDK_BaseController.ControllerElements.ButtonOne, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.ButtonTwo, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.Body, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.GripLeft, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.GripRight, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.StartMenu, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.SystemMenu, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.Touchpad, color, fadeDuration);
            HighlightElement(SDK_BaseController.ControllerElements.Trigger, color, fadeDuration);

            controllerHighlighted = true;
            highlightController = color;
            lastHighlightController = color;
        }

        /// <summary>
        /// The UnhighlightController method attempts to remove the highlight from all sub models of the controller.
        /// </summary>
        public virtual void UnhighlightController()
        {
            controllerHighlighted = false;
            highlightController = Color.clear;
            lastHighlightController = Color.clear;

            UnhighlightElement(SDK_BaseController.ControllerElements.ButtonOne);
            UnhighlightElement(SDK_BaseController.ControllerElements.ButtonTwo);
            UnhighlightElement(SDK_BaseController.ControllerElements.Body);
            UnhighlightElement(SDK_BaseController.ControllerElements.GripLeft);
            UnhighlightElement(SDK_BaseController.ControllerElements.GripRight);
            UnhighlightElement(SDK_BaseController.ControllerElements.StartMenu);
            UnhighlightElement(SDK_BaseController.ControllerElements.SystemMenu);
            UnhighlightElement(SDK_BaseController.ControllerElements.Touchpad);
            UnhighlightElement(SDK_BaseController.ControllerElements.Trigger);
        }

        /// <summary>
        /// The HighlightElement method attempts to highlight a specific controller element.
        /// </summary>
        /// <param name="elementType">The element type on the controller.</param>
        /// <param name="color">The colour to highlight the controller element to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        public virtual void HighlightElement(SDK_BaseController.ControllerElements elementType, Color color, float fadeDuration = 0f)
        {
            Transform element = GetElementTransform(GetPathForControllerElement(elementType));
            if (element != null)
            {
                VRTK_SharedMethods.HighlightObject(element.gameObject, color, fadeDuration);
                SetColourParameter(elementType, color);
            }
        }

        /// <summary>
        /// The UnhighlightElement method attempts to remove the highlight from the specific controller element.
        /// </summary>
        /// <param name="elementType">The element type on the controller.</param>
        public virtual void UnhighlightElement(SDK_BaseController.ControllerElements elementType)
        {
            if (!controllerHighlighted)
            {
                Transform element = GetElementTransform(GetPathForControllerElement(elementType));
                if (element != null)
                {
                    VRTK_SharedMethods.UnhighlightObject(element.gameObject);
                    SetColourParameter(elementType, Color.clear);
                }
            }
            else if (highlightController != Color.clear && GetColourParameter(elementType) != highlightController)
            {
                HighlightElement(elementType, highlightController, 0f);
            }
        }

        protected virtual void OnEnable()
        {
            if (controllerAlias == null)
            {
                VRTK_ControllerTracker controllerTracker = GetComponentInParent<VRTK_ControllerTracker>();
                controllerAlias = (controllerTracker != null ? controllerTracker.gameObject : null);
            }

            if (controllerAlias == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "VRTK_ControllerHighlighter", "Controller Alias GameObject", "controllerAlias", "the same"));
                return;
            }

            ConfigureControllerPaths();
            modelContainer = (modelContainer != null ? modelContainer : VRTK_DeviceFinder.GetModelAliasController(controllerAlias));
            initHighlightersRoutine = StartCoroutine(WaitForModel());
        }

        protected virtual void OnDisable()
        {
            if (initHighlightersRoutine != null)
            {
                StopCoroutine(initHighlightersRoutine);
            }
        }

        protected virtual void Update()
        {
            ToggleControllerState();
            ToggleHighlightState(highlightBody, ref lastHighlightBody, bodyElements);
            ToggleHighlightState(highlightTrigger, ref lastHighlightTrigger, triggerElements);
            ToggleHighlightState(highlightGrip, ref lastHighlightGrip, gripElements);
            ToggleHighlightState(highlightTouchpad, ref lastHighlightTouchpad, touchpadElements);
            ToggleHighlightState(highlightButtonOne, ref lastHighlightButtonOne, buttonOneElements);
            ToggleHighlightState(highlightButtonTwo, ref lastHighlightButtonTwo, buttonTwoElements);
            ToggleHighlightState(highlightSystemMenu, ref lastHighlightSystemMenu, systemMenuElements);
            ToggleHighlightState(highlightStartMenu, ref lastHighlightStartMenu, startMenuElements);
        }

        protected virtual void ResetLastHighlights()
        {
            lastHighlightController = Color.clear;
            lastHighlightBody = Color.clear;
            lastHighlightTrigger = Color.clear;
            lastHighlightGrip = Color.clear;
            lastHighlightTouchpad = Color.clear;
            lastHighlightButtonOne = Color.clear;
            lastHighlightButtonTwo = Color.clear;
            lastHighlightSystemMenu = Color.clear;
            lastHighlightStartMenu = Color.clear;
        }

        protected virtual void SetColourParameter(SDK_BaseController.ControllerElements element, Color color)
        {
            color = (color == Color.clear && highlightController != Color.clear ? highlightController : color);

            switch (element)
            {
                case SDK_BaseController.ControllerElements.Body:
                    highlightBody = color;
                    lastHighlightBody = color;
                    break;
                case SDK_BaseController.ControllerElements.Trigger:
                    highlightTrigger = color;
                    lastHighlightTrigger = color;
                    break;
                case SDK_BaseController.ControllerElements.GripLeft:
                case SDK_BaseController.ControllerElements.GripRight:
                    highlightGrip = color;
                    lastHighlightGrip = color;
                    break;
                case SDK_BaseController.ControllerElements.Touchpad:
                    highlightTouchpad = color;
                    lastHighlightTouchpad = color;
                    break;
                case SDK_BaseController.ControllerElements.ButtonOne:
                    highlightButtonOne = color;
                    lastHighlightButtonOne = color;
                    break;
                case SDK_BaseController.ControllerElements.ButtonTwo:
                    highlightButtonTwo = color;
                    lastHighlightButtonTwo = color;
                    break;
                case SDK_BaseController.ControllerElements.SystemMenu:
                    highlightSystemMenu = color;
                    lastHighlightSystemMenu = color;
                    break;
                case SDK_BaseController.ControllerElements.StartMenu:
                    highlightStartMenu = color;
                    lastHighlightStartMenu = color;
                    break;
            }
        }

        protected virtual Color GetColourParameter(SDK_BaseController.ControllerElements element)
        {
            switch (element)
            {
                case SDK_BaseController.ControllerElements.Body:
                    return highlightBody;
                case SDK_BaseController.ControllerElements.Trigger:
                    return highlightTrigger;
                case SDK_BaseController.ControllerElements.GripLeft:
                case SDK_BaseController.ControllerElements.GripRight:
                    return highlightGrip;
                case SDK_BaseController.ControllerElements.Touchpad:
                    return highlightTouchpad;
                case SDK_BaseController.ControllerElements.ButtonOne:
                    return highlightButtonOne;
                case SDK_BaseController.ControllerElements.ButtonTwo:
                    return highlightButtonTwo;
                case SDK_BaseController.ControllerElements.SystemMenu:
                    return highlightSystemMenu;
                case SDK_BaseController.ControllerElements.StartMenu:
                    return highlightStartMenu;
            }

            return Color.clear;
        }

        protected virtual void ToggleControllerState()
        {
            if (highlightController != lastHighlightController)
            {
                if (highlightController == Color.clear)
                {
                    UnhighlightController();
                }
                else
                {
                    HighlightController(highlightController, transitionDuration);
                }
            }
        }

        protected virtual void ToggleHighlightState(Color currentColor, ref Color lastColorState, SDK_BaseController.ControllerElements[] elements)
        {
            if (currentColor != lastColorState)
            {
                if (currentColor == Color.clear)
                {
                    for (int i = 0; i < elements.Length; i++)
                    {
                        UnhighlightElement(elements[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < elements.Length; i++)
                    {
                        HighlightElement(elements[i], currentColor, transitionDuration);
                    }
                }
                lastColorState = currentColor;
            }
        }

        protected virtual IEnumerator WaitForModel()
        {
            while (GetElementTransform(modelElementPaths.bodyModelPath) == null)
            {
                yield return null;
            }
            PopulateHighlighters();
            ResetLastHighlights();
        }

        protected virtual void AddHighlighterToElement(Transform element, VRTK_BaseHighlighter parentHighlighter, VRTK_BaseHighlighter overrideHighlighter)
        {
            if (element != null)
            {
                VRTK_BaseHighlighter highlighter = (overrideHighlighter != null ? overrideHighlighter : parentHighlighter);
                VRTK_BaseHighlighter clonedHighlighter = (VRTK_BaseHighlighter)VRTK_SharedMethods.CloneComponent(highlighter, element.gameObject);
                clonedHighlighter.Initialise(null, highlighterOptions);
            }
        }

        protected virtual string GetElementPath(string currentPath, SDK_BaseController.ControllerElements elementType)
        {
            SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerAlias);
            string foundElementPath = VRTK_SDK_Bridge.GetControllerElementPath(elementType, controllerHand);
            return (currentPath.Trim() == "" && foundElementPath != null ? foundElementPath : currentPath.Trim());
        }

        protected virtual string GetPathForControllerElement(SDK_BaseController.ControllerElements controllerElement)
        {
            switch (controllerElement)
            {
                case SDK_BaseController.ControllerElements.Body:
                    return modelElementPaths.bodyModelPath;
                case SDK_BaseController.ControllerElements.Trigger:
                    return modelElementPaths.triggerModelPath;
                case SDK_BaseController.ControllerElements.GripLeft:
                    return modelElementPaths.leftGripModelPath;
                case SDK_BaseController.ControllerElements.GripRight:
                    return modelElementPaths.rightGripModelPath;
                case SDK_BaseController.ControllerElements.Touchpad:
                    return modelElementPaths.touchpadModelPath;
                case SDK_BaseController.ControllerElements.ButtonOne:
                    return modelElementPaths.buttonOneModelPath;
                case SDK_BaseController.ControllerElements.ButtonTwo:
                    return modelElementPaths.buttonTwoModelPath;
                case SDK_BaseController.ControllerElements.SystemMenu:
                    return modelElementPaths.systemMenuModelPath;
                case SDK_BaseController.ControllerElements.StartMenu:
                    return modelElementPaths.startMenuModelPath;
            }
            return "";
        }

        protected virtual Transform GetElementTransform(string path)
        {
            if (cachedElements == null || path == null)
            {
                return null;
            }

            if (!cachedElements.ContainsKey(path) || cachedElements[path] == null)
            {
                if (!modelContainer)
                {
                    VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "Controller Model", "Controller SDK"));
                    return null;
                }
                cachedElements[path] = modelContainer.transform.Find(path);
            }
            return cachedElements[path];
        }

        protected virtual void ToggleHighlightAlias(bool state, string transformPath, Color? highlight, float duration = 0f)
        {
            Transform element = GetElementTransform(transformPath);
            if (element)
            {
                if (state)
                {
                    VRTK_SharedMethods.HighlightObject(element.gameObject, (highlight != null ? highlight : Color.white), duration);
                }
                else
                {
                    VRTK_SharedMethods.UnhighlightObject(element.gameObject);
                }
            }
        }
    }
}