namespace VRTK
{
    using UnityEditor;

    [CustomEditor(typeof(VRTK_NavMeshData), true)]
    public class VRTK_NavMeshDataEditor : Editor
    {
        public new VRTK_NavMeshData target { get { return base.target as VRTK_NavMeshData; } }

        private SerializedProperty distanceLimit = null;
        private bool _showLayers;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(distanceLimit);

            _showLayers = EditorGUILayout.Foldout(_showLayers, VRTK_EditorUtilities.BuildGUIContent<VRTK_NavMeshData>("validAreas"));

            if (_showLayers)
            {
                EditorGUI.indentLevel++;
                int currentAreas = target.validAreas;
                string[] areas = GameObjectUtility.GetNavMeshAreaNames();
                for (int i = 0; i < areas.Length; i++)
                {
                    var selected = (currentAreas & (1 << i)) != 0;
                    if (EditorGUILayout.Toggle(areas[i], selected))
                    {
                        target.validAreas |= (1 << i);
                    }
                    else
                    {
                        target.validAreas &= ~(1 << i);
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        protected virtual void OnEnable()
        {
            distanceLimit = serializedObject.FindProperty("distanceLimit");
        }
    }
}