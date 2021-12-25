using UnityEngine;

namespace BoM {
	public class Main : MonoBehaviour {
		public Network.Host host;
		public Network.Server server;
		public Network.Client client;

		private void Start() {
#if UNITY_EDITOR
			if(ParrelSync.ClonesManager.IsClone()) {
				client.accountId = ParrelSync.ClonesManager.GetArgument();
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
