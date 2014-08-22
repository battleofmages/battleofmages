using UnityEngine;

public class ReturnButton : HUDElement {
	// Draw
	public override void Draw() {
		// Return to lobby
		GUI.backgroundColor = Color.white;
		if(GUI.Button(new Rect(0, 0, 128, 32), "Return")) {
			OldLogin.instance.ReturnToWorld();
		}
	}
}
