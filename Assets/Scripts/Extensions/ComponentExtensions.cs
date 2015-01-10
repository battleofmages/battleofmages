using UnityEngine;

static class ComponentExtensions {
	// Fade
	public static void Fade(this Component component, float nFadeTime, FadeCallBack nCallBackFade, CallBack nCallBackFadeEnd = null) {
		var gameObject = component.gameObject;
		
		// Disable all fades
		foreach(var runningFade in gameObject.GetComponents<Fade>())
			runningFade.Stop();

		// Add new fade
		var fade = gameObject.AddComponent<Fade>();
		
		fade.fadeTime = nFadeTime;
		fade.callBackFade = nCallBackFade;
		fade.callBackFadeEnd = nCallBackFadeEnd;
	}
}