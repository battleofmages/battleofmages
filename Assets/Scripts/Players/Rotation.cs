using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Rotation : NetworkBehaviour {
		public Player player;
		public Transform model;
		public float speed;
		private Quaternion targetRotation;

		private void Update() {
			UpdateRotation();
		}
		
		private void UpdateRotation() {
			if(player.RemoteDirection != Vector3.zero) {
				targetRotation = Quaternion.LookRotation(player.RemoteDirection);
			}

			model.rotation = Quaternion.Slerp(
				model.rotation,
				targetRotation,
				Time.deltaTime * speed
			);
		}
	}
}