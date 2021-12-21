using System;
using UnityEngine;

namespace BoM {
	public class Main : MonoBehaviour {
		private void Start() {
#if UNITY_EDITOR
			if(ParrelSync.ClonesManager.IsClone()) {
				var projectPath = ParrelSync.ClonesManager.GetCurrentProjectPath();
				var parts = projectPath.Split("_clone_");
				var cloneId = Int32.Parse(parts[parts.Length - 1]);
				var accountId = $"id{cloneId + 1}";
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
