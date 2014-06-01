using UnityEngine;
using System.Collections;

public class MatchRewards : HUDElement {
	// Draw
	public override void Draw() {
		if(Player.main == null || Player.main.artifactReward == null)
			return;
		
		var arti = Player.main.artifactReward;
		
		using(new GUIHorizontal("box")) {
			GUILayout.Box(new GUIContent("", arti.icon), GUILayout.Width(64), GUILayout.Height(64));
			GUILayout.Label(arti.tooltip);
		}
	}
}
