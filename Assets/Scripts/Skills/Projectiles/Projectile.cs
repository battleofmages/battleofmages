using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : SkillInstance {
	public Transform explosionPrefab;
	public float projectileSpeed;
	
	protected bool collided = false;
	
	// After the prefab has been initialized
	void OnEnable() {
		// Add force to the cloned object in the object's forward direction
		rigidbody.AddForce(transform.forward * projectileSpeed);
	}
	
	// OnCollisionEnter
	void OnCollisionEnter(Collision collision) {
		if(collided)
			return;
		
		// Melee weapon?
		if(collision.gameObject.tag == "Weapon")
			return;
		
		// Create an explosion
		if(explosionPrefab && caster != null) {
			caster.SpawnExplosion(explosionPrefab, collision, this);
		}
		
		SkillInstance.DestroyButKeepParticles(gameObject);
		//SkillInstance.StopEmitters(this.gameObject);
		//this.gameObject.SetActive(false);
		
		rigidbody.velocity = Cache.vector3Zero;
		collided = true;
		collider.enabled = false;
	}
}
