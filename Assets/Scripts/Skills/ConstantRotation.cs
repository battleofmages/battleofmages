using UnityEngine;
using System.Collections;

public class ConstantRotation : MonoBehaviour {
	public float rotationSpeed;
	public Space relativeTo = Space.Self;
	public Vector3 axis = Vector3.up;
	
	private Transform myTransform;

	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.Rotate(axis, rotationSpeed * Time.deltaTime, relativeTo);
	}
}
