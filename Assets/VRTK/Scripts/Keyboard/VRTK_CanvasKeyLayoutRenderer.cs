// Canvas Key Layout Renderer|Keyboard|81021
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
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
        [Tooltip("Canvases to render to. Will use current game object when not set. If size > 1, then size must match the number of key areas created by the attached key layout renderer.")]
        public RectTransform[] canvases = new RectTransform[0];

        protected DrivenRectTransformTracker drivenRuntimeRectTransforms = new DrivenRectTransformTracker();

        /// <summary>
        /// The SetupKeyboardUI method resets the canvas's children and creates
        /// the canvas objects that make up a rendered keyboard.
        /// </summary>
        public override void SetupKeyboardUI()
        {
            drivenRuntimeRectTransforms.Clear();
            keysetObjects = null;
            GameObject[] roots = null;
            if (canvases != null && canvases.Length > 0)
            {
                roots = new GameObject[canvases.Length];
                for (int i = 0; i < canvases.Length; i++)
                {
                    if (canvases[i] == null)
                    {
                        roots = null;
                        break;
                    }
                    roots[i] = GetRuntimeObjectContainer(canvases[i].gameObject, empty: true);
                }
            }
            if (roots == null && transform is RectTransform)
            {
                roots = new GameObject[1];
                roots[0] = GetRuntimeObjectContainer(gameObject, empty: true);
            }
            if (roots == null)
            {
                Debug.LogWarning(name + " must be part of a UI.Canvas or have the canvases array set.");
                return;
            }

            foreach (GameObject root in roots)
            {
                drivenRuntimeRectTransforms.Add(this, root.GetComponent<RectTransform>(), DrivenTransformProperties.All);
            }

            Vector2[] containerSizes = Array.ConvertAll(roots,
                (root) => root.GetComponent<RectTransform>().rect.size);
            RKeyLayout layout = CalculateRenderableKeyLayout(containerSizes);
            if (layout == null)
            {
                return;
            }

            SetupTemplates();

            Vector2 areaPivot = Vector2.one * 0.5f;
            keysetObjects = new Dictionary<GameObject, int>();
            keysetGroups = new Dictionary<CanvasGroup, int>();

            for (int s = 0; s < layout.keysets.Length; s++)
            {
                // Keyset
                RKeyset rKeyset = layout.keysets[s];
                GameObject[] uiKeysets = new GameObject[roots.Length];
                for (int r = 0; r < roots.Length; r++)
                {
                    GameObject uiKeyset = new GameObject(rKeyset.name, typeof(RectTransform));
                    uiKeysets[r] = uiKeyset;
                    ProcessRuntimeObject(uiKeyset);
                    keysetObjects.Add(uiKeyset, s);
                    CanvasGroup keysetGroup = uiKeyset.AddComponent<CanvasGroup>();
                    keysetGroup.alpha = s == 0 ? 1 : 0;
                    keysetGroup.interactable = s == 0;
                    keysetGroup.blocksRaycasts = s == 0;
                    keysetGroups.Add(keysetGroup, s);
                    RectTransform keysetTransform = uiKeyset.GetComponent<RectTransform>();
                    keysetTransform.SetParent(roots[r].transform, false);
                    keysetTransform.pivot = new Vector2(0.5f, 0.5f);
                    keysetTransform.anchorMin = new Vector2(0, 0);
                    keysetTransform.anchorMax = new Vector2(1, 1);
                    keysetTransform.offsetMin = new Vector2(0, 0);
                    keysetTransform.offsetMax = new Vector2(0, 0);
                    drivenRuntimeRectTransforms.Add(this, keysetTransform, DrivenTransformProperties.All);
                }

                for (int a = 0; a < rKeyset.areas.Length; a++)
                {
                    // Area
                    RKeyArea rKeyArea = rKeyset.areas[a];
                    GameObject uiArea = new GameObject(rKeyArea.name, typeof(RectTransform));
                    ProcessRuntimeObject(uiArea);
                    RectTransform areaTransform = uiArea.GetComponent<RectTransform>();
                    areaTransform.SetParent(uiKeysets[rKeyArea.container].transform, false);
                    areaTransform.pivot = areaPivot;
                    ApplyRectLayoutToRectTransform(rKeyArea.rect, areaTransform, containerSizes[rKeyArea.container]);
                    drivenRuntimeRectTransforms.Add(GetComponent<VRTK_BaseKeyLayoutCalculator>(), areaTransform,
                        DrivenTransformProperties.AnchoredPosition3D |
                        DrivenTransformProperties.Anchors |
                        DrivenTransformProperties.Rotation |
                        DrivenTransformProperties.Scale |
                        DrivenTransformProperties.SizeDelta);
                    drivenRuntimeRectTransforms.Add(this, areaTransform, DrivenTransformProperties.Pivot);

                    foreach (RKey rKey in rKeyArea.keys)
                    {
                        // Key
                        GameObject template = GetTemplateForKey(rKey);
                        GameObject uiKey = Instantiate<GameObject>(template);
                        ProcessRuntimeObject(uiKey);
                        RectTransform keyTransform = uiKey.GetComponent<RectTransform>();
                        keyTransform.SetParent(areaTransform, false);
                        ApplyRectLayoutToRectTransform(rKey.rect, keyTransform, rKeyArea.rect.size);
                        ApplyKeySettingsToUIKey(s, rKey, uiKey);
                        drivenRuntimeRectTransforms.Add(keyTemplate, keyTransform,
                            DrivenTransformProperties.AnchoredPositionZ |
                            DrivenTransformProperties.Pivot |
                            DrivenTransformProperties.Rotation |
                            DrivenTransformProperties.Scale);
                        drivenRuntimeRectTransforms.Add(GetComponent<VRTK_BaseKeyLayoutCalculator>(), keyTransform,
                            DrivenTransformProperties.AnchoredPosition |
                            DrivenTransformProperties.Anchors |
                            DrivenTransformProperties.SizeDelta);
                    }
                }
            }

            UpdateActiveKeyset();
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
