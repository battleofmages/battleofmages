using UnityEngine;

public class Explosion : SkillInstance {
	public int power;
	public float radius;
	public float upwardsModifier = 3.0f;
	public bool instantSelfDestruction = false;

	// Awake
	void Awake() {
		// Disable sound on server
		if(GameManager.isServer && audio != null) {
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
		
		foreach(Collider coll in colliders) {
			var entity = coll.GetComponent<Entity>();

			// Is it an entity?
			if(entity != null) {
				// Ignore caster
				if(entity == caster)
					return;

				// Ignore own party
				if(caster.party != null && entity.party == caster.party)
					return;

				// Lose health
				Entity.ApplyDamage(entity, this, power);
			}
			
			// TODO: We can remove this if we don't need physics objects on the map
			// Real explosion in the physics engine
			if(coll.tag != "Player" && coll.rigidbody != null) { // uLink.Network.isServer && 
				coll.rigidbody.AddExplosionForce(power * 3, transform.position, radius * 1.5f, upwardsModifier);
			}
		}
		
		// Self destruct
		if(instantSelfDestruction)
			Destroy(this.gameObject);
	}
}
