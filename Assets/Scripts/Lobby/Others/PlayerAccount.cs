using uLobby;
using UnityEngine;
using System.Collections.Generic;

public class PlayerAccount {
	public static Dictionary<string, PlayerAccount> idToAccount = new Dictionary<string, PlayerAccount>();
	public static Dictionary<string, PlayerAccount> playerNameToAccount = new Dictionary<string, PlayerAccount>();
	public static HashSet<string> namesRequested = new HashSet<string>();
	public static HashSet<string> emailsRequested = new HashSet<string>();
	public static PlayerAccount mine;
	public const string loadingSymbol = "-";

	// Account data
	public string accountId;
	public PlayerStats stats;
	public PlayerStats ffaStats;
	public CharacterCustomization custom;
	public CharacterStats charStats;
	public List<string> followersOnly;
	public GuildList guildList;
	public ArtifactTree artifactTree;
	public ArtifactInventory artifactInventory;
	public ItemInventory itemInventory;
	public int crystals;
	public AccessLevel accessLevel;
	public SkillBuild skillBuild;
	public OnlineStatus onlineStatus;

	// Private account data
	private string _playerName;
	private string _email;
	private FriendsList _friends;
	private string[] _followers;
	
	// Private constructor
	private PlayerAccount() {
		// ...
	}
	
	// Get
	public static PlayerAccount Get(string accountId) {
		PlayerAccount acc;
		
		// Load from cache or create new account
		if(!PlayerAccount.idToAccount.TryGetValue(accountId, out acc)) {
			acc = new PlayerAccount {
				accountId = accountId
			};
			
			PlayerAccount.idToAccount[accountId] = acc;
		}
		
		// Set my account
		if(PlayerAccount.mine == null) {
			PlayerAccount.mine = acc;
			
			if(InGameLobby.instance != null)
				InGameLobby.instance.displayedAccount = acc;
		}
		
		return acc;
	}
	
	// GetByPlayerName
	public static PlayerAccount GetByPlayerName(string playerName) {
		if(string.IsNullOrEmpty(playerName))
			return null;
		
		if(PlayerAccount.playerNameToAccount.ContainsKey(playerName))
			return PlayerAccount.playerNameToAccount[playerName];
		
		return null;
	}
	
	// Online status image
	public Texture2D onlineStatusImage {
		get {
			return OnlinePlayersGUI.instance.statusIcons[(int)onlineStatus];
		}
	}
	
	// Friends
	public FriendsList friends {
		get {
			return _friends;
		}
		
		set {
			_friends = value;
			UpdateFollowersOnly();
		}
	}
	
	// Followers
	public string[] followers {
		get {
			return _followers;
		}
		
		set {
			_followers = value;
			UpdateFollowersOnly();
		}
	}
	
	// RequestPlayerInfo
	public void RequestPlayerInfo() {
		Lobby.RPC("RequestPlayerInfo", Lobby.lobby, accountId);
	}
	
	// UpdateFollowersOnly
	private void UpdateFollowersOnly() {
		if(followers == null || friends == null)
			return;
		
		followersOnly = new List<string>();
		
		foreach(var followerAccountId in followers) {
			if(friends.ContainsAccount(followerAccountId))
				continue;
			
			followersOnly.Add(followerAccountId);
		}
	}
	
	// Player name
	public string playerName {
		get {
			// Request player name from lobby if it's not available
			if(string.IsNullOrEmpty(_playerName) && !namesRequested.Contains(accountId)) {
				Lobby.RPC("RequestPlayerName", Lobby.lobby, accountId);
				namesRequested.Add(accountId);
			}
			
			return _playerName;
		}
		
		set {
			if(!string.IsNullOrEmpty(_playerName))
				PlayerAccount.playerNameToAccount.Remove(_playerName);
			
			_playerName = value;
			PlayerAccount.playerNameToAccount[_playerName] = this;
		}
	}

	// Email
	public string email {
		get {
			// Request player email from lobby if it's not available
			if(string.IsNullOrEmpty(_email) && !emailsRequested.Contains(accountId)) {
				Lobby.RPC("RequestPlayerEmail", Lobby.lobby, accountId);
				emailsRequested.Add(accountId);
			}

			return _email;
		}

		set {
			_email = value;
		}
	}
	
	// Level
	public double level {
		get {
			if(stats == null || ffaStats == null)
				return 0d;
			
			return (stats.level + ffaStats.level) / 2d;
		}
	}
	
	// Character class name
	public string characterClassName {
		get {
			return "Magus";
		}
	}
	
	// League content
	public GUIContent leagueContent {
		get {
			if(stats == null)
				return new GUIContent(loadingSymbol);
			
			var leagueIndex = RankingGUI.GetLeagueIndex(stats.bestRanking);
			var content = InGameLobby.instance.leagues[leagueIndex];
			
			if(leagueIndex < InGameLobby.instance.leagues.Length - 1) {
				var nextLeagueName = InGameLobby.instance.leagues[leagueIndex + 1].text;
				var nextLeagueRankingPoints = ((int)(stats.bestRanking / RankingGUI.leaguePoints) + 1) * RankingGUI.leaguePoints - stats.bestRanking;
				content.tooltip = string.Format("<b>{0}</b> points left to reach <b>{1}</b> league", nextLeagueRankingPoints, nextLeagueName);
			}
			
			return content;
		}
	}
	
	// Division content
	public GUIContent divisionContent {
		get {
			if(stats == null)
				return new GUIContent(loadingSymbol);

			var leagueIndex = RankingGUI.GetLeagueIndex(stats.bestRanking);
			
			if(leagueIndex < 5) {
				var divisions = InGameLobby.instance.divisions;
				var content = divisions[stats.bestRanking % RankingGUI.leaguePoints / RankingGUI.divisionPoints];
				var nextDivisionRankingPoints = ((int)(stats.bestRanking / RankingGUI.divisionPoints) + 1) * RankingGUI.divisionPoints - stats.bestRanking;
				content.tooltip = string.Format("<b>{0}</b> points left to reach the next division", nextDivisionRankingPoints);
				return content;
			} else {
				return new GUIContent("-");
			}
		}
	}
	
	// Guild name
	public string guildName {
		get {
			// Still loading guild list
			if(guildList == null)
				return loadingSymbol;

			// Doesn't have a guild that is being represented
			if(string.IsNullOrEmpty(guildList.mainGuildId))
				return "-";

			// Try to get the guild object by guild ID
			Guild mainGuild;
			if(GameDB.guildIdToGuild.TryGetValue(guildList.mainGuildId, out mainGuild))
				return mainGuild.name;

			// Should it fail, display no guild
			return "-";
		}
	}
	
	// Clan name
	public string clanName {
		get {
			return "-";
		}
	}
	
	// Char stat values
	public string[] charStatValues {
		get {
			if(charStats == null || artifactTree == null)
				return new string[]{"-", "-", "-", "-", "-", "-"};
			
			var artiStats = artifactTree.charStats;
			
			return new string[] {
				(charStats.attack + artiStats.attack).ToString(),
				(charStats.defense + artiStats.defense).ToString(),
				(charStats.block + artiStats.block).ToString(),
				(charStats.cooldownReduction + artiStats.cooldownReduction).ToString(),
				(charStats.attackSpeed + artiStats.attackSpeed).ToString(),
				(charStats.moveSpeed + artiStats.moveSpeed).ToString()
			};
		}
	}
	
	// Is mine
	public bool isMine {
		get {
			return this == PlayerAccount.mine;
		}
	}
}
