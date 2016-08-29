using UnityEngine;
using UnityEditor;
using VRTK;

[CustomEditor(typeof(VRTK_WorldPointer), true)]
public class VRTK_WorldPointerEditor : Editor
{
    public new VRTK_WorldPointer target { get { return base.target as VRTK_WorldPointer; } }

    private bool _showLayers;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        _showLayers = EditorGUILayout.Foldout(_showLayers, "NavMesh Layers");

        if (_showLayers)
        {
            var currentAreas = this.target.NavMeshLayerMask;
            var areas = GameObjectUtility.GetNavMeshAreaNames();
            for (int i = 0; i < areas.Length; i++)
            {
                var selected = (currentAreas & (1 << i)) != 0;
                if (EditorGUILayout.Toggle(areas[i], selected))
                    this.target.NavMeshLayerMask |= (1 << i);
                else
                    this.target.NavMeshLayerMask &= ~(1 << i);
            }
        }
    }
}
