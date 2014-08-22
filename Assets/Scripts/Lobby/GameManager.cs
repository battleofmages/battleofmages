using UnityEngine;
using uLobby;

// State
public enum State {
	StartUp,
	ConnectingToLobby,
	PleaseUpdateClient,
	Disconnected,
	LogIn,
	Register,
	License,
	WaitingForAccountInfo,
	Game,
}

// GameManager
public class GameManager : SingletonMonoBehaviour<GameManager> {
	// Server type
	public static ServerType serverType;
	
	public static bool isArena { get { return serverType == ServerType.Arena; } }
	public static bool isFFA { get { return serverType == ServerType.FFA; } }
	public static bool isTown { get { return serverType == ServerType.Town; } }
	public static bool isWorld { get { return serverType == ServerType.World; } }
	public static bool isRankedGame { get { return serverType == ServerType.Arena; } }

	public static bool isPvE { get { return isTown || isWorld; } }
	public static bool isPvP { get { return !isPvE; } }

	// Game status
	public static bool gameStarted {get; set;}
	public static bool gameEnded {get; set;}

	// Client or server
	public static bool isServer { get { return ServerInit.instance != null; } }
	public static bool isClient { get { return !isServer; } }
	
	// Current state
	public static State currentState {
		get;
		set;
	}
	
	// In login
	public static bool inLogIn {
		get {
			return currentState == State.LogIn;
		}
	}
	
	// In game
	public static bool inGame {
		get {
			return currentState == State.Game;
		}
	}

	// Start
	void Start() {
		if(isClient)
			Lobby.AddListener(this);
	}

	// Quit
	public void Quit() {
		LogManager.General.Log("Quitting game...");
		Application.Quit();
	}

#region RPCs (Lobby)
	[RPC]
	public void ReceiveServerType(ServerType type) {
		LogManager.General.Log("Received server type: " + type);
		GameManager.serverType = type;
		GameManager.gameEnded = false;
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
	public void LoadMap(string mapName) {
		// Music manager
		var audioGameObject = GameObject.Find("Audio");
		if(audioGameObject != null) {
			// Correct position of audio object
			audioGameObject.transform.parent = Camera.main.transform;
			audioGameObject.transform.localPosition = Cache.vector3Zero;
		}
		
		StartCoroutine(MapManager.LoadMapAsync(mapName));
	}
#endregion
}
