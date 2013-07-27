using UnityEngine;
using System.Collections;

public class StrangeTerrain : MonoBehaviour
{
	[SerializeField]
	private StrangeTerrainTile[,] tiles;
	[SerializeField]
	private int width;
	[SerializeField]
	private int height;
	[SerializeField]
	private float tileSize;
	[SerializeField]
	private TileTexturer tileTexturer;
	private MeshFilter _myMeshFilter;
	private MeshCollider _myMeshCollider;
	
	private MeshFilter MyMeshFilter {
		get {
			if (_myMeshFilter == null) {
				_myMeshFilter = GetComponent<MeshFilter> ();
			}
			return _myMeshFilter;
		}
	}

	private MeshCollider MyMeshCollider {
		get {
			if (_myMeshCollider == null) {
				_myMeshCollider = GetComponent<MeshCollider> ();
			}
			return _myMeshCollider;
		}
	}
	
	public StrangeTerrainTile[,] Tiles {
		get {
			return tiles;
		}
	}

	// Use this for initialization
	void Start ()
	{
		RefreshMesh ();
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
		
		// Set Material
		renderer.material = tileTexturer.MyMaterial;
		
		// CreateMesh
		if (MyMeshFilter.sharedMesh != null) {
			if (Application.isPlaying) {
				Destroy (MyMeshFilter.sharedMesh);
			} else {
				DestroyImmediate (MyMeshFilter.sharedMesh);
			}
		}
		MyMeshFilter.sharedMesh = CreateMesh ();
		
		// Collider
		MyMeshCollider.sharedMesh = MyMeshFilter.sharedMesh;
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
				
				TextureTile tt = tileTexturer.TextureTiles [tiles [i, j].textureIndex];
				
				uvs [firstVertIndex] = tt.uv0;
				uvs [firstVertIndex + 1] = tt.uv1;
				uvs [firstVertIndex + 2] = tt.uv2;
				uvs [firstVertIndex + 3] = tt.uv3;
				
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
	
	public Vector3 GetTileCenterAtPoint (Vector3 point)
	{
		Vector3 ret = new Vector3 ();
		
		// If width or height are even halftile offset should be added to center of tile.
		// If width or height are odd halftile offset should be added to target point
		bool isXEven = (width % 2) == 0;
		bool isYEven = (height % 2) == 0;
		
		float halfTileOffsetX = point.x > 0 ? 0.5f : -0.5f;
		float halfTileOffsetZ = point.z > 0 ? 0.5f : -0.5f;
		
		int tileXIndex = (int)(((isXEven ? 0 : halfTileOffsetX) * tileSize + point.x) / tileSize);
		int tileYIndex = (int)(((isYEven ? 0 : halfTileOffsetZ) * tileSize + point.z) / tileSize);
		
		ret = new Vector3 (
			(tileXIndex + (isXEven ? halfTileOffsetX : 0)) * tileSize,
			0f,
			(tileYIndex + (isYEven ? halfTileOffsetZ : 0)) * tileSize
		);
		Debug.Log (point + " " + ret);
		return ret;
	}
}
