using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class TargetPickingProjectile : Projectile {
	public float searchRadius = 50.0f;
	public float rotationSpeed = 10.0f;
	public float rotationSpeedIncrease = 15.0f;
	public float maxAngle = 10.0f;
	
	protected Transform myTransform;
	protected Rigidbody myRigidbody;
	private Quaternion targetRotation;
	private Entity target;
	private int layerMask;
	
	// Start
	void Start() {
		myTransform = transform;
		myRigidbody = rigidbody;
		
		layerMask = caster.enemiesLayerMask;
	}

	// FixedUpdate
	void FixedUpdate () {
		if(target != null || collided)
			return;
		
		float distanceSqr;
		float targetDistanceSqr = float.MaxValue;
		
		// Search for a target
		Collider[] colliders = Physics.OverlapSphere(myTransform.position, searchRadius, layerMask);
		
		foreach(Collider coll in colliders) {
			var entity = coll.GetComponent<Entity>();
			
			if(entity && entity.isAlive) {
				Vector3 targetDirection = entity.position - myTransform.position;
				distanceSqr = targetDirection.sqrMagnitude;
				
				// We don't care about targets that are even further away from us
				if(distanceSqr > targetDistanceSqr)
					continue;
				
				targetRotation = Quaternion.LookRotation(targetDirection);
				float angle = Quaternion.Angle(myTransform.rotation, targetRotation);
				
				// Only track targets in a certain angle
				if(angle > maxAngle)
					continue;
				
				target = entity;
				targetDistanceSqr = distanceSqr;
			}
		}
	}

	// Update
	void Update() {
		if(collided)
			return;
		
		if(target != null) {
			Vector3 targetPosition = target.center;
			targetRotation = Quaternion.LookRotation(targetPosition - myTransform.position);
			myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
		}
		
		myRigidbody.velocity = myTransform.forward * projectileSpeed;
		rotationSpeed += rotationSpeedIncrease * Time.deltaTime;
	}
}
