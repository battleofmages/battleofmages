using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class NetworkShadow : MonoBehaviour {
		public Player player;
		public ProxyMovement proxyMovement;

		private void Awake() {
			player.account.NickChanged += nick => {
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
