using System;
using UnityEngine;

namespace BoM.UI {
	// Data
	public class FaderData : MonoBehaviour {
		public bool IsReversed { get; protected set; }
		protected float time;
		protected float progress;
		protected float duration;
		protected float durationInverse;
	}

	// Logic
	public class Fader : FaderData {
		public Action<float> onFade;
		public Action onFadeEnd;

		public float Duration {
			get {
				return duration;
			}

			set {
				duration = value;
				durationInverse = 1f / value;
			}
		}

		private void Update() {
			time += Time.deltaTime;
			progress = time * durationInverse;

			if(progress >= 1f) {
				if(IsReversed) {
					onFade(0f);
				} else {
					onFade(1f);
				}

				onFadeEnd?.Invoke();
				Stop();
				return;
			}

			if(IsReversed) {
				onFade(1f - progress);
			} else {
				onFade(progress);
			}
		}

		public void Stop() {
			Destroy(this);
		}

		public void Reverse() {
			IsReversed = !IsReversed;
			progress = 1f - progress;
		}

		public void Restart() {
			progress = 0f;
		}
	}
}
