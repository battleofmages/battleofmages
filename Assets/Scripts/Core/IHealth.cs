namespace BoM.Core {
	public interface IHealth {
		void TakeDamage(float damage, ISkill skill, IPlayer caster);
	}
}
