using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(StrangeTerrain))]
public class StrangeTerrainInspector : Editor {
	
	StrangeTerrain myTarget;
	
	void Awake () {
		myTarget = target as StrangeTerrain;
	}
	
	public override void OnInspectorGUI () {
		if (GUILayout.Button("Refresh Terrain")) {
			myTarget.RefreshMesh();
		}
	}
}
