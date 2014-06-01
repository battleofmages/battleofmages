using UnityEngine;
using System.Collections;

public class InterpolateToTarget : MonoBehaviour {
	public Transform interpolationTarget;
	public float interpolationSpeed;
	
	private Transform myTransform;
	
	void Start() {
		myTransform = this.transform;
	}
	
	void Update() {
		float time = Time.deltaTime * interpolationSpeed;
		
		myTransform.position = Vector3.Lerp(myTransform.position, interpolationTarget.position, time);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, interpolationTarget.rotation, time);
	}
}
