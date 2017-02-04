// Keyboard|Keyboard|81000
namespace VRTK
{
    using UnityEngine;
    using UnityEngine.UI;
    using RKey = VRTK_RenderableKeyLayout.Key;

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

        public void HandleKeypress(RKey key)
        {
            if (input == null)
            {
                Debug.LogWarning("Key pressed on " + name + " but no input field is assigned");
                return;
            }

            Event evt;
            switch (key.type)
            {
                case RKey.Type.Character:
                    evt = new Event();
                    evt.type = EventType.KeyUp;
                    evt.modifiers = EventModifiers.None;
                    evt.character = key.character;

                    input.ProcessEvent(evt);
                    break;
                case RKey.Type.KeysetModifier:
                    if (layoutRenderer != null)
                    {
                        layoutRenderer.SetKeyset(key.keyset);
                    }
                    break;
                case RKey.Type.Backspace:
                    evt = new Event();
                    evt.modifiers = EventModifiers.None;
                    evt.keyCode = KeyCode.Backspace;

                    input.ProcessEvent(evt);
                    break;
                case RKey.Type.Enter:
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
                case RKey.Type.Done:
                    // TODO: Done signals should be sent to a KeyboardActor if one is attached
                    //       or some way of submitting an input field using input.DeactivateInputField(); should be done
                    break;
            }
        }
    }
}
