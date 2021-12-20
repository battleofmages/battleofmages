using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	public class Server: NetworkBehaviour {
		public Player player;
		private Vector3 lastPositionSent;
		private Vector3 lastDirectionSent;
		private CustomMessagingManager messenger;

		public override void OnNetworkSpawn() {
			messenger = NetworkManager.Singleton.CustomMessagingManager;
		}

		private void FixedUpdate() {
			BroadcastPosition();
		}

		public void BroadcastPosition() {
			if(transform.position == lastPositionSent && player.RemoteDirection == lastDirectionSent) {
				return;
			}

			using FastBufferWriter writer = new FastBufferWriter(32, Allocator.Temp);
			writer.WriteValueSafe(player.ClientId);
			writer.WriteValueSafe(transform.position);
			writer.WriteValueSafe(player.RemoteDirection);

			var delivery = NetworkDelivery.UnreliableSequenced;

			if(player.RemoteDirection == Vector3.zero) {
				delivery = NetworkDelivery.ReliableSequenced;
			}

			messenger.SendNamedMessageToAll("server position", writer, delivery);

			lastPositionSent = player.RemotePosition;
			lastDirectionSent = player.RemoteDirection;
		}

	#region RPC
		[ServerRpc]
		public void JumpServerRpc() {
			if(!player.gravity.Jump()) {
				return;
			}

			player.JumpClientRpc();
		}

		[ServerRpc]
		public void SendChatMessageServerRpc(string message) {
			player.ReceiveMessageClientRpc(message);
		}

		[ServerRpc]
		public async void UseSkillServerRpc(byte index, Vector3 cursorPosition) {
			if(IsClient) {
				return;
			}
			
			player.UseSkillClientRpc(index, cursorPosition);
			player.animations.Animator.SetBool("Attack", true);
			await Task.Delay(300);
			player.UseSkill(player.currentElement.skills[index], cursorPosition);
		}
	#endregion
	}
}