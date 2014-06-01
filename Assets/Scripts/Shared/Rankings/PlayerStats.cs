using System;
using UnityEngine;

[System.Serializable]
public class PlayerStats {
	// Stats saved in the database
	public double level = 1;
	public int bestRanking;
	public int ping;
	
	// Total stats
	public PlayerQueueStats total;
	
	// Saved for each queue
	public PlayerQueueStats[] queue;
	
	// Constructor
	public PlayerStats() {
		total = new PlayerQueueStats();
		
		queue = new PlayerQueueStats[QueueSettings.queueCount];
		
		for(int i = 0; i < QueueSettings.queueCount; i++) {
			queue[i] = new PlayerQueueStats();
		}
	}
	
	// Calculates new stats
	public void MergeWithMatch(PlayerStats matchStats) {
		// Total stats
		MergeQueueStats(total, matchStats.total);
		
		// Current queue stats
		if(QueueSettings.queueIndex >= 0 && QueueSettings.queueIndex < queue.Length) {
			MergeQueueStats(queue[QueueSettings.queueIndex], matchStats.total);
		}
		
		level = CalculateLevel();
		bestRanking = ChooseBestRanking();
		ping = matchStats.ping;
	}
	
	// Merges 2 queue stats
	void MergeQueueStats(PlayerQueueStats db, PlayerQueueStats matchStats) {
		db.kills += matchStats.kills;
		db.deaths += matchStats.deaths;
		db.assists += matchStats.assists;
		
		db.wins += matchStats.wins;
		db.losses += matchStats.losses;
		db.leaves += matchStats.leaves;
		
		db.hits += matchStats.hits;
		db.hitsTaken += matchStats.hitsTaken;
		
		db.blocks += matchStats.blocks;
		db.blocksTaken += matchStats.blocksTaken;
		
		db.lifeDrain += matchStats.lifeDrain;
		db.lifeDrainTaken += matchStats.lifeDrainTaken;
		
		db.heal += matchStats.heal;
		db.healTaken += matchStats.healTaken;
		
		db.damage += matchStats.damage;
		db.damageTaken += matchStats.damageTaken;
		
		db.cc += matchStats.cc;
		db.ccTaken += matchStats.ccTaken;
		
		db.runeDetonations += matchStats.runeDetonations;
		db.runeDetonationsTaken += matchStats.runeDetonationsTaken;
		
		db.runeDetonationsLevel += matchStats.runeDetonationsLevel;
		db.runeDetonationsLevelTaken += matchStats.runeDetonationsLevelTaken;
		
		db.topScorerOwnTeam += matchStats.topScorerOwnTeam;
		db.topScorerAllTeams += matchStats.topScorerAllTeams;
		
		db.secondsPlayed += matchStats.secondsPlayed;
		
		db.CalculateRanking();
	}
	
	// CalculateLevel
	double CalculateLevel() {
		double winValue = 0d;
		double loseValue = 0d;
		double killValue = 0d;
		
		if(total.wins > 0)
			winValue = Math.Log(1d + (double)(total.wins));
		
		if(total.losses > 0)
			loseValue = Math.Log(1d + (double)(total.losses) / 2d);
		
		if(total.kills > 0)
			killValue = Math.Log(1d + (double)(total.kills) / 5d);
		
		// Just to be safe
		if(winValue == Mathf.Infinity)
			winValue = 0d;
		
		if(loseValue == Mathf.Infinity)
			loseValue = 0d;
		
		if(killValue == Mathf.Infinity)
			killValue = 0d;
		
		return 1.0d + winValue + loseValue + killValue;
	}
	
	// ChooseBestRanking
	int ChooseBestRanking() {
		int tmpBestRanking = 0;
		
		for(int i = 0; i < QueueSettings.queueCount; i++) {
			int queueRanking = queue[i].ranking;
			
			if(queueRanking > tmpBestRanking) {
				tmpBestRanking = queueRanking;
			}
		}
		
		if(total.ranking > tmpBestRanking) {
			tmpBestRanking = total.ranking;
		}
		
		return tmpBestRanking;
	}
	
	// uLink Bitstream write
	/*public static void BitStreamSerializer(
		uLink.BitStream stream,
		object val,
		params object[] codecOptions
	) {
		PlayerStats stats = (PlayerStats)val;
		stream.Write<double>(stats.level);
		stream.Write<int>(stats.bestRanking);
		stream.Write<int>(stats.ping);
	}*/
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		GenericSerializer.WriteJSONClassInstance<PlayerStats>(writer, (PlayerStats)instance);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<PlayerStats>(reader);
	}
	
	// Calculated stats
	public int ranking {get{
		return bestRanking;
	}}
}
