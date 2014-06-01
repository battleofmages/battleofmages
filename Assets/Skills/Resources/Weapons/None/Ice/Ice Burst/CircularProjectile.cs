using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class CircularProjectile : Projectile {
	public float rotationSpeed = 180.0f;
	public float yVelocity = 0.0f;
	
	private float angle = 0.0f;
	//private Transform myTransform;
	private Rigidbody myRigidbody;
	//private Collider myCollider;
	
	// Use this for initialization
	void Start () {
		//myTransform = this.transform;
		myRigidbody = this.rigidbody;
		//myCollider = this.collider;
	}
	
	// Projectile movement
	void FixedUpdate() {
		myRigidbody.velocity = new Vector3(Mathf.Cos(angle) * projectileSpeed, yVelocity, Mathf.Sin(angle) * projectileSpeed);
		angle += rotationSpeed * Time.deltaTime;
	}
}
