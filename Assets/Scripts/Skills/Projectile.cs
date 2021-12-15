using UnityEngine;
using System.Collections;

public class Projectile : SkillInstance {
	public float speed;
	public Rigidbody rigidBody;
	public Collider collision;
	public ParticleSystem particles;
	public GameObject explosion;

	void OnEnable() {
		rigidBody.AddRelativeForce(Vector3.forward * speed);
		StartCoroutine(StopAfterSeconds(3f));
	}

	private IEnumerator StopAfterSeconds(float seconds) {
		yield return new WaitForSeconds(seconds);
		Stop();
	}

	private void OnTriggerEnter(Collider other) {
		Debug.Log($"Trigger: {other.name}");
		Stop();
		GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
	}

	private void Stop() {
		particles.Stop();
		collision.enabled = false;
		rigidBody.isKinematic = true;
		Destroy(gameObject, 2f);
	}
}
