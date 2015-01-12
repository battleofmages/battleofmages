using UnityEngine;
using uLobby;

public class LobbyConnector : SingletonMonoBehaviour<LobbyConnector>, Initializable {
	public string lobbyHost;
	public int lobbyPort;

	// Init
	public void Init() {
		// Public key
		NetworkHelper.InitPublicLobbyKey();
		
		// Receive lobby events
		Lobby.AddListener(this);

		// Connect
		Connect();
	}

	// Connect: Also used for reconnecting
	void Connect() {
		if(Lobby.connectionStatus == LobbyConnectionStatus.Connected)
			return;

		// Activate UI
		UIManager.instance.currentState = "Connect";

		// Connect
		LogManager.General.Log("Connecting to lobby @ " + lobbyHost + ":" + lobbyPort);
		Lobby.ConnectAsClient(lobbyHost, lobbyPort);
	}

	// Update
	void Update() {
		if(Lobby.connectionStatus == LobbyConnectionStatus.Disconnected) {
			LogManager.General.Log("Disconnected from lobby");
			
			// Connect
			Connect();
		}
	}

#region Callbacks
	// uLobby: Connected
	void uLobby_OnConnected() {
		LogManager.General.Log("Connected to lobby");
	}
	
	// uLobby: Disconnected
	void uLobby_OnDisconnected() {
		// Not used, see Update() instead
	}
#endregion
}
