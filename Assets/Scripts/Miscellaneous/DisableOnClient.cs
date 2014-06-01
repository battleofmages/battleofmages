using UnityEngine;
using System.Collections;

public class DisableOnClient : MonoBehaviour {
	void uLink_OnConnectedToServer(System.Net.IPEndPoint server) {
		MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
		
		foreach(MonoBehaviour c in comps) {
			c.enabled = false;
		}
	}
}
