using uLobby;
using UnityEngine;
using System.Collections;

public sealed class ProfileGUI : LobbyModule<ProfileGUI> {
	public GUIStyle statsStyle;
	public GUIStyle statsNameStyle;
	public GUIStyle rankingPointsStyle;
	
	public GUIContent[] profileMenuContents;
	public GUIContent[] queueContents;
	public GUIContent[] statNameContents;
	
	private Vector2 scrollPosition;
	private int profileType = 0;
	private int currentQueue = 0;
	private PlayerAccount account;
	private PlayerStats stats;
	
	// Start
	void Start() {
		// Receive lobby RPCs
		Lobby.AddListener(this);
	}
	
	// Account statistics
	public override void Draw() {
		account = InGameLobby.instance.displayedAccount;
		stats = account.stats;
		
		if(stats == null)
			return;
		
		using(new GUIScrollView(ref scrollPosition)) {
			using(new GUIVertical()) {
				using(new GUIHorizontal()) {
					using(new GUIVertical()) {
						using(new GUIHorizontalCenter()) {
							var nextProfileType = GUIHelper.Toolbar(profileType, profileMenuContents, null, GUILayout.Width(110));
							
							ExecuteLater(() => {
								profileType = nextProfileType;
							});
						}
						
						using(new GUIVertical("box")) {
							switch(profileType) {
								case 0: DrawOverview(); break;
								case 1: DrawStats(); break;
								case 2: DrawInfo(); break;
							}
							
							GUILayout.FlexibleSpace();
						}
					}
					
					using(new GUIVertical(GUILayout.Width(GUIArea.width / 2))) {
						using(new GUIHorizontalCenter()) {
							var nextQueue = GUIHelper.Toolbar(currentQueue, queueContents, null, GUILayout.Width(72));
							
							ExecuteLater(() => {
								currentQueue = nextQueue;
							});
						}
						
						using(new GUIVertical("box")) {
							DrawQueueStats(stats.queue[currentQueue]);
							GUILayout.FlexibleSpace();
						}
					}
				}
			}
		}
	}
	
	// DrawOverview
	void DrawOverview() {
		using(new GUIHorizontalCenter()) {
			GUILayout.Label("<size=50>" + stats.bestRanking + "</size> points", rankingPointsStyle);
		}
		
		// Level
		float progress = (float)account.level - (int)account.level;
		GUIHelper.ProgressBar(
			"Level: <b>" + ((int)(account.level)) + "</b>",
			progress,
			(progress * 100).ToString("0") + " %"
		);
		
		GUILayout.Space(4);
		
		using(new GUIHorizontal()) {
			using(new GUIVertical()) {
				GUILayout.Label("Class:", statsNameStyle);
				GUILayout.Label("League:", statsNameStyle);
				GUILayout.Label("Division:", statsNameStyle);
				GUILayout.Label("Guild:", statsNameStyle);
				GUILayout.Label("Clan:", statsNameStyle);
			}
			
			using(new GUIVertical()) {
				GUILayout.Label(account.characterClassName, statsStyle);
				GUILayout.Label(account.leagueContent, statsStyle);
				GUILayout.Label(account.divisionContent, statsStyle);
				GUILayout.Label(account.guildName, statsStyle);
				GUILayout.Label(account.clanName, statsStyle);
			}
		}
	}
	
	// DrawStats
	void DrawStats() {
		using(new GUIHorizontal()) {
			DrawColumn(CharacterStats.statNames);
			DrawColumn(account.charStatValues);
		}
	}
	
	// DrawInfo
	void DrawInfo() {
		
	}
	
	// DrawColumn
	void DrawColumn(string[] labels) {
		using(new GUIVertical()) {
			foreach(var label in labels) {
				GUILayout.Label(label, statsStyle);
			}
		}
	}
	
	// DrawColumn
	void DrawColumn(GUIContent[] labels) {
		using(new GUIVertical()) {
			foreach(var label in labels) {
				GUILayout.Label(label, statsStyle);
			}
		}
	}
	
	// On coming close to the NPC
	public override void OnNPCEnter() {
		InGameLobby.instance.currentLobbyModule = this;
	}
	
	// Queue statistics
	void DrawQueueStats(PlayerQueueStats qStats) {
		GUILayout.Label("       <size=50>" + qStats.ranking + "</size> points", rankingPointsStyle);
		
		using(new GUIHorizontal()) {
			// Left column
			using(new GUIVertical()) {
				GUILayout.Label("Wins / Losses:", statsNameStyle);
				GUILayout.Label(new GUIContent("Average KDA:", "Average kills / deaths / assists"), statsNameStyle);
				GUILayout.Label(new GUIContent("DPS:", "Damage per second"), statsNameStyle);
				GUILayout.Label(new GUIContent("CCPM:", "Crowd control per minute"), statsNameStyle);
				GUILayout.Label(new GUIContent("Damage Ratio:", "Damage / damage taken"), statsNameStyle);
				GUILayout.Label(new GUIContent("Hit Ratio:", "Hits / hits taken"), statsNameStyle);
				GUILayout.Label(new GUIContent("Block Ratio:", "Own successful blocks / enemies' successful blocks"), statsNameStyle);
				
				if(qStats.topScorerOwnTeam > 0)
					GUILayout.Label("Top Scorer:", statsNameStyle);
			}
			
			// Right column
			using(new GUIVertical()) {
				GUILayout.Label(qStats.wins + " / " + qStats.losses, statsStyle);
				GUILayout.Label(qStats.kdaString, statsStyle);
				GUILayout.Label(qStats.dps.ToString("0.0"), statsStyle);
				GUILayout.Label(qStats.ccpm.ToString("0.0"), statsStyle);
				GUILayout.Label(qStats.damageRatio.ToString("0.0"), statsStyle);
				GUILayout.Label(qStats.hitRatio.ToString("0.0"), statsStyle);
				GUILayout.Label(qStats.blockRatio.ToString("0.0"), statsStyle);
				
				if(qStats.topScorerOwnTeam > 0)
					GUILayout.Label(GUIHelper.Plural(qStats.topScorerOwnTeam, "time"), statsStyle);
			}
		}
	}
	
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
#region RPCs
	[RPC]
	void ReceivePlayerStats(string accountId, string jsonStats) {
		LogManager.General.Log("ProfileGUI: Received player stats!");
		
		PlayerAccount.Get(accountId).stats = Jboy.Json.ReadObject<PlayerStats>(jsonStats);
	}
	
	[RPC]
	void ReceivePlayerFFAStats(string accountId, string jsonStats) {
		LogManager.General.Log("ProfileGUI: Received player FFA stats!");
		
		PlayerAccount.Get(accountId).ffaStats = Jboy.Json.ReadObject<PlayerStats>(jsonStats);
	}
#endregion
}
