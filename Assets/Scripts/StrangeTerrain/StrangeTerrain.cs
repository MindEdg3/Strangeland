using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class StrangeTerrain : MonoBehaviour
{
	[SerializeField]
	private StrangeTerrainTile[,] tiles;
	[SerializeField]
	private int width;
	
	public int Width {
		get {
			return width;
		}
	}
	
	[SerializeField]
	private int height;
	
	public int Height {
		get {
			return height;
		}
	}
	
	[SerializeField]
	private float tileSize;
	[SerializeField]
	private TileTexturer _tileTexturer;
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
	
	public TileTexturer TileTexturer {
		get {
			return _tileTexturer;
		}
		set {
			_tileTexturer = value;
		}
	}
	
	public float TileSize {
		set {
			tileSize = value;
		}
		get {
			return tileSize;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		if (MyMeshFilter.sharedMesh == null) {
			RefreshMesh ();
		}
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
		renderer.material = _tileTexturer.MyMaterial;
		
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
				
				TextureTile tt = _tileTexturer.TextureTiles [tiles [i, j].textureIndex];
				
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
	public Vector3[] GetVerticesAtTile (int horIndex, int vertIndex)
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
	
	/// <summary>
	/// Searches for a tile that contains the point. Can retrieve indices of founded tile.
	/// </summary>
	/// <returns>
	/// Is tile exists at point.
	/// </returns>
	/// <param name='point'>
	/// Point in world. Only X and Z are used, to find closest point at Strangeterrain.
	/// </param>
	/// <param name='xIndex'>
	/// X index of founded tile.
	/// </param>
	/// <param name='yIndex'>
	/// Y index of founded tile.
	/// </param>
	public bool GetTileAtPoint (Vector3 point, out int xIndex, out int yIndex)
	{
		// TODO: Should return true when point in some tile. Otherwise false.
		bool ret = false;
		
		// If width or height are even halftile offset should be added to center of tile.
		// If width or height are odd halftile offset should be added to target point
		bool isXEven = (width % 2) == 0;
		bool isYEven = (height % 2) == 0;
		
		float halfTileOffsetX = point.x > 0 ? 0.5f : -0.5f;
		float halfTileOffsetZ = point.z > 0 ? 0.5f : -0.5f;
		
		xIndex = (int)(((isXEven ? 0 : halfTileOffsetX) * tileSize + point.x) / tileSize);
		yIndex = (int)(((isYEven ? 0 : halfTileOffsetZ) * tileSize + point.z) / tileSize);
		
		return ret;
	}
	
	/// <summary>
	/// Gets the center of a tile closest to point.
	/// </summary>
	/// <returns>
	/// Position in center of a tile closest to point.
	/// </returns>
	/// <param name='point'>
	/// Point in world. Only X and Z are used, to find closest point at Strangeterrain.
	/// </param>
	public Vector3 GetTileCenterAtPoint (Vector3 point)
	{
		Vector3 ret = new Vector3 ();
		
		// If width or height are even halftile offset should be added to center of tile.
		// If width or height are odd halftile offset should be added to target point
		bool isXEven = (width % 2) == 0;
		bool isYEven = (height % 2) == 0;
		
		float halfTileOffsetX = point.x > 0 ? 0.5f : -0.5f;
		float halfTileOffsetZ = point.z > 0 ? 0.5f : -0.5f;
		
		int tileXIndex, tileYIndex;
		GetTileAtPoint (point, out tileXIndex, out tileYIndex);
		
		ret = new Vector3 (
			(tileXIndex + (isXEven ? halfTileOffsetX : 0)) * tileSize,
			0f,
			(tileYIndex + (isYEven ? halfTileOffsetZ : 0)) * tileSize
		);
		
		return ret;
	}
}
