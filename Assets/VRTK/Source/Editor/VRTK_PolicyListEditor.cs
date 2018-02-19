namespace VRTK
{
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(VRTK_PolicyList))]
    public class VRTK_PolicyListEditor : Editor
    {
        SerializedProperty staticFlagMask;
        SerializedProperty identifiers;

        private void OnEnable()
        {
            staticFlagMask = serializedObject.FindProperty("checkType");
            identifiers = serializedObject.FindProperty("identifiers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("operation"));
 #if UNITY_2017_3_OR_NEWER
            staticFlagMask.intValue = (int)((VRTK_PolicyList.CheckTypes)EditorGUILayout.EnumFlagsField("Check Types", (VRTK_PolicyList.CheckTypes)staticFlagMask.intValue));
 #else
            staticFlagMask.intValue = (int)((VRTK_PolicyList.CheckTypes)EditorGUILayout.EnumMaskField("Check Types", (VRTK_PolicyList.CheckTypes)staticFlagMask.intValue));
 #endif
            ArrayGUI(identifiers);

            serializedObject.ApplyModifiedProperties();
        }

        void ArrayGUI(SerializedProperty property)
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