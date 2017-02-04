namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [CustomEditor(typeof(VRTK_BaseKeyboardLayoutCalculator), true)]
    public class VRTK_KeyboardLayoutCalculatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VRTK_BaseKeyboardLayoutCalculator calculator = (VRTK_BaseKeyboardLayoutCalculator)target;

            if ( calculator.keyboardLayout == null )
            {
                EditorGUILayout.HelpBox("A Keyboard Layout is required", MessageType.Error);
            }

            DrawDefaultInspector();
        }
    }
}
