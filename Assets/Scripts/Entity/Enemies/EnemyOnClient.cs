using UnityEngine;

public class EnemyOnClient : Enemy {
	// Awake
	protected override void Awake() {
		base.Awake();

		healthBarWidth = 48;
	}
	
	// OnGUI
	void OnGUI() {
		if(Player.main == null)
			return;

		if(!hasTarget && (Player.main as PlayerOnClient).selectedEntity != this)
			return;
		
		GUI.depth = 3000;
		
		// Visibility check
		if(isVisible && isAlive && charGraphicsBody.renderer.isVisible) {
			// Health and energy bars
			entityGUI.Draw();
		}
	}
	
	// FixedUpdate
	void FixedUpdate() {
		// Energy
		ApplyEnergyDrain();

		// Skill effects
		UpdateSkillEffects();
		
		// Movement
		UpdateProxyMovement();

		// Animations
		UpdateSkillAnimations();
	}
	
	// Server sent us new movement data
	[RPC]
	protected void M(Vector3 position, Vector3 direction, ushort rotationY, uLink.NetworkMessageInfo info) {
		direction.Normalize();
		var extraPolationOffset = direction * moveSpeed * info.GetPacketArrivalTime();
		//if(id == 0)
		//	Debug.Log(direction + ", " + this.moveSpeed + ", " + info.GetPacketArrivalTime() + ", " + extraPolationOffset);

		serverPosition = position + extraPolationOffset;
		serverRotationY = rotationY * Cache.rotationShortToFloat;
		interpolationStartPosition = myTransform.position;
		
		proxyInterpolationTime = 0f;

		// Animation
		animator.SetBool("Moving", direction != Cache.vector3Zero);
	}
}
