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
        /// <summary>
        /// A serializable class containing settings to apply to the keysets' UI.VerticalLayoutGroup
        /// </summary>
        [Serializable]
        public class KeysetLayoutGroupSettings
        {
            public RectOffset padding;
            public float spacing;
            public TextAnchor childAlignment;
            public bool childControlWidth = true;
            public bool childControlHeight = true;
            public bool childForceExpandWidth = true;
            public bool childForceExpandHeight = true;

            public void Apply(VerticalLayoutGroup layoutGroup)
            {
                layoutGroup.padding = padding;
                layoutGroup.spacing = spacing;
                layoutGroup.childAlignment = childAlignment;
                layoutGroup.childControlWidth = childControlWidth;
                layoutGroup.childControlHeight = childControlHeight;
                layoutGroup.childForceExpandWidth = childForceExpandWidth;
                layoutGroup.childForceExpandHeight = childForceExpandHeight;
            }
        }
        /// <summary>
        /// A serializable class containing settings to apply to the rows' UI.HorizontalLayoutGroup
        /// </summary>
        [Serializable]
        public class RowLayoutGroupSettings
        {
            public RectOffset padding;
            public float spacing;
            public TextAnchor childAlignment;
            public bool childControlWidth = true;
            public bool childControlHeight = true;
            public bool childForceExpandWidth = true;
            public bool childForceExpandHeight = true;

            public void Apply(HorizontalLayoutGroup layoutGroup)
            {
                layoutGroup.padding = padding;
                layoutGroup.spacing = spacing;
                layoutGroup.childAlignment = childAlignment;
                layoutGroup.childControlWidth = childControlWidth;
                layoutGroup.childControlHeight = childControlHeight;
                layoutGroup.childForceExpandWidth = childForceExpandWidth;
                layoutGroup.childForceExpandHeight = childForceExpandHeight;
            }
        }
        /// <summary>
        /// A serializable class containing settings to apply to the keys' UI.LayoutElement
        /// </summary>
        [Serializable]
        public class KeyLayoutElementSettings
        {
            public float minWidth;
            public float minHeight;
            public float preferredWidth;
            public float preferredHeight;
            public float flexibleWidth = 1f;
            public float flexibleHeight = 1f;

            public void Apply(LayoutElement element)
            {
                element.minWidth = minWidth;
                element.minHeight = minHeight;
                element.preferredWidth = preferredWidth;
                element.preferredHeight = preferredHeight;
                element.flexibleWidth = flexibleWidth;
                element.flexibleHeight = flexibleHeight;
            }
        }

        [Tooltip("VerticalGroupLayout settings used by keysets to lay out rows")]
        public KeysetLayoutGroupSettings keysetLayoutSettings;
        [Tooltip("HorizontalGroupLayout settings used by rows to lay out keys")]
        public RowLayoutGroupSettings rowLayoutSettings;
        [Tooltip("LayoutElement settings used by keys")]
        public KeyLayoutElementSettings keyLayoutSettings;

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

            Vector2 rowPivot = Vector2.one * 0.5f;
            Vector2 keyPivot = Vector2.one * 0.5f;

            for (int s = 0; s < layout.keysets.Length; s++)
            {
                // Keyset
                KLKeyset keyset = layout.keysets[s];
                GameObject uiKeyset = new GameObject(keyset.name, typeof(RectTransform));
                ProcessRuntimeObject(uiKeyset);
                uiKeyset.SetActive(s == 0);
                RectTransform keysetTransform = uiKeyset.GetComponent<RectTransform>();
                keysetTransform.SetParent(root.transform, false);
                keysetTransform.pivot = new Vector2(0.5f, 0.5f);
                keysetTransform.anchorMin = new Vector2(0, 0);
                keysetTransform.anchorMax = new Vector2(1, 1);
                keysetTransform.offsetMin = new Vector2(0, 0);
                keysetTransform.offsetMax = new Vector2(0, 0);
                VerticalLayoutGroup keysetLayoutGroup = uiKeyset.AddComponent<VerticalLayoutGroup>();
                keysetLayoutSettings.Apply(keysetLayoutGroup);

                foreach (KLRow row in keyset.rows)
                {
                    // Area
                    GameObject uiRow = new GameObject("KeyboardRow", typeof(RectTransform));
                    ProcessRuntimeObject(uiRow);
                    RectTransform rowTransform = uiRow.GetComponent<RectTransform>();
                    rowTransform.SetParent(keysetTransform, false);
                    rowTransform.pivot = rowPivot;
                    HorizontalLayoutGroup rowLayoutGroup = uiRow.AddComponent<HorizontalLayoutGroup>();
                    rowLayoutSettings.Apply(rowLayoutGroup);

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
                        keyLayoutSettings.Apply(keyLayoutElement);
                    }
                }
            }
        }
    }
}