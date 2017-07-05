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

        public static void DrawUsingDestructiveStyle(GUIStyle styleToCopy, Action<GUIStyle> drawAction)
        {
            Color previousBackgroundColor = GUI.backgroundColor;
            GUIStyle destructiveButtonStyle = new GUIStyle(styleToCopy)
            {
                normal =
                {
                    textColor = Color.white
                },
                active =
                {
                    textColor = Color.white
                }
            };

            GUI.backgroundColor = Color.red;
            drawAction(destructiveButtonStyle);
            GUI.backgroundColor = previousBackgroundColor;
        }

        public static void DrawScrollableSelectableLabel(ref Vector2 scrollPosition, ref float width, string text, GUIStyle style)
        {
            using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollViewScope.scrollPosition;

                float textHeight = style.CalcHeight(new GUIContent(text), width);
                EditorGUILayout.SelectableLabel(text, style, GUILayout.MinHeight(textHeight));

                if (Event.current.type == EventType.Repaint)
                {
                    width = GUILayoutUtility.GetLastRect().width;
                }
            }
        }
    }
}