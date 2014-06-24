using UnityEngine;
using System.Linq;

public abstract class PlayerOnClient : Player {
	public bool ignoreNewPosition = false;
	
	//public float movementSensitivity;
	public Transform serverPositionVisualizationPrefab;
	public Texture targetCrosshairTex;
	
	public AudioClip winSound;
	public AudioClip lossSound;
	
	[HideInInspector]
	public Entity selectedEntity;
	
	// Raycast
	[HideInInspector]
	public bool raycastSuccess;
	
	[HideInInspector]
	public RaycastHit raycastHit;

	// Components
	protected LastKills lastKills;

	// RPC names
	protected string rpcClientState = "S";
	//protected string rpcCameraState = "C";
	protected string rpcClientHitPoint = "CHP";
	
	// Reliable / unreliable switches
	//protected bool reliableCamPosSent = false;
	//protected Vector3 lastCamPosition = Cache.vector3Zero;
	protected int lastReliableBitMaskSent;
	
	protected Transform serverPositionVisualization;
	
	protected float _maxDistanceToServer = 1.1f;
	protected float maxDistanceToServerSqr;
	
	protected bool blockKeyDown;
	
	// Removes Y from hit point when player is hit
	protected Vector3 raycastHitPointOnGround;
	
	[HideInInspector]
	public Transform camTransform;
	protected Camera cam;

	protected InputManager inputManager;
	protected HumanController.InputButtons buttons;
	
	protected Color hudColor = Color.white;
	
	// Init
	protected override void Awake() {
		base.Awake();
		
		maxDistanceToServer = _maxDistanceToServer;
		
		serverPositionVisualization = (Transform)Object.Instantiate(serverPositionVisualizationPrefab, serverPosition, Cache.quaternionIdentity);
		serverPositionVisualization.gameObject.SetActive(false);
		
		// Camera
		cam = Camera.main;
		camTransform = cam.transform;
		
		if(InGameLobby.instance != null)
			lobbyChat = InGameLobby.instance.lobbyChat;

		lastKills = cam.GetComponent<LastKills>();

		// Events
		onDeath += OnDeath;

		// Loot trail
		onKill += CreateLootTrail;
	}
	
#region Update
	// FixedUpdate
	protected void FixedUpdate() {
		// Reset rotation
		ResetRotation();
		
		// Energy
		ApplyEnergyDrain();
		
		// Debuffs
		UpdateSkillEffects();
		
		// Myself
		if(networkViewIsMine) {
			// Line of sight
			camPosition = camTransform.position;
			UpdateLineOfSight(Player.allPlayers);
			UpdateLineOfSight(Enemy.allEnemies);
			
			// Weapon targeting
//			if(currentWeapon != null && currentWeapon.autoTarget && target == null) {
//				aboutToTarget = FindTarget(50);
//			}
		}
	}
#endregion
	
#region Miscellaneous
	// OnGUI
	protected void OnGUI() {
		GUI.depth = 2000;
		
		// Fade over distance
		GUI.color = hudColor;
		
		// Visibility check
		if(isVisible && isAlive && charGraphicsBody.renderer.isVisible) {
			// Health and energy bars
			entityGUI.Draw();
			
			// League medal
			if(InGameLobby.instance != null) {
				var margin = 2;
				var leagueIcon = InGameLobby.instance.leagues[RankingGUI.GetLeagueIndex(stats.bestRanking)].image;
				Rect leagueRect = nameLabel.rect;
				
				// Invert y
				leagueRect.y = Screen.height - (leagueRect.y + 1);
				leagueRect.y -= leagueRect.height - 2;
				
				leagueRect.width = leagueIcon.width;
				leagueRect.height = leagueIcon.height;
				leagueRect.x -= leagueRect.width + margin;
				
				GUI.DrawTexture(leagueRect, leagueIcon, ScaleMode.StretchToFill, true);
			}
			
			// Auto target
			if(Player.main != null && (Player.main.target == this || Player.main.aboutToTarget == this)) {
				int targetCrosshairWidth = 128;
				int targetCrosshairHeight = 128;
				
				Vector3 screenPosition = cam.WorldToScreenPoint(this.center);
				screenPosition.y = Screen.height - (screenPosition.y + 1);
				
				if(Player.main.target == this) {
					GUI.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
				} else {
					GUI.color = new Color(0.95f, 0.95f, 0.95f, 0.5f);
				}
				
				GUI.DrawTexture(new Rect(
					screenPosition.x - targetCrosshairWidth / 2,
					screenPosition.y - targetCrosshairHeight / 2,
					targetCrosshairWidth,
					targetCrosshairHeight
				), targetCrosshairTex);
			}
		}
	}

	// CreateLootTrail
	public void CreateLootTrail(Entity killer, Entity victim, short skillId) {
		Log("Creating loot trail");
		var lootTrailObject = (GameObject)Object.Instantiate(Config.instance.lootTrail, victim.center, Cache.quaternionIdentity);
		var lootTrail = lootTrailObject.GetComponent<LootTrail>();
		lootTrail.target = this;
	}
	
	// TODO: Make this a property (max distance to serverPosition)
	protected float maxDistanceToServer {
		get {
			return _maxDistanceToServer;
		}
		
		set {
			_maxDistanceToServer = value;
			maxDistanceToServerSqr = maxDistanceToServer * maxDistanceToServer;
		}
	}
#endregion
	
#region Callbacks
	// --------------------------------------------------------------------------------
	// Callbacks
	// --------------------------------------------------------------------------------
	
	// OnDeath
	void OnDeath() {
		animator.SetBool("Dead", true);
		
		Invoke("DisableCollider", Config.instance.deathColliderDisableTime);
		lastDeathTime = uLink.Network.time;
	}
#endregion
	
#region RPCs
	// Server sent us new movement data
	[RPC]
	protected void M(Vector3 newPosition, Vector3 direction, ushort rotationY, byte newBitMask, uLink.NetworkMessageInfo info) {
		if(info.timestamp <= ignoreNewPositionEarlierThanTimestamp) {
			LogSpam("Server position packet outdated, dropped!");
			return;
		}

		if(networkViewIsProxy) {
			serverPosition = newPosition + direction * moveSpeed * info.GetPacketArrivalTime() * Config.instance.serverPositionPredictionFactor;
			
			movementKeysPressed = (newBitMask & 1) != 0;
			jumping = (newBitMask & 2) != 0;
			
			proxyInterpolationTime = 0f;
			interpolationStartPosition = myTransform.position;
			//moveVector = newMoveVector;
			
			animator.SetBool("Moving", movementKeysPressed);
			animator.SetBool("Jump", jumping);
		} else {
			serverPosition = newPosition;
		}
		
		// Rotation
		serverRotationY = rotationY * Cache.rotationShortToFloat;
		
		// Visualization
		if(Debugger.instance.showServerPosition) {
			if(!serverPositionVisualization.gameObject.activeSelf) {
				serverPositionVisualization.gameObject.SetActive(Debugger.instance.showServerPosition);
			}
			
			serverPositionVisualization.position = serverPosition;
			serverPositionVisualization.rotation = Quaternion.AngleAxis(serverRotationY, Vector3.up);
		}
		
		// Track time
		//Debug.Log(info.timestampInMillis - lastPositionReceived + " ms");
		//lastPositionReceived = info.timestampInMillis;
	}
	
	[RPC]
	protected void UpdatePing(ushort nPing) {
		stats.ping = nPing;
		
		// The higher the ping, the more error in distance we can allow
		maxDistanceToServer = 0.95f + nPing * 0.03f;
	}
	
	[RPC]
	protected void UpdateStats(
		int nDmgDealt,
		ushort nCCDealt,
		byte nKills,
		byte nDeaths,
		byte nAssists,
		ushort nPing,
		int randomSeed
	) {
		PlayerQueueStats qStats;
		
		/*if(GameManager.serverType == ServerType.Arena)
			qStats = this.stats.total;
		else if(GameManager.serverType == ServerType.FFA)
			qStats = this.stats.totalFFA;
		else
			return;*/
		
		qStats = stats.total;
		
		qStats.damage = nDmgDealt;
		qStats.cc = nCCDealt;
		qStats.kills = nKills;
		qStats.deaths = nDeaths;
		qStats.assists = nAssists;
		
		UpdatePing(nPing);
		
		// Interpolation speed: Higher ping = lower interpolation speed
		/*proxyInterpolationSpeed = 30.0f - nPing * 0.02f;
		if(proxyInterpolationSpeed < 20.0f)
			proxyInterpolationSpeed = 20.0f;*/
		
		// Set random seed
		Random.seed = randomSeed;
	}
	
	[RPC]
	protected void EndGame(int winnerPartyId) {
		winnerParty = GameServerParty.partyList[winnerPartyId];
		GameManager.gameEnded = true;
		
		if(networkViewIsMine) {
			// Message in chat
			if(lobbyChat != null) {
				if(winnerParty == this.party) {
					lobbyChat.AddEntry("Your team has won the game.");
					
					// Win sound
					if(winSound != null)
						audio.PlayOneShot(winSound);
				} else {
					lobbyChat.AddEntry(winnerParty.name + " has won the game.");
					
					// Loss sound
					if(lossSound != null)
						audio.PlayOneShot(lossSound);
				}
			}
			
			// Enable return button
			GetComponent<ReturnButton>().enabled = true;
			
			LogManager.General.Log("Game ended");
		}
	}
	
	[RPC]
	protected void GameMaxScore(int maxScore) {
		var teamScore = GetComponent<TeamScore>();
		teamScore.maxScore = maxScore;
		teamScore.enabled = true;
	}
	
	[RPC]
	protected void WeaponAttunement(byte weaponId, byte attunementId) {
		weapons[weaponId].currentAttunementId = attunementId;
	}
	
	[RPC]
	protected void Chat(string entry) {
		if(lobbyChat)
			lobbyChat.AddEntry(entry);
	}
	
	[RPC]
	public void Respawn(Vector3 spawnPosition) {
		BasicRespawn(spawnPosition);
		
		// Set server position
		serverPosition = spawnPosition;
	}
#endregion
	
#region Death Review RPCs
	[RPC]
	protected void SkillDamage(short skillId, int damage) {
		//DLog(skillId + " did " + damage + " damage.");
		
		skillDamageReceived[skillId] = damage;
	}
	
	[RPC]
	protected void SkillDamageSent() {
		// Sort death review by damage, descending
		skillDamageReceived = (
			from entry in skillDamageReceived
			orderby entry.Value descending
			select entry
		).ToDictionary(
			pair => pair.Key,
			pair => pair.Value
		);
	}
#endregion
}
