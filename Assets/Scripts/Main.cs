using UnityEngine;

namespace BoM {
	public class Main : MonoBehaviour {
		private void Start() {
#if UNITY_EDITOR
			if(ParrelSync.ClonesManager.IsClone()) {
				Debug.Log(ParrelSync.ClonesManager.GetCurrentProjectPath());
				Network.Client.Start("id1");
			} else {
				Network.Host.Start("id0");
			}
#elif UNITY_SERVER
			Network.Server.Start();
#else
			Network.Client.Start("id2");
#endif
		}
	}
}
