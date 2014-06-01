using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LastKills : HUDElement {
	public static LastKills instance;

	public AnimationCurve alphaCurve;
	public GUIStyle playerNameStyle;
	public float entryDuration;
	
	// KillEntry
	public class KillEntry {
		public string killer;
		public Skill skill;
		public string victim;
		public float time;
		
		// Constructor
		public KillEntry(string nKiller, Skill nSkill, string nVictim) {
			killer = nKiller;
			skill = nSkill;
			victim = nVictim;
			time = 0.0f;
		}
	}
	
	private List<KillEntry> killEntries;
	
	// Start
	void Start() {
		instance = this;
		killEntries = new List<KillEntry>();
	}
	
	// Draw
	public override void Draw() {
		if(killEntries.Count == 0)
			return;
		
		using(new GUIVertical()) {
			for(int i = killEntries.Count - 1; i >= 0; i--) {
				var entry = killEntries[i];
				
				// Fade out
				entry.time += Time.deltaTime / entryDuration;
				float alpha = alphaCurve.Evaluate(entry.time);
				if(alpha <= 0f) {
					// We can simply remove the first entry because it will be the one to trigger it first anyway
					killEntries.RemoveAt(0);
					i -= 1;
					continue;
				}
				GUI.color = new Color(1f, 1f, 1f, alpha);
				
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(entry.killer, playerNameStyle);
				if(entry.skill != null) {
					GUILayout.Label(entry.skill.icon, playerNameStyle);
				}
				GUILayout.Label(entry.victim, playerNameStyle);
				GUILayout.EndHorizontal();
			}
		}
	}
	
	// AddEntry
	public void AddEntry(string killer, Skill skill, string victim) {
		killEntries.Add(new KillEntry(killer, skill, victim));
	}
}
