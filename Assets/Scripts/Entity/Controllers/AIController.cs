using UnityEngine;

public class AIController : Controller {
	private Entity entity;
	private byte slotId;
	private float maxSkillValue;

	// Constructor
	public AIController(Entity nPlayer) {
		entity = nPlayer;
	}

	// Update
	public void Update() {
		
	}

	// UpdateInput
	public void UpdateInput() {
		
	}

	// GetNextSkill
	public byte GetNextSkill() {
		maxSkillValue = -1f;
		slotId = byte.MaxValue;
		float skillValue;
		
		// Skill slot buttons
		for(byte i = 0; i < entity.skills.Count; i++) {
			skillValue = GetSkillValue(entity.skills[i]);

			if(skillValue > maxSkillValue) {
				slotId = i;
				maxSkillValue = skillValue;
			}
		}

		return slotId;
	}

	// GetSkillValue
	private float GetSkillValue(Skill skill) {
		if(skill.currentStage.isOnCooldown)
			return -1f;

		return skill.currentStage.cooldown;
	}

	// OnSkillIsOnCooldown
	public void OnSkillIsOnCooldown() {
		
	}

	// OnNotEnoughEnergyForSkillCast
	public void OnNotEnoughEnergyForSkillCast() {
		
	}

	// Holds skill
	public bool holdsSkill {
		get {
			return false;
		}
	}

	// Can start cast
	public bool canStartCast {
		get {
			if(entity.skillBuild == null)
				return false;

			if(!entity.hasTarget)
				return false;
			
			return true;
		}
	}
}
