using UnityEngine;
using System.Collections.Generic;

public enum SkillEffectId {
	None,
	Stun_Meteor,
	Slow_IceBall,
	Slow_IceBurst,
	Sleep,
	Stun_ThunderBall,
	Immobilize_BindingField,
	Immobilize_FreezingField,
}

public class SkillEffect {
	public static List<SkillEffect> skillEffectList;
	public const SkillEffect None = null;
	
	public static void InitSkillEffects() {
		skillEffectList = new List<SkillEffect>();
		// TODO: Make this shorter
		skillEffectList.Add(null);
		skillEffectList.Add(StunDebuff.meteor);
		skillEffectList.Add(SlowDebuff.iceball);
		skillEffectList.Add(SlowDebuff.iceBurst);
		skillEffectList.Add(SleepDebuff.sleep);
		skillEffectList.Add(StunDebuff.thunderBall);
		skillEffectList.Add(ImmobilizeDebuff.bindingField);
		skillEffectList.Add(ImmobilizeDebuff.freezingField);
		
		StunDebuff.meteor.id = SkillEffectId.Stun_Meteor;
		SlowDebuff.iceball.id = SkillEffectId.Slow_IceBall;
		SlowDebuff.iceBurst.id = SkillEffectId.Slow_IceBurst;
		SleepDebuff.sleep.id = SkillEffectId.Sleep;
		StunDebuff.thunderBall.id = SkillEffectId.Stun_ThunderBall;
		ImmobilizeDebuff.bindingField.id = SkillEffectId.Immobilize_BindingField;
		ImmobilizeDebuff.freezingField.id = SkillEffectId.Immobilize_FreezingField;
	}
	
	public float duration = 0.0f;
	public string type;
	
	protected SkillEffectId _id;
	
	/*public SkillEffect() {
		_id = SkillEffect.skillEffectList.Count;
		SkillEffect.skillEffectList.Add(this);
	}*/
	
	public SkillEffectId id {
		get { return _id; }
		set { _id = value; }
	}
	
	public virtual void Apply(Entity ent) {
		// ...
	}
	
	public virtual void Remove(Entity ent) {
		// ...
	}
}
