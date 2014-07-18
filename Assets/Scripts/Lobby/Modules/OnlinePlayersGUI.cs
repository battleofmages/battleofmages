using UnityEngine;
using System.Collections;

public class OnlinePlayersGUI : LobbyModule<OnlinePlayersGUI> {
	public GUIStyle memberNameStyle;
	public GUIStyle onlinePlayerCountStyle;
	public Texture2D[] statusIcons;
	
	private Vector2 chatMembersScrollPosition;
	
	public override void Draw() {
		PlayerAccount account;
		string playerName;
		
		using(new GUIVertical("box")) { // GUILayout.MinWidth(200)
			chatMembersScrollPosition = GUILayout.BeginScrollView(chatMembersScrollPosition);
			foreach(var member in LobbyChat.instance.members) {
				GUILayout.BeginHorizontal();
				account = PlayerAccount.Get(member.accountId);
				playerName = account.playerName;
				DrawPlayerName(playerName, new GUIContent(playerName), memberNameStyle);
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			
			// Online player count
			GUILayout.Label(_("{0} online.", GUIHelper.Plural(InGameLobby.instance.totalOnlinePlayers, "player")), onlinePlayerCountStyle);
		}
	}
}
