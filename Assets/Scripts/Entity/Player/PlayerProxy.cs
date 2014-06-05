using UnityEngine;
using System.Collections;

public class PlayerProxy : PlayerOnClient {
	// Awake
	protected override void Awake() {
		base.Awake();
	}
	
	// Start
	void Start() {
		InvokeRepeating("UpdateDistanceToCamera", 0.001f, 0.1f);
	}
	
#region Update
	// Update my position to the position the server sent me
	void Update() {
		// Movement
		UpdateProxyMovement();
		
		// Animations
		UpdateSkillAnimations();
		
		// Map boundaries
		StayInMapBoundaries();
	}
	
	// UpdateDistanceToCamera
	void UpdateDistanceToCamera() {
		// Fade over distance
		float camDistance = Vector3.Distance(camTransform.position, myTransform.position);
		float alpha = Config.instance.playerLabelAlphaWithDistance.Evaluate(camDistance / Config.instance.entityVisibilityDistance);
		hudColor = new Color(1f, 1f, 1f, alpha);
		nameLabel.textColor = new Color(nameLabel.textColor.r, nameLabel.textColor.g, nameLabel.textColor.b, alpha);
	}
#endregion
}
