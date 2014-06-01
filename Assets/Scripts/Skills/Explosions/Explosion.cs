using UnityEngine;

public class Explosion : SkillInstance {
	public int power;
	public float radius;
	public float upwardsModifier = 3.0f;
	public bool instantSelfDestruction = false;
	
	void Awake() {
		// Disable sound on server
		if(uLink.Network.isServer && audio != null) {
			audio.enabled = false;
		}
	}
	
	// HP drain of explosions is ONLY calculated on the server, the client uses an approximate value
	void Start() {
		if(this.skill == null)
			return;
		
		power = (int)(power * this.skillStage.powerMultiplier);
		
		//LogManager.General.Log("Explosion: " + this.skillStage.powerMultiplier);
		//LogManager.General.Log("Explosion Power: " + power);
		
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius, this.caster.enemiesLayerMask);
		
		foreach(Collider hit in colliders) {
			//if(!hit)
			//	continue;
			
			// TODO: We can remove this if we don't need physics objects on the map
			// Real explosion in the physics engine
			if(hit.tag != "Player" && hit.rigidbody != null) { // uLink.Network.isServer && 
				hit.rigidbody.AddExplosionForce(power * 3, transform.position, radius * 1.5f, upwardsModifier);
			}
			
			// Lose health
			Entity entity = hit.GetComponent<Entity>();
			if(entity != null) {
				Entity.ApplyDamage(entity, this, power);
			}
		}
		
		// Self destruct
		if(instantSelfDestruction)
			Destroy(this.gameObject);
	}
}
