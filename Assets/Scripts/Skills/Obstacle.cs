using UnityEngine;

public class Obstacle : SkillInstance {
	public float duration = 3.0f;
	public bool friendlyFire = true;
	public Transform spawnExplosion;
	public Vector3 explosionOffsetPosition = Vector3.up;
	
	// Start
	void Start() {
		Destroy(gameObject, duration);
		
		if(friendlyFire) {
			gameObject.layer = 1;
		}
		
		if(spawnExplosion != null) {
			caster.SpawnExplosion(spawnExplosion, transform.position + explosionOffsetPosition, transform.rotation, this);
		}
	}
}
