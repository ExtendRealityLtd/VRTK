namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;

    public class VRTK_EditorUtilities : MonoBehaviour
    {
        public static GUIContent BuildGUIContent<T>(string fieldName, string displayOverride = null)
        {
            var displayName = (displayOverride != null ? displayOverride : ObjectNames.NicifyVariableName(fieldName));
            var fieldInfo = typeof(T).GetField(fieldName);
            var tooltipAttribute = (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
            return (tooltipAttribute == null ? new GUIContent(displayName) : new GUIContent(displayName, tooltipAttribute.tooltip));
        }
    }
}