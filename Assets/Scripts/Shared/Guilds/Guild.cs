using UnityEngine;

[System.Serializable]
public class Guild : JsonSerializable<Guild> {
	public string name;
	public string tag;
	public string introduction;
	public string messageOfTheDay;
	public TimeStamp creationDate;
	public string founderAccountId;
	public Texture2D icon;

	// Constructor
	public Guild() {
		name = "";
		tag = "";
		introduction = "";
		messageOfTheDay = "";
		creationDate = new TimeStamp();
		icon = null; //new Texture2D(64, 64);
	}

	// Constructor
	public Guild(string guildName, string guildTag, string nFounderAccountId) {
		name = guildName;
		tag = guildTag;
		introduction = "";
		messageOfTheDay = "";
		creationDate = new TimeStamp();
		founderAccountId = nFounderAccountId;
		icon = null;
	}

	// ToString
	public override string ToString() {
		return name + " [" + tag + "]";
	}
	
	// Can the account invite persons?
	public static bool CanInvite(string guildId, string accountId) {
		try {
			return GameDB.guildIdToGuildMembers[guildId].Find(o => o.accountId == accountId).rank == (byte)GuildMember.Rank.Leader;
		} catch {
			return false;
		}
	}
	
	// Can the account kick persons?
	public static bool CanKick(string guildId, string accountId) {
		try {
			return GameDB.guildIdToGuildMembers[guildId].Find(o => o.accountId == accountId).rank == (byte)GuildMember.Rank.Leader;
		} catch {
			return false;
		}
	}
	
	// Can the account disband the guild?
	public static bool CanDisband(string guildId, string accountId) {
		try {
			return GameDB.guildIdToGuildMembers[guildId].Find(o => o.accountId == accountId).rank == (byte)GuildMember.Rank.Leader;
		} catch {
			return false;
		}
	}
}
