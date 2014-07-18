using uLobby;
using UnityEngine;

public class ArenaGUI : LobbyModule<ArenaGUI> {
	private static int queueButtonWidth = 150;
	private static int queueButtonHeight = 46;
	
	public GUIStyle gameTypeStyle;
	
	// Queue info
	private int[] queuePlayers = new int[5];
	private byte currentQueue = 0;
	private bool matchFound;
	private byte matchFoundQueue = 0;
	
	// In queue
	public bool inQueue {
		get {
			return currentQueue != 0;
		}
	}
	
	// Start
	void Start() {
		// Receive lobby RPCs
		Lobby.AddListener(this);
	}
	
	// OnGUI
	void OnGUI() {
		if(Player.main == null)
			return;
		
		if(inQueue) {
			GUI.depth = 10;
			
			using(new GUIArea(Screen.width * 0.75f, Screen.height * 1.0f - queueButtonHeight - 4, queueButtonWidth, Screen.height * 0.1f)) {
				DrawMatchmakingButton(currentQueue);
			}
		}
	}
	
	// Draw
	public override void Draw() {
		if(currentQueue != 0)
			return;

		using(new GUIVertical()) { //GUILayout.MinWidth(200)
			DrawMatchmakingQueues();
		}
	}
	
	// Matchmaking queue buttons
	public void DrawMatchmakingQueues() {
		using(new GUIHorizontalCenter()) {
			using(new GUIVerticalCenter()) { //GUILayout.Width(queueButtonWidth)
				using(new GUIHorizontal()) {
					using(new GUIVertical()) {
						GUILayout.Label("Arena", gameTypeStyle);
						
						for(byte i = 1; i <= QueueSettings.queueCount; i++) {
							DrawMatchmakingButton(i);
							
							//if(i != QueueSettings.queueCount)
							//	GUILayout.FlexibleSpace();
						}
					}
					
					using(new GUIVertical()) {
						GUILayout.Label("FFA", gameTypeStyle);
						
						for(byte i = 1; i <= QueueSettings.queueCount; i++) {
							GUI.enabled = (i == 1);
							if(GUIHelper.Button(_("<b><size=16>Play {0}v{0}v{0}v...</size></b>", i), GUILayout.Width(queueButtonWidth), GUILayout.Height(queueButtonHeight))) {
								Sounds.instance.PlayButtonClick();
								
								Lobby.RPC("JoinFFARequest", Lobby.lobby, i);
								matchFound = true;
							}
						}
						
						GUI.enabled = true;
					}
					
					GUI.enabled = false;
					
					// Conquest
					using(new GUIVertical()) {
						GUILayout.Label("Conquest", gameTypeStyle);
						GUILayout.Space(queueButtonHeight * 2 + 8);
						
						for(byte i = 3; i <= QueueSettings.queueCount; i++) {
							if(GUIHelper.Button(_("<b><size=16>Play {0}v{0}</size></b>", i), GUILayout.Width(queueButtonWidth), GUILayout.Height(queueButtonHeight))) {
								Sounds.instance.PlayButtonClick();
								// ...
							}
						}
					}
					
					// Real Time Strategy
					using(new GUIVertical()) {
						GUILayout.Label("RTS", gameTypeStyle);
						
						if(GUIHelper.Button("<b><size=16>Play as soldier</size></b>", GUILayout.Width(queueButtonWidth), GUILayout.Height(queueButtonHeight))) {
							Sounds.instance.PlayButtonClick();
							// ...
						}
						
						if(GUIHelper.Button("<b><size=16>Play as strategist</size></b>", GUILayout.Width(queueButtonWidth), GUILayout.Height(queueButtonHeight))) {
							Sounds.instance.PlayButtonClick();
							// ...
						}
					}
					
					// Capture The Flag
					using(new GUIVertical()) {
						GUILayout.Label("CTF", gameTypeStyle);
						GUILayout.Space(queueButtonHeight * 2 + 8);
						
						for(byte i = 3; i <= QueueSettings.queueCount; i++) {
							if(GUIHelper.Button(_("<b><size=16>Play {0}v{0}</size></b>", i), GUILayout.Width(queueButtonWidth), GUILayout.Height(queueButtonHeight))) {
								Sounds.instance.PlayButtonClick();
								// ...
							}
						}
					}
					
					// Aether Ball
					using(new GUIVertical()) {
						GUILayout.Label("Aether Ball", gameTypeStyle);
						GUILayout.Space(queueButtonHeight * 2 + 8);
						
						for(byte i = 3; i <= QueueSettings.queueCount; i++) {
							if(GUIHelper.Button(_("<b><size=16>Play {0}v{0}</size></b>", i), GUILayout.Width(queueButtonWidth), GUILayout.Height(queueButtonHeight))) {
								Sounds.instance.PlayButtonClick();
								// ...
							}
						}
					}
					
					GUI.enabled = true;
				}
			}
		}
	}
	
	// Matchmaking button
	void DrawMatchmakingButton(byte i) {
		string buttonCaption;
		string playerCount = "<size=11>" + GUIHelper.Plural(queuePlayers[i - 1], "player") + "</size>";
		
		string prefix = "<b><size=16>";
		string postfix = "</size></b>";
		
		if(i == currentQueue) {
			if(matchFound)
				GUI.backgroundColor = Color.green;
			else
				GUI.backgroundColor = Color.yellow;
			buttonCaption = prefix + "In " + i + "v" + i + " queue..." + postfix + "\n" + playerCount;
		} else {
			GUI.backgroundColor = Color.white;
			buttonCaption = prefix + "Play " + i + "v" + i + postfix + "\n" + playerCount;
		}
		
		if(GUIHelper.Button(buttonCaption, GUILayout.Width(queueButtonWidth), GUILayout.Height(queueButtonHeight))) {
			Sounds.instance.PlayButtonClick();
			
			if(i == currentQueue)
				LeaveQueue();
			else
				EnterQueue(i);
		}
		
		// Reset color
		GUI.backgroundColor = Color.white;
	}
	
	// Request to enter a queue
	void EnterQueue(byte playersPerTeam) {
		if(matchFound)
			return;
		
		Lobby.RPC("EnterQueue", Lobby.lobby, playersPerTeam);
	}
	
	// Request to leave a queue
	void LeaveQueue() {
		if(matchFound)
			return;
		
		Lobby.RPC("LeaveQueue", Lobby.lobby);
	}
	
	// Resets the queue info
	public void ResetQueueInfo() {
		matchFound = false;
		currentQueue = 0;
		
		for(int i = 0; i < queuePlayers.Length; i++) {
			queuePlayers[i] = 0;
		}
		
		LogManager.General.Log("Reset queue info");
	}
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	[RPC]
	void EnteredQueue(byte playersPerTeam) {
		if(currentQueue == playersPerTeam)
			return;
		
		// Decrease player count in the previous queue I was in
		if(currentQueue > 0)
			queuePlayers[currentQueue - 1] -= 1;
		
		// Log
		LogManager.General.Log("Entered the queue: " + playersPerTeam);
		
		// Set new queue
		currentQueue = playersPerTeam;
		
		// Increase player count for the queue I joined
		queuePlayers[currentQueue - 1] += 1;
	}
	
	[RPC]
	void LeftQueue() {
		if(currentQueue == 0)
			return;
		
		// Log
		LogManager.General.Log("Left the queue: " + currentQueue);
		
		queuePlayers[currentQueue - 1] -= 1;
		currentQueue = 0;
	}
	
	[RPC]
	void QueueStats(int nTotalOnlinePlayers, int q1v1, int q2v2, int q3v3, int q4v4, int q5v5) {
		InGameLobby.instance.totalOnlinePlayers = nTotalOnlinePlayers;
		queuePlayers[0] = q1v1;
		queuePlayers[1] = q2v2;
		queuePlayers[2] = q3v3;
		queuePlayers[3] = q4v4;
		queuePlayers[4] = q5v5;
	}
	
	[RPC]
	void MatchFound() {
		LogManager.General.Log("Match has been created");
		
		Sounds.instance.PlayQueueMatchFound();
		matchFound = true;
		matchFoundQueue = currentQueue;
		
		new TimedConfirm(
			"An arena match has been created, would you like to enter?",
			Config.instance.matchAcceptTime,
			() => {
				Lobby.RPC("AcceptMatch", Lobby.lobby, true);
			},
			() => {
				this.matchFoundQueue = 0;
				Lobby.RPC("AcceptMatch", Lobby.lobby, false);
			}
		);
	}
	
	[RPC]
	void MatchCanceled() {
		LogManager.General.Log("Match has been canceled");
		
		Sounds.instance.PlayQueueMatchCanceled();
		matchFound = false;
		currentQueue = 0;
		
		// Close popup window
		if(Login.instance.popupWindow != null && Login.instance.popupWindow is TimedConfirm)
			Login.instance.popupWindow.Close();
		
		if(matchFoundQueue != 0) {
			LobbyChat.instance.AddEntry("A player did not enter the match, re-entering the matchmaking queue.");
			EnterQueue(matchFoundQueue);
		} else {
			LobbyChat.instance.AddEntry("A player did not enter the match, matchmaking has been canceled.");
		}
		
		matchFoundQueue = 0;
	}
#endregion
}
