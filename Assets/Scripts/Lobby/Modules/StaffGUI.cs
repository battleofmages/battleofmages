using uLobby;
using UnityEngine;

public class StaffGUI : LobbyModule<StaffGUI> {
	[HideInInspector]
	public int pendingStaffRequests = 0;
	
	private KeyValue<TimeStamp>[] lastLogins;
	private KeyValue<TimeStamp>[] lastRegistrations;
	private Vector2 scrollPosition;
	private PlayerAccount account;
	
	// Start
	void Start() {
		// Receive lobby RPCs
		Lobby.AddListener(this);
	}

	// OnClick
	public override void OnClick() {
		if(pendingStaffRequests == 0) {
			Sounds.instance.PlayButtonClick();

			LogManager.General.Log("Requesting staff information");
			Lobby.RPC("StaffInfoRequest", Lobby.lobby);
			pendingStaffRequests += 2;
		}
	}
	
	// Account statistics
	public override void Draw() {
		account = InGameLobby.instance.displayedAccount;
		
		GUILayout.Label(_("Your access level: <b>{0}</b>", account.accessLevel));
		
		using(new GUIScrollView(ref scrollPosition)) {
			GUILayout.Label("Last logins");
			DrawAccountTimeStampData(lastLogins);
			
			GUILayout.Label("Last registrations");
			DrawAccountTimeStampData(lastRegistrations);
		}
	}
	
	// DrawAccountTimeStampData
	void DrawAccountTimeStampData(KeyValue<TimeStamp>[] data) {
		if(data == null)
			return;
		
		var now = System.DateTime.UtcNow;
		
		foreach(var entry in data) {
			var account = PlayerAccount.Get(entry.key);
			var timeSpan = now - entry.val.dateTime;
			var timeSpanString = TimeSpanToString(timeSpan);
			var columnWidth = GUILayout.Width(GUIArea.width * 0.25f);
			
			using(new GUIHorizontal()) {
				DrawPlayerName(
					account.playerName,
					new GUIContent(
						"<b>" + account.playerName + "</b> <size=10>(ID: " + account.accountId + ")</size>"
					),
					GUI.skin.label,
					columnWidth
				);
				
				GUILayout.Label(timeSpanString, columnWidth);
				GUILayout.Label("<size=12>" + entry.val.readableDateTime + " UTC</size>", columnWidth);
			}
		}
	}
	
	// TimeSpanToString
	string TimeSpanToString(System.TimeSpan t) {
		if(t.TotalSeconds < 60)
			return string.Format("{0} ago", GUIHelper.Plural(t.Seconds, "second"));
		else if(t.TotalMinutes < 60)
			return string.Format("{0} ago", GUIHelper.Plural(t.Minutes, "minute"));
		else if(t.TotalHours < 24)
			return string.Format("{0} ago", GUIHelper.Plural(t.Hours, "hour"));
		else
			return string.Format("{0} ago", GUIHelper.Plural(t.Days, "day"));
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
#region RPCs
	[RPC]
	void ReceiveLastLogins(KeyValue<TimeStamp>[] data, bool dummy) {
		LogManager.General.Log("StaffGUI: Received last logins!");
		lastLogins = data;
		
		if(pendingStaffRequests > 0)
			pendingStaffRequests -= 1;
	}
	
	[RPC]
	void ReceiveLastRegistrations(KeyValue<TimeStamp>[] data, bool dummy) {
		LogManager.General.Log("StaffGUI: Received last registrations!");
		lastRegistrations = data;
		
		if(pendingStaffRequests > 0)
			pendingStaffRequests -= 1;
	}
#endregion
}
