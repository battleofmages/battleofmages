using UnityEngine;
using uLobby;
using System.Collections;

public class Login : LobbyModule<Login> {
	public static string changeLog = null;
	
	public string lobbyHost;
	public string editorLobbyHost;
	public int lobbyPort;
	public int lobbyFrameRate;
	
	public bool lowerFPSWhenIdle;
	public int lobbyFrameRateIdle;
	
	public bool enableLobby;
	
	public int guiDepth = 0;
	
	public string changeLogURL;
	public Rect changeLogRect;
	
	private Vector2 changeLogScroll = Vector2.zero;
	
	public bool retrieveLobbyIP = false;
	public string lobbyIpURL;
	
	public bool editorAutoLogin;
	public string editorAccountName;
	public string editorPassword;

	public Vector2 loginGUIDimensions;
	public Vector2 registerGUIDimensions;
	
	public GUIStyle tooltipStyle;
	public Vector2 tooltipOffset;
	
	public Texture2D closeIcon;
	
	public ProgressBarStyle progressBarStyle;
	
	[HideInInspector]
	public PopupMenu popupMenu;
	
	[HideInInspector]
	public PopupWindow popupWindow;
	
	[HideInInspector]
	public PopupWindow nextPopupWindow;
	
	[HideInInspector]
	public string accountEmail = "";
	
	[HideInInspector]
	public string accountPassword = "";
	
	[HideInInspector]
	public string accountPasswordEncrypted = null;
	
	private string statusMessage = "";
	private bool accountNameFocused = false;
	private bool accountNameLoaded = false;
	private bool accountPasswordFocused = false;
	private const string encryptedPasswordString = "--------------------";
	
	private int loginRequests;
	private int loginRequestResponses;
	private int clientVersionNumber;
	private int serverVersionNumber;
	
	private InGameLobby gameLobby;
	private GameObject audioGameObject;
	private MusicManager musicManager;
	private GUILayoutOption textFieldHeight;
	
	public static Account myAccount;
	
	private string inactiveEmail;
	private bool accountNotActivated;
	
	[HideInInspector]
	public bool clearFlag;
	
	private Intro intro;
	private bool introEnabled;
	
	// Setup
	void Start() {
		LogManager.System.GenerateReport();
		
		if(Application.genuineCheckAvailable && !Application.genuine) {
			LogManager.General.LogError("Client files have been modified, quitting.");
			Application.Quit();
			return;
		}
		
		SetupApplicationForLobby();
		gameLobby = GetComponent<InGameLobby>();
		clientVersionNumber = GetComponent<Version>().versionNumber;
		intro = GetComponent<Intro>();
		introEnabled = intro.enabled;
		textFieldHeight = GUILayout.Height(24);
		popupMenu = null;
		accountNotActivated = false;
		
		// Music objects
		audioGameObject = GameObject.Find("Audio");
		if(audioGameObject != null) {
			musicManager = audioGameObject.GetComponent<MusicManager>();
		}
		
		// FOR TESTING ONLY
#if UNITY_EDITOR
		if(!Application.CanStreamedLevelBeLoaded("Client")) {
			editorAutoLogin = false;
			LogManager.General.LogError("<color=red>YOU FORGOT TO ADD THE CLIENT SCENE IN THE BUILD SETTINGS</color>");
		}
		
		if(editorAutoLogin) {
			intro.enabled = false;
			introEnabled = false;
		}
		
		retrieveLobbyIP = false;
		lobbyHost = editorLobbyHost;
#endif
		
		// Retrieve lobby IP
		if(retrieveLobbyIP) {
			WWW ipRequest = new WWW(lobbyIpURL);
			StartCoroutine(WaitForLobbyIP(ipRequest));
		}
		
		// Retrieve changelog
		WWW changeLogRequest = new WWW(changeLogURL);
		StartCoroutine(DownloadChangeLog(changeLogRequest));
		
		// Public key
		Lobby.publicKey = new uLobby.PublicKey(
			"td076m4fBadO7bRuEkoOaeaQT+TTqMVEWOEXbUBRXZwf1uR0KE8A/BbOWNripW1eZinvsC+skgVT/G8mrhYTWVl0TrUuyOV6rpmgl5PnoeLneQDEfrGwFUR4k4ijDcSlNpUnfL3bBbUaI5XjPtXD+2Za2dRXT3GDMrePM/QO8xE=",
			"EQ=="
		);
		
		Lobby.AddListener(this);
		
		// Add this class as a listener to different account events.
		AccountManager.OnAccountLoggedIn += OnAccountLoggedIn;
		AccountManager.OnAccountLoggedOut += OnAccountLoggedOut;
		AccountManager.OnAccountRegistered += OnAccountRegistered;
		AccountManager.OnLogInFailed += OnLogInFailed;
		AccountManager.OnRegisterFailed += OnRegisterFailed;
		
		// Connect to lobby
		if(!retrieveLobbyIP)
			ConnectToLobby();
	}
	
	// OnGUI
	void OnGUI() {
		Draw();
	}
	
	// Draw
	public override void Draw() {
		// Top layer
		GUI.depth = 0;

		// Set GUI skin
		GUI.skin = Config.instance.guiSkin;
		
		GUIArea.width = Screen.width;
		GUIArea.height = Screen.height;
		
		if(introEnabled)
			return;
		
		// Always draw the invisible focus control
		GUIHelper.UnityFocusFix();
		
		// Clear focus
		ClearFocus();
		
		// Exit button and version number
		DrawExitButtonAndVersion();
		
		// Clearing popup menu
		bool popupClear = false;
		if(
			popupMenu != null && 
			(
				(Event.current.isMouse && Event.current.type == EventType.MouseUp && Event.current.button == 0) || 
				(Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
			)
		) {
			popupClear = true;
		}
		
		// General keys
		UpdateGeneralKeys();
		
		// State
		switch(GameManager.currentState) {
			case State.ConnectingToLobby:	ConnectingToLobbyGUI(); break;
			case State.Disconnected:		DisconnectedGUI(); 		break;
			case State.Update:				PleaseUpdateGUI();		break;
			case State.LogIn:				LoginGUI(); 			break;
			case State.Register:			RegisterGUI(); 			break;
			case State.License:				LicenseGUI();			break;
			case State.Lobby:				LobbyGUI(); 			break;
			case State.Game:
				/*if(Event.current.isKey && Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t' && !gameLobby.lobbyChat.chatInputFocused) {
					if(GameManager.serverType != ServerType.Town || Player.main == null || !Player.main.talkingWithNPC) {
						GUIHelper.Focus(GUI.GetNameOfFocusedControl());
						Event.current.Use();
					}
				}*/
				
				if(MainMenu.instance != null && (MainMenu.instance.currentState == InGameMenuState.Lobby)) {
					using(new GUIArea(Screen.width * 0.95f, Screen.height * 0.95f)) {
						LobbyGUI();
					}
				}
				
				// On 1366 x 768 laptop screen you should use
				// 0.85f width for profile at least, 0.91f for artifacts
				// 0.75f height max
				if(Player.main != null && Player.main.talkingWithNPC) {
					using(new GUIArea(Screen.width * 0.91f, Screen.height * 0.75f)) {
						Player.main.talkingWithNPC.Draw();
					}
				}
				break;
		}
		
		// Changelog
		DrawChangeLog();
		
		// Popup menu
		if(popupMenu != null) {
			popupMenu.Draw();
			
			if(popupClear && popupMenu != null)
				popupMenu.Close();
		}
		
		// Tooltip
		DrawTooltip();
		
		// Popup window
		if(popupWindow != null) {
			// Dim background
			DimBackground();
			
			using(new GUIArea(new Rect(GUIArea.width * 0.25f, GUIArea.height * 0.25f, GUIArea.width * 0.5f, GUIArea.height * 0.5f))) {
				using(new GUIVerticalCenter()) {
					using(new GUIHorizontalCenter()) {
						popupWindow.DrawAll();
					}
				}
			}
		}
		
		// Disable game input when GUI is active
		if(GUIUtility.hotControl != 0 || GUIUtility.keyboardControl != 0) {
			InputManager.ignoreInput = true;
			InputManager.instance.Clear();
		} else {
			InputManager.ignoreInput = false;
		}
		
		// Debug
		if(Debugger.instance.debugGUI && Event.current.type == EventType.Layout) {
			Debugger.Label("FocusedControl: " + GUI.GetNameOfFocusedControl());
			Debugger.Label("HotControl: " + GUIUtility.hotControl.ToString());
			Debugger.Label("KeyboardControl: " + GUIUtility.keyboardControl.ToString());
		}
		
		// Debugger
		Debugger.instance.Draw();
	}
	
	// Dim background
	void DimBackground() {
		// TODO: Make a filled texture out of it
		GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
	}
	
	// Draw tooltip
	void DrawTooltip() {
		string tooltip = GUI.tooltip;
		
		if(string.IsNullOrEmpty(tooltip))
			return;
		
		GUI.color = Color.white;
		GUI.backgroundColor = Color.white;
		GUI.contentColor = Color.white;
		
		Vector2 tooltipSize = GUI.skin.label.CalcSize(new GUIContent(tooltip));
		tooltipSize.x += 5;
		tooltipSize.y += 4;
		
		Vector2 mousePos = InputManager.GetMousePosition();
		Rect tooltipRect = new Rect(mousePos.x + tooltipOffset.x, mousePos.y + tooltipOffset.y, tooltipSize.x + 3, tooltipSize.y + 3);
		
		float offset;
		
		if(tooltipRect.x + tooltipRect.width > GUIArea.width) {
			offset = (tooltipRect.x + tooltipRect.width) - GUIArea.width;
			tooltipRect.x -= offset;
		}
		
		if(tooltipRect.y + tooltipRect.height > GUIArea.height) {
			offset = (tooltipRect.y + tooltipRect.height) - GUIArea.height;
			tooltipRect.y -= offset;
		}
		
		// 3 times because GUI.Box sucks >.>
		GUI.Box(tooltipRect, "");
		GUI.Box(tooltipRect, "");
		GUI.Box(tooltipRect, "");
		
		GUI.color = Color.white;
		GUI.Label(tooltipRect, tooltip, tooltipStyle);
	}
	
	// Update
	public new void Update() {
		base.Update();
		
		popupWindow = nextPopupWindow;
		
		if(popupWindow != null)
			popupWindow.Update();
		
		if(introEnabled == true && intro.enabled == false) {
			accountNameFocused = false;
			accountPasswordFocused = false;
			introEnabled = intro.enabled;
		}
		
		switch(Lobby.connectionStatus) {
			case LobbyConnectionStatus.Connecting: 		GameManager.currentState = State.ConnectingToLobby;	break;
			case LobbyConnectionStatus.Disconnected:	GameManager.currentState = State.Disconnected; 		break;
		}
	}
	
	// Update general keys
	void UpdateGeneralKeys() {
		if(Event.current.type != EventType.KeyUp)
			return;
		
		switch(Event.current.keyCode) {
			case KeyCode.Escape:
				//popupMenu = null;
				if(popupWindow != null)
					popupWindow.Close();
				
				if(!introEnabled && (accountNameFocused || accountPasswordFocused)) {
					GUIHelper.ClearAllFocus();
				}
				break;
				
			// TODO: This doesn't work with KeyDown...but why?
			case KeyCode.A:
				if(Event.current.modifiers == EventModifiers.Control && GUIUtility.keyboardControl != 0) {
					TextEditor t = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
					t.SelectAll();
				}
				break;
		}
	}
	
	// Change state
	public void ChangeState(State newState) {
		clearFlag = true;
		GameManager.currentState = newState;
		statusMessage = "";
	}
	
	// Also used for reconnecting
	void ConnectToLobby() {
		if(Lobby.connectionStatus == LobbyConnectionStatus.Disconnected) {
			// Reset pending request count because we were disconnected
			if(gameLobby.donationsGUI != null)
				gameLobby.donationsGUI.pendingCrystalBalanceRequests = 0;
			
			if(gameLobby.guildsGUI != null)
				gameLobby.guildsGUI.pendingGuildListRequests = 0;
			
			if(gameLobby.rankingGUI != null)
				gameLobby.rankingGUI.pendingRankingListRequests = 0;
			
			//gameLobby.ResetQueueInfo();
			
			// Connect
			Lobby.ConnectAsClient(lobbyHost, lobbyPort);
		}
	}
	
	// DO NOT USE THIS FUNCTION DIRECTLY!
	// Use the scoreboard function instead.
	// SecureLoadLevel is called before calling this function.
	public void ReturnToLobbyFromGame() {
		ChangeState(State.Lobby);
		gameLobby.currentState = GameLobbyState.MainMenu;
		
		// If it was disabled before by accident...
		gameLobby.lobbyChat.chatInputEnabled = true;
		gameLobby.lobbyChat.currentChannel = "Global";
		
		// Make it clear focus on next OnGUI call
		clearFlag = true;
		
		// Disable character
		//CharacterCustomizationGUI.instance.EndCustomization();
		
		if(audioGameObject != null) {
			audioGameObject.transform.parent = null;
			audioGameObject.transform.position = Cache.vector3Zero;
			musicManager.PlayCategory("Lobby");
		}
		
		// Frame rate
		SetupApplicationForLobby();
		
		// Fade out loading screen
		LoadingScreen.instance.Disable();
	}
	
	// Wait for lobby IP
	IEnumerator WaitForLobbyIP(WWW ipRequest) {
		yield return ipRequest;
		
		if(ipRequest.error == null) {
			string ipAndPort = ipRequest.text;
			string[] parts = ipAndPort.Split(':');
			lobbyHost = parts[0];
			lobbyPort = int.Parse(parts[1]);
			
			ConnectToLobby();
		}
	}
	
	// Download changelog
	IEnumerator DownloadChangeLog(WWW changeLogRequest) {
		yield return changeLogRequest;
		
		if(changeLogRequest.error == null) {
			changeLog = changeLogRequest.text;
		}
	}
	
	// Clear focus
	void ClearFocus() {
		if(!clearFlag)
			return;
		
		GUIHelper.ClearAllFocus();
		
		// Clear pressed keys
		if(Event.current != null && Event.current.isKey) {
			Event.current.keyCode = KeyCode.None;
			Event.current.Use();
			Event.current = null;
		}
		
		// Show mouse cursor
		Screen.lockCursor = false;
		Screen.showCursor = true;
		
		clearFlag = false;
	}
	
	// Sets application settings for lobby
	void SetupApplicationForLobby() {
		//Application.runInBackground = false;
		Application.targetFrameRate = lobbyFrameRate;
	}
	
#region GUIs
	public void LoginGUI() {
		GUIHelper.BeginBox(loginGUIDimensions.x, loginGUIDimensions.y);

		GUILayout.Label("Enter your account name and password to log in.");
		GUILayout.Space(10);
		GUILayout.FlexibleSpace();
		
		// Disable GUI while trying to log in
		GUI.enabled = (loginRequests == loginRequestResponses);
		
		GUILayout.Label("<b>E-Mail</b>");
		GUI.SetNextControlName("AccountEmail");
		accountEmail = GUILayout.TextField(accountEmail, GameDB.maxEmailLength, textFieldHeight).Trim();
		
		// Focus
		if(!accountNameFocused && !accountNameLoaded) {
			GUIHelper.Focus("AccountEmail");
			accountNameFocused = true;
		}

		GUILayout.Label("<b>Password</b>");
		GUI.SetNextControlName("AccountPassword");
		var newAccountPassword = GUILayout.PasswordField(accountPassword, '*', textFieldHeight);
		
		// User modified password
		if(accountPassword != newAccountPassword) {
			accountPasswordEncrypted = null;
			accountPassword = newAccountPassword;
		}
		
		// Focus
		if(!accountPasswordFocused && accountNameLoaded) {
			if(string.IsNullOrEmpty(accountPasswordEncrypted))
				GUIHelper.Focus("AccountPassword");
			accountPasswordFocused = true;
		}
		
		GUILayout.Space(5);
		GUILayout.FlexibleSpace();
		
		// Login button
		using(new GUIHorizontal()) {
			GUI.enabled = (
				GameDB.IsTestAccount(accountEmail) || 
				(
					loginRequests == loginRequestResponses &&
					Validator.email.IsMatch(accountEmail) //&&
					//Validator.password.IsMatch(accountPassword)
				)
			);
			
			if(GUIHelper.Button("Log in", GUILayout.Width(80)) || (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)) {
				Sounds.instance.PlayButtonClick();
				
				// Log in to an account. We will get a response in the form of either the OnAccountRegistered callback, or
				// OnLogInFailed if something went wrong.
				if(string.IsNullOrEmpty(accountPasswordEncrypted)) {
					SendLoginRequest(accountEmail, accountPassword);
				} else {
					SendEncryptedLoginRequest(accountEmail, accountPasswordEncrypted);
				}
			}
			GUI.enabled = true;
			
			GUI.contentColor = GUIColor.StatusMessage;
			
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			
			// Status message
			GUILayout.Label(statusMessage);
			GUI.contentColor = Color.white;
		}
		
		GUILayout.Space(10);
		GUILayout.FlexibleSpace();
		
		// Footer
		GUILayout.Label("Not registered?");
		
		using(new GUIHorizontal()) {
			// Register
			if(GUIHelper.Button("Register account", GUILayout.Width(140))) {
				Sounds.instance.PlayButtonClick();
				ChangeState(State.Register);
			}
			
			// Account activation mail
			if(accountNotActivated && accountEmail == inactiveEmail) {
				GUILayout.FlexibleSpace();
				if(GUIHelper.Button("Resend activation mail", GUILayout.Width(180))) {
					Sounds.instance.PlayButtonClick();
					Lobby.RPC("ResendActivationMail", Lobby.lobby, inactiveEmail);
				}
			}
		}

		GUIHelper.EndBox();
	}
	
	// Registration
	public void RegisterGUI() {
		GUIHelper.BeginBox(registerGUIDimensions.x, registerGUIDimensions.y);
		
		GUILayout.Label("Register a new account.");
		GUILayout.Space(10);
		
		int validationErrors = 0;
		
		// E-Mail
		GUILayout.Label("<b>E-Mail</b>");
		accountEmail = GUILayout.TextField(accountEmail, textFieldHeight);
		
		// Validate
		if(Validator.email.IsMatch(accountEmail)) {
			GUI.contentColor = GUIColor.Validated;
		} else {
			GUI.contentColor = GUIColor.NotValidated;
			validationErrors += 1;
		}
		GUILayout.Label("Please enter your email address, we will send you an account activation link and notify you when the Open Beta starts.");
		GUI.contentColor = Color.white;
		GUILayout.Space(5);
		
		// Password
		GUILayout.Label("<b>Password</b>");
		var newAccountPassword = GUILayout.PasswordField(accountPassword, '*', textFieldHeight);
		
		// User modified password
		if(accountPassword != newAccountPassword) {
			accountPasswordEncrypted = null;
			accountPassword = newAccountPassword;
		}
		
		// Validate
		if(Validator.password.IsMatch(accountPassword)) {
			GUI.contentColor = GUIColor.Validated;
		} else {
			GUI.contentColor = GUIColor.NotValidated;
			validationErrors += 1;
		}
		GUILayout.Label("Your password should contain a minimum of 6 characters.");
		GUI.contentColor = Color.white;
		GUILayout.Space(5);
		
		// Register + status message
		using(new GUIHorizontal()) {
			GUI.enabled = (validationErrors == 0 || GameDB.IsTestAccount(accountEmail));
			if(GUIHelper.Button("Register", GUILayout.Width(80))) {
				Sounds.instance.PlayButtonClick();
				
				// Registers a new account. The OnAccountRegistered callback will be called if the account was registered -
				// otherwise, the OnRegisterFailed callback is called.
				//AccountManager.RegisterAccount(accountName, accountPassword, accountEmail);
				SendRegisterRequest(accountEmail, accountPassword);
			}
			GUI.enabled = true;
			
			GUI.contentColor = GUIColor.StatusMessage;
			
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			
			// Status message
			GUILayout.Label(statusMessage);
			GUI.contentColor = Color.white;
		}
		
		GUILayout.FlexibleSpace();

		if (GUIHelper.Button("Back", GUILayout.Width(50))) {
			Sounds.instance.PlayButtonClick();
			ChangeState(State.LogIn);
		}

		GUIHelper.EndBox();
	}
	
	void LicenseGUI() {
		GUIHelper.BeginBox(460, 300);
		GUILayout.Label(@"You hereby agree to the following conditions:

1. Understand that most graphics are simply dummy graphics used in the development process and do not represent the quality of the final product.

2. Music is not integrated in the test versions anymore for faster publishing.

3. Your character will be reset at the start of the Closed Beta.

4. You will receive an Alpha Tester title in the future.
");
		GUILayout.FlexibleSpace();
		using(new GUIHorizontalCenter()) {
			if(GUIHelper.Button("I agree.") || (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)) {
				Sounds.instance.PlayButtonClick();
				ChangeState(State.Lobby);
				gameLobby.UpdateAccountInfo();
			}
		}
		GUIHelper.EndBox();
	}
	
	// Draw exit button and version
	void DrawExitButtonAndVersion() {
		if(!GameManager.inLobby && !GameManager.inGame) {
			GUI.color = Color.white;
			int margin = 2;
			
#if !UNITY_WEBPLAYER
			// Exit button
			int cWidth = closeIcon.width + 10;
			int cHeight = closeIcon.height + 5;
			
			if(GUI.Button(new Rect(GUIArea.width - cWidth - margin, margin, cWidth, cHeight), new GUIContent("", closeIcon, "Quit"))) {
				Sounds.instance.PlayButtonClick();
				Application.Quit();
			}
#endif
			// Version number
			if(gameLobby.currentState != GameLobbyState.Game) {
				string version = "Development Version " + GUIHelper.MakePrettyVersion(clientVersionNumber);
				Vector2 vSize = GUI.skin.label.CalcSize(new GUIContent(version));
				GUI.Label(new Rect(GUIArea.width - vSize.x - margin - 2, GUIArea.height - vSize.y - margin, vSize.x, vSize.y), version);
			}
		}
	}
	
	// Draw change log
	void DrawChangeLog() {
		if(GameManager.inLogIn && !AccountManager.isLoggedIn && !string.IsNullOrEmpty(Login.changeLog)) {
			using(new GUIArea(changeLogRect)) {
				using(new GUIVertical("box")) {
					using(new GUIScrollView(ref changeLogScroll)) {
						GUILayout.Label(Login.changeLog);
					}
				}
			}
		}
	}
	
	// Send login request
	void SendLoginRequest(string email, string password) {
		LogManager.General.Log("Login request...");
		
		// Login RPC
		SendEncryptedLoginRequest(email, GameDB.EncryptPasswordString(password));
	}
	
	// Send encrypted login request
	void SendEncryptedLoginRequest(string email, string encryptedPassword) {
		LogManager.General.Log("Using encrypted login request...");
		
		// This needs to be done in case we still got a delayed RPC in the login screen
		// We need to make sure PlayerAccount.mine is null
		gameLobby.ResetAccountInfo();
		
		// Login RPC
		accountPasswordEncrypted = encryptedPassword;
		Lobby.RPC("LobbyAccountLogIn", Lobby.lobby, email, accountPasswordEncrypted, SystemInfo.deviceUniqueIdentifier);
		
		loginRequests += 1;
		statusMessage = "Logging in to account " + email + "...";
	}
	
	// Send register request
	void SendRegisterRequest(string email, string password) {
		accountPasswordEncrypted = GameDB.EncryptPasswordString(password);
		Lobby.RPC("LobbyRegisterAccount", Lobby.lobby, email, accountPasswordEncrypted);
		
		statusMessage = "Registering new account " + email + "...";
	}
	
	void LobbyGUI() {
		gameLobby.Draw();
	}
	
	void PleaseUpdateGUI() {
		GUIHelper.BeginBox(600, 140);
		GUILayout.Label("Your client version is out of date, you need to update your client.\n\nClient version: " + clientVersionNumber + "\nServer version: " + serverVersionNumber);
#if UNITY_WEBPLAYER
		GUILayout.Label("\nPlease refresh the website.");
#else
		GUILayout.Label("Alternatively you can go to http://battle-of-mages.com and play with the latest client.");
#endif
		GUIHelper.EndBox();
	}
	
	public void ConnectingToLobbyGUI() {
		GUIHelper.BeginBox(400, 50);
		GUILayout.Label("Connecting to lobby...");
		GUIHelper.EndBox();
	}
	
	public void DisconnectedGUI() {
		GUIHelper.BeginBox(400, 80);

		GUILayout.Label("Disconnected from lobby.");
		
		GUILayout.FlexibleSpace();
		
		if(GUIHelper.Button("Reconnect", GUILayout.Width(100))) {
			Sounds.instance.PlayButtonClick();
			ConnectToLobby();
		}
		
		GUIHelper.EndBox();
	}
#endregion
	
#region Callbacks
	// OnAccountLoggedIn
	void OnAccountLoggedIn(Account account) {
		PlayerPrefs.SetString("AccountEmail", accountEmail);
		PlayerPrefs.SetString("AccountSaltedAndHashedPassword", accountPasswordEncrypted);
		
		Login.myAccount = account;

		// Play sound
		Sounds.instance.PlayLoginSuccess();
		
		// Reconnect from game?
		if(gameLobby.currentState == GameLobbyState.Game) {
			ChangeState(State.Game);
		} else {
			ChangeState(State.License);
#if UNITY_EDITOR
			if(editorAutoLogin) {
				ChangeState(State.Lobby);
			}
#endif
		}
		
		ArenaGUI.instance.ResetQueueInfo();
		
		loginRequestResponses += 1;
	}
	
	// OnAccountLoggedOut
	void OnAccountLoggedOut(Account account) {
		// Ingame logout
		if(Player.main != null) {
			uLink.Network.Disconnect();
			MainMenu.instance.ReturnToLobby();
			gameLobby.currentState = GameLobbyState.WaitingForAccountInfo;
		}
		
		// Clean up
		gameLobby.ResetAccountInfo();
		ArenaGUI.instance.ResetQueueInfo();
		
		ChangeState(State.LogIn);
	}
	
	// OnLogInFailed
	void OnLogInFailed(string accountName, AccountError error) {
		statusMessage = "Failed to log in to account " + accountName + " - ";
		
		switch(error) {
			case AccountError.InvalidPassword:
				statusMessage += "invalid password";
				break;
			case AccountError.NameNotRegistered:
				statusMessage += "name not registered";
				break;
			case AccountError.AlreadyLoggedIn:
				statusMessage += "account already logged in to";
				break;
			default:
				statusMessage += "unknown error occurred";
				break;
		}
		
		loginRequestResponses += 1;
	}
	
	// OnAccountRegistered
	private void OnAccountRegistered(Account account) {
		ChangeState(State.LogIn);
		statusMessage = "New account has been registered. Please check your mail to activate your account.";
		//SendLoginRequest(accountEmail, accountPassword);
	}
	
	// OnRegisterFailed
	private void OnRegisterFailed(string accountName, AccountError error) {
		statusMessage = "Failed to register account '" + accountName + "': ";

		switch(error) {
			case AccountError.NameAlreadyRegistered : statusMessage += "Name already registered."; break;
			default : statusMessage += "Unknown error occurred."; break;
		}
	}
	
	// On connection to lobby
	void uLobby_OnConnected() {
		LogManager.General.Log("Connected to lobby");
		
		accountEmail = PlayerPrefs.GetString("AccountEmail", "");
		accountPasswordEncrypted = PlayerPrefs.GetString("AccountSaltedAndHashedPassword", "");
		
		// Reset these counters to prevent the user from not being
		// able to log in after a reconnect:
		loginRequests = 0;
		loginRequestResponses = 0;
		
		// Decode
		if(!string.IsNullOrEmpty(accountPasswordEncrypted)) {
			accountPassword = encryptedPasswordString;
		}
		
		if(accountEmail != "") {
			accountNameLoaded = true;
		}
		
// FOR TESTING ONLY
#if UNITY_EDITOR
		if(editorAutoLogin) {
			accountEmail = editorAccountName;
			accountPassword = editorPassword;
			SendLoginRequest(accountEmail, accountPassword);
		}
#endif
		
		// Auto login for reconnect
		if(gameLobby.currentState == GameLobbyState.Game && accountEmail != "" && accountPassword != "") {
			SendLoginRequest(accountEmail, accountPassword);
		}
	}

	void uLobby_OnDisconnected() {
		LogManager.General.Log("Disconnected from lobby");
	}
	
	void OnApplicationFocus(bool focused) {
		if(!lowerFPSWhenIdle)
			return;
		
		if(focused) {
			Application.targetFrameRate = GameManager.inGame ? -1 : lobbyFrameRate;
			LogManager.Spam.Log("Application focused. Target frame rate: " + Application.targetFrameRate);
		} else {
			Application.targetFrameRate = lobbyFrameRateIdle;
			LogManager.Spam.Log("Application unfocused. Target frame rate: " + Application.targetFrameRate);
		}
	}
#endregion
	
#region RPCs
	[RPC]
	void EmailAlreadyExists() {
		statusMessage = "Failed to register account: E-Mail has already been registered.";
	}
	
	[RPC]
	void AccountNotActivated(string nInactiveEmail) {
		inactiveEmail = nInactiveEmail;
		accountNotActivated = true;
		statusMessage = "Failed to login: Account has not been activated yet. Please check your mail.";
		
		loginRequestResponses += 1;
	}
	
	[RPC]
	void ActivationMailSent() {
		statusMessage = "Account activation mail has been sent again. Please check your mail.";
	}
	
	[RPC]
	void VersionNumber(int nServerVersionNumber) {
		serverVersionNumber = nServerVersionNumber;
		LogManager.General.Log("Client version: " + clientVersionNumber + ", server version: " + serverVersionNumber);
		
		if(clientVersionNumber >= serverVersionNumber) {
			ChangeState(State.LogIn);
		} else {
			ChangeState(State.Update);
		}
	}
#endregion
}