using UnityEngine;

public class KeyOverlay : HUDElement {
	public GUIStyle keyStyle;

	// Draw
	public override void Draw() {
		if(Player.main == null)
			return;

		// Show action target
		if(Player.main.actionTarget != null) {
			var actionIndex = InputManager.instance.GetButtonIndex("action");
			var actionInputControl = InputManager.instance.controls[actionIndex];

			// Draw key label
			GUILayout.Label(actionInputControl.keyCodeString, keyStyle);
		}
	}
}
