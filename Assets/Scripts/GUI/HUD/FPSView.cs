using UnityEngine;

// Requirements
[RequireComponent(typeof(GUIText))]

public class FPSView : MonoBehaviour {
	// Attach this to a GUIText to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// correct overall FPS even if the interval renders something like
	// 5.5 frames.
	
	// Global FPS
	public static int fps = 0;
	
	// Update interval
	public float updateInterval = 1.0f;
	
	// FPS accumulated over the interval
	private float accum = 0;
	
	// Frames drawn over the interval
	public static int frames = 0;
	
	// Left time for current interval
	private float timeLeft;
	
	// Start
	void Start() {
		if(!guiText) {
			LogManager.General.LogWarning("FPSView needs a GUIText component!");
			enabled = false;
			return;
		}
		
		timeLeft = updateInterval;  
	}
	
	// Update
	void Update() {
		timeLeft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if(timeLeft <= 0f) {
			fps = (int)accum / frames;
			
			if(fps < 0) {
				guiText.material.color = new Color(1f, 1f, 1f, 0f);
			} else if(fps < 30) {
				if(fps < 10)
					guiText.material.color = Color.red;
				else
					guiText.material.color = Color.yellow;
			} else {
				guiText.material.color = Color.white;
			}
			
			string format = System.String.Format("<b>{0}</b> FPS", fps);
			guiText.text = format;
			
			timeLeft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}