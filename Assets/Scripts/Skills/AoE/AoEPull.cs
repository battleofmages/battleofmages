using UnityEngine;

public class AoEPull : AoEOverTime {
	public float pullPower;
	public float rigidBodyPullPower;

	// The AoE effect on entities
	protected override void AoEHit(Entity entity) {
		if(entity.canBePulled) {
			entity.hasControlOverMovement = false;
			entity.characterController.Move((position - entity.transform.position) * pullPower * Time.deltaTime);
		} else {
			AoEStop(entity);
		}
	}

	// The AoE effect on game objects
	protected override void AoEHit(Rigidbody obj) {
		obj.AddForce((position - obj.position) * rigidBodyPullPower);
	}
	
	// Reset control over movement
	protected override void AoEStop(Entity entity) {
		entity.hasControlOverMovement = true;
	}
}
