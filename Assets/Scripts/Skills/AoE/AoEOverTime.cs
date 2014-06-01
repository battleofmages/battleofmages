using UnityEngine;
using System.Collections.Generic;

public class AoEOverTime : SkillInstance {
	// Settings
	public float radius;
	public float duration;

	// Cache
	protected Vector3 position;
	private int layerMask;

	private bool activeAoE = true;
	private Dictionary<Entity, bool> affectedEntities = new Dictionary<Entity, bool>();

	// Awake
	void Awake() {
		Invoke("StopAoE", duration);
	}
	
	// Start
	void Start() {
		position = transform.position;
		layerMask = caster.enemiesLayerMask;
		
		Init();
	}

	// FixedUpdate
	void FixedUpdate() {
		if(!activeAoE)
			return;
		
		// Before updating
		UpdateStart();
		
		//if(uLink.Network.isServer) {
		Collider[] colliders = Physics.OverlapSphere(position, radius, layerMask);
		
		foreach(Collider coll in colliders) {
			var entity = coll.GetComponent<Entity>();
			
			if(entity) {
				AoEHit(entity);
				
				affectedEntities[entity] = true;
			} else {
				var rigidBody = coll.rigidbody;

				if(rigidBody != null)
					AoEHit(rigidBody);
			}
		}
		
		// After updating
		UpdateEnd();
	}
	
	// Reset control over movement
	void StopAoE() {
		activeAoE = false;
		
		foreach(var entity in affectedEntities.Keys) {
			//LogManager.General.Log("Resetting AoE for " + player.playerName);
			AoEStop(entity);
		}
		
		// Server doesn't have auto destruct
		if(uLink.Network.isServer) {
			Destroy(gameObject);
		}
	}
	
	// Needs to be implemented by derived class
	protected virtual void Init() {}
	protected virtual void AoEHit(Entity entity) {}
	protected virtual void AoEHit(Rigidbody obj) {}
	protected virtual void UpdateStart() {}
	protected virtual void UpdateEnd() {}
	protected virtual void AoEStop(Entity entity) {}
}
