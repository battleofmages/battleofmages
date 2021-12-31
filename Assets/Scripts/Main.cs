using UnityEngine;

namespace BoM {
	// Data
	public class MainData : MonoBehaviour {
		public Network.Host host;
		public Network.Server server;
		public Network.Client client;
	}

	// Logic
	public class Main : MainData {
		private void Start() {
#if UNITY_EDITOR
			if(ParrelSync.ClonesManager.IsClone()) {
				client.AccountId = ParrelSync.ClonesManager.GetArgument();
				client.Start();
			} else {
				host.Start();
			}
#elif UNITY_SERVER
			server.Start();
#else
			client.Start();
#endif
		}
	}
}
