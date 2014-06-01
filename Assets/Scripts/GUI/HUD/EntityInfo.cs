using UnityEngine;
using System.Collections;

public class EntityInfo : HUDElement {
	Entity selectedEntity;
	
	// Draw
	public override void Draw() {
		if(Player.main == null)
			return;
		
		//if(!GameManager.isPvE && !GameManager.isTown)
		//	return;
		
		// Player
		selectedEntity = (Player.main as PlayerOnClient).selectedEntity;
		if(selectedEntity is Player) {
			DrawPlayerInfo(selectedEntity as Player);
		} else if(selectedEntity is Enemy) {
			DrawEnemyInfo(selectedEntity as Enemy);
		}
	}
	
	// DrawPlayerInfo
	void DrawPlayerInfo(Player selectedPlayer) {
		// Player popup menu
		if(InGameLobby.instance != null) {
			// Info
			using(new GUIVertical("box")) {
				GUILayout.Label("<b>Name:</b> " + selectedPlayer.account.playerName);
				GUILayout.Label("<b>Level:</b> " + (int)(selectedPlayer.level));
				GUILayout.Label("<b>Guild:</b> " + selectedPlayer.account.guildName);
				GUILayout.Label("<b>Class:</b> " + selectedPlayer.account.characterClassName);
				GUILayout.Label("<b>Ranking:</b> " + selectedPlayer.stats.bestRanking + " points");
				GUILayout.Label("<b>Ping:</b> " + selectedPlayer.stats.ping + " ms");

				// GM specific info
				if(PlayerAccount.mine.accessLevel >= AccessLevel.GameMaster) {
					GUILayout.Label("<b>E-Mail:</b> " + selectedPlayer.account.email);
					GUILayout.Label("<b>Account ID:</b> " + selectedPlayer.accountId);
				}
			}
			
			// Popup
			if(Input.GetMouseButtonDown(1) && !string.IsNullOrEmpty(selectedPlayer.name)) {
				ToggleMouseLook.instance.DisableMouseLook();
				InGameLobby.instance.CreatePlayerPopupMenu(selectedPlayer.name);
			}
		}
	}

	// DrawEnemyInfo
	void DrawEnemyInfo(Enemy selectedEnemy) {
		using(new GUIVertical("box")) {
			GUILayout.Label("<b>Name:</b> " + selectedEnemy.name);
			GUILayout.Label("<b>Level:</b> " + (int)(selectedEnemy.level));
			GUILayout.Label("<b>Element:</b> " + selectedEnemy.currentAttunement.name);
		}
	}
}