using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class SnapData : NetworkBehaviour {
		[SerializeField] protected Player player;
		[SerializeField] protected Movement movement;
		[SerializeField] protected float minDistanceSqr;
		[SerializeField] protected float coolDown;

		protected float maxDistanceSqr;
		protected float lastSnap;
	}

	// Logic
	public class Snap : SnapData {
		private void Start() {
			var maxDistance = movement.Speed * coolDown;
			maxDistanceSqr = maxDistance * maxDistance;
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
