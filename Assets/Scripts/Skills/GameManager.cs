using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// GameManager
public class GameManager : SingletonMonoBehaviour<GameManager> {
	// Game started
	public static bool gameStarted {
		get;
		set;
	}

	// Game ended
	public static bool gameEnded {
		get;
		set;
	}

	// In lobby
	public static bool inLobby {
		get {
			return false;
		}
	}

	// In game
	public static bool inGame {
		get {
			return false;
		}
	}

	// Quit
	public void Quit() {
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	
	// Client or server
	/*public static bool isServer { get { return Server.instance != null; } }
	public static bool isClient { get { return !isServer; } }*/
}