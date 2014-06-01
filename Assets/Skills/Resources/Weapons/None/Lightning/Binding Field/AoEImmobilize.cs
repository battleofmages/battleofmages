using UnityEngine;
using System.Collections;

public class AoEImmobilize : AoEOverTime {
	// The AoE effect on entities
	protected override void AoEHit(Entity entity) {
		if(entity.canBePulled) {
			entity.hasControlOverMovement = false;
		} else {
			AoEStop(entity);
		}
	}

	// The AoE effect on game objects
	protected override void AoEHit(Rigidbody obj) {
		obj.velocity = Vector3.zero;
		obj.Sleep();
	}
	
	// Reset control over movement
	protected override void AoEStop(Entity entity) {
		entity.hasControlOverMovement = true;
	}
}

