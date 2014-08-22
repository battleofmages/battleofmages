using UnityEngine;
using UnityEngine.UI;
using uLobby;

public class Login : LobbyModule<Login> {
	private const string encryptedPasswordString = "--------------------";

	[Header("Lobby Server")]
	public string lobbyHost;
	public string editorLobbyHost;
	public int lobbyPort;

	[Header("Retrieve Lobby Server IP")]
	public bool retrieveLobbyIP = false;
	public string lobbyIpURL;

	[Header("Lobby FPS")]
	public int lobbyFrameRate;
	public bool lowerFPSWhenIdle;
	public int lobbyFrameRateIdle;

	[Header("Auto Login")]
	public bool editorAutoLogin;
	public string editorAccountName;
	public string editorPassword;

	[Header("UI")]
	public InputField emailField;
	public InputField passwordField;
	public Button loginButton;
	public GameObject connectingPanel;
	public GameObject loginPanel;
	public Text statusMessageLabel;
	public Button resendActivationMailButton;

	// Inactivate account
	private string inactiveEmail;

#region Functions
	// Start
	void Start() {
		// System report
		LogManager.System.GenerateReport();

		// Genuine check
		if(Application.genuineCheckAvailable && !Application.genuine) {
			LogManager.General.LogError("Client files have been modified, quitting.");
			Application.Quit();
			return;
		}

		// Set frame rate
		Application.targetFrameRate = lobbyFrameRate;

		// Reset status message
		statusMessage = "";

		// Editor only
#if UNITY_EDITOR
		InitEditorData();
#endif
		// Public key
		NetworkHelper.InitPublicLobbyKey();
		
		// Receive lobby events
		Lobby.AddListener(this);
		
		// Add this class as a listener to different account events.
		AccountManager.OnAccountLoggedIn += OnAccountLoggedIn;
		AccountManager.OnAccountLoggedOut += OnAccountLoggedOut;
		AccountManager.OnAccountRegistered += OnAccountRegistered;
		AccountManager.OnLogInFailed += OnLogInFailed;
		AccountManager.OnRegisterFailed += OnRegisterFailed;

		// Connect
		if(retrieveLobbyIP) {
			// Download IP and port from text file
			StartCoroutine(NetworkHelper.DownloadIPAndPort(lobbyIpURL, (host, port) => {
				lobbyHost = host;
				lobbyPort = port;
				
				Connect();
			}));
		} else {
			// Connect with the provided host data
			Connect();
		}
	}

	// Connect: Also used for reconnecting
	void Connect() {
		if(Lobby.connectionStatus == LobbyConnectionStatus.Connected)
			return;

		// Reset pending request count because we were disconnected
		DonationsGUI.instance.pendingCrystalBalanceRequests = 0;
		GuildsGUI.instance.pendingGuildListRequests = 0;
		RankingGUI.instance.pendingRankingListRequests = 0;

		// UI
		state = State.ConnectingToLobby;

		// Connect
		LogManager.General.Log("Connecting to lobby @ " + lobbyHost + ":" + lobbyPort);
		Lobby.ConnectAsClient(lobbyHost, lobbyPort);
	}

	// LogIn
	public void LogIn() {
		if(string.IsNullOrEmpty(accountPasswordEncrypted))
			SendLoginRequest(accountEmail, accountPassword);
		else
			SendEncryptedLoginRequest(accountEmail, accountPasswordEncrypted);
	}
	
	// LogOut
	public void LogOut() {
		// Activate loading screen
		LoadingScreen.instance.Enable(() => {
			LoadingScreen.instance.statusMessage = "Logging out...";
			
			LogManager.General.Log("Logging out...");
			Lobby.RPC("LobbyAccountLogOut", Lobby.lobby);
			
			if(AccountManager.isLoggedIn)
				Lobby.RPC("LeaveInstance", Lobby.lobby, GameManager.gameEnded);
		});
	}
	
	// ReturnToWorld
	public void ReturnToWorld() {
		LogManager.General.Log("Returning to the world");
		
		if(AccountManager.isLoggedIn)
			Lobby.RPC("LeaveInstance", Lobby.lobby, GameManager.gameEnded);
	}
	
	// ReturnToLogin
	public void ReturnToLogin() {
		// Then we load the lobby up again
		LoadingScreen.instance.SecureLoadLevel("LobbyClient", () => {
			// If it was disabled before by accident...
			InGameLobby.instance.lobbyChat.chatInputEnabled = true;
			LobbyChat.instance.currentChannel = "Global";

			// Play login screen music
			MusicManager.instance.PlayCategory(GameObject.FindGameObjectWithTag("Map").GetComponent<MusicCategory>());
			
			// Frame rate
			Application.targetFrameRate = lobbyFrameRate;
			
			// Re-enable cursor
			Screen.lockCursor = false;
			Screen.showCursor = true;
			
			// Fade out loading screen
			LoadingScreen.instance.Disable();
		});
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
		InGameLobby.instance.ResetAccountInfo();
		
		// Login RPC
		accountPasswordEncrypted = encryptedPassword;
		Lobby.RPC("LobbyAccountLogIn", Lobby.lobby, email, accountPasswordEncrypted, SystemInfo.deviceUniqueIdentifier);
		
		loginRequestCount += 1;
		statusMessage = "Logging in to account " + email + "...";
	}

	// ResendActivationMail
	public void ResendActivationMail() {
		Lobby.RPC("ResendActivationMail", Lobby.lobby, inactiveEmail);
	}

	// Validate
	public void Validate() {
		if(emailField == null || passwordField == null || loginButton == null)
			return;

		loginButton.interactable = (
			GameDB.IsTestAccount(accountEmail) || 
			(
				loginRequestCount == loginRequestResponseCount &&
				Validator.email.IsMatch(accountEmail)
			)
		);
	}

	// InitEditorData
	void InitEditorData() {
		if(!Application.CanStreamedLevelBeLoaded("Client")) {
			editorAutoLogin = false;
			LogManager.General.LogError("<color=red>YOU FORGOT TO ADD THE CLIENT SCENE IN THE BUILD SETTINGS</color>");
		}
		
		// Disable intro
		if(editorAutoLogin)
			GetComponent<Intro>().enabled = false;
		
		// Don't retrieve IP in the editor
		retrieveLobbyIP = false;
		
		// Set lobby host to the editor lobby host setting
		lobbyHost = editorLobbyHost;
	}
#endregion

#region Properties
	// E-Mail
	public string accountEmail {
		get {
			return emailField.value;
		}

		set {
			emailField.value = value;
		}
	}

	// Password
	public string accountPassword {
		get {
			return passwordField.value;
		}
		
		set {
			passwordField.value = value;
		}
	}

	// Account password encrypted
	public string accountPasswordEncrypted {
		get;
		protected set;
	}

	// Login request count
	public int loginRequestCount {
		get;
		protected set;
	}

	// Login request response count
	public int loginRequestResponseCount {
		get;
		protected set;
	}

	// Status message
	public string statusMessage {
		get {
			return statusMessageLabel.text;
		}

		protected set {
			statusMessageLabel.text = value;
		}
	}

	// State
	public State state {
		set {
			GameManager.currentState = value;
			statusMessage = "";

			connectingPanel.SetActive(false);
			loginPanel.SetActive(false);

			switch(GameManager.currentState) {
				case State.ConnectingToLobby:
					connectingPanel.SetActive(true);
					break;

				case State.LogIn:
					loginPanel.SetActive(true);
					break;
			}
		}
	}
#endregion

#region Callbacks
	// OnAccountLoggedIn
	void OnAccountLoggedIn(Account account) {
		PlayerPrefs.SetString("AccountEmail", accountEmail);
		PlayerPrefs.SetString("AccountSaltedAndHashedPassword", accountPasswordEncrypted);
		
		// Play sound
		Sounds.instance.loginSuccess.Play();

		// Reset arena info
		ArenaGUI.instance.ResetQueueInfo();

		// Set state
		state = State.License;

		// Increase login response count
		loginRequestResponseCount += 1;
	}
	
	// OnAccountLoggedOut
	void OnAccountLoggedOut(Account account) {
		// Ingame logout
		if(Player.main != null) {
			uLink.Network.Disconnect();
			ReturnToLogin();
			InGameLobby.instance.currentState = GameLobbyState.WaitingForAccountInfo;
		}
		
		// Clean up
		InGameLobby.instance.ResetAccountInfo();
		ArenaGUI.instance.ResetQueueInfo();
		
		state = State.LogIn;
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
		
		loginRequestResponseCount += 1;
	}
	
	// OnAccountRegistered
	private void OnAccountRegistered(Account account) {
		state = State.LogIn;
		statusMessage = "New account has been registered. Please check your mail to activate your account.";
	}
	
	// OnRegisterFailed
	private void OnRegisterFailed(string accountName, AccountError error) {
		statusMessage = "Failed to register account '" + accountName + "': ";
		
		switch(error) {
			case AccountError.NameAlreadyRegistered:
				statusMessage += "Name already registered.";
				break;

			default:
				statusMessage += "Unknown error occurred.";
				break;
		}
	}
	
	// uLobby: Connected
	void uLobby_OnConnected() {
		LogManager.General.Log("Connected to lobby");

		accountEmail = PlayerPrefs.GetString("AccountEmail", "");
		accountPasswordEncrypted = PlayerPrefs.GetString("AccountSaltedAndHashedPassword", "");
		
		// Reset these counters to prevent the user from not being
		// able to log in after a reconnect:
		loginRequestCount = 0;
		loginRequestResponseCount = 0;
		
		// Decode
		if(!string.IsNullOrEmpty(accountPasswordEncrypted)) {
			accountPassword = encryptedPasswordString;
		}

		// Set state
		state = State.LogIn;

		// Validate
		Validate();
		
		// FOR TESTING ONLY
#if UNITY_EDITOR
		if(editorAutoLogin) {
			accountEmail = editorAccountName;
			accountPassword = editorPassword;
			SendLoginRequest(accountEmail, accountPassword);
		}
#endif

		// Auto login for reconnect
		if(GameManager.currentState == State.Game && accountEmail != "" && accountPassword != "") {
			SendLoginRequest(accountEmail, accountPassword);
		}
	}
	
	// uLobby: Disconnected
	void uLobby_OnDisconnected() {
		LogManager.General.Log("Disconnected from lobby");
	}
	
	// On application focus
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
		resendActivationMailButton.enabled = true;
		statusMessage = "Failed to login: Account has not been activated yet. Please check your mail.";
		loginRequestResponseCount += 1;
	}
	
	[RPC]
	void ActivationMailSent() {
		statusMessage = "Account activation mail has been sent again. Please check your mail.";
	}
#endregion
}
