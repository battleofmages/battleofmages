using UnityEngine;

namespace BoM.Skill.Instances {
	public class Projectile : Instance {
		public Rigidbody rigidBody;
		public Collider collision;
		public Light lighting;
		public Explosion explosion;
		public float speed;

		private bool isAlive;
		private float aliveTime;
		private float deadTime;
		private float lightIntensity;

		private void Awake() {
			lightIntensity = lighting.intensity;
		}

		public override void Init() {
			isAlive = true;
			aliveTime = 0f;
			deadTime = 0f;
			lighting.intensity = lightIntensity;
			rigidBody.WakeUp();
			rigidBody.velocity = transform.forward * speed;
		}

		private void Update() {
			if(isAlive) {
				AliveTimer();
			} else {
				DeadTimer();
			}
		}

		void AliveTimer() {
			aliveTime += Time.deltaTime;

			if(aliveTime > 3f) {
				Die();
			}
		}

		void DeadTimer() {
			deadTime += Time.deltaTime;
			lighting.intensity = Mathf.Lerp(lightIntensity, 0f, deadTime);

			if(deadTime > 1f) {
				Release();
			}
		}

		private void OnCollisionEnter(Collision other) {
			Die();
			var explosionPool = PoolManager.GetPool(explosion);
			var instance = explosionPool.Get();
			instance.transform.position = transform.position;
			instance.skill = skill;
			instance.pool = explosionPool;
			instance.Init();
		}

		private void Die() {
			rigidBody.Sleep();
			isAlive = false;
		}
	}
}
