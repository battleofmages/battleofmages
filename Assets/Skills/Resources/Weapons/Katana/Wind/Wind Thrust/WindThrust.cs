using UnityEngine;

public class WindThrust : MonoBehaviour {
	public float moveSpeedMultiplier;
	
	private SkillInstance inst;
	private Entity caster;
	private Vector3 forwardVector;
	private float speed;

	// Start
	void Start() {
		inst = GetComponent<SkillInstance>();
		caster = inst.caster;
		speed = caster.baseMoveSpeed * moveSpeedMultiplier;

		//forwardVector = caster.charGraphics.forward * (caster.baseMoveSpeed * moveSpeedMultiplier);
		//caster.hasControlOverMovement = false;
	}
	
	// Update
	void Update() {
		if(caster.target != null) {
			forwardVector = (caster.target.myTransform.position - caster.myTransform.position).normalized;
		} else {
			var distance = inst.hitPoint - caster.myTransform.position;

			if(distance.sqrMagnitude > 1f) {
				distance.Normalize();
			}

			forwardVector = distance;
		}
		
		caster.characterController.Move(forwardVector * speed * Time.deltaTime);
	}

	// OnDestroy
	void OnDestroy() {
		//caster.hasControlOverMovement = true;
	}
}
