using UnityEngine;
using System.Collections;

public class ConstantScale : MonoBehaviour {
	public float scaleSpeed;
	
	private Transform myTransform;
	
	// Use this for initialization
	void Start () {
		myTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		float offset = scaleSpeed * Time.deltaTime;
		
		myTransform.localScale = new Vector3(
			myTransform.localScale.x + offset,
			myTransform.localScale.y + offset,
			myTransform.localScale.z + offset
		);
	}
}
