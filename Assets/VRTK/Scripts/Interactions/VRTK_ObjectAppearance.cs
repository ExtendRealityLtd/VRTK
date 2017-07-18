﻿// Object Appearance|Interactions|30090
namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;

    /// <summary>
    /// The Object Appearance script provides a collection of static methods for controlling the appearance of a GameObject.
    /// </summary>
    /// <remarks>
    /// The GameObject can have it's opacity changed, or it's renderers toggled, or highlighters toggled.
    /// </remarks>
    public class VRTK_ObjectAppearance : MonoBehaviour
    {
        protected static VRTK_ObjectAppearance instance;
        protected Dictionary<GameObject, Coroutine> setOpacityCoroutines = new Dictionary<GameObject, Coroutine>();

        /// <summary>
        /// The SetOpacity method allows the opacity of the given GameObject to be changed. A lower alpha value will make the object more transparent, such as `0.5f` will make the controller partially transparent where as `0f` will make the controller completely transparent.
        /// </summary>
        /// <param name="model">The GameObject to change the renderer opacity on.</param>
        /// <param name="alpha">The alpha level to apply to opacity of the controller object. `0f` to `1f`.</param>
        /// <param name="transitionDuration">The time to transition from the current opacity to the new opacity.</param>
        public static void SetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalSetOpacity(model, alpha, transitionDuration);
            }
        }

        /// <summary>
        /// The SetRendererVisible method turns on renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to show the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public static void SetRendererVisible(GameObject model, GameObject ignoredModel = null)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalSetRendererVisible(model, ignoredModel);
            }
        }

        /// <summary>
        /// The SetRendererHidden method turns off renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to hide the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public static void SetRendererHidden(GameObject model, GameObject ignoredModel = null)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalSetRendererHidden(model, ignoredModel);
            }
        }

        /// <summary>
        /// The ToggleRenderer method turns on or off the renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle of.
        /// </summary>
        /// <param name="state">If true then the renderers will be enabled, if false the renderers will be disabled.</param>
        /// <param name="model">The GameObject to toggle the renderer states of.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public static void ToggleRenderer(bool state, GameObject model, GameObject ignoredModel = null)
        {
            if (state)
            {
                SetRendererVisible(model, ignoredModel);
            }
            else
            {
                SetRendererHidden(model, ignoredModel);
            }
        }

        /// <summary>
        /// The HighlightObject method calls the Highlight method on the highlighter attached to the given GameObject with the provided colour.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Highlight on.</param>
        /// <param name="highlightColor">The colour to highlight to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        public static void HighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalHighlightObject(model, highlightColor, fadeDuration);
            }
        }

        /// <summary>
        /// The UnhighlightObject method calls the Unhighlight method on the highlighter attached to the given GameObject.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Unhighlight on.</param>
        public static void UnhighlightObject(GameObject model)
        {
            SetupInstance();
            if (instance != null)
            {
                instance.InternalUnhighlightObject(model);
            }
        }

        protected virtual void OnDisable()
        {
            foreach (KeyValuePair<GameObject, Coroutine> setOpacityCoroutine in setOpacityCoroutines)
            {
                CancelSetOpacityCoroutine(setOpacityCoroutine.Key);
            }
        }

        protected static void SetupInstance()
        {
            if (instance == null && VRTK_SDKManager.instance != null)
            {
                instance = VRTK_SDKManager.instance.gameObject.AddComponent<VRTK_ObjectAppearance>();
            }
        }

        protected virtual void InternalSetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
        {
            if (model && model.activeInHierarchy)
            {
                if (transitionDuration == 0f)
                {
                    ChangeRendererOpacity(model, alpha);
                }
                else
                {
                    CancelSetOpacityCoroutine(model);
                    Coroutine setOpacityCoroutine = StartCoroutine(TransitionRendererOpacity(model, GetInitialAlpha(model), alpha, transitionDuration));
                    if (!setOpacityCoroutines.ContainsKey(model))
                    {
                        setOpacityCoroutines.Add(model, setOpacityCoroutine);
                    }
                }
            }
        }

        protected virtual void InternalSetRendererVisible(GameObject model, GameObject ignoredModel = null)
        {
            if (model != null)
            {
                foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
                {
                    if (renderer.gameObject != ignoredModel && (ignoredModel == null || !renderer.transform.IsChildOf(ignoredModel.transform)))
                    {
                        renderer.enabled = true;
                    }
                }
            }

            EmitControllerEvents(model, true);
        }

        protected virtual void InternalSetRendererHidden(GameObject model, GameObject ignoredModel = null)
        {
            if (model != null)
            {
                foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
                {
                    if (renderer.gameObject != ignoredModel && (ignoredModel == null || !renderer.transform.IsChildOf(ignoredModel.transform)))
                    {
                        renderer.enabled = false;
                    }
                }
            }

            EmitControllerEvents(model, false);
        }

        protected virtual void InternalHighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
        {
            VRTK_BaseHighlighter highlighter = model.GetComponentInChildren<VRTK_BaseHighlighter>();
            if (model.activeInHierarchy && highlighter != null)
            {
                highlighter.Highlight((highlightColor != null ? highlightColor : Color.white), fadeDuration);
            }
        }

        protected virtual void InternalUnhighlightObject(GameObject model)
        {
            VRTK_BaseHighlighter highlighter = model.GetComponentInChildren<VRTK_BaseHighlighter>();
            if (model.activeInHierarchy && highlighter != null)
            {
                highlighter.Unhighlight();
            }
        }

        //If the object is a controller, then emit the relevant event for it.
        protected virtual void EmitControllerEvents(GameObject model, bool state)
        {
            GameObject controllerObject = null;

            //Check to see if the given model is either the left or right controller model alias object
            if (VRTK_DeviceFinder.GetModelAliasControllerHand(model) == SDK_BaseController.ControllerHand.Left)
            {
                controllerObject = VRTK_DeviceFinder.GetControllerLeftHand(false);
            }
            else if (VRTK_DeviceFinder.GetModelAliasControllerHand(model) == SDK_BaseController.ControllerHand.Right)
            {
                controllerObject = VRTK_DeviceFinder.GetControllerRightHand(false);
            }

            //if it is then attempt to get the controller events script from the script alias
            if (controllerObject != null && controllerObject.activeInHierarchy)
            {
                VRTK_ControllerEvents controllerEvents = controllerObject.GetComponent<VRTK_ControllerEvents>();
                if (controllerEvents != null)
                {
                    if (state)
                    {
                        controllerEvents.OnControllerVisible(controllerEvents.SetControllerEvent());
                    }
                    else
                    {
                        controllerEvents.OnControllerHidden(controllerEvents.SetControllerEvent());
                    }
                }
            }
        }

        protected virtual void ChangeRendererOpacity(GameObject model, float alpha)
        {
            if (model != null)
            {
                alpha = Mathf.Clamp(alpha, 0f, 1f);
                foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
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

        protected virtual float GetInitialAlpha(GameObject model)
        {
            Renderer modelRenderer = model.GetComponentInChildren<Renderer>(true);
            if (modelRenderer.material.HasProperty("_Color"))
            {
                return modelRenderer.material.color.a;
            }
            return 0f;
        }

        protected virtual IEnumerator TransitionRendererOpacity(GameObject model, float initialAlpha, float targetAlpha, float transitionDuration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < transitionDuration)
            {
                float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, (elapsedTime / transitionDuration));
                ChangeRendererOpacity(model, newAlpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            ChangeRendererOpacity(model, targetAlpha);
        }

        protected virtual void CancelSetOpacityCoroutine(GameObject model)
        {
            if (setOpacityCoroutines.ContainsKey(model) && setOpacityCoroutines[model] != null)
            {
                StopCoroutine(setOpacityCoroutines[model]);
            }
        }
    }
}