using UnityEngine;
using System.Collections;

public class RepeatingExplosions : SkillInstance {
	public Transform explosionPrefab;
	public float delayTime;
	public float destroyAfter = -1f;
	
	void Start() {
		InvokeRepeating("Explode", 0.001f, delayTime);
		
		if(destroyAfter >= 0f)
			Destroy(this.gameObject, destroyAfter);
	}
	
	void Explode() {
		if(this.caster)
			this.caster.SpawnExplosion(explosionPrefab, transform.position, transform.rotation, this);
	}
}
