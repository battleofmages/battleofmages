using BoM.Core;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class RotationData : NetworkBehaviour {
		public Transform Center;

		[SerializeField] protected float speed;
		[SerializeField] protected Player player;
		[SerializeField] protected Flight flight;

		protected Vector3 direction;
		protected Quaternion targetRotation;
		protected IController movement { get; set; }
	}

	// Logic
	public class Rotation : RotationData {
		public override void OnNetworkSpawn() {
			if(IsOwner) {
				movement = GetComponent<OwnerMovement>();
			}
		}

		public void SetRotation(Quaternion newRotation) {
			Center.rotation = newRotation;
			targetRotation = newRotation;
		}

		private void Update() {
			if(IsOwner) {
				direction = movement.Direction;
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

			Center.rotation = Quaternion.Slerp(
				Center.rotation,
				targetRotation,
				Time.deltaTime * speed
			);
		}
	}
}
