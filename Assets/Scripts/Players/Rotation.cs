using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Rotation : NetworkBehaviour {
		public Player player;
		public Transform center;
		public float speed;
		public Flight flight;
		private Vector3 direction;
		private Quaternion targetRotation;
		private IController movement { get; set; }

		public override void OnNetworkSpawn() {
			if(IsOwner) {
				movement = GetComponent<OwnerMovement>();
			}
		}

		public void SetRotation(Quaternion newRotation) {
			center.rotation = newRotation;
			targetRotation = newRotation;
		}

		private void Update() {
			if(IsOwner) {
				direction = movement.direction;
			} else {
				direction = player.RemoteDirection;
			}

			if(direction != Const.ZeroVector) {
				targetRotation = Quaternion.LookRotation(direction);
			}

			if(!flight.enabled) {
				var eulerAngles = targetRotation.eulerAngles;

				if(eulerAngles.x != 0f || eulerAngles.z != 0f) {
					targetRotation.eulerAngles = new Vector3(0f, eulerAngles.y, 0f);
				}
			}

			center.rotation = Quaternion.Slerp(
				center.rotation,
				targetRotation,
				Time.deltaTime * speed
			);
		}
	}
}
