using UnityEngine;
using Unity.Netcode;

public class Proxy : NetworkBehaviour {
	public Player player;
	public Camera cam;
	public float interpolationTime;
	private float interpolationSpeed;

	private void OnEnable() {
		CameraManager.AddCamera(cam);

		if(interpolationTime != 0f) {
			interpolationSpeed = 1f / interpolationTime;
		}
	}

	private void OnDisable() {
		CameraManager.RemoveCamera(cam);
	}

	private void FixedUpdate() {
		if(transform.position == player.Position) {
			return;
		}

		var distance = player.Position - transform.position;
		float speed = 1f;

		if(interpolationTime != 0f) {
			speed = Mathf.Clamp(interpolationSpeed * Time.fixedDeltaTime, 0f, 1f);
		}

		player.controller.Move(distance * speed);
	}
}