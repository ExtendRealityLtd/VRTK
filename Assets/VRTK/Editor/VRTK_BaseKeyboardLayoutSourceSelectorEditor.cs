namespace VRTK
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// This abstract class is the base for classes extending the `VRTK_KeyboardEditor` Key Layout Source selector
    /// </summary>
    public abstract class VRTK_BaseKeyboardLayoutSourceSelectorEditor
    {
        public abstract void KeyboardEditorGUI();

        public abstract void SetupComponent(Component component);
    }
}