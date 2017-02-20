namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using AttributeUtils = VRTK_AttributeUtilities;
    using KeyboardLayout = VRTK_KeyboardLayout;
    using RKeyLayout = VRTK_RenderableKeyLayout;

    [CustomEditor(typeof(VRTK_BaseCanvasKeyLayoutRenderer), true)]
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
            VRTK_BaseCanvasKeyLayoutRenderer renderer = (VRTK_BaseCanvasKeyLayoutRenderer)target;
            VRTK_KeyLayoutRendererAttribute attribute = AttributeUtils.GetAttribute<VRTK_KeyLayoutRendererAttribute>(target.GetType());

            if (attribute.requireCalculator)
            {
                RKeyLayout keyLayout = renderer.CalculateRenderableKeyLayout(new Vector2[] { Vector2.one * 100 });
                keysetNames = keyLayout == null
                    ? null
                    : Array.ConvertAll(keyLayout.keysets, (keyset) => keyset.name);

                if (keyLayout == null)
                {
                    EditorGUILayout.HelpBox("Layout calculator did not return a key layout, editor will not be complete until layout calculator issues are fixed", MessageType.Warning);
                }
            }
            else if (attribute.requireSource)
            {
                KeyboardLayout keyLayout = renderer.GetKeyLayout();
                keysetNames = keyLayout == null
                    ? null
                    : Array.ConvertAll(keyLayout.keysets, (keyset) => keyset.name);

                if (keyLayout == null)
                {
                    EditorGUILayout.HelpBox("Layout source did not return a key layout, editor will not be complete until layout source issues are fixed", MessageType.Warning);
                }
            }

            if (renderer.keyTemplate == null)
            {
                EditorGUILayout.HelpBox("A Key Template is required", MessageType.Error);
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
