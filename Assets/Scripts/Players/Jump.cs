using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class Jump : NetworkBehaviour {
		public Gravity gravity;
		public float JumpHeight;
		private float jumpSpeed;

		private void Start() {
			jumpSpeed = Mathf.Sqrt(JumpHeight * 2 * -Physics.gravity.y);
		}

		public bool TryJump() {
			if(!enabled) {
				return false;
			}

			if(!gravity.CanJump) {
				return false;
			}

			gravity.Speed = jumpSpeed;
			return true;
		}

		[ClientRpc]
		public void JumpClientRpc() {
			if(!enabled) {
				return;
			}

			if(IsOwner || IsServer) {
				return;
			}

			TryJump();
		}

		[ServerRpc]
		public void JumpServerRpc() {
			if(!enabled) {
				return;
			}

			if(IsOwner && IsHost) {
				JumpClientRpc();
				return;
			}

			if(!TryJump()) {
				return;
			}

			JumpClientRpc();
		}
	}
}
