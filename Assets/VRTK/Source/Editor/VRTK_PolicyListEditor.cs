namespace VRTK
{
    using UnityEditor;
    using System;

    [CustomEditor(typeof(VRTK_PolicyList))]
    public class VRTK_PolicyListEditor : Editor
    {
        SerializedProperty staticFlagMask;
        SerializedProperty identifiers;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("operation"));
            staticFlagMask.intValue = (int)((VRTK_PolicyList.CheckTypes)EnumField("Check Types", (VRTK_PolicyList.CheckTypes)staticFlagMask.intValue));
            ArrayGUI(identifiers);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            staticFlagMask = serializedObject.FindProperty("checkType");
            identifiers = serializedObject.FindProperty("identifiers");
        }

        private Enum EnumField(string label, Enum enumValue)
        {
#if UNITY_2017_3_OR_NEWER
            return EditorGUILayout.EnumFlagsField(label, enumValue);
#else
            return EditorGUILayout.EnumMaskField(label, enumValue);
#endif
        }

        private void ArrayGUI(SerializedProperty property)
        {
            SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(arraySizeProp);
            EditorGUI.indentLevel++;

            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i));
            }
            EditorGUI.indentLevel--;
        }
    }
}