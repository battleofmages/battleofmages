using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LobbyModule<T> : SingletonMonoBehaviour<T>, NPCModule, DrawableLobbyModule where T : LobbyModule<T> {
	private List<CallBack> guiDelayedExecution = new List<CallBack>();
	
	// TODO: Reserved for localization
	protected static string _(string format, params System.Object[] args) {
		return string.Format(format, args);
	}
	
	// Display player name
	protected static void DrawPlayerName(string playerName, GUIContent content, params GUILayoutOption[] options) {
		DrawPlayerName(playerName, content, null, options);
	}
	
	// Display player name
	protected static void DrawPlayerName(string playerName, GUIContent content, GUIStyle style, params GUILayoutOption[] options) {
		// Add status icon
		if(content.image == null) {
			var account = PlayerAccount.GetByPlayerName(playerName);
			
			if(account != null)
				content.image = account.onlineStatusImage;
		}
		
		// Player name on button
		if(GUIHelper.Button(content, style, options)) {
			if(Event.current.button == 1) {
				InGameLobby.instance.CreatePlayerPopupMenu(playerName);
			}
		}
	}
	
	// ExecuteLater
	protected void ExecuteLater(CallBack callBack) {
		guiDelayedExecution.Add(callBack);
	}
	
	// Update
	protected void Update() {
		if(guiDelayedExecution.Count > 0) {
			foreach(var callBack in guiDelayedExecution) {
				callBack();
			}
			
			guiDelayedExecution.Clear();
		}
	}
	
	public virtual void Draw() {}
	public virtual void OnClick() {}
	public virtual void OnNPCEnter() {}
	public virtual void OnNPCTalk() {}
	public virtual void OnNPCExit() {}
}
