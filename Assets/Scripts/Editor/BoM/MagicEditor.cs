using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Magic))]
public class MagicEditor : Editor {
	bool toggleIcons = false;
	const int descriptionHeight = 80;
	Dictionary<int, bool> toggled = new Dictionary<int, bool>();
	GUILayoutOption valueHeight;
	Magic magic;
	
	// Inspector
	public override void OnInspectorGUI() {
		valueHeight = GUILayout.Height(16);
		magic = (Magic)target;
		magic.Init();
		
		// No indentation
		EditorGUI.indentLevel = 0;
		
		// Weapons
		using(new GUIVertical()) {
			for(int i = 0; i < magic.weapons.Count; i++) {
				DrawWeapon(magic.weapons[i]);
			}
			
			using(new GUIHorizontal()) {
				GUILayout.FlexibleSpace();
				
				if(GUILayout.Button("Add weapon")) {
					magic.weapons.Add(new Weapon("Unknown weapon", 0, false));
				}
				
				if(GUILayout.Button("Remove weapon") && magic.weapons.Count > 0) {
					magic.weapons.RemoveAt(magic.weapons.Count - 1);
				}
			}
			
			// Runes
			DrawAttunement(magic.runeDetonators);
		}
		
		// Icons
		toggleIcons = EditorGUILayout.Foldout(toggleIcons, "Icons");
		if(toggleIcons) {
			using(new GUIVertical()) {
				magic.castDurationIcon = PickIcon(magic.castDurationIcon, "Cast Duration");
				magic.cooldownIcon = PickIcon(magic.cooldownIcon, "Cooldown");
				magic.runeIcon = PickIcon(magic.runeIcon, "Rune");
				magic.runeDetonationIcon = PickIcon(magic.runeDetonationIcon, "Rune Detonation");
				magic.ccIcon = PickIcon(magic.ccIcon, "CC");
				magic.lifeDrainIcon = PickIcon(magic.lifeDrainIcon, "Life Drain");
				magic.blockCostIcon = PickIcon(magic.blockCostIcon, "Energy Cost");
				magic.skillLevelIcon = PickIcon(magic.skillLevelIcon, "Skill Level");
			}
		}
		
		if(GUI.changed)
			EditorUtility.SetDirty(magic);
	}
	
	// Draw weapon
	void DrawWeapon(Weapon weapon) {
		var attunements = weapon.attunements;
		
		using(new GUIHorizontal()) {
			GUILayout.Label("Weapon: " + weapon.name);
			GUILayout.FlexibleSpace();
			

		}
		
		using(new GUIHorizontal()) {
			weapon.id = EditorGUILayout.IntField(weapon.id, GUILayout.Width(40));
			weapon.name = EditorGUILayout.TextField(weapon.name);
			weapon.runeType = (Skill.RuneType)EditorGUILayout.EnumPopup(weapon.runeType, GUILayout.Width(80));
			weapon.autoTarget = EditorGUILayout.Toggle(weapon.autoTarget, GUILayout.Width(16));
		}
		
		for(int i = 0; i < attunements.Count; i++) {
			DrawAttunement(attunements[i]);
		}

		using(new GUIHorizontal()) {
			GUILayout.FlexibleSpace();

			// Add attunement
			if(GUILayout.Button("Add attunement")) {
				attunements.Add(new Attunement("Unknown attunement", 0, null));
			}

			// Remove attunement
			if(GUILayout.Button("Remove attunement") && attunements.Count > 0) {
				attunements.RemoveAt(attunements.Count - 1);
			}
		}

		EditorGUILayout.Separator();
	}
	
	// Draw attunement
	void DrawAttunement(Attunement attunement) {
		var skills = attunement.skills;
		
		// Header
		using(new GUIHorizontal()) {
			//GUILayout.Label("<b><size=14>" + attunement.name + "</size></b>");
			attunement.id = EditorGUILayout.IntField(attunement.id, GUILayout.Width(40));
			attunement.name = EditorGUILayout.TextField(attunement.name, GUILayout.Width(100));
			GUILayout.FlexibleSpace();
		}
		
		// Skills
		DrawSkills(skills);

		// Add skill
		using(new GUIHorizontal()) {
			GUILayout.FlexibleSpace();

			if(GUILayout.Button("Add skill")) {
				skills.Add(new Skill("Unknown skill", skills.Count == 0 ? 0 : skills[skills.Count - 1].id + 1, Skill.SkillType.Normal, false, null));
			}

			// Remove skill
			if(GUILayout.Button("Remove skill") && skills.Count > 0) {
				skills.RemoveAt(skills.Count - 1);
			}
		}

		EditorGUILayout.Separator();
	}
	
	// Draw skills
	void DrawSkills(List<Skill> skills) {
		for(int i = 0; i < skills.Count; i++) {
			var skill = skills[i];
			var skillHash = skill.GetHashCode();
			
			// Skill info
			using(new GUIHorizontal()) {
				bool foldedOut = false;
				toggled.TryGetValue(skillHash, out foldedOut);
				toggled[skillHash] = EditorGUILayout.Foldout(foldedOut, GUIContent.none);
				
				skill.id = EditorGUILayout.IntField(skill.id, GUILayout.Width(40));
				
				//GUILayout.Label(new GUIContent(skill.icon), GUILayout.Width(16));
				var oldPadding = GUI.skin.box.padding;
				GUI.skin.box.padding = new RectOffset();
				GUILayout.Box(new GUIContent(skill.icon), GUILayout.Width(16), GUILayout.Height(16));
				GUI.skin.box.padding = oldPadding;
				
				skill.skillName = EditorGUILayout.TextField(skill.skillName);
				skill.type = (Skill.SkillType)EditorGUILayout.EnumPopup(skill.type, GUILayout.Width(80));
				
				var upDownWidth = GUILayout.Width(24);
				
				GUI.enabled = (i != 0);
				if(GUILayout.Button("U", upDownWidth)) {
					var tmp = skills[i];
					skills[i] = skills[i - 1];
					skills[i - 1] = tmp;
				}
				
				GUI.enabled = (i != skills.Count - 1);
				if(GUILayout.Button("D", upDownWidth)) {
					var tmp = skills[i];
					skills[i] = skills[i + 1];
					skills[i + 1] = tmp;
				}
				
				GUI.enabled = true;
			}
			
			// Stages view toggled?
			if(!toggled[skillHash])
				continue;
			
			// Can hold?
			skill.canHold = EditorGUILayout.Toggle(new GUIContent("Can Hold"), skill.canHold);
			
			var stages = skill.stages;
			
			// Stages
			using(new GUIHorizontal()) {
				GUI.skin.label.padding = new RectOffset(2, 2, 0, 0);
				using(new GUIVertical()) {
					GUILayout.Label("");
					GUILayout.Label("Description", GUILayout.Height(descriptionHeight));
					GUILayout.Space(1);
					GUILayout.Space(1);
					GUILayout.Label("Cast Duration", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Cooldown", valueHeight);
					GUILayout.Space(2);
					GUILayout.Label("Effect", valueHeight);
					GUILayout.Label("Animation", valueHeight);
					GUILayout.Label("Position Type", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Position Offset", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Rotation Type", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Cast Effect", valueHeight);
					GUILayout.Space(2);
					GUILayout.Label("Cast Voices", valueHeight);
					GUILayout.Space(2);
					GUILayout.Label("Energy Cost", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Life Drain", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Power Multiplier", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Stagger Duration", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Can Move - Cast", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Can Move - Attack", valueHeight);
					GUILayout.Space(1);
					GUILayout.Label("Rune Detonator", valueHeight);
					GUILayout.Label("Rune Type", valueHeight);
				}
				
				for(int h = 0; h < stages.Count; h++) {
					using(new GUIVertical()) {
						GUILayout.Label("Stage " + (h + 1));
						DrawSkillStage(stages[h]);
					}
				}
			}
			
			// Stage add/remove buttons
			using(new GUIHorizontal()) {
				GUILayout.FlexibleSpace();
				
				if(GUILayout.Button("Add stage")) {
					if(stages.Count > 0)
						stages.Add(Skill.Stage.Copy(stages[stages.Count - 1]));
					else
						stages.Add(new Skill.Stage());
				}
				
				if(GUILayout.Button("Remove stage") && stages.Count > 0) {
					stages.RemoveAt(stages.Count - 1);
				}
			}
		}
	}
	
	// Draw a skill stage
	void DrawSkillStage(Skill.Stage stage) {
		// Position
		/*switch(stage.posType) {
			case Skill.PositionType.AtRightHand:
				break;
				
			case Skill.PositionType.AboveCaster:
				break;
				
			case Skill.PositionType.AtHitPoint:
				break;
		}*/
		
		GUI.skin.textField.wordWrap = true;
		stage.description = EditorGUILayout.TextField(stage.description, GUILayout.Width(0), GUILayout.Height(descriptionHeight), GUILayout.ExpandWidth(true));
		stage.castDuration = EditorGUILayout.FloatField(stage.castDuration, valueHeight);
		stage.cooldown = EditorGUILayout.FloatField(stage.cooldown, valueHeight);
		stage.effectId = (SkillEffectId)EditorGUILayout.EnumPopup(stage.effectId, valueHeight);
		stage.animType = (Skill.CastAnimation)EditorGUILayout.EnumPopup(stage.animType, valueHeight);
		stage.posType = (Skill.PositionType)EditorGUILayout.EnumPopup(stage.posType, valueHeight);
		
		using(new GUIHorizontal()) {
			//stage.posOffset.x = EditorGUILayout.FloatField(stage.posOffset.x, valueHeight);
			stage.posOffset.y = EditorGUILayout.FloatField(stage.posOffset.y, valueHeight);
			//stage.posOffset.z = EditorGUILayout.FloatField(stage.posOffset.z, valueHeight);
		}
		
		stage.rotType = (Skill.RotationType)EditorGUILayout.EnumPopup(stage.rotType, valueHeight);
		stage.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField(stage.castEffectPrefab, typeof(GameObject), false, valueHeight);
		//stage.attackParticlesType = (Skill.ParticlesType)EditorGUILayout.EnumPopup(stage.attackParticlesType, valueHeight);
		
		using(new GUIHorizontal()) {
			for(int i = 0; i < stage.castVoices.Length; i++) {
				stage.castVoices[i] = (AudioClip)EditorGUILayout.ObjectField(stage.castVoices[i], typeof(AudioClip), false, valueHeight);
			}
			
			if(GUILayout.Button("+", GUILayout.Width(22))) {
				var nList = new List<AudioClip>(stage.castVoices);
				nList.Add(null);
				stage.castVoices = nList.ToArray();
			}
			
			if(GUILayout.Button("-", GUILayout.Width(22))) {
				var nList = new List<AudioClip>(stage.castVoices);
				nList.RemoveAt(nList.Count - 1);
				stage.castVoices = nList.ToArray();
			}
		}
		
		stage.energyCostAbs = EditorGUILayout.FloatField(stage.energyCostAbs, valueHeight);
		stage.lifeDrainRel = EditorGUILayout.FloatField(stage.lifeDrainRel, valueHeight);
		stage.powerMultiplier = EditorGUILayout.FloatField(stage.powerMultiplier, valueHeight);
		stage.staggerDuration = EditorGUILayout.FloatField(stage.staggerDuration, valueHeight);
		stage.canMoveWhileCasting = EditorGUILayout.Toggle(stage.canMoveWhileCasting, valueHeight);
		stage.canMoveWhileAttacking = EditorGUILayout.Toggle(stage.canMoveWhileAttacking, valueHeight);
		stage.isRuneDetonator = EditorGUILayout.Toggle(stage.isRuneDetonator, valueHeight);
		stage.runeType = (Skill.RuneType)EditorGUILayout.EnumPopup(stage.runeType, valueHeight);
	}
	
	// Pick icon
	Texture2D PickIcon(Texture2D tex, string label) {
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label(label);
		tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), false, GUILayout.Width(32), GUILayout.Height(32));
		EditorGUILayout.EndVertical();
		return tex;
	}
	
	// Scene
	/*public void OnSceneGUI() {
		var magic = (Magic)target;
		
		if(GUI.changed)
			EditorUtility.SetDirty(magic);
	}*/
}
