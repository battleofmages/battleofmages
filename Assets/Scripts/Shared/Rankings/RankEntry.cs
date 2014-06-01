using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jboy;

enum RankingPage {
	Best,
	Global,
	V1,
	V2,
	V3,
	V4,
	V5,
}

enum RankingSubject {
	Player,
	Team,
	Guild,
}

[System.Serializable]
public class RankEntry {
	public int rankIndex;
	public string accountId;
	public string name;
	public string country;
	public int bestRanking;
	public long totalDamage;
	
	// BitStream Writer
	public static void WriteToBitStream(uLink.BitStream stream, object val, params object[] args) {
		RankEntry myObj = (RankEntry)val;
		
		//stream.WriteInt32(myObj.rankIndex);
		//stream.WriteString(myObj.accountId);
		stream.WriteString(myObj.name);
		stream.WriteString(myObj.country);
		stream.WriteInt32(myObj.bestRanking);
		//stream.WriteInt64(myObj.totalDamage);
	}
	
	// BitStream Reader
	public static object ReadFromBitStream(uLink.BitStream stream, params object[] args) {
		RankEntry myObj = new RankEntry();
		
		//myObj.rankIndex = stream.ReadInt32();
		//myObj.accountId = stream.ReadString();
		myObj.name = stream.ReadString();
		myObj.country = stream.ReadString();
		myObj.bestRanking = stream.ReadInt32();
		//myObj.totalDamage = stream.ReadInt64();
		
		return myObj;
	}
	
	// Writer
	public static void JsonSerializer(JsonWriter writer, object instance) {
		var scoreEntry = (RankEntry)instance;
		
		writer.WriteArrayStart();
		writer.WriteNumber(scoreEntry.rankIndex);
		writer.WriteString(scoreEntry.accountId);
		writer.WriteString(scoreEntry.name);
		writer.WriteString(scoreEntry.country);
		writer.WriteNumber(scoreEntry.bestRanking);
		writer.WriteNumber(scoreEntry.totalDamage);
		writer.WriteArrayEnd();
	}
	
	// Reader
	public static object JsonDeserializer(JsonReader reader) {
		var scoreEntry = new RankEntry();
		
		reader.ReadArrayStart();
		scoreEntry.rankIndex = (int)reader.ReadNumber();
		scoreEntry.accountId = reader.ReadString();
		scoreEntry.name = reader.ReadString();
		scoreEntry.country = reader.ReadString();
		scoreEntry.bestRanking = (int)reader.ReadNumber();
		scoreEntry.totalDamage = (long)reader.ReadNumber();
		reader.ReadArrayEnd();
		
		return scoreEntry;
	}
}
