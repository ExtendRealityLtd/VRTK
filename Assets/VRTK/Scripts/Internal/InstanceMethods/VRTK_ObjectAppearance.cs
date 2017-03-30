namespace VRTK
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Highlighters;

    public class VRTK_ObjectAppearance : MonoBehaviour
    {
        protected Dictionary<GameObject, Coroutine> setOpacityCoroutines = new Dictionary<GameObject, Coroutine>();

        /// <summary>
        /// The SetOpacity method allows the opacity of the given GameObject to be changed. A lower alpha value will make the object more transparent, such as `0.5f` will make the controller partially transparent where as `0f` will make the controller completely transparent.
        /// </summary>
        /// <param name="model">The GameObject to change the renderer opacity on.</param>
        /// <param name="alpha">The alpha level to apply to opacity of the controller object. `0f` to `1f`.</param>
        /// <param name="transitionDuration">The time to transition from the current opacity to the new opacity.</param>
        public virtual void SetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
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

        /// <summary>
        /// The SetRendererVisible method turns on renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to show the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public virtual void SetRendererVisible(GameObject model, GameObject ignoredModel = null)
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

        /// <summary>
        /// The SetRendererHidden method turns off renderers of a given GameObject. It can also be provided with an optional model to ignore the render toggle on.
        /// </summary>
        /// <param name="model">The GameObject to hide the renderers for.</param>
        /// <param name="ignoredModel">An optional GameObject to ignore the renderer toggle on.</param>
        public virtual void SetRendererHidden(GameObject model, GameObject ignoredModel = null)
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

        /// <summary>
        /// The HighlightObject method calls the Highlight method on the highlighter attached to the given GameObject with the provided colour.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Highlight on.</param>
        /// <param name="highlightColor">The colour to highlight to.</param>
        /// <param name="fadeDuration">The duration in time to fade from the initial colour to the target colour.</param>
        public virtual void HighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
        {
            VRTK_BaseHighlighter highlighter = model.GetComponentInChildren<VRTK_BaseHighlighter>();
            if (model.activeInHierarchy && highlighter != null)
            {
                highlighter.Highlight((highlightColor != null ? highlightColor : Color.white), fadeDuration);
            }
        }

        /// <summary>
        /// The UnhighlightObject method calls the Unhighlight method on the highlighter attached to the given GameObject.
        /// </summary>
        /// <param name="model">The GameObject to attempt to call the Unhighlight on.</param>
        public virtual void UnhighlightObject(GameObject model)
        {
            VRTK_BaseHighlighter highlighter = model.GetComponentInChildren<VRTK_BaseHighlighter>();
            if (model.activeInHierarchy && highlighter != null)
            {
                highlighter.Unhighlight();
            }
        }

        protected virtual void OnDisable()
        {
            foreach (KeyValuePair<GameObject, Coroutine> setOpacityCoroutine in setOpacityCoroutines)
            {
                CancelSetOpacityCoroutine(setOpacityCoroutine.Key);
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