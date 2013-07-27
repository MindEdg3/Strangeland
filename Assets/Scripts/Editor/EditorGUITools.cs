using UnityEngine;
using System.Collections;
using UnityEditor;

public static class EditorGUITools
{
	private static Texture2D _cursorArrow;
	
	public static Texture2D Arrow {
		get {
			if (_cursorArrow == null) {
				_cursorArrow = AssetDatabase.LoadAssetAtPath ("Assets" + System.IO.Path.DirectorySeparatorChar + "Resources" + System.IO.Path.DirectorySeparatorChar + "Editor" + System.IO.Path.DirectorySeparatorChar + "icon_arrow.png", typeof(Texture2D)) as Texture2D;
				_cursorArrow.hideFlags = HideFlags.HideAndDontSave;
			}
			return _cursorArrow;
		}
	}

	private static Texture2D _cursorBrush;
	
	public static Texture2D Brush {
		get {
			if (_cursorBrush == null) {
				_cursorBrush = AssetDatabase.LoadAssetAtPath ("Assets" + System.IO.Path.DirectorySeparatorChar + "Resources" + System.IO.Path.DirectorySeparatorChar + "Editor" + System.IO.Path.DirectorySeparatorChar + "icon_brush.png", typeof(Texture2D)) as Texture2D;
				_cursorBrush.hideFlags = HideFlags.HideAndDontSave;
			}
			return _cursorBrush;
		}
	}
}