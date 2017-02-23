namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections.Generic;
    using KeyClass = VRTK_Keyboard.KeyClass;
    using IKey = VRTK_Keyboard.IKey;
    using IKeyMeta = VRTK_Keyboard.IKeyMeta;
    using KeyMeta = VRTK_Keyboard.KeyMeta;

    /// <summary>
    /// This abstract class is the base for both key layout renderer implementations that render to a UI.Canvas
    /// </summary>
    public abstract class VRTK_BaseCanvasKeyLayoutRenderer : VRTK_BaseKeyLayoutRenderer
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
        /// <summary>
        /// An optional prefab to use as the UI.Button for keyset modifier keys
        /// </summary>
        /// <remarks>
        /// Must be or contain a UI.Button with a UI.Image
        /// 
        /// A number of properties will be overwritten or modified by the renderer:
        /// - The template's RectTransform
        /// - The UI.Button's OnClick
        /// - The UI.Image's sourceImage
        /// 
        /// Can be used to apply variable image styles to keyset modifier keys
        /// </remarks>
        [Tooltip("An optional prefab to use as the UI.Button for keyset modifier keys")]
        public GameObject modifierKeyTemplate;
        [Tooltip("Sprite set to apply to key images")]
        public VRTK_KeyLayoutSprites keyLayoutSprites;

        // These are used in place of the public properties so editing them will not change values in the editor
        protected GameObject _keyTemplate;
        protected GameObject _specialKeyTemplate;
        protected GameObject _modifierKeyTemplate;

        protected DrivenRectTransformTracker drivenTemplateRectTransforms = new DrivenRectTransformTracker();
        protected Dictionary<CanvasGroup, int> keysetGroups;

        /// <summary>
        /// Update canvas group on keyset objects to change which one is dispalyed
        /// </summary>
        protected override void UpdateActiveKeyset()
        {
            foreach (KeyValuePair<CanvasGroup, int> keysetGroup in keysetGroups)
            {
                bool isActive = keysetGroup.Value == currentKeyset;
                CanvasGroup group = keysetGroup.Key;
                group.alpha = isActive ? 1 : 0;
                group.interactable = isActive;
                group.blocksRaycasts = isActive;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    // Disable objects in editor so invisible keys are not selected instead of visible ones
                    group.gameObject.SetActive(isActive);
                }
#endif
            }
        }

        protected virtual void SetupTemplates()
        {
            drivenTemplateRectTransforms.Clear();

            _keyTemplate = _keyTemplate != null ? _keyTemplate : keyTemplate;
            _specialKeyTemplate = _specialKeyTemplate != null ? _specialKeyTemplate : specialKeyTemplate;
            _modifierKeyTemplate = _modifierKeyTemplate != null ? _modifierKeyTemplate : modifierKeyTemplate;

            if (_keyTemplate == null)
            {
                Debug.LogWarning("Canvas Key Renderer in " + name + " requires a keyTemplate prefab");
                // Create an empty canvas object so key positions are visible without a key template
                _keyTemplate = new GameObject("KeyboardKey", typeof(RectTransform));
                ProcessRuntimeObject(_keyTemplate, hide: true);
                _keyTemplate.transform.SetParent(gameObject.transform, false);
            }
            else if (keyTemplate != null)
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
            else if (specialKeyTemplate != null)
            {
                Button keyTemplateButton = _specialKeyTemplate.GetComponentInChildren<Button>();
                if (keyTemplateButton == null)
                {
                    Debug.LogError("specialKeyTemplate prefab in " + name + " must contain a UI.Button");
                }
                else if (keyTemplateButton.GetComponentInChildren<Text>() == null)
                {
                    Debug.LogError(name + "'s specialKeyTemplate prefab's UI.Button must contain a UI.Text");
                }
            }

            if (_modifierKeyTemplate == null)
            {
                _modifierKeyTemplate = _specialKeyTemplate;
            }
            else if (modifierKeyTemplate != null)
            {
                Button keyTemplateButton = _modifierKeyTemplate.GetComponentInChildren<Button>();
                if (keyTemplateButton == null)
                {
                    Debug.LogError("modifierKeyTemplate prefab in " + name + " must contain a UI.Button");
                }
                else if (GetComponentExclusivelyInChildren<Image>(keyTemplateButton.gameObject) == null)
                {
                    Debug.LogError(name + "'s modifierKeyTemplate prefab's UI.Button must contain a UI.Image");
                    Debug.Log(_modifierKeyTemplate);
                    Debug.Log(modifierKeyTemplate);
                }
            }
        }

        protected virtual void CleanupTemplates()
        {
            // Cleanup dynamically generated key templates
            if (_keyTemplate != null && keyTemplate == null)
            {
                DestroyImmediate(_keyTemplate);
            }

            _keyTemplate = null;
            _specialKeyTemplate = null;
            _modifierKeyTemplate = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CleanupTemplates();
        }

        /// <summary>
        /// Apply a key's text, sprites, and click handlers to a UI.Button
        /// </summary>
        /// <param name="keyset">The keyset the key belongs to</param>
        /// <param name="key">The key</param>
        /// <param name="uiKey">The UI.Button GameObject</param>
        public void ApplyKeySettingsToUIKey(int keyset, IKey key, GameObject uiKey)
        {
            IKeyMeta keyMeta = KeyMeta.FromKey(key);

            uiKey.name = keyMeta.GetName();

            Button uiKeyButton = uiKey.GetComponentInChildren<Button>();
            if (uiKeyButton != null)
            {
                uiKeyButton.onClick.AddListener(GetKeypressHandler(key));

                Text uiKeyText = uiKeyButton.GetComponentInChildren<Text>();
                if (uiKeyText != null)
                {
                    uiKeyText.text = keyMeta.GetLabel();
                }

                Image uiKeyImage = GetComponentExclusivelyInChildren<Image>(uiKeyButton.gameObject);
                if (uiKeyImage != null)
                {
                    Sprite sprite = GetSpriteForKey(keyset, key);
                    if (sprite != null)
                    {
                        uiKeyImage.sprite = sprite;
                    }
                }
            }
        }

        /// <summary>
        /// Return the most specific key template for a renderable key
        /// </summary>
        /// <param name="key">The renderable key</param>
        /// <returns>The template to use</returns>
        protected GameObject GetTemplateForKey(IKey key)
        {
            if (key.GetKeyClass() == KeyClass.KeysetModifier)
            {
                return _modifierKeyTemplate;
            }

            if (key.GetKeyClass() != KeyClass.Character)
            {
                return _specialKeyTemplate;
            }

            return _keyTemplate;
        }

        /// <summary>
        /// Return the Sprite to use for a key if one is set
        /// </summary>
        /// <param name="keyset">The keyset the key belongs to</param>
        /// <param name="key">The key</param>
        /// <returns>A Sprite instance to use, if one is associated with the key</returns>
        protected Sprite GetSpriteForKey(int keyset, IKey key)
        {
            if (keyLayoutSprites != null)
            {
                return keyLayoutSprites.GetSpriteForKey(keyset, key);
            }

            return null;
        }

        /// <summary>
        /// Returns the component of Type type in the GameObject's children (excluding the GameObject itself) using depth first search.
        /// </summary>
        /// <remarks>
        /// A component is returned only if it is found on an active GameObject.
        /// </remarks>
        /// <typeparam name="T">The type of Component to retrieve.</typeparam>
        /// <param name="parent">The game object to search in</param>
        /// <returns> A component of the matching type, if found.</returns>
        protected static T GetComponentExclusivelyInChildren<T>(GameObject parent) where T : Component
        {
            foreach (T component in parent.GetComponentsInChildren<T>())
            {
                if (component.gameObject == parent)
                {
                    continue;
                }

                return component;
            }

            return null;
        }
    }
}
