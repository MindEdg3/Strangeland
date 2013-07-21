using UnityEngine;
using UnityEditor;
using System.Collections;

public class StrangeTerrainEditor : EditorWindow {
	
	[MenuItem("Strangeland/Create new StrangeTerrain")]
	public static void CreateNewTerrain () {
		StrangeTerrain newStrangeTerrain = new GameObject("New StrangeTerrain " + Random.Range(100,1000), typeof(StrangeTerrain)).GetComponent<StrangeTerrain> ();
		Selection.objects = new Object[] {newStrangeTerrain.gameObject};
	}
	
	[MenuItem("Strangeland/Show Land editor")]
	public static void ShowWindow () {
		
	}
}
