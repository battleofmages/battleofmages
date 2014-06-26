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
			if(mouseLook.enabled) {
				DisableMouseLook();
			} else {
				EnableMouseLook();
			}
			
			Event.current.Use();
		}
		
		if(mouseLook.enabled) {
			Screen.showCursor = false;
			Screen.lockCursor = true;
			return;
		}
		
		Screen.showCursor = true;
		Screen.lockCursor = false;
		
		if(dimBackground) {
			if(MainMenu.instance.currentState == InGameMenuState.Lobby) {
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
			Player.main.GetComponent<CrossHair>().enabled = true;
	}

	// DisableMouseLook
	public void DisableMouseLook(bool showMainMenu = true) {
		// Talking with NPC
		if(Player.main.talkingWithNPC != null) {
			HideMenu();
			dimBackground = false;
		} else if(showMainMenu) {
			ShowMenu();
			dimBackground = true;
		}
		
		mouseLook.enabled = false;
		
		if(Player.main != null)
			Player.main.GetComponent<CrossHair>().enabled = false;
	}

	// ShowMenu
	void ShowMenu() {
		if(Player.main != null)
			Player.main.GetComponent<MainMenu>().enabled = true;
	}

	// HideMenu
	void HideMenu() {
		if(Player.main != null)
			Player.main.GetComponent<MainMenu>().enabled = false;
	}
}
