namespace VRTK
{
	using UnityEngine;

	public class VRTK_TerrainAudioMaterial : MonoBehaviour
	{
		public VRTK_AudioMaterialData[] audioMats;

		private Terrain terrain;

		private void Awake()
		{
			terrain = GetComponent<Terrain>();
		}

		public VRTK_AudioMaterialData GetAudioMaterialData(Vector3 position)
		{
			Vector3 terrPos = terrain.GetPosition();
			TerrainData terrainData = terrain.terrainData;

			int terrX = (int)(((position.x - terrPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
			int terrZ = (int)(((position.z - terrPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
			float[,,] splatData = terrainData.GetAlphamaps(terrX, terrZ, 1, 1 );
			int index = 0;
			float maxSplat = splatData[0, 0, 0];

			for (int i = 1; i < terrainData.splatPrototypes.Length; i++)
			{
				if(splatData[0, 0, i] > maxSplat)
				{
					index = i;
					maxSplat = splatData[0, 0, i];
				}
			}
			return audioMats[index];
		}
	}
}
