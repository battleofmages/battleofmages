using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class NetworkShadowData : MonoBehaviour {
		[SerializeField] protected Player player;
		[SerializeField] protected ProxyMovement proxyMovement;
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
				latency = player.Latency.OneWay;
			} else {
				latency = Player.Main.Latency.OneWay;
			}

			gameObject.transform.position = proxyMovement.CalculatePosition(player.RemotePosition, player.RemoteDirection, latency);
		}
	}
}
