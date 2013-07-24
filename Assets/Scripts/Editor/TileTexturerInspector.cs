using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// TileTexturer inspector editor.
/// </summary>
[CustomEditor(typeof(TileTexturer))]
public class TileTexturerInspector : Editor
{
	private Color nativeColor;
	private int _selectedTextureTile;
	private TileTexturer _myTileTexturer;
	
	#region Access Properties
	/// <summary>
	/// Gets target tile texturer.
	/// </summary>
	/// <value>
	/// Target tile texturer.
	/// </value>
	private TileTexturer MyTileTexturer {
		get {
			if (_myTileTexturer == null) {
				_myTileTexturer = target as TileTexturer;
			}
			return _myTileTexturer;
		}
	}
	
	/// <summary>
	/// Gets or sets the target texture.
	/// </summary>
	/// <value>
	/// The target texture.
	/// </value>
	private Texture2D TargetTexture {
		get {
			return MyTileTexturer.texture;
		}
		set {
			if (MyTileTexturer.texture != value) {
				Undo.RegisterUndo (MyTileTexturer, "Modify TileTexturer Texture");
				MyTileTexturer.texture = value;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the selected tile name, registering undo.
	/// </summary>
	/// <value>
	/// The selected tile name.
	/// </value>
	private string Name {
		get {
			return MyTileTexturer.TextureTiles [_selectedTextureTile].name;
		}
		set {
			if (MyTileTexturer.TextureTiles [_selectedTextureTile].name != value) {
				Undo.RegisterUndo (MyTileTexturer, "Modify Tile Name");
				MyTileTexturer.TextureTiles [_selectedTextureTile].name = value;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the selected tile offset, registering undo.
	/// </summary>
	/// <value>
	/// The selected tile offset.
	/// </value>
	private Vector2 TileOffset {
		get {
			return MyTileTexturer.TextureTiles [_selectedTextureTile].TileOffset;
		}
		set {
			if (MyTileTexturer.TextureTiles [_selectedTextureTile].TileOffset != value) {
				Undo.RegisterUndo (MyTileTexturer, "Modify Tile Offset");
				MyTileTexturer.TextureTiles [_selectedTextureTile].TileOffset = value;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the selected tile tiling, registering undo.
	/// </summary>
	/// <value>
	/// The selected tile tiling.
	/// </value>
	private Vector2 TileTiling {
		get {
			return MyTileTexturer.TextureTiles [_selectedTextureTile].TileTiling;
		}
		set {
			if (MyTileTexturer.TextureTiles [_selectedTextureTile].TileTiling != value) {
				Undo.RegisterUndo (MyTileTexturer, "Modify Tile Tiling");
				MyTileTexturer.TextureTiles [_selectedTextureTile].TileTiling = value;
			}
		}
	}
	#endregion

	public override void OnInspectorGUI ()
	{
		nativeColor = GUI.color;
		
		TargetTexture = EditorGUILayout.ObjectField ("Texture", TargetTexture, typeof(Texture2D), false) as Texture2D;
		
		if (TargetTexture == null) {
			GUI.color = Color.red;
			GUILayout.Box ("Select texture first!", GUILayout.Width (Screen.width));
			GUI.color = nativeColor;
		} else {
			GUILayout.BeginHorizontal ();
			GUI.color = Color.green;
			if (GUILayout.Button ("Create new tile")) {
				Undo.RegisterUndo (MyTileTexturer, "Create New Texture Tile");
				MyTileTexturer.TextureTiles.Add (new TextureTile ("New Tile " + Random.Range (100, 1000), new Vector2 (0, 0), new Vector2 (0, 1), new Vector2 (1, 1), new Vector2 (1, 0)));
				_selectedTextureTile = MyTileTexturer.TextureTiles.Count - 1;
			}
			GUI.color = nativeColor;
		
			if (_selectedTextureTile >= MyTileTexturer.TextureTiles.Count) {
				_selectedTextureTile = MyTileTexturer.TextureTiles.Count - 1;
			}
			if (MyTileTexturer.TextureTiles.Count > 0) {
				GUI.color = Color.red;
				if (GUILayout.Button ("Remove tile")) {
					Undo.RegisterUndo (MyTileTexturer, "Remove Texture Tile");
					MyTileTexturer.TextureTiles.Remove (MyTileTexturer.TextureTiles [_selectedTextureTile]);
					if (MyTileTexturer.TextureTiles.Count == 0) {
						return;
					} else if (_selectedTextureTile == MyTileTexturer.TextureTiles.Count) {
						_selectedTextureTile = MyTileTexturer.TextureTiles.Count - 1;
					}
				} 
				GUI.color = nativeColor;
				GUILayout.EndHorizontal ();
			
				string[] tilesNames = new string[MyTileTexturer.TextureTiles.Count];
				for (int i = 0; i < tilesNames.Length; i++) {
					tilesNames [i] = MyTileTexturer.TextureTiles [i].name;
				}
				_selectedTextureTile = EditorGUILayout.Popup ("Texture", _selectedTextureTile, tilesNames);

				TextureTile tt = MyTileTexturer.TextureTiles [_selectedTextureTile];
				DrawTileProperties (tt);
				DrawTilePreview (tt);
			} else {
				GUILayout.EndHorizontal ();
			}
		}
		
		EditorUtility.SetDirty (target);
	}
	
	private void DrawTileProperties (TextureTile tt)
	{
		Name = EditorGUILayout.TextField ("Tile Name", Name);
		TileOffset = EditorGUILayout.Vector2Field ("Texture Offset", TileOffset);
		TileTiling = EditorGUILayout.Vector2Field ("Texture Tiling", TileTiling);
	}
	
	private void DrawTilePreview (TextureTile tt)
	{
		GUILayout.Space (16f);
		Rect rect = GUILayoutUtility.GetLastRect ();
			
		GUI.DrawTextureWithTexCoords (
			new Rect (
				16f,
				rect.yMin + 16f,
				Screen.width - 32f,
				Screen.width - 32f
			),
			TargetTexture,
			new Rect (
				 TileOffset.x,
				 TileOffset.y,
				 TileTiling.x,
				 TileTiling.y
			)
		);
		
		GUILayout.Space (Screen.width - 16f);
	}
}
