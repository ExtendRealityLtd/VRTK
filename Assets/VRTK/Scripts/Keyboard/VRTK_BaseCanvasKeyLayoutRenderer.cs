namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using KeyClass = VRTK_Keyboard.KeyClass;
    using IKey = VRTK_Keyboard.IKey;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// This abstract class is the base for both key layout renderer implementations that render to a UI.Canvas
    /// </summary>
    public abstract class VRTK_BaseCanvasKeyLayoutRenderer : VRTK_BaseKeyLayoutRenderer
    {
        [Serializable]
        public class KeysetModifierImage
        {
            [Tooltip("Only apply inside this keyset")]
            public int inKeyset = -1;
            [Tooltip("Apply to this modifier key")]
            public int keyset = -1;
            [Tooltip("Image to apply to the keyset modifier key's UI.Image")]
            public Sprite sourceImage;
        }

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
        [Tooltip("Rule based overrides for keyset modifier key images")]
        public KeysetModifierImage[] keysetModifierImages;

        // These are used in place of the public properties so editing them will not change values in the editor
        protected GameObject _keyTemplate;
        protected GameObject _specialKeyTemplate;
        protected GameObject _modifierKeyTemplate;

        protected DrivenRectTransformTracker drivenTemplateRectTransforms = new DrivenRectTransformTracker();

        protected virtual void SetupTemplates()
        {
            drivenTemplateRectTransforms.Clear();

            _keyTemplate = keyTemplate;
            _specialKeyTemplate = specialKeyTemplate;
            _modifierKeyTemplate = modifierKeyTemplate;

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
            else
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
            else
            {
                Button keyTemplateButton = _modifierKeyTemplate.GetComponentInChildren<Button>();
                if (keyTemplateButton == null)
                {
                    Debug.LogError("modifierKeyTemplate prefab in " + name + " must contain a UI.Button");
                }
                else if (GetComponentExclusivelyInChildren<Image>(keyTemplateButton.gameObject) == null)
                {
                    Debug.LogError(name + "'s modifierKeyTemplate prefab's UI.Button must contain a UI.Image");
                }
            }
        }

        /// <summary>
        /// Apply a key's text, sprites, and click handlers to a UI.Button
        /// </summary>
        /// <param name="keyset">The keyset the key belongs to</param>
        /// <param name="key">The key</param>
        /// <param name="uiKey">The UI.Button GameObject</param>
        public void ApplyKeySettingsToUIKey(int keyset, IKey key, GameObject uiKey)
        {
            RKey rKey = key as RKey;

            Button uiKeyButton = uiKey.GetComponentInChildren<Button>();
            if (uiKeyButton != null)
            {
                uiKeyButton.onClick.AddListener(GetKeypressHandler(key));

                Text uiKeyText = uiKeyButton.GetComponentInChildren<Text>();
                if (uiKeyText != null)
                {
                    if (rKey != null)
                    {
                        // FIXME: Should not only be available to rendered keys
                        uiKeyText.text = rKey.label;
                    }
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
            if (key.GetKeyClass() == KeyClass.KeysetModifier)
            {
                foreach (KeysetModifierImage kmi in keysetModifierImages)
                {
                    if (kmi.inKeyset != -1 && kmi.inKeyset != keyset)
                    {
                        continue;
                    }

                    if (kmi.keyset != -1 && kmi.keyset != key.GetKeyset())
                    {
                        continue;
                    }

                    return kmi.sourceImage;
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
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
