using UnityEngine;
using System.Collections;

public class InstantConnect : MonoBehaviour {
	public string serverIP = "127.0.0.1";
	public int serverPort = 6000;
	
	// Start
	void Start() {
#if !UNITY_WEBPLAYER
		if(Login.instance == null) {
			uLink.Network.Connect(serverIP, serverPort);
		}
#endif
	}
}
