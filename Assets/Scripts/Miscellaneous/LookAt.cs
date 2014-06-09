using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {
	public Transform target;
	
	private Transform myTransform;
	
	// Start
	void Start() {
		myTransform = transform;
	}
	
	// Update
	void Update() {
		myTransform.LookAt(target, target.up);
	}
}
