namespace VRTK
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(Limits2D))]
    public class Limits2DDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.tooltip = VRTK_EditorUtilities.GetTooltipAttribute(fieldInfo).tooltip;
            SerializedProperty minimum = property.FindPropertyRelative("minimum");
            SerializedProperty maximum = property.FindPropertyRelative("maximum");

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            float updatePositionX = position.x;
            float labelWidth = 30f;
            float fieldWidth = (position.width / 3f) - labelWidth;

            EditorGUI.LabelField(new Rect(updatePositionX, position.y, labelWidth, position.height), "Min");
            updatePositionX += labelWidth;
            minimum.floatValue = EditorGUI.FloatField(new Rect(updatePositionX, position.y, fieldWidth, position.height), minimum.floatValue);
            updatePositionX += fieldWidth;

            EditorGUI.LabelField(new Rect(updatePositionX, position.y, labelWidth, position.height), "Max");
            updatePositionX += labelWidth;
            maximum.floatValue = EditorGUI.FloatField(new Rect(updatePositionX, position.y, fieldWidth, position.height), maximum.floatValue);
            updatePositionX += fieldWidth;

            EditorGUI.indentLevel = indent;
        }
    }
}