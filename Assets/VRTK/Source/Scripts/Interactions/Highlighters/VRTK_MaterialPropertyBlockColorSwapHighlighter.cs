// Material Property Block Colour Swap|Highlighters|40021
namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Swaps the texture colour on the Renderers material for the given highlight colour using property blocks.
    /// </summary>
    /// <remarks>
    ///   > Utilising the MaterialPropertyBlock means that Draw Call Batching in Unity is not compromised.
    ///
    /// **Script Usage:**
    ///  * Place the `VRTK_MaterialPropertyBlockColorSwapHighlighter` script on either:
    ///    * The GameObject of the Interactable Object to highlight.
    ///    * Any other scene GameObject and then link that GameObject to the Interactable Objects `Object Highlighter` parameter to denote use of the highlighter.
    ///  * Ensure the `Active` parameter is checked.
    /// </remarks>
    [AddComponentMenu("VRTK/Scripts/Interactions/Highlighters/VRTK_MaterialPropertyBlockColorSwapHighlighter")]
    public class VRTK_MaterialPropertyBlockColorSwapHighlighter : VRTK_MaterialColorSwapHighlighter
    {
        protected Dictionary<string, MaterialPropertyBlock> originalMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();
        protected Dictionary<string, MaterialPropertyBlock> highlightMaterialPropertyBlocks = new Dictionary<string, MaterialPropertyBlock>();

        /// <summary>
        /// The Initialise method sets up the highlighter for use.
        /// </summary>
        /// <param name="color">Not used.</param>
        /// <param name="affectObject">An optional GameObject to specify which object to apply the highlighting to.</param>
        /// <param name="options">A dictionary array containing the highlighter options:\r     * `&lt;'resetMainTexture', bool&gt;` - Determines if the default main texture should be cleared on highlight. `true` to reset the main default texture, `false` to not reset it.</param>
        public override void Initialise(Color? color = null, GameObject affectObject = null, Dictionary<string, object> options = null)
        {
            objectToAffect = (affectObject != null ? affectObject : gameObject);
            originalMaterialPropertyBlocks.Clear();
            highlightMaterialPropertyBlocks.Clear();
            // call to parent highlighter
            base.Initialise(color, affectObject, options);
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
                MaterialPropertyBlock storedPropertyBlock = VRTK_SharedMethods.GetDictionaryValue(originalMaterialPropertyBlocks, objectReference);
                if (storedPropertyBlock == null)
                {
                    continue;
                }
                renderer.SetPropertyBlock(storedPropertyBlock);
            }
        }

        protected override void StoreOriginalMaterials()
        {
            originalMaterialPropertyBlocks.Clear();
            highlightMaterialPropertyBlocks.Clear();
            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                string objectReference = renderer.gameObject.GetInstanceID().ToString();
                // get the initial material property block to restore the original material properties later on
                MaterialPropertyBlock originalPropertyBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(originalPropertyBlock);
                VRTK_SharedMethods.AddDictionaryValue(originalMaterialPropertyBlocks, objectReference, originalPropertyBlock, true);
                // we need a second instance of the original material property block which will be modified for highlighting
                MaterialPropertyBlock highlightPropertyBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(highlightPropertyBlock);
                VRTK_SharedMethods.AddDictionaryValue(highlightMaterialPropertyBlocks, objectReference, highlightPropertyBlock, true);
            }
        }

        protected override void ChangeToHighlightColor(Color color, float duration = 0f)
        {
            Renderer[] renderers = objectToAffect.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                string faderRoutineID = renderer.gameObject.GetInstanceID().ToString();
                if (VRTK_SharedMethods.GetDictionaryValue(originalMaterialPropertyBlocks, faderRoutineID) == null)
                {
                    continue;
                }

                Coroutine existingFaderRoutine = VRTK_SharedMethods.GetDictionaryValue(faderRoutines, faderRoutineID);
                if (existingFaderRoutine != null)
                {
                    StopCoroutine(existingFaderRoutine);
                    faderRoutines.Remove(faderRoutineID);
                }

                MaterialPropertyBlock highlightMaterialPropertyBlock = highlightMaterialPropertyBlocks[faderRoutineID];
                renderer.GetPropertyBlock(highlightMaterialPropertyBlock);
                if (resetMainTexture)
                {
                    highlightMaterialPropertyBlock.SetTexture("_MainTex", Texture2D.whiteTexture);
                }

                if (duration > 0f)
                {
                    VRTK_SharedMethods.AddDictionaryValue(faderRoutines, faderRoutineID, StartCoroutine(CycleColor(renderer, highlightMaterialPropertyBlock, color, duration)), true);
                }
                else
                {
                    highlightMaterialPropertyBlock.SetColor("_Color", color);
                    highlightMaterialPropertyBlock.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(color, emissionDarken));
                    renderer.SetPropertyBlock(highlightMaterialPropertyBlock);
                }
            }
        }

        protected virtual IEnumerator CycleColor(Renderer renderer, MaterialPropertyBlock highlightMaterialPropertyBlock, Color endColor, float duration)
        {
            float elapsedTime = 0f;
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
