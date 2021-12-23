using BoM.Core;

namespace BoM.Players {
	public class DamageEvent {
		public IPlayer Receiver;
		public float Damage;
		public ISkill Skill;
		public IPlayer Caster;

		public DamageEvent(IPlayer receiver, float damage, ISkill skill, IPlayer caster) {
			Receiver = receiver;
			Damage = damage;
			Skill = skill;
			Caster = caster;
		}
	}
}
