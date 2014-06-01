// This script is only executed on the server
public class ArenaGameMode : GameMode {
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
			// All players disconnected?
			CheckShutdown();
			
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
}
