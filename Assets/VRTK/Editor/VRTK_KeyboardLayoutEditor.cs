namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;

    [CustomEditor(typeof(VRTK_KeyboardLayout))]
    public class VRTK_KeyboardLayoutEditor : Editor
    {
        SerializedProperty keysets;
        SerializedProperty defaultKeyset;

        protected int selectedKeyset = 0;
        protected int? selectedRow;
        protected int? selectedKey;

        private void OnEnable()
        {
            keysets = serializedObject.FindProperty("keysets");
            defaultKeyset = serializedObject.FindProperty("defaultKeyset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Action run = null;

            // Selector
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < keysets.arraySize; i++)
            {
                SerializedProperty set = keysets.GetArrayElementAtIndex(i);
                if (GUILayout.Toggle(selectedKeyset == i, set.FindPropertyRelative("name").stringValue, "Button") && selectedKeyset != i)
                {
                    selectedKeyset = i;
                    selectedRow = null;
                    selectedKey = null;
                }
            }
            EditorGUILayout.EndHorizontal();

            // Keyset editor
            SerializedProperty selectedSet = keysets.GetArrayElementAtIndex(selectedKeyset);
            SerializedProperty rows = selectedSet.FindPropertyRelative("rows");

            // Buttons
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.PropertyField(selectedSet.FindPropertyRelative("name"));

            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(selectedKeyset <= 0))
            {
                if (GUILayout.Button("<"))
                {
                    run = () => MoveKeyset(selectedKeyset - 1);
                }
            }
            if (GUILayout.Button("Duplicate"))
            {
                run = () => DuplicateKeyset();
            }
            using (new EditorGUI.DisabledScope(keysets.arraySize <= 1))
            {
                if (GUILayout.Button("Delete"))
                {
                    run = () => DeleteKeyset();
                }
            }
            using (new EditorGUI.DisabledScope(selectedKeyset >= keysets.arraySize - 1))
            {
                if (GUILayout.Button(">"))
                {
                    run = () => MoveKeyset(selectedKeyset + 1);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            // Key editor
            EditorGUILayout.BeginVertical("Box");
            for (int r = 0; r < rows.arraySize; r++)
            {
                SerializedProperty row = rows.GetArrayElementAtIndex(r);
                SerializedProperty keys = row.FindPropertyRelative("keys");
                EditorGUILayout.BeginHorizontal();
                for (int k = 0; k < keys.arraySize; k++)
                {
                    SerializedProperty key = keys.GetArrayElementAtIndex(k);
                    SerializedProperty keytype = key.FindPropertyRelative("type");
                    string label = "";
                    switch ((VRTK_KeyboardLayout.Keytype)keytype.intValue)
                    {
                        case VRTK_KeyboardLayout.Keytype.Character:
                            label = char.ConvertFromUtf32(key.FindPropertyRelative("character").intValue);
                            break;
                        case VRTK_KeyboardLayout.Keytype.KeysetModifier:
                            label = "#" + key.FindPropertyRelative("keyset").intValue.ToString();
                            break;
                        case VRTK_KeyboardLayout.Keytype.Backspace:
                            label = "Backspace";
                            break;
                        case VRTK_KeyboardLayout.Keytype.Enter:
                            label = "⏎";
                            break;
                        case VRTK_KeyboardLayout.Keytype.Done:
                            label = "Done";
                            break;
                    }

                    if (GUILayout.Toggle(selectedRow == r && selectedKey == k, label, "Button"))
                    {
                        selectedRow = r;
                        selectedKey = k;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            // Key editor
            if (selectedRow != null && selectedKey != null)
            {
                SerializedProperty key = keysets.GetArrayElementAtIndex(selectedKeyset)
                        .FindPropertyRelative("rows").GetArrayElementAtIndex(selectedRow ?? -1)
                        .FindPropertyRelative("keys").GetArrayElementAtIndex(selectedKey ?? -1);
                SerializedProperty keyType = key.FindPropertyRelative("type");

                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.PropertyField(keyType);

                switch ((VRTK_KeyboardLayout.Keytype)keyType.intValue)
                {
                    case VRTK_KeyboardLayout.Keytype.Character:
                        EditorGUILayout.PropertyField(key.FindPropertyRelative("character"));

                        GUILayout.Label("Subkeys");
                        SerializedProperty subkeys = key.FindPropertyRelative("subkeys");
                        EditorGUILayout.BeginHorizontal();

                        for (int sk = 0; sk < subkeys.arraySize; sk++)
                        {
                            SerializedProperty subkey = subkeys.GetArrayElementAtIndex(sk).FindPropertyRelative("character");
                            subkey.stringValue = EditorGUILayout.TextField(subkey.stringValue, GUILayout.MaxWidth(20f));
                        }

                        using (new EditorGUI.DisabledScope(subkeys.arraySize == 0))
                        {
                            if (GUILayout.Button("-")) { subkeys.arraySize--; }
                        }
                        if (GUILayout.Button("+")) { subkeys.arraySize++; }

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VRTK_KeyboardLayout.Keytype.KeysetModifier:
                        SerializedProperty keyset = key.FindPropertyRelative("keyset");
                        string[] keysetNames = new string[keysets.arraySize];
                        for (int i = 0; i < keysets.arraySize; i++)
                        {
                            keysetNames[i] = keysets.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
                        }
                        keyset.intValue = EditorGUILayout.Popup("Keyset", keyset.intValue, keysetNames);
                        break;
                }

                EditorGUILayout.IntSlider(key.FindPropertyRelative("weight"), 1, 5);

                EditorGUILayout.EndVertical();
            }

            if (run != null)
            {
                run();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected void MoveKeyset(int newIndex)
        {
            keysets.MoveArrayElement(selectedKeyset, newIndex);
            selectedKeyset = newIndex;
        }

        protected void DuplicateKeyset()
        {
            SerializedProperty keyset = keysets.GetArrayElementAtIndex(selectedKeyset);
            keyset.DuplicateCommand();
            selectedKeyset++;
            selectedRow = null;
            selectedKey = null;
        }

        protected void DeleteKeyset()
        {
            keysets.DeleteArrayElementAtIndex(selectedKeyset);
            selectedKeyset--;
            if (selectedKeyset == -1)
            {
                selectedKeyset = 0;
            }
            selectedRow = null;
            selectedKey = null;
        }
    }
};