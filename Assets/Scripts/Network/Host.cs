using Unity.Netcode;

namespace BoM.Network {
	public static class Host {
		public static void Start(string accountId) {
			NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.UTF8.GetBytes(accountId);

			Server.Ready += () => {
				NetworkManager.Singleton.StartHost();
				Server.Listen();
				Client.Listen();
			};

			Server.Init();
		}
	}
}