using UnityEngine;

public class SleepDebuff : SkillEffect {
	// Instances
	public static SleepDebuff sleep = new SleepDebuff(3.0f);
	
	public SleepDebuff(float nDuration) {
		duration = nDuration;
		type = "Sleep";
	}
	
	public override void Apply(Entity ent) {
		ent.StartSleep();
	}
	
	public override void Remove(Entity ent) {
		ent.EndSleep();
	}
}
