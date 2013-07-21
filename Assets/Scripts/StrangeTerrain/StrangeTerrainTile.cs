using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StrangeTerrainTile
{
	public int textureIndex;

	public StrangeTerrainTile ()
	{
		textureIndex = Random.Range (0, 4);
	}
}
