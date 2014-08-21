using UnityEngine;
using uLobby;

// State
public enum State {
	ConnectingToLobby,
	Disconnected,
	Update,
	LogIn,
	Register,
	License,
	Lobby,
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

	// Game state
	private static State _currentState = State.ConnectingToLobby;
	private static State _nextState = State.ConnectingToLobby;
	
	// Current state
	public static State currentState {
		get {
			return _currentState;
		}
		
		set {
			_nextState = value;
		}
	}
	
	// Next state
	public static State nextState {
		get {
			return _nextState;
		}
	}
	
	// In lobby
	public static bool inLobby {
		get {
			return _currentState == State.Lobby;
		}
	}
	
	// In login
	public static bool inLogIn {
		get {
			return _currentState == State.LogIn;
		}
	}
	
	// In game
	public static bool inGame {
		get {
			return _currentState == State.Game;
		}
	}

	// Start
	void Start() {
		if(isClient)
			Lobby.AddListener(this);
	}
	
	// Update to next state
	void Update() {
		_currentState = _nextState;
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
