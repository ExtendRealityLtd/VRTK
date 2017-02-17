namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using RKeyLayout = VRTK_RenderableKeyLayout;
    using RKeyset = VRTK_RenderableKeyLayout.Keyset;

    [CustomEditor(typeof(VRTK_CanvasKeyLayoutRenderer), true)]
    public class VRTK_CanvasKeyLayoutRendererEditor : Editor
    {
        SerializedProperty keysetModifierImages;

        protected bool keysetModifierImagesExpanded = false;
        protected string[] keysetNames;

        private void OnEnable()
        {
            keysetModifierImages = serializedObject.FindProperty("keysetModifierImages");
        }

        public override void OnInspectorGUI()
        {
            VRTK_CanvasKeyLayoutRenderer renderer = (VRTK_CanvasKeyLayoutRenderer)target;
            RKeyLayout keyLayout = renderer.CalculateRenderableKeyLayout(Vector2.one * 100);
            keysetNames = keyLayout == null
                ? null
                : Array.ConvertAll(keyLayout.keysets, (keyset) => keyset.name);

            if (renderer.keyTemplate == null)
            {
                EditorGUILayout.HelpBox("A Key Template is required", MessageType.Error);
            }

            if (keyLayout == null)
            {
                EditorGUILayout.HelpBox("Layout calculator did not return a key layout, editor will not be complete until layout calculator issues are fixed", MessageType.Warning);
            }

            serializedObject.Update();

            Action run = null;

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name != "m_Script" && iterator.name != "keysetModifierImages")
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }

                enterChildren = false;
            }

            keysetModifierImagesExpanded = EditorGUILayout.Foldout(keysetModifierImagesExpanded, "Keyset Modifier Images");

            if (keysetModifierImagesExpanded)
            {
                for (int i = 0; i < keysetModifierImages.arraySize; i++)
                {
                    SerializedProperty kmi = keysetModifierImages.GetArrayElementAtIndex(i);
                    EditorGUILayout.BeginVertical("box");
                    KeysetPropertyField(kmi.FindPropertyRelative("inKeyset"));
                    KeysetPropertyField(kmi.FindPropertyRelative("keyset"));
                    EditorGUILayout.PropertyField(kmi.FindPropertyRelative("sourceImage"));
                    if (GUILayout.Button("Remove image"))
                    {
                        int index = i;
                        run = () => keysetModifierImages.DeleteArrayElementAtIndex(index);
                    }
                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("Add image"))
                {
                    run = () =>
                    {
                        keysetModifierImages.arraySize++;
                        SerializedProperty kmi = keysetModifierImages.GetArrayElementAtIndex(keysetModifierImages.arraySize - 1);
                        kmi.FindPropertyRelative("inKeyset").intValue = -1;
                        kmi.FindPropertyRelative("keyset").intValue = -1;
                    };
                }
            }

            if (run != null)
            {
                run();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected void KeysetPropertyField(SerializedProperty prop)
        {
            if (keysetNames == null)
            {
                EditorGUILayout.PropertyField(prop);
            }
            else
            {
                GUIContent[] options = new GUIContent[keysetNames.Length + 1];
                options[0] = new GUIContent("All");
                for (int i = 0; i < keysetNames.Length; i++)
                {
                    options[i + 1] = new GUIContent(keysetNames[i]);
                }
                GUIContent label = EditorGUI.BeginProperty(Rect.zero, null, prop);
                prop.intValue = EditorGUILayout.Popup(label, prop.intValue + 1, options) - 1;
                EditorGUI.EndProperty();
            }
        }
    }
}
