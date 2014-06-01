using uLobby;
using UnityEngine;
using System.Collections;

public class MailFeedback : HUDElement {
	public GUIContent feedbackButtonContent;
	
	// Start
	void Start() {
		if(!uLink.Network.isClient)
			this.enabled = false;
	}
	
	// Draw
	public override void Draw() {
		if(!GameManager.isTown || MainMenu.instance.currentState == InGameMenuState.Lobby)
			return;
		
		if(GUIHelper.Button(feedbackButtonContent)) {
			new TextAreaWindow(
				"Let us know what you like or dislike and how we can improve your experience with the game!",
				"\n\n\n\n\n",
				(text) => {
					Lobby.RPC("MailFeedback", Lobby.lobby, text);
				}
			).acceptText = "Send";
		}
	}
}
