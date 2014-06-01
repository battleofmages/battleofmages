using UnityEngine;
using System.Collections;
using System;

// Delegate
public delegate void CallBackFade(float nValue);

// Fade
public class Fade : MonoBehaviour {
	public float fadeTime;
	public DateTime lastActionTime;
	
	public CallBackFade callBackFade;
	public CallBack callBackFadeEnd;
	
	// Start
	void Start() {
		lastActionTime = DateTime.UtcNow;
		fadeTime *= 1000f;
	}
	
	// Update
	void Update() {
		var passedTime = (float)((DateTime.UtcNow - lastActionTime).TotalMilliseconds);
		
		float fadeValue = 1f;
		if(fadeTime != 0)
			fadeValue = passedTime / fadeTime;
		
		callBackFade(fadeValue);
		
		if(fadeValue >= 1f) {
			this.Stop();
		}
	}
	
	// Stop
	public void Stop() {
		Destroy(this);
	}
	
	// OnDestroy
	void OnDestroy() {
		if(callBackFadeEnd != null)
			callBackFadeEnd();
	}
}

// Extensions for mono behaviour
static class MonoBehaviourExtensions {
	public static void Fade(this uLink.MonoBehaviour monoBehaviour, float nFadeTime, CallBackFade nCallBackFade, CallBack nCallBackFadeEnd = null) {
		var gameObject = monoBehaviour.gameObject;
		
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