using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uLobby;

public sealed class DonationsGUI : LobbyModule<DonationsGUI> {
	public string paypalButtonId;
	public string paypalSandboxButtonId;
	public int buttonWidth;
	public int buttonHeight;
	public Texture2D donateButtonImage;
	public bool useSandBox;
	
	[HideInInspector]
	public int pendingCrystalBalanceRequests = 0;
	
	private Login loginClientGUI;
	private Vector2 scrollPosition;
	
	// Start
	void Start() {
		loginClientGUI = this.GetComponent<Login>();
		
		// Receive lobby RPCs
		Lobby.AddListener(this);
		
		// Retrieve crystal balance when logging in
		AccountManager.OnAccountLoggedIn += OnAccountLoggedIn;
	}

	// OnClick
	public override void OnClick() {
		if(pendingCrystalBalanceRequests == 0) {
			Sounds.instance.PlayButtonClick();
			
			Lobby.RPC("CrystalBalanceRequest", Lobby.lobby);
			pendingCrystalBalanceRequests += 1;
		}
	}
	
	// Draw
	public override void Draw() {
		using(new GUIScrollView(ref scrollPosition)) {
			// Donate button
			using(new GUIHorizontalCenter()) {
				if(GUIHelper.Button(new GUIContent("  Support the development of Battle of Mages! ", donateButtonImage), GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
					string baseURL;
					string buttonId;
					
					if(useSandBox) {
						baseURL = "https://www.sandbox.paypal.com";
						buttonId = paypalSandboxButtonId;
					} else {
						baseURL = "https://www.paypal.com";
						buttonId = paypalButtonId;
					}
					
					string accountName = loginClientGUI.accountEmail;
					string finalURL = baseURL + "/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=" + buttonId + "&item_name=Battle%20of%20Mages%20(Account:%20" + accountName + ")&custom=" + accountName;
					
#if !UNITY_EDITOR
					Application.ExternalEval("window.open('" + finalURL + "', 'Battle of Mages - Donation')");
#else
					Application.OpenURL(finalURL);
#endif			
				}
			}
			
			// Donate text
			using(new GUIHorizontalCenter()) {
				GUILayout.Label(@"
Unlike big companies whose goal is mainly to make profit, my main goal is to give players
a highly enjoyable experience playing the game. Developing and maintaining a game of the scale of
Battle of Mages costs a lot of money, not to mention the monthly server costs. Therefore I
appreciate every single contribution because it helps me affording better servers and create
better content which will affect <b>your</b> gameplay experience.

If you'd like this game to improve and stay alive please consider a little donation.");
			}
		}
	}
	
	// On coming close to the NPC
	public override void OnNPCEnter() {
		// Fetch the new crystal balance
		InGameLobby.instance.currentLobbyModule = this;
	}
	
	void OnAccountLoggedIn(Account account) {
		Lobby.RPC("CrystalBalanceRequest", Lobby.lobby);
		pendingCrystalBalanceRequests += 1;
	}
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	[RPC]
	void ReceiveCrystalBalance(string accountId, int newCrystalBalance) {
		PlayerAccount.Get(accountId).crystals = newCrystalBalance;
		
		if(pendingCrystalBalanceRequests > 0)
			pendingCrystalBalanceRequests -= 1;
	}
#endregion
}
