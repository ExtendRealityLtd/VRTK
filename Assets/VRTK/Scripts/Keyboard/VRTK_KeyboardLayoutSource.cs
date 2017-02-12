// Keyboard Layout Source|Keyboard|81051
namespace VRTK
{
    using UnityEngine;
    using KeyboardLayout = VRTK_KeyboardLayout;

    /// <summary>
    /// A basic key layout source providing a serialized `VRTK_KeyboardLayout` as a key layout
    /// </summary>
    [VRTK_KeyLayoutSource(name = "Keyboard Layout", help = "Use a serialized Keyboard Layout as a source")]
    public class VRTK_KeyboardLayoutSource : VRTK_BaseKeyLayoutSource
    {
        [Tooltip("Keyboard layout to render")]
        public KeyboardLayout keyboardLayout;

        /// <summary>
        /// Provide the serialized `VRTK_KeyboardLayout` as the key layout
        /// </summary>
        /// <returns>The key layout</returns>
        public override KeyboardLayout GetKeyLayout()
        {
            return keyboardLayout;
        }
    }
}