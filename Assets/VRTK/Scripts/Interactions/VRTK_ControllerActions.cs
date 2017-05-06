// Controller Actions|Interactions|30020
namespace VRTK
{
    using UnityEngine;
    using System;

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="controllerIndex">The index of the controller that was used.</param>
    [Obsolete("`ControllerActionsEventArgs` will be removed in a future version of VRTK.")]
    public struct ControllerActionsEventArgs
    {
        public uint controllerIndex;
    }

    /// <summary>
    /// Event Payload
    /// </summary>
    /// <param name="sender">this object</param>
    /// <param name="e"><see cref="ControllerActionsEventArgs"/></param>
    [Obsolete("`ControllerActionsEventHandler` will be removed in a future version of VRTK.")]
    public delegate void ControllerActionsEventHandler(object sender, ControllerActionsEventArgs e);

    /// <summary>
    /// The Controller Actions script provides helper methods to deal with common controller actions. It deals with actions that can be done to the controller.
    /// </summary>
    /// <remarks>
    /// The highlighting of the controller is defaulted to use the `VRTK_MaterialColorSwapHighlighter` if no other highlighter is applied to the Object.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the ability to change the opacity of a controller model and to highlight specific elements of a controller such as the buttons or even the entire controller model.
    /// </example>
    [Obsolete("`VRTK_ControllerActions` has been replaced with a combination of `VRTK_ControllerHighlighter` and calls to `VRTK_SharedMethods`. This script will be removed in a future version of VRTK.")]
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
        [Obsolete("`VRTK_ControllerActions.modelElementPaths` has been replaced with `VRTK_ControllerHighlighter.modelElementPaths`, it will be removed in a future version of VRTK.")]
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
        [Obsolete("`VRTK_ControllerActions.elementHighlighterOverrides` has been replaced with `VRTK_ControllerHighlighter.elementHighlighterOverrides`, it will be removed in a future version of VRTK.")]
        public VRTK_ControllerElementHighlighters elementHighlighterOverrides;

        /// <summary>
        /// Emitted when the controller model is toggled to be visible.
        /// </summary>
        [Obsolete("`VRTK_ControllerActions.ControllerModelVisible` has been replaced with `VRTK_ControllerEvents.ControllerVisible`. This method will be removed in a future version of VRTK.")]
        public event ControllerActionsEventHandler ControllerModelVisible;

        /// <summary>
        /// Emitted when the controller model is toggled to be invisible.
        /// </summary>
        [Obsolete("`VRTK_ControllerActions.ControllerModelInvisible` has been replaced with `VRTK_ControllerEvents.ControllerHidden`. This method will be removed in a future version of VRTK.")]
        public event ControllerActionsEventHandler ControllerModelInvisible;

        protected GameObject modelContainer;
        protected bool controllerVisible = true;
        protected VRTK_ControllerHighlighter controllerHighlighter;
        protected bool generateControllerHighlighter;

        [Obsolete("`VRTK_ControllerActions.OnControllerModelVisible(e)` has been replaced with `VRTK_ControllerEvents.OnControllerVisible(e)`. This method will be removed in a future version of VRTK.")]
        public virtual void OnControllerModelVisible(ControllerActionsEventArgs e)
        {
            if (ControllerModelVisible != null)
            {
                ControllerModelVisible(this, e);
            }
        }

        [Obsolete("`VRTK_ControllerActions.OnControllerModelInvisible(e)` has been replaced with `VRTK_ControllerEvents.OnControllerHidden(e)`. This method will be removed in a future version of VRTK.")]
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
        [Obsolete("`VRTK_ControllerActions.IsControllerVisible()` has been replaced with `VRTK_ControllerEvents.controllerVisible`. This method will be removed in a future version of VRTK.")]
        public virtual bool IsControllerVisible()
        {
            return controllerVisible;
        }

        /// <summary>
        /// The ToggleControllerModel method is used to turn on or off the controller model by enabling or disabling the renderers on the object. It will also work for any custom controllers. It should also not disable any objects being held by the controller if they are a child of the controller object.
        /// </summary>
        /// <param name="state">The visibility state to toggle the controller to, `true` will make the controller visible - `false` will hide the controller model.</param>
        /// <param name="grabbedChildObject">If an object is being held by the controller then this can be passed through to prevent hiding the grabbed game object as well.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleControllerModel(state, grabbedChildObject)` has been replaced with `VRTK_SharedMethods.ToggleRenderer(model, state, ignoredModel)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleControllerModel(bool state, GameObject grabbedChildObject)
        {
            if (!enabled)
            {
                return;
            }

            VRTK_SharedMethods.ToggleRenderer(state, modelContainer, grabbedChildObject);

            //<obsolete>
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
            //</obsolete>
        }

        /// <summary>
        /// The SetControllerOpacity method allows the opacity of the controller model to be changed to make the controller more transparent. A lower alpha value will make the object more transparent, such as `0.5f` will make the controller partially transparent where as `0f` will make the controller completely transparent.
        /// </summary>
        /// <param name="alpha">The alpha level to apply to opacity of the controller object. `0f` to `1f`.</param>
        [Obsolete("`VRTK_ControllerActions.SetControllerOpacity(alpha)` has been replaced with `VRTK_SharedMethods.SetOpacity(model, alpha)`. This method will be removed in a future version of VRTK.")]
        public virtual void SetControllerOpacity(float alpha)
        {
            if (!enabled)
            {
                return;
            }

            VRTK_SharedMethods.SetOpacity(modelContainer, alpha);
        }

        /// <summary>
        /// The HighlightControllerElement method allows for an element of the controller to have its colour changed to simulate a highlighting effect of that element on the controller. It's useful for being able to draw a user's attention to a specific button on the controller.
        /// </summary>
        /// <param name="element">The element of the controller to apply the highlight to.</param>
        /// <param name="highlight">The colour of the highlight.</param>
        /// <param name="fadeDuration">The duration of fade from white to the highlight colour. Optional parameter defaults to `0f`.</param>
        [Obsolete("`VRTK_ControllerActions.HighlightControllerElement(element, highlight, fadeDuration)` has been replaced with `VRTK_SharedMethods.HighlightObject(model, highlight, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void HighlightControllerElement(GameObject element, Color? highlight, float fadeDuration = 0f)
        {
            if (!enabled)
            {
                return;
            }

            VRTK_SharedMethods.HighlightObject(element, highlight, fadeDuration);
        }

        /// <summary>
        /// The UnhighlightControllerElement method is the inverse of the HighlightControllerElement method and resets the controller element to its original colour.
        /// </summary>
        /// <param name="element">The element of the controller to remove the highlight from.</param>
        [Obsolete("`VRTK_ControllerActions.UnhighlightControllerElement(element)` has been replaced with `VRTK_SharedMethods.UnhighlightObject(model)`. This method will be removed in a future version of VRTK.")]
        public virtual void UnhighlightControllerElement(GameObject element)
        {
            if (!enabled)
            {
                return;
            }

            VRTK_SharedMethods.UnhighlightObject(element);
        }

        /// <summary>
        /// The ToggleHighlightControllerElement method is a shortcut method that makes it easier to highlight and unhighlight a controller element in a single method rather than using the HighlightControllerElement and UnhighlightControllerElement methods separately.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the given element and `false` will remove the highlight from the given element.</param>
        /// <param name="element">The element of the controller to apply the highlight to.</param>
        /// <param name="highlight">The colour of the highlight.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightControllerElement(state, element, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)/UnhighlightElement(elementType)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightControllerElement(bool state, GameObject element, Color? highlight = null, float duration = 0f)
        {
            if (element)
            {
                if (state)
                {
                    VRTK_SharedMethods.HighlightObject(element, (highlight != null ? highlight : Color.white), duration);
                }
                else
                {
                    VRTK_SharedMethods.UnhighlightObject(element);
                }
            }
        }

        /// <summary>
        /// The ToggleHighlightTrigger method is a shortcut method that makes it easier to toggle the highlight state of the controller trigger element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the trigger and `false` will remove the highlight from the trigger.</param>
        /// <param name="highlight">The colour to highlight the trigger with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightTrigger(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightTrigger(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.Trigger, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightGrip method is a shortcut method that makes it easier to toggle the highlight state of the controller grip element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the grip and `false` will remove the highlight from the grip.</param>
        /// <param name="highlight">The colour to highlight the grip with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightGrip(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightGrip(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.GripLeft, highlight, duration);
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.GripRight, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightTouchpad method is a shortcut method that makes it easier to toggle the highlight state of the controller touchpad element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the touchpad and `false` will remove the highlight from the touchpad.</param>
        /// <param name="highlight">The colour to highlight the touchpad with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightTouchpad(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightTouchpad(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.Touchpad, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightButtonOne method is a shortcut method that makes it easier to toggle the highlight state of the button one controller element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on button one and `false` will remove the highlight from button one.</param>
        /// <param name="highlight">The colour to highlight button one with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightButtonOne(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightButtonOne(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.ButtonOne, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightButtonTwo method is a shortcut method that makes it easier to toggle the highlight state of the button two controller element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on button two and `false` will remove the highlight from button two.</param>
        /// <param name="highlight">The colour to highlight button two with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightButtonTwo(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightButtonTwo(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.ButtonTwo, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightStartMenu method is a shortcut method that makes it easier to toggle the highlight state of the start menu controller element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the start menu and `false` will remove the highlight from the start menu.</param>
        /// <param name="highlight">The colour to highlight the start menu with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightStartMenu(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightStartMenu(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.StartMenu, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlighBody method is a shortcut method that makes it easier to toggle the highlight state of the controller body element.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the body and `false` will remove the highlight from the body.</param>
        /// <param name="highlight">The colour to highlight the body with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlighBody(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightElement(elementType, color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlighBody(bool state, Color? highlight = null, float duration = 0f)
        {
            ToggleElementHighlight(state, SDK_BaseController.ControllerElements.Body, highlight, duration);
        }

        /// <summary>
        /// The ToggleHighlightController method is a shortcut method that makes it easier to toggle the highlight state of the entire controller.
        /// </summary>
        /// <param name="state">The highlight colour state, `true` will enable the highlight on the entire controller `false` will remove the highlight from the entire controller.</param>
        /// <param name="highlight">The colour to highlight the entire controller with.</param>
        /// <param name="duration">The duration of fade from white to the highlight colour.</param>
        [Obsolete("`VRTK_ControllerActions.ToggleHighlightController(state, highlight, duration)` has been replaced with `VRTK_ControllerHighlighter.HighlightController(color, fadeDuration)`. This method will be removed in a future version of VRTK.")]
        public virtual void ToggleHighlightController(bool state, Color? highlight = null, float duration = 0f)
        {
            if (controllerHighlighter != null)
            {
                if (state)
                {
                    controllerHighlighter.HighlightController((Color)(highlight == null ? Color.white : highlight), duration);
                }
                else
                {
                    controllerHighlighter.UnhighlightController();
                }
            }
        }

        /// <summary>
        /// The TriggerHapticPulse/1 method calls a single haptic pulse call on the controller for a single tick.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        [Obsolete("`VRTK_ControllerActions.TriggerHapticPulse(strength)` has been replaced with `VRTK_SharedMethods.TriggerHapticPulse(index, strength)`. This method will be removed in a future version of VRTK.")]
        public virtual void TriggerHapticPulse(float strength)
        {
            VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(gameObject), strength);
        }

        /// <summary>
        /// The TriggerHapticPulse/3 method calls a haptic pulse for a specified amount of time rather than just a single tick. Each pulse can be separated by providing a `pulseInterval` to pause between each haptic pulse.
        /// </summary>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        /// <param name="duration">The length of time the rumble should continue for.</param>
        /// <param name="pulseInterval">The interval to wait between each haptic pulse.</param>
        [Obsolete("`VRTK_ControllerActions.TriggerHapticPulse(strength, duration, pulseInterval)` has been replaced with `VRTK_SharedMethods.TriggerHapticPulse(index, strength, duration, pulseInterval)`. This method will be removed in a future version of VRTK.")]
        public virtual void TriggerHapticPulse(float strength, float duration, float pulseInterval)
        {
            VRTK_SharedMethods.TriggerHapticPulse(VRTK_DeviceFinder.GetControllerIndex(gameObject), strength, duration, pulseInterval);
        }

        /// <summary>
        /// The InitaliseHighlighters method sets up the highlighters on the controller model.
        /// </summary>
        [Obsolete("`VRTK_ControllerActions.InitaliseHighlighters()` has been replaced with `VRTK_ControllerHighlighter.PopulateHighlighters()`. This method will be removed in a future version of VRTK.")]
        public virtual void InitaliseHighlighters()
        {
            controllerHighlighter.PopulateHighlighters();
        }

        protected virtual void OnEnable()
        {
            modelContainer = (!modelContainer ? VRTK_DeviceFinder.GetModelAliasController(gameObject) : modelContainer);

            generateControllerHighlighter = false;
            VRTK_ControllerHighlighter existingControllerHighlighter = GetComponent<VRTK_ControllerHighlighter>();
            if (existingControllerHighlighter == null)
            {
                generateControllerHighlighter = true;
                controllerHighlighter = gameObject.AddComponent<VRTK_ControllerHighlighter>();
                controllerHighlighter.modelElementPaths = modelElementPaths;
                controllerHighlighter.elementHighlighterOverrides = elementHighlighterOverrides;
                controllerHighlighter.ConfigureControllerPaths();
            }
            else
            {
                controllerHighlighter = existingControllerHighlighter;
            }
        }

        protected virtual void OnDisable()
        {
            if (generateControllerHighlighter)
            {
                Destroy(controllerHighlighter);
            }
        }

        protected virtual void ToggleElementHighlight(bool state, SDK_BaseController.ControllerElements elementType, Color? color, float fadeDuration = 0f)
        {
            if (controllerHighlighter != null)
            {
                if (state)
                {
                    controllerHighlighter.HighlightElement(elementType, (Color) (color == null ? Color.white : color), fadeDuration);
                }
                else
                {
                    controllerHighlighter.UnhighlightElement(elementType);
                }
            }
        }

        protected virtual ControllerActionsEventArgs SetActionEvent(uint index)
        {
            ControllerActionsEventArgs e;
            e.controllerIndex = index;
            return e;
        }
    }
}