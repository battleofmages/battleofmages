using UnityEngine;
using System.Collections;

public class Motor : MonoBehaviour {
	public CharacterController characterController;
	public float moveSpeed;

	[System.NonSerialized]
	private Vector3 _moveVector;

	// FixedUpdate
	void FixedUpdate() {
		characterController.Move(_moveVector * moveSpeed * Time.deltaTime);
	}

	// SetMoveVector
	public void SetMoveVector(float x, float y, float z) {
		_moveVector.x = x;
		_moveVector.y = y;
		_moveVector.z = z;

		FixMoveVector();
	}

	// FixMoveVector
	void FixMoveVector() {
		_moveVector.Normalize();
		_moveVector.y += Physics.gravity.y;
	}

#region Properties
	// Move vector
	public Vector3 moveVector {
		get {
			return _moveVector;
		}
		
		set {
			_moveVector = value;

			FixMoveVector();
		}
	}
#endregion
}
