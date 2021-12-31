using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class NetworkShadowData : MonoBehaviour {
		public Player player;
		public ProxyMovement proxyMovement;
	}

	// Logic
	public class NetworkShadow : NetworkShadowData {
		private void Awake() {
			player.Account.NickChanged += nick => {
				gameObject.name = nick + " - Shadow";
			};
		}

		private void Update() {
			float latency;

			if(NetworkManager.Singleton.IsServer) {
				latency = player.Latency.oneWay;
			} else {
				latency = Player.Main.Latency.oneWay;
			}

			gameObject.transform.position = proxyMovement.CalculatePosition(player.RemotePosition, player.RemoteDirection, latency);
		}
	}
}
