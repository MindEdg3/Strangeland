using UnityEngine;
using System.Collections;

public class StrangeTerrain : MonoBehaviour
{
	//[SerializeField]
	private StrangeTerrainTile[,] tiles;
	//[SerializeField]
	private int width = 64;
	//[SerializeField]
	private int height = 64;
	//[SerializeField]
	private float tileSize = 2f;
	
	public StrangeTerrainTile[,] Tiles {
		get {
			return tiles;
		}
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void RefreshMesh ()
	{
		// Create data array
		if (tiles == null || tiles.GetLength (0) != width || tiles.GetLength (0) != height) {
			tiles = new StrangeTerrainTile[width, height];
		}
		
		// Fill array with empty tiles
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				tiles [i, j] = new StrangeTerrainTile ();
			}
		}
		
		// CreateMesh
		GetComponent<MeshFilter> ().mesh = CreateMesh ();
	}
	
	public Mesh CreateMesh ()
	{
		// new Mesh creating
		Mesh newMesh = new Mesh ();
		
#if UNITY_EDITOR
		newMesh.hideFlags = HideFlags.DontSave;
#endif

		Vector3[] vertices = new Vector3[4 * width * height];
		Vector2[] uvs = new Vector2[4 * width * height];
		int[] triangles = new int[6 * width * height];
		Vector3[] normals = new Vector3[4 * width * height];
		
		// Object name
		newMesh.name = "Strange Terrain";
		
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int firstVertIndex = (i * height + j) * 4;
				
				Vector3[] verticesAtTile = GetVerticesAtTile (i, j);
				
				vertices [firstVertIndex] = verticesAtTile [0];
				vertices [firstVertIndex + 1] = verticesAtTile [1];
				vertices [firstVertIndex + 2] = verticesAtTile [2];
				vertices [firstVertIndex + 3] = verticesAtTile [3];
				
				uvs [firstVertIndex] = new Vector2 (0,0);
				uvs [firstVertIndex + 1] = new Vector2 (0,1);
				uvs [firstVertIndex + 2] = new Vector2 (1,1);
				uvs [firstVertIndex + 3] = new Vector2 (1,0);
				
				int firstTriangleIndex = (i * height + j) * 6;
				
				triangles [firstTriangleIndex] = 0 + (i * height + j) * 4;
				triangles [firstTriangleIndex + 1] = 1 + (i * height + j) * 4;
				triangles [firstTriangleIndex + 2] = 2 + (i * height + j) * 4;
				triangles [firstTriangleIndex + 3] = 0 + (i * height + j) * 4;
				triangles [firstTriangleIndex + 4] = 2 + (i * height + j) * 4;
				triangles [firstTriangleIndex + 5] = 3 + (i * height + j) * 4;
				
				normals [firstVertIndex] = Vector3.up;
				normals [firstVertIndex + 1] = Vector3.up;
				normals [firstVertIndex + 2] = Vector3.up;
				normals [firstVertIndex + 3] = Vector3.up;
			}
		}
		
		newMesh.vertices = vertices;
		newMesh.uv = uvs;
		/*
		newMesh.uv = new Vector2[] {
				spriteBoundaries.textureOffset,
				new Vector2 (spriteBoundaries.textureOffset.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureTiling.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureTiling.x, spriteBoundaries.textureOffset.y + spriteBoundaries.textureTiling.y),
				new Vector2 (spriteBoundaries.textureOffset.x + spriteBoundaries.textureTiling.x, spriteBoundaries.textureOffset.y)
		};
		*/
		newMesh.triangles = triangles;
		newMesh.normals = normals;

		return newMesh;
	}
	
	/// <summary>
	/// Gets the vertices of tile specified by in chunk.
	/// </summary>
	/// <returns>
	/// The vertices of tile.
	/// </returns>
	/// <param name='horIndex'>
	/// Horizontal index.
	/// </param>
	/// <param name='vertIndex'>
	/// Vertical index.
	/// </param>
	private Vector3[] GetVerticesAtTile (int horIndex, int vertIndex)
	{
		Vector3[] ret;
		
		ret = new Vector3[] {
				new Vector3 (
					-1 * tileSize * width * 0.5f + (horIndex * tileSize),
					0f,
					-1 * tileSize * height * 0.5f + (vertIndex * tileSize)
				),
				new Vector3 (
					-1 * tileSize * width * 0.5f + (horIndex * tileSize),
					0f,
					-1 * tileSize * height * 0.5f + ((1 + vertIndex) * tileSize)
				),
				new Vector3 (
					-1 * tileSize * width * 0.5f + ((1 + horIndex) * tileSize),
					0f,
					-1 * tileSize * height * 0.5f + ((1 + vertIndex) * tileSize)
				),
				new Vector3 (
					-1 * tileSize * width * 0.5f + ((1 + horIndex) * tileSize),
					0f,
					-1 * tileSize * height * 0.5f + (vertIndex * tileSize)
				)
		};
		
		return ret;
	}
}