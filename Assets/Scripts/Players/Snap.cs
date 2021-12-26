using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Snap : NetworkBehaviour {
		public Player player;
		public Movement movement;
		public float minDistanceSqr;
		public float coolDown;
		private float maxDistanceSqr;
		private float lastSnap;

		private void Start() {
			maxDistanceSqr = movement.speed * movement.speed * coolDown * coolDown;
		}

		private void FixedUpdate() {
			CheckSnap(player.RemotePosition);
		}

		private void CheckSnap(Vector3 remotePosition) {
			var difference = remotePosition - transform.position;
			var distanceSqr = difference.sqrMagnitude;

			if(distanceSqr > minDistanceSqr && distanceSqr < maxDistanceSqr && Time.time - lastSnap > coolDown) {
				transform.position = remotePosition;
				lastSnap = Time.time;
			}
		}
	}
}
