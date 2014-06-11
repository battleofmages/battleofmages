using UnityEngine;
using System.Collections;
using uGameDB;

public class ServerGameDB {
	// Get character stats
	public static IEnumerator GetCharacterStats(Player player) {
		LogManager.DB.Log("Getting character stats for account ID '" + player.accountId + "'");
		
		var bucket = new Bucket("AccountToCharacterStats");
		var request = bucket.Get(player.accountId);
		yield return request.WaitUntilDone();
		
		if(request.isSuccessful) {
			player.charStats = request.GetValue<CharacterStats>();
			LogManager.DB.Log("Retrieved character stats of account ID '" + player.accountId + "' successfully: " + player.charStats.ToString());
		} else {
			player.charStats = new CharacterStats();
			LogManager.DB.LogWarning("Account '" + player.accountId + "' doesn't have any character stats yet, creating default ones");
		}
		
		// Send other players and myself information about stats
		player.networkView.RPC("ReceiveCharacterStats", uLink.RPCMode.All, player.charStats);
	}
	
	// Send stats for a single account
	public static IEnumerator SendAccountStats(Player player) {
		string accountId = player.accountId;
		PlayerStats instanceStats = player.stats;
		
		//LogManager.DB.Log("Going to send player stats of '" + player.GetName() + "' to the database");
		LogManager.DB.Log("Retrieving stats for account " + accountId);
		
		// Which bucket?
		string bucketName;
		switch(GameManager.serverType) {
			case ServerType.Arena:
				bucketName = "AccountToStats";
				break;
				
			case ServerType.FFA:
				bucketName = "AccountToFFAStats";
				break;
				
			default:
				LogManager.DB.LogError("Unknown server type " + GameManager.serverType + ", can't select a bucket to save the stats");
				yield break;
		}
		
		// Retrieve old stats
		LogManager.DB.Log("Using bucket '" + bucketName + "'");
		var bucket = new Bucket(bucketName);
		var getRequest = bucket.Get(accountId);
		yield return getRequest.WaitUntilDone();
		
		PlayerStats statsInDB;
		
		if(getRequest.isSuccessful) {
			statsInDB = getRequest.GetValue<PlayerStats>();
			
			LogManager.DB.Log("Queried stats of account '" + accountId + "' successfully (Ranking: " + statsInDB.ranking + ")");
		} else {
			statsInDB = new PlayerStats();
			
			LogManager.DB.Log("Account " + accountId + " doesn't have any player stats yet");
		}
		
		// Calculate new stats
		LogManager.DB.Log("Merging account stats of '" + accountId + "'...");
		
		// Merge database stats with the stats in the instance
		statsInDB.MergeWithMatch(instanceStats);
		
		if(GameManager.isArena) {
			LogManager.DB.Log("Sending new best ranking to '" + accountId + "'...");
			player.newBestRanking = statsInDB.bestRanking;
			player.networkView.RPC("ReceiveNewBestRanking", uLink.RPCMode.Others, player.newBestRanking);
		}
		
		/*if(ServerInit.instance.isArena) {
		
		} else if(ServerInit.instance.isFFA) {
			LogManager.DB.Log("Merging account stats of '" + accountId + "' for FFA...");
			
			statsInDB.MergeWithFFA(stats);
		}*/
		
		// Write new stats
		LogManager.DB.Log("Writing account stats of '" + accountId + "'...");
		var setRequest = bucket.Set(accountId, statsInDB, Encoding.Json);
		yield return setRequest.WaitUntilDone();
		
		if(setRequest.isSuccessful) {
			LogManager.DB.Log("Wrote account stats of '" + accountId + "' successfully (Ranking: " + statsInDB.ranking + ", Level: " + statsInDB.level + ")");
		} else {
			LogManager.DB.LogError("Could not write account stats for '" + accountId + "'");
		}
	}
	
	// SendArtifactRewards
	public static IEnumerator SendArtifactRewards(Player player) {
		string accountId = player.accountId;
		
		// Retrieve inventory
		var bucket = new Bucket("AccountToArtifactInventory");
		var getRequest = bucket.Get(accountId);
		yield return getRequest.WaitUntilDone();
		
		ArtifactInventory artifactInventory;
		
		if(getRequest.isSuccessful) {
			artifactInventory = getRequest.GetValue<ArtifactInventory>();
			
			LogManager.DB.Log("Queried artifact inventory of account '" + accountId + "' successfully");
		} else {
			artifactInventory = new ArtifactInventory();
			
			LogManager.DB.Log("Account " + accountId + " doesn't have any artifact inventory yet");
		}
		
		// TODO: Skill dependant
		var arti = new Artifact((byte)Random.Range(0, 3));
		artifactInventory.AddArtifact(arti);
		
		// Let the player know about his reward
		player.networkView.RPC("ReceiveArtifactReward", uLink.RPCMode.Owner, arti.id);
		
		// Write new stats
		var setRequest = bucket.Set(accountId, artifactInventory, Encoding.Json);
		yield return setRequest.WaitUntilDone();
		
		if(setRequest.isSuccessful) {
			LogManager.DB.Log("Wrote artifact inventory of '" + accountId + "' successfully");
		} else {
			LogManager.DB.LogError("Could not write artifact inventory for '" + accountId + "'");
		}
	}
}
