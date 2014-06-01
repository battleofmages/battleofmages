using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {
	public Transform target;
	
	private Transform myTransform;
	
	// Use this for initialization
	void Start () {
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		myTransform.LookAt(target, target.up);
	}
}
