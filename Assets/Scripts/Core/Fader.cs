using System;
using UnityEngine;

namespace BoM.Core {
	public class Fader : MonoBehaviour {
		public bool isReversed { get; private set; }
		public float duration;
		public Action<float> onFade;
		public Action onFadeEnd;
		private float time;
		private float progress;

		private void Update() {
			time += Time.deltaTime;
			progress = time / duration;

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
	}
}
