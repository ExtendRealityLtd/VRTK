// Keyboard|Keyboard|81000
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A VR keyboard
    /// </summary>
    /// <remarks>
    /// `VRTK_Keyboard` only provides some basic internal keyboard handling functionality.
    /// To make a keyboard that the user is able to interact with the Keyboard also requires a Key Renderer
    /// and a Key Layout Calculator.
    /// 
    /// A Keyboard can be used in one of two modes.
    /// 
    /// If a "Fixed Input Field" is defined all keyboard interactions will be send to that UI.InputField and
    /// the position of the keyboard will remain static.
    /// 
    /// Otherwise, the keyboard can be used with a `VRTK_KeyboardActor`. The actor will be responsible for
    /// displaying the keyboard when needed and may position the keyboard relative to a InputField's canvas
    /// or the player.
    /// </remarks>
    public class VRTK_Keyboard : MonoBehaviour
    {
        /// <summary>
        /// Keyboard key type
        /// </summary>
        /// <param name="Character">A key with character that should be typed.</param>
        /// <param name="KeysetModifier">A key that switches keysets.</param>
        /// <param name="Backspace">A backspace/delete key.</param>
        /// <param name="Enter">An enter/return key.</param>
        /// <param name="Done">A done key.</param>
        public enum KeyClass
        {
            Character,
            KeysetModifier,
            Backspace,
            Enter,
            Done
        }

        /// <summary>
        /// The core interface defining metadata a key requirires for `VRTK_Keyboard` to
        /// handle key presses for it.
        /// </summary>
        public interface IKey
        {
            /// <summary>
            /// Get the class of this key
            /// </summary>
            /// <returns></returns>
            KeyClass GetKeyClass();
            /// <summary>
            /// Get the character to type if key class is KeyClass.Character
            /// </summary>
            /// <returns>The character to type</returns>
            char GetCharacter();
            /// <summary>
            /// Get the keyset to switch to if key class is KeyClass.Keyset
            /// </summary>
            /// <returns>The keyset to switch to</returns>
            int GetKeyset();
        }

        /// <summary>
        /// An interface describing higher level key metadata 
        /// </summary>
        public interface IKeyMeta : IKey
        {
            /// <summary>
            /// Get the preferred GameObject name for this key
            /// </summary>
            string GetName();

            /// <summary>
            /// Get the button text for this key
            /// </summary>
            string GetLabel();
        }

        /// <summary>
        /// The standard IKeyMeta implementation used to generate and hold key metadata from an IKey
        /// </summary>
        public class KeyMeta : IKeyMeta
        {
            /// <summary>
            /// The preferred GameObject name for this key
            /// </summary>
            private readonly string name;
            /// <summary>
            /// The button text for this key
            /// </summary>
            private readonly string label;
            /// <summary>
            /// The class of this key
            /// </summary>
            private readonly KeyClass keyClass;
            /// <summary>
            /// The character to type (for keyClass = Character)
            /// </summary>
            private readonly char character;
            /// <summary>
            /// The keyset to switch to (for keyClass = KeysetModifier)
            /// </summary>
            private readonly int keyset;

            /// <summary>
            /// Create a basic non-Character non-KeysetModifier key
            /// </summary>
            /// <param name="name">The preferred GameObject name for this key</param>
            /// <param name="label"></param>
            /// <param name="keyClass">The keyset to switch to (for keyClass = KeysetModifier)</param>
            public KeyMeta(string name, string label, KeyClass keyClass)
            {
                this.name = name;
                this.label = label;
                this.keyClass = keyClass;
            }
            /// <summary>
            /// Create a Character key
            /// </summary>
            /// <param name="name">The preferred GameObject name for this key</param>
            /// <param name="label">The button text for this key</param>
            /// <param name="character">The character to type (for keyClass = Character)</param>
            public KeyMeta(string name, string label, char character) : this(name, label, KeyClass.Character)
            {
                this.character = character;
            }
            /// <summary>
            /// Create a KeysetModifier key
            /// </summary>
            /// <param name="name">The preferred GameObject name for this key</param>
            /// <param name="label">The button text for this key</param>
            /// <param name="keyset"></param>
            public KeyMeta(string name, string label, int keyset) : this(name, label, KeyClass.KeysetModifier)
            {
                this.keyset = keyset;
            }

            /// <summary>
            /// Use an existing key to generate the metadata
            /// </summary>
            /// <param name="key">The key to generate metadata for</param>
            public KeyMeta(IKey key)
            {
                keyClass = key.GetKeyClass();
                switch (keyClass)
                {
                    case KeyClass.Character:
                        character = key.GetCharacter();
                        label = name = character.ToString();
                        if (character == ' ')
                        {
                            name = "Spacebar";
                        }
                        break;
                    case KeyClass.KeysetModifier:
                        keyset = key.GetKeyset();
                        name = "KeysetModifier";
                        label = ""; // We do not have enough information to set this now
                        break;
                    case KeyClass.Backspace:
                        label = name = "Backspace";
                        break;
                    case KeyClass.Enter:
                        label = name = "Enter";
                        break;
                }
            }

            /// <summary>
            /// Return an IKeyMeta for an IKey
            /// </summary>
            /// <remarks>
            /// If key already implements IKeyMeta it will be returned.
            /// 
            /// Otherwise, a new KeyMeta instance will be created.
            /// </remarks>
            /// <param name="key">An IKey key</param>
            /// <returns>A</returns>
            public static IKeyMeta FromKey(IKey key)
            {
                IKeyMeta keyMeta = key as IKeyMeta;
                if (keyMeta == null)
                {
                    keyMeta = new KeyMeta(key);
                }
                return keyMeta;
            }

            public string GetName()
            {
                return name;
            }

            public string GetLabel()
            {
                return label;
            }

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

        [Tooltip("A UI.InputField to send all keyboard info to")]
        public InputField fixedInputField;

        protected InputField input;
        protected VRTK_BaseKeyLayoutRenderer layoutRenderer;

        void Start()
        {
            if (fixedInputField != null)
            {
                input = fixedInputField;
                layoutRenderer.SetEnterEnabled(input.multiLine);
            }

            layoutRenderer = GetComponent<VRTK_BaseKeyLayoutRenderer>();
        }

        void SetInputField(InputField input)
        {
            if (fixedInputField == null)
            {
                this.input = input;
                layoutRenderer.SetEnterEnabled(input.multiLine);
            }
            else
            {
                Debug.LogWarning("The input field of a VRTK_Keyboard with a fixedInputField cannot be changed");
            }
        }

        public void HandleKeypress(IKey key)
        {
            if (input == null)
            {
                Debug.LogWarning("Key pressed on " + name + " but no input field is assigned");
                return;
            }

            Event evt;
            switch (key.GetKeyClass())
            {
                case KeyClass.Character:
                    evt = new Event();
                    evt.type = EventType.KeyUp;
                    evt.modifiers = EventModifiers.None;
                    evt.character = key.GetCharacter();

                    input.ProcessEvent(evt);
                    break;
                case KeyClass.KeysetModifier:
                    if (layoutRenderer != null)
                    {
                        layoutRenderer.SetKeyset(key.GetKeyset());
                    }
                    break;
                case KeyClass.Backspace:
                    evt = new Event();
                    evt.modifiers = EventModifiers.None;
                    evt.keyCode = KeyCode.Backspace;

                    input.ProcessEvent(evt);
                    break;
                case KeyClass.Enter:
                    if (input.multiLine)
                    {
                        evt = new Event();
                        evt.modifiers = EventModifiers.None;
                        evt.keyCode = KeyCode.Return;
                        evt.character = '\n';

                        input.ProcessEvent(evt);
                    }
                    else
                    {
                        Debug.LogWarning("Enter pressed on " + name + " but input field " + input.name + " is single line");
                    }
                    break;
                case KeyClass.Done:
                    // TODO: Done signals should be sent to a KeyboardActor if one is attached
                    //       or some way of submitting an input field using input.DeactivateInputField(); should be done
                    break;
            }
        }
    }
}
