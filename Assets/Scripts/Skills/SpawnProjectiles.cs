using UnityEngine;

public class SpawnProjectiles : MonoBehaviour {
	public GameObject projectile;
	public Transform spawn;
	public float radius;
	public float interval;
	public float duration;

	private void Start() {
		InvokeRepeating("SpawnProjectile", 0f, interval);
		Destroy(gameObject, duration);
	}

	void SpawnProjectile() {
		var random = Random.insideUnitCircle * radius;
		var offset = new Vector3(random.x, 0f, random.y);
		GameObject.Instantiate(projectile, spawn.position + offset, spawn.rotation);
	}
}
