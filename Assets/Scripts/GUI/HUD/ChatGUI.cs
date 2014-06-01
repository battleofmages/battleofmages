using UnityEngine;

public class ChatGUI : HUDElement {
	private LobbyChat lobbyChat;
	
	// Start
	void Start() {
		if(InGameLobby.instance != null)
			lobbyChat = InGameLobby.instance.lobbyChat;
		else
			this.enabled = false;
		
		if(!uLink.Network.isClient)
			this.enabled = false;
	}
	
	// Draw
	public override void Draw() {
		/*GUI.Label(new Rect(5, 5, 200, 20), Event.current.keyCode.ToString());
		GUI.Label(new Rect(5, 25, 200, 20), Event.current.character.ToString());
		GUI.Label(new Rect(5, 45, 200, 20), GUI.GetNameOfFocusedControl());*/
		
		if(Player.main != null && (Player.main.talkingWithNPC || (MainMenu.instance.currentState != InGameMenuState.None && MainMenu.instance.currentState != InGameMenuState.PvP))) {
			GUI.color = new Color(1f, 1f, 1f, 0.1f);
		} else {
			GUI.color = new Color(1f, 1f, 1f, 1.0f);
		}
		
		lobbyChat.DrawChat();
	}
}