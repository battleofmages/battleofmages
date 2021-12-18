using UnityEngine;
using System.Collections;

namespace BoM.Skills {
	public class Projectile : Instance {
		public float speed;
		public Rigidbody rigidBody;
		public Collider collision;
		public ParticleSystem particles;
		public GameObject explosion;
		public Light lighting;
		private bool dead;
		private float deadTime;

		private void Update() {
			if(!dead) {
				return;
			}

			deadTime += Time.deltaTime;
			lighting.intensity = Mathf.Lerp(lighting.intensity, 0f, deadTime);
		}

		void OnEnable() {
			rigidBody.AddRelativeForce(Vector3.forward * speed);
			StartCoroutine(StopAfterSeconds(3f));
		}

		private IEnumerator StopAfterSeconds(float seconds) {
			yield return new WaitForSeconds(seconds);
			Stop();
		}

		private void OnCollisionEnter(Collision other) {
			Stop();
			GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
		}

		private void Stop() {
			dead = true;
			particles.Stop();
			collision.enabled = false;
			rigidBody.isKinematic = true;
			Destroy(gameObject, 2f);
		}
	}
}
