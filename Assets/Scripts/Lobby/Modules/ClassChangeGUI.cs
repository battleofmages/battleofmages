using UnityEngine;

public class ClassChangeGUI : LobbyModule<ClassChangeGUI> {
	// Start
	void Start() {

	}

	// Draw
	public override void Draw() {
		using(new GUIVertical("box")) {
			GUILayout.Label("Work in progress");
		}
	}
}