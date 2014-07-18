using UnityEngine;

public class ToggleMainMenu : MonoBehaviour {
	public KeyCode toggleKey = KeyCode.Escape;

	// OnGUI
	void OnGUI() {
		if(Event.current.type == EventType.KeyDown && Event.current.keyCode == toggleKey) {
			Sounds.instance.PlayButtonClick();

			// Toggle main menu
			MainMenu.instance.enabled = !MainMenu.instance.enabled;

			// Prevent event from bubbling
			Event.current.Use();
		}
	}
}
