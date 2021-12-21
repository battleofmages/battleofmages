using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	public class Client: NetworkBehaviour {
		public Player player;
		public Server server;
		public Transform model;
		public float rotationSpeed;
		public Vector3 inputDirection { get; set; }
		private CustomMessagingManager messenger;
		private Vector3 direction;
		private Quaternion targetRotation;
		private Vector3 lastPositionSent;
		private Vector3 lastDirectionSent;

		private void OnEnable() {
			if(IsOwner) {
				server.ReadyServerRpc();
			}
		}

		public override void OnNetworkSpawn() {
			messenger = NetworkManager.Singleton.CustomMessagingManager;
		}

		private void Update() {
			UpdateRotation();
		}

		private void FixedUpdate() {
			UpdatePosition();
			SendPositionToServer();
		}

		private void UpdatePosition() {
			direction = player.cam.transform.TransformDirection(inputDirection);
			direction.y = 0f;
			direction.Normalize();

			player.Move(direction);
		}

		private void UpdateRotation() {
			if(direction != Vector3.zero) {
				targetRotation = Quaternion.LookRotation(direction);
			}

			model.rotation = Quaternion.Slerp(
				model.rotation,
				targetRotation,
				Time.deltaTime * rotationSpeed
			);
		}

		private void SendPositionToServer() {
			if(IsServer) {
				player.RemotePosition = transform.position;
				player.RemoteDirection = direction;
				return;
			}

			if(messenger == null) {
				return;
			}

			if(transform.position == lastPositionSent && direction == lastDirectionSent) {
				return;
			}

			using FastBufferWriter writer = new FastBufferWriter(24, Allocator.Temp);
			writer.WriteValueSafe(transform.position);
			writer.WriteValueSafe(direction);

			var receiver = NetworkManager.Singleton.ServerClientId;
			var delivery = NetworkDelivery.Unreliable;

			if(direction == Vector3.zero) {
				delivery = NetworkDelivery.Reliable;
			}

			messenger.SendNamedMessage("client position", receiver, writer, delivery);

			lastPositionSent = transform.position;
			lastDirectionSent = direction;
		}
	}
}