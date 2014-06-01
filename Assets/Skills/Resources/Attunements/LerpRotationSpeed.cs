using UnityEngine;
using System.Collections;

public class LerpRotationSpeed : MonoBehaviour {
	public float toRotationSpeed;
	public float lerpSpeed;
	
	private ConstantRotation rotY;
	private float currentRotationSpeed;
	
	// Use this for initialization
	void Start () {
		rotY = this.GetComponent<ConstantRotation>();
	}
	
	// Update is called once per frame
	void Update () {
		rotY.rotationSpeed = Mathf.Lerp(rotY.rotationSpeed, toRotationSpeed, Time.deltaTime * lerpSpeed);
		
		if(Mathf.Abs(rotY.rotationSpeed - toRotationSpeed) <= 1) {
			rotY.rotationSpeed = toRotationSpeed;
			this.enabled = false;
		}
	}
}
