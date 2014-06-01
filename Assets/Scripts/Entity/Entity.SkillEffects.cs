using UnityEngine;
using System.Collections.Generic;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	[HideInInspector]
	public List<SkillEffect> skillEffects;
	
	[HideInInspector]
	public List<double> skillEffectsTimeApplied;

#region Methods
	// AddSkillEffect
	public void AddSkillEffect(SkillEffect effect) {
		skillEffectsTimeApplied.Add(uLink.Network.time);
		skillEffects.Add(effect);
		effect.Apply(this);
		
		//LogManager.General.Log("Added skill effect " + effect.type + ", " + effect.duration);
	}
	
	// UpdateSkillEffects
	public void UpdateSkillEffects() {
		SkillEffect effect;
		var now = uLink.Network.time;
		
		for(int i = 0; i < skillEffects.Count; i++) {
			effect = skillEffects[i];
			if(now - skillEffectsTimeApplied[i] >= effect.duration) {
				skillEffects.RemoveAt(i);
				skillEffectsTimeApplied.RemoveAt(i);
				effect.Remove(this);
				
				// To get the correct index for the next iteration
				i -= 1;
			}
		}
	}
#endregion

#region RPCs
	[RPC]
	public void ReceiveSkillEffect(byte skillEffectId, ushort casterId) {
		SkillEffect effect = SkillEffect.skillEffectList[skillEffectId];
		
		if(effect == null) {
			LogManager.General.LogError("Skill effect is null!");
			return;
		}
		
		AddSkillEffect(effect);
		
		// Show the CC effect
		if(uLink.Network.isClient) {
			int ccOffset = 40;
			bool alwaysShow = false;
			
			if(effect is SleepDebuff) {
				alwaysShow = true;
			}
			
			// Who casted this?
			Entity caster;
			Entity.idToEntity.TryGetValue(casterId, out caster);
			
			// CC I received from someone else
			if(this == Player.main) {
				Entity.SpawnText(this, effect.type, new Color(0.95f, 0.0f, 0.0f, 1.0f), Random.Range(-10, 10), ccOffset + Config.instance.ownDmgOffset);
				// CC the main player casted on myself (the proxy)
			} else if(caster == Player.main) {
				Entity.SpawnText(this, effect.type, new Color(0.95f, 0.95f, 0.0f, 1.0f), Random.Range(-10, 10), ccOffset);
				// CC texts that are always shown
			} else if(alwaysShow) {
				Entity.SpawnText(this, effect.type, new Color(0.5f, 0.5f, 0.5f, 1.0f), Random.Range(-10, 10), ccOffset);
			}
		}
		
		//LogManager.General.Log("Added effect " + skillEffectId);
	}
#endregion
}
