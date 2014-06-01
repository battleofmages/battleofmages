using UnityEngine;
using System.Collections;

public class AddTorque : MonoBehaviour {
	public Vector3 torque;
	
	private Rigidbody myRigidBody;
	
	// Use this for initialization
	void Start () {
		myRigidBody = this.rigidbody;
	}
	
	// Update
	void FixedUpdate() {
		myRigidBody.AddTorque(torque * Time.fixedDeltaTime);
	}
}
