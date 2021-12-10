using UnityEngine;
using Unity.Netcode;

public class Proxy : NetworkBehaviour {
	public Player player;
	public Camera cam;
	public float movementPrediction;

	private void OnEnable() {
		CameraManager.AddCamera(cam);
	}

	private void OnDisable() {
		CameraManager.RemoveCamera(cam);
	}

	private void FixedUpdate() {
		var direction = player.Position - transform.position;
		var prediction = player.Direction * movementPrediction;
		player.Move(direction + prediction);
	}
}