namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using KeyboardLayout = VRTK_KeyboardLayout;

    [CustomEditor(typeof(VRTK_KeyLayoutSprites))]
    public class VRTK_KeyLayoutSpritesEditor : Editor
    {
        SerializedProperty keyboardLayoutReference;
        SerializedProperty keysetModifierSprites;

        protected string[] keysetNames;

        private void OnEnable()
        {
            keyboardLayoutReference = serializedObject.FindProperty("keyboardLayoutReference");
            keysetModifierSprites = serializedObject.FindProperty("keysetModifierSprites");

        }

        public override void OnInspectorGUI()
        {
            KeyboardLayout keyboardLayout = keyboardLayoutReference.objectReferenceValue as KeyboardLayout;
            keysetNames = keyboardLayout == null
                ? null
                : Array.ConvertAll(keyboardLayout.keysets, (keyset) => keyset.name);

            Action run = null;

            serializedObject.Update();

            EditorGUILayout.PropertyField(keyboardLayoutReference);

            GUILayout.Label("Keyset Modifier Sprites", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Rules for applying sprites to keyset modifier keys. The first rule that matches a key will be used so place more generic rules at the end of the list.", MessageType.None);

            if (keysetModifierSprites.arraySize > 0 && keysetNames == null)
            {
                EditorGUILayout.HelpBox("Reference keyboard layout is not available. Keyset selectors will be simple integer fields. (-1 means \"All keysets\"; anything higher is the keyset's index)", MessageType.Warning);
            }

            for (int i = 0; i < keysetModifierSprites.arraySize; i++)
            {
                SerializedProperty rule = keysetModifierSprites.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal("box");

                EditorGUILayout.BeginVertical();
                KeysetPropertyField(rule.FindPropertyRelative("inKeyset"));
                KeysetPropertyField(rule.FindPropertyRelative("keyset"));
                EditorGUILayout.PropertyField(rule.FindPropertyRelative("sourceImage"));
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(20f));
                if (GUILayout.Button("x", GUILayout.MaxWidth(20f)))
                {
                    int index = i;
                    run = () =>
                    {
                        keysetModifierSprites.DeleteArrayElementAtIndex(index);
                    };
                }
                using (new EditorGUI.DisabledScope(i <= 0))
                {
                    if (GUILayout.Button("▲", GUILayout.MaxWidth(20f)))
                    {
                        int index = i;
                        run = () =>
                        {
                            keysetModifierSprites.MoveArrayElement(index, index - 1);
                        };
                    }
                }

                using (new EditorGUI.DisabledScope(i >= keysetModifierSprites.arraySize - 1))
                {
                    if (GUILayout.Button("▼", GUILayout.MaxWidth(20f)))
                    {

                        int index = i;
                        run = () =>
                        {
                            keysetModifierSprites.MoveArrayElement(index, index + 1);
                        };
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Create Rule"))
            {
                keysetModifierSprites.arraySize++;
                SerializedProperty kmi = keysetModifierSprites.GetArrayElementAtIndex(keysetModifierSprites.arraySize - 1);
                kmi.FindPropertyRelative("inKeyset").intValue = -1;
                kmi.FindPropertyRelative("keyset").intValue = -1;
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