using UnityEngine;
using System.Collections.Generic;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	[HideInInspector]
	public List<Rune> runes;

#region Methods
	// Cleansing runes by detonating someone else's runes
	protected void CleanseRuneLevels() {
		ResetRuneLevels();
	}
	
	// Resets all rune levels
	protected void ResetRuneLevels(int newLevel = 0) {
		if(runes == null)
			return;
		
		foreach(Rune rune in runes) {
			rune.level = newLevel;
		}
	}
	
	// Detonates all runes
	public void DetonateRunes(Entity caster) {
		//LogManager.General.Log("Trying to detonate runes on " + this.GetName() + " (" + runes.Count + " rune types)");
		if(runes == null)
			return;
		
		bool detonated = false;
		foreach(Rune rune in runes) {
			if(rune.level > 0) {
				caster.UseSkill(rune.detonationSkill, myTransform.position, rune.level - 1);
				detonated = true;
				
				// Caster stats
				var casterQueueStats = caster.stats.total;
				casterQueueStats.runeDetonations += 1;
				casterQueueStats.runeDetonationsLevel += rune.level;
				
				// Player stats
				var queueStats = stats.total;
				queueStats.runeDetonationsTaken += 1;
				queueStats.runeDetonationsLevelTaken += rune.level;
			}
		}
		
		// If caster managed to hit, cleanse his runes
		if(detonated) {
			// Main player gets attacked
			if(this == Player.main) {
				Entity.SpawnText(this, "Detonation", new Color(1f, 0.5f, 0f, 1f), Random.Range(-10, 10), 35 + Config.instance.ownDmgOffset);
				
				if(caster.hasRunes)
					Entity.SpawnText(caster, "Cleanse", new Color(0f, 1f, 0.5f, 1f), Random.Range(-10, 10), 35);
				// Main player attacks someone else
			} else if (caster == Player.main) {
				Entity.SpawnText(this, "Detonation", new Color(1f, 0.5f, 0f, 1f), Random.Range(-10, 10), 35);
				
				if(caster.hasRunes)
					Entity.SpawnText(caster, "Cleanse", new Color(0f, 1f, 0.5f, 1f), Random.Range(-10, 10), 35 + Config.instance.ownDmgOffset);
			}
			
			caster.CleanseRuneLevels();
		}
		
		this.ResetRuneLevels();
	}
#endregion

#region Properties
	// Has runes
	public bool hasRunes {
		get {
			foreach(Rune rune in runes) {
				if(rune.level > 0)
					return true;
			}
			
			return false;
		}
	}
#endregion

#region RPCs
	[RPC]
	public void ReceiveRune(byte runeId) {
		runes[runeId].level += 1;
		//LogManager.General.Log(this.GetName() + " received rune " + runeId + " which is now at level " + this.runes[runeId].level);
	}
#endregion
}
