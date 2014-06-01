using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Attunement : Cooldown {
	public static Dictionary<int, Attunement> idToAttunement = new Dictionary<int, Attunement>();
	public static byte Undefined = 255;
	//public static byte attunementCount = 0;
	
	public string name;
	public int id;
	public List<Skill> skills;
	public Texture2D icon;
	
	//private byte _id;
	
	// TODO: Variable arg list
	public Attunement(string nName, int nID, Skill[] skillInitList = null) {
		//_id = Attunement.attunementCount;
		name = nName;
		id = nID;
		skills = new List<Skill>();
		cooldown = 0.1f;
		
		if(skillInitList != null) {
			foreach(Skill skill in skillInitList) {
				this.AddSkill(Skill.Copy(skill));
			}
		}
		
		this.PostSerialize(null);
	}

	// PostSerialize
	public void PostSerialize(Weapon weapon) {
		if(!uLink.Network.isServer)
			icon = (Texture2D)Resources.Load("Attunements/Icons/" + name);
		
		if(!Attunement.idToAttunement.ContainsKey(id))
			Attunement.idToAttunement[id] = this;
		
		Skill.PostSerialize(weapon, this, skills);
	}

	// PostSerialize
	public static void PostSerialize(Weapon weapon, List<Attunement> attunements) {
		foreach(var attunement in attunements) {
			attunement.PostSerialize(weapon);
		}
	}

	// AddSkill
	public void AddSkill(Skill skill) {
		skill.attunement = this;
		skills.Add(skill);
	}
	
	/*public byte id {
		get { return _id; }
	}*/
}
