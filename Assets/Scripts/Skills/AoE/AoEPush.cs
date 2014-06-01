using UnityEngine;

public class AoEPush : AoEOverTime {
	public float maxPushRadius;
	public float pushPower;
	public float rigidBodyExplosionPower;
	
	// The AoE effect on entities
	protected override void AoEHit(Entity entity) {
		if(entity.canBePulled) {
			entity.hasControlOverMovement = false;

			var distanceToCenter = (entity.myTransform.position - position);
			var maxDistance = distanceToCenter.normalized * maxPushRadius;
			entity.characterController.Move((maxDistance - distanceToCenter) * pushPower * Time.deltaTime);
			
			if(skillStage.isRuneDetonator)
				entity.DetonateRunes(caster);
		} else {
			AoEStop(entity);
		}
	}

	// The AoE effect on game objects
	protected override void AoEHit(Rigidbody obj) {
		obj.AddExplosionForce(400f, position, radius);
	}
	
	// Reset control over movement
	protected override void AoEStop(Entity entity) {
		entity.hasControlOverMovement = true;
	}
}
