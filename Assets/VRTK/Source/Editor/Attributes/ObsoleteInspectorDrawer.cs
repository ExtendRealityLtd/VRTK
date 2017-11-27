namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;

    [CustomPropertyDrawer(typeof(ObsoleteInspectorAttribute))]
    class ObsoleteInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ObsoleteAttribute obsoleteAttribute = (ObsoleteAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ObsoleteAttribute));
            EditorStyles.label.richText = true;
            EditorGUI.PropertyField(position, property, new GUIContent("<color=red><i>" + label.text + "</i></color>", "**OBSOLETE**\n\n" + obsoleteAttribute.Message), true);
        }
    }
}