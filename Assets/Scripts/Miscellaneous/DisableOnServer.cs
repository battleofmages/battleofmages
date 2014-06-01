using UnityEngine;
using System.Collections;

public class DisableOnServer : MonoBehaviour {
	void uLink_OnServerInitialized() {
		MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
		
		foreach(MonoBehaviour c in comps) {
			c.enabled = false;
		}
	}
}
