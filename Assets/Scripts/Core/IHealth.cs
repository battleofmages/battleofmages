namespace BoM.Core {
	public interface IHealth {
		void TakeDamage(int damage, ISkill skill, IPlayer caster);
	}
}
