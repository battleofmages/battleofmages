public interface Controller {
	void Update();
	void UpdateInput();
	byte GetNextSkill();

	void OnSkillIsOnCooldown();
	void OnNotEnoughEnergyForSkillCast();

	bool holdsSkill {get;}
	bool canStartCast {get;}
}