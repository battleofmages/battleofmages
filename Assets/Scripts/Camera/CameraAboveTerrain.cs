using UnityEngine;
using System.Collections;

public class CameraAboveTerrain : MonoBehaviour {
	public Transform camTransform;
	public Transform yOffset;
	public float camHeight = 3.0f;
	//public float lerpSpeed = 8.0f;
	
	private Vector3 lerpToPos;
	private GameObject map;
	
	// Update
	void Update () {
		if(Player.main == null)
			return;
		
		if(map == null) {
			map = GameObject.FindGameObjectWithTag("Map");
			
			if(map == null)
				return;
		}
		
		//Vector3 localPos = transform.localPosition;
		transform.localPosition = Cache.vector3Zero;
		
		Vector3 centerPos = yOffset.position;
		Vector3 camPos = camTransform.position;
		//camPos.y -= camHeight;
		
		RaycastHit hit;
		//Ray ray = new Ray(camPos, playerPos - camPos);
		int layerMask = (1 << map.layer);
		
		if(Physics.Linecast(centerPos, camPos, out hit, layerMask)) { //Physics.Raycast(ray, out hit, 100, layerMask)) {
			Vector3 diffToHitPoint = (hit.point - camPos);
			Vector3 diffToHitPointNormalized = diffToHitPoint.normalized;
			
			/*if(hit.collider is TerrainCollider) {
				lerpToPos = (hit.point - camPos) + diffToHitPointNormalized * camHeight; //Vector3.Lerp(localPos, hit.normal * (hit.point - camPos).magnitude, Time.deltaTime * lerpSpeed);			
			} else {
				lerpToPos = (hit.point - camPos) + diffToHitPointNormalized * camHeight;
			}*/
			
			lerpToPos = (hit.point - camPos) + diffToHitPointNormalized * camHeight;
		} else {
			lerpToPos = Cache.vector3Zero;
		}
		
		//transform.localPosition = Vector3.Lerp(localPos, lerpToPos, Time.deltaTime * lerpSpeed);
		transform.localPosition = lerpToPos;
	}
}
