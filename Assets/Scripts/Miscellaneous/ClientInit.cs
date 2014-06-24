using uLink;
using UnityEngine;

public class ClientInit : uLink.MonoBehaviour {
	private bool disconnected;
	private System.Net.IPEndPoint server;
	
	// Awake
	void Awake() {
		disconnected = false;
		server = null;
	}
	
	// Start
	void Start() {
		uLink.Network.publicKey = new uLink.PublicKey(
@"r6tfUZ4YwT16YA4GGXN7xdd1A5rTIwSi5Yn6euIGK/Z0WrTkkgBVHnMtxFLtqmh8kh
aUbPeoIIU5C/zwZitj5Ef7pd91LTrabTIDd4T9V/eMo2wHXfOxLJm6oC372pvMFQKL
Cr4/8FWgrh1kGpVXYg3a5HzckLpC286Y7b07g7aAJJv/WcUJfuxtmR+0tN6eUs1evD
qFK2VRdmlB9sWSpUDZ/5LGdWBof2MDLFBfKZ7pg/av6fXKrfufgXRHu/7sn/Awrysr
3Lt+Ajj3f/9pOE39mwIqioSUjBjXoh9N+zacGxv+iKlPOQsGMs8kqTpVHRrjUs8BHh
uzf/lC9NYarw==", @"EQ==");
		
		// Encryption
		uLink.Network.InitializeSecurity(true);
		
		// Init codecs for quick client test mode
		if(Login.instance == null) {
			GameDB.InitCodecs();
		}
		
		// Set to desktop resolution
		//Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
		
		// Graphics settings
		UpdateGraphicsSettings();
	}
	
	// OnGUI
	void OnGUI() {
		if(disconnected) {
			GUIHelper.BeginBox(400, 72);
			GUILayout.Label("Disconnected from the game server.");
			if(server != null) {
				using(new GUIHorizontalCenter()) {
					if(GUIHelper.Button("Reconnect")) {
						LogManager.General.Log("Reconnecting to " + server.ToString());
						
						if(InGameLobby.instance != null) {
							// Real server
							InGameLobby.instance.ConnectToGameServerByIPEndPoint(server);
						} else {
							// Test server
							Application.LoadLevel("Client");
							uLink.Network.Connect(server);
						}
					}
				}
			}
			
			GUIHelper.EndBox();
		}
	}
	
	// Connected to server
	void uLink_OnConnectedToServer(System.Net.IPEndPoint nServer) {
		LogManager.General.Log("Successfully connected to server: " + nServer.ToString());
		
		if(Time.timeScale != 1f) {
			LogManager.General.Log("Time scale: 100%");
			Time.timeScale = 1f;
		}
		
		disconnected = false;
		server = nServer;

		Screen.showCursor = false;
	}
	
	// Disconnected from server
	void uLink_OnDisconnectedFromServer(uLink.NetworkDisconnection mode) {
		if(mode == uLink.NetworkDisconnection.LostConnection) {
			LogManager.General.Log("Lost connection to the server after timeout");
		} else {
			LogManager.General.Log("Lost connection to the server");
		}
		
		LogManager.General.Log("Time scale: 0%");
		
		Time.timeScale = 0f;
		disconnected = true;
	}
	
	// On application quit we close log files
	void OnApplicationQuit() {
		LogManager.CloseAll();
	}
	
	// Update graphics settings
	public void UpdateGraphicsSettings() {
		// For editor testing
		if(InGameLobby.instance == null) {
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 0;
			return;
		}
		
		// VSync
		QualitySettings.vSyncCount = PlayerPrefs.GetInt("Graphics_VSync", 0);
		
		var cam = Camera.main;
		var settings = InGameLobby.instance.settingsGUI;
		
		foreach(var effect in settings.effects) {
			var comp = cam.GetComponent(effect.componentName);
			
			// Don't use effect.activated because that is only set later
			if(comp != null)
				((UnityEngine.MonoBehaviour)comp).enabled = PlayerPrefs.GetInt("Graphics_" + effect.prefsId) != 0;
		}
	}
	
#region RPCs
	[RPC]
	void ReceiveServerType(ServerType type) {
		LogManager.General.Log("Received server type: " + type);
		GameManager.serverType = type;
		MapManager.InitPhysics(type);
		
		if(type == ServerType.FFA) {
			GameServerParty.CreateParties(10, 1);
		} else if(type == ServerType.Arena) {
			GameServerParty.CreateParties(2);
		} else {
			GameServerParty.CreateParties(1);
		}
	}
	
	[RPC]
	void LoadMap(string mapName) {
		// Music manager
		var audioGameObject = GameObject.Find("Audio");
		if(audioGameObject != null) {
			var musicManager = audioGameObject.GetComponent<MusicManager>();
			
			// Play ingame music
			audioGameObject.transform.parent = Camera.main.transform;
			audioGameObject.transform.localPosition = Cache.vector3Zero;
			musicManager.PlayCategory(mapName + (GameManager.isPvP ? ":Combat" : ""));
		}
		
		StartCoroutine(MapManager.LoadMapAsync(mapName));
	}
#endregion
}
