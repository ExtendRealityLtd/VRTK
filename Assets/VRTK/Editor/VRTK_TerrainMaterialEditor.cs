namespace VRTK
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;

	[CustomEditor(typeof(VRTK_TerrainAudioMaterial))]
	public class VRTK_TerrainMaterialEditor : Editor
	{
		List<Texture2D> textures;
		SerializedProperty audioMats;

		private void OnEnable()
        {
			TerrainData terrainData = Terrain.activeTerrain.terrainData;
			audioMats = serializedObject.FindProperty("audioMats");
			textures = new List<Texture2D>();
			foreach(SplatPrototype tex in terrainData.splatPrototypes)
			{
				textures.Add(tex.texture);
			}
        }

		public override void OnInspectorGUI()
        {
            serializedObject.Update();

			Rect pos = (EditorGUILayout.GetControlRect(GUILayout.Height(60 * textures.Count + 10), GUILayout.Width(50)));

            for (int i = 0; i < textures.Count; i++)
            {
				EditorGUILayout.BeginHorizontal();
				EditorGUI.DrawPreviewTexture(new Rect(pos.x, pos.y + i * 60 + 10, 50, 50), textures[i]);
				EditorGUI.ObjectField(new Rect(pos.x + -71, pos.y + i * 60 + 25, 410, 17), audioMats.GetArrayElementAtIndex(i));
				EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }
	}
}
