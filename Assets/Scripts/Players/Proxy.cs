using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Proxy : NetworkBehaviour {
		public Player player;
		public Camera cam;
		public Transform model;
		public float rotationSpeed;
		public float movementPrediction;

		private void OnEnable() {
			CameraManager.AddCamera(cam);
		}

		private void OnDisable() {
			CameraManager.RemoveCamera(cam);
		}

		private void Update() {
			UpdateRotation();
		}

		private void FixedUpdate() {
			UpdatePosition();
		}

		public void UpdatePosition() {
			var direction = player.RemotePosition - transform.position;
			var prediction = player.RemoteDirection * movementPrediction;
			var finalDirection = direction + prediction;

			finalDirection.y = 0f;
			player.Move(finalDirection);
		}

		public void UpdateRotation() {
			if(player.RemoteDirection == Vector3.zero) {
				return;
			}

			model.rotation = Quaternion.Slerp(
				model.rotation,
				Quaternion.LookRotation(player.RemoteDirection),
				Time.deltaTime * rotationSpeed
			);
		}

	#region RPC
		[ClientRpc]
		public void JumpClientRpc() {
			if(IsOwner) {
				return;
			}
			
			if(!player.gravity.Jump()) {
				return;
			}
		}
	#endregion
	}
}