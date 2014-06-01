using UnityEngine;

public class StunDebuff : SkillEffect {
	// Instances
	public static StunDebuff meteor = new StunDebuff(1.5f);
	public static StunDebuff thunderBall = new StunDebuff(1.2f);
	
	public StunDebuff(float nDuration) {
		duration = nDuration;
		type = "Stun";
	}
	
	public override void Apply(Entity ent) {
		//if(ent.stunned > 0)
		//	this.duration *= Mathf.Pow(0.5f, ent.stunned);
		
		ent.StartStun();
		//LogManager.General.Log("Applied stun");
	}
	
	public override void Remove(Entity ent) {
		//LogManager.General.Log("Remove stun");
		ent.EndStun();
		//LogManager.General.Log("Removed stun");
	}
}
