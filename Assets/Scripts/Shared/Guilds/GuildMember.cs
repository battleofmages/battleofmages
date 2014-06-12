using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GuildMember {
	public string accountId;
	public byte rank;
	public TimeStamp joinDate;
	
	[System.NonSerialized]
	public string name;
	
	public enum Rank {
		Leader,
		Default
	}
	
	// Empty constructor
	public GuildMember() {
		accountId = "";
		name = "";
		rank = 0;
		joinDate = new TimeStamp();
	}
	
	// Constructor
	public GuildMember(string nAccountId, byte nRank) {
		accountId = nAccountId;
		name = "";
		rank = nRank;
		joinDate = new TimeStamp();
	}
	
	// Constructor
	public GuildMember(string nAccountId, string nName, byte nRank) {
		accountId = nAccountId;
		name = nName;
		rank = nRank;
		joinDate = new TimeStamp();
	}
	
	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		GenericSerializer.WriteJSONClassInstance<GuildMember>(writer, (GuildMember)instance, null, new HashSet<string>(){
			"name"
		});
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return GenericSerializer.ReadJSONClassInstance<GuildMember>(reader);
	}
	
	// BitStream Writer
	public static void WriteToBitStream(uLink.BitStream stream, object val, params object[] args) {
		GuildMember obj = (GuildMember)val;
		
		stream.WriteString(obj.accountId);
		stream.WriteString(obj.name);
		stream.WriteByte(obj.rank);
		
		// TODO: Transfer joinDate
	}
	
	// BitStream Reader
	public static object ReadFromBitStream(uLink.BitStream stream, params object[] args) {
		GuildMember obj = new GuildMember();
		
		obj.accountId = stream.ReadString();
		obj.name = stream.ReadString();
		obj.rank = stream.ReadByte();
		
		return obj;
	}
}
