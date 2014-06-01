using UnityEngine;
using System.Collections;

public class Teleport : SkillInstance {
	public float catchUpSpeed = 7.0f;
	public float safeOffsetNormal = 0.5f;
	private Transform myTransform;
	private Vector3 teleportTo;
	private float charHalfHeight;
	private Vector3 charHalfOffsetY;
	
	// Use this for initialization
	void Start () {
		charHalfHeight = this.caster.characterController.height / 2;
		charHalfOffsetY = new Vector3(0, charHalfHeight, 0);
		
		teleportTo = MapManager.StayInMapBoundaries(new Vector3(hitPoint.x, hitPoint.y + charHalfHeight, hitPoint.z));
		teleportTo += GetTerrainNormal() * safeOffsetNormal;
		
		this.caster.myTransform.position = teleportTo;
		this.caster.serverPosition = teleportTo;
		this.caster.clientPosition = teleportTo;
		this.caster.hasControlOverMovement = true;
		this.caster.immuneToPull += 1;
		this.caster.ignoreNewPositionEarlierThanTimestamp = uLink.Network.time;
		this.caster.disableSnappingToNewPosition += 1;
		
		// Prevent snapping to client position
		if(uLink.Network.isServer) {
			var pms = ((PlayerOnServer)(this.caster));
			pms.clientGrounded = false;
		}
		
		myTransform = this.transform;
		
		Destroy(this.gameObject, 1.3f);
	}
	
	// Update
	void Update() {
		Vector3 targetPos = this.caster.myTransform.position + charHalfOffsetY;
		
		myTransform.position = Vector3.Lerp(myTransform.position, targetPos, Time.deltaTime * catchUpSpeed);
	}
	
	// Get terrain normal
	Vector3 GetTerrainNormal() {
		RaycastHit hit;
		Ray ray = new Ray(teleportTo + Vector3.up * 10, Vector3.down);
		
		if(Physics.Raycast(ray, out hit, 20)) {
			return hit.normal;
		}
		
		return Vector3.up;
	}
	
	// On destroy
	void OnDestroy() {
		this.caster.immuneToPull -= 1;
		this.caster.disableSnappingToNewPosition -= 1;
	}
}
