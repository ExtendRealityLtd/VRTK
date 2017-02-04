namespace VRTK
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [CustomEditor(typeof(VRTK_CanvasKeyLayoutRenderer), true)]
    public class VRTK_CanvasKeyLayoutRendererEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VRTK_CanvasKeyLayoutRenderer renderer = (VRTK_CanvasKeyLayoutRenderer)target;

            if (renderer.keyTemplate == null)
            {
                EditorGUILayout.HelpBox("A Key Template is required", MessageType.Error);
            }

            DrawDefaultInspector();
        }
    }
}
