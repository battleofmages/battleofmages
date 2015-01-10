using UnityEngine;

public class Explosion : SkillInstance {
	public int power;
	public float radius;
	public float upwardsModifier;
	public bool instantSelfDestruction;
	
	// Start
	void Start() {
		var colliders = Physics.OverlapSphere(transform.position, radius, this.caster.enemiesLayerMask);
		
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
				entity.ApplyDamage(this, power);
			}

			// Real explosion in the physics engine
			if(coll.tag != "Player" && coll.GetComponent<Rigidbody>() != null) {
				coll.GetComponent<Rigidbody>().AddExplosionForce(power * 3, transform.position, radius * 1.5f, upwardsModifier);
			}
		}
		
		// Self destruct
		if(instantSelfDestruction)
			Destroy(this.gameObject);
	}
}