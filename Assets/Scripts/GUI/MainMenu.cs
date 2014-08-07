using uLobby;
using UnityEngine;

public enum InGameMenuState {
	None,
	Continue,
	Lobby,
	PvP,
	Settings,
	Logout,
	LeaveGame,
}

[System.Serializable]
public class MainMenuItem {
	public GUIContent content;
	public InGameMenuState menuState;
}

public class MainMenu : DestroyableSingletonMonoBehaviour<MainMenu> {
	public int width = 200;
	private float height;
	private float menuItemHeight;

	public MainMenuItem[] menuItemsWorld;
	public MainMenuItem[] menuItemsTown;
	public MainMenuItem[] menuItemsArena;
	public GUIStyle contentStyle;
	
	//private Transform camPivot;
	//private ToggleMouseLook toggleMouseLook;
	
	public InGameMenuState currentState = InGameMenuState.None;
	
	[HideInInspector]
	public InGameMenuState nextState;

	private bool leftGame = false;
	private bool loggedOut = false;
	private MainMenuItem[] menuItems;
	private Rect drawRect;
	
	// Start
	void Start() {
		//camPivot = GameObject.Find("CamPivot").transform;
		//toggleMouseLook = camPivot.GetComponent<ToggleMouseLook>();
		drawRect = new Rect(0f, 0f, 0f, 0f);
		enabled = false;
	}
	
	// OnEnable
	void OnEnable() {
		currentState = InGameMenuState.None;
		nextState = InGameMenuState.None;

		Screen.showCursor = true;

		if(Player.main != null)
			Player.main.crossHair.enabled = false;
	}
	
	// OnDisable
	void OnDisable() {
		currentState = InGameMenuState.None;
		nextState = InGameMenuState.None;

		Screen.showCursor = false;

		if(Player.main != null)
			Player.main.crossHair.enabled = true;
		
		if(InGameLobby.instance)
			InGameLobby.instance.displayedAccount = PlayerAccount.mine;
	}
	
	// OnGUI
	void OnGUI() {
		if(GameManager.gameEnded)
			return;
		
		if(currentState != InGameMenuState.Lobby) {
			switch(GameManager.serverType) {
				case ServerType.World:
					menuItems = menuItemsWorld;
					break;

				case ServerType.Town:
					menuItems = menuItemsTown;
					break;

				default:
					menuItems = menuItemsArena;
					break;
			}

			menuItemHeight = contentStyle.CalcSize(menuItems[0].content).y + 4;
			height = menuItems.Length * menuItemHeight;
			
			drawRect.y = GUIArea.height / 2 - height / 2;
			drawRect.width = width;
			drawRect.height = height;
			
			using(new GUIArea(drawRect)) {
				using(new GUIVertical()) {
					for(int i = 0; i < menuItems.Length; i++) {
						GUILayout.BeginHorizontal();
						
						if(menuItems[i].menuState == currentState) {
							GUI.backgroundColor = GUIColor.MenuItemActive;
						} else {
							GUI.backgroundColor = GUIColor.MenuItemInactive;
						}
						
						bool isLeave = (i == menuItems.Length - 1);
						bool isLogOut = (i == menuItems.Length - 2);
						
						if(isLeave) {
							if(GameManager.isPvP) {
								menuItems[i].content.text = " Leave";
							} else {
								menuItems[i].content.text = " Exit game";
							}
						}
						
						if(GUIHelper.Button(menuItems[i].content, contentStyle)) {
							Sounds.instance.buttonClick.Play();
							
							var newState = menuItems[i].menuState;
							if(newState == currentState) {
								nextState = InGameMenuState.None;
							} else {
								if(isLeave) {
									new Confirm(
										"Are you sure you want to leave?",
										() => { this.nextState = newState; },
										null
									);
								} else if(isLogOut) {
									new Confirm(
										"Are you sure you want to log out?",
										() => { this.nextState = newState; },
										null
									);
								} else {
									nextState = newState;
								}
							}
						}
						GUILayout.EndHorizontal();
					}
				}
			}
		}
		
		GUI.backgroundColor = Color.white;
		HandleInGameMenu(currentState);
	}
	
	// LateUpdate
	void LateUpdate() {
		currentState = nextState;
	}
	
	// HandleInGameMenu
	void HandleInGameMenu(InGameMenuState state) {
		// Disable intro
		if(state != InGameMenuState.None) {
			if(MapManager.mapIntro != null && MapManager.mapIntro.enabled)
				MapManager.mapIntro.enabled = false;
			
			switch(state) {
				case InGameMenuState.Continue:
					CameraMode.current.Continue();
					break;
					
				case InGameMenuState.Lobby:
					/*if(InGameLobby.instance != null) {
						using(new GUIArea((int)(Screen.width * 0.8f), (int)(Screen.height * 0.7f))) {
							InGameLobby.instance.Draw();
						}
					}*/
					break;
					
				case InGameMenuState.PvP:
					if(ArenaGUI.instance != null) {
						using(new GUIArea((int)(Screen.width * 0.7f), (int)(Screen.height * 0.7f))) {
							if(!ArenaGUI.instance.inQueue) {
								ArenaGUI.instance.Draw();
							}
						}
					}
					break;
					
				case InGameMenuState.Settings:
					if(SettingsGUI.instance != null) {
						using(new GUIArea((int)(Screen.width * 0.7f), (int)(Screen.height * 0.7f))) {
							SettingsGUI.instance.Draw();
						}
					}
					break;
					
				case InGameMenuState.Logout:
					if(!loggedOut) {
						Login.instance.LogOut();
						loggedOut = true;
					}
					break;
					
				case InGameMenuState.LeaveGame:
					if(!leftGame) {
						if(GameManager.isPvE) {
							LogManager.General.Log("Exiting application...");
							Application.Quit();
						} else {
							LogManager.General.Log("Leaving game...");
							Login.instance.ReturnToWorld();
						}
						
						leftGame = true;
					}
					break;
			}
		}
	}
	
	// uLink_OnConnectedToServer
	/*void uLink_OnConnectedToServer(System.Net.IPEndPoint server) {
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}*/
}