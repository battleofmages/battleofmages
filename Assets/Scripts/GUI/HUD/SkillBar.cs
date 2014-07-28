using UnityEngine;
using System.Collections.Generic;

public class SkillBar : HUDElement {
	public const int defaultMainIconSize = 32;
	public const int defaultMainMargin = 12;
	public const int defaultMainMarginBottom = 37;
	public const int defaultMiniMarginBottom = 3;
	public const int defaultMiniIconSize = 16;
	
	public static int mainIconSize = defaultMainIconSize;
	public static int mainMargin = defaultMainMargin;
	public static int mainMarginBottom = defaultMainMarginBottom;
	public static int miniMarginBottom = defaultMiniMarginBottom;
	public static int miniIconSize = defaultMiniIconSize;
	
	public bool drawBorder;
	
	public GUIStyle skillDescriptionStyle;
	public GUIStyle cooldownNumberStyle;
	public GUIStyle miniCooldownNumberStyle;
	
	public GUIStyle progressBarEmptyStyle;
	public GUIStyle progressBarBGFillStyle;
	public GUIStyle progressBarFullStyle;
	public GUIStyle skillStageTextStyle;
	
	public int skillDescriptionHeight = 140;
	public float lerpSpeed = 5.0f;
	
	private int skillBarWidth;
	private Player player;
	private Attunement lastCurrentAttunement;
	private float time;
	
	private Skill skillToShow;
	private Rect skillDescPos;
	
	// Start
	void Start() {
		player = GetComponent<Player>();
	}
	
	// Update
	void Update() {
		time += Time.deltaTime * lerpSpeed;
	}
	
	// Draw
	public override void Draw() {
		if(player.skillBuild == null)
			return;

		if(lastCurrentAttunement != player.currentAttunement) {
			time = 0f;
			lastCurrentAttunement = player.currentAttunement;
		}

		int skillCount = 5;
		int attunementCount = player.currentWeapon.attunements.Count;
		int miniMargin = mainMargin / 4;
		int miniSkillBarWidth = (skillCount * attunementCount * (miniIconSize + miniMargin) - miniMargin);
		int offsetX = ((int)GUIArea.width) / 2 - miniSkillBarWidth / 2 + (5 * (miniIconSize + miniMargin) - miniMargin) / 2;
		//int offsetX = 0;
		skillToShow = null;
		
		// Fix width and height of the GUI style
		cooldownNumberStyle.fixedWidth = mainIconSize;
		cooldownNumberStyle.fixedHeight = mainIconSize;
		cooldownNumberStyle.fontSize = 18 + (mainIconSize - defaultMainIconSize) / 2;
		miniCooldownNumberStyle.fixedWidth = miniIconSize;
		miniCooldownNumberStyle.fixedHeight = miniIconSize;
		miniCooldownNumberStyle.fontSize = 10 + (miniIconSize - defaultMiniIconSize) / 2;
		
		foreach(var attunement in player.attunements) {
			if(attunement == player.currentAttunement) {
				DrawSkills(
					attunement.skills,
					(int)Mathf.Lerp(miniIconSize, mainIconSize, time),
					(int)Mathf.Lerp(miniMargin, mainMargin, time),
					(int)Mathf.Lerp(miniMarginBottom, miniMarginBottom + mainMarginBottom, time),
					(int)Mathf.Lerp(offsetX, GUIArea.width / 2, time),
					time >= 1.0f ? cooldownNumberStyle : miniCooldownNumberStyle
				);
				
				offsetX += (miniIconSize + miniMargin) * attunement.skills.Count;
				continue;
			}
			
			offsetX += DrawSkills(attunement.skills, miniIconSize, miniMargin, miniMarginBottom, offsetX, miniCooldownNumberStyle) + miniMargin;
		}
		
		// Skill description
		if(skillToShow != null) {
			GUILayout.BeginArea(skillDescPos);
			GUILayout.BeginVertical("box");
			skillToShow.DrawDescription(skillDescriptionStyle);
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
	
	// DrawSkills
	int DrawSkills(List<Skill> skills, int iconSize, int margin, int marginBottom, int offsetX, GUIStyle cdNumberStyle) {
		skillBarWidth = skills.Count * iconSize + (skills.Count - 1) * margin;
		
		// Recalculate it because of attunement changes
		int x = offsetX - skillBarWidth / 2;
		int y = ((int)GUIArea.height) - marginBottom - iconSize;
		int xStart = x;
		
		foreach(Skill skill in skills) {
			if(skill.currentStage == null)
				continue;
			
			if(skill.icon != null) {
				Rect iconPos = new Rect(x, y, iconSize, iconSize);
				bool onCD = skill.currentStage.isOnCooldown && !Debugger.instance.skillTestMode;
				Color iconColor;
				
				if(onCD) {
					float fade = 0.75f - skill.currentStage.cooldownRemainingRelative;
					
					if(fade < 0.0f)
						fade = 0.0f;
					
					fade += 0.25f;
					
					iconColor = new Color(fade, fade, fade, fade);
				// Not enough block capacity
				} else if(skill.currentStage.energyCostAbs > Player.main.energy) {
					iconColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);
				// Currently activating
				} else if(skill == player.currentSkill) {
					// Cast progress
					if(player.currentCastStart != -1 && skill.currentStage.castDuration > 0) {
						int stageTextHeight = 20;
						Rect stageRect = new Rect(x, y - stageTextHeight, iconSize, stageTextHeight);
						
						// TODO: Generic function for shadows
						GUI.color = Color.black;
						stageRect.x += 1;
						stageRect.y += 1;
						GUI.Label(stageRect, Skill.prefabPostfixForLevel[skill.currentStageIndex], skillStageTextStyle);
						
						GUI.color = Color.white;
						stageRect.x -= 1;
						stageRect.y -= 1;
						GUI.Label(stageRect, Skill.prefabPostfixForLevel[skill.currentStageIndex], skillStageTextStyle);
						
						/*int castBarHeight = 8;
						Rect castBarPos = new Rect(x, y + iconSize + 1, iconSize, castBarHeight);
						
						GUIHelper.ProgressBar(
							castBarPos,
							Skill.prefabPostfixForLevel[skill.currentStageIndex],
							(float)((uLink.Network.time - player.currentCastStart) / skill.currentStage.castDuration),
							Color.white,
							progressBarEmptyStyle,
							Color.black,
							progressBarBGFillStyle,
							new Color(0.0f, 0.5f, 1.0f, 1.0f),
							progressBarFullStyle,
							Color.white,
							skillStageTextStyle
						);*/
					}
					
					// Grayed out icon
					iconColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);
				} else {
					iconColor = Color.white;
				}
				
				// Border
				if(drawBorder) {
					Rect borderPos = new Rect(iconPos);
					borderPos.x -= 1;
					borderPos.y -= 1;
					borderPos.width += 2;
					borderPos.height += 2;
					GUI.color = new Color(0f, 0f, 0f, iconColor.a);
					GUI.DrawTexture(borderPos, skill.icon, ScaleMode.StretchToFill, true);
				}
				
				// Draw the actual icon
				GUI.color = iconColor;
				GUI.DrawTexture(iconPos, skill.icon, ScaleMode.StretchToFill, true);
				
				// Cooldown number
				if(onCD) {
					GUI.color = new Color(0f, 0f, 0f, GUI.color.a + 0.3f);
					GUI.Label(iconPos, ((int)(skill.currentStage.cooldownRemaining + 0.99f)).ToStringLookup(), cdNumberStyle);
				}
				
				GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				
				// Skill description
				var mousePos2D = InputManager.GetMousePosition();
				iconPos.x += GUIArea.x;
				iconPos.y += GUIArea.y;
				if(skillToShow == null && iconPos.Contains(mousePos2D)) {
					skillDescPos = new Rect(xStart - skillBarWidth / 2, y - mainMargin, skillBarWidth * 2, skillDescriptionHeight);
					skillDescPos.y -= skillDescPos.height;
					skillToShow = skill;
				}
				
				x += iconSize + margin;
			} else {
				// Skill icon has not been loaded yet
				//LogManager.General.LogWarning("Skill Icon for " + skill.skillName + " missing.");
			}
		}
		
		return skillBarWidth;
	}
}
