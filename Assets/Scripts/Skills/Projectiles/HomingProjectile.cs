using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class HomingProjectile : Projectile {
	public float rotationSpeed = 5.0f;
	public GameObject hitTarget;

	private Quaternion targetRotation;
	private float lastSqrMagnitude = Mathf.Infinity;
	private float currentSqrMagnitude;
	
	private Transform myTransform;
	private Rigidbody myRigidbody;
	private Collider myCollider;
	
	// Start
	void Start() {
		myTransform = transform;
		myRigidbody = rigidbody;
		myCollider = collider;
	}
	
	// FixedUpdate
	void FixedUpdate() {
		if(myCollider.enabled) {
			if(hitTarget != null)
				hitPoint = hitTarget.transform.position;

			Vector3 relativePos = hitPoint - myTransform.position;
			currentSqrMagnitude = relativePos.sqrMagnitude;
			
			// If our current distance is higher than the previous one we already passed the target
			if(currentSqrMagnitude <= lastSqrMagnitude)	{
				targetRotation = Quaternion.LookRotation(relativePos);
				
				myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
				myRigidbody.velocity = myTransform.forward * projectileSpeed;
				
				//LogManager.General.Log("[ ] RelPos: " + relativePos + " ---> " + currentSqrMagnitude + "  //  " + lastSqrMagnitude);
				
				lastSqrMagnitude = currentSqrMagnitude;
			} else {
				myRigidbody.velocity = Vector3.down * projectileSpeed;
				
				//LogManager.General.Log("[X] RelPos: " + relativePos + " ---> " + currentSqrMagnitude + "  //  " + lastSqrMagnitude);
			}
			
			//Debug.DrawLine(transform.position, transform.position + rigidbody.velocity, Color.white);
			//Debug.DrawLine(transform.position, this.hitPoint, Color.green);
		} else {
			myRigidbody.velocity = Cache.vector3Zero;
		}
	}
}
