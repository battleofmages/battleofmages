using UnityEngine;
using System.Collections.Generic;

public static class GuildsDB {
	// --------------------------------------------------------------------------------
	// Guilds
	// --------------------------------------------------------------------------------
	
	// Put guild
	public static Coroutine PutGuild(Guild guild, GameDB.PutActionOnResult<Guild> func) {
		return GameDB.instance.StartCoroutine(GameDB.Put<Guild>(
			"Guilds",
			guild,
			func
		));
	}
	
	// Set guild
	public static Coroutine SetGuild(string guildId, Guild guild) {
		return GameDB.instance.StartCoroutine(GameDB.Set<Guild>(
			"Guilds",
			guildId,
			guild,
			data => {
				// ...
			}
		));
	}
	
	// Get guild
	public static Coroutine GetGuild(string guildId) {
		return GameDB.instance.StartCoroutine(GameDB.Get<Guild>(
			"Guilds",
			guildId,
			data => {
				if(data == null) {
					// ...
				} else {
					GameDB.guildIdToGuild[guildId] = data;
				}
			}
		));
	}
	
	// Get guild
	public static Coroutine GetGuild(string guildId, GameDB.ActionOnResult<Guild> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<Guild>(
			"Guilds",
			guildId,
			func
		));
	}
	
	// Remove guild members
	public static Coroutine RemoveGuild(string guildId) {
		return GameDB.instance.StartCoroutine(GameDB.Remove(
			"Guilds",
			guildId,
			success => {
				if(success) {
					GameDB.guildIdToGuild.Remove(guildId);
				}
			}
		));
	}
	
	// --------------------------------------------------------------------------------
	// AccountToGuilds
	// --------------------------------------------------------------------------------
	
	// Get guild list
	public static Coroutine GetGuildList(string accountId, GameDB.ActionOnResult<GuildList> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<GuildList>(
			"AccountToGuilds",
			accountId,
			func
		));
	}
	
	// Set guild list
	public static Coroutine SetGuildList(string accountId, GuildList guildIdList, GameDB.ActionOnResult<GuildList> func = null) {
		return GameDB.instance.StartCoroutine(GameDB.Set<GuildList>(
			"AccountToGuilds",
			accountId,
			guildIdList,
			func
		));
	}
	
	// --------------------------------------------------------------------------------
	// GuildToMembers
	// --------------------------------------------------------------------------------
	
	// Set guild members
	public static Coroutine SetGuildMembers(string guildId, List<GuildMember> members) {
		return GameDB.instance.StartCoroutine(GameDB.Set<List<GuildMember>>(
			"GuildToMembers",
			guildId,
			members,
			data => {
				// ...
			}
		));
	}
	
	// Get guild members
	public static Coroutine GetGuildMembers(string guildId) {
		return GameDB.instance.StartCoroutine(GameDB.Get<List<GuildMember>>(
			"GuildToMembers",
			guildId,
			data => {
				if(data == null) {
					// ...
				} else {
					GameDB.guildIdToGuildMembers[guildId] = data;
				}
			}
		));
	}
	
	// Remove guild members
	public static Coroutine RemoveGuildMembers(string guildId) {
		return GameDB.instance.StartCoroutine(GameDB.Remove(
			"GuildToMembers",
			guildId,
			success => {
				if(success) {
					GameDB.guildIdToGuildMembers.Remove(guildId);
				}
			}
		));
	}
	
	// --------------------------------------------------------------------------------
	// AccountToGuildInvitations
	// --------------------------------------------------------------------------------
	
	// Set guild invitations
	public static Coroutine SetGuildInvitations(string accountId, List<string> gInvitations, GameDB.ActionOnResult<List<string>> func) {
		return GameDB.instance.StartCoroutine(GameDB.Set<List<string>>(
			"AccountToGuildInvitations",
			accountId,
			gInvitations,
			func
		));
	}
	
	// Get guild invitations
	public static Coroutine GetGuildInvitations(string accountId, GameDB.ActionOnResult<List<string>> func) {
		return GameDB.instance.StartCoroutine(GameDB.Get<List<string>>(
			"AccountToGuildInvitations",
			accountId,
			func
		));
	}
	
	// --------------------------------------------------------------------------------
	// MapReduce
	// --------------------------------------------------------------------------------
	
	// Get guild ID by guild name
	public static Coroutine GetGuildIdByGuildName(string guildName, GameDB.ActionOnResult<string> func) {
		return GameDB.instance.StartCoroutine(GameDB.MapReduce<KeyValue<string>>(
			"Guilds",
			GameDB.GetSearchMapFunction("name"),
			GameDB.GetSearchReduceFunction(),
			guildName,
			data => {
				if(data != null && data.Length == 1) {
					func(data[0].key);
				} else {
					func(default(string));
				}
			}
		));
	}
}
