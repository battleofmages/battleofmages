using UnityEngine;

namespace BoM.Skills {
	public class Explosion : Instance {
		public float radius;
		public ParticleSystem particles;
		private Collider[] colliders;
		private float time;
		private int enemyTeamLayer;

		private void Awake() {
			colliders = new Collider[16];
		}

		public override void Init() {
			time = 0f;
			particles.Play();
			var team = Teams.Manager.Teams[caster.TeamId];

			ExplosionDamage(transform.position, radius, team.enemyTeamLayerMask);
		}

		private void ExplosionDamage(Vector3 center, float radius, int layerMask) {
			int numColliders = Physics.OverlapSphereNonAlloc(center, radius, colliders, layerMask);

			for (int i = 0; i < numColliders; i++) {
				colliders[i].SendMessage("AddDamage");
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
