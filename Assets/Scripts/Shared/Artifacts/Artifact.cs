using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Artifact : Item {
	public static string[] rarityColors = {"white", "green", "blue", "yellow", "orange"};
	
	public const int maxLevel = 5;
	public const int maxStats = 3;
	
	public byte level;
	public Artifact.Stat[] stats;
	private string _name;
	
	[System.Serializable]
	public enum Stat {
		Attack,
		Defense,
		Energy,
		CooldownReduction,
		AttackSpeed,
		MoveSpeed
	}

	// Constructor
	public Artifact() {
		level = 0;
		stats = new Artifact.Stat[maxStats];
		_name = "";
	}

	// Constructor
	public Artifact(byte nLevel) {
		level = nLevel;
		stats = new Artifact.Stat[maxStats];
		_name = "";
		
		for(int i = 0; i < stats.Length; i++) {
			stats[i] = (Artifact.Stat)Random.Range(0, 6);
		}
	}

	// Constructor
	public Artifact(int itemId) {
		stats = new Artifact.Stat[maxStats];
		this.id = itemId;
		_name = "";
	}

	// HasStat
	public bool HasStat(Artifact.Stat nStat, int times = 1) {
		int count = 0;
		foreach(var stat in stats) {
			if(stat == nStat)
				count++;
			
			if(count == times)
				return true;
		}
		
		return false;
	}

	// Rarity
	public Rarity rarity {
		get {
			if(stats[0] == stats[1]) {
				if(stats[0] == stats[2])
					return Rarity.Rare;
				
				return Rarity.Uncommon;
			}
			
			if(stats[0] == stats[2] || stats[1] == stats[2])
				return Rarity.Uncommon;
			
			return Rarity.Common;
		}
	}

	// Icon
	public Texture2D icon {
		get {
#if !LOBBY_SERVER
			if(ArtifactsGUI.instance != null)
				return ArtifactsGUI.instance.rarityIcons[(int)this.rarity];
#endif
			return null;
		}
	}
	
	// Create character stats from an artifact
	public CharacterStats charStats {
		get {
			CharacterStats nCharStats = new CharacterStats(0);
			
			foreach(var stat in stats) {
				switch(stat) {
				case Stat.Attack:
					nCharStats.attack += level + 1;
					break;
				case Stat.Defense:
					nCharStats.defense += level + 1;
					break;
				case Stat.Energy:
					nCharStats.block += level + 1;
					break;
				case Stat.CooldownReduction:
					nCharStats.cooldownReduction += level + 1;
					break;
				case Stat.AttackSpeed:
					nCharStats.attackSpeed += level + 1;
					break;
				case Stat.MoveSpeed:
					nCharStats.moveSpeed += level + 1;
					break;
				}
			}
			
			return nCharStats;
		}
	}

	// Artifact special name
	public string name {
		get {
			if(_name != "")
				return _name;
			
#if !LOBBY_SERVER
			if(ArtifactsGUI.instance != null) {
				_name = ArtifactsGUI.instance.GetArtifactName(this);
				return _name;
			}
#endif
			
			_name = "Unknown Artifact";
			return "Unknown Artifact";
		}
	}
	
	// Artifact tooltip
	public string tooltip {
		get {
			//this.id = this.id;
			string tip = string.Format("<color={0}><b>L{1}: {2}</b></color>", rarityColors[(int)this.rarity], level + 1, this.name);
			foreach(var stat in stats) {
				tip += "\n" + stat.ToString() + " +" + (level + 1);
			}
			return tip;
		}
	}
	
	// Artifact item ID
	public int id {
		get {
			return 	(int)stats[0] +
					(int)stats[1] 	* (6) +
					(int)stats[2] 	* (36) +
					(int)level 		* (216);
		}
		
		set {
			int rest = value;
			
			level = (byte)(rest / 216);
			rest -= (int)level * 216;
			
			stats[2] = (Artifact.Stat)(rest / 36);
			rest -= (int)stats[2] * 36;
			
			stats[1] = (Artifact.Stat)(rest / 6);
			rest -= (int)stats[1] * 6;
			
			stats[0] = (Artifact.Stat)rest;
		}
	}

	// WriteItemMetaData
	public void WriteItemMetaData(Jboy.JsonWriter writer) {
		// TODO: Enchantments
	}

	// ReadItemMetaData
	public void ReadItemMetaData(Jboy.JsonReader reader) {
		// TODO: Enchantments
	}

	// Writer
	public static void JsonSerializer(Jboy.JsonWriter writer, object instance) {
		ItemSerializer.JsonSerializer(writer, instance);
	}
	
	// Reader
	public static object JsonDeserializer(Jboy.JsonReader reader) {
		return ItemSerializer.JsonDeserializer(reader);
	}
}
