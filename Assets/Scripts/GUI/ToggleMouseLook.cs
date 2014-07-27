using UnityEngine;

public class ToggleMouseLook : DestroyableSingletonMonoBehaviour<ToggleMouseLook> {
	public KeyCode toggleKey = KeyCode.LeftAlt;
	public GUIStyle backgroundBoxStyle;
	public int guiDepth;
	public MouseLook mouseLook;
	
	private bool dimBackground;

	// OnGUI
	void OnGUI() {
		GUI.depth = guiDepth;
		
		// Toggle mouse look with toggle key
		if(Event.current.type == EventType.KeyDown && Event.current.keyCode == toggleKey) {
			//Sounds.instance.PlayMenuBack();

			if(mouseLook.enabled) {
				DisableMouseLook();
			} else {
				EnableMouseLook();
			}
			
			Event.current.Use();
		}

		// IMPORTANT:
		// The cursor settings should stay inside
		// a main loop because they could be lost.

		// Lock cursor and hide it when mouse look is enabled
		if(mouseLook.enabled) {
			Screen.showCursor = false;
			Screen.lockCursor = true;
		// Show cursor and unlock it when mouse look is disabled
		} else {
			Screen.showCursor = true;
			Screen.lockCursor = false;
			
			if(dimBackground && MainMenu.instance.currentState == InGameMenuState.Lobby) {
				GUI.backgroundColor = new Color(1f, 1f, 1f, 0.95f);
				GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", backgroundBoxStyle);
			}
		}
	}

	// EnableMouseLook
	public void EnableMouseLook() {
		if(Player.main == null)
			return;
		
		// No looking around after game ended
		if(GameManager.gameEnded)
			return;
		
		// Stop talking with NPC
		Player.main.talkingWithNPC = null;
		
		mouseLook.enabled = true;
		HideMenu();
		
		if(Player.main.target == null)
			Player.main.crossHair.enabled = true;
	}

	// DisableMouseLook
	public void DisableMouseLook(bool showMainMenu = true) {
		if(Player.main == null)
			return;

		// Talking with NPC
		if(Player.main.talkingWithNPC != null) {
			HideMenu();
			dimBackground = false;
		} else if(showMainMenu) {
			ShowMenu();
			dimBackground = true;
		}
		
		mouseLook.enabled = false;
		Player.main.crossHair.enabled = false;
	}

	// ShowMenu
	void ShowMenu() {
		MainMenu.instance.enabled = true;
	}

	// HideMenu
	void HideMenu() {
		MainMenu.instance.enabled = false;
	}
}
