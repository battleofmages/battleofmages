using System;
using UnityEngine;

namespace BoM.UI {
	public class Fader : MonoBehaviour {
		public float Duration {
			get {
				return duration;
			}

			set {
				duration = value;
				durationInverse = 1f / value;
			}
		}

		public bool isReversed { get; private set; }
		public Action<float> onFade;
		public Action onFadeEnd;
		private float time;
		private float progress;
		private float duration;
		private float durationInverse;

		private void Update() {
			time += Time.deltaTime;
			progress = time * durationInverse;

			if(progress >= 1f) {
				if(isReversed) {
					onFade(0f);
				} else {
					onFade(1f);
				}

				onFadeEnd?.Invoke();
				Stop();
				return;
			}

			if(isReversed) {
				onFade(1f - progress);
			} else {
				onFade(progress);
			}
		}

		public void Stop() {
			Destroy(this);
		}

		public void Reverse() {
			isReversed = !isReversed;
			progress = 1f - progress;
		}

		public void Restart() {
			progress = 0f;
		}
	}
}
