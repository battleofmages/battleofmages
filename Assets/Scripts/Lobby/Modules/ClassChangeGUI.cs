using UnityEngine;

public class ClassChangeGUI : LobbyModule<ClassChangeGUI> {
	// Start
	void Start() {

	}

	// Draw
	public override void Draw() {
		using(new GUIVertical("box")) {
			GUILayout.Label("<size=32>Class change</size>");
			GUILayout.Space(16);
			GUILayout.Label("Work in progress.");
		}
	}
}