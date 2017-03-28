// Material Colour Swap|Highlighters|40020
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The Material Colour Swap Highlighter is a basic implementation that simply swaps the texture colour for the given highlight colour.
    /// </summary>
    /// <remarks>
    /// Due to the way the object material is interacted with, changing the material colour will break Draw Call Batching in Unity whilst the object is highlighted.
    ///
    /// The Draw Call Batching will resume on the original material when the item is no longer highlighted.
    ///
    /// This is the default highlighter that is applied to any script that requires a highlighting component (e.g. `VRTK_Interactable_Object`).
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the solid highlighting on the green cube, red cube and flying saucer when the controller touches it.
    ///
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the solid highlighting if the right controller collides with the green box or if any of the buttons are pressed.
    /// </example>
    public class VRTK_MaterialColorSwapHighlighter : VRTK_BaseHighlighter
    {
        [Tooltip("The emission colour of the texture will be the highlight colour but this percent darker.")]
        public float emissionDarken = 50f;
        [Tooltip("A custom material to use on the highlighted object.")]
        public Material customMaterial;

        protected Dictionary<string, Material[]> originalSharedRendererMaterials = new Dictionary<string, Material[]>();
        protected Dictionary<string, Material[]> originalRendererMaterials = new Dictionary<string, Material[]>();
        protected Dictionary<string, Coroutine> faderRoutines;
        protected bool resetMainTexture = false;

        /// <summary>
        /// The Initialise method sets up the highlighter for use.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="options">A dictionary array containing the highlighter options:\r     * `&lt;'resetMainTexture', bool&gt;` - Determines if the default main texture should be cleared on highlight. `true` to reset the main default texture, `false` to not reset it.</param>
        public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
        {
            originalSharedRendererMaterials = new Dictionary<string, Material[]>();
            originalRendererMaterials = new Dictionary<string, Material[]>();
            faderRoutines = new Dictionary<string, Coroutine>();
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
            if (originalRendererMaterials == null)
            {
                return;
            }

            if (faderRoutines != null)
            {
                foreach (var fadeRoutine in faderRoutines)
                {
                    StopCoroutine(fadeRoutine.Value);
                }
                faderRoutines.Clear();
            }

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                if (!originalRendererMaterials.ContainsKey(objectReference))
                {
                    continue;
                }

                renderer.materials = originalRendererMaterials[objectReference];
                renderer.sharedMaterials = originalSharedRendererMaterials[objectReference];
            }
        }

        protected virtual void StoreOriginalMaterials()
        {
            originalSharedRendererMaterials.Clear();
            originalRendererMaterials.Clear();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                originalSharedRendererMaterials[objectReference] = renderer.sharedMaterials;
                originalRendererMaterials[objectReference] = renderer.materials;
                renderer.sharedMaterials = originalSharedRendererMaterials[objectReference];
            }
        }

        protected virtual void ChangeToHighlightColor(Color color, float duration = 0f)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                var swapCustomMaterials = new Material[renderer.materials.Length];

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];
                    if (customMaterial)
                    {
                        material = customMaterial;
                        swapCustomMaterials[i] = material;
                    }

                    var faderRoutineID = material.GetInstanceID().ToString();

                    if (faderRoutines.ContainsKey(faderRoutineID) && faderRoutines[faderRoutineID] != null)
                    {
                        StopCoroutine(faderRoutines[faderRoutineID]);
                        faderRoutines.Remove(faderRoutineID);
                    }

                    material.EnableKeyword("_EMISSION");

                    if (resetMainTexture && material.HasProperty("_MainTex"))
                    {
                        renderer.material.SetTexture("_MainTex", new Texture());
                    }

                    if (material.HasProperty("_Color"))
                    {
                        if (duration > 0f)
                        {
                            faderRoutines[faderRoutineID] = StartCoroutine(CycleColor(material, material.color, color, duration));
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

                if (customMaterial)
                {
                    renderer.materials = swapCustomMaterials;
                }
            }
        }

        protected virtual IEnumerator CycleColor(Material material, Color startColor, Color endColor, float duration)
        {
            var elapsedTime = 0f;
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
