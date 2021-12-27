using BoM.Core;
using UnityEngine;
using UnityEngine.VFX;

namespace BoM.Skills {
	public class Explosion : Instance {
		public float damage { get; set; }
		public float radius;
		public VisualEffect particles;
		public Teams.Manager teamsManager;
		private Collider[] colliders;
		private float time;

		private void Awake() {
			colliders = new Collider[16];
		}

		public override void Init() {
			time = 0f;
			particles.Play();
			var team = teamsManager.teams[caster.TeamId];

			ExplosionDamage(transform.localPosition, radius, teamsManager.GetEnemyTeamsLayerMask(team));
		}

		private void ExplosionDamage(Vector3 center, float radius, int layerMask) {
			int numColliders = Physics.OverlapSphereNonAlloc(center, radius, colliders, layerMask);

			for(int i = 0; i < numColliders; i++) {
				var health = colliders[i].GetComponent<IHealth>();
				health.TakeDamage(damage, skill, caster);
			}
		}

		private void Update() {
			time += Time.deltaTime;

			if(time > 1f) {
				particles.Stop();
				Release();
			}
		}
	}
}
