// Canvas Key Layout Renderer|Keyboard|81021
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;
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
    public class VRTK_CanvasKeyLayoutRenderer : VRTK_BaseKeyLayoutRenderer
    {
        /// <summary>
        /// The prefab to use as the UI.Button for keys
        /// </summary>
        /// <remarks>
        /// Must be or contain a UI.Button with a UI.Text
        /// 
        /// A number of properties will be overwritten or modified by the renderer:
        /// - The template's RectTransform
        /// - The UI.Button's OnClick
        /// - The UI.Text's text
        /// </remarks>
        [Tooltip("The prefab to use as the UI.Button for keys")]
        public GameObject keyTemplate;
        /// <summary>
        /// An optional prefab to use as the UI.Button for special keys
        /// </summary>
        /// <remarks>
        /// Works the same as `keyTemplate`.
        /// 
        /// Can be used to apply different styles for the special keys (Modifiers, Backspace, Enter, Done).
        /// </remarks>
        [Tooltip("An optional prefab to use as the UI.Button for special keys")]
        public GameObject specialKeyTemplate;

        // These are used in place of the public properties so editing them will not change values in the editor
        protected GameObject _keyTemplate;
        protected GameObject _specialKeyTemplate;

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

            _keyTemplate = keyTemplate;
            _specialKeyTemplate = specialKeyTemplate;

            if (_keyTemplate == null)
            {
                Debug.LogWarning("Canvas Key Renderer in " + name + " requires a keyTemplate prefab");
                // Create an empty canvas object so key positions are visible without a key template
                _keyTemplate = new GameObject("KeyboardKey", typeof(RectTransform));
                ProcessRuntimeObject(_keyTemplate);
                _keyTemplate.transform.SetParent(gameObject.transform, false);
            }
            else
            {
                Button keyTemplateButton = _keyTemplate.GetComponentInChildren<Button>();
                if (keyTemplateButton == null)
                {
                    Debug.LogError("keyTemplate prefab in " + name + " must contain a UI.Button");
                }
                else if (keyTemplateButton.GetComponentInChildren<Text>() == null)
                {
                    Debug.LogError(name + "'s keyTemplate prefab's UI.Button must contain a UI.Text");
                }
            }

            if (_specialKeyTemplate == null)
            {
                _specialKeyTemplate = _keyTemplate;
            }

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
                        GameObject uiKey = Instantiate<GameObject>(_keyTemplate);
                        uiKey.name = rKey.name;
                        ProcessRuntimeObject(uiKey);
                        RectTransform keyTransform = uiKey.GetComponent<RectTransform>();
                        keyTransform.SetParent(rowTransform, false);
                        keyTransform.pivot = keyPivot;
                        ApplyRectLayoutToRectTransform(rKey.rect, keyTransform, rKeyArea.rect.size);

                        Button uiKeyButton = uiKey.GetComponentInChildren<Button>();
                        if (uiKeyButton != null)
                        {
                            uiKeyButton.onClick.AddListener(GetKeypressHandler(rKey));

                            Text uiKeyText = uiKeyButton.GetComponentInChildren<Text>();
                            if (uiKeyText != null)
                            {
                                uiKeyText.text = rKey.label;
                            }
                        }
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
