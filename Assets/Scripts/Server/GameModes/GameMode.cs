using UnityEngine;
using System.Collections;

public abstract class GameMode : MonoBehaviour {
	public static double secondsPlayed;
	public static float startGameDelay = 5.0f;
	
	public double maxWaitTimeUntilShutdown = 20.0d;
	
	[HideInInspector]
	public bool gameStarted = false;
	
	[HideInInspector]
	public bool gameEnded = false;
	
	[HideInInspector]
	public GameServerParty winnerParty = null;
	
	[HideInInspector]
	public int scoreNeededToWin = 5000;
	
	protected bool statsSentToDB = false;
	protected int highestScore;
	protected double lastActivity;
	protected ServerInit server;
	
	protected virtual void Start() {
		lastActivity = uLink.Network.time;
		server = GameObject.Find("Server").GetComponent<ServerInit>();
	}
	
	// Check if we can shut down the server
	protected void CheckShutdown() {
		// Players diconnected?
		if(uLink.Network.connections.Length == 0) {
			if(!server.isTestServer && uLink.Network.time - lastActivity > maxWaitTimeUntilShutdown) {
				LogManager.General.Log(string.Format("Exceeded maximum waiting time: {0} / {1}", uLink.Network.time - lastActivity, maxWaitTimeUntilShutdown));
				ShutdownServer();
			}
		} else {
			lastActivity = uLink.Network.time;
		}
	}
	
	// Send game start
	public void SendGameStart() {
		foreach(var pty in GameServerParty.partyList) {
			foreach(Entity player in pty.members) {
				player.networkView.RPC("StartGame", uLink.RPCMode.All);
			}
		}
		
		gameStarted = true;
	}
	
	// TODO: Send game start countdown
	public void SendGameStartCountdown() {
		foreach(var pty in GameServerParty.partyList) {
			foreach(Entity player in pty.members) {
				player.networkView.RPC("Chat", uLink.RPCMode.Owner, string.Format("Match will start in {0} seconds...", GameMode.startGameDelay));
			}
		}
		
		Invoke("SendGameStart", GameMode.startGameDelay);
	}
	
	protected void ShutdownServer() {
		LogManager.General.Log("Shutting down server");
		LogManager.CloseAll();
		
		// TODO: Do not quit before we made sure the stats arrived in the DB
#if UNITY_WEBPLAYER
		Application.Quit();
#else
		System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
	}
	
	// Send stats for all players in all parties
	protected void SendAllPlayerStats() {
		Entity topScorerAllTeams = GameServerParty.GetTopScorerAllTeams();
		
		foreach(var pty in GameServerParty.partyList) {
			var topScorerOwnTeam = pty.topScorer;
			
			foreach(Player player in pty.members) {
				PlayerQueueStats queueStats = player.stats.total;
				
				queueStats.secondsPlayed = secondsPlayed;
				queueStats.wins = (player.party == winnerParty ? 1 : 0);
				queueStats.losses = (player.party != winnerParty ? 1 : 0);
				
				// Calculate top scorer when not in 1v1
				if(QueueSettings.queueIndex != 0) {
					queueStats.topScorerOwnTeam = (player == topScorerOwnTeam ? 1 : 0);
					queueStats.topScorerAllTeams = (player == topScorerAllTeams ? 1 : 0);
				}
				
				if(player.accountId != "") {
					// Stats
					StartCoroutine(ServerGameDB.SendAccountStats(player));
					
					// Artifact rewards
					StartCoroutine(ServerGameDB.SendArtifactRewards(player));
				}
			}
		}
	}
	
	// Send stats for all players in all parties
	public void SendPlayerFFAStats(Player player) {
		LogManager.General.Log("Sending FFA stats for player '" + player.name + "'");
		player.stats.total.secondsPlayed = uLink.Network.time;
		
		if(player.accountId != "") {
			// Stats
			StartCoroutine(ServerGameDB.SendAccountStats(player));
		}
	}
}
