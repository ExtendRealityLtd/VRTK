namespace VRTK.Highlighters
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class VRTK_MaterialColorSwapHighlighter : VRTK_Highlighter
    {
        private Dictionary<string, Color[]> originalObjectColours;
        private Dictionary<string, Coroutine> faderRoutines;
        private bool resetMainTexture = false;
        private Texture originalMainTexture;

        public override void Initialise(Color? color = null)
        {
            faderRoutines = new Dictionary<string, Coroutine>();
        }

        public override void ResetMainTexture()
        {
            resetMainTexture = true;
        }

        public override void Highlight(Color? color, float duration = 0f)
        {
            if (color == null)
            {
                return;
            }

            if (originalObjectColours == null)
            {
                originalObjectColours = StoreOriginalColors();
            }

            var colorArray = BuildHighlightColorArray((Color)color);
            ChangeColor(colorArray, duration);
        }

        public override void Unhighlight(Color? color = null, float duration = 0f)
        {
            if (originalObjectColours == null)
            {
                return;
            }

            ChangeColor(originalObjectColours, duration, true);
        }

        private Dictionary<string, Color[]> StoreOriginalColors()
        {
            var colors = new Dictionary<string, Color[]>();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                colors[objectReference] = new Color[renderer.materials.Length];

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];
                    if (resetMainTexture && material.HasProperty("_MainTex"))
                    {
                        originalMainTexture = material.mainTexture;
                    }

                    if (material.HasProperty("_Color"))
                    {
                        colors[objectReference][i] = material.color;
                    }
                }
            }
            return colors;
        }

        private Dictionary<string, Color[]> BuildHighlightColorArray(Color color)
        {
            var colors = new Dictionary<string, Color[]>();
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                colors[objectReference] = new Color[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];

                    if (resetMainTexture && material.HasProperty("_MainTex"))
                    {
                        renderer.material.SetTexture("_MainTex", new Texture());
                    }

                    if (material.HasProperty("_Color"))
                    {
                        colors[objectReference][i] = color;
                    }
                }
            }
            return colors;
        }

        private void ChangeColor(Dictionary<string, Color[]> colors, float duration = 0f, bool unhighlight = false)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                var objectReference = renderer.gameObject.GetInstanceID().ToString();
                if (!colors.ContainsKey(objectReference))
                {
                    continue;
                }

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];
                    var faderRoutineID = material.GetInstanceID().ToString();

                    if (faderRoutines.ContainsKey(faderRoutineID) && faderRoutines[faderRoutineID] != null)
                    {
                        StopCoroutine(faderRoutines[faderRoutineID]);
                        faderRoutines.Remove(faderRoutineID);
                    }

                    if (unhighlight && resetMainTexture && originalMainTexture != null && material.HasProperty("_MainTex"))
                    {
                        material.mainTexture = originalMainTexture;
                    }

                    if (material.HasProperty("_Color"))
                    {
                        if (duration > 0f)
                        {
                            faderRoutines[faderRoutineID] = StartCoroutine(CycleColor(material, material.color, colors[objectReference][i], duration));
                        }
                        else
                        {
                            material.color = colors[objectReference][i];
                        }
                    }
                }
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
    }
}