using UnityEngine;
using System.Collections;

public class BindingFieldExplosion : Explosion {
	public GameObject lightningRayPrefab;
	
	// Start
	void Start() {
		if(this.skill == null)
			return;
		
		var myTransform = this.transform;
		power = (int)(power * this.skillStage.powerMultiplier);
		
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius, this.caster.enemiesLayerMask);
		
		foreach(Collider hit in colliders) {
			//if(!hit)
			//	continue;
			
			// Real explosion in the physics engine
			if(hit.tag != "Player" && hit.rigidbody != null) { // uLink.Network.isServer && 
				hit.rigidbody.Sleep();
			}
			
			// Lose health
			Entity player = hit.GetComponent<Entity>();
			if(player != null) {
				Entity.ApplyDamage(player, this, power);
				
				// Spawn prefab
				var distanceVector = player.myTransform.position - myTransform.position;
				var distance = distanceVector.magnitude;
				Quaternion rotation = Quaternion.identity;
				
				if(distance != 0f)
					rotation = Quaternion.LookRotation(distanceVector);
				
				var clone = (GameObject)GameObject.Instantiate(lightningRayPrefab, transform.position, rotation);
				var lightningRenderer = clone.GetComponent<LightningRenderer>();
				lightningRenderer.distance = distance;
				lightningRenderer.vertexCount = (int)(distance / 2);
			}
		}
		
		// Self destruct
		if(instantSelfDestruction)
			Destroy(this.gameObject);
	}
}
