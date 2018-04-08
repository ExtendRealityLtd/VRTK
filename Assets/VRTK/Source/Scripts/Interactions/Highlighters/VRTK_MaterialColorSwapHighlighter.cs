// Material Colour Swap|Highlighters|40020
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Swaps the texture colour on the Renderers material for the given highlight colour.
    /// </summary>
    /// <remarks>
    ///   > Due to the way the object material is interacted with, changing the material colour will break Draw Call Batching in Unity whilst the object is highlighted. The Draw Call Batching will resume on the original material when the item is no longer highlighted.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_MaterialColorSwapHighlighter` script on either:
    ///    * The GameObject of the Interactable Object to highlight.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Object Highlighter` parameter to denote use of the highlighter.
    ///  * Ensure the `Active` parameter is checked.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the solid highlighting on the green cube, red cube and flying saucer when the controller touches it.
    ///
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the solid highlighting if the right controller collides with the green box or if any of the buttons are pressed.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Highlighters/VRTK_MaterialColorSwapHighlighter")]
    public class VRTK_MaterialColorSwapHighlighter : VRTK_BaseHighlighter
    {
        [Tooltip("The emission colour of the texture will be the highlight colour but this percent darker.")]
        public float emissionDarken = 50f;
        [Tooltip("A custom material to use on the highlighted object.")]
        public Material customMaterial;

        protected Dictionary<string, Material[]> originalSharedRendererMaterials = new Dictionary<string, Material[]>();
        protected Dictionary<string, Material[]> originalRendererMaterials = new Dictionary<string, Material[]>();
        protected Dictionary<string, Coroutine> faderRoutines = new Dictionary<string, Coroutine>();
        protected bool resetMainTexture = false;

        /// <summary>
        /// The Initialise method sets up the highlighter for use.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="affectObject">An optional GameObject to specify which object to apply the highlighting to.</param>
        /// <param name="options">A dictionary array containing the highlighter options:\r     * `&lt;'resetMainTexture', bool&gt;` - Determines if the default main texture should be cleared on highlight. `true` to reset the main default texture, `false` to not reset it.</param>
        public override void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null)
        {
            objectToAffect = (affectObject != null ? affectObject : gameObject);
            originalSharedRendererMaterials.Clear();
            originalRendererMaterials.Clear();
            faderRoutines.Clear();
            resetMainTexture = GetOption<bool>(options, "resetMainTexture");
            ResetHighlighter();
        }

        /// <summary>
        /// The ResetHighlighter method stores the object's materials and shared materials prior to highlighting.
        /// </summary>
        public override void ResetHighlighter()
        {
            StoreOriginalMaterials();
        }

        /// <summary>
        /// The Highlight method initiates the change of colour on the object and will fade to that colour (from a base white colour) for the given duration.
        /// </summary>
        /// <param name="color">The colour to highlight to.</param>
        /// <param name="duration">The time taken to fade to the highlighted colour.</param>
        public override void Highlight(Color? color, float duration = 0f)
        {
            if (color == null)
            {
                return;
            }
            ChangeToHighlightColor((Color)color, duration);
        }

        /// <summary>
        /// The Unhighlight method returns the object back to it's original colour.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="duration">Not used.</param>
        public override void Unhighlight(Color? color = null, float duration = 0f)
        {
            if (objectToAffect == null)
            {
                return;
            }

            if (faderRoutines != null)
            {
                foreach (KeyValuePair<string, Coroutine> fadeRoutine in faderRoutines)
                {
                    StopCoroutine(fadeRoutine.Value);
                }
                faderRoutines.Clear();
            }

            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                string objectReference = renderer.gameObject.GetInstanceID().ToString();
                Material[] storedMaterials = VRTK_SharedMethods.GetDictionaryValue(originalRendererMaterials, objectReference);
                if (storedMaterials == null)
                {
                    continue;
                }

                Material[] storedSharedMaterials = VRTK_SharedMethods.GetDictionaryValue(originalSharedRendererMaterials, objectReference);
                if (storedSharedMaterials == null)
                {
                    continue;
                }

                renderer.materials = storedMaterials;
                renderer.sharedMaterials = storedSharedMaterials;
            }
        }

        protected virtual void StoreOriginalMaterials()
        {
            originalSharedRendererMaterials.Clear();
            originalRendererMaterials.Clear();
            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                string objectReference = renderer.gameObject.GetInstanceID().ToString();
                VRTK_SharedMethods.AddDictionaryValue(originalSharedRendererMaterials, objectReference, renderer.sharedMaterials, true);
                VRTK_SharedMethods.AddDictionaryValue(originalRendererMaterials, objectReference, renderer.materials, true);
                renderer.sharedMaterials = VRTK_SharedMethods.GetDictionaryValue(originalSharedRendererMaterials, objectReference);
            }
        }

        protected virtual void ChangeToHighlightColor(Color color, float duration = 0f)
        {
            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(true);
            for (int j = 0; j < renderers.Length; j++)
            {
                Renderer renderer = renderers[j];
                Material[] swapCustomMaterials = new Material[renderer.materials.Length];

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    Material material = renderer.materials[i];
                    if (customMaterial != null)
                    {
                        material = customMaterial;
                        swapCustomMaterials[i] = material;
                    }

                    string faderRoutineID = material.GetInstanceID().ToString();
                    Coroutine existingFaderRoutine = VRTK_SharedMethods.GetDictionaryValue(faderRoutines, faderRoutineID);
                    if (existingFaderRoutine != null)
                    {
                        StopCoroutine(existingFaderRoutine);
                        faderRoutines.Remove(faderRoutineID);
                    }

                    material.EnableKeyword("_EMISSION");

                    if (resetMainTexture && material.HasProperty("_MainTex"))
                    {
                        renderer.material.SetTexture("_MainTex", Texture2D.whiteTexture);
                    }

                    if (material.HasProperty("_Color"))
                    {
                        if (duration > 0f)
                        {
                            VRTK_SharedMethods.AddDictionaryValue(faderRoutines, faderRoutineID, StartCoroutine(CycleColor(material, material.color, color, duration)), true);
                        }
                        else
                        {
                            material.color = color;
                            if (material.HasProperty("_EmissionColor"))
                            {
                                material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, emissionDarken));
                            }
                        }
                    }
                }

                if (customMaterial != null)
                {
                    renderer.materials = swapCustomMaterials;
                }
            }
        }

        protected virtual IEnumerator CycleColor(Material material, Color startColor, Color endColor, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                if (material.HasProperty("_Color"))
                {
                    material.color = Color.Lerp(startColor, endColor, (elapsedTime / duration));
                }
                if (material.HasProperty("_EmissionColor"))
                {
                    material.SetColor("_EmissionColor", Color.Lerp(startColor, VRTK_SharedMethods.ColorDarken(endColor, emissionDarken), (elapsedTime / duration)));
                }
                yield return null;
            }
        }
    }
}
