using Unity.Netcode;

namespace BoM.Network {
	public static class Host {
		public static void Start() {
			Server.Ready += () => {
				NetworkManager.Singleton.StartHost();
				Server.Listen();
				Client.Listen();
			};

			Server.Init();
		}
	}
}