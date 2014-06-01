using UnityEngine;
using System.Collections;
using uLobby;

public sealed class TraitsGUI : LobbyModule<TraitsGUI> {
	public GUIStyle characterStatNameStyle;
	public GUIStyle characterStatValueStyle;
	public GUIStyle statPointsMaxStyle;
	public GUIStyle statRealBonusStyle;
	public GUIStyle headerStyle;
	
	private Vector2 scrollPosition;
	private CharacterStats charStats;
	private CharacterStats lastCharacterStatsSent;
	private InGameLobby inGameLobby;
	
	private string[] tooltips = new string[] {
		"Attack increases your damage",
		"Defense reduces the damage you take",
		"Energy increases the maximum energy",
		"Cooldown reduction reduces your skill cooldowns",
		"Attack speed reduces cast and animation time of skills",
		"Move speed makes you move faster",
	};
	
	// Start
	void Start() {
		inGameLobby = this.GetComponent<InGameLobby>();
		Lobby.AddListener(this);
	}
	
	// Draw
	public override void Draw() {
		charStats = InGameLobby.instance.displayedAccount.charStats;
		
		if(charStats == null)
			return;
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		
		//GUILayout.Label("Traits", headerStyle);
		
		GUILayout.BeginHorizontal("box");
		
		// Keys
		GUILayout.BeginVertical();
		GUILayout.Label(new GUIContent("Attack:", tooltips[0]), characterStatNameStyle);
		GUILayout.Label(new GUIContent("Defense:", tooltips[1]), characterStatNameStyle);
		GUILayout.Label(new GUIContent("Energy:", tooltips[2]), characterStatNameStyle);
		GUILayout.Label(new GUIContent("Cooldown reduction:", tooltips[3]), characterStatNameStyle);
		GUILayout.Label(new GUIContent("Attack speed:", tooltips[4]), characterStatNameStyle);
		GUILayout.Label(new GUIContent("Move speed:", tooltips[5]), characterStatNameStyle);
		GUILayout.EndVertical();
		
		// Values as numbers
		GUILayout.BeginVertical();
		DrawValue(charStats.attack);
		DrawValue(charStats.defense);
		DrawValue(charStats.block);
		DrawValue(charStats.cooldownReduction);
		DrawValue(charStats.attackSpeed);
		DrawValue(charStats.moveSpeed);
		GUILayout.EndVertical();
		
		GUILayout.FlexibleSpace();
		
		var oldCharStats = new CharacterStats(charStats);
		
		GUI.enabled = InGameLobby.instance.displayedAccount.isMine;
		
		// Buttons
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		DrawButtons(ref charStats.attack);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		DrawButtons(ref charStats.defense);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		DrawButtons(ref charStats.block);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		DrawButtons(ref charStats.cooldownReduction);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		DrawButtons(ref charStats.attackSpeed);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		DrawButtons(ref charStats.moveSpeed);
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		
		// Values
		GUILayout.BeginVertical();
		var sliderWidth = GUILayout.Width((GUIArea.width - 16) * 0.52f);
		var sliderHeight = GUILayout.Height(28);
		charStats.attack = (int)GUIHelper.HorizontalSliderVCenter(charStats.attack, 0, 100, sliderWidth, sliderHeight);
		charStats.defense = (int)GUIHelper.HorizontalSliderVCenter(charStats.defense, 0, 100, sliderWidth, sliderHeight);
		charStats.block = (int)GUIHelper.HorizontalSliderVCenter(charStats.block, 0, 100, sliderWidth, sliderHeight);
		charStats.cooldownReduction = (int)GUIHelper.HorizontalSliderVCenter(charStats.cooldownReduction, 0, 100, sliderWidth, sliderHeight);
		charStats.attackSpeed = (int)GUIHelper.HorizontalSliderVCenter(charStats.attackSpeed, 0, 100, sliderWidth, sliderHeight);
		charStats.moveSpeed = (int)GUIHelper.HorizontalSliderVCenter(charStats.moveSpeed, 0, 100, sliderWidth, sliderHeight);
		GUILayout.EndVertical();
		
		// Don't go below 0
		if(charStats.attack < 0)
			charStats.attack = 0;
		
		if(charStats.defense < 0)
			charStats.defense = 0;
		
		if(charStats.block < 0)
			charStats.block = 0;
		
		if(charStats.cooldownReduction < 0)
			charStats.cooldownReduction = 0;
		
		if(charStats.attackSpeed < 0)
			charStats.attackSpeed = 0;
		
		if(charStats.moveSpeed < 0)
			charStats.moveSpeed = 0;
		
		// Cap points
		if(charStats.totalStatPointsUsed > charStats.maxStatPoints) {
			// TODO: Can't we generalize this stuff?
			if(charStats.attack != oldCharStats.attack)
				charStats.attack = oldCharStats.attack + oldCharStats.statPointsLeft;
			
			if(charStats.defense != oldCharStats.defense)
				charStats.defense = oldCharStats.defense + oldCharStats.statPointsLeft;
			
			if(charStats.block != oldCharStats.block)
				charStats.block = oldCharStats.block + oldCharStats.statPointsLeft;
			
			if(charStats.cooldownReduction != oldCharStats.cooldownReduction)
				charStats.cooldownReduction = oldCharStats.cooldownReduction + oldCharStats.statPointsLeft;
			
			if(charStats.attackSpeed != oldCharStats.attackSpeed)
				charStats.attackSpeed = oldCharStats.attackSpeed + oldCharStats.statPointsLeft;
			
			if(charStats.moveSpeed != oldCharStats.moveSpeed)
				charStats.moveSpeed = oldCharStats.moveSpeed + oldCharStats.statPointsLeft;
		}
		
		GUILayout.EndHorizontal();
		
		GUI.enabled = true;
		
		GUI.backgroundColor = new Color(0f, 0f, 0f, 0f);
		GUILayout.BeginVertical("box");
		GUILayout.Label(charStats.totalStatPointsUsed + " / " + charStats.maxStatPoints + " stat points used", statPointsMaxStyle);
		GUILayout.EndVertical();
		GUI.backgroundColor = Color.white;
		
		// Footer
		GUILayout.FlexibleSpace();
		
		// Bonus
		var prefix = "<size=32>+";
		var postfix = "%</size>\n";
		
		// We "fake" the values shown
		charStats.ApplyOffset(50);
		
		GUILayout.Label("Stats", headerStyle);
		
		GUILayout.BeginVertical("box");
		GUILayout.Space(8);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(new GUIContent(prefix + ((charStats.attackDmgMultiplier - 1.0f) * 100).ToString("0") + postfix + "Damage", tooltips[0]), statRealBonusStyle);
		GUILayout.FlexibleSpace();
		GUILayout.Label(new GUIContent(prefix + ((1.0f - charStats.defenseDmgMultiplier) * 100).ToString("0") + postfix + "Damage reduction", tooltips[1]), statRealBonusStyle);
		GUILayout.FlexibleSpace();
		GUILayout.Label(new GUIContent(prefix + ((charStats.energyMultiplier - 1.0f) * 100).ToString("0") + postfix + "Energy", tooltips[2]), statRealBonusStyle);
		GUILayout.FlexibleSpace();
		GUILayout.Label(new GUIContent(prefix + ((1.0f - charStats.cooldownMultiplier) * 100).ToString("0") + postfix + "Cooldown reduction", tooltips[3]), statRealBonusStyle);
		GUILayout.FlexibleSpace();
		GUILayout.Label(new GUIContent(prefix + ((1.0f - charStats.attackSpeedMultiplier) * 100).ToString("0") + postfix + "Attack speed", tooltips[4]), statRealBonusStyle);
		GUILayout.FlexibleSpace();
		GUILayout.Label(new GUIContent(prefix + ((charStats.moveSpeedMultiplier - 1.0f) * 100).ToString("0") + postfix + "Move speed", tooltips[5]), statRealBonusStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(8);
		GUILayout.EndVertical();
		
		// Reset the fake
		charStats.ApplyOffset(-50);
		
		GUILayout.FlexibleSpace();
		
		GUILayout.EndScrollView();
		
		// Save or reset traits
		DrawToolbar();
	}
	
	// Save or reset traits
	void DrawToolbar() {
		if(inGameLobby.displayedAccount.isMine) {
			if(lastCharacterStatsSent.Compare(charStats) == false)
				GUI.enabled = true;
			else
				GUI.enabled = false;
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			// Save button
			if(GUI.enabled)
				GUI.backgroundColor = Color.yellow;
			else
				GUI.backgroundColor = Color.white;
			
			if(GUIHelper.Button("Save", GUILayout.Width(96)) && charStats.valid) {
				Sounds.instance.PlayButtonClick();
				Lobby.RPC("ClientCharacterStats", Lobby.lobby, charStats);
				
				if(Player.main != null)
					Player.main.networkView.RPC("CharacterStatsUpdate", uLink.RPCMode.Server, charStats);
				
				PlayerAccount.mine.charStats = charStats;
				lastCharacterStatsSent = new CharacterStats(charStats);
			}
			
			// Reset button
			GUI.backgroundColor = Color.white;
			if(GUIHelper.Button("Reset", GUILayout.Width(96))) {
				Sounds.instance.PlayButtonClick();
				PlayerAccount.mine.charStats = new CharacterStats(lastCharacterStatsSent);
			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(8);
			
			GUI.enabled = true;
		}
	}
	
	void DrawValue(int val) {
		if(val >= 75)
			GUI.contentColor = Color.green;
		else if(val >= 50)
			GUI.contentColor = new Color(0.0f, 1.0f, 1.0f, 1.0f);
		else if(val >= 25)
			GUI.contentColor = Color.yellow;
		else
			GUI.contentColor = Color.red;
		
		GUILayout.Label(val.ToString(), characterStatValueStyle);
		GUI.contentColor = Color.white;
	}
	
	void DrawButtons(ref int stat) {
		var buttonLayout = GUILayout.Width(24);
		bool guiEnabled = GUI.enabled;
		bool accIsMine = inGameLobby.displayedAccount.isMine;
		
		GUI.enabled = accIsMine && (stat > 0);
		if(GUIHelper.Button("-", buttonLayout)) {
			Sounds.instance.PlayButtonClick();
			stat -= 1;
		}
		
		GUI.enabled = accIsMine && (stat < 100) && (charStats.statPointsLeft > 0);
		if(GUIHelper.Button("+", buttonLayout)) {
			Sounds.instance.PlayButtonClick();
			stat += 1;
		}
		
		buttonLayout = GUILayout.Width(42);
		for(int i = 0; i <= 100; i += 25) {
			GUI.enabled = accIsMine && ((i - stat) <= charStats.statPointsLeft) && (i != stat);
			
			if(i != stat)
				GUI.contentColor = (i > stat ? Color.green : Color.red);
			else
				GUI.contentColor = Color.white;
			
			if(GUIHelper.Button(i.ToString(), buttonLayout)) {
				Sounds.instance.PlayButtonClick();
				stat = i;
			}
		}
		
		GUI.enabled = guiEnabled;
		GUI.contentColor = Color.white;
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	void ReceiveCharacterStats(string accountId, CharacterStats charStats) {
		LogManager.General.Log("TraitsGUI: Received character stats!");
		
		var acc = PlayerAccount.Get(accountId);
		acc.charStats = charStats;
		
		if(acc.isMine)
			lastCharacterStatsSent = new CharacterStats(charStats);
	}
	
	[RPC]
	void CharacterStatsSaveError() {
		LogManager.General.LogWarning("Error saving character stats!");
	}
}
