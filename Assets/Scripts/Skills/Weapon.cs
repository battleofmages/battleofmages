using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Weapon {
	public static Dictionary<string, Weapon> nameToWeapon = new Dictionary<string, Weapon>();
	public static Dictionary<int, Weapon> idToWeapon = new Dictionary<int, Weapon>();
	public static byte Undefined = 255;
	public const int SummonSkillIdStart = 10000;
	
	public string name;
	public int id;
	public List<Attunement> attunements;
	public Skill.RuneType runeType;
	public bool autoTarget;
	
	[System.NonSerialized]
	public byte currentAttunementId;
	
	[System.NonSerialized]
	public Skill summonSkill;
	
	[System.NonSerialized]
	public float damageMultiplier;

	// Constructor
	public Weapon(string nName, int nID, bool nAutoTarget) {
		name = nName;
		id = nID;
		attunements = new List<Attunement>();
		autoTarget = nAutoTarget;
		runeType = Skill.RuneType.None;
		
		this.PostSerialize();
	}

	// Constructor
	public Weapon(Weapon other) : this(other.name, other.id, other.autoTarget) {
		
	}

	// PostSerialize
	public void PostSerialize() {
		currentAttunementId = 0;
		damageMultiplier = 1.0f;
		
		if(!Weapon.idToWeapon.ContainsKey(id)) {
			Weapon.nameToWeapon[name] = this;
			Weapon.idToWeapon[id] = this;
		}
		
		summonSkill = new Skill("Summon " + name, Weapon.SummonSkillIdStart + id, Skill.SkillType.Normal, false, null);
		summonSkill.stages.Add(new Skill.Stage());
		summonSkill.weapon = this;
		
		Attunement.PostSerialize(this, attunements);
	}

	// PostSerialize
	public static void PostSerialize(List<Weapon> weapons) {
		foreach(var weapon in weapons) {
			weapon.PostSerialize();
		}
	}

	// AddAttunement
	public void AddAttunement(Attunement attunement) {
		foreach(var skill in attunement.skills) {
			skill.weapon = this;
			skill.LoadResources();
		}
		
		attunements.Add(attunement);
	}
}
