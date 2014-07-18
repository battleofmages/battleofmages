using UnityEngine;

public class CameraZoom : MonoBehaviour {
	public Transform distanceTo;
	public float zoomSpeed = 1f;
	public float interpolationSpeed = 1f;
	public float minDistance = 7.07f;
	public float maxDistance = 12.2f;
	public float yFactor = 0.5f;
	
	private Transform myTransform;
	private float distance;
	
	private MouseLook mouseLook;
	
	// Start
	void Start() {
		myTransform = transform;
		
		distance = Mathf.Abs(myTransform.localPosition.z);
	}
	
	// Update
	void Update() {
		/*if(mouseLook == null)
			mouseLook = GameObject.Find("CamPivot").GetComponent<MouseLook>();
		
		if(!mouseLook.enabled)
			return;*/

		float wheel = Input.GetAxis("Mouse ScrollWheel");

		// Zoom doesn't work while main menu is on or while the player is talking with an NPC
		if(MainMenu.instance.enabled || (Player.main != null && Player.main.talkingWithNPC))
			return;

		distance = Mathf.Clamp(distance - wheel * zoomSpeed, minDistance, maxDistance);
		
		var targetPosition = new Vector3(0, -(maxDistance - distance) * yFactor, -distance);
		myTransform.localPosition = Vector3.Lerp(myTransform.localPosition, targetPosition, Time.deltaTime * interpolationSpeed);
	}
}
