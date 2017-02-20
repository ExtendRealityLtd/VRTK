// Keyboard Layout|Keyboard|81010
namespace VRTK
{
    using UnityEngine;
    using System;
    using KeyClass = VRTK_Keyboard.KeyClass;
    using IKey = VRTK_Keyboard.IKey;

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
    /// The "Backspace" and "Enter" keys are special key types.
    /// 
    /// A "Keyset Modifier" key is used to switch between key sets.
    ///   
    /// </remarks>
    [CreateAssetMenu(fileName = "KeyboardLayout", menuName = "VRTK/Keyboard Layout")]
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
            public int splitIndex;
        }
        
        [Serializable]
        public abstract class BaseKey
        {
            public char character;
        }

        [Serializable]
        public class Key : BaseKey, IKey
        {
            public KeyClass keyClass;
            public int keyset;
            public Subkey[] subkeys;
            public int weight = 1;

            public KeyClass GetKeyClass()
            {
                return keyClass;
            }

            public char GetCharacter()
            {
                return character;
            }

            public int GetKeyset()
            {
                return keyset;
            }
        }

        [Serializable]
        public class Subkey : BaseKey, IKey
        {
            public KeyClass GetKeyClass()
            {
                return KeyClass.Character;
            }

            public char GetCharacter()
            {
                return character;
            }

            public int GetKeyset()
            {
                throw new NotImplementedException();
            }
        }

        public Keyset[] keysets = new Keyset[1];
    }
}
