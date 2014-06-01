using UnityEngine;
using System.Collections.Generic;

public class DangerDetector : MonoBehaviour {
	// Number of projectiles that entered the collider
	private HashSet<Projectile> projectiles = new HashSet<Projectile>();
	private List<Projectile> projectilesToRemove = new List<Projectile>();

	// OnTriggerEnter
	void OnTriggerEnter(Collider coll) {
		var projectile = coll.GetComponent<Projectile>();

		if(projectile != null) {
			//Debug.Log("[ENTER] Projectile layer: " + coll.gameObject.layer);
			projectiles.Add(projectile);
		}
	}

	// OnTriggerExit
	void OnTriggerExit(Collider coll) {
		var projectile = coll.GetComponent<Projectile>();

		if(projectile != null) {
			//Debug.Log("[EXIT] Projectile layer: " + coll.gameObject.layer);
			projectiles.Remove(projectile);
		}
	}

	// OnDisable
	void OnDisable() {
		projectiles.Clear();
	}

	// FixedUpdate
	void FixedUpdate() {
		// Check if our objects are still valid
		foreach(var projectile in projectiles) {
			if(projectile == null)
				projectilesToRemove.Add(projectile);
		}

		// Remove dead ones
		foreach(var projectile in projectilesToRemove) {
			projectiles.Remove(projectile);
		}

		projectilesToRemove.Clear();
	}

	// Detected danger
	public bool detectedDanger {
		get {
			return projectiles.Count > 0;
		}
	}
}
