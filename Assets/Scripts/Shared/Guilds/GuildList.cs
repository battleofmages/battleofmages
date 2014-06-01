using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GuildList {
	public string mainGuildId;
	public List<string> idList;
	
	// Constructor
	public GuildList() {
		mainGuildId = "";
		idList = new List<string>();
	}
	
	// Add
	public void Add(string guildId) {
		idList.Add(guildId);
	}
	
	// Remove
	public void Remove(string guildId) {
		if(guildId == mainGuildId) {
			mainGuildId = "";
		}
		
		idList.Remove(guildId);
	}
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		GenericSerializer.WriteJSONClassInstance<GuildList>(writer, (GuildList)instance);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<GuildList>(reader);
	}
}
