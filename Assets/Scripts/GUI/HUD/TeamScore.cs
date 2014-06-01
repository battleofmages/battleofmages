using UnityEngine;
using System.Collections;

public class TeamScore : HUDElement {
	public GUIStyle teamScoreStyle;
	
	[HideInInspector]
	public int maxScore;
	
	// Draw
	public override void Draw() {
		// Score on the top
		//using(new GUIArea(new Rect(offX, offY, Screen.width - offX * 2, 50))) {
		using(new GUIHorizontal()) {
			// My team
			if(Player.main != null && Player.main.party != null) {
				var pty = Player.main.party;
				GUI.backgroundColor = new Color(pty.color.r, pty.color.g, pty.color.b, 0.85f);
				GUILayout.Box("Allies: " + pty.score.ToString() + " / " + maxScore, teamScoreStyle, GUILayout.Width(200));
			}
			
			foreach(GameServerParty pty in GameServerParty.partyList) {
				if(Player.main && Player.main.party == pty) {
					continue;
				}
				
				GUI.backgroundColor = new Color(pty.color.r, pty.color.g, pty.color.b, 0.85f);
				//GUI.contentColor = pty.color;
				
				string partyName = "Enemies";
				GUILayout.Box(partyName + ": " + pty.score.ToString() + " / " + maxScore, teamScoreStyle, GUILayout.Width(200));
				
				// All except for last party
				//if(pty.id + 1 != ptyCount)
				//	GUILayout.FlexibleSpace();
			}
		}
	}
}
