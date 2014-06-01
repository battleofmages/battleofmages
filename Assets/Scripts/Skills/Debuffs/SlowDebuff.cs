using UnityEngine;

public class SlowDebuff : SkillEffect {
	// Instances
	public static SlowDebuff iceball = new SlowDebuff(5.0f, 0.06f);
	public static SlowDebuff iceBurst = new SlowDebuff(5.0f, 0.5f);
	
	// Parameters
	private float slowBy = 0.0f;
	
	// Constructor
	public SlowDebuff(float nDuration, float nSlowBy) {
		duration = nDuration;
		slowBy = nSlowBy;
		type = "Slow";
	}
	
	// Apply
	public override void Apply(Entity ent) {
		ent.moveSpeedModifier -= slowBy;
	}
	
	// Remove
	public override void Remove(Entity ent) {
		ent.moveSpeedModifier += slowBy;
	}
}
