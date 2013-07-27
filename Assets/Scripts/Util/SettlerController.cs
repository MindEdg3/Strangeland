using UnityEngine;
using System.Collections;

public class SettlerController : MonoBehaviour
{
	private Entity _myEntity;
	private StrangeTerrain _myStrangeTerrain;
	
	// Use this for initialization
	void Start ()
	{
		_myEntity = GetComponent<Entity> ();
		_myStrangeTerrain = GameObject.Find ("StrangeTerrain").GetComponent<StrangeTerrain> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit, 100f, 1 << LayerMask.NameToLayer ("StrangeTerrain"))) {
				_myEntity.MoveTo (_myStrangeTerrain.GetTileCenterAtPoint (hit.point));
			}
		}
	}
}
