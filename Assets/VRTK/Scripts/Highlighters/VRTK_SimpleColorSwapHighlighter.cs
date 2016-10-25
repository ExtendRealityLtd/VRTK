// Simple Colour Swap|Highlighters|0020
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The Simple Colour Swap Highlighter is a basic implementation that simply swaps the texture colour for the given highlight colour.
    /// </summary>
    /// <remarks>
    /// Due to the way the object material is interacted with, changing the material colour will break Draw Call Batching in Unity whilst the object is highlighted.
    ///
    /// The Draw Call Batching will resume on the original material when the item is no longer highlighted.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/005_Controller_BasicObjectGrabbing` demonstrates the solid highlighting on the green cube, red cube and flying saucer when the controller touches it.
    ///
    /// `VRTK/Examples/035_Controller_OpacityAndHighlighting` demonstrates the solid highlighting if the right controller collides with the green box or if any of the buttons are pressed.
    /// </example>
    public class VRTK_SimpleColorSwapHighlighter : VRTK_BaseHighlighter
    {
        [Tooltip("The emission colour of the texture will be the highlight colour but this percent darker.")]
        public float emissionDarken = 50f;

        private Dictionary<string, MaterialPropertyBlock> originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
        private Dictionary<string, MaterialPropertyBlock> highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
        private Dictionary<string, Coroutine> faderRoutines;

        /// <summary>
        /// The Initialise method sets up the highlighter for use.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="options">Not used.</param>
        public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
        {
            originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
            highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
            faderRoutines = new Dictionary<string, Coroutine>();
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
            if (originalMaterialPropertyBlocks == null)
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
                if (!originalMaterialPropertyBlocks.ContainsKey(objectReference))
                {
                    continue;
                }

                renderer.SetPropertyBlock(originalMaterialPropertyBlocks[objectReference]);
            }
        }

        private void StoreOriginalMaterials()
        {
            originalMaterialPropertyBlocks.Clear();
            highlightMaterialPropertyBlocks.Clear();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                originalMaterialPropertyBlocks[objectReference] = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(originalMaterialPropertyBlocks[objectReference]);
                highlightMaterialPropertyBlocks[objectReference] = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(highlightMaterialPropertyBlocks[objectReference]);
            }
        }

        private void ChangeToHighlightColor(Color color, float duration = 0f)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();

                if (faderRoutines.ContainsKey(objectReference) && faderRoutines[objectReference] != null)
                {
                    StopCoroutine(faderRoutines[objectReference]);
                    faderRoutines.Remove(objectReference);
                }

                var highlightMaterialPropertyBlock = highlightMaterialPropertyBlocks[objectReference];
                if (duration > 0f)
                {
                    faderRoutines[objectReference] = StartCoroutine(CycleColor(renderer, highlightMaterialPropertyBlock, color, duration));
                }
                else
                {
                    highlightMaterialPropertyBlock.SetColor("_Color", color);
                    highlightMaterialPropertyBlock.SetColor("_EmissionColor", Utilities.ColorDarken(color, emissionDarken));
                    renderer.SetPropertyBlock(highlightMaterialPropertyBlock);
                }
            }
        }

        private IEnumerator CycleColor(Renderer renderer, MaterialPropertyBlock highlightMaterialPropertyBlock, Color endColor, float duration)
        {
            var elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                Color startColor = highlightMaterialPropertyBlock.GetVector("_Color");
                highlightMaterialPropertyBlock.SetColor("_Color", Color.Lerp(startColor, endColor, (elapsedTime / duration)));
                highlightMaterialPropertyBlock.SetColor("_EmissionColor", Color.Lerp(startColor, endColor, (elapsedTime / duration)));
                renderer.SetPropertyBlock(highlightMaterialPropertyBlock);
                yield return null;
            }
        }
    }
}