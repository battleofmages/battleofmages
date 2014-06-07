using UnityEngine;
using System.Collections;

public class Intro : uLink.MonoBehaviour {
	public bool canCancel;
	public Texture2D background;
	public float bgFadeOutStart;
	public float bgFadeOutDuration;
	public GUIStyle textStyle;
	public TextPhase[] phases;
	
	private bool goingToDisable;
	private float startTime;
	
	// Start
	void Start() {
		Restart();
	}
	
	public void Restart() {
		startTime = Time.time;
		this.enabled = true;
	}
	
	void OnGUI() {
		GUI.depth = -1;
		var timePassed = Time.time - startTime;
		
		if(timePassed >= bgFadeOutStart && bgFadeOutDuration != 0f) {
			GUI.color = new Color(1f, 1f, 1f, 1f - (timePassed - bgFadeOutStart) / bgFadeOutDuration);
			
			if(!goingToDisable && timePassed > bgFadeOutStart + bgFadeOutDuration) {
				goingToDisable = true;
				return;
			}
		}
		
		if(background != null)
			GUI.DrawTexture(new Rect(0, 0, GUIArea.width, GUIArea.height), background);
		
		foreach(var phase in phases) {
			if(timePassed < phase.start || timePassed > phase.start + phase.duration)
				continue;
			
			var phaseTimePassed = timePassed - phase.start;
			
			// Overwrite text style?
			if(!phase.overwriteTextStyle) {
				phase.textStyle = textStyle;
			}
			
			GUI.contentColor = Color.white;
			
			// Fade in
			if(phase.fadeInDuration != 0f) {
				if(phaseTimePassed <= phase.fadeInDuration) {
					GUI.contentColor = new Color(1f, 1f, 1f, phaseTimePassed / phase.fadeInDuration * phase.textStyle.normal.textColor.a);
				}
			}
			
			if(phase.fadeOutDuration != 0f) {
				var fadeOutStart = phase.duration - phase.fadeOutDuration;
				if(phaseTimePassed >= fadeOutStart) {
					GUI.contentColor = new Color(1f, 1f, 1f, 1.0f - (phaseTimePassed - fadeOutStart) / phase.fadeOutDuration * phase.textStyle.normal.textColor.a);
				}
			}
			
			/*// Set matrix
			Vector2 ratio = new Vector2(1f, 1f - phaseTimePassed * 0.01f);
			Matrix4x4 guiMatrix = Matrix4x4.identity;
			guiMatrix.SetTRS(new Vector3(1, 1, 1), Cache.quaternionIdentity, new Vector3(ratio.x, ratio.y, 1));
			GUI.matrix = guiMatrix;*/
			
			GUIHelper.Shadowed(
				phase.position.x * GUIArea.width,
				phase.position.y * GUIArea.height,
				(x, y) => {
					GUI.Label(new Rect(
						x, y,
						0, 0
					), phase.text, phase.textStyle);
				},
				new Color(1f, 1f, 1f, 0.1f)
			);
		}
		
		if(canCancel && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape) {
			Event.current.Use();
			goingToDisable = true;
			return;
		}
	}
	
	void LateUpdate() {
		if(goingToDisable) {
			goingToDisable = false;
			this.enabled = false;
		}	
	}
	
	void OnEnable() {
		// Show mouse cursor
		// This is also called after returning from a game to the lobby.
		if(!GameManager.inGame) {
			Screen.lockCursor = false;
			Screen.showCursor = false;
		}
	}
	
	void OnDisable() {
		// Show mouse cursor
		if(!GameManager.inGame) {
			Screen.lockCursor = false;
			Screen.showCursor = true;
		}
	}
	
	// Disconnected from server
	void uLink_OnDisconnectedFromServer(uLink.NetworkDisconnection mode) {
		this.enabled = false;
	}
}
