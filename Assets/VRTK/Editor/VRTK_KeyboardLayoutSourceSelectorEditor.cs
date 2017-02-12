namespace VRTK
{
    using UnityEditor;
    using UnityEngine;
    using KeyboardLayout = VRTK_KeyboardLayout;

    /// <summary>
    /// Extends the `VRTK_KeyboardLayoutSource` selector with a `VRTK_KeyboardLayout` field
    /// </summary>
    [VRTK_CustomLayoutSourceSelector(typeof(VRTK_KeyboardLayoutSource))]
    public class VRTK_KeyboardLayoutSourceSelectorEditor : VRTK_BaseKeyboardLayoutSourceSelectorEditor
    {
        public KeyboardLayout keyboardLayout;

        public override void KeyboardEditorGUI()
        {
            keyboardLayout = (KeyboardLayout)EditorGUILayout.ObjectField(keyboardLayout, typeof(KeyboardLayout), false);
        }

        public override void SetupComponent(Component component)
        {
            VRTK_KeyboardLayoutSource source = (VRTK_KeyboardLayoutSource)component;
            source.keyboardLayout = keyboardLayout;
        }
    }
}