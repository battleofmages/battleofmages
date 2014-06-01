using UnityEngine;
using System.Collections;

public class IceBurst : SkillInstance {
	public GameObject projectileSpawned;
	public GameObject burstSpawned;
	public float radius = 15.0f;
	public float duration = 7.0f;
	public float interval = 0.2f;
	public float burstTime = 2.0f;
	public float stopSpawningProjectilesTime = 1.8f;
	public float radiusIncrease = 0.0f;
	public float heightIncrease = 0.0f;
	public int angleIncrease = 18;
	
	private int angle = 0;
	private float height = 0.0f;
	private bool continueSpawningProjectiles = true;
	
	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnProjectile", interval, interval);
		Invoke("StopProjectiles", stopSpawningProjectilesTime);
		Invoke("SpawnBurst", burstTime);
		Destroy(this.gameObject, duration);
	}
	
	void StopProjectiles() {
		continueSpawningProjectiles = false;
	}
	
	void SpawnBurst() {
		this.caster.SpawnExplosion(burstSpawned.transform, this.hitPoint, Quaternion.identity, this);
	}
	
	// Spawns a projectile
	void SpawnProjectile() {
		if(!continueSpawningProjectiles)
			return;
		
		// Spawn one ice ball
		Vector3 offset = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward * radius;
		offset.y = height;
		
		Vector3 direction = new Vector3(offset.x, -radius, offset.z);
		
		GameObject clone;
		SkillInstance inst;
		
		SpawnSkillPrefab(
			projectileSpawned,
			transform.position + offset,
			Quaternion.LookRotation(direction),
			out clone,
			out inst
		);
		
		// Just to be safe
		//Destroy(clone, 1.0f);
		
		// Next iteration
		angle += angleIncrease;
		radius += radiusIncrease;
		height += heightIncrease;
	}
}
