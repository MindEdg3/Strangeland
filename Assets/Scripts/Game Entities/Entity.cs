using UnityEngine;
using System.Collections;

/// <summary>
/// Entity in the world.
/// </summary>
public class Entity : MonoBehaviour
{
	public float moveSpeed;
	private Vector3 _moveTarget;
	private bool _isMoving;
	private Transform _tr;

	// Use this for initialization
	void Start ()
	{
		_tr = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_isMoving) {
			Move ();
		}
	}
	
	/// <summary>
	/// Move this entity to target.
	/// </summary>
	public void Move ()
	{
		Vector3 movementToTarget = _moveTarget - _tr.position;
		Vector3 nextMove = movementToTarget.normalized * moveSpeed * Time.deltaTime;
		
		if (nextMove.sqrMagnitude > movementToTarget.sqrMagnitude) {
			_tr.position = _moveTarget;
			_isMoving = false;
		} else {
			_tr.position += nextMove;
		}
	}
	
	/// <summary>
	/// Forces entity to move to target with speed.
	/// </summary>
	/// <param name='target'>
	/// Moving target.
	/// </param>
	public void MoveTo (Vector3 target)
	{
		_moveTarget = target;
		_isMoving = true;
	}
}
