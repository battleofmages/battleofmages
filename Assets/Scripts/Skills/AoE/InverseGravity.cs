using UnityEngine;

public class InverseGravity : AoEOverTime {
	public GameObject projectileSpawned;
	public double projectileInterval;
	public float rigidBodyUpForce;
	public float projectileYOffset;
	public float launchProjectilesDuration;
	
	private double lastProjectileTime;
	private float invertDirection = 1.0f;
	private float projectileStartPositionY;
	private Quaternion casterRotation;
	private bool createProjectiles;
	private Vector3 upVector;
	private double timeStarted;
	
	// OnAoEInit
	protected override void Init() {
		projectileStartPositionY = position.y + projectileYOffset;
		casterRotation = caster.GetLookRotation(this);
		upVector = -Physics.gravity;
		timeStarted = uLink.Network.time;
	}
	
	// The AoE effect on entities
	protected override void AoEHit(Entity entity) {
		if(entity.canBePulled) {
			entity.hasControlOverMovement = false;

			// TODO: Fix for multiple players
			if(createProjectiles) {
				SpawnProjectile(entity);
			}
			
			// Move up
			entity.characterController.Move(upVector * Time.deltaTime);
		} else {
			AoEStop(entity);
		}
	}

	// The AoE effect on game objects
	protected override void AoEHit(Rigidbody obj) {
		obj.AddForce(-Physics.gravity * rigidBodyUpForce);
		obj.AddTorque(
			Random.Range(0, 2),
			Random.Range(0, 2),
			Random.Range(0, 2)
		);
	}

	// OnAoEUpdateStart
	protected override void UpdateStart() {
		var now = uLink.Network.time;
		var timeSpent = now - timeStarted;
		var timeSinceLastLaunch = now - lastProjectileTime;

		createProjectiles = (timeSpent < launchProjectilesDuration) && (timeSinceLastLaunch >= projectileInterval);
	}

	// OnAoEUpdateEnd
	protected override void UpdateEnd() {
		if(createProjectiles) {
			lastProjectileTime = uLink.Network.time;
		}
	}
	
	// Spawns a projectile flying towards the player
	void SpawnProjectile(Entity target) {
		Vector3 toPosition = target.position + new Vector3(0, target.height + upVector.y * 0.4f, 0);
		Vector3 offset = casterRotation * new Vector3(35 * invertDirection, 0, 0);
		Vector3 fromPosition = new Vector3(toPosition.x + offset.x, projectileStartPositionY, toPosition.z + offset.z);
		
		invertDirection = -invertDirection;
		
		//Debug.Log ("Spawn projectile " + fromPosition + " -> " + toPosition);
		
		GameObject clone;
		SkillInstance inst;
		
		SpawnSkillPrefab(
			projectileSpawned,
			fromPosition,
			Quaternion.LookRotation((toPosition - fromPosition) + new Vector3(0, -20, 0)),
			out clone,
			out inst
		);

		inst.hitPoint = toPosition;
		Destroy(clone, 2.0f);
	}
	
	// Reset control over movement
	protected override void AoEStop(Entity entity) {
		entity.hasControlOverMovement = true;
	}
}
