using Unity.Netcode;

namespace BoM.Players {
	public class Jump : NetworkBehaviour {
		public Gravity gravity;

		[ClientRpc]
		public void JumpClientRpc() {
			if(IsOwner || IsServer) {
				return;
			}

			if(!gravity.Jump()) {
				return;
			}
		}

		[ServerRpc]
		public void JumpServerRpc() {
			if(IsOwner && IsHost) {
				JumpClientRpc();
				return;
			}

			if(!gravity.Jump()) {
				return;
			}

			JumpClientRpc();
		}
	}
}
