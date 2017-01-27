// Controller Actions|Interactions|30020
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;

    [System.Serializable]
    public class VRTK_ControllerModelElementPaths
    {
        public string bodyModelPath = "";
        public string triggerModelPath = "";
        public string leftGripModelPath = "";
        public string rightGripModelPath = "";
        public string touchpadModelPath = "";
        public string buttonOneModelPath = "";
        public string buttonTwoModelPath = "";
        public string systemMenuModelPath = "";
        public string startMenuModelPath = "";
    }

    [System.Serializable]
    public struct VRTK_ControllerElementHighlighers
    {
        public VRTK_BaseHighlighter body;
        public VRTK_BaseHighlighter trigger;
        public VRTK_BaseHighlighter gripLeft;
        public VRTK_BaseHighlighter gripRight;
        public VRTK_BaseHighlighter touchpad;
        public VRTK_BaseHighlighter buttonOne;
        public VRTK_BaseHighlighter buttonTwo;
        public VRTK_BaseHighlighter systemMenu;
        public VRTK_BaseHighlighter startMenu;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the controller that was used.</param>
    public struct ControllerActionsEventArgs
    {
        public uint controllerIndex;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllerActionsEventArgs"/></param>
    public delegate void ControllerActionsEventHandler(object sender, ControllerActionsEventArgs e);

    /// <summary>
    /// The Controller Actions script provides helper methods to deal with common controller actions. It deals with actions that can be done to the controller.
    /// </summary>
    /// <remarks>
    /// The highlighting of the controller is defaulted to use the `VRTK_MaterialColorSwapHighlighter` if no other highlighter is applied to the Object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/016_Controller_HapticRumble` demonstrates the ability to hide a controller model and make the controller vibrate for a given length of time at a given intensity.
    ///
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the ability to change the opacity of a controller model and to highlight specific elements of a controller such as the buttons or even the entire controller model.
    /// </example>
    public class VRTK_ControllerActions : MonoBehaviour
    {
        [Tooltip("A collection of strings that determine the path to the controller model sub elements for identifying the model parts at runtime. If the paths are left empty they will default to the model element paths of the selected SDK Bridge.\n\n"
         + "* The available model sub elements are:\n\n"
         + " * `Body Model Path`: The overall shape of the controller.\n"
         + " * `Trigger Model Path`: The model that represents the trigger button.\n"
         + " * `Grip Left Model Path`: The model that represents the left grip button.\n"
         + " * `Grip Right Model Path`: The model that represents the right grip button.\n"
         + " * `Touchpad Model Path`: The model that represents the touchpad.\n"
         + " * `Button One Model Path`: The model that represents button one.\n"
         + " * `Button Two Model Path`: The model that represents button two.\n"
         + " * `System Menu Model Path`: The model that represents the system menu button."
         + " * `Start Menu Model Path`: The model that represents the start menu button.")]
        public VRTK_ControllerModelElementPaths modelElementPaths;

        [Tooltip("A collection of highlighter overrides for each controller model sub element. If no highlighter override is given then highlighter on the Controller game object is used.\n\n"
         + "* The available model sub elements are:\n\n"
         + " * `Body`: The highlighter to use on the overall shape of the controller.\n"
         + " * `Trigger`: The highlighter to use on the trigger button.\n"
         + " * `Grip Left`: The highlighter to use on the left grip button.\n"
         + " * `Grip Right`: The highlighter to use on the  right grip button.\n"
         + " * `Touchpad`: The highlighter to use on the touchpad.\n"
         + " * `Button One`: The highlighter to use on button one.\n"
         + " * `Button Two`: The highlighter to use on button two.\n"
         + " * `System Menu`: The highlighter to use on the system menu button."
         + " * `Start Menu`: The highlighter to use on the start menu button.")]
        public VRTK_ControllerElementHighlighers elementHighlighterOverrides;

        /// <summary>
        /// Emitted when the controller model is toggled to be visible.
        /// </summary>
        public event ControllerActionsEventHandler ControllerModelVisible;

        /// <summary>
        /// Emitted when the controller model is toggled to be invisible.
        /// </summary>
        public event ControllerActionsEventHandler ControllerModelInvisible;

        private GameObject modelContainer;
        private bool controllerVisible = true;
        private bool controllerHighlighted = false;
        private Dictionary<string, Transform> cachedElements;
        private Dictionary<string, object> highlighterOptions;
        private Coroutine hapticLoop;

        public virtual void OnControllerModelVisible(ControllerActionsEventArgs e)
        {
            if (ControllerModelVisible != null)
            {
                ControllerModelVisible(this, e);
            }
        }

        public virtual void OnControllerModelInvisible(ControllerActionsEventArgs e)
        {
            if (ControllerModelInvisible != null)
            {
                ControllerModelInvisible(this, e);
            }
        }

        /// <summary>
        /// The IsControllerVisible method returns true if the controller is currently visible by whether the renderers on the controller are enabled.
        /// </summary>
        /// <returns>Is true if the controller model has the renderers that are attached to it are enabled.</returns>
        public bool IsControllerVisible()
        {
            return controllerVisible;
        }

        /// <summary>
        /// The ToggleControllerModel method is used to turn on or off the controller model by enabling or disabling the renderers on the object. It will also work for any custom controllers. It should also not disable any objects being held by the controller if they are a child of the controller object.
        /// </summary>
        /// <param name="state">The visibility state to toggle the controller to, `true` will make the controller visible - `false` will hide the controller model.</param>
        /// <param name="grabbedChildObject">If an object is being held by the controller then this can be passed through to prevent hiding the grabbed game object as well.</param>
        public virtual void ToggleControllerModel(bool state, GameObject grabbedChildObject)
        {
            if (!enabled)
            {
                return;
            }

            ToggleModelRenderers(modelContainer, state, grabbedChildObject);

            controllerVisible = state;
            var controllerIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
            if (state)
            {
                OnControllerModelVisible(SetActionEvent(controllerIndex));
            }
            else
            {
                OnControllerModelInvisible(SetActionEvent(controllerIndex));
            }
        }

        /// <summary>
        /// The SetControllerOpacity method allows the opacity of the controller model to be changed to make the controller more transparent. A lower alpha value will make the object more transparent, such as `0.5f` will make the controller partially transparent where as `0f` will make the controller completely transparent.
        /// </summary>
        /// <param name="alpha">The alpha level to apply to opacity of the controller object. `0f` to `1f`.</param>
        public virtual void SetControllerOpacity(float alpha)
        {
            if (!enabled)
            {
                return;
            }

            alpha = Mathf.Clamp(alpha, 0f, 1f);
            SetModelOpacity(modelContainer, alpha);
        }

        /// <summary>
        /// The HighlightControllerElement method allows for an element of the controller to have its colour changed to simulate a highlighting effect of that element on the controller. It's useful for being able to draw a user's attention to a specific button on the controller.
        /// </summary>
        /// <param name="element">The element of the controller to apply the highlight to.</param>
        /// <param name="highlight">The colour of the highlight.</param>
        /// <param name="fadeDuration">The duration of fade from white to the highlight colour. Optional parameter defaults to `0f`.</param>
        public virtual void HighlightControllerElement(GameObject element, Color? highlight, float fadeDuration = 0f)
        {
            if (!enabled)
            {
                return;
            }

            var highlighter = element.GetComponent<VRTK_BaseHighlighter>();
            if (highlighter)
            {
                highlighter.Highlight(highlight ?? Color.white, fadeDuration);
            }
        }

        /// <summary>
        /// The UnhighlightControllerElement method is the inverse of the HighlightControllerElement method and resets the controller element to its original colour.
        /// </summary>
        /// <param name="element">The element of the controller to remove the highlight from.</param>
        public virtual void UnhighlightControllerElement(GameObject element)
        {
            if (!enabled)
            {
                return;
            }

            var highlighter = element.GetComponent<VRTK_BaseHighlighter>();
            if (highlighter)
            {
                highlighter.Unhighlight();
            }
        }

        /// <summary>
        /// The ToggleHighlightControllerElement method is a shortcut method that makes it easier to highlight and unhighlight a controller element in a single method rather than using the HighlightControllerElement and UnhighlightControllerElement methods separately.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the given element and `false` will remove the highlight from the given element.</param>
        /// <param name="element">The element of the controller to apply the highlight to.</param>
        /// <param name="highlight">The colour of the highlight.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightControllerElement(bool state, GameObject element, Color? highlight = null, float duration = 0f)
        {
            if (element)
            {
                if (state)
                {
                    HighlightControllerElement(element, highlight ?? Color.white, duration);
                }
                else
                {
                    UnhighlightControllerElement(element);
                }
            }
        }

        /// <summary>
        /// The ToggleHighlightTrigger method is a shortcut method that makes it easier to toggle the highlight state of the controller trigger element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the trigger and `false` will remove the highlight from the trigger.</param>
        /// <param name="highlight">The colour to highlight the trigger with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightTrigger(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.triggerModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightGrip method is a shortcut method that makes it easier to toggle the highlight state of the controller grip element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the grip and `false` will remove the highlight from the grip.</param>
        /// <param name="highlight">The colour to highlight the grip with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightGrip(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.leftGripModelPath, highlight, duration);
            ToggleHighlightAlias(state, modelElementPaths.rightGripModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightTouchpad method is a shortcut method that makes it easier to toggle the highlight state of the controller touchpad element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the touchpad and `false` will remove the highlight from the touchpad.</param>
        /// <param name="highlight">The colour to highlight the touchpad with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightTouchpad(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.touchpadModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightButtonOne method is a shortcut method that makes it easier to toggle the highlight state of the button one controller element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on button one and `false` will remove the highlight from button one.</param>
        /// <param name="highlight">The colour to highlight button one with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightButtonOne(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.buttonOneModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightButtonTwo method is a shortcut method that makes it easier to toggle the highlight state of the button two controller element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on button two and `false` will remove the highlight from button two.</param>
        /// <param name="highlight">The colour to highlight button two with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightButtonTwo(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.buttonTwoModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightStartMenu method is a shortcut method that makes it easier to toggle the highlight state of the start menu controller element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the start menu and `false` will remove the highlight from the start menu.</param>
        /// <param name="highlight">The colour to highlight the start menu with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightStartMenu(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.startMenuModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlighBody method is a shortcut method that makes it easier to toggle the highlight state of the controller body element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the body and `false` will remove the highlight from the body.</param>
        /// <param name="highlight">The colour to highlight the body with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlighBody(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.bodyModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightController method is a shortcut method that makes it easier to toggle the highlight state of the entire controller.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the entire controller `false` will remove the highlight from the entire controller.</param>
        /// <param name="highlight">The colour to highlight the entire controller with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public virtual void ToggleHighlightController(bool state, Color? highlight = null, float duration = 0f)
        {
            controllerHighlighted = state;
            ToggleHighlightTrigger(state, highlight, duration);
            ToggleHighlightGrip(state, highlight, duration);
            ToggleHighlightTouchpad(state, highlight, duration);
            ToggleHighlightButtonOne(state, highlight, duration);
            ToggleHighlightButtonTwo(state, highlight, duration);
            ToggleHighlightStartMenu(state, highlight, duration);
            ToggleHighlightAlias(state, modelElementPaths.systemMenuModelPath, highlight, duration);
            ToggleHighlightAlias(state, modelElementPaths.bodyModelPath, highlight, duration);
        }

        /// <summary>
        /// The TriggerHapticPulse/1 method calls a single haptic pulse call on the controller for a single tick.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public virtual void TriggerHapticPulse(float strength)
        {
            if (enabled)
            {
                CancelHapticPulse();
                var hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
                VRTK_SDK_Bridge.HapticPulseOnIndex(VRTK_DeviceFinder.GetControllerIndex(gameObject), hapticPulseStrength);
            }
        }

        /// <summary>
        /// The TriggerHapticPulse/3 method calls a haptic pulse for a specified amount of time rather than just a single tick. Each pulse can be separated by providing a `pulseInterval` to pause between each haptic pulse.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        /// <param name="duration">The length of time the rumble should continue for.</param>
        /// <param name="pulseInterval">The interval to wait between each haptic pulse.</param>
        public virtual void TriggerHapticPulse(float strength, float duration, float pulseInterval)
        {
            if (enabled)
            {
                CancelHapticPulse();
                var hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
                var hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
                hapticLoop = StartCoroutine(HapticPulse(duration * hapticModifiers.durationModifier, hapticPulseStrength, pulseInterval * hapticModifiers.intervalModifier));
            }
        }

        /// <summary>
        /// The InitaliseHighlighters method sets up the highlighters on the controller model.
        /// </summary>
        public virtual void InitaliseHighlighters()
        {
            highlighterOptions = new Dictionary<string, object>();
            highlighterOptions.Add("resetMainTexture", true);
            VRTK_BaseHighlighter objectHighlighter = VRTK_BaseHighlighter.GetActiveHighlighter(gameObject);

            if (objectHighlighter == null)
            {
                objectHighlighter = gameObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }

            var controllerHand = VRTK_DeviceFinder.GetControllerHand(gameObject);

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

        protected virtual void Awake()
        {
            cachedElements = new Dictionary<string, Transform>();

            var controllerHand = VRTK_DeviceFinder.GetControllerHand(gameObject);

            if (modelElementPaths.bodyModelPath.Trim() == "")
            {
                modelElementPaths.bodyModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Body, controllerHand);
            }
            if (modelElementPaths.triggerModelPath.Trim() == "")
            {
                modelElementPaths.triggerModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Trigger, controllerHand);
            }
            if (modelElementPaths.leftGripModelPath.Trim() == "")
            {
                modelElementPaths.leftGripModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.GripLeft, controllerHand);
            }
            if (modelElementPaths.rightGripModelPath.Trim() == "")
            {
                modelElementPaths.rightGripModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.GripRight, controllerHand);
            }
            if (modelElementPaths.touchpadModelPath.Trim() == "")
            {
                modelElementPaths.touchpadModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.Touchpad, controllerHand);
            }
            if (modelElementPaths.buttonOneModelPath.Trim() == "")
            {
                modelElementPaths.buttonOneModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.ButtonOne, controllerHand);
            }
            if (modelElementPaths.buttonTwoModelPath.Trim() == "")
            {
                modelElementPaths.buttonTwoModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.ButtonTwo, controllerHand);
            }
            if (modelElementPaths.systemMenuModelPath.Trim() == "")
            {
                modelElementPaths.systemMenuModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.SystemMenu, controllerHand);
            }
            if (modelElementPaths.startMenuModelPath.Trim() == "")
            {
                modelElementPaths.startMenuModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_BaseController.ControllerElements.StartMenu, controllerHand);
            }
        }

        protected virtual void OnEnable()
        {
            modelContainer = (!modelContainer ? VRTK_DeviceFinder.GetModelAliasController(gameObject) : modelContainer);
            StartCoroutine(WaitForModel());
        }

        private IEnumerator WaitForModel()
        {
            while (GetElementTransform(modelElementPaths.bodyModelPath) == null)
            {
                yield return null;
            }

            InitaliseHighlighters();
        }

        private void AddHighlighterToElement(Transform element, VRTK_BaseHighlighter parentHighlighter, VRTK_BaseHighlighter overrideHighlighter)
        {
            if (element)
            {
                var highlighter = (overrideHighlighter != null ? overrideHighlighter : parentHighlighter);
                VRTK_BaseHighlighter clonedHighlighter = (VRTK_BaseHighlighter)VRTK_SharedMethods.CloneComponent(highlighter, element.gameObject);
                clonedHighlighter.Initialise(null, highlighterOptions);
            }
        }

        private void CancelHapticPulse()
        {
            if (hapticLoop != null)
            {
                StopCoroutine(hapticLoop);
            }
        }

        private IEnumerator HapticPulse(float duration, float hapticPulseStrength, float pulseInterval)
        {
            if (pulseInterval <= 0)
            {
                yield break;
            }

            while (duration > 0)
            {
                VRTK_SDK_Bridge.HapticPulseOnIndex(VRTK_DeviceFinder.GetControllerIndex(gameObject), hapticPulseStrength);
                yield return new WaitForSeconds(pulseInterval);
                duration -= pulseInterval;
            }
        }

        private IEnumerator CycleColor(Material material, Color startColor, Color endColor, float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                if (material.HasProperty("_Color"))
                {
                    material.color = Color.Lerp(startColor, endColor, (elapsedTime / duration));
                }
                yield return null;
            }
        }

        private Transform GetElementTransform(string path)
        {
            if (cachedElements == null || path == null)
            {
                return null;
            }

            if (!cachedElements.ContainsKey(path) || cachedElements[path] == null)
            {
                if (!modelContainer)
                {
                    Debug.LogError("No model container could be found. Have you selected a valid Controller SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Controller SDK from the dropdown.");
                    return null;
                }
                cachedElements[path] = modelContainer.transform.Find(path);
            }
            return cachedElements[path];
        }

        private void ToggleHighlightAlias(bool state, string transformPath, Color? highlight, float duration = 0f)
        {
            var element = GetElementTransform(transformPath);
            if (element)
            {
                ToggleHighlightControllerElement(state, element.gameObject, highlight, duration);
            }
        }

        private ControllerActionsEventArgs SetActionEvent(uint index)
        {
            ControllerActionsEventArgs e;
            e.controllerIndex = index;
            return e;
        }

        private void ToggleModelRenderers(GameObject obj, bool state, GameObject grabbedChildObject)
        {
            if (obj)
            {
                foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                {
                    if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
                    {
                        renderer.enabled = state;
                    }
                }
            }
        }

        private void SetModelOpacity(GameObject obj, float alpha)
        {
            if (obj)
            {
                foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
                {
                    if (alpha < 1f)
                    {
                        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        renderer.material.SetInt("_ZWrite", 0);
                        renderer.material.DisableKeyword("_ALPHATEST_ON");
                        renderer.material.DisableKeyword("_ALPHABLEND_ON");
                        renderer.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        renderer.material.renderQueue = 3000;
                    }
                    else
                    {
                        renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        renderer.material.SetInt("_ZWrite", 1);
                        renderer.material.DisableKeyword("_ALPHATEST_ON");
                        renderer.material.DisableKeyword("_ALPHABLEND_ON");
                        renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        renderer.material.renderQueue = -1;
                    }

                    if (renderer.material.HasProperty("_Color"))
                    {
                        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, alpha);
                    }
                }
            }
        }
    }
}