using System;
using UnityEngine;

namespace BoM.Core {
	public class Fader : MonoBehaviour {
		public float duration;
		public Action<float> onFade;
		public Action onFadeEnd;
		private float time;
		private float progress;

		private void Update() {
			time += Time.deltaTime;
			progress = time / duration;

			if(progress >= 1f) {
				onFade(1f);
				onFadeEnd?.Invoke();
				Stop();
				return;
			}

			onFade(progress);
		}

		public void Stop() {
			Destroy(this);
		}

		public void ReverseProgress() {
			time = duration - time;
		}
	}
}
