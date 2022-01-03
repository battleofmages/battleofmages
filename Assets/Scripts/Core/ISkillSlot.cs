namespace BoM.Core {
	public interface ISkillSlot {
		ISkill Skill { get; }
		float LastUsed { get; }
		bool IsReady { get; }
	}
}
