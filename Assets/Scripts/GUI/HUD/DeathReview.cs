using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathReview : HUDElement {
	public GUIStyle damageStyle;
	public GUIStyle respawnTimerStyle;
	
	private Player player;
	private Vector2 scrollPosition;
	
	// Start
	void Start() {
		player = this.GetComponent<Player>();
	}
	
	// Draw
	public override void Draw() {
		// Player alive?
		if(player.isAlive) {
			scrollPosition = Vector2.zero;
			return;
		}
		
		// Skill damage
		using(new GUIScrollView(ref scrollPosition)) {;
			using(new GUIVertical("box")) {
				foreach(KeyValuePair<int, int> entry in player.skillDamageReceived) {
					GUILayout.BeginHorizontal();
					
					Skill skill = Skill.idToSkill[entry.Key];
					GUILayout.Label(skill.icon, damageStyle, GUILayout.Width(32));
					
					// Damage string
					string dmg;
					if(entry.Value == 0)
						dmg = "-";
					else
						dmg = entry.Value.ToString();
					
					GUILayout.Label(skill.skillName + "\n" + dmg, damageStyle);
					GUILayout.EndHorizontal();
				}
			}
		}
		
		// Respawn timer
		using(new GUIVertical("box")) {
			float respawnTimer = Config.instance.playerRespawnTime - (float)(uLink.Network.time - player.lastDeathTime);
			if(respawnTimer < 0)
				respawnTimer = 0;
			
			GUILayout.Space(4);
			GUILayout.Label("Respawn in " + ((int)(respawnTimer + 0.99f)).ToString(), respawnTimerStyle);
			GUILayout.Space(4);
		}
	}
}
