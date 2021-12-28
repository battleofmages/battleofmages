using Unity.Netcode;
using UnityEngine;

namespace BoM.Network {
	[CreateAssetMenu(fileName = "Host", menuName = "BoM/Host", order = 52)]
	public class Host : ScriptableObject {
		[SerializeField] private Server Server;
		[SerializeField] private Client Client;

		public void Start() {
			NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(Client.AccountId);

			Server.Ready += () => {
				NetworkManager.Singleton.StartHost();
				Server.Listen();
				Client.Listen();
			};

			Server.Init();
		}
	}
}
