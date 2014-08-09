using System.Collections.Generic;

public abstract partial class Entity : uLink.MonoBehaviour, PartyMember<Entity> {
	// Threat from each entity
	public Dictionary<Entity, int> entityToThreat = new Dictionary<Entity, int>();
	
	// AddThreat
	public void AddThreat(Entity caster, int dmg) {
		var enemy = this as EnemyOnServer;
		int previousDmg;
		
		if(enemy.entityToThreat.TryGetValue(caster, out previousDmg))
			enemy.entityToThreat[caster] = previousDmg + dmg;
		else
			enemy.entityToThreat[caster] = dmg;
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
}