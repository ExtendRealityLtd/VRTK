// Keyset Modifier Images|Keyboard|81015
namespace VRTK
{
    using UnityEngine;
    using System;
    using KeyboardLayout = VRTK_KeyboardLayout;
    using KeyClass = VRTK_Keyboard.KeyClass;
    using IKey = VRTK_Keyboard.IKey;

    /// <summary>
    /// Key Layout Sprites scriptable objects define a series of rules for assigning sprites to keys.
    /// </summary>
    [CreateAssetMenu(fileName = "KeyLayoutSprites", menuName = "VRTK/Key Layout Sprites")]
    public class VRTK_KeyLayoutSprites : ScriptableObject
    {
        [Serializable]
        public class KeysetModifierSprite
        {
            [Tooltip("Only apply inside this keyset")]
            public int inKeyset = -1;
            [Tooltip("Apply to this modifier key")]
            public int keyset = -1;
            [Tooltip("Image sprite to apply to the keyset modifier key's UI.Image")]
            public Sprite sourceImage;
        }

        [Tooltip("Optional reference to a Keyboard Layout used to display keyset names while editing ")]
        public KeyboardLayout keyboardLayoutReference;

        [Tooltip("Rule based overrides for keyset modifier key sprites")]
        public KeysetModifierSprite[] keysetModifierSprites;

        /// <summary>
        /// Return the Sprite to use for a key if one is set
        /// </summary>
        /// <param name="keyset">The keyset the key belongs to</param>
        /// <param name="key">The key</param>
        /// <returns>A Sprite instance to use, if one is associated with the key</returns>
        public Sprite GetSpriteForKey(int keyset, IKey key)
        {
            if (key.GetKeyClass() == KeyClass.KeysetModifier)
            {
                foreach (KeysetModifierSprite kms in keysetModifierSprites)
                {
                    if (kms.inKeyset != -1 && kms.inKeyset != keyset)
                    {
                        continue;
                    }

                    if (kms.keyset != -1 && kms.keyset != key.GetKeyset())
                    {
                        continue;
                    }

                    return kms.sourceImage;
                }
            }

            return null;
        }
    }
}