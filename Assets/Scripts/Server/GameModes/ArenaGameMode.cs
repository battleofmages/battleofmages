// This script is only executed on the server
public class ArenaGameMode : GameMode {
	// OnEnable
	protected override void OnEnable() {
		base.OnEnable();
		
		InvokeRepeating("GameModeUpdate", 1f, 1f);
	}

	// Update
	void Update () {
		if(gameEnded) {
			// Send ranking stats
			if(!statsSentToDB) {
				secondsPlayed = uLink.Network.time;
				SendAllPlayerStats();
				statsSentToDB = true;
			}
			
			// Quit game server after all players disconnected
			if(uLink.Network.connections.Length == 0) {
				ShutdownServer();
			}
			
			return;
		} else {
			// Check if the game ended now
			foreach(var pty in GameServerParty.partyList) {
				// Win conditions
				if(pty.score >= scoreNeededToWin && pty.score > highestScore) {
					winnerParty = pty;
					highestScore = pty.score;
					gameEnded = true;
				}
			}
		}
	}

	// GameModeUpdate
	void GameModeUpdate() {
		// All players disconnected?
		if(!gameEnded)
			CheckShutdown();
	}
}
