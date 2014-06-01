using UnityEngine;

public class ImmobilizeDebuff : SkillEffect {
	// Instances
	//public static ImmobilizeDebuff shacklingEarth;
	public static ImmobilizeDebuff bindingField = new ImmobilizeDebuff(0.45f);
	public static ImmobilizeDebuff freezingField = new ImmobilizeDebuff(1.8f);
	
	public ImmobilizeDebuff(float nDuration) {
		duration = nDuration;
		type = "Immobilize";
	}
	
	public override void Apply(Entity ent) {
		ent.immobilized += 1;
	}
	
	public override void Remove(Entity ent) {
		ent.immobilized -= 1;
	}
}
