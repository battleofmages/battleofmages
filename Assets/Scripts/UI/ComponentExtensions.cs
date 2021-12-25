using System;
using UnityEngine;

namespace BoM.UI {
	public static class ComponentExtensions {
		private static Fader BaseFade(this Component component, float fadeTime, Action<float> onFade, Action onFadeEnd = null) {
			var gameObject = component.gameObject;
			var fader = gameObject.GetComponent<Fader>();

			if(fader == null) {
				fader = gameObject.AddComponent<Fader>();
			}

			fader.Duration = fadeTime;
			fader.onFade = onFade;
			fader.onFadeEnd = onFadeEnd;
			return fader;
		}

		// Fade will always replace the existing Fade with a new one and reset the progress to zero.
		public static Fader Fade(this Component component, float fadeTime, Action<float> onFade, Action onFadeEnd = null) {
			var fader = component.BaseFade(fadeTime, onFade, onFadeEnd);
			fader.Restart();
			return fader;
		}

		// FadeIn will never change the progress of a "fade in" but will reverse the progress of "fade out".
		public static Fader FadeIn(this Component component, float fadeTime, Action<float> onFade, Action onFadeEnd = null) {
			var fader = component.BaseFade(fadeTime, onFade, onFadeEnd);

			if(fader.isReversed) {
				fader.Reverse();
			}

			return fader;
		}

		// FadeOut will never change the progress of a "fade out" but will reverse the progress of "fade in".
		public static Fader FadeOut(this Component component, float fadeTime, Action<float> onFade, Action onFadeEnd = null) {
			var fader = component.BaseFade(fadeTime, onFade, onFadeEnd);

			if(!fader.isReversed) {
				fader.Reverse();
			}

			return fader;
		}
	}
}
