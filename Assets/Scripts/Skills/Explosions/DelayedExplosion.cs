using UnityEngine;
using System.Collections;

public class DelayedExplosion : SkillInstance {
	public Transform explosionPrefab;
	public float delayTime; 
	
	void Start() {
		Invoke("Explode", delayTime);
	}
	
	void Explode() {
		if(this.caster)
			this.caster.SpawnExplosion(explosionPrefab, transform.position, transform.rotation, this);
	}
}
