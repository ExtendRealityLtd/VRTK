namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using KeyClass = VRTK_Keyboard.KeyClass;

    [CustomEditor(typeof(VRTK_KeyboardLayout))]
    public class VRTK_KeyboardLayoutEditor : Editor
    {
        SerializedProperty keysets;

        protected int selectedKeyset = 0;
        protected int? selectedRow;
        protected int? selectedKey;
        protected bool showKeysetDimensionsFoldout = false;

        private void OnEnable()
        {
            keysets = serializedObject.FindProperty("keysets");
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

            // Keyset dimensions
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel++;
            showKeysetDimensionsFoldout = EditorGUILayout.Foldout(showKeysetDimensionsFoldout, "Dimensions");
            EditorGUI.indentLevel--;

            if (showKeysetDimensionsFoldout)
            {
                EditorGUILayout.PropertyField(rows.FindPropertyRelative("Array.size"),
                    new GUIContent("Rows"));

                for (int r = 0; r < rows.arraySize; r++)
                {
                    SerializedProperty row = rows.GetArrayElementAtIndex(r);
                    SerializedProperty rowKeys = row.FindPropertyRelative("keys");

                    GUILayout.Label("Row #" + (r + 1));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(rowKeys.FindPropertyRelative("Array.size"),
                        new GUIContent("Keys"));
                    EditorGUILayout.PropertyField(row.FindPropertyRelative("splitIndex"),
                        new GUIContent("Split Index", "Index at which row may be split into a left and right section"));
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();

            // Fix selected key and row index if any edits made it out of bounds
            if ((selectedRow ?? -1) >= rows.arraySize)
            {
                selectedRow = null;
                selectedKey = null;
            }

            if (selectedRow != null && selectedKey != null)
            {
                SerializedProperty rowKeys = rows.GetArrayElementAtIndex(selectedRow ?? -1)
                    .FindPropertyRelative("keys");

                if ((selectedKey ?? -1) >= rowKeys.arraySize)
                {
                    selectedRow = null;
                    selectedKey = null;
                }
            }

            // Key editor
            if (rows.arraySize != 0)
            {
                EditorGUILayout.BeginVertical("box");
                for (int r = 0; r < rows.arraySize; r++)
                {
                    SerializedProperty row = rows.GetArrayElementAtIndex(r);
                    int splitIndex = row.FindPropertyRelative("splitIndex").intValue;
                    SerializedProperty keys = row.FindPropertyRelative("keys");

                    EditorGUILayout.BeginHorizontal();
                    for (int k = 0; k < keys.arraySize; k++)
                    {
                        if (k == splitIndex)
                        {
                            GUILayout.Space(32);
                        }

                        SerializedProperty key = keys.GetArrayElementAtIndex(k);
                        SerializedProperty keyClass = key.FindPropertyRelative("keyClass");
                        
                        string label = "";
                        switch ((KeyClass)keyClass.intValue)
                        {
                            case KeyClass.Character:
                                label = char.ConvertFromUtf32(key.FindPropertyRelative("character").intValue);
                                break;
                            case KeyClass.KeysetModifier:
                                label = "#" + key.FindPropertyRelative("keyset").intValue.ToString();
                                break;
                            case KeyClass.Backspace:
                                label = "Backspace";
                                break;
                            case KeyClass.Enter:
                                label = "⏎";
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
            }

            // Key editor
            if (selectedRow != null && selectedKey != null)
            {
                SerializedProperty keys = rows.GetArrayElementAtIndex(selectedRow ?? -1)
                        .FindPropertyRelative("keys");
                SerializedProperty key = keys.GetArrayElementAtIndex(selectedKey ?? -1);
                SerializedProperty keyClass = key.FindPropertyRelative("keyClass");

                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.PropertyField(keyClass);

                switch ((KeyClass)keyClass.intValue)
                {
                    case KeyClass.Character:
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
                    case KeyClass.KeysetModifier:
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