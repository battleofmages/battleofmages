using UnityEngine;

public class ToggleMainMenu : MonoBehaviour {
	public KeyCode toggleKey = KeyCode.Escape;

	// OnGUI
	void OnGUI() {
		if(Event.current.type == EventType.KeyDown && Event.current.keyCode == toggleKey) {
			// Toggle main menu
			var mainMenu = GetComponent<MainMenu>();
			mainMenu.enabled = !mainMenu.enabled;

			// Prevent event from bubbling
			Event.current.Use();
		}
	}
}
