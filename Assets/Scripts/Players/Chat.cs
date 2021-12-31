using System;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class ChatData : NetworkBehaviour {
		[SerializeField] protected Player player;
	}

	// Logic
	public class Chat : ChatData {
		public static event Action<Player, string> MessageReceived;

		public void SubmitMessage(string message) {
			if(message == "/dc") {
				NetworkManager.Shutdown();
				return;
			}

			if(message.StartsWith("/maxfps ")) {
				var fps = int.Parse(message.Split(' ')[1]);
				Application.targetFrameRate = fps;
				return;
			}

			SendChatMessageServerRpc(message);
		}

		[ServerRpc]
		public void SendChatMessageServerRpc(string message) {
			ReceiveMessageClientRpc(message);
		}

		[ClientRpc]
		public void ReceiveMessageClientRpc(string message) {
			MessageReceived?.Invoke(player, message);
		}
	}
}
