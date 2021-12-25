using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Rotation : NetworkBehaviour {
		public Transform model;
		public float speed;
		private Quaternion targetRotation;
		private IController movement { get; set; }

		public override void OnNetworkSpawn() {
			if(IsOwner) {
				movement = GetComponent<OwnerMovement>();
			} else {
				movement = GetComponent<ProxyMovement>();
			}
		}

		private void Update() {
			var direction = movement.direction;
			direction.y = 0f;

			if(direction != Vector3.zero) {
				targetRotation = Quaternion.LookRotation(direction);
			}

			model.rotation = Quaternion.Slerp(
				model.rotation,
				targetRotation,
				Time.deltaTime * speed
			);
		}
	}
}
