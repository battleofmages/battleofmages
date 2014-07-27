using UnityEngine;

public enum ProjectileSpawnDirectionType {
	Down,
	Up,
	Blizzard
}

public class Blizzard : SkillInstance {
	public GameObject projectileSpawned;
	public float radius = 10.0f;
	public float interval = 0.2f;
	public float duration = 7.0f;
	public ProjectileSpawnDirectionType spawnDirectionType;
	public bool destroyAfterDuration;
	//public float offset = 1f;
	
	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnIceball", interval, interval);
		
		if(destroyAfterDuration)
			Invoke("Stop", duration);
	}
	
	// SpawnIceball
	void SpawnIceball() {
		// Spawn one ice ball
		float angle = Random.Range(0f, 360f);
		
		// We use -radius to make 0 the median
		float currentRadius = Random.Range(-radius, radius);
		
		Vector3 offset = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(0f, 0f, currentRadius);
		//Vector3 direction = new Vector3(offset.x * damping, -radius, offset.z * damping);
		Vector3 direction;
		
		switch(spawnDirectionType) {
			case ProjectileSpawnDirectionType.Blizzard:
				direction = new Vector3(Random.Range(-radius, radius), -radius, Random.Range(-radius, radius));
				break;
			
			case ProjectileSpawnDirectionType.Down:
				direction = Vector3.down;
				break;
				
			case ProjectileSpawnDirectionType.Up:
				direction = Vector3.up;
				break;
				
			default:
				direction = Vector3.zero;
				break;
		}
		
		GameObject clone;
		SkillInstance inst;
		
		SpawnSkillPrefab(
			projectileSpawned,
			transform.position + offset,
			Quaternion.LookRotation(direction),
			out clone,
			out inst
		);
		
		inst.hitPoint = this.hitPoint;
		
		// Just to be safe
		Destroy(clone, 3.0f);
	}
	
	// Stop
	void Stop() {
		SkillInstance.DestroyButKeepParticles(this.gameObject);
	}
}
