
using UnityEngine;

namespace BoM {
	public class Main : MonoBehaviour {
		private void Start() {
#if UNITY_EDITOR
			if(ParrelSync.ClonesManager.IsClone()) {
				Network.Client.Start();
			} else {
				Network.Host.Start();
			}
#elif UNITY_SERVER
			Network.Server.Start();
#else
			Network.Client.Start();
#endif
		}
	}
}
