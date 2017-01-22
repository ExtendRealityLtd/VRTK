// Material Property Block Colour Swap|Highlighters|40021
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// This highlighter swaps the texture colour for the given highlight colour using MaterialPropertyBlocks.
    /// The effect of this highlighter is the same as of the VRTK_MaterialColorSwapHighlighter.cs but this highlighter
    /// can additionally handle objects which make use material instances.
    /// </summary>
    /// <remarks>
    /// Due to the way the object material is interacted with, changing the material colour will break Draw Call Batching in Unity whilst the object is highlighted.
    ///
    /// The Draw Call Batching will resume on the original material when the item is no longer highlighted.
    /// </remarks>
    public class VRTK_MaterialPropertyBlockColorSwapHighlighter : VRTK_MaterialColorSwapHighlighter
    {
        private Dictionary<string, MaterialPropertyBlock> originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
        private Dictionary<string, MaterialPropertyBlock> highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();

        /// <summary>
        /// The Initialise method sets up the highlighter for use.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="options">A dictionary array containing the highlighter options:\r     * `&lt;'resetMainTexture', bool&gt;` - Determines if the default main texture should be cleared on highlight. `true` to reset the main default texture, `false` to not reset it.</param>
        public override void Initialise(Color? color = null, Dictionary<string, object> options = null)
        {
            originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
            highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
            // call to parent highlighter
            base.Initialise(color, options);
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

        protected override void StoreOriginalMaterials()
        {
            originalMaterialPropertyBlocks.Clear();
            highlightMaterialPropertyBlocks.Clear();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                originalMaterialPropertyBlocks[objectReference] = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(originalMaterialPropertyBlocks[objectReference]);
                highlightMaterialPropertyBlocks[objectReference] = new MaterialPropertyBlock();
            }
        }

        protected override void ChangeToHighlightColor(Color color, float duration = 0f)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                if (!originalMaterialPropertyBlocks.ContainsKey(objectReference))
                {
                    continue;
                }

                if (faderRoutines.ContainsKey(objectReference) && faderRoutines[objectReference] != null)
                {
                    StopCoroutine(faderRoutines[objectReference]);
                    faderRoutines.Remove(objectReference);
                }

                var highlightMaterialPropertyBlock = highlightMaterialPropertyBlocks[objectReference];
                renderer.GetPropertyBlock(highlightMaterialPropertyBlock);
                if (resetMainTexture)
                {
                    highlightMaterialPropertyBlock.SetTexture("_MainTex", Texture2D.whiteTexture);
                }

                if (duration > 0f)
                {
                    faderRoutines[objectReference] = StartCoroutine(CycleColor(renderer, highlightMaterialPropertyBlock, color, duration));
                }
                else
                {
                    highlightMaterialPropertyBlock.SetColor("_Color", color);
                    highlightMaterialPropertyBlock.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, emissionDarken));
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
                if (!renderer)
                {
                    yield break;
                }
                renderer.SetPropertyBlock(highlightMaterialPropertyBlock);
                yield return null;
            }
        }
    }
}
