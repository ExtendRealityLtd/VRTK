// Mesh Key Layout Renderer|Keyboard|81023
namespace VRTK
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using KeyClass = VRTK_Keyboard.KeyClass;
    using IKey = VRTK_Keyboard.IKey;
    using IKeyMeta = VRTK_Keyboard.IKeyMeta;
    using KeyMeta = VRTK_Keyboard.KeyMeta;
    using RKeyLayout = VRTK_RenderableKeyLayout;
    using RKeyset = VRTK_RenderableKeyLayout.Keyset;
    using RKeyArea = VRTK_RenderableKeyLayout.KeyArea;
    using RKey = VRTK_RenderableKeyLayout.Key;

    /// <summary>
    /// The mesh key layout renderer script renders a functional keyboard made out of 3d game objects
    /// </summary>
    /// <remarks>
    /// A key layout calculator component is required on the same gameobject as the key layout renderer.
    /// </remarks>
    [ExecuteInEditMode]
    [VRTK_KeyLayoutRenderer(name = "Mesh Renderer", help = "Renders 3d game objects using a calculated keyboard layout", requireCalculator = true)]
    public class VRTK_MeshKeyLayoutRenderer : VRTK_BaseKeyLayoutRenderer
    {
        [Tooltip("GameObject to render to. Will use current game object when not set.")]
        public GameObject target;

        [Tooltip("Dimensions of the area to fit the keyboard into")]
        public Vector2 size = Vector2.one;

        /// <summary>
        /// The prefab to use for keys
        /// </summary>
        [Tooltip("The prefab to use for keys")]
        public GameObject keyTemplate;
        /// <summary>
        /// An optional prefab to use for special keys
        /// </summary>
        /// <remarks>
        /// Works the same as `keyTemplate`.
        /// 
        /// Can be used to apply different styles for the special keys (Modifiers, Backspace, Enter, Done).
        /// </remarks>
        [Tooltip("An optional prefab to use for special keys")]
        public GameObject specialKeyTemplate;
        /// <summary>
        /// An optional prefab to use for keyset modifier keys
        /// </summary>
        /// <remarks>
        /// Can be used to apply variable image styles to keyset modifier keys
        /// </remarks>
        [Tooltip("An optional prefab to use for keyset modifier keys")]
        public GameObject modifierKeyTemplate;
        [Tooltip("Sprite set to apply to key images")]
        public VRTK_KeyLayoutSprites keyLayoutSprites;

        // These are used in place of the public properties so editing them will not change values in the editor
        protected GameObject _keyTemplate;
        protected GameObject _specialKeyTemplate;
        protected GameObject _modifierKeyTemplate;

        /// <summary>
        /// The SetupKeyboardUI method resets the target's children and creates
        /// the game objects that make up a rendered keyboard.
        /// </summary>
        public override void SetupKeyboardUI()
        {
            GameObject _target = target != null ? target : gameObject;
            keysetObjects = null;
            GameObject root = GetRuntimeObjectContainer(_target, empty: true);

            Vector2[] containerSizes = new Vector2[] { size };
            RKeyLayout layout = CalculateRenderableKeyLayout(containerSizes);
            if (layout == null)
            {
                return;
            }

            SetupTemplates();

            keysetObjects = new Dictionary<GameObject, int>();

            for (int s = 0; s < layout.keysets.Length; s++)
            {
                // Keyset
                RKeyset rKeyset = layout.keysets[s];
                GameObject gKeyset = new GameObject(rKeyset.name);
                ProcessRuntimeObject(gKeyset);
                keysetObjects.Add(gKeyset, s);
                gKeyset.transform.SetParent(root.transform, false);

                for (int a = 0; a < rKeyset.areas.Length; a++)
                {
                    // Area
                    RKeyArea rKeyArea = rKeyset.areas[a];
                    GameObject gArea = new GameObject(rKeyArea.name);
                    ProcessRuntimeObject(gArea);
                    gArea.transform.SetParent(gKeyset.transform, false);
                    ApplyRectLayoutToTransform(rKeyArea.rect, gArea.transform, size);

                    foreach (RKey rKey in rKeyArea.keys)
                    {
                        // Key
                        GameObject template = GetTemplateForKey(rKey);
                        GameObject gKey = Instantiate<GameObject>(template);
                        ProcessRuntimeObject(gKey);
                        gKey.transform.SetParent(gArea.transform, false);
                        ApplyRectLayoutToTransform(rKey.rect, gKey.transform, rKeyArea.rect.size);
                        ApplyKeySettingsToKey(s, rKey, gKey);
                    }
                }
            }

            UpdateActiveKeyset();
        }

        protected virtual void SetupTemplates()
        {
            CleanupTemplates();
            _keyTemplate = keyTemplate;
            _specialKeyTemplate = specialKeyTemplate;
            _modifierKeyTemplate = modifierKeyTemplate;

            if (_keyTemplate == null)
            {
                Debug.LogWarning("Canvas Key Renderer in " + name + " requires a keyTemplate prefab");
                // Create an empty game object so key positions are visible without a key template
                _keyTemplate = new GameObject("KeyboardKey");
                ProcessRuntimeObject(_keyTemplate, hide: true);
                _keyTemplate.transform.SetParent(gameObject.transform, false);
            }

            if (_specialKeyTemplate == null)
            {
                _specialKeyTemplate = _keyTemplate;
            }

            if (_modifierKeyTemplate == null)
            {
                _modifierKeyTemplate = _specialKeyTemplate;
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

            // Cleanup runtime container when disabled
            GameObject runtimeContainer = GetRuntimeObjectContainer(gameObject, create: false);
            if (runtimeContainer != null)
            {
                DestroyImmediate(runtimeContainer);
            }

            // Cleanup dynamically generated key template
            CleanupTemplates();
        }

        /// <summary>
        /// Apply a key's text, sprites, and event handlers to a game object
        /// </summary>
        /// <param name="keyset">The keyset the key belongs to</param>
        /// <param name="key">The key</param>
        /// <param name="gKey">The GameObject</param>
        public void ApplyKeySettingsToKey(int keyset, IKey key, GameObject gKey)
        {
            IKeyMeta keyMeta = KeyMeta.FromKey(key);

            gKey.name = keyMeta.GetName();

            TextMesh gKeyText = gKey.GetComponentInChildren<TextMesh>();
            if (gKeyText != null)
            {
                gKeyText.text = keyMeta.GetLabel();
            }

            SpriteRenderer gKeySprite = gKey.GetComponentInChildren<SpriteRenderer>();
            if (gKeySprite != null)
            {
                Sprite sprite = GetSpriteForKey(keyset, key);
                if (sprite != null)
                {
                    gKeySprite.sprite = sprite;
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

        protected void ApplyRectLayoutToTransform(Rect layout, Transform transform, Vector2 containerSize)
        {
            // Invert keyboard rect coordinates (y=down) to world space (y=up)
            Rect tRect = new Rect(layout);
            tRect.position = new Vector2(tRect.position.x, containerSize.y - tRect.size.y - tRect.position.y);

            Vector2 center = tRect.position + tRect.size / 2f - containerSize / 2f;
            transform.localPosition = new Vector3(center.x, center.y, 0);
        }
    }
}
