using uLobby;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class SkillsGUI : LobbyModule<SkillsGUI> {
	public InGameLobby gameLobby;
	
	public GUIStyle titleStyle;
	public GUIStyle selectableStyle;
	public GUIStyle descriptionStyle;
	
	//private List<Weapon> allWeapons;
	
	private Vector2 scrollPosition;
	private Vector2 scrollPositionSelect;
	
	private Weapon currentWeapon;
	private AttunementBuild currentAttunementBuild;
	private int currentSkillSlotIndex;
	
	private SkillBuild build;
	private Skill showSkill;
	private Skill currentSkill;
	
	// Start
	void Start() {
		//allWeapons = Magic.instance.allWeapons;
		currentWeapon = null;
		currentAttunementBuild = null;
		currentSkillSlotIndex = -1;
		showSkill = null;
		
		// Receive RPCs from the lobby
		Lobby.AddListener(this);
	}
	
	// Draw
	public override void Draw() {
		build = gameLobby.displayedAccount.skillBuild;
		GUI.enabled = gameLobby.displayedAccount.isMine;
		
		if(build == null)
			return;
		
		using(new GUIHorizontal()) {
			// Draw the current build
			using(new GUIVertical("box")) {
				DrawCurrentBuild();
			}
			
			GUILayout.Space(8);
			
			// Attunement / Skill selection
			using(new GUIVertical("box")) {
				DrawSelectableChanges();
			}
			
			GUILayout.Space(8);
			GUI.enabled = true;
			
			// Skill description
			using(new GUIVertical("box", GUILayout.Width(GUIArea.width / 3))) {
				GUILayout.Label("Description", titleStyle);
				
				GUILayout.Space(8);
				
				if(showSkill != null && showSkill.stages.Count > 0) {
					using(new GUIVertical()) {
						showSkill.DrawDescription(descriptionStyle);
					}
				} else {
					GUILayout.FlexibleSpace();
				}
			}
		}
	}
	
	// Draw current build
	void DrawCurrentBuild() {
		using(new GUIScrollView(ref scrollPosition)) {
			using(new GUIHorizontal()) {
				using(new GUIVertical()) {
					foreach(var weaponBuild in build.weapons) {
						DrawWeaponBuild(weaponBuild);
					}
				}
			}
			
			GUILayout.FlexibleSpace();
		}
	}
	
	// Draw weapon build
	void DrawWeaponBuild(WeaponBuild weaponBuild) {
		var weapon = Weapon.idToWeapon[weaponBuild.weaponId];
		GUILayout.Label("Weapon: <b>" + weapon.name + "</b>", titleStyle);
		
		// Reset shown skill description
		ExecuteLater(() => {
			showSkill = null;
		});
		
		// Add attunements from attunement IDs
		foreach(var attunementBuild in weaponBuild.attunements) {
			var lambdaAttunementBuild = attunementBuild;
			var attunement = Attunement.idToAttunement[attunementBuild.attunementId];
			
			if(currentAttunementBuild != null && attunementBuild == currentAttunementBuild && currentSkillSlotIndex == -1)
				GUI.backgroundColor = GUIColor.MenuItemActive;
			else
				GUI.backgroundColor = GUIColor.MenuItemInactive;
			
			using(new GUIHorizontal()) {
				if(GUIHelper.Button(new GUIContent(attunement.icon))) {
					ExecuteLater(() => {
						currentWeapon = weapon;
						currentAttunementBuild = lambdaAttunementBuild;
						currentSkillSlotIndex = -1;
						scrollPositionSelect = Vector2.zero;
					});
				}
				
				// Add skills from skill IDs
				for(int slotIndex = 0; slotIndex < attunementBuild.skills.Length; slotIndex++) {
					var skillId = attunementBuild.skills[slotIndex];
					var skillIdString = skillId.ToStringLookup();
					var skill = Skill.idToSkill[skillId];
					
					if(attunementBuild == currentAttunementBuild && slotIndex == currentSkillSlotIndex)
						GUI.backgroundColor = GUIColor.MenuItemActive;
					else
						GUI.backgroundColor = GUIColor.MenuItemInactive;
					
					if(slotIndex == 0)
						GUILayout.Space(8);
					
					if(GUIHelper.Button(new GUIContent(skill.icon, skillIdString))) {
						var lambdaSlotIndex = slotIndex;
						
						// Remove skill from slot
						if(Event.current.button == 1) {
							ExecuteLater(() => {
								currentSkill = Skill.idToSkill[lambdaAttunementBuild.skills[lambdaSlotIndex]];
								
								var lambdaSkill = currentSkill.type == Skill.SkillType.AutoAttack ? Magic.EmptyAutoAttackSkill : Magic.EmptySkill;
								lambdaAttunementBuild.skills[lambdaSlotIndex] = lambdaSkill.id;
							});
						} else {
							ExecuteLater(() => {
								currentWeapon = weapon;
								currentAttunementBuild = lambdaAttunementBuild;
								currentSkillSlotIndex = lambdaSlotIndex;
								scrollPositionSelect = Vector2.zero;
							});
						}
					}
					
					if(GUI.tooltip == skillIdString) {
						ExecuteLater(() => {
							showSkill = skill;
						});
						GUI.tooltip = null;
					}
					
					if(slotIndex == 0)
						GUILayout.Space(8);
				}
			}
		}
	}
	
	// Draw selectable changes
	void DrawSelectableChanges() {
		using(new GUIScrollView(ref scrollPositionSelect)) {
			using(new GUIVertical(GUILayout.Width(GUIArea.width / 3))) {
				if(currentWeapon != null) {
					if(currentAttunementBuild != null && currentSkillSlotIndex == -1)
						DrawAttunements();
					else if(currentSkillSlotIndex != -1)
						DrawSkills();
				} else {
					GUILayout.Label("", titleStyle);
				}
			}
		}
	}
	
	// Save skill build
	void SaveSkillBuild() {
		// Lobby
		Lobby.RPC("ClientSkillBuild", Lobby.lobby, build);
		
		// Live update on game server
		if(Player.main != null)
			Player.main.networkView.RPC("SkillBuildUpdate", uLink.RPCMode.Server, build);
	}
	
	// Draw attunements
	void DrawAttunements() {
		GUILayout.Label("Elements", titleStyle);
		
		foreach(Attunement attunement in currentWeapon.attunements) {
			if(attunement.id != currentAttunementBuild.attunementId) {
				if(GUIHelper.Button(new GUIContent(" " + attunement.name, attunement.icon), selectableStyle)) {
					var lambdaAttunement = attunement;
					
					ExecuteLater(() => {
						var attunements = build.GetWeaponBuildById(currentWeapon.id).attunements;
						
						for(int i = 0; i < attunements.Length; i++) {
							if(attunements[i] == currentAttunementBuild) {
								attunements[i].SwitchAttunement(lambdaAttunement.id);
								break;
							}
						}
						
						SaveSkillBuild();
					});
				}
			}
		}
	}
	
	// Draw skills
	void DrawSkills() {
		var currentAttunement = Attunement.idToAttunement[currentAttunementBuild.attunementId];
		currentSkill = Skill.idToSkill[currentAttunementBuild.skills[currentSkillSlotIndex]];
		
		GUILayout.Label(currentAttunement.name + (currentSkill.type == Skill.SkillType.AutoAttack ? " auto attacks" : " skills"), titleStyle);
		
		foreach(var skill in currentAttunement.skills) {
			DrawSkill(skill);
		}
		
		// Empty
		/*if(currentSkill.type == Skill.SkillType.AutoAttack)
			DrawSkill(Magic.EmptyAutoAttackSkill);
		else
			DrawSkill(Magic.EmptySkill);*/
	}
	
	// Draw skill
	void DrawSkill(Skill skill) {
		var lambdaSkill = skill;
		
		if(skill != currentSkill && skill.type == currentSkill.type && (skill == Magic.EmptySkill || skill == Magic.EmptyAutoAttackSkill || !build.HasSkill(skill.id))) {
			var skillIdString = skill.id.ToStringLookup();
			
			if(GUIHelper.Button(new GUIContent(" " + skill.skillName, skill.icon, skillIdString), selectableStyle)) {
				ExecuteLater(() => {
					currentAttunementBuild.skills[currentSkillSlotIndex] = lambdaSkill.id;
					if(currentSkillSlotIndex < currentAttunementBuild.skills.Length - 1)
						currentSkillSlotIndex++;
					SaveSkillBuild();
				});
			}
			
			if(GUI.tooltip == skillIdString) {
				ExecuteLater(() => {
					showSkill = lambdaSkill;
				});
				GUI.tooltip = null;
			}
		}
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	void ReceiveSkillBuild(string accountId, SkillBuild build) {
		PlayerAccount.Get(accountId).skillBuild = build;
		LogManager.General.Log("SkillsGUI: Received skill build!");
	}
	
	[RPC]
	void SkillBuildSaveError() {
		LogManager.General.Log("Error saving the skill build!");
	}
}

// AttunementBuild extensions
public static class Extension {
	public static void SwitchAttunement(this AttunementBuild myBuild, int nAttunementId) {
		myBuild.attunementId = nAttunementId;
		
		//var attunement = Attunement.idToAttunement[myBuild.attunementId];
		//attunement.skills[i].id;
		
		myBuild.skills[0] = Magic.EmptyAutoAttackSkill.id;
		
		for(int i = 1; i < myBuild.skills.Length; i++) {
			myBuild.skills[i] = Magic.EmptySkill.id;
		}
	}
}
