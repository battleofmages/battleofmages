using UnityEngine;

namespace BoM {
	// Data
	public class MainData : MonoBehaviour {
		public Network.Host Host;
		public Network.Server Server;
		public Network.Client Client;
	}

	// Logic
	public class Main : MainData {
		private void Start() {
#if UNITY_EDITOR
			if(ParrelSync.ClonesManager.IsClone()) {
				Client.AccountId = ParrelSync.ClonesManager.GetArgument();
				Client.Start();
			} else {
				Host.Start();
			}
#elif UNITY_SERVER
			Server.Start();
#else
			Client.Start();
#endif
		}
	}
}
