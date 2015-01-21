using UnityEngine;
using System.Collections.Generic;
using uLobby;

public class UIManager : DestroyableSingletonMonoBehaviour<UIManager>, Initializable {
	public FadeTime fadeTime;
	public GameObject root;
	
	private Dictionary<string, GameObject> stateNameToGameObject = new Dictionary<string, GameObject>();
	private string _currentState = "";

	// Init
	public void Init() {
		foreach(Transform child in root.transform) {
			var state = child.gameObject;

			if(!state.name.Contains(" UI"))
				continue;

			state.SetActive(false);
			stateNameToGameObject.Add(state.name.Replace(" UI", ""), state);
		}

		// Go to lobby or waiting page on login
		Login.instance.onLogIn += (account) => {
			if(account.playerName.available)
				UIManager.instance.currentState = "Lobby";
			else
				UIManager.instance.currentState = "Waiting";
		};

		// Return to login page on logout
		Login.instance.onLogOut += (account) => {
			if(Lobby.connectionStatus == LobbyConnectionStatus.Connected)
				UIManager.instance.currentState = "Login";
		};
	}

	// FadeOut
	void FadeOut(string stateName, CallBack fadeEndCallBack = null) {
		GameObject oldObject;
		
		if(stateNameToGameObject.TryGetValue(stateName, out oldObject)) {
			var oldCanvasGroup = oldObject.GetComponent<CanvasGroup>();
			
			// Fade alpha
			oldCanvasGroup.Fade(
				fadeTime.fadeOut,
				(val) => {
					oldCanvasGroup.alpha = 1f - val;
				},
				() => {
					oldObject.SetActive(false);
					
					if(fadeEndCallBack != null)
						fadeEndCallBack();
				}
			);
		} else if(fadeEndCallBack != null) {
			fadeEndCallBack();
		}
	}

	// FadeIn
	void FadeIn(string stateName) {
		// Fade in new
		GameObject newObject;
		
		if(stateNameToGameObject.TryGetValue(stateName, out newObject)) {
			var newCanvasGroup = newObject.GetComponent<CanvasGroup>();
			newCanvasGroup.alpha = 0f;
			
			newObject.SetActive(true);
			
			// Fade alpha
			newCanvasGroup.Fade(
				fadeTime.fadeIn,
				(val) => {
					newCanvasGroup.alpha = val;
				}
			);
		} else {
			LogManager.General.LogWarning("UI state '" + stateName + "' doesn't exist");
		}
	}

	// Current state
	public string currentState {
		get {
			return _currentState;
		}

		set {
			// No changes if they're the same already
			if(_currentState == value)
				return;

			// Update
			var oldState = _currentState;
			_currentState = value;
			var newState = _currentState;

			// Fade out
			FadeOut(oldState, () => {
				// Fade in
				FadeIn(newState);
			});
		}
	}
}
