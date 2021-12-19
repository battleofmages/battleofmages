using System;
using UnityEngine;

namespace BoM.Core {
	public static class ComponentExtensions {
		public static void Fade(this Component component, float fadeTime, Action<float> onFade, Action onFadeEnd = null) {
			var gameObject = component.gameObject;
			var fader = gameObject.GetComponent<Fader>();

			if(fader != null) {
				fader.ReverseProgress();
			} else {
				fader = gameObject.AddComponent<Fader>();
			}

			fader.duration = fadeTime;
			fader.onFade = onFade;
			fader.onFadeEnd = onFadeEnd;
		}
	}
}
