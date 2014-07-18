using uLobby;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GuildMenuState {
	ListGuilds,
	CreateGuild,
}

public sealed class GuildsGUI : LobbyModule<GuildsGUI> {
	public GUIStyle headerStyle;
	public GUIStyle guildButtonStyle;
	public GUIStyle guildNameHeaderStyle;
	public GUIStyle guildMemberNameStyle;
	
	[HideInInspector]
	public string statusMessage = "";
	
	[HideInInspector]
	public GuildList guildList = null;
	
	[HideInInspector]
	public int pendingGuildListRequests = 0;
	
	private GuildMenuState guildMenuState;
	private List<string> invitationsList = null;
	private string currentGuildId = "";
	private string guildName = "";
	private string guildTag = "";
	private Vector2 scrollGuildListPosition;
	private Vector2 scrollPosition;
	
	// Start
	void Start() {
		guildMenuState = GuildMenuState.ListGuilds;
		Lobby.AddListener(this);
		
		// Retrieve guild list when logging in
		AccountManager.OnAccountLoggedIn += OnAccountLoggedIn;
	}

	// OnClick
	public override void OnClick() {
		if(pendingGuildListRequests == 0) {
			Sounds.instance.PlayButtonClick();
			
			Lobby.RPC("GuildListRequest", Lobby.lobby);
			pendingGuildListRequests += 1;
		}
	}
	
	// OnAccountLoggedIn
	void OnAccountLoggedIn(Account account) {
		Lobby.RPC("GuildListRequest", Lobby.lobby);
	}
	
	// ChangeState
	void ChangeState(GuildMenuState newState) {
		ExecuteLater(() => {
			guildMenuState = newState;
			statusMessage = "";
		});
	}
	
	// Draw
	public override void Draw() {
		guildList = InGameLobby.instance.displayedAccount.guildList;
		
		using(new GUIVertical()) {
			switch(guildMenuState) {
				case GuildMenuState.ListGuilds:		DrawGuildInterface();		break;
				case GuildMenuState.CreateGuild:	DrawCreateGuildInterface();	break;
			}
		}
	}
	
	// On coming close to the NPC
	public override void OnNPCEnter() {
		// Fetch the new guild list
		InGameLobby.instance.currentLobbyModule = this;
	}
	
	// DrawGuildInterface
	void DrawGuildInterface() {
		//GUILayout.Label("Guilds", headerStyle);
		
		using(new GUIHorizontal()) {
			// Guild list
			DrawGuildList();
			
			// Current guild members
			DrawCurrentGuildMembers();
			
			// Guild invitations
			DrawInvitations();
		}
		
		// TODO: Show status message in the chat
	}
	
	// DrawCreateGuildInterface
	void DrawCreateGuildInterface() {
		// Guild creation
		using(new GUIHorizontal()) {
			// Guild list
			DrawGuildList();
			
			// Create guild
			DrawCreateGuild();
		}
	}
	
	// DrawGuildList
	void DrawGuildList() {
		if(guildList == null)
			return;
		
		using(new GUIVertical("box", GUILayout.MinWidth(GUIArea.width * 0.3f))) {
			scrollGuildListPosition = GUILayout.BeginScrollView(scrollGuildListPosition);
			foreach(var guildId in guildList.idList) {
				if(GameDB.guildIdToGuild.ContainsKey(guildId)) {
					var guild = GameDB.guildIdToGuild[guildId];
					
					// Assign first one if none are selected
					if(currentGuildId == "")
						currentGuildId = guildId;
					
					// Text color
					if(guildId == currentGuildId && guildMenuState == GuildMenuState.ListGuilds)
						GUI.contentColor = Color.green;
					else
						GUI.contentColor = Color.white;
					
					// Guild name
					GUILayout.BeginHorizontal("box");
					if(GUIHelper.Button(guild.name, guildButtonStyle)) {
						Sounds.instance.PlayButtonClick();
						currentGuildId = guildId;
						statusMessage = "";
						scrollPosition = Vector2.zero;
						Lobby.RPC("GuildMembersRequest", Lobby.lobby, guildId);
						ChangeState(GuildMenuState.ListGuilds);
					}
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndScrollView();
			
			// Create guild button
			GUI.contentColor = Color.white;
			if(guildMenuState != GuildMenuState.CreateGuild) {
				GUI.enabled = (guildList.idList.Count <= 5);
				if(GUIHelper.Button("Create a guild")) {
					ChangeState(GuildMenuState.CreateGuild);
				}
				GUI.enabled = true;
			}
		}
	}
	
	// Current guild
	void DrawCurrentGuildMembers() {
		// Member list
		GUILayout.BeginVertical("box");
		
		if(currentGuildId != "") {
			// Guild name header
			GUILayout.BeginHorizontal();
			Guild guild = null;
			if(GameDB.guildIdToGuild.ContainsKey(currentGuildId)) {
				guild = GameDB.guildIdToGuild[currentGuildId];
				GUILayout.Label(guild.ToString(), guildNameHeaderStyle);
			}
			
			GUILayout.FlexibleSpace();
			
			// Status message
			GUI.color = GUIColor.StatusMessage;
			GUILayout.Label(statusMessage);
			GUI.color = Color.white;
			GUILayout.Space(8);
			
			// Invite
			if(Guild.CanInvite(currentGuildId, PlayerAccount.mine.accountId)) {
				DrawGuildInvite();
			}
			GUILayout.EndHorizontal();
			
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			if(GameDB.guildIdToGuildMembers.ContainsKey(currentGuildId)) {
				var memberList = GameDB.guildIdToGuildMembers[currentGuildId];
				
				foreach(var member in memberList) {
					GUILayout.BeginHorizontal("box");
					
					// Guild member name
					if(member.name != null && member.name != "") {
						//GUILayout.Label(member.name, guildMemberNameStyle);
						DrawPlayerName(member.name, new GUIContent(member.name), guildMemberNameStyle);
					}
					
					GUILayout.FlexibleSpace();
					
					// Kick members
					if(Guild.CanKick(currentGuildId, PlayerAccount.mine.accountId) && member.accountId != PlayerAccount.mine.accountId && GUIHelper.Button("Kick")) {
						Sounds.instance.PlayButtonClick();
						
						// Save correct references for the lambda
						var tmpMemberName = member.name;
						var tmpGuildId = currentGuildId;
						var tmpAccountId = member.accountId;
						
						new Confirm(
							"Do you really want to kick '" + tmpMemberName + "'?",
							() => {
								Lobby.RPC("GuildKickRequest", Lobby.lobby, tmpGuildId, tmpAccountId);
							},
							null
						);
					}
					
					// Leave
					if(member.accountId == PlayerAccount.mine.accountId && member.rank != (byte)GuildMember.Rank.Leader && GUIHelper.Button("Leave")) {
						Sounds.instance.PlayButtonClick();
						
						// Save correct references for the lambda
						var tmpGuildName = guild.ToString();
						var tmpGuildId = currentGuildId;
						var tmpAccountId = member.accountId;
						
						new Confirm(
							"Do you really want to leave '" + tmpGuildName + "'?",
							() => {
								Lobby.RPC("GuildLeaveRequest", Lobby.lobby, tmpGuildId, tmpAccountId);
							},
							null
						);
					}
					
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
			
			// Represent
			DrawGuildFooter();
		} else {
			GUILayout.FlexibleSpace();
		}
		
		GUILayout.EndVertical();
	}
	
	// Guild invite interface
	void DrawGuildInvite() {
		string textFieldName = "InvitedPlayerName";
		
		using(new GUIHorizontal()) {
			//GUI.SetNextControlName(textFieldName);
			//playerToInvite = GUILayout.TextField(playerToInvite, GUILayout.Width(120));
			
			if(GUIHelper.Button("Invite", GUILayout.Width(70)) || (GUI.GetNameOfFocusedControl() == textFieldName && GUIHelper.ReturnPressed())) {
				Sounds.instance.PlayButtonClick();
				
				new TextFieldWindow(
					"Which player would you like to invite?",
					"",
					(text) => {
						Lobby.RPC("GuildInvitationRequest", Lobby.lobby, currentGuildId, text);
					}
				);
			}
		}
	}
	
	// Footer
	void DrawGuildFooter() {
		string representText;
		bool represent = (currentGuildId != guildList.mainGuildId);
		
		if(represent) {
			representText = "Represent";
		} else {
			representText = "Don't represent";
		}
		
		using(new GUIHorizontalCenter()) {
			if(GUIHelper.Button(representText)) {
				Sounds.instance.PlayButtonClick();
				Lobby.RPC("GuildRepresentRequest", Lobby.lobby, currentGuildId, represent);
				
				// Live update in town
				if(Player.main != null)
					Player.main.networkView.RPC("GuildRepresentRequest", uLink.RPCMode.Server, currentGuildId, represent);
			}
			
			if(Guild.CanDisband(currentGuildId, PlayerAccount.mine.accountId) && GUIHelper.Button("Disband")) {
				Sounds.instance.PlayButtonClick();
				var guild = GameDB.guildIdToGuild[currentGuildId];
				
				new Confirm("Do you really want to disband '" + guild.ToString() + "'?",
					() => {
						Lobby.RPC("GuildDisbandRequest", Lobby.lobby, currentGuildId);
					},
					null
				);
			}
		}
	}
	
	// Outstanding guild invitations
	void DrawInvitations() {
		if(invitationsList == null || invitationsList.Count == 0)
			return;
		
		GUILayout.Space(8);
		
		using(new GUIVertical()) {
			foreach(var guildId in invitationsList) {
				if(!GameDB.guildIdToGuild.ContainsKey(guildId))
					continue;
				
				var guild = GameDB.guildIdToGuild[guildId];
				using(new GUIHorizontalCenter()) {
					using(new GUIHorizontal("box")) {
						GUILayout.Label("Invitation to: <b>" + guild + "</b>");
						GUILayout.FlexibleSpace();
						
						// Accept
						if(GUIHelper.Button("Accept")) {
							Sounds.instance.PlayButtonClick();
							Lobby.RPC("GuildInvitationResponse", Lobby.lobby, guildId, true);
						}
						
						// Deny
						if(GUIHelper.Button("Deny")) {
							Sounds.instance.PlayButtonClick();
							Lobby.RPC("GuildInvitationResponse", Lobby.lobby, guildId, false);
						}
					}
				}
			}
		}
	}
	
	// DrawCreateGuild
	void DrawCreateGuild() {
		using(new GUIVertical("box")) {
			int validationErrors = 0;
			var height = GUILayout.Height(24);
			var width = GUILayout.Width(500);
			
			GUILayout.Label("<b>Guild name</b>");
			GUI.SetNextControlName("GuildNameInput");
			guildName = GUILayout.TextField(guildName, GameDB.maxGuildNameLength, width, height);
			guildName = guildName.Capitalize();
			
			// Validate
			if(Validator.guildName.IsMatch(guildName)) {
				GUI.color = GUIColor.Validated;
			} else {
				GUI.color = GUIColor.NotValidated;
				validationErrors += 1;
			}
			GUILayout.Label("Guild name may only contain letters, numbers and spaces and must be starting with a capital letter.", width);
			GUI.color = Color.white;
			
			GUILayout.Space(8);
			
			GUILayout.Label("<b>Guild tag</b>");
			guildTag = GUILayout.TextField(guildTag, GameDB.maxGuildTagLength, GUILayout.Width(60), height);
			
			// Validate
			if(Validator.guildTag.IsMatch(guildTag)) {
				GUI.color = GUIColor.Validated;
			} else {
				GUI.color = GUIColor.NotValidated;
				validationErrors += 1;
			}
			GUILayout.Label("Guild tag may only contain letters and numbers and requires at least 2 characters.", width);
			GUI.color = Color.white;
			
			//GUILayout.Space(8);
			GUILayout.FlexibleSpace();
			
			// Buttons
			//GUIHelper.StartHCenter();
			GUILayout.BeginHorizontal();
			
			// Cancel button
			if(GUIHelper.Button("Cancel", GUILayout.Width(70))) {
				ChangeState(GuildMenuState.ListGuilds);
			}
			
			// Create guild button
			GUI.enabled = (validationErrors == 0);
			if(GUIHelper.Button("Create guild <b>" + guildName + " [" + guildTag + "]</b>")) {
				Sounds.instance.PlayButtonClick();
				Lobby.RPC("GuildCreationRequest", Lobby.lobby, guildName, guildTag);
				statusMessage = "Creating guild...";
			}
			GUI.enabled = true;
			
			// Status message
			GUILayout.Space(8);
			GUI.color = GUIColor.StatusMessage;
			GUILayout.Label(statusMessage);
			GUI.color = Color.white;
			
			//GUIHelper.EndHCenter();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
#region Guild List
	[RPC]
	void ReceiveGuildList(string accountId, string newGuildListJson) {
		var tmpGuildList = Jboy.Json.ReadObject<GuildList>(newGuildListJson);
		var account = PlayerAccount.Get(accountId);
		account.guildList = tmpGuildList;
		
		// Request guild info for each guild ID
		foreach(var guildId in tmpGuildList.idList) {
			if(!GameDB.guildIdToGuild.ContainsKey(guildId)) {
				Lobby.RPC("GuildInfoRequest", Lobby.lobby, guildId);
			}
			
			if(!GameDB.guildIdToGuildMembers.ContainsKey(guildId)) {
				Lobby.RPC("GuildMembersRequest", Lobby.lobby, guildId);
			}
		}
		
		// Did the player get kicked?
		if(account == InGameLobby.instance.displayedAccount && !tmpGuildList.idList.Contains(currentGuildId)) {
			if(tmpGuildList.idList.Count > 0)
				currentGuildId = tmpGuildList.idList[0];
			else
				currentGuildId = "";
		}
		
		Lobby.RPC("GuildInvitationsListRequest", Lobby.lobby);
		
		if(pendingGuildListRequests > 0) {
			pendingGuildListRequests -= 1;
		}
	}
#endregion
	
#region Guild info
	[RPC]
	void ReceiveGuildInfo(string guildId, string guildString) {
		GameDB.guildIdToGuild[guildId] = Jboy.Json.ReadObject<Guild>(guildString);
	}
	
	[RPC]
	void ReceiveGuildInfoError(string guildId) {
		statusMessage = "Error retrieving guild info for guild ID " + guildId + ".";
		LogManager.General.Log(statusMessage);
	}
#endregion
	
#region Guild members
	[RPC]
	void ReceiveGuildMembers(string guildId, GuildMember[] guildMembers, bool dummy) {
		GameDB.guildIdToGuildMembers[guildId] = new List<GuildMember>(guildMembers);
	}
	
	[RPC]
	void ReceiveGuildMembersError() {
		statusMessage = "Error receiving guild member list.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildMemberJoined(string guildId, string playerName) {
		if(GameDB.guildIdToGuild.ContainsKey(guildId)) {
			var guild = GameDB.guildIdToGuild[guildId];
			
			statusMessage = playerName + " has joined " + guild.name;
			LogManager.General.Log(statusMessage);
		}
	}
	
	[RPC]
	void GuildKickError(string guildId, string accountId) {
		statusMessage = "Error: Couldn't kick the player from the guild.";
		LogManager.General.Log(statusMessage);
	}
#endregion

#region Guild invitations
	[RPC]
	void ReceiveGuildInvitationsList(string[] invitations, bool dummy) {
		invitationsList = new List<string>(invitations);
		
		foreach(var guildId in invitationsList) {
			if(!GameDB.guildIdToGuild.ContainsKey(guildId)) {
				Lobby.RPC("GuildInfoRequest", Lobby.lobby, guildId);
			}
		}
	}
	
	[RPC]
	void GuildInvitationSuccess(string playerName) {
		statusMessage = "Guild invitation to player '" + playerName + "' has been sent.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildInvitationAlreadyMember(string playerName) {
		statusMessage = "'" + playerName + "' is already a guild member.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildInvitationAlreadySent(string playerName) {
		statusMessage = "Guild invitation to player '" + playerName + "' has already been sent.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildInvitationPlayerDoesntExistError(string playerName) {
		statusMessage = "Error: Player '" + playerName + "' doesn't exist.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildInvitationError(string playerName) {
		statusMessage = "Error: Could not send guild invitation to player '" + playerName + "'.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildInvitationResponseSuccess(string guildId, bool accepted) {
		LogManager.General.Log("Responded to guild invitation from '" + GameDB.guildIdToGuild[guildId] + "'");
		
		/*if(accepted) {
			Lobby.RPC("GuildListRequest", Lobby.lobby);
		}*/
		
		Lobby.RPC("GuildInvitationsListRequest", Lobby.lobby);
	}
#endregion
	
#region Guild represent
	[RPC]
	void GuildRepresentError(string guildId, bool represent) {
		if(represent)
			statusMessage = "Error representing the guild.";
		else
			statusMessage = "Error not representing the guild.";
		
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildRepresentSuccess(string guildId, bool represent) {
		if(represent) {
			statusMessage = "You are now representing " + GameDB.guildIdToGuild[guildId] + ".";
			guildList.mainGuildId = guildId;
		} else {
			statusMessage = "You stopped representing " + GameDB.guildIdToGuild[guildId] + ".";
			guildList.mainGuildId = "";
		}
		
		LogManager.General.Log(statusMessage);
	}
#endregion

#region Guild creation
	[RPC]
	void GuildCreationSuccess() {
		statusMessage = "Guild has been created.";
		LogManager.General.Log(statusMessage);
		
		guildName = "";
		guildTag = "";
		ChangeState(GuildMenuState.ListGuilds);
	}
	
	[RPC]
	void GuildCreationError() {
		statusMessage = "Unknown error creating the guild.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildNameAlreadyExists() {
		statusMessage = "Guild name has already been taken.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void GuildDisbandSuccess(string guildId) {
		//statusMessage = "Guild has been disbanded.";
		LogManager.General.Log(statusMessage);
	}
#endregion
}