// Keyboard Layout|Keyboard|81010
namespace VRTK
{
    using UnityEngine;
    using System;

    /// <summary>
    /// The Keyboard Layout script defines a creatable scriptable object defining the keys present on a keyboard rendered by a Keyboard Renderer.
    /// </summary>
    /// <remarks>
    /// Every keyboard layout contains a number of key sets, these key sets can be used both for lowercase/uppercase shifting and for symbols key sets.
    /// 
    /// Each key set has a number of rows each containing keys.
    /// 
    /// Keys may have a numeric weight and come in different types:
    /// 
    /// Normal keys defining a character that will be typed are "Character" keys and may optionally contain subkeys that may display when the key is held down.
    /// 
    /// The "Backspace", "Enter", and "Done" keys are special key types.
    /// 
    /// A "Keyset Modifier" key is used to switch between key sets.
    ///   
    /// </remarks>
    [CreateAssetMenu(fileName = "KeyboardLayout", menuName = "VRTK/KeyboardLayout")]
    public class VRTK_KeyboardLayout : ScriptableObject
    {
        [Serializable]
        public class Keyset
        {
            public string name;
            public Row[] rows;
        }

        [Serializable]
        public class Row
        {
            public Key[] keys;
        }

        /// <summary>
        /// Keyboard key type
        /// </summary>
        /// <param name="Character">A key with character that should be typed.</param>
        /// <param name="KeysetModifier">A key that switches keysets.</param>
        /// <param name="Backspace">A backspace/delete key.</param>
        /// <param name="Enter">An enter/return key.</param>
        /// <param name="Done">A done key intended to finish input.</param>
        public enum Keytype
        {
            Character,
            KeysetModifier,
            Backspace,
            Enter,
            Done
        }

        [Serializable]
        public abstract class BaseKey
        {
            public char character;
        }

        [Serializable]
        public class Key : BaseKey
        {
            public Keytype type;
            public int keyset;
            public Subkey[] subkeys;
            public int weight = 1;
        }

        [Serializable]
        public class Subkey : BaseKey
        {

        }

        public Keyset[] keysets = new Keyset[1];
    }
}
