using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StrangeTerrain))]
public class StrangeTerrainInspector : Editor
{
	private int _selectedTile;
	private Vector2 _toolBarScrollPosition;
	private Vector2 _paletteBarScrollPosition;
	
#region Properties
	private ToolMode _currentToolMode = ToolMode.Move;
	
	/// <summary>
	/// Gets or sets Strangeterrain editor tool mode.
	/// </summary>
	/// <value>
	/// The current tool mode.
	/// </value>
	private ToolMode CurrentToolMode {
		get {
			return _currentToolMode;
		}
		set {
			if (_currentToolMode != value) {
				if (value == ToolMode.Move) {
					if (Tools.current == Tool.None) {
						// Restore some tool if nothing was selected
						Tools.current = Tool.Move;
					}
					// Restore mesh wireframe
					EditorUtility.SetSelectedWireframeHidden (MyTarget.renderer, false);
				} else if (value == ToolMode.TilePaint) {
					// Disable any built-in tool
					Tools.current = Tool.None;
					// Hide mesh wireframe
					EditorUtility.SetSelectedWireframeHidden (MyTarget.renderer, true);
				}
				_currentToolMode = value;
			}
		}
	}
	
	private StrangeTerrain _myTarget;
	
	/// <summary>
	/// Gets and caches inspector target.
	/// </summary>
	/// <value>
	/// Inspector target.
	/// </value>
	private StrangeTerrain MyTarget {
		get {
			if (_myTarget == null) {
				_myTarget = target as StrangeTerrain;
			}
			return _myTarget;
		}
	}
	
	/// <summary>
	/// Gets and caches the scene camera.
	/// </summary>
	/// <value>
	/// The scene camera.
	/// </value>
	private Camera SceneCamera {
		get {
			if (SceneView.lastActiveSceneView != null) {
				return SceneView.lastActiveSceneView.camera;
			} else {
				return null;
			}
		}
	}
	
	/// <summary>
	/// Gets the width of the screen.
	/// </summary>
	/// <value>
	/// The width of the screen.
	/// </value>
	private int ScenePixelWidth {
		get {
			if (SceneCamera != null) {
				return (int)SceneCamera.pixelWidth;
			} else {
				return (int)Screen.width;
			}
		}
	}
	
	/// <summary>
	/// Gets the height of the screen.
	/// </summary>
	/// <value>
	/// The height of the screen.
	/// </value>
	private int ScenePixelHeight {
		get {
			if (SceneCamera != null) {
				return (int)SceneCamera.pixelHeight;
			} else {
				return (int)Screen.height;
			}
		}
	}

	private Texture2D[] _tilesTextures;
	
	/// <summary>
	/// Gets and caches separate textures for all tiles from TileTexturer.
	/// </summary>
	/// <value>
	/// The tiles textures.
	/// </value>
	private Texture2D[] TilesTextures {
		get {
			if (_tilesTextures == null) {
				_tilesTextures = new Texture2D [MyTileTexturer.TextureTiles.Count];
				for (int i = 0; i < _tilesTextures.Length; i++) {
					_tilesTextures [i] = MyTileTexturer.GetTileAsTexture (i);
				}
			}
			return _tilesTextures;
		}
	}
	
	private TileTexturer _tileTexturer;
	
	/// <summary>
	/// Gets or sets TileTexturer of target Strangeterrain.
	/// </summary>
	/// <value>
	/// Target TileTexturer.
	/// </value>
	public TileTexturer MyTileTexturer {
		get {
			return MyTarget.TileTexturer;
		}
		set {
			MyTarget.TileTexturer = value;
		}
	}
#endregion
	
#region Menu Items
	[MenuItem("Strangeland/Create new StrangeTerrain")]
	public static void CreateNewTerrain ()
	{
		StrangeTerrain newStrangeTerrain = new GameObject ("New StrangeTerrain " + Random.Range (100, 1000), typeof(StrangeTerrain)).GetComponent<StrangeTerrain> ();
		newStrangeTerrain.tag = "StrangeTerrain";
		newStrangeTerrain.gameObject.layer = LayerMask.NameToLayer ("StrangeTerrain");
		Selection.objects = new Object[] {newStrangeTerrain.gameObject};
	}
#endregion
	
	public override void OnInspectorGUI ()
	{
		if (MyTarget.GetComponent<MeshFilter> ().sharedMesh == null) {
			MyTarget.RefreshMesh ();
		}
		
		MyTileTexturer = EditorGUILayout.ObjectField ("Tile Texturer", MyTileTexturer, typeof(TileTexturer), false) as TileTexturer;
	}
	
	public void OnSceneGUI ()
	{
		// Nothing to do if TileTexturer hasn't been set
		if (MyTileTexturer != null) {
			
			if (_currentToolMode == ToolMode.Move) {
			}
			if (_currentToolMode == ToolMode.TilePaint) {
				// Dirty trick to repaint painting cursor instantly
				HandleUtility.Repaint ();
				DrawBrush ();
			}
			
			GUI.backgroundColor = new Color (1f, 1f, 1f, 1f);
			GUI.color = new Color (1f, 1f, 1f, 1f);
			
			Handles.BeginGUI ();
			
			DrawTools ();
			DrawPallete ();
			HandleHandlesGUI ();

			Handles.EndGUI ();
		}
	}
	
	/// <summary>
	/// Draws the tools selector.
	/// </summary>
	public void DrawTools ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("\nTools: ", GUILayout.Width (50), GUILayout.Height (40));
		_toolBarScrollPosition = EditorGUILayout.BeginScrollView (_toolBarScrollPosition, GUILayout.Width (ScenePixelWidth - 50), GUILayout.Height (62));
		
		Texture[] icons = {EditorGUITools.Arrow, EditorGUITools.Brush};
		CurrentToolMode = (ToolMode)GUILayout.SelectionGrid ((int)CurrentToolMode, icons, icons.Length, GUILayout.Width (45 * icons.Length), GUILayout.Height (40));
		
		EditorGUILayout.EndScrollView ();
		GUILayout.EndHorizontal ();
	}
	
	/// <summary>
	/// Draws the pallete for brush tools.
	/// </summary>
	public void DrawPallete ()
	{
		if (_currentToolMode != ToolMode.Move) {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("\nPallete: ", GUILayout.Width (50), GUILayout.Height (40));
			_paletteBarScrollPosition = EditorGUILayout.BeginScrollView (_paletteBarScrollPosition, GUILayout.Width (ScenePixelWidth - 50), GUILayout.Height (62));
			
			if (_currentToolMode == ToolMode.TilePaint) {
				_selectedTile = GUILayout.SelectionGrid (_selectedTile, TilesTextures, TilesTextures.Length, GUILayout.Width (45 * TilesTextures.Length), GUILayout.Height (40));
			}
			
			EditorGUILayout.EndScrollView ();
			GUILayout.EndHorizontal ();
		}
	}
	
	/// <summary>
	/// Handles some GUI tasks for various tool modes.
	/// </summary>
	public void HandleHandlesGUI ()
	{
		// Check if one of Strangeterrain tools selected
		if (Tools.current == Tool.None) {
			if (CurrentToolMode == ToolMode.Move) {
			} else if (CurrentToolMode == ToolMode.TilePaint) {
				// Disable selecting with mouse
				HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
			}
		} else {
			// Otherwise reset our tools to ToolMode.Move
			CurrentToolMode = ToolMode.Move;
		}
	}
	
	/// <summary>
	/// Draws the brush for Strangeterrain painting.
	/// </summary>
	public void DrawBrush ()
	{
		float tileSize = MyTarget.TileSize;
		int width = MyTarget.Width;
		int height = MyTarget.Height;
		
		// Vertices for Strangeterrain frame
		Vector3[] vertices = {
			new Vector3 (-1 * tileSize * width * 0.5f, 0f, -1 * tileSize * height * 0.5f),
			new Vector3 (-1 * tileSize * width * 0.5f, 0f, -1 * tileSize * height * 0.5f + (height * tileSize)),
			new Vector3 (-1 * tileSize * width * 0.5f + (width * tileSize), 0f, -1 * tileSize * height * 0.5f + (height * tileSize)),
			new Vector3 (-1 * tileSize * width * 0.5f + (width * tileSize), 0f, -1 * tileSize * height * 0.5f)
		};
		
		// Strangeterrain frame
		Handles.DrawSolidRectangleWithOutline (vertices, new Color (1f, 1f, 1f, 0f), Color.red);
		
		// Raycasting from scene camera to any object
		object hit; 
		Event e = Event.current; 
		Ray ray = HandleUtility.GUIPointToWorldRay (e.mousePosition); 
		hit = HandleUtility.RaySnap (ray);
		
		// ATTENTION! There can be any object with collider
		if (hit != null) {
			int xIndex, yIndex;
			Vector3 hitPoint = ((RaycastHit)hit).point;
			
			MyTarget.GetTileAtPoint (hitPoint, out xIndex, out yIndex);
			
			bool isXEven = (width % 2) == 0;
			bool isYEven = (height % 2) == 0;
			
			float halfTileOffsetX = hitPoint.x > 0 ? 0.5f : -0.5f;
			float halfTileOffsetZ = hitPoint.z > 0 ? 0.5f : -0.5f;
			
			// Hovered tile frame
			Handles.DrawSolidRectangleWithOutline (MyTarget.GetVerticesAtTile ((int)(xIndex + width * 0.5f + (isXEven ? halfTileOffsetX : 0)), (int)(yIndex + height * 0.5f + (isYEven ? halfTileOffsetZ : 0))), new Color (1f, 1f, 1f, 0f), Color.green);
		}
	}
}

/// <summary>
/// Strangeterrain editor tool mode.
/// </summary>
public enum ToolMode
{
	Move = 0, // Does nothing
	TilePaint = 1 // Painting tiles with TileTextures
}