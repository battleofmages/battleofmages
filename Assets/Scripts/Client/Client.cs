using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour {
	private System.Net.IPEndPoint server;

	// Start
	void Start () {
		uLink.Network.publicKey = new uLink.PublicKey(
@"r6tfUZ4YwT16YA4GGXN7xdd1A5rTIwSi5Yn6euIGK/Z0WrTkkgBVHnMtxFLtqmh8kh
aUbPeoIIU5C/zwZitj5Ef7pd91LTrabTIDd4T9V/eMo2wHXfOxLJm6oC372pvMFQKL
Cr4/8FWgrh1kGpVXYg3a5HzckLpC286Y7b07g7aAJJv/WcUJfuxtmR+0tN6eUs1evD
qFK2VRdmlB9sWSpUDZ/5LGdWBof2MDLFBfKZ7pg/av6fXKrfufgXRHu/7sn/Awrysr
3Lt+Ajj3f/9pOE39mwIqioSUjBjXoh9N+zacGxv+iKlPOQsGMs8kqTpVHRrjUs8BHh
uzf/lC9NYarw==", @"EQ=="
		);

		// Encryption
		uLink.Network.InitializeSecurity(true);

		// Test
		uLink.Network.Connect("localhost", 7000);
	}

	// Connected to server
	void uLink_OnConnectedToServer(System.Net.IPEndPoint nServer) {
		LogManager.General.Log("Successfully connected to server: " + nServer);

		server = nServer;
	}

	// Disconnected from server
	void uLink_OnDisconnectedFromServer(uLink.NetworkDisconnection mode) {
		if(mode == uLink.NetworkDisconnection.LostConnection) {
			LogManager.General.Log("Lost connection to the server " + server + " after timeout");
		} else {
			LogManager.General.Log("Lost connection to the server " + server);
		}
	}

	// On application quit we close log files
	void OnApplicationQuit() {
		LogManager.CloseAll();
	}

#region RPCs (Test server)
	[RPC]
	void LoadMap(string mapName) {
		//GameManager.instance.LoadMap(mapName);
	}
#endregion
}
