using uLobby;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class RankingGUI : LobbyModule<RankingGUI> {
	public const int leaguePoints = 500;
	public static int divisionPoints;
	public Dictionary<string, Texture2D> countryCodeToIcon = new Dictionary<string, Texture2D>();
	
	public GUIStyle rankingNumberStyle;
	public GUIStyle rankingNameStyle;
	public GUIStyle rankingScoreStyle;
	public GUIContent[] rankingPageNames;
	
	[HideInInspector]
	public int pendingRankingListRequests = 0;
	
	private RankingPage currentPage;
	private int currentPageIndex;
	private int nextPageIndex;
	private RankingSubject currentSubject;
	private Vector2 scrollPosition;
	
	// Start
	void Start() {
		divisionPoints = leaguePoints / InGameLobby.instance.divisions.Length;
		
		// Receive RPCs from the lobby
		Lobby.AddListener(this);
		
		// Retrieve ranking lists when logging in
		AccountManager.OnAccountLoggedIn += OnAccountLoggedIn;
		
		// Init ranking lists
		GameDB.InitRankingLists();
		
		currentSubject = RankingSubject.Player;
		ChangePage(RankingPage.Best);
		
		// Empty icon
		countryCodeToIcon[""] = (Texture2D)Resources.Load("Country/empty");
	}

	// OnClick
	public override void OnClick() {
		if(pendingRankingListRequests > 0)
			return;

		Sounds.instance.PlayButtonClick();
	}

	// RequestRankingLists
	private void RequestRankingLists() {
		if(pendingRankingListRequests > 0)
			return;

		for(byte i = 0; i < GameDB.numRankingPages; i++) {
			SendRankingListRequest((byte)RankingSubject.Player, i);
		}
	}
	
	// OnAccountLoggedIn
	void OnAccountLoggedIn(Account account) {
		for(byte i = 0; i < GameDB.numRankingPages; i++) {
			this.SendRankingListRequest((byte)RankingSubject.Player, i);
		}
	}
	
	// ChangePage
	void ChangePage(RankingPage newPage) {
		currentPage = newPage;
		currentPageIndex = (int)currentPage;
	}
	
	// ChangePage
	void ChangePage(int newPage) {
		currentPage = (RankingPage)newPage;
		currentPageIndex = newPage;
	}
	
	// Rankings
	public override void Draw() {
		int currentSubjectIndex = (int)currentSubject;
		
		// Queue types
		using(new GUIHorizontalCenter()) {
			nextPageIndex = GUIHelper.Toolbar(currentPageIndex, rankingPageNames, null, GUILayout.Width(72));
			
			if(nextPageIndex != currentPageIndex) {
				ExecuteLater(() => {
					ChangePage(nextPageIndex);
				});
			}
		}
		
		RankEntry[] entries = GameDB.rankingLists[currentSubjectIndex][currentPageIndex];
		
		// We didn't receive the results yet
		if(entries == null)
			return;
		
		using(new GUIScrollView(ref scrollPosition)) {
			bool highlight = false;
			
			foreach(var entry in entries) {
				if(entry.name == InGameLobby.instance.displayedAccount.playerName) {
					highlight = true;
					break;
				}
			}
			
			for(int i = 0; i < entries.Length; i++) {
				var entry = entries[i];
				
				// TODO: Add page offset
				entry.rankIndex = i;
				
				// Highlight
				if(highlight) {
					bool isMine = (entry.name == InGameLobby.instance.displayedAccount.playerName);
					
					if(isMine) {
						GUI.color = GUIColor.RankingBGMine;
					} else {
						GUI.color = GUIColor.RankingBGOther;
					}
				}
				
				DrawRankEntry(entry);
			}
		}
		
		GUI.color = Color.white;
		GUI.contentColor = Color.white;
	}
	
	// On coming close to the NPC
	public override void OnNPCEnter() {
		RequestRankingLists();
	}
	
	// Draw a single rank entry
	void DrawRankEntry(RankEntry entry) {
		using(new GUIHorizontal("box")) {
			GUI.contentColor = Color.white;
			GUILayout.Label((entry.rankIndex + 1).ToString() + ".", rankingNumberStyle);
			
			// Load country icon if not loaded yet
			if(!countryCodeToIcon.ContainsKey(entry.country)) {
				countryCodeToIcon[entry.country] = (Texture2D)Resources.Load("Country/" + entry.country);
			}
			
			if(entry.name == InGameLobby.instance.displayedAccount.playerName) {
				GUI.contentColor = GUIColor.RankingMine;
			} else {
				GUI.contentColor = GUIColor.RankingOther;
			}
			
			// Player name
			DrawPlayerName(entry.name, new GUIContent(" " + entry.name, countryCodeToIcon[entry.country]), rankingNameStyle);
			
			GUILayout.FlexibleSpace();
			GUILayout.Label(entry.bestRanking + " p", rankingScoreStyle);
		}
	}
	
	// SendRankingListRequest
	public void SendRankingListRequest(byte subject, byte page) {
		Lobby.RPC("RankingListRequest", Lobby.lobby, subject, page);
		pendingRankingListRequests += 1;
		
		//LogManager.General.Log("Requested ranking list " + subject + ", " + page + " (" + pendingRankingListRequests + " requests pending)");
	}
	
	// GetLeagueIndex
	public static int GetLeagueIndex(int ranking) {
		return Mathf.Min(ranking / leaguePoints, 5);
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
#region RPCs
	[RPC]
	void ReceiveRankingList(byte subject, byte page, RankEntry[] nRankingEntries, bool dummy) {
		GameDB.rankingLists[subject][page] = nRankingEntries;
		pendingRankingListRequests -= 1;
		
		//LogManager.General.Log("Received ranking list " + subject + ", " + page + " (" + pendingRankingListRequests + " requests pending)" + nRankingEntries.Length);
	}
#endregion
}
