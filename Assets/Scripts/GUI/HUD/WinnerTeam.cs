using UnityEngine;
using System.Collections;

public class WinnerTeam : HUDElement {
	public GUIStyle winnerTeamStyle;
	
	// Draw
	public override void Draw() {
		// Winning
		if(GameManager.gameEnded) {
			// Show winner team
			Color c = Player.main.winnerParty.color;
			GUI.backgroundColor = new Color(c.r, c.g, c.b, 0.85f);
			
			string teamName = "Your team";
			
			if(Player.main.party != Player.main.winnerParty) {
				teamName = "Enemy team";
			}
			
			GUI.Box(new Rect(0, 0, GUIArea.width, GUIArea.height), teamName + " has won the game.", winnerTeamStyle);
		}
	}
}
