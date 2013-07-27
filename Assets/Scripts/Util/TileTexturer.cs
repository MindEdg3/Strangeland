using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tile texturer component. Provides access for user created TextureTiles.
/// </summary>
public class TileTexturer : MonoBehaviour
{
	/// <summary>
	/// Texture that will be used for tiles.
	/// </summary>
	public Texture2D texture;
	/// <summary>
	/// The list storing texture tiles.
	/// </summary>
	[SerializeField]
	private List<TextureTile> textureTiles;
	private Material _myMaterial;
	
	/// <summary>
	/// Gets material generated for this TileTexturer.
	/// </summary>
	/// <value>
	/// Material with texture.
	/// </value>
	public Material MyMaterial {
		get {
			if (_myMaterial == null) {
				Shader shader = Shader.Find ("Diffuse");
				_myMaterial = new Material (shader);
				_myMaterial.mainTexture = texture;
#if UNITY_EDITOR
				_myMaterial.hideFlags = HideFlags.DontSave;
#endif
			}
			return _myMaterial;
		}
	}
	
	/// <summary>
	/// Gets the list storing texture tiles.
	/// </summary>
	/// <value>
	/// The texture tiles.
	/// </value>
	public List<TextureTile> TextureTiles {
		get {
			if (this.textureTiles == null) {
				this.textureTiles = new List<TextureTile> ();
			}
			return this.textureTiles;
		}
	}
	
	public Texture2D GetTileAsTexture (int id)
	{
		Texture2D ret;
		TextureTile tt = TextureTiles [id];
		
		int width = (int)(texture.width * (tt.uv3.x - tt.uv0.x));
		int height = (int)(texture.height * (tt.uv1.y - tt.uv0.y));
		
		Color[] colors = texture.GetPixels ((int)(texture.width * tt.uv0.x), (int)(texture.height * tt.uv0.y), width, height);
		ret = new Texture2D (width, height, TextureFormat.RGB24, false);
		ret.SetPixels (colors);
		ret.Apply (false);
		ret.hideFlags = HideFlags.HideAndDontSave;
		
		return ret;
	}
}

/// <summary>
/// Texture tile.
/// </summary>
[System.Serializable]
public class TextureTile
{
	public string name;
	/// <summary>
	/// Left-bottom vertice UV. This value must not be edited manually!
	/// </summary>
	public Vector2 uv0;
	/// <summary>
	/// Left-top vertice UV. This value must not be edited manually!
	/// </summary>
	public Vector2 uv1;
	/// <summary>
	/// Rigth-top vertice UV. This value must not be edited manually!
	/// </summary>
	public Vector2 uv2;
	/// <summary>
	/// Right-bottom vertice UV. This value must not be edited manually!
	/// </summary>
	public Vector2 uv3;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="TextureTile"/> class.
	/// </summary>
	/// <param name='name'>
	/// Name.
	/// </param>
	/// <param name='uv0'>
	/// Left-bottom vertice UV.
	/// </param>
	/// <param name='uv1'>
	/// Left-top vertice UV.
	/// </param>
	/// <param name='uv2'>
	/// Rigth-top vertice UV.
	/// </param>
	/// <param name='uv3'>
	/// Right-bottom vertice UV.
	/// </param>
	public TextureTile (string name, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
	{
		this.name = name;
		this.uv0 = uv0;
		this.uv1 = uv1;
		this.uv2 = uv2;
		this.uv3 = uv3;
	}
	
	/// <summary>
	/// Gets or sets the texture offset of this tile, converting to/from UVs.
	/// </summary>
	/// <value>
	/// The tile offset.
	/// </value>
	public Vector2 TileOffset {
		get {
			return new Vector2 (uv0.x, uv0.y);
		}
		set {
			float width = uv3.x - uv0.x;
			float height = uv1.y - uv0.y;
			
			uv0 = new Vector2 (value.x, value.y);
			uv1 = new Vector2 (value.x, height + value.y);
			uv2 = new Vector2 (width + value.x, height + value.y);
			uv3 = new Vector2 (width + value.x, value.y);
		}
	}
	
	/// <summary>
	/// Gets or sets the texture tiling of this tile, converting to/from UVs.
	/// </summary>
	/// <value>
	/// The tile tiling.
	/// </value>
	public Vector2 TileTiling {
		get {
			return new Vector2 (uv3.x - uv0.x, uv1.y - uv0.y);
		}
		set {
			uv1 = new Vector2 (uv0.x, uv0.y + value.y);
			uv2 = new Vector2 (uv0.x + value.x, uv0.y + value.y);
			uv3 = new Vector2 (uv0.x + value.x, uv0.y);
		}
	}
}