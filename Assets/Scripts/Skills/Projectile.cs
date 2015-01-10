using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public class Projectile : SkillInstance {
	public float projectileSpeed;
	public GameObject explosion;
	
	protected bool collided = false;
	private Rigidbody rigidBody;
	private Collider projectileCollider;
	
	// Start
	void Start() {
		rigidBody = GetComponent<Rigidbody>();
		projectileCollider = GetComponent<Collider>();

		// Ignore collision between the projectile and the caster
		Physics.IgnoreCollision(caster.motor.characterController, projectileCollider);

		// Add force to the cloned object in the object's forward direction
		rigidBody.AddForce(transform.forward * projectileSpeed);
	}
	
	// OnCollisionEnter
	void OnCollisionEnter(Collision collision) {
		if(collided)
			return;
		
		// Create an explosion
		if(explosion && caster != null) {
			caster.SpawnExplosion(explosion, collision, this);
		}

		rigidBody.velocity = Cache.vector3Zero;
		collided = true;
		projectileCollider.enabled = false;

		// Detach particles and destroy object
		SkillInstance.DetachParticles(this.gameObject);
		Destroy(this.gameObject);
	}
}