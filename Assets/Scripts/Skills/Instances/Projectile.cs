using UnityEngine;

namespace BoM.Skills.Instances {
	public class Projectile : Instance {
		public Rigidbody rigidBody;
		public Collider collision;
		public Light lighting;
		public Explosion explosionPrefab;
		public float speed;

		private bool isAlive;
		private float aliveTime;
		private float deadTime;
		private float lightIntensity;

		private void Awake() {
			lightIntensity = lighting.intensity;
		}

		public override void Init() {
			aliveTime = 0f;
			deadTime = 0f;
			lighting.intensity = lightIntensity;
			Revive();
		}

		private void Revive() {
			isAlive = true;
			StartMovement();
		}

		private void Die() {
			isAlive = false;
			StopMovement();
		}

		private void StartMovement() {
			rigidBody.WakeUp();
			rigidBody.velocity = transform.forward * speed;
		}

		private void StopMovement() {
			rigidBody.Sleep();
		}

		private void Update() {
			if(isAlive) {
				AliveTimer();
			} else {
				DeadTimer();
			}
		}

		private void AliveTimer() {
			aliveTime += Time.deltaTime;

			if(aliveTime > 3f) {
				Die();
			}
		}

		private void DeadTimer() {
			deadTime += Time.deltaTime;
			lighting.intensity = Mathf.Lerp(lightIntensity, 0f, deadTime);

			if(deadTime > 1f) {
				Release();
			}
		}

		private void OnCollisionEnter(Collision other) {
			Die();
			Explode();
		}

		private void Explode() {
			var explosion = PoolManager.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			explosion.skill = skill;
			explosion.Init();
		}
	}
}
