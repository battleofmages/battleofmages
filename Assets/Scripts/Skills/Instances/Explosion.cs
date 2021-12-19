using UnityEngine;

namespace BoM.Skills {
	public class Explosion : Instance {
		public ParticleSystem particles;
		private float time;

		public override void Init() {
			time = 0f;
			particles.Play();
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
