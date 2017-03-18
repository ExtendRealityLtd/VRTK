namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;

    public static class VRTK_EditorUtilities
    {
        public static GUIContent BuildGUIContent<T>(string fieldName, string displayOverride = null)
        {
            var displayName = (displayOverride != null ? displayOverride : ObjectNames.NicifyVariableName(fieldName));
            var fieldInfo = typeof(T).GetField(fieldName);
            var tooltipAttribute = (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
            return (tooltipAttribute == null ? new GUIContent(displayName) : new GUIContent(displayName, tooltipAttribute.tooltip));
        }

        public static void AddHeader<T>(string fieldName, string displayOverride = null)
        {
            var displayName = (displayOverride != null ? displayOverride : ObjectNames.NicifyVariableName(fieldName));
            var fieldInfo = typeof(T).GetField(fieldName);
            var headerAttribute = (HeaderAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(HeaderAttribute));
            AddHeader(headerAttribute == null ? displayName : headerAttribute.header);
        }

        public static void AddHeader(string header, bool spaceBeforeHeader = true)
        {
            if (spaceBeforeHeader)
            {
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
        }

        public static GUIStyle CreateStyle(GUIStyle styleType, Color contentColor, Color backgroundColor)
        {
            GUIStyle generatedStyle = new GUIStyle(styleType);
            generatedStyle.normal.textColor = contentColor;
            Texture2D backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(1, 1, backgroundColor);
            backgroundTexture.Apply();
            generatedStyle.normal.background = backgroundTexture;

            return generatedStyle;
        }
    }
}