using UnityEngine;

public class LookAtCameraYOnly : MonoBehaviour {
	Vector3 camPos;
	Vector3 diff;
	
	// Update is called once per frame
	void Update() {
		if(Camera.main == null)
			return;

		camPos = Camera.main.transform.position;
		diff = camPos - transform.position;
		diff.x = diff.z = 0f;
		
		transform.LookAt(camPos - diff);
		transform.Rotate(0, 180, 0);
	}
}