using UnityEngine;
using Unity.Netcode;

public class Proxy : NetworkBehaviour {
	public Player player;
	public Camera cam;
	public Transform model;
	public Animator animator;
	public float rotationSpeed;
	public float movementPrediction;

	private void OnEnable() {
		CameraManager.AddCamera(cam);
	}

	private void OnDisable() {
		CameraManager.RemoveCamera(cam);
	}

	private void FixedUpdate() {
		UpdatePosition();
		UpdateRotation();
		UpdateAnimation();
	}

	public void UpdatePosition() {
		var direction = player.Position - transform.position;
		var prediction = player.Direction * movementPrediction;
		var finalDirection = direction + prediction;

		finalDirection.y = 0f;
		player.Move(finalDirection);
	}

	public void UpdateRotation() {
		if(player.Direction == Vector3.zero) {
			return;
		}

		model.rotation = Quaternion.Slerp(
			model.rotation,
			Quaternion.LookRotation(player.Direction),
			Time.deltaTime * rotationSpeed
		);
	}

	public void UpdateAnimation() {
		animator.SetFloat("Speed", player.Direction.sqrMagnitude);
	}

#region RPC
	[ClientRpc]
	public void JumpClientRpc() {
		if(IsOwner) {
			return;
		}
		
		player.Jump();
	}
#endregion
}