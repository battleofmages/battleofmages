using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameServerParty : Party<Entity> {
	public static List<GameServerParty> partyList = new List<GameServerParty>();
	public static Material enemySkillCircleMaterial = (Material)Resources.Load("Circles/Circle 2");
	
	private int _index;
	private int _layer;
	private Color _color;
	
	public Transform spawn;
	public Spawn spawnComp;
	public Material skillCircleMaterial;
	
	// Constructor
	public GameServerParty(string partyName, Color partyColor) : base() {
		name = partyName;
		_index = partyList.Count;
		_id = _index;
		_layer = 8 + _index;
		_color = partyColor;
		spawn = null;
		spawnComp = null;
		skillCircleMaterial = (Material)Resources.Load("Circles/Circle " + (_index + 1));
		partyList.Add(this);
	}
	
	// Update spawn position
	public void UpdateSpawn() {
		var gameObject = GameObject.Find("Spawn " + (_index + 1));
		
		if(gameObject == null) {
			LogManager.General.LogError("Couldn't find Spawn " + (_index + 1));
			return;
		}
		
		spawn = gameObject.transform;
		spawnComp = spawn.GetComponent<Spawn>();
	}
	
	// TODO: Make use of partyCount
	public static void CreateParties(int partyCount, int expectedMemberCount = 0) {
		for(int i = 0; i < partyCount; i++) {
			Color color = Color.black;
			
			// Not on server side
			if(ServerInit.instance == null) {
				color = PartyColors.instance.colors[i];
			}
			
			var pty = new GameServerParty("Team " + (i + 1), color);
			pty.expectedMemberCount = expectedMemberCount;
		}
	}
	
	// Update spawns of all parties
	public static void UpdateSpawns() {
		LogManager.General.Log("Updating party spawns...");
		
		foreach(GameServerParty pty in GameServerParty.partyList) {
			pty.UpdateSpawn();
		}

		if(Player.main != null) {
			// When the player has a party update his camera rotation
			Player.main.UpdateCameraYRotation();
		}
		
		LogManager.General.Log("Updated party spawns");
	}
	
	// Get the top scorer among all teams
	public static Entity GetTopScorerAllTeams() {
		Entity winner = null;
		int highestScore = -1;
		
		foreach(var pty in GameServerParty.partyList) {
			foreach(var member in pty.members) {
				int memberScore = member.score;
				
				if(memberScore > highestScore) {
					winner = member;
					highestScore = memberScore;
				}
			}
		}
		
		return winner;
	}
	
	// Are all parties ready?
	public static bool AllPartiesReady() {
		if(partyList.Count == 0)
			return false;
		
		foreach(var pty in partyList) {
			if(!pty.isReady)
				return false;
		}
		
		return true;
	}
	
#region Properties
	// Party color
	public Color color {
		get { return _color; }
	}
	
	// Collision layer
	public int layer {
		get { return _layer; }
		set { _layer = value; }
	}
	
	// Index
	public int index {
		get { return _index; }
	}
	
	// Party score
	public int score {
		get {
			int totalScore = 0;
			
			foreach(var member in _members) {
				totalScore += member.score;
			}
			
			return totalScore;
		}
	}
	
	// Top scorer in this party
	public Entity topScorer {
		get {
			Entity winner = null;
			int highestScore = -1;
			
			foreach(Entity member in _members) {
				int memberScore = member.score;
				
				if(memberScore > highestScore) {
					winner = member;
					highestScore = memberScore;
				}
			}
			
			return winner;
		}
	}
	
	// Are all party members ready?
	public bool isReady {
		get {
			if(this.members.Count != expectedMemberCount)
				return false;
			
			foreach(Entity member in this.members) {
				if(!member.isReady)
					return false;
			}
			
			return true;
		}
	}
#endregion
}