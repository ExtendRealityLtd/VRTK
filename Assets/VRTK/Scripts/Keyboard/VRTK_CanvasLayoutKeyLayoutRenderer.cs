// Canvas Layout Key Layout Renderer|Keyboard|81022
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using KeyboardLayout = VRTK_KeyboardLayout;
    using KLKeyset = VRTK_KeyboardLayout.Keyset;
    using KLRow = VRTK_KeyboardLayout.Row;
    using KLKey = VRTK_KeyboardLayout.Key;

    /// <summary>
    /// The CanvasLayout key layout renderer script renders a functional keyboard row-by-row to a UI.Canvas using canvas layout components for styling
    /// </summary>
    /// <remarks>
    /// This CanvasLayout renderer renders to a UI.Canvas using horizontal and vertical layout groups and layout elements instead of
    /// a Key Layout Calculator. This makes the calculation implementation simpler and makes customizing sizes and spacing simpler
    /// however the CanvasLayout requires the use of rows and makes some styles of keyboard layout impossible to implement.
    /// 
    /// A key layout source component is required on the same gameobject as the key layout renderer.
    /// </remarks>
    [ExecuteInEditMode]
    [VRTK_KeyLayoutRenderer(name = "Canvas Layout Renderer", help = "Renders to a UI.Canvas using simple Canvas Layout elements", requireSource = true)]
    public class VRTK_CanvasLayoutKeyLayoutRenderer : VRTK_BaseCanvasKeyLayoutRenderer
    {
        [Tooltip("VerticalLayoutGroup template used by keysets to lay out rows")]
        public VerticalLayoutGroup keysetLayoutTemplate;
        [Tooltip("HorizontalLayoutGroup template used by rows to lay out keys")]
        public HorizontalLayoutGroup rowLayoutTemplate;
        [Tooltip("LayoutElement template used by keys")]
        public LayoutElement keyLayoutTemplate;

        protected DrivenRectTransformTracker drivenRuntimeRectTransforms = new DrivenRectTransformTracker();

        public override void SetupKeyboardUI()
        {
            // TODO: Better method of finding canvas(es)
            drivenRuntimeRectTransforms.Clear();
            GameObject root = GetRuntimeObjectContainer(gameObject, empty: true);
            drivenRuntimeRectTransforms.Add(this, root.GetComponent<RectTransform>(), DrivenTransformProperties.All);

            KeyboardLayout layout = GetKeyLayout();
            if (layout == null)
            {
                return;
            }

            SetupTemplates();

            for (int s = 0; s < layout.keysets.Length; s++)
            {
                // Keyset
                KLKeyset keyset = layout.keysets[s];
                GameObject uiKeyset = Instantiate<GameObject>(keysetLayoutTemplate.gameObject, root.transform, false);
                uiKeyset.name = keyset.name;
                ProcessRuntimeObject(uiKeyset);
                uiKeyset.SetActive(s == 0);
                RectTransform keysetTransform = uiKeyset.GetComponent<RectTransform>();
                keysetTransform.pivot = new Vector2(0.5f, 0.5f);
                keysetTransform.anchorMin = new Vector2(0, 0);
                keysetTransform.anchorMax = new Vector2(1, 1);
                keysetTransform.offsetMin = new Vector2(0, 0);
                keysetTransform.offsetMax = new Vector2(0, 0);
                drivenRuntimeRectTransforms.Add(this, keysetTransform, DrivenTransformProperties.All);

                foreach (KLRow row in keyset.rows)
                {
                    // Row
                    GameObject uiRow = Instantiate<GameObject>(rowLayoutTemplate.gameObject, keysetTransform, false);
                    uiRow.name = "KeyboardRow";
                    ProcessRuntimeObject(uiRow);
                    RectTransform rowTransform = uiRow.GetComponent<RectTransform>();
                    drivenRuntimeRectTransforms.Add(this, rowTransform,
                        DrivenTransformProperties.AnchoredPositionZ |
                        DrivenTransformProperties.Pivot |
                        DrivenTransformProperties.Rotation |
                        DrivenTransformProperties.Scale);

                    foreach (KLKey key in row.keys)
                    {
                        // Key
                        GameObject template = GetTemplateForKey(key);
                        GameObject uiKey = Instantiate<GameObject>(template);
                        ProcessRuntimeObject(uiKey);
                        RectTransform keyTransform = uiKey.GetComponent<RectTransform>();
                        keyTransform.SetParent(rowTransform, false);
                        ApplyKeySettingsToUIKey(s, key, uiKey);
                        LayoutElement keyLayoutElement = uiKey.AddComponent<LayoutElement>();
                        keyLayoutElement.ignoreLayout = keyLayoutTemplate.ignoreLayout;
                        keyLayoutElement.minWidth = keyLayoutTemplate.minWidth;
                        keyLayoutElement.minHeight = keyLayoutTemplate.minHeight;
                        keyLayoutElement.preferredWidth = keyLayoutTemplate.preferredWidth;
                        keyLayoutElement.preferredHeight = keyLayoutTemplate.preferredHeight;
                        keyLayoutElement.flexibleWidth = keyLayoutTemplate.flexibleWidth;
                        keyLayoutElement.flexibleHeight = keyLayoutTemplate.flexibleHeight;
                        drivenRuntimeRectTransforms.Add(template, keyTransform,
                            DrivenTransformProperties.AnchoredPositionZ |
                            DrivenTransformProperties.Pivot |
                            DrivenTransformProperties.Rotation |
                            DrivenTransformProperties.Scale);
                    }
                }
            }
        }

        protected override void SetupTemplates()
        {
            base.SetupTemplates();

            if (keysetLayoutTemplate == null)
            {
                GameObject keysetTemplate = new GameObject("KeysetLayoutTemplate", typeof(RectTransform));
                RectTransform keysetTransform = keysetTemplate.GetComponent<RectTransform>();
                keysetTransform.pivot = Vector2.one * 0.5f;
                keysetTransform.anchorMin = new Vector2(0, 0);
                keysetTransform.anchorMax = new Vector2(1, 1);
                keysetTransform.offsetMin = new Vector2(0, 0);
                keysetTransform.offsetMax = new Vector2(0, 0);
                keysetTemplate.transform.SetParent(gameObject.transform, false);
                keysetLayoutTemplate = keysetTemplate.AddComponent<VerticalLayoutGroup>();
                keysetLayoutTemplate.childControlHeight = true;
                keysetLayoutTemplate.childControlWidth = true;
                keysetLayoutTemplate.childForceExpandWidth = true;
                keysetLayoutTemplate.childForceExpandHeight = true;
            }

            if (rowLayoutTemplate == null)
            {
                GameObject rowTemplate = new GameObject("RowLayoutTemplate", typeof(RectTransform));
                rowTemplate.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
                rowTemplate.transform.SetParent(gameObject.transform, false);
                rowLayoutTemplate = rowTemplate.AddComponent<HorizontalLayoutGroup>();
                rowLayoutTemplate.childControlHeight = true;
                rowLayoutTemplate.childControlWidth = true;
                rowLayoutTemplate.childForceExpandWidth = false;
                rowLayoutTemplate.childForceExpandHeight = false;
            }

            if (keyLayoutTemplate == null)
            {
                GameObject keyTemplate = new GameObject("KeyLayoutTemplate", typeof(RectTransform));
                keyTemplate.GetComponent<RectTransform>().pivot = Vector2.one * 0.5f;
                keyTemplate.transform.SetParent(gameObject.transform, false);
                keyLayoutTemplate = keyTemplate.AddComponent<LayoutElement>();
                keyLayoutTemplate.flexibleWidth = 1f;
                keyLayoutTemplate.flexibleHeight = 1f;
            }

            // Register drivers for properties in template rect transforms
            {
                // All keyset transform properties are set by renderer
                RectTransform keysetTransform = keysetLayoutTemplate.gameObject.GetComponent<RectTransform>();
                drivenTemplateRectTransforms.Add(this, keysetTransform, DrivenTransformProperties.All);

                // Row transform is controlled by the keyset template's VerticalLayoutGroup
                RectTransform rowTransform = rowLayoutTemplate.gameObject.GetComponent<RectTransform>();
                drivenTemplateRectTransforms.Add(keysetLayoutTemplate.GetComponent<VerticalLayoutGroup>(), rowTransform,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);

                // Key transform is controlled by the row template's HorizontalLayoutGroup and the keyTemplate prefab
                RectTransform keyTransform = keyLayoutTemplate.gameObject.GetComponent<RectTransform>();
                drivenRuntimeRectTransforms.Add(this, keyTransform,
                    DrivenTransformProperties.AnchoredPositionZ |
                    DrivenTransformProperties.Pivot |
                    DrivenTransformProperties.Rotation |
                    DrivenTransformProperties.Scale);
                drivenTemplateRectTransforms.Add(rowLayoutTemplate.GetComponent<HorizontalLayoutGroup>(), keyTransform,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);
            }
        }
    }
}