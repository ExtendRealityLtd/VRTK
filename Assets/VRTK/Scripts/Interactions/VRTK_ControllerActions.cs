// Controller Actions|Scripts|0020
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
        public string appMenuModelPath = "";
        public string systemMenuModelPath = "";
    }

    [System.Serializable]
    public struct VRTK_ControllerElementHighlighers
    {
        public VRTK_BaseHighlighter body;
        public VRTK_BaseHighlighter trigger;
        public VRTK_BaseHighlighter gripLeft;
        public VRTK_BaseHighlighter gripRight;
        public VRTK_BaseHighlighter touchpad;
        public VRTK_BaseHighlighter appMenu;
        public VRTK_BaseHighlighter systemMenu;
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
         + " * `App Menu Model Path`: The model that represents the application menu button.\n"
         + " * `System Menu Model Path`: The model that represents the system menu button.")]
        public VRTK_ControllerModelElementPaths modelElementPaths;

        [Tooltip("A collection of highlighter overrides for each controller model sub element. If no highlighter override is given then highlighter on the Controller game object is used.\n\n"
         + "* The available model sub elements are:\n\n"
         + " * `Body`: The highlighter to use on the overall shape of the controller.\n"
         + " * `Trigger`: The highlighter to use on the trigger button.\n"
         + " * `Grip Left`: The highlighter to use on the left grip button.\n"
         + " * `Grip Right`: The highlighter to use on the  right grip button.\n"
         + " * `Touchpad`: The highlighter to use on the touchpad.\n"
         + " * `App Menu`: The highlighter to use on the application menu button.\n"
         + " * `System Menu`: The highlighter to use on the system menu button.")]
        public VRTK_ControllerElementHighlighers elementHighlighterOverrides;

        /// <summary>
        /// Emitted when the controller model is toggled to be visible.
        /// </summary>
        public event ControllerActionsEventHandler ControllerModelVisible;

        /// <summary>
        /// Emitted when the controller model is toggled to be invisible.
        /// </summary>
        public event ControllerActionsEventHandler ControllerModelInvisible;

        private bool controllerVisible = true;
        private ushort hapticPulseStrength;
        private uint controllerIndex;
        private ushort maxHapticVibration = 3999;
        private bool controllerHighlighted = false;
        private Dictionary<string, Transform> cachedElements;
        private Dictionary<string, object> highlighterOptions;

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
        public void ToggleControllerModel(bool state, GameObject grabbedChildObject)
        {
            if (!enabled)
            {
                return;
            }

            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
                {
                    renderer.enabled = state;
                }
            }

            foreach (SkinnedMeshRenderer renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (renderer.gameObject != grabbedChildObject && (grabbedChildObject == null || !renderer.transform.IsChildOf(grabbedChildObject.transform)))
                {
                    renderer.enabled = state;
                }
            }

            controllerVisible = state;
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
        public void SetControllerOpacity(float alpha)
        {
            if (!enabled)
            {
                return;
            }

            alpha = Mathf.Clamp(alpha, 0f, 1f);
            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
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

        /// <summary>
        /// The HighlightControllerElement method allows for an element of the controller to have its colour changed to simulate a highlighting effect of that element on the controller. It's useful for being able to draw a user's attention to a specific button on the controller.
        /// </summary>
        /// <param name="element">The element of the controller to apply the highlight to.</param>
        /// <param name="highlight">The colour of the highlight.</param>
        /// <param name="fadeDuration">The duration of fade from white to the highlight colour. Optional parameter defaults to `0f`.</param>
        public void HighlightControllerElement(GameObject element, Color? highlight, float fadeDuration = 0f)
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
        public void UnhighlightControllerElement(GameObject element)
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
        public void ToggleHighlightControllerElement(bool state, GameObject element, Color? highlight = null, float duration = 0f)
        {
            if (element)
            {
                if (state)
                {
                    HighlightControllerElement(element.gameObject, highlight ?? Color.white, duration);
                }
                else
                {
                    UnhighlightControllerElement(element.gameObject);
                }
            }
        }

        /// <summary>
        /// The ToggleHighlightTrigger method is a shortcut method that makes it easier to toggle the highlight state of the controller trigger element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the trigger and `false` will remove the highlight from the trigger.</param>
        /// <param name="highlight">The colour to highlight the trigger with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public void ToggleHighlightTrigger(bool state, Color? highlight = null, float duration = 0f)
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
        public void ToggleHighlightGrip(bool state, Color? highlight = null, float duration = 0f)
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
        public void ToggleHighlightTouchpad(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.touchpadModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightApplicationMenu method is a shortcut method that makes it easier to toggle the highlight state of the controller application menu element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the application menu and `false` will remove the highlight from the application menu.</param>
        /// <param name="highlight">The colour to highlight the application menu with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public void ToggleHighlightApplicationMenu(bool state, Color? highlight = null, float duration = 0f)
        {
            if (!state && controllerHighlighted)
            {
                return;
            }
            ToggleHighlightAlias(state, modelElementPaths.appMenuModelPath, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlighBody method is a shortcut method that makes it easier to toggle the highlight state of the controller body element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the body and `false` will remove the highlight from the body.</param>
        /// <param name="highlight">The colour to highlight the body with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        public void ToggleHighlighBody(bool state, Color? highlight = null, float duration = 0f)
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
        public void ToggleHighlightController(bool state, Color? highlight = null, float duration = 0f)
        {
            controllerHighlighted = state;
            ToggleHighlightTrigger(state, highlight, duration);
            ToggleHighlightGrip(state, highlight, duration);
            ToggleHighlightTouchpad(state, highlight, duration);
            ToggleHighlightApplicationMenu(state, highlight, duration);
            ToggleHighlightAlias(state, modelElementPaths.systemMenuModelPath, highlight, duration);
            ToggleHighlightAlias(state, modelElementPaths.bodyModelPath, highlight, duration);
        }

        /// <summary>
        /// The TriggerHapticPulse/1 method calls a single haptic pulse call on the controller for a single tick.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `3999`.</param>
        public void TriggerHapticPulse(ushort strength)
        {
            if (!enabled)
            {
                return;
            }

            hapticPulseStrength = (strength <= maxHapticVibration ? strength : maxHapticVibration);
            VRTK_SDK_Bridge.HapticPulseOnIndex(controllerIndex, hapticPulseStrength);
        }

        /// <summary>
        /// The TriggerHapticPulse/3 method calls a haptic pulse for a specified amount of time rather than just a single tick. Each pulse can be separated by providing a `pulseInterval` to pause between each haptic pulse.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `3999`.</param>
        /// <param name="duration">The length of time the rumble should continue for.</param>
        /// <param name="pulseInterval">The interval to wait between each haptic pulse.</param>
        public void TriggerHapticPulse(ushort strength, float duration, float pulseInterval)
        {
            if (!enabled)
            {
                return;
            }

            hapticPulseStrength = (strength <= maxHapticVibration ? strength : maxHapticVibration);
            StartCoroutine(HapticPulse(duration, hapticPulseStrength, pulseInterval));
        }

        /// <summary>
        /// The InitaliseHighlighters method sets up the highlighters on the controller model.
        /// </summary>
        public void InitaliseHighlighters()
        {

            highlighterOptions = new Dictionary<string, object>();
            highlighterOptions.Add("resetMainTexture", true);
            VRTK_BaseHighlighter objectHighlighter = Utilities.GetActiveHighlighter(gameObject);

            if (objectHighlighter == null)
            {
                objectHighlighter = gameObject.AddComponent<VRTK_MaterialColorSwapHighlighter>();
            }

            objectHighlighter.Initialise(null, highlighterOptions);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.ApplicationMenu)), objectHighlighter, elementHighlighterOverrides.appMenu);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.Body)), objectHighlighter, elementHighlighterOverrides.body);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.GripLeft)), objectHighlighter, elementHighlighterOverrides.gripLeft);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.GripRight)), objectHighlighter, elementHighlighterOverrides.gripRight);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.SystemMenu)), objectHighlighter, elementHighlighterOverrides.systemMenu);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.Touchpad)), objectHighlighter, elementHighlighterOverrides.touchpad);
            AddHighlighterToElement(GetElementTransform(VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.Trigger)), objectHighlighter, elementHighlighterOverrides.trigger);
        }

        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            cachedElements = new Dictionary<string, Transform>();

            var controllerHand = VRTK_DeviceFinder.GetControllerHand(gameObject);

            if (modelElementPaths.bodyModelPath.Trim() == "")
            {
                modelElementPaths.bodyModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.Body, controllerHand);
            }
            if (modelElementPaths.triggerModelPath.Trim() == "")
            {
                modelElementPaths.triggerModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.Trigger, controllerHand);
            }
            if (modelElementPaths.leftGripModelPath.Trim() == "")
            {
                modelElementPaths.leftGripModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.GripLeft, controllerHand);
            }
            if (modelElementPaths.rightGripModelPath.Trim() == "")
            {
                modelElementPaths.rightGripModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.GripRight, controllerHand);
            }
            if (modelElementPaths.touchpadModelPath.Trim() == "")
            {
                modelElementPaths.touchpadModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.Touchpad, controllerHand);
            }
            if (modelElementPaths.appMenuModelPath.Trim() == "")
            {
                modelElementPaths.appMenuModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.ApplicationMenu, controllerHand);
            }
            if (modelElementPaths.systemMenuModelPath.Trim() == "")
            {
                modelElementPaths.systemMenuModelPath = VRTK_SDK_Bridge.GetControllerElementPath(SDK_Base.ControllerElelements.SystemMenu, controllerHand);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(WaitForModel());
        }

        private void Update()
        {
            controllerIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
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
                VRTK_BaseHighlighter clonedHighlighter = (VRTK_BaseHighlighter)Utilities.CloneComponent(highlighter, element.gameObject);
                clonedHighlighter.Initialise(null, highlighterOptions);
            }
        }

        private IEnumerator HapticPulse(float duration, ushort hapticPulseStrength, float pulseInterval)
        {
            if (pulseInterval <= 0)
            {
                yield break;
            }

            while (duration > 0)
            {
                VRTK_SDK_Bridge.HapticPulseOnIndex(controllerIndex, hapticPulseStrength);
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
            if (cachedElements == null)
            {
                return null;
            }

            if (!cachedElements.ContainsKey(path) || cachedElements[path] == null)
            {
                cachedElements[path] = transform.Find(path);
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
    }
}