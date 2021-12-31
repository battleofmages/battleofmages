using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class JumpData : NetworkBehaviour {
		[SerializeField] protected Gravity gravity;
		[SerializeField] protected float jumpHeight;
		protected float jumpSpeed;
	}

	// Logic
	public class Jump : JumpData {
		private void Start() {
			jumpSpeed = Mathf.Sqrt(jumpHeight * 2 * -Physics.gravity.y);
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
