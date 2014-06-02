using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Magic : MonoBehaviour {
	public static Magic instance;
	public static Skill EmptySkill;
	public static Skill EmptyAutoAttackSkill;
	
	public Texture2D castDurationIcon;
	public Texture2D cooldownIcon;
	public Texture2D runeIcon;
	public Texture2D runeDetonationIcon;
	public Texture2D ccIcon;
	public Texture2D lifeDrainIcon;
	public Texture2D blockCostIcon;
	public Texture2D skillLevelIcon;
	
	public List<Weapon> weapons = new List<Weapon>();
	public Attunement runeDetonators;
	public Skill[] swordMagic1;
	
	// Awake
	void Awake() {
		// Don't destroy this object on level loading
		if(instance == null) {
			this.Init();
			DontDestroyOnLoad(this.gameObject);
			instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}
	
	// Init
	public void Init() {
		SkillEffect.InitSkillEffects();
		this.PostSerialize();
		
		// Empty skills for skill build
		EmptySkill = new Skill("Remove skill", Skill.Undefined, Skill.SkillType.Normal, false, null);
		EmptyAutoAttackSkill = new Skill("Remove auto attack", Skill.Undefined - 1, Skill.SkillType.AutoAttack, false, null);
		
		EmptySkill.LoadResources();
		EmptyAutoAttackSkill.LoadResources();
	}
	
	// Post serialization
	public void PostSerialize() {
		Weapon.PostSerialize(weapons);
		runeDetonators.PostSerialize(null);
	}
	
	// Get a copy of the weapons
	public List<Weapon> GetWeapons(List<Rune> runes, SkillBuild build) {
		// Create runes for each skill
		foreach(var skill in runeDetonators.skills) {
			runes.Add(new Rune(skill));
		}
		
		// Weapon list
		var weapons = new List<Weapon>();
		
		foreach(var weaponBuild in build.weapons) {
			LogManager.Spam.Log("Trying to find weapon ID " + weaponBuild.weaponId);
			var originalWeapon = Weapon.idToWeapon[weaponBuild.weaponId];
			var weapon = new Weapon(originalWeapon);

			LogManager.Spam.Log("Registered weapon " + weapon.name);
			weapons.Add(weapon);
			
			// Add attunements from attunement IDs
			foreach(var attunementBuild in weaponBuild.attunements) {
				var originalAttunement = Attunement.idToAttunement[attunementBuild.attunementId];
				var attunement = new Attunement(
					originalAttunement.name,
					originalAttunement.id,
					null
				);
				
				// Add skills from skill IDs
				foreach(var skillId in attunementBuild.skills) {
					try {
						var skill = Skill.idToSkill[skillId];
						attunement.AddSkill(Skill.Copy(skill));
					} catch(KeyNotFoundException) {
						LogManager.General.LogError("Could not find skill with ID: " + skillId);
					}
				}
				
				weapon.AddAttunement(attunement);
			}
		}
		
		return weapons;
	}
	
	public AudioClip randomCastVoiceClip {
		get {
			var audioClips = new List<AudioClip>();
			
			foreach(var weapon in weapons) {
				foreach(var attunement in weapon.attunements) {
					foreach(var skill in attunement.skills) {
						foreach(var stage in skill.stages) {
							foreach(var castVoice in stage.castVoices) {
								audioClips.Add(castVoice);
							}
						}
					}
				}
			}
			
			return audioClips[Random.Range(0, audioClips.Count)];
		}
	}
	
	/*public Attunement GetSwordMagic1() {
		return new Attunement("Wind", 30, this.swordMagic1);
	}*/
}
