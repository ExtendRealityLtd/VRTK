namespace VRTK
{
    using UnityEditor;

    [CustomEditor(typeof(VRTK_BaseCanvasKeyLayoutRenderer), true)]
    public class VRTK_CanvasKeyLayoutRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VRTK_BaseCanvasKeyLayoutRenderer renderer = (VRTK_BaseCanvasKeyLayoutRenderer)target;
            VRTK_KeyboardLayoutSource source = renderer.GetComponent<VRTK_KeyboardLayoutSource>();

            if (renderer.keyTemplate == null)
            {
                EditorGUILayout.HelpBox("A Key Template is required", MessageType.Error);
            }

            // As a special case, if a simple KeyboardLayoutSource is being used, make sure that the
            // KeyboardLayout used matches the keyLayoutSprite's reference layout
            if (renderer.keyLayoutSprites != null &&
                renderer.keyLayoutSprites.keyboardLayoutReference != null &&
                source != null &&
                source.keyboardLayout != null &&
                renderer.keyLayoutSprites.keyboardLayoutReference != source.keyboardLayout)
            {
                EditorGUILayout.HelpBox("Key Layout Sprites' reference layout does not match this keyboard layout", MessageType.Warning);
            }

            DrawDefaultInspector();
        }
    }
}
