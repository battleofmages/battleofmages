using System.Collections.Generic;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	// Threat from each entity
	public Dictionary<Entity, int> entityToThreat = new Dictionary<Entity, int>();
	
	// AddThreat
	public void AddThreat(Entity caster, int dmg) {
		int previousDmg;
		
		if(entityToThreat.TryGetValue(caster, out previousDmg))
			entityToThreat[caster] = previousDmg + dmg;
		else
			entityToThreat[caster] = dmg;
	}
	
	// ResetThreat
	public void ResetThreat() {
		entityToThreat = new Dictionary<Entity, int>();
	}

	// DistributeExperience
	protected void DistributeExperience(int levelFactor) {
		uint exp = (uint)level * (uint)levelFactor;
		
		foreach(var threat in entityToThreat) {
			var entity = threat.Key;
			
			entity.GainExperience(exp);
		}
		
		ResetThreat();
	}

	// DistributeSharedExperience
	protected void DistributeSharedExperience(int levelFactor) {
		uint exp = (uint)level * (uint)levelFactor;

		// Find max threat
		int maxThreat = 1;
		foreach(var entry in entityToThreat) {
			var threatNumber = entry.Value;

			if(threatNumber > maxThreat)
				maxThreat = threatNumber;
		}

		// Distribute EXP based on threat participation
		foreach(var entry in entityToThreat) {
			var entity = entry.Key;
			var participation = (float)entry.Value / maxThreat;
			
			entity.GainExperience((uint)(participation * (float)exp));
		}
		
		ResetThreat();
	}
}