using System;
using UnityEngine;

namespace BoM {
	public class Main : MonoBehaviour {
		private void Start() {
#if UNITY_EDITOR
			if(ParrelSync.ClonesManager.IsClone()) {
				var accountId = ParrelSync.ClonesManager.GetArgument();
				Network.Client.Start(accountId);
			} else {
				Network.Host.Start("id0");
			}
#elif UNITY_SERVER
			Network.Server.Start();
#else
			Network.Client.Start("id1");
#endif
		}
	}
}
