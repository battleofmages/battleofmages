using uLobby;
using UnityEngine;
using System.Collections;
using System.Linq;

public class ScoreBoard : HUDElement {
	public GUIStyle partyBoxStyle;
	public GUIStyle ownScoreLineStyle;
	public GUIStyle teamScoreStyle;
	public GUIStyle scoreStyle;
	public GUIStyle nameStyle;
	
	private bool drawScoreboard;
	private int boardWidth;
	private int boardHeight;
	private int marginX = 10;
	private int columnWidth;
	
	private Transform camPivot;
	private ToggleMouseLook toggleMouseLook;
	
	private CrossHair crossHair;
	private InputManager inputManager;
	private int scoreboardButton;
	
	// Start
	void Start() {
		camPivot = GameObject.FindWithTag("CamPivot").transform;
		toggleMouseLook = camPivot.GetComponent<ToggleMouseLook>();
		crossHair = this.GetComponent<CrossHair>();
		
		// Input manager
		inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
		scoreboardButton = inputManager.GetButtonIndex("scoreboard");
	}
	
	// Update
	void Update() {
		if(GameManager.gameEnded) {
			// No mouse look
			toggleMouseLook.DisableMouseLook();
			
			EnableScoreboard();
			return;
		}
		
		if(inputManager.GetButton(scoreboardButton)) {
			if(!drawScoreboard) {
				EnableScoreboard();
			}
			
			crossHair.enabled = false;
		} else {
			if(drawScoreboard == true)
				crossHair.enabled = true;
			
			drawScoreboard = false;
		}
	}
	
	// EnableScoreboard
	void EnableScoreboard() {
		drawScoreboard = true;
	}
	
	// Draw
	public override void Draw() {
		int ptyCount = GameServerParty.partyList.Count;
		
		if(!drawScoreboard || ptyCount == 0)
			return;
		
		boardWidth = (int)GUIArea.width;
		boardHeight = (int)GUIArea.height;
		columnWidth = boardWidth / 7;
		
		GUI.BeginGroup(new Rect(GUIArea.width / 2 - boardWidth / 2, GUIArea.height / 2 - boardHeight / 2, boardWidth, boardHeight));
		//GUI.Box(new Rect(0, 0, boardWidth, boardHeight), "Scoreboard");
		
		int marginBottom = 4;
		int ptyHeight = (boardHeight - (ptyCount - 1) * marginBottom) / ptyCount;
		
		foreach(GameServerParty pty in GameServerParty.partyList) {
			using(new GUIArea(new Rect(0, pty.index * (ptyHeight + marginBottom), boardWidth, ptyHeight))) {
				DrawPartyScore(pty, boardWidth, ptyHeight);
			}
		}
		
		GUI.EndGroup();
		
		// Prevent Tab key from focusing the chat
		//if(Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t')
		//	Event.current.Use();
	}
	
	// Draws the score for one party
	void DrawPartyScore(GameServerParty pty, int width, int height) {
		GUI.color = pty.color;
		
		// Party total score
		GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 0.9f);
		GUI.Box(new Rect(0, 0, width, height), pty.name, partyBoxStyle);
		
		int memberCount = 5;
		int marginTitleBar = height / (memberCount + 2);
		int plHeight = (height - marginTitleBar * 2) / memberCount;
		int counter = 0;
		
		// Header
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.8f);
		
		/*GUILayout.BeginHorizontal();
		GUILayout.Label(" ", nameStyle);
		GUILayout.Label("Score", scoreStyle);
		GUILayout.Label("Damage", scoreStyle);
		GUILayout.Label("CC", scoreStyle);
		GUILayout.Label("Kills", scoreStyle);
		GUILayout.Label("Deaths", scoreStyle);
		GUILayout.Label("Ping", scoreStyle);
		GUILayout.Label("Ranking", scoreStyle);
		GUILayout.EndHorizontal();*/
		
		GUI.Label(new Rect(width - marginX - 6 * columnWidth, marginTitleBar, columnWidth, plHeight), "Score", scoreStyle);
		GUI.Label(new Rect(width - marginX - 5 * columnWidth, marginTitleBar, columnWidth, plHeight), "Damage", scoreStyle);
		GUI.Label(new Rect(width - marginX - 4 * columnWidth, marginTitleBar, columnWidth, plHeight), "CC", scoreStyle);
		GUI.Label(new Rect(width - marginX - 3 * columnWidth, marginTitleBar, columnWidth, plHeight), "K / D / A", scoreStyle);
		GUI.Label(new Rect(width - marginX - 2 * columnWidth, marginTitleBar, columnWidth, plHeight), "Ping", scoreStyle);
		GUI.Label(new Rect(width - marginX - columnWidth, marginTitleBar, columnWidth, plHeight), "Ranking", scoreStyle);
		
		GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		// Sort by score
		// TODO: Only sort when needed
		var sortedMemberList =
			from p in pty.members
			orderby p.score descending
			select p;
		
		foreach(Player player in sortedMemberList) {
			using(new GUIArea(new Rect(0, marginTitleBar * 2 + counter * plHeight, width, plHeight))) {
				DrawPlayerScore(player, width, plHeight);
			}
			
			counter += 1;
		}
	}
	
	// Draws the score for a single player
	void DrawPlayerScore(Player player, int width, int height) {
		/*if(player == Player.main) {
			GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		} else {
			GUI.backgroundColor = new Color(0, 0, 0, 0);
		}*/
		
		//GUILayout.BeginHorizontal(player == Player.main ? "box" : null);
		
		//nameStyle.fixedWidth = width - columnWidth * 7 - marginX;
		//scoreStyle.fixedWidth = columnWidth;
		
		/*GUILayout.Label(player.GetName(), nameStyle);
		GUILayout.Label(player.score.ToString(), scoreStyle);
		GUILayout.Label(player.dmgDealt.ToString(), scoreStyle);
		GUILayout.Label(player.ccDealt.ToString(), scoreStyle);
		GUILayout.Label(player.kills.ToString(), scoreStyle);
		GUILayout.Label(player.deaths.ToString(), scoreStyle);
		GUILayout.Label(player.pingToServer.ToString(), scoreStyle);
		GUILayout.Label(player.ranking.ToString(), scoreStyle);*/
		
		if(player == Player.main) {
			GUI.Box(new Rect(0, 0, width, height), "", ownScoreLineStyle);
		}
		
		PlayerQueueStats stats = player.stats.total;
		
		// Name should not expand to the next column
		string playerName = player.name;
		
		var textDimensions = GUI.skin.label.CalcSize(new GUIContent(playerName));
		if(textDimensions.x > columnWidth) {
			int maxNameLength = 15;
			
			if(playerName.Length > maxNameLength) {
				playerName = playerName.Substring(0, maxNameLength - 3) + "...";
			}
		}
		
		GUI.Label(new Rect(marginX, 0, width - 7 * columnWidth, height), playerName, nameStyle);
		GUI.Label(new Rect(width - marginX - 6 * columnWidth, 0, columnWidth, height), "<b>" + player.score.ToString().HumanReadableInteger() + "</b>", scoreStyle);
		GUI.Label(new Rect(width - marginX - 5 * columnWidth, 0, columnWidth, height), stats.damage.ToString().HumanReadableInteger(), scoreStyle);
		GUI.Label(new Rect(width - marginX - 4 * columnWidth, 0, columnWidth, height), stats.cc.ToStringLookup().HumanReadableInteger(), scoreStyle);
		GUI.Label(new Rect(width - marginX - 3 * columnWidth, 0, columnWidth, height), string.Concat(stats.kills.ToStringLookup(), " / ", stats.deaths.ToStringLookup(), " / ", stats.assists.ToStringLookup()), scoreStyle);
		GUI.Label(new Rect(width - marginX - 2 * columnWidth, 0, columnWidth, height), player.stats.ping.ToStringLookup().HumanReadableInteger(), scoreStyle);
		
		// Ranking change
		string rankingBonus = "";
		if(player.newBestRanking != -1) {
			int rankingDiff = player.newBestRanking - player.stats.bestRanking;
			
			if(rankingDiff > 0) {
				rankingBonus = " <color=green>+" + rankingDiff.ToStringLookup() + "</color>";
			} else if(rankingDiff < 0) {
				rankingBonus = " <color=red>" + rankingDiff.ToStringLookup() + "</color>";
			}
		}
		
		GUI.Label(new Rect(width - marginX - columnWidth, 0, columnWidth, height), player.stats.bestRanking.ToString() + rankingBonus, scoreStyle);
		
		//GUILayout.EndHorizontal();
	}
}
