using UnityEngine;
using BoM.Input;

public class RayCaster : MonoBehaviour {
	private Camera cam;
	private Ray ray;
	private RaycastHit _hit;

	// Start
	void Start() {
		cam = Camera.main;
	}

	// Update
	void Update() {
		// Raycast
		ray = cam.ViewportPointToRay(InputManager.GetRelativeMousePositionToScreen());
		
		// Do the raycast
		if(Physics.Raycast(ray, out _hit, Config.instance.raycastMaxDistance)) {
			
		}
	}

#region Properties
	// Ray cast hit
	public RaycastHit hit {
		get {
			return _hit;
		}
	}
#endregion
}
