// Canvas Layout Key Layout Renderer|Keyboard|81022
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
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

        public override void SetupKeyboardUI()
        {
            // TODO: Better method of finding canvas(es)
            GameObject root = GetRuntimeObjectContainer(gameObject, empty: true);

            KeyboardLayout layout = GetKeyLayout();
            if (layout == null)
            {
                return;
            }

            SetupTemplates();
            
            Vector2 keyPivot = Vector2.one * 0.5f;

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

                foreach (KLRow row in keyset.rows)
                {
                    // Area
                    GameObject uiRow = Instantiate<GameObject>(rowLayoutTemplate.gameObject, keysetTransform, false);
                    uiRow.name = "KeyboardRow";
                    ProcessRuntimeObject(uiRow);
                    RectTransform rowTransform = uiRow.GetComponent<RectTransform>();

                    foreach (KLKey key in row.keys)
                    {
                        // Key
                        GameObject template = GetTemplateForKey(key);
                        GameObject uiKey = Instantiate<GameObject>(template);
                        // uiKey.name = key.name;
                        ProcessRuntimeObject(uiKey);
                        RectTransform keyTransform = uiKey.GetComponent<RectTransform>();
                        keyTransform.SetParent(rowTransform, false);
                        keyTransform.pivot = keyPivot;
                        ApplyKeySettingsToUIKey(s, key, uiKey);
                        LayoutElement keyLayoutElement = uiKey.AddComponent<LayoutElement>();
                        keyLayoutElement.ignoreLayout = keyLayoutTemplate.ignoreLayout;
                        keyLayoutElement.minWidth = keyLayoutTemplate.minWidth;
                        keyLayoutElement.minHeight = keyLayoutTemplate.minHeight;
                        keyLayoutElement.preferredWidth = keyLayoutTemplate.preferredWidth;
                        keyLayoutElement.preferredHeight = keyLayoutTemplate.preferredHeight;
                        keyLayoutElement.flexibleWidth = keyLayoutTemplate.flexibleWidth;
                        keyLayoutElement.flexibleHeight = keyLayoutTemplate.flexibleHeight;
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
                keysetTemplate.GetComponent<RectTransform>().pivot = Vector2.one / 2f;
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
                rowTemplate.GetComponent<RectTransform>().pivot = Vector2.one / 2f;
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
                keyTemplate.GetComponent<RectTransform>().pivot = Vector2.one / 2f;
                keyTemplate.transform.SetParent(gameObject.transform, false);
                keyLayoutTemplate = keyTemplate.AddComponent<LayoutElement>();
                keyLayoutTemplate.flexibleWidth = 1f;
                keyLayoutTemplate.flexibleHeight = 1f;
            }
        }
    }
}