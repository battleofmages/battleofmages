using UnityEngine;
using System.Collections;

public class MeleeAttack : SkillInstance {
	public float startDelay;
	public AnimationCurve forwardMovementCurve;

	// Duration must be LOWER than the attack anim duration of the skill
	private float duration;
	private GameObject weaponModelReference;
	
	private MeleeWeaponTrail trail;
	private float time;
	
	// Start
	void Start() {
		duration = skillStage.attackAnimDuration;

		trail = caster.weaponModel.GetComponent<MeleeWeaponTrail>();


		Invoke("StartMeleeAttack", startDelay);
		Invoke("StopMeleeAttack", duration);
	}

	// Update
	void Update() {
		time += Time.deltaTime;
		caster.characterController.Move(caster.charGraphicsModel.forward * forwardMovementCurve.Evaluate(time) * Time.deltaTime);
	}

	// SetTrail
	void SetTrail(bool trailEnabled) {
		// Server doesn't draw trails
		if(uLink.Network.isServer)
			return;

		trail.emit = trailEnabled;
	}

	// StartMeleeAttack
	void StartMeleeAttack() {
		SetTrail(true);
		caster.StartMeleeAttack();
	}

	// StopMeleeAttack
	void StopMeleeAttack() {
		SetTrail(false);
		caster.StopMeleeAttack();
		Destroy(gameObject);
	}
}
