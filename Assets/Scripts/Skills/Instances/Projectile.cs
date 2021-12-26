using BoM.Core;
using UnityEngine;
using UnityEngine.VFX;

namespace BoM.Skills.Instances {
	public class Projectile : Instance {
		const float finalGroundDistance = 0.15f;

		public Rigidbody rigidBody;
		public SphereCollider collision;
		public VisualEffect particles;
		public Light lighting;
		public Explosion explosionPrefab;
		public float directHitDamage;
		public float splashDamage;
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

		private void OnTriggerEnter(Collider other) {
			if(GetCollisionPoint(out RaycastHit hit)) {
				transform.localPosition = hit.point - transform.forward * finalGroundDistance;
			}

			// Direct hit damage
			if(other.tag == "Player") {
				var health = other.GetComponent<IHealth>();
				health.TakeDamage(directHitDamage, skill, caster);
			}

			Die();
			Explode();
		}

		private bool GetCollisionPoint(out RaycastHit hit) {
			Vector3 forward = transform.forward;
			Vector3 backward = -forward;
			float maxDistance = 1f;

			return Physics.Raycast(transform.localPosition + backward * maxDistance, forward, out hit, maxDistance, Physics.DefaultRaycastLayers);
		}

		private void Explode() {
			var explosion = PoolManager.Instantiate(explosionPrefab, transform.localPosition, Quaternion.identity);
			explosion.GetComponent<Explosion>().damage = splashDamage;
			explosion.transform.SetLayer(gameObject.layer);
			explosion.skill = skill;
			explosion.caster = caster;
			explosion.Init();
		}

		private void Revive() {
			isAlive = true;
			collision.enabled = true;
			rigidBody.WakeUp();
			rigidBody.velocity = transform.forward * speed;
			particles.Play();
		}

		private void Die() {
			isAlive = false;
			collision.enabled = false;
			rigidBody.Sleep();
			particles.Stop();
		}
	}
}
