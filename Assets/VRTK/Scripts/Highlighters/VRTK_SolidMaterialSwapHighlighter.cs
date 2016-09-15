namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections.Generic;

    public class VRTK_SolidMaterialSwapHighlighter : VRTK_Highlighter
    {
        private Dictionary<string, Material[]> originalRendererMaterials;
        private Dictionary<string, Material[]> originalSharedRendererMaterials;
        private Dictionary<string, Material[]> highlightRendererMaterials;
        private Color? cachedHighlightColor = null;

        public override void Initialise(Color? color = null)
        {
            if (color != cachedHighlightColor)
            {
                originalRendererMaterials = new Dictionary<string, Material[]>();
                originalSharedRendererMaterials = new Dictionary<string, Material[]>();
                highlightRendererMaterials = new Dictionary<string, Material[]>();

                SetupCloneMaterials((Color)color);
                cachedHighlightColor = color;
            }
        }

        public override void ResetMainTexture()
        {
        }

        public override void Highlight(Color? color = null, float duration = 0f)
        {
            Initialise(color);
            SwapToHighlightMaterial();
        }

        public override void Unhighlight(Color? color = null, float duration = 0f)
        {
            ResetMaterials();
        }

        private void OnDestroy()
        {
            Unhighlight();
            highlightRendererMaterials = null;
        }

        private void SetupCloneMaterials(Color color)
        {
            originalRendererMaterials.Clear();
            originalSharedRendererMaterials.Clear();
            foreach (var tmpRenderer in GetComponentsInChildren<Renderer>())
            {
                var dictKey = tmpRenderer.GetInstanceID().ToString();
                originalSharedRendererMaterials[dictKey] = tmpRenderer.sharedMaterials;
                originalRendererMaterials[dictKey] = tmpRenderer.materials;
                BuildHighlightMaterial(tmpRenderer, color);
            }
        }

        private void BuildHighlightMaterial(Renderer tmpRenderer, Color color)
        {
            var highlightMaterials = new Material[tmpRenderer.materials.Length];
            for (int i = 0; i < tmpRenderer.materials.Length; i++)
            {
                var originalMaterial = tmpRenderer.materials[i];
                var tmpMaterial = Instantiate(originalMaterial);
                tmpMaterial.name = originalMaterial.name + "_HighlightMaterial";

                if (tmpMaterial.HasProperty("_Mode"))
                {
                    tmpMaterial.SetFloat("_Mode", 3);
                }

                if (tmpMaterial.HasProperty("_Color"))
                {
                    tmpMaterial.color = color;
                }

                tmpMaterial.EnableKeyword("_EMISSION");
                if (tmpMaterial.HasProperty("_EmissionColor"))
                {
                    tmpMaterial.SetColor("_EmissionColor", Darken(color, 20f));
                }
                highlightMaterials[i] = tmpMaterial;
            }

            var dictKey = tmpRenderer.GetInstanceID().ToString();
            highlightRendererMaterials[dictKey] = highlightMaterials;
            tmpRenderer.sharedMaterials = originalSharedRendererMaterials[dictKey];
        }

        private void ResetMaterials()
        {
            foreach (var tmpRenderer in GetComponentsInChildren<Renderer>())
            {
                var dictKey = tmpRenderer.GetInstanceID().ToString();
                if (originalRendererMaterials.ContainsKey(dictKey))
                {
                    tmpRenderer.materials = originalRendererMaterials[dictKey];
                    tmpRenderer.sharedMaterials = originalSharedRendererMaterials[dictKey];
                }
            }
        }

        private void SwapToHighlightMaterial()
        {
            foreach (var renderers in GetComponentsInChildren<Renderer>())
            {
                var dictKey = renderers.GetInstanceID().ToString();
                if (highlightRendererMaterials.ContainsKey(dictKey))
                {
                    renderers.materials = highlightRendererMaterials[dictKey];
                }
            }
        }

        private Color Darken(Color color, float percent)
        {
            return new Color(ColorPercent(color.r, percent), ColorPercent(color.g, percent), ColorPercent(color.b, percent), color.a);
        }

        private float ColorPercent(float value, float percent)
        {
            percent = Mathf.Clamp(percent, 0f, 100f);
            return (percent == 0f ? value : (value - (percent / 100f)));
        }
    }
}