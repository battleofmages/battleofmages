using Unity.Netcode;
using UnityEngine;

namespace BoM.Network {
	// Data
	public class HostData : ScriptableObject {
		[SerializeField] protected Server server;
		[SerializeField] protected Client client;
	}

	// Logic
	[CreateAssetMenu(fileName = "Host", menuName = "BoM/Host", order = 52)]
	public class Host : HostData {
		public void Start() {
			NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(client.AccountId);

			server.Ready += () => {
				NetworkManager.Singleton.StartHost();
				server.Listen();
				client.Listen();
			};

			server.Init();
		}
	}
}
