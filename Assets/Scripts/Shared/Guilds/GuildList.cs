using System.Collections.Generic;

[System.Serializable]
public class GuildList : JsonSerializable<GuildList> {
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
}
