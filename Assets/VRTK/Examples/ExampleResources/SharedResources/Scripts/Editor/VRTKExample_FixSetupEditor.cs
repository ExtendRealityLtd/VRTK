namespace VRTK.Examples.Utilities
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(VRTKExample_FixSetup))]
    public class VRTKExample_FixSetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            VRTKExample_FixSetup myScript = (VRTKExample_FixSetup)target;
            if (GUILayout.Button("Fix SDK Setups"))
            {
                myScript.ApplyFixes();
            }
        }
    }
}