// Interactive Haptics Input|Interactions|30102
namespace VRTK
{
    using UnityEngine;

    /// <summary>
    /// The Interactive Haptics script is attached on the same GameObject as an Interactable Object script and provides customizable haptic feedback curves for more detailed interactions.
    /// </summary>
    public abstract class VRTK_InteractiveHapticsInput : MonoBehaviour
    {
        public delegate void InputProvidedHandler(float normalizedValue);
        public InputProvidedHandler InputProvided;
        
        protected void OnInputProvided(float normalizedValue)
        {
            if(InputProvided != null)
            {
                InputProvided(normalizedValue);
            }
        }
    }
}