using Unity.Netcode;
using UnityEngine;

namespace BoM.Network {
	[CreateAssetMenu(fileName="Host", menuName="BoM/Host", order=52)]
	public class Host : ScriptableObject {
		public Server server;
		public Client client;

		public void Start() {
			NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(client.accountId);

			server.Ready += () => {
				NetworkManager.Singleton.StartHost();
				server.Listen();
				client.Listen();
			};

			server.Init();
		}
	}
}
