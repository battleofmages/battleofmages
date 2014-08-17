using UnityEngine;

public class DisableOnServer : uLink.MonoBehaviour {
	// uLink_OnServerInitialized
	void uLink_OnServerInitialized() {
		MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
		
		foreach(MonoBehaviour c in comps) {
			c.enabled = false;
		}
	}
}
