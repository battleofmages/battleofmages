using System;
using UnityEngine;

namespace BoM.UI {
	public static class ComponentExtensions {
		public static Fader FadeIn(this Component component, float fadeTime, Action<float> onFade, Action onFadeEnd = null) {
			var gameObject = component.gameObject;
			var fader = gameObject.GetComponent<Fader>();

			if(fader == null) {
				fader = gameObject.AddComponent<Fader>();
			}
			
			fader.duration = fadeTime;
			fader.onFade = onFade;
			fader.onFadeEnd = onFadeEnd;
			
			if(fader.isReversed) {
				fader.Reverse();
			}
			
			return fader;
		}

		public static Fader FadeOut(this Component component, float fadeTime, Action<float> onFade, Action onFadeEnd = null) {
			var gameObject = component.gameObject;
			var fader = gameObject.GetComponent<Fader>();

			if(fader == null) {
				fader = gameObject.AddComponent<Fader>();
			}
			
			fader.duration = fadeTime;
			fader.onFade = onFade;
			fader.onFadeEnd = onFadeEnd;

			if(!fader.isReversed) {
				fader.Reverse();
			}
			
			return fader;
		}
	}
}
