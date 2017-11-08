namespace VRTK
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(Vector3State))]
    public class Vector3StateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.tooltip = VRTK_EditorUtilities.GetTooltipAttribute(fieldInfo).tooltip;
            SerializedProperty xState = property.FindPropertyRelative("xState");
            SerializedProperty yState = property.FindPropertyRelative("yState");
            SerializedProperty zState = property.FindPropertyRelative("zState");

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            float updatePositionX = position.x;
            float labelWidth = 15f;
            float fieldWidth = (position.width / 3f) - labelWidth;

            EditorGUI.LabelField(new Rect(updatePositionX, position.y, labelWidth, position.height), "X");
            updatePositionX += labelWidth;
            xState.boolValue = EditorGUI.Toggle(new Rect(updatePositionX, position.y, fieldWidth, position.height), xState.boolValue);
            updatePositionX += fieldWidth;

            EditorGUI.LabelField(new Rect(updatePositionX, position.y, labelWidth, position.height), "Y");
            updatePositionX += labelWidth;
            yState.boolValue = EditorGUI.Toggle(new Rect(updatePositionX, position.y, fieldWidth, position.height), yState.boolValue);
            updatePositionX += fieldWidth;

            EditorGUI.LabelField(new Rect(updatePositionX, position.y, labelWidth, position.height), "Z");
            updatePositionX += labelWidth;
            zState.boolValue = EditorGUI.Toggle(new Rect(updatePositionX, position.y, fieldWidth, position.height), zState.boolValue);
            updatePositionX += fieldWidth;

            EditorGUI.indentLevel = indent;
        }
    }
}