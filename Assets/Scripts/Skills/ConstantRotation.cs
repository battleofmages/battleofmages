using UnityEngine;
using System.Collections;

public class ConstantRotation : MonoBehaviour {
	public float rotationSpeed;
	public Space relativeTo = Space.Self;
	public Vector3 axis = Vector3.up;
	
	private Transform myTransform;

	// Start
	void Start() {
		myTransform = transform;
	}
	
	// Update
	void Update() {
		myTransform.Rotate(axis, rotationSpeed * Time.deltaTime, relativeTo);
	}
}
