using UnityEngine;
using System;

// Delegate
public delegate void FadeCallBack(float nValue);

// Fade
public class Fade : MonoBehaviour {
	public float fadeTime;
	public DateTime lastActionTime;
	
	public FadeCallBack callBackFade;
	public CallBack callBackFadeEnd;

	private float fadeValue;
	private float passedTime;
	
	// Start
	void Start() {
		lastActionTime = DateTime.UtcNow;
		fadeTime *= 1000f;
	}
	
	// Update
	void Update() {
		fadeValue = 1f;

		if(fadeTime != 0) {
			passedTime = (float)((DateTime.UtcNow - lastActionTime).TotalMilliseconds);
			fadeValue = passedTime / fadeTime;
		}

		if(fadeValue >= 1f) {
			callBackFade(1f);
			Stop();
		} else {
			callBackFade(fadeValue);
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