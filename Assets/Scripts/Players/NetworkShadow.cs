using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class NetworkShadowData : MonoBehaviour {
		[SerializeField] protected Player player;
		[SerializeField] protected Movement movement;
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

			gameObject.transform.position = ProxyMovement.CalculatePosition(player.RemotePosition, player.RemoteDirection, movement.Speed, latency);
		}
	}
}
