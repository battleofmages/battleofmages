using UnityEngine;
using UnityEngine.UI;
using uLobby;

public class Login : SingletonMonoBehaviour<Login>, Initializable {
	public InputField emailField;
	public InputField passwordField;
	private bool autoLogin;

	// Init
	public void Init() {
		// Add this class as a listener to different account events
		AccountManager.OnAccountLoggedIn += OnAccountLoggedIn;
		AccountManager.OnAccountLoggedOut += OnAccountLoggedOut;
		AccountManager.OnLogInFailed += OnLogInFailed;

		// Load saved login data
		emailField.text = PlayerPrefs.GetString("AccountEmail", "");
		autoLogin = PlayerPrefs.GetInt("AutoLogin", 0) != 0;

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
		
		// Login RPC
		Lobby.RPC("AccountLogIn", Lobby.lobby, email, encryptedPassword, SystemInfo.deviceUniqueIdentifier);
	}

	// AutoLogin
	void AutoLogin() {
		var email = PlayerPrefs.GetString("AccountEmail", "");
		var encryptedPassword = PlayerPrefs.GetString("AccountSaltedAndHashedPassword", "");

		if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(encryptedPassword)) {
			LogManager.General.Log("No login data saved, can't automatically+ login");
			return;
		}

		Login.instance.SendEncryptedLoginRequest(email, encryptedPassword);
	}

#region Callbacks
	// uLobby: Connected
	void uLobby_OnConnected() {
		// Activate UI
		UIManager.instance.currentState = "Login";
		
		// Auto login
		if(autoLogin)
			AutoLogin();
	}

	// OnAccountLoggedIn
	void OnAccountLoggedIn(Account account) {
		LogManager.General.Log("Successfully logged in: " + account.name);

		// Login sound
		Sounds.instance.Play("logIn");

		// Deactivate low pass
		AudioManager.instance.Fade(
			4f,
			(val) => {
				AudioManager.instance.mixer.SetFloat("musicCutOffFreq", 500f + 21500f * val * val);
			}
		);

		// Set my account
		PlayerAccount.mine = PlayerAccount.Get(account.id.value);

		/*PlayerPrefs.SetString("AccountEmail", accountEmail);
		PlayerPrefs.SetString("AccountSaltedAndHashedPassword", accountPasswordEncrypted);*/

		// Go to lobby
		if(PlayerAccount.mine.playerName.available)
			UIManager.instance.currentState = "Lobby";
		else
			UIManager.instance.currentState = "Waiting";
	}
	
	// OnAccountLoggedOut
	void OnAccountLoggedOut(Account account) {
		// Login page
		if(Lobby.connectionStatus == LobbyConnectionStatus.Connected)
			UIManager.instance.currentState = "Login";

		// Activate low pass
		AudioManager.instance.Fade(
			2f,
			(val) => {
				val = 1f - val;
				AudioManager.instance.mixer.SetFloat("musicCutOffFreq", 500f + 21500f * val * val);
			}
		);

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
