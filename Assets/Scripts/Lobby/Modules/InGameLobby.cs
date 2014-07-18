using uLobby;
using UnityEngine;

[RequireComponent(typeof(LobbyChat))]

public class InGameLobby : LobbyModule<InGameLobby> {
	public int chatHeight = 258;

	public bool chatEnabled;
	
	public GUIStyle nameStyle;
	
	public Texture2D profileIcon;
	public Texture2D buildsIcon;
	public Texture2D teamsIcon;
	public Texture2D guildsIcon;
	public Texture2D historyIcon;
	public Texture2D learnIcon;
	public Texture2D rankingIcon;
	public Texture2D friendsIcon;
	public Texture2D inventoryIcon;
	public Texture2D donationsIcon;
	public Texture2D musicIcon;
	public Texture2D staffIcon;
	public Texture2D settingsIcon;
	public Texture2D logoutIcon;
	
	public GUIContent[] leagues;
	public GUIContent[] divisions;
	public GUIContent[] playerPopupMenuContents;
	
	[HideInInspector]
	public int totalOnlinePlayers;
	
	private GameLobbyState _currentState;
	
	[HideInInspector]
	public PlayerAccount displayedAccount;
	
	// Name change
	private string playerNameRequest = "";
	private string statusMessage = "";
	private string lastPlayerNameChecked = "";
	private float lastPlayerNameCheckedTime;
	private bool nameAvailable;
	private bool playerNameFocused;
	
	private KeyCode currentKeyCode;
	
	private RequestStatus[] accountSetupPageStatus;
	
	public LobbyChat lobbyChat;
	public ProfileGUI profileGUI;
	public BuildsGUI buildsGUI;
	public GuildsGUI guildsGUI;
	public LearnGUI learnGUI;
	public RankingGUI rankingGUI;
	public FriendsGUI friendsGUI;
	public ItemInventoryGUI itemInventoryGUI;
	public DonationsGUI donationsGUI;
	public StaffGUI staffGUI;
	public SettingsGUI settingsGUI;
	public CharacterCustomizationGUI characterCustomizationGUI;
	
	private GameObject audioGameObject;
	private MusicManager musicManager;

	private DrawableLobbyModule _currentLobbyModule;

	// Start
	void Start() {
		// Register codecs for serialization
		GameDB.InitCodecs();
		
		// Listen to lobby events
		Lobby.AddListener(this);
		
		displayedAccount = null;
		accountSetupPageStatus = new RequestStatus[2];
		currentState = GameLobbyState.WaitingForAccountInfo;
		currentLobbyModule = profileGUI;
		
		lobbyChat.currentChannel = "Global";

		// Music objects
		audioGameObject = GameObject.Find("Audio");
		if(audioGameObject != null) {
			musicManager = audioGameObject.GetComponent<MusicManager>();
		}
		
		ArenaGUI.instance.ResetQueueInfo();
	}
	
	// State handling
	public override void Draw() {
		// Key codes
		if(Event.current.type == EventType.KeyUp)
			currentKeyCode = Event.current.keyCode;
		else
			currentKeyCode = KeyCode.None;
		
		// Depending on state
		switch(currentState) {
			case GameLobbyState.WaitingForAccountInfo:	WaitingForAccountInfo();			break;
			case GameLobbyState.CustomizingCharacter:	characterCustomizationGUI.Draw();	break;
			case GameLobbyState.AskingPlayerName:		AskingPlayerName();					break;
			case GameLobbyState.Ready:					DrawReady();						break;
			case GameLobbyState.Game:
			case GameLobbyState.MainMenu:
				if(GameManager.inGame || Login.instance.enableLobby)
					DrawMainMenu();
				break;
		}
	}
	
	void DrawReady() {
		GUIHelper.BeginBox(300, 50);
		GUILayout.Label("Waiting for game server...");
		GUIHelper.EndBox();
	}
	
	// Main menu
	void DrawMainMenu() {
		/*int chatHeight = (int)(GUIArea.height * 0.3f);
		if(chatHeight < 250)
			chatHeight = 250;*/
		
		int chatYPos;
		if(_currentState == GameLobbyState.Game || !chatEnabled) {
			chatYPos = (int)GUIArea.height;
		} else {
			chatYPos = (int)GUIArea.height - chatHeight - 4;
		}
		
		GUIHelper.BeginBox(4, 4, (int)GUIArea.width - 8, chatYPos - 8);
		
		// Header
		DrawHeader();
		
		GUILayout.Space(4);
		
		// Body
		DrawBody();
		
		GUIHelper.EndBox();
		
		// Footer with chat and matchmaking buttons
		if(_currentState != GameLobbyState.Game && chatEnabled) {
			DrawFooter(4, chatYPos, (int)GUIArea.width - 8, chatHeight);
		}
		
		// Navigation for game pads
		GamePadNavigation();
	}
	
	// Header
	void DrawHeader() {
		using(new GUIHorizontal()) {
			// Player name
			using(new GUIHorizontal()) {
				if(!displayedAccount.isMine) {
					if(GUIHelper.Button("Return")) {
						displayedAccount = PlayerAccount.mine;
					}
				}
				GUILayout.Label(displayedAccount.playerName, nameStyle);
			}
			
			GUILayout.FlexibleSpace();
			
			// Menu structure
			using(new GUIHorizontal()) {
				var width = GUILayout.Width(96);
				
				// Profile
				GUI.enabled = profileGUI.enabled;
				MainMenuButtonColor(profileGUI, displayedAccount.stats != null);
				if(GUIHelper.Button(new GUIContent(" Profile", profileIcon), width)) {
					currentLobbyModule = profileGUI;
				}
				
				// Builds
				GUI.enabled = buildsGUI.enabled;
				MainMenuButtonColor(buildsGUI, true);
				if(GUIHelper.Button(new GUIContent(" Builds", buildsIcon), width)) {
					currentLobbyModule = buildsGUI;
				}
				
				// Teams
				GUI.enabled = false;
				MainMenuButtonColor(null, true);
				if(GUIHelper.Button(new GUIContent(" Teams", teamsIcon), width)) {
					currentLobbyModule = null;
				}
				
				// Guilds
				GUI.enabled = guildsGUI.enabled && displayedAccount.isMine;
				MainMenuButtonColor(guildsGUI, guildsGUI.guildList != null && guildsGUI.pendingGuildListRequests == 0);
				if(GUIHelper.Button(new GUIContent(" Guilds", guildsIcon), width)) {
					currentLobbyModule = guildsGUI;
				}
				
				// History
				GUI.enabled = false;
				MainMenuButtonColor(null, true);
				if(GUIHelper.Button(new GUIContent(" History", historyIcon), width)) {
					currentLobbyModule = null;
				}
				
				// Ranking
				GUI.enabled = rankingGUI.enabled;
				MainMenuButtonColor(rankingGUI, GameDB.rankingLists != null && rankingGUI.pendingRankingListRequests == 0);
				if(GUIHelper.Button(new GUIContent(" Ranking", rankingIcon), width)) {
					currentLobbyModule = rankingGUI;
				}
			}
			
			GUILayout.FlexibleSpace();
			
			// Mini buttons
			using(new GUIHorizontal()) {
				// Donations
				GUI.enabled = donationsGUI.enabled && displayedAccount.isMine;
				MainMenuButtonColor(donationsGUI, donationsGUI.pendingCrystalBalanceRequests == 0);
				if(GUIHelper.Button(new GUIContent(" " + displayedAccount.crystals.ToString(), donationsIcon, "Donations"))) {
					currentLobbyModule = donationsGUI;
				}

				// Friends
				GUI.enabled = friendsGUI.enabled && displayedAccount.isMine;
				MainMenuButtonColor(friendsGUI, displayedAccount.friends != null);
				
				int onlineFriends = 0;
				if(displayedAccount.friends != null)
					onlineFriends = displayedAccount.friends.onlineCount;
				
				if(GUIHelper.Button(new GUIContent(" " + onlineFriends, friendsIcon, "Friends"))) {
					currentLobbyModule = friendsGUI;
				}

				// Inventory
				GUI.enabled = itemInventoryGUI.enabled && displayedAccount.isMine;
				MainMenuButtonColor(itemInventoryGUI, true);
				if(GUIHelper.Button(new GUIContent("", inventoryIcon, "Inventory"))) {
					currentLobbyModule = itemInventoryGUI;
				}
				
				// Music
				if(musicManager != null) {
					GUI.enabled = learnGUI.enabled;
					MainMenuButtonColor(musicManager, true);
					if(GUIHelper.Button(new GUIContent("", musicIcon, "Music"))) {
						currentLobbyModule = musicManager;
					}
				}
				
				// Staff
				if(PlayerAccount.mine.accessLevel >= AccessLevel.VIP) {
					GUI.enabled = staffGUI.enabled && displayedAccount.isMine;
					MainMenuButtonColor(staffGUI, staffGUI.pendingStaffRequests == 0);
					if(GUIHelper.Button(new GUIContent("", staffIcon, "Staff"))) {
						currentLobbyModule = staffGUI;
					}
				}
				
				// Learn
				GUI.enabled = learnGUI.enabled;
				MainMenuButtonColor(learnGUI, true);
				if(GUIHelper.Button(new GUIContent("", learnIcon, "Help"))) {
					currentLobbyModule = learnGUI;
				}
				
				// Settings
				/*GUI.enabled = settingsGUI.enabled;
				MainMenuButtonColor(MainMenuState.Settings, true);
				if(GUIHelper.Button(new GUIContent("", settingsIcon, "Settings"))) {
					this.currentMainMenuState = MainMenuState.Settings;
				}*/
				
				/*GUILayout.Space(8);
				
				// Log out
				GUI.enabled = true;
				GUI.backgroundColor = Color.white;
				if(GUIHelper.Button(new GUIContent("", logoutIcon, "Log out"))) {
					MenuSounds.instance.PlayButtonClick();
					
					Lobby.RPC("LobbyAccountLogOut", Lobby.lobby);
				}*/
			}
		}
		
		// Reset
		GUI.backgroundColor = Color.white;
		GUI.enabled = true;
	}
	
	// Body
	void DrawBody() {
		if(currentLobbyModule != null)
			currentLobbyModule.Draw();
		else
			GUILayout.Label("Coming soon.");
	}
	
	// Footer
	void DrawFooter(int x, int y, int width, int height) {
		GUIHelper.BeginBox(x, y, width, height);
		using(new GUIHorizontal(GUILayout.MaxWidth(GUIArea.width))) {
			ArenaGUI.instance.DrawMatchmakingQueues();
			lobbyChat.Draw();
		}
		GUIHelper.EndBox();
	}
	
	// Toolbar button
	void MainMenuButtonColor(DrawableLobbyModule module, bool requestSuccess) {
		if(currentLobbyModule == module) {
			if(requestSuccess) {
				GUI.backgroundColor = GUIColor.MenuItemActive;
			} else {
				GUI.backgroundColor = GUIColor.MenuItemLoading;
			}
		} else {
			GUI.backgroundColor = GUIColor.MenuItemInactive;
		}
	}
	
	// Creates a popup menu
	public void CreatePlayerPopupMenu(string playerName) {
		new PopupMenu(
			playerPopupMenuContents,
			new PopupMenu.CallBack[] {
				() => { Lobby.RPC("ViewProfile", Lobby.lobby, playerName); },
				() => { Lobby.RPC("InviteToParty", Lobby.lobby, playerName); },
				() => { Lobby.RPC("ClientChat", Lobby.lobby, "Map", "//ginvite " + playerName); },
				() => {
					new FriendAddWindow(
						"Add a friend:",
						playerName
					);
				},
				() => { lobbyChat.AddEntry("Coming soon!"); },
				() => { lobbyChat.AddEntry("Coming soon!"); },
				() => { lobbyChat.AddEntry("Coming soon!"); }
			}
		);
	}
	
	// While waiting for account information
	void WaitingForAccountInfo() {
		GUIHelper.BeginBox(400, 50);
		GUILayout.Label("Loading account information...");
		GUIHelper.EndBox();
	}
	
	// Send player name change request
	void SendPlayerNameChangeRequest() {
		statusMessage = "Changing player name to '" + playerNameRequest + "'...";
		Lobby.RPC("PlayerNameChange", Lobby.lobby, playerNameRequest);
	}
	
	// Ask for player name
	void AskingPlayerName() {
		GUIHelper.BeginBox(400, 250);
		GUILayout.Label("Please enter your player name.");
		
		GUI.SetNextControlName("PlayerNameInput");
		playerNameRequest = GUIHelper.PlayerNameField(playerNameRequest);
		
		if(!playerNameFocused) {
			GUIHelper.Focus("PlayerNameInput");
			playerNameFocused = true;
		}
		
		// Validator
		bool validName = Validator.playerName.IsMatch(playerNameRequest);
		
		// We will not allow use of Return key when unicode characters are used
		// because Return is used in the IME input of eastern languages.
		bool imeInputUsed = playerNameRequest.ContainsUnicodeCharacter();
		if(currentKeyCode == KeyCode.Return && !imeInputUsed) {
			// We use keyboardControl because GetNameOfFocusedControl() is bugged in Unity
			if(GUIUtility.keyboardControl != 0) {
				if(validName) {
					new Confirm(
						_("Are you sure you want to change your name to <b>{0}</b>?", playerNameRequest),
						SendPlayerNameChangeRequest
					);
				}
			} else {
				GUIHelper.Focus("PlayerNameInput");
			}
		}
		
		// Check availability
		if(validName && playerNameRequest != lastPlayerNameChecked && Time.time - lastPlayerNameCheckedTime >= 0.5f) {
			Lobby.RPC("PlayerNameExists", Lobby.lobby, playerNameRequest);
			lastPlayerNameCheckedTime = Time.time;
			lastPlayerNameChecked = playerNameRequest;
			nameAvailable = false;
		}
		
		GUILayout.Label("You can only set it once and it will be visible for everyone, so think carefully about the name you choose.");
		GUILayout.FlexibleSpace();
		
		if(validName) {
			GUI.contentColor = GUIColor.Validated;
		} else {
			GUI.contentColor = GUIColor.NotValidated;
		}
		
		GUILayout.Label("Player name must contain letters and spaces only and each word needs to start with an uppercase letter.");
		GUILayout.FlexibleSpace();
		
		using(new GUIHorizontal()) {
			GUI.contentColor = GUIColor.StatusMessage;
			GUILayout.Label(statusMessage);
			
			GUILayout.FlexibleSpace();
			
			GUI.contentColor = Color.white;
			GUI.enabled = validName && nameAvailable;
			if(GUIHelper.Button("Accept")) {
				SendPlayerNameChangeRequest();
			}
			GUI.enabled = true;
		}
		
		GUIHelper.EndBox();
	}
	
	// Checks if the initial data is available after login
	public void UpdateAccountInfo() {
		if(GameManager.inGame || (GameManager.nextState != State.Lobby && GameManager.currentState != State.Disconnected))
			return;
		
		// Check if we have all request responses
		int pageNeeded = -1;
		for(int i = 0; i < accountSetupPageStatus.Length; i++) {
			var status = accountSetupPageStatus[i];
			if(status == RequestStatus.Unknown)
				return;
			
			if(pageNeeded == -1 && status == RequestStatus.DoesntExist)
				pageNeeded = i;
		}
		
		// Show the first page where the user still needs to enter information
		if(pageNeeded == -1) {
			if(!Login.instance.enableLobby) {
				Lobby.RPC("Ready", Lobby.lobby);
				this.currentState = GameLobbyState.Ready;
			} else {
				this.currentState = GameLobbyState.MainMenu;
			}
		} else {
			switch((AccountSetupPage)pageNeeded) {
				case AccountSetupPage.CharacterCustomization:
					characterCustomizationGUI.StartCustomization();
					this.currentState = GameLobbyState.CustomizingCharacter;
					break;
				case AccountSetupPage.Name:
					this.currentState = GameLobbyState.AskingPlayerName;
					break;
			}
		}
	}
	
	// Navigation for game pads
	void GamePadNavigation() {
		// Next / previous page
		/*if(currentKeyCode == KeyCode.RightArrow && GUIUtility.keyboardControl == 0) {
			currentMainMenuState = IncrementPage(
				currentMainMenuState,
				MainMenuState.Profile,
				MainMenuState.Settings,
				1
			);
		}
		
		if(currentKeyCode == KeyCode.LeftArrow && GUIUtility.keyboardControl == 0) {
			currentMainMenuState = IncrementPage(
				currentMainMenuState,
				MainMenuState.Profile,
				MainMenuState.Settings,
				-1
			);
		}*/
	}

	// ResetAccountInfo
	public void ResetAccountInfo() {
		PlayerAccount.mine = null;
		displayedAccount = null;
		
		for(int i = 0; i < accountSetupPageStatus.Length; i++) {
			accountSetupPageStatus[i] = RequestStatus.Unknown;
		}
	}
	
	// Increment page
	T IncrementPage<T>(T currentPage, T minPage, T maxPage, int increment) where T : struct {
		int curPage = (int)(object)currentPage;
		
		if(curPage + increment > (int)(object)maxPage)
			return minPage;
		else if(curPage + increment < (int)(object)minPage)
			return maxPage;
		else
			return (T)(object)(curPage + increment);
	}
	
	// Wrapper for IPEndPoint
	public void ConnectToGameServerByIPEndPoint(System.Net.IPEndPoint ipEndPoint) {
		this.ConnectToGameServer(ipEndPoint.Address.ToString(), ipEndPoint.Port); 
	}
	
#region Properties
	// Current state
	public GameLobbyState currentState {
		get {
			return _currentState;
		}

		set {
			statusMessage = "";
			_currentState = value;
		}
	}

	// Current lobby module
	public DrawableLobbyModule currentLobbyModule {
		get { return _currentLobbyModule; }
		set {
			statusMessage = "";

			ExecuteLater(() => {
				_currentLobbyModule = value;
				if(_currentLobbyModule != null)
					currentLobbyModule.OnClick();
			});

			Login.instance.clearFlag = true;
		}
	}
#endregion
	
#region RPCs
	[RPC]
	public void ConnectToGameServer(string ip, int port) {
		// Reset state
		ArenaGUI.instance.ResetQueueInfo();
		
		Login clientGUI = Login.instance;
		clientGUI.ChangeState(State.Game);
		this.currentState = GameLobbyState.Game;
		clientGUI.clearFlag = true;
		
		// Change frame rate
		Application.runInBackground = true;
		Application.targetFrameRate = -1;
		
		// Load the game scene
		LoadingScreen.instance.SecureLoadLevel(
			"Client",
			() => {
				LoadingScreen.instance.statusMessage = "Connecting to game server...";
				
				// TODO: Remove this later, this is a special case for myself, testing the game server
				if(clientGUI.lobbyHost == "127.0.0.1" && GameDB.IsTestAccount(clientGUI.accountEmail)) {
					LogManager.General.Log("Special account: Using localhost (127.0.0.1) as game server IP");
					ip = "127.0.0.1";
				}
				
				// Connect to the game server instance we were assigned to
				object[] args = {PlayerAccount.mine.accountId, PlayerAccount.mine.accountId};
				
				LogManager.General.Log("Connecting to game server: " + ip + ":" + port + " (Account ID: " + PlayerAccount.mine.accountId + ")");
				uLink.Network.Connect(ip, port, "", args);
				
				lobbyChat.currentChannel = "Map";
			}
		);
	}
	
	[RPC]
	void CustomizeCharacter(string accountId) {
		LogManager.General.Log("Character customization not available yet!");
		
		PlayerAccount.Get(accountId).custom = new CharacterCustomization();
		this.accountSetupPageStatus[(int)AccountSetupPage.CharacterCustomization] = RequestStatus.DoesntExist;
		UpdateAccountInfo();
	}
	
	[RPC]
	void AskPlayerName() {
		LogManager.General.Log("Player does not have a name yet, going to ask.");
		
		playerNameFocused = false;
		this.accountSetupPageStatus[(int)AccountSetupPage.Name] = RequestStatus.DoesntExist;
		UpdateAccountInfo();
	}
	
	[RPC]
	void PlayerNameAlreadyExists(string name) {
		statusMessage = _("The name '{0}' is already being used by another player.", name);
	}
	
	[RPC]
	void PlayerNameFree(string name) {
		if(!nameAvailable) {
			statusMessage = _("The name '{0}' is free.", name);
			
			if(name == playerNameRequest) {
				nameAvailable = true;
			}
		}
	}
	
	[RPC]
	void PlayerNameChangeError() {
		statusMessage = "Error setting new player name.";
		LogManager.General.Log(statusMessage);
	}
	
	[RPC]
	void ViewProfile(string accountId) {
		displayedAccount = PlayerAccount.Get(accountId);
		
		if(GameManager.inGame) {
			// Stop talking with NPC
			Player.main.talkingWithNPC = null;
			
			// Activate main menu
			//ToggleMouseLook.instance.DisableMouseLook();
			
			// Show lobby
			MainMenu.instance.nextState = InGameMenuState.Lobby;
			MainMenu.instance.currentState = MainMenu.instance.nextState;
			
			// Jump to profile
			currentLobbyModule = profileGUI;
		}
	}
	
	[RPC]
	void ViewProfileError(string accountId) {
		lobbyChat.AddEntry("Error retrieving information for this player.");
	}

	[RPC]
	void ReceivePlayerName(string accountId, string playerName) {
		LogManager.General.Log("Lobby: Received player info. Account ID: " + accountId + ", Name: " + playerName);
		
		var acc = PlayerAccount.Get(accountId);
		acc.playerName = playerName;
		
		var accPage = (int)AccountSetupPage.Name;
		
		if(acc.isMine && accountSetupPageStatus[accPage] != RequestStatus.Exists) {
			accountSetupPageStatus[accPage] = RequestStatus.Exists;
			UpdateAccountInfo();
		}
	}

	[RPC]
	void ReceivePlayerEmail(string accountId, string playerEmail) {
		LogManager.General.Log("Lobby: Received player info. Account ID: " + accountId + ", Email: " + playerEmail);

		var acc = PlayerAccount.Get(accountId);
		acc.email = playerEmail;
	}
	
	[RPC]
	void ReceiveCharacterCustomization(string accountId, CharacterCustomization custom) {
		LogManager.General.Log(string.Format("Lobby: Received character customization for account ID '{0}'!", accountId));
		
		var acc = PlayerAccount.Get(accountId);
		acc.custom = custom;
		
		var accPage = (int)AccountSetupPage.CharacterCustomization;
		
		if(acc.isMine && accountSetupPageStatus[accPage] != RequestStatus.Exists) {
			characterCustomizationGUI.UpdateCustomization();
			characterCustomizationGUI.EndCustomization();
			
			accountSetupPageStatus[accPage] = RequestStatus.Exists;
			UpdateAccountInfo();
		}
	}
	
	[RPC]
	void ReceiveAccessLevel(string accountId, byte newAccessLevel) {
		var accLevel = (AccessLevel)newAccessLevel;
		LogManager.General.Log("Lobby: Received access level: " + accLevel.ToString() + "!");
		
		PlayerAccount.Get(accountId).accessLevel = accLevel;
	}
	
	[RPC]
	void ReceiveOnlineStatus(string accountId, OnlineStatus status) {
		LogManager.General.Log("Lobby: Received online status: " + status.ToString() + "!");
		
		PlayerAccount.Get(accountId).onlineStatus = status;
	}
#endregion
}
