using UnityEngine;
using UnityEngine.UI;
using uLobby;

public class Login : SingletonMonoBehaviour<Login>, Initializable {
	public InputField emailField;
	public InputField passwordField;
	public Toggle autoLoginToggle;

	private string lastLoginMail;
	private string lastLoginEncryptedPassword;

	public event AccountChangedCallBack onLogIn;
	public event AccountChangedCallBack onLogOut;

	// Init
	public void Init() {
		// Add this class as a listener to different account events
		AccountManager.OnAccountLoggedIn += OnAccountLoggedIn;
		AccountManager.OnAccountLoggedOut += OnAccountLoggedOut;
		AccountManager.OnLogInFailed += OnLogInFailed;

		// Load saved login data
		emailField.text = PlayerPrefs.GetString("AccountEmail", "");
		autoLoginToggle.isOn = PlayerPrefs.GetInt("AutoLogin", 0) != 0;

		// Receive lobby events
		Lobby.AddListener(this);
	}
	
	// LogIn
	public void LogIn() {
		SendLoginRequest(emailField.text, passwordField.text);
	}

	// LogOut
	public void LogOut() {
		Lobby.RPC("AccountLogOut", Lobby.lobby);
	}

	// Send login request
	public void SendLoginRequest(string email, string password) {
		LogManager.General.Log("Sending login request...");
		
		// Login RPC
		SendEncryptedLoginRequest(email, GameDB.EncryptPasswordString(password));
	}

	// Send encrypted login request
	public void SendEncryptedLoginRequest(string email, string encryptedPassword) {
		LogManager.General.Log("Using encrypted login request...");

		// Set this to save it in PlayerPrefs later if login was successful
		lastLoginMail = email;
		lastLoginEncryptedPassword = encryptedPassword;

		// Login RPC
		Lobby.RPC("AccountLogIn", Lobby.lobby, email, encryptedPassword, SystemInfo.deviceUniqueIdentifier);
	}

	// AutoLogin
	void AutoLogin() {
		var email = PlayerPrefs.GetString("AccountEmail", "");
		var encryptedPassword = PlayerPrefs.GetString("AccountSaltedAndHashedPassword", "");

		if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(encryptedPassword)) {
			LogManager.General.Log("No login data saved, can't automatically login");
			return;
		}

		Login.instance.SendEncryptedLoginRequest(email, encryptedPassword);
	}

	// SetAutoLogin
	public void SetAutoLogin(bool enabled) {
		PlayerPrefs.SetInt("AutoLogin", enabled ? 1 : 0);
	}

#region Callbacks
	// uLobby: Connected
	void uLobby_OnConnected() {
		// Activate UI
		UIManager.instance.currentState = "Login";
		
		// Auto login
		if(autoLoginToggle.isOn)
			AutoLogin();
	}

	// OnAccountLoggedIn
	void OnAccountLoggedIn(Account account) {
		LogManager.General.Log("Successfully logged in: " + account.name);

		// Set my account
		PlayerAccount.mine = PlayerAccount.Get(account.id.value);

		// Login sound
		Sounds.instance.Play("logIn");

		PlayerPrefs.SetString("AccountEmail", lastLoginMail);
		PlayerPrefs.SetString("AccountSaltedAndHashedPassword", lastLoginEncryptedPassword);

		// Callback
		if(onLogIn != null)
			onLogIn(PlayerAccount.mine);
	}
	
	// OnAccountLoggedOut
	void OnAccountLoggedOut(Account account) {
		// Callback
		if(onLogOut != null)
			onLogOut(PlayerAccount.mine);

		// Reset player account
		PlayerAccount.mine = null;
	}
	
	// OnLogInFailed
	void OnLogInFailed(string email, AccountError error) {
		string statusMessage = "";
		
		switch(error) {
			case AccountError.InvalidPassword:
				statusMessage += "Invalid password";
				break;
			case AccountError.NameNotRegistered:
				statusMessage += "E-Mail not registered";
				break;
			case AccountError.AlreadyLoggedIn:
				statusMessage += "Account already logged in to";
				break;
			default:
				statusMessage += "Unknown error occurred";
				break;
		}

		NotificationManager.instance.CreateNotification(statusMessage, 3f);
	}
#endregion
}
