using uLink;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Skill {
	public static Dictionary<string, int> nameToId = new Dictionary<string, int>();
	public static Dictionary<int, Skill> idToSkill = new Dictionary<int, Skill>();
	public static string[] prefabPostfixForLevel = {"I", "II", "III", "IV", "V"};
	public static float genericAnimDuration = 0.30f;
	public static int Undefined = (int)short.MaxValue;
	
	// TODO: Parametrized position types
	public enum PositionType {
		AtRightHand,
		AtCasterFeet,
		AtHitPoint,
		AtGround,
		
		// Weapons and hands
		/*AtHands,
		AtLeftHand,
		AtRightHand,
		AtWeaponHandles,
		AtWeaponBlades*/
	}
	
	public enum RotationType {
		None,
		ToHitPoint,
		ToCasterOnY,
		Up,
	}
	
	public enum CastAnimation {
		None,
		Projectile,
		AoE,
		Sword,
		SideSlashLToR,
		HandRDown,
		HandsLDownRUp,
		DiagonalSlashTopRToBottomL,
		Thrust,
	}
	
	public enum RuneType {
		None,
		FireRune,
		IceRune,
		DarkRune,
		LightRune,
		WindRune,
		LightningRune,
	}
	
	public enum SkillType {
		Normal,
		Elite,
		AutoAttack,
	}
	
	public string skillName = "";
	public int id;
	public SkillType type;
	public bool canHold;
	public List<Stage> stages;
	
	// Constructor
	public Skill(
		string nSkillName,
		int nID,
		SkillType nType,
		bool nCanHold,
		List<Stage> nStages
	) {
		skillName = nSkillName;
		id = nID;
		type = nType;
		canHold = nCanHold;
		
		// Copy each stage
		if(nStages != null && nStages.Count > 0) {
			stages = new List<Stage>();
			for(int i = 0; i < nStages.Count; i++) {
				stages.Add(Stage.Copy(nStages[i]));
			}
		} else {
			stages = new List<Stage>();
		}
		
		this.PostSerialize();
	}

	// PostSerialize
	public void PostSerialize() {
		// Set currentStage and currentStageIndex
		currentStageIndex = 0;
		
		// Prefabs
		prefabs = new GameObject[Rune.maxLevel];
		
		// Skill Id
		if(!Skill.idToSkill.ContainsKey(id)) {
			Skill.idToSkill.Add(id, this);
			//LogManager.General.Log("Skill registered: " + skillName + " with ID " + id);
		}
		
		if(!Skill.nameToId.ContainsKey(skillName)) {
			Skill.nameToId.Add(skillName, id);
		}
		
		LoadResources();
	}

	// PostSerialize
	public static void PostSerialize(Weapon nWeapon, Attunement nAttunement, List<Skill> skills) {
		foreach(var skill in skills) {
			skill.attunement = nAttunement;
			skill.weapon = nWeapon;
			skill.PostSerialize();
		}
	}
	
	// Stage
	[System.Serializable]
	public class Stage : Cooldown {
		public string description = "";
		public float castDuration = 1.0f;
		public PositionType posType = PositionType.AtHitPoint;
		public Vector3 posOffset = Cache.vector3Zero;
		public RotationType rotType = RotationType.None;
		public CastAnimation animType = CastAnimation.Projectile;
		public SkillEffectId effectId = SkillEffectId.None;
		public GameObject castEffectPrefab = null;
		//public ParticlesType attackParticlesType = ParticlesType.None;
		public AudioClip[] castVoices;
		public float powerMultiplier = 1.0f;
		public float staggerDuration = 0.0f;
		public float energyCostAbs = 0.0f;
		public float lifeDrainRel = 0.0f;
		public bool canMoveWhileCasting = true;
		public bool canMoveWhileAttacking = true;
		public bool isRuneDetonator = false;
		public RuneType runeType = RuneType.None;
		
		[System.NonSerialized]
		public SkillEffect effect = SkillEffect.None;

		// Constructor
		public Stage(
			string nDescription = "",
			float nCastDuration = 0.0f,
			float nCooldown = 0.0f,
			PositionType nPosType = PositionType.AtHitPoint,
			Vector3 nPosOffset = default(Vector3),
			RotationType nRotType = RotationType.None,
			CastAnimation nAnimType = CastAnimation.None,
			SkillEffectId nEffectId = SkillEffectId.None,
			GameObject nCastEffectPrefab = null,
			//ParticlesType nAttackParticlesType = ParticlesType.None,
			AudioClip[] nCastVoices = null,
			float nPowerMultiplier = 1.0f,
			float nStaggerDuration = 0.0f,
			float nBlockCostAbs = 0.0f,
			float nLifeDrainRel = 0.0f,
			bool nCanMoveWhileCasting = true,
			bool nCanMoveWhileAttacking = true,
			bool nIsRuneDetonator = false,
			RuneType nRuneType = RuneType.None
		) {
			description = nDescription;
			castDuration = nCastDuration;
			//nCooldown = 0.0f;
			originalCooldown = nCooldown;
			cooldown = nCooldown;
			posType = nPosType;
			posOffset = nPosOffset;
			rotType = nRotType;
			animType = nAnimType;
			effectId = nEffectId;
			castEffectPrefab = nCastEffectPrefab;
			//attackParticlesType = nAttackParticlesType;
			castVoices = nCastVoices;
			effect = SkillEffect.skillEffectList[(int)effectId];
			energyCostAbs = nBlockCostAbs;
			lifeDrainRel = nLifeDrainRel;
			powerMultiplier = nPowerMultiplier;
			staggerDuration = nStaggerDuration;
			
			// Check
			if(lifeDrainRel > 1f) {
				LogManager.General.LogError("More than 100% life drain is not allowed for ranking reasons!");
			}
			
			if(castVoices == null) {
				castVoices = new AudioClip[0];
			}
			
			canMoveWhileCasting = nCanMoveWhileCasting;
			canMoveWhileAttacking = nCanMoveWhileAttacking;
			
			// Runes
			isRuneDetonator = nIsRuneDetonator;
			runeType = nRuneType;
		}

		// Copy
		public static Stage Copy(Stage stage) {
			if(stage == null)
				return null;
			
			return new Stage(
				stage.description,
				stage.castDuration,
				stage.cooldown,
				stage.posType,
				stage.posOffset,
				stage.rotType,
				stage.animType,
				stage.effectId,
				stage.castEffectPrefab,
				//stage.attackParticlesType,
				stage.castVoices,
				stage.powerMultiplier,
				stage.staggerDuration,
				stage.energyCostAbs,
				stage.lifeDrainRel,
				stage.canMoveWhileCasting,
				stage.canMoveWhileAttacking,
				stage.isRuneDetonator,
				stage.runeType
			);
		}

		// Attack anim duration
		public float attackAnimDuration {
			get {
				switch(animType) {
					case CastAnimation.SideSlashLToR:
						return 0.4f;

					case Skill.CastAnimation.Thrust:
						return 0.4f;
					
					default:
						return genericAnimDuration;
				}
			}
		}

		// Cast anim name
		public string castAnimName {
			get {
				if(this.animType == Skill.CastAnimation.None)
					return "";

				return "Cast" + this.animType.ToString();
			}
		}

		// Attack anim name
		public string attackAnimName {
			get {
				if(this.animType == Skill.CastAnimation.None)
					return "";
				
				return "Fire" + this.animType.ToString();
			}
		}

		// Is instant cast
		public bool isInstantCast {
			get {
				return castDuration == 0f;
			}
		}
	}
	
	[System.NonSerialized]
	private byte _currentStageIndex;
	
	[System.NonSerialized]
	private Stage _currentStage;
	
	[System.NonSerialized]
	public Attunement _attunement;
	
	[System.NonSerialized]
	public Weapon _weapon;

	// Icon
	protected WWWResource<Texture2D> iconWWW;
	public Texture2D icon {
		get {
			if(iconWWW == null)
				iconWWW = new WWWResource<Texture2D>("https://battleofmages.com/assets/skill-icons/" + skillName + ".png");

			return iconWWW.data;
		}
	}
	
	[System.NonSerialized]
	public GameObject[] prefabs;
	
	[System.NonSerialized]
	public Magic magic;

	// DrawDescription
	public void DrawDescription(GUIStyle skillDescriptionStyle, byte stageIndex = 0) {
		var stage = stages[stageIndex];
		
		if(stage == null)
			return;
		
		if(magic == null)
			magic = GameObject.Find("Skills").GetComponent<Magic>();
		
		GUILayout.BeginHorizontal();
		
		// Name
		GUILayout.Label("<b>" + skillName + "</b>", skillDescriptionStyle);
		GUILayout.FlexibleSpace();
		
		// Additional info
		
		GUILayout.Label(new GUIContent(stage.castDuration.ToString("0.0") + "s", magic.castDurationIcon, "Cast time"), skillDescriptionStyle);
		GUILayout.Space(8);
		GUILayout.Label(new GUIContent(stage.cooldown.ToString("0.0") + "s", magic.cooldownIcon, "Cooldown"), skillDescriptionStyle);
		
		GUILayout.EndHorizontal();
		GUILayout.Space(8);
		
		// Description
		GUILayout.Label(stage.description, skillDescriptionStyle);
		GUILayout.FlexibleSpace();
		
		// Other infos
		if(stage.effect != null) {
			GUILayout.Space(2);
			GUILayout.Label(new GUIContent(" " + stage.effect.type + " (" + stage.effect.duration.ToString("0.0") + " s)", magic.ccIcon), skillDescriptionStyle);
		}
		
		if(stage.runeType != RuneType.None) {
			GUILayout.Space(2);
			GUILayout.Label(new GUIContent(" " + stage.runeType.ToString(), magic.runeIcon), skillDescriptionStyle);
		}
		
		if(stage.isRuneDetonator) {
			GUILayout.Space(2);
			GUILayout.Label(new GUIContent(" Detonates runes", magic.runeDetonationIcon), skillDescriptionStyle);
		}
		
		if(stage.energyCostAbs != 0f) {
			GUILayout.Space(2);
			GUILayout.Label(new GUIContent(" Cost: " + stage.energyCostAbs.ToString("0") + " block", magic.blockCostIcon), skillDescriptionStyle);
		}
		
		if(stage.lifeDrainRel != 0f) {
			GUILayout.Space(2);
			GUILayout.Label(new GUIContent(" Life drain: " + (stage.lifeDrainRel * 100).ToString("0") + "% of inflicted damage", magic.lifeDrainIcon), skillDescriptionStyle);
		}
	}

	// Copy
	public static Skill Copy(Skill skill) {
		return new Skill(
			skill.skillName,
			skill.id,
			skill.type,
			skill.canHold,
			skill.stages
		);
	}
	
	/*public short id {
		get { return _id; }
	}*/

	// Attunement
	public Attunement attunement {
		get{
			return _attunement;
		}
		
		set {
			_attunement = value;
		}
	}

	// Weapon
	public Weapon weapon {
		get{
			return _weapon;
		}
		
		set {
			_weapon = value;
		}
	}

	// Current stage
	public Stage currentStage {
		get { return _currentStage; }
	}

	// Current stage index
	public byte currentStageIndex {
		get { return _currentStageIndex; }
		set {
			_currentStageIndex = value;
			
			if(_currentStageIndex < stages.Count)
				_currentStage = stages[_currentStageIndex];
			else
				_currentStage = null;
		}
	}

	// Skill stage name
	public string skillStageName {
		get {
			return skillName + " " + Skill.prefabPostfixForLevel[_currentStageIndex];
		}
	}

	// Has next stage
	public bool hasNextStage {
		get { return _currentStageIndex < stages.Count - 1; }
	}

	// Can advance
	public bool canAdvance {
		get { return _currentStageIndex < stages.Count - 1 && !stages[_currentStageIndex + 1].isOnCooldown; }
	}

	// LoadResources
	public void LoadResources() {
		string rootFolder = "";

		if(skillName.StartsWith("Summon ") && _attunement == null) {
			rootFolder += "Summons/";
		} else {
			if(_weapon != null) {
				rootFolder += "Weapons/" + _weapon.name + "/";
			}
			
			if(_attunement != null) {
				rootFolder += _attunement.name + "/";
			}
		}
		
		rootFolder += skillName + "/";
		
		try {
			for(int i = 0; i < Rune.maxLevel; i++) {
				prefabs[i] = (GameObject)Resources.Load(rootFolder + skillName + " " + Skill.prefabPostfixForLevel[i]);
			}
		} catch(System.ArgumentException) {
			LogManager.General.LogWarning("Couldn't load resources for skill '" + skillName + "'");
		}
	}

	// ToString
	public override string ToString() {
		return skillName;
	}
}
