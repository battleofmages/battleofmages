using UnityEngine;
using System.Collections;

public class TimedConfirm : Confirm {
	private float timeStarted;
	private float timeLeft;
	
	// Constructor
	public TimedConfirm(string nText, float nTimeLeft, CallBack nYes, CallBack nNo = null) : base(nText, nYes, nNo) {
		timeStarted = Time.time;
		timeLeft = nTimeLeft;
		
		if(ToggleMouseLook.instance != null) {
			ToggleMouseLook.instance.DisableMouseLook(false);
		}
	}
	
	// Update
	public override void Update() {
		if(Time.time - timeStarted > timeLeft) {
			if(cancel != null)
				cancel();
			
			this.Close();
		}
	}
	
	// Draw
	public override void Draw() {
		// Confirm box
		using(new GUIVertical("box")) {
			// Title
			DrawText();
			
			// Time left
			using(new GUIHorizontal()) {
				GUILayout.Space(4);
				GUIHelper.ProgressBar((timeLeft - (Time.time - timeStarted)).ToString("0") + " s", (Time.time - timeStarted) / timeLeft);
				GUILayout.Space(4);
			}
			
			// Yes / No
			DrawButtons();
		}
	}
}
