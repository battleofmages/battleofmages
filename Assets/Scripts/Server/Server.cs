using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {
	public int serverFrameRate;
	public int defaultServerPort;

	private int serverPort;
	private int maxPlayerCount = 10;

	// Start
	void Start() {
		// Default
		serverPort = defaultServerPort;

		// No audio
		AudioListener.pause = true;

		// Init server
		LogManager.General.Log("Initializing the server on port " + serverPort);
		uLink.Network.InitializeServer(maxPlayerCount, serverPort);
		
		// Encryption
		LogManager.General.Log("Initializing security");
		uLink.Network.InitializeSecurity(true);
	}

	// Server successfully initialized
	void uLink_OnServerInitialized() {
		LogManager.General.Log("Server successfully started!");
		
		// Set private key
		Server.InitPrivateKey();

		// Disable network emulation on server
		LogManager.General.Log("Disabling network emulation");
		NetworkHelper.DisableNetworkEmulation();

		// Fastest quality level on server
		LogManager.General.Log("Enabling lowest quality level");
		QualitySettings.SetQualityLevel(0);
		
		// Lower CPU consumption
		LogManager.General.Log("Setting frame rate to " + serverFrameRate);
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = serverFrameRate;
	}

	// Player connected
	void uLink_OnPlayerConnected(uLink.NetworkPlayer netPlayer) {
		LogManager.General.Log("Player successfully connected from " + netPlayer.ipAddress + ":" + netPlayer.port);
	}
	
	// Player disconnected
	void uLink_OnPlayerDisconnected(uLink.NetworkPlayer netPlayer) {
		LogManager.General.Log("Player " + netPlayer.id + " disconnected!");
		
		uLink.Network.DestroyPlayerObjects(netPlayer);
		uLink.Network.RemoveRPCs(netPlayer);
		uLink.Network.RemoveInstantiates(netPlayer);
	}

	// On application quit we close log files
	void OnApplicationQuit() {
		LogManager.CloseAll();
	}
	
	// InitPrivateKey
	public static void InitPrivateKey() {
		// TODO: Load private key dynamically from a file
		LogManager.General.Log("Initializing private key");
		uLink.Network.privateKey = new uLink.PrivateKey(
@"r6tfUZ4YwT16YA4GGXN7xdd1A5rTIwSi5Yn6euIGK/Z0WrTkkgBVHnMtxFLtqmh8kh
aUbPeoIIU5C/zwZitj5Ef7pd91LTrabTIDd4T9V/eMo2wHXfOxLJm6oC372pvMFQKL
Cr4/8FWgrh1kGpVXYg3a5HzckLpC286Y7b07g7aAJJv/WcUJfuxtmR+0tN6eUs1evD
qFK2VRdmlB9sWSpUDZ/5LGdWBof2MDLFBfKZ7pg/av6fXKrfufgXRHu/7sn/Awrysr
3Lt+Ajj3f/9pOE39mwIqioSUjBjXoh9N+zacGxv+iKlPOQsGMs8kqTpVHRrjUs8BHh
uzf/lC9NYarw==", @"EQ==", @"zSlbiDsMfiApy6/QXugNNh3/nChL9dSgJCrToc
3f+CR6RZCUO9DnpChW6mh4TSzmj6pujDWiGAc7IXYg/Ufe+zjAMN8a5gHBuPN/PBP2
d+SPooIFMdjc2mVEEXWMfzMlmnRMTWe45oiBDOr0r60MjHQkd/kbNHiWCB39Qj++Ev
8=", @"2zMmZzIuY8kdbWafZrlDNJv/4dbl7po38pBpEAah+d3GrPrwHyFqrfST5uS
RVuNZkmg2yns0oA4icrqtnZPQoChJU3LfTGayhYL+YEi3coVdq84BvIagFHDIgtdiO
jig5Mx+k1czoxmXmzPbAFVejE2diBCzXWowAs6FmVaF6FE=", @"nONkHOHcYHLyyO
/bk96gsOnDd2ob+DkvKrdWisqcNjoDJiNELb3eUFsVSddrDdcKqhjrH+zHP40PGZaR
sqBfOJTPNG5f3RBm9thSPQA08kVex5Caj4e38k1wSZYgJQj+o0nf/vT2zmhir4Z+4L
GCEQ2FTK9vKB/6QnFJMqk2/3c=", @"tIR5+qHL2bSu0pC/gcW+4Af/5ylyD8pMMSu
h0PZnRj4rJQrj3WbQUwWm+mHhGmDgWnPw4vwNOIQcXnueCUyNsRIeRLj0IM0LfQJ3I
h3EQDGYb15byG7eLvN380f2ikzAvGwsAN5mwo2L6TnDaa+3Rl4JQuCTtldy1SKMI+z
mv1E=", @"Uy8Nk2p+jc5tvLJyg8Tw1LyBREC3WL5x7h6n8b0a2TtxXVTJdYPtPBxQ
FnWRLBiPf5vxTmXJPYDbSX9VvY6X8Yex4Jqb2IZx+FYmgqKgza0Rccr03hdG6hRVul
ds1MBDaErC+sO3aaAXM0PsHYyYjgfuIirz559FIdQfAsoW750=", @"M6rfzLYHR+T
nweYBy0AVWE5ttcQf+z2ZUpH+YGCYZ0h8kyYlG+H66taU/YHNblr3dkLgXEjXGJ+nX
eD7aVgOUjNKA5wTZ6fl4+GIjJCGv4ULPx/F7nTZwdLciXbvuMRpM1sZ1fu4c9zzBgi
k+MKDHNbmBveMKpEiuR6lcxmJ+Y+KwmIErvt1DeVLrfeCjUfxDtMj+1zj+ziYzaOx6
/XF1ZaPXl6YF3ydLAU72EH6yzn+bJ90mFRu4ZflNdZgBbz5xzTJSJXvBBT0koJmGXJ
SeO9CnZUv0GupNYsi3NINk5lx5dufWXPBpN3fb0siTy3kk1Zg91p7EXOL7yLTG9Cf0
Q=="
		);
	}
}
