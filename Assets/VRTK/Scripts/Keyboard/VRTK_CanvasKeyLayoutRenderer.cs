// Canvas Key Layout Renderer|Keyboard|81021
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using KeyClass = VRTK_Keyboard.KeyClass;
    using RKeyLayout = VRTK_RenderableKeyLayout;
    using RKeyset = VRTK_RenderableKeyLayout.Keyset;
    using RKeyArea = VRTK_RenderableKeyLayout.KeyArea;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// The canvas key layout renderer script renders a functional keyboard to a UI.Canvas
    /// </summary>
    /// <remarks>
    /// A key layout calculator component is required on the same gameobject as the key layout renderer.
    /// </remarks>
    [ExecuteInEditMode]
    [VRTK_KeyLayoutRenderer(name = "Canvas Renderer", help = "Renders to a UI.Canvas using a calculated keyboard layout", requireCalculator = true)]
    public class VRTK_CanvasKeyLayoutRenderer : VRTK_BaseCanvasKeyLayoutRenderer
    {

        /// <summary>
        /// The SetupKeyboardUI method resets the canvas's children and creates
        /// the canvas objects that make up a rendered keyboard.
        /// </summary>
        public override void SetupKeyboardUI()
        {
            // TODO: Better method of finding canvas(es)
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyRuntimeObject(transform.GetChild(i).gameObject);
            }

            Rect containerRect = gameObject.GetComponent<RectTransform>().rect;

            RKeyLayout layout = CalculateRenderableKeyLayout(containerRect.size);
            if (layout == null)
            {
                return;
            }

            SetupTemplates();

            Vector2 areaPivot = Vector2.one * 0.5f;
            Vector2 keyPivot = Vector2.one * 0.5f;

            for (int s = 0; s < layout.keysets.Length; s++)
            {
                // Keyset
                RKeyset rKeyset = layout.keysets[s];
                GameObject uiKeyset = new GameObject(rKeyset.name, typeof(RectTransform));
                ProcessRuntimeObject(uiKeyset);
                uiKeyset.SetActive(s == 0);
                RectTransform keysetTransform = uiKeyset.GetComponent<RectTransform>();
                keysetTransform.SetParent(gameObject.transform, false);
                keysetTransform.pivot = new Vector2(0.5f, 0.5f);
                keysetTransform.anchorMin = new Vector2(0, 0);
                keysetTransform.anchorMax = new Vector2(1, 1);
                keysetTransform.offsetMin = new Vector2(0, 0);
                keysetTransform.offsetMax = new Vector2(0, 0);

                foreach (RKeyArea rKeyArea in rKeyset.areas)
                {
                    // Area
                    GameObject uiArea = new GameObject(rKeyArea.name, typeof(RectTransform));
                    ProcessRuntimeObject(uiArea);
                    RectTransform rowTransform = uiArea.GetComponent<RectTransform>();
                    rowTransform.SetParent(keysetTransform, false);
                    rowTransform.pivot = areaPivot;
                    ApplyRectLayoutToRectTransform(rKeyArea.rect, rowTransform, containerRect.size);

                    foreach (RKey rKey in rKeyArea.keys)
                    {
                        // Key
                        GameObject template = GetTemplateForKey(rKey);
                        GameObject uiKey = Instantiate<GameObject>(template);
                        uiKey.name = rKey.name;
                        ProcessRuntimeObject(uiKey);
                        RectTransform keyTransform = uiKey.GetComponent<RectTransform>();
                        keyTransform.SetParent(rowTransform, false);
                        keyTransform.pivot = keyPivot;
                        ApplyRectLayoutToRectTransform(rKey.rect, keyTransform, rKeyArea.rect.size);
                        ApplyKeySettingsToUIKey(s, rKey, uiKey);
                    }
                }
            }
        }

        /// <summary>
        /// Apply a layout Rect to a RectTransform
        /// </summary>
        /// <remarks>
        /// This method accepts a Rect in (x,y=right,down) coordinate space, coverts it to RectTransform's
        /// (x,y=right,up) coordinate space, and calculates the necessary anchors and offsets required.
        /// </remarks>
        /// <param name="layout">The (x,y=right,down) to apply</param>
        /// <param name="transform">The RectTransform to modify</param>
        /// <param name="containerSize">The dimensions of the container for reference</param>
        protected void ApplyRectLayoutToRectTransform(Rect layout, RectTransform transform, Vector2 containerSize)
        {
            // Convert keyboard rect coordinates (x,y=right,down) to RectTransform rect coordinates (x,y=right,up)
            Rect rRect = new Rect(layout);
            rRect.position = new Vector2(rRect.position.x, containerSize.y - rRect.size.y - rRect.position.y);

            // Anchor set to lower left
            transform.anchorMin = transform.anchorMax = Vector2.zero;
            // lower left corner is offset by the rect position
            transform.offsetMin = rRect.position;
            // upper right corner is offset by the size and position
            transform.offsetMax = rRect.size + rRect.position;
        }
    }
}
