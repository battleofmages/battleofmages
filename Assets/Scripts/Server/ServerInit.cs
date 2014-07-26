using uLink;
using uGameDB;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerInit : uLink.MonoBehaviour {
	public static ServerInit instance = null;
	
	public ServerType testServerType;
	
	// Map configuration
	public string mapName;
	
	public bool isTestServer {
		get;
		protected set;
	}
	
	// Misc
	public int serverFrameRate;
	public int defaultServerPort;
	
	//************************************************************************************************
	// Owner is the actual player using the client Scene. It has animations + camera.  
	//
	// Proxy is what appears on the the opponent players computers, It has animations but no camera connection.
	//
	// Creator is instantiated on the server and it has no camera.
	//************************************************************************************************
	public GameObject proxyPrefab = null;
	public GameObject ownerPrefab = null;
	public GameObject creatorPrefab = null;
	
	// Assigns every account ID a party
	public static Dictionary<string, GameServerParty> accountToParty = new Dictionary<string, GameServerParty>();
	
	[HideInInspector]
	public bool restrictedAccounts;
	
	private bool batchMode;
	private int serverPort;
	private int partyCount;
	
	public GameMode gameMode { get; protected set; }
	
	// Awake
	void Awake() {
		// TODO: Checks...
		instance = this;
		
		// This will be reset later if -batchmode was specified
#if UNITY_STANDALONE_WIN
		isTestServer = true;
#else
		isTestServer = false;
#endif
		
		// Default values
		serverPort = defaultServerPort;
		partyCount = 1;
		
		// -party0 account1 -party1 account2
		// -batchmode -port7100
		// taskkill /IM
		LogManager.General.Log("Parsing command line arguments");
		string[] args = System.Environment.GetCommandLineArgs();
		int partyId = GameServerParty.Undefined;
		
		foreach(string arg in args) {
			LogManager.General.Log("Command line argument: '" + arg + "'");

			// Overwrite port
			if(arg.StartsWith("-port") && arg.Length > 5) {
				serverPort = int.Parse(arg.Substring(5));
			// Batchmode
			} else if(arg == "-batchmode") {
				batchMode = true;
				isTestServer = false;
			// Party count
			} else if(arg.StartsWith("-partycount") && arg.Length > 6) {
				partyCount = int.Parse(arg.Substring("-partycount".Length));

				LogManager.General.Log(string.Format("Creating parties: {0}", partyCount));
				GameServerParty.partyList.Clear();
				GameServerParty.CreateParties(partyCount);
			// Teams
			} else if(arg.StartsWith("-party") && arg.Length > 6) {
				partyId = int.Parse(arg.Substring("-party".Length));
				restrictedAccounts = true;
			// Map
			} else if(arg.StartsWith("-map") && arg.Length > 4) {
				mapName = arg.Substring("-map".Length);
			// Server type
			} else if(arg.StartsWith("-type") && arg.Length > 5) {
				string serverTypeString = arg.Substring("-type".Length);
				switch(serverTypeString) {
					case "Arena":
						GameManager.serverType = ServerType.Arena;
						break;

					case "Town":
						GameManager.serverType = ServerType.Town;
						break;

					case "FFA":
						GameManager.serverType = ServerType.FFA;
						break;

					case "World":
						GameManager.serverType = ServerType.World;
						break;
				}
			// Account ID
			} else {
				if(partyId >= 0 && partyId < GameServerParty.partyList.Count) {
					var currentParty = GameServerParty.partyList[partyId];
					accountToParty[arg] = currentParty;
					currentParty.expectedMemberCount += 1;
				}
			}
		}

		// For testing
		if(isTestServer)
			GameManager.serverType = testServerType;

		// Create at least 1 party if no party count has been specified
		if(GameServerParty.partyList.Count != partyCount) {
			switch(GameManager.serverType) {
				case ServerType.Arena:
					partyCount = 2;
					break;
					
				case ServerType.FFA:
					partyCount = 10;
					break;
			}

			LogManager.General.Log(string.Format("Creating parties: {0}", partyCount));
			GameServerParty.partyList.Clear();
			GameServerParty.CreateParties(partyCount);
		}

		// Server type
		LogManager.General.Log("Server type: " + GameManager.serverType);
		MapManager.InitPhysics(GameManager.serverType);
		
		if(restrictedAccounts) {
			LogManager.General.Log("Server is restricted to the following accounts: " + accountToParty.Keys);
		}
		
		if(GameManager.isArena) {
			QueueSettings.queueIndex = accountToParty.Count / 2 - 1;
			LogManager.General.Log("Queue type is: " + (QueueSettings.queueIndex + 1) + "v" + (QueueSettings.queueIndex + 1));
		} else if(GameManager.isFFA) {
			QueueSettings.queueIndex = 0;
			LogManager.General.Log("Queue type is: " + (QueueSettings.queueIndex + 1) + "v" + (QueueSettings.queueIndex + 1));
		}
		
		// Server batchmode
		//if(batchMode) {
		LogManager.General.Log("Batchmode is being used: Destroy the camera and disable renderers");
		
		// Destroy the camera
		Destroy(Camera.main);
		
		// Disable renderers in batchmode
		DisableRenderers(creatorPrefab);
		
		// No statistics GUI
#if UNITY_EDITOR
		GetComponent<uLinkStatisticsGUI>().enabled = true;
#endif
		
		// No audio
		AudioListener.pause = true;
		
		// Disable all game modes just to be safe
		var gameModes = GetComponents<GameMode>();
		foreach(var mode in gameModes) {
			mode.enabled = false;
		}
		
		// Pick correct game mode
		int maxPlayerCount = 0;
		switch(GameManager.serverType) {
			case ServerType.Arena:
				gameMode = GetComponent<ArenaGameMode>();
				int maxSpectators = 10;
				maxPlayerCount = 10 + maxSpectators;
				break;

			case ServerType.Town:
				gameMode = GetComponent<TownGameMode>();
				maxPlayerCount = 1024;
				break;

			case ServerType.FFA:
				gameMode = GetComponent<FFAGameMode>();
				maxPlayerCount = 10;
				break;

			case ServerType.World:
				gameMode = GetComponent<WorldGameMode>();
				maxPlayerCount = 1024;
				break;
		}
		
		// FFA
		if(GameManager.isFFA) {
			GameServerParty.partyList.Clear();
			GameServerParty.CreateParties(10, 1);
		}
		
		// Load map
		StartCoroutine(MapManager.LoadMapAsync(
			mapName,

			// Map is loaded asynchronously...
			// When it's finished, we use the callback:
			() => {
				// Register codecs for serialization
				GameDB.InitCodecs();
				
				// Init uZone
				if(uZone.Instance.wasStartedByuZone) {
					uZone.Instance.GlobalEvents events = new uZone.Instance.GlobalEvents();
					events.onInitialized = uZone_OnInitialized;
					
					// This will tell uZone that the instance is ready
					// and we can let other players connect to it
					LogManager.General.Log("Initializing the uZone instance");
					uZone.Instance.Initialize(events);
				}
				
				// Server port
				if(!isTestServer) {
					try {
						serverPort = uZone.Instance.port;
						LogManager.General.Log("Using port assigned from uZone: " + serverPort);
					} catch {
						LogManager.General.Log("Failed to retrieve port info from uZone! Using port " + serverPort);
					}
				} else {
					LogManager.General.Log("Using test server port: " + serverPort);
				}
				
				// Init server
				LogManager.General.Log("Initializing the server on port " + serverPort);
				uLink.Network.InitializeServer(maxPlayerCount, serverPort);
				
				// Encryption
				LogManager.General.Log("Initializing security");
				uLink.Network.InitializeSecurity(true);
				
#if !UNITY_EDITOR
				// Clean up
				DestroyServerAssets();
#endif
			}
		));
	}
	
	// InstantiatePlayer
	void InstantiatePlayer(uLink.NetworkPlayer networkPlayer, string accountId, string playerName, PlayerStats stats) {
		LogManager.General.Log(string.Format("Instantiating player prefabs for '{0}' with account ID '{1}'", accountId, playerName));
		
		// Instantiates an avatar for the player connecting to the server
		// The player will be the "owner" of this object. Read the manual chapter 7 for more
		// info about object roles: Creator, Owner and Proxy.
		GameObject obj = uLink.Network.Instantiate(
			networkPlayer,		// Owner
			proxyPrefab,
			ownerPrefab,
			creatorPrefab,
			transform.position,
			transform.rotation,
			0,					// Network group
			accountId			// Initial data
		);
		
		// Player component
		Player player = obj.GetComponent<Player>();
		player.accountId = accountId;
		networkPlayer.localData = player;
		
		// Async: DB requests
		if(isTestServer) {
			// This is for quick client tests
			// Send other players and myself information about stats
			player.skillBuild = SkillBuild.GetStarterBuild();
			player.customization = new CharacterCustomization();
			
			player.networkView.RPC("ReceiveSkillBuild", uLink.RPCMode.All, player.skillBuild);
			player.networkView.RPC("ReceiveCharacterCustomization", uLink.RPCMode.All, player.customization);
			player.networkView.RPC("ReceiveCharacterStats", uLink.RPCMode.All, new CharacterStats());
			player.networkView.RPC("ReceiveArtifactTree", uLink.RPCMode.All, Jboy.Json.WriteObject(ArtifactTree.GetStarterArtifactTree()));
			
			// After the skill build has been sent, switch the attunement
			player.networkView.RPC("SwitchWeapon", uLink.RPCMode.All, (byte)0);
			player.networkView.RPC("SwitchAttunement", uLink.RPCMode.All, (byte)0);
		} else {
			// TODO: We need to wait until this is finished in ApplyCharacterStats
			// Skill build
			SkillBuildsDB.GetSkillBuild(
				accountId,
				data => {
					if(data == null) {
						player.skillBuild = SkillBuild.GetStarterBuild();
					} else {
						player.skillBuild = data;
					}
					
					// Send build
					player.networkView.RPC("ReceiveSkillBuild", uLink.RPCMode.All, player.skillBuild);
					
					// After the build has been sent, switch the attunement
					player.networkView.RPC("SwitchWeapon", uLink.RPCMode.All, (byte)0);
					player.networkView.RPC("SwitchAttunement", uLink.RPCMode.All, (byte)0);
				}
			);

			// Character customization
			CharacterCustomizationDB.GetCharacterCustomization(
				accountId,
				data => {
					if(data == null) {
						player.customization = new CharacterCustomization();
					} else {
						player.customization = data;
					}
					
					// Send customization
					player.networkView.RPC("ReceiveCharacterCustomization", uLink.RPCMode.All, player.customization);
				}
			);
			
			// Character stats
			StartCoroutine(ServerGameDB.GetCharacterStats(player));
			
			// Guild
			GuildsDB.GetGuildList(accountId, data => {
				if(data != null) {
					GuildsDB.GetGuild(data.mainGuildId, guild => {
						if(guild != null) {
							player.networkView.RPC("ReceiveMainGuildInfo", uLink.RPCMode.All, guild.name, guild.tag);
						}
					});
				}
			});
			
			// Artifacts
			ArtifactsDB.GetArtifactTree(
				accountId,
				data => {
					if(data == null) {
						player.artifactTree = ArtifactTree.GetStarterArtifactTree();
					} else {
						player.artifactTree = data;
					}
					
					player.networkView.RPC("ReceiveArtifactTree", uLink.RPCMode.All, Jboy.Json.WriteObject(player.artifactTree));
				}
			);
		}
		
		// Assign party
		int partyId;
		
		if(GameManager.isTown) {
			partyId = 0;
		} else if(GameManager.isFFA) {
			partyId = FindFFAPartyWithLeastMembers();
		} else {
			if(restrictedAccounts) {
				partyId = accountToParty[accountId].id;
			} else {
				partyId = FindPartyWithLeastMembers();
			}
		}
		
		if(GameManager.isArena)
			player.networkView.RPC("GameMaxScore", uLink.RPCMode.Owner, gameMode.scoreNeededToWin);

		// Data all players need to know about the new player
		player.networkView.RPC("ReceivePlayerInfo", uLink.RPCMode.All, playerName, stats.bestRanking);
		
		if(!GameManager.isTown) {
			player.networkView.RPC("ChangeParty", uLink.RPCMode.All, partyId);
		}
		
		var party = GameServerParty.partyList[partyId];
		player.networkView.RPC("ChangeLayer", uLink.RPCMode.All, party.layer);

		// Respawn position
		if(GameManager.isPvE) {
			PortalDB.GetPortal(accountId, portalInfo => {
				// Player did not come via a portal
				if(portalInfo == null) {
					PositionsDB.GetPosition(accountId, data => {
						Vector3 respawnPosition;

						if(data != null) {
							respawnPosition = data.ToVector3();
							LogManager.General.Log("Found player position: Respawning at " + respawnPosition);
						} else {
							respawnPosition = party.spawnComp.GetNextSpawnPosition();
							LogManager.General.Log("Couldn't find player position: Respawning at " + respawnPosition);
						}
						
						player.networkView.RPC("Respawn", uLink.RPCMode.All, respawnPosition);
					});
				// Player did come via a portal
				} else {
					Vector3 respawnPosition;
					var portals = GameObject.FindGameObjectsWithTag("Portal");

					foreach(var portalObject in portals) {
						var portal = portalObject.GetComponent<Portal>();

						if(portal.portalId == portalInfo.id) {
							respawnPosition = portal.spawns[Random.Range(0, portal.spawns.Length - 1)].position;

							// Respawn
							LogManager.General.Log("Player came via a portal: Respawning at " + respawnPosition);
							player.networkView.RPC("Respawn", uLink.RPCMode.All, respawnPosition);

							// Update position to be 100% sure our position data is correct now
							PositionsDB.SetPosition(accountId, respawnPosition);
							
							// Delete portal info so we won't use it again
							PortalDB.RemovePortal(accountId);

							break;
						}
					}
				}
			});
		} else {
			var respawnPosition = party.spawnComp.GetNextSpawnPosition();

			LogManager.General.Log("PvP game: Respawning at " + respawnPosition);
			player.networkView.RPC("Respawn", uLink.RPCMode.All, respawnPosition);
		}

		//player.networkView.RPC("SetCameraYRotation", uLink.RPCMode.Owner, party.spawnComp.transform.eulerAngles.y);
		
		// On non account restricted servers we start the game instantly
		if(!GameManager.isArena || isTestServer) {
			player.networkView.RPC("StartGame", uLink.RPCMode.Owner);
			GameManager.gameStarted = true;
		}
		
		// Disable encryption in non-ranked games
		if(!GameManager.isRankedGame)
			uLink.Network.UninitializeSecurity(networkPlayer);
	}
	
	// Retrieves the player information
	IEnumerator RetrievePlayerInformation(uLink.NetworkPlayer networkPlayer, string accountId) {
		LogManager.General.Log("Retrieving player information for account " + accountId);
		
		var bucket = new Bucket("AccountToName");
		var request = bucket.Get(accountId);
		yield return request.WaitUntilDone();
		
		if(request.isSuccessful) {
			string playerName = request.GetValue<string>();
			
			LogManager.General.Log("Queried player name of '" + accountId + "' successfully: " + playerName);
			
			// Retrieve stats
			var statsBucket = new Bucket("AccountToStats");
			var statsRequest = statsBucket.Get(accountId);
			yield return statsRequest.WaitUntilDone();
			
			PlayerStats statsInDB;
			
			if(statsRequest.isSuccessful) {
				statsInDB = statsRequest.GetValue<PlayerStats>();
				
				LogManager.General.Log("Queried stats of account '" + accountId + "' successfully (Ranking: " + statsInDB.bestRanking + ")");
			} else {
				statsInDB = new PlayerStats();
				
				LogManager.General.Log("Account '" + accountId + "' aka player '" + playerName + "' doesn't have any player stats yet");
			}
			
			// After we got the name, instantiate it
			InstantiatePlayer(networkPlayer, accountId, playerName, statsInDB);
		} else {
			LogManager.General.LogWarning("Account " + accountId + " doesn't have a player name.");
		}
	}
	
	// Arena test server
	int FindPartyWithLeastMembers() {
		int partyId = 0;
		int lowestMembers = 1000000;
		int memberCount;
		
		for(byte i = 0; i < GameServerParty.partyList.Count; i++) {
			memberCount = GameServerParty.partyList[i].memberCount;
			
			if(memberCount < lowestMembers) {
				partyId = i;
				lowestMembers = memberCount;
			}
		}
		
		return partyId;
	}
	
	// FindFFAPartyWithLeastMembers
	int FindFFAPartyWithLeastMembers() {
		int partyId = GameServerParty.Undefined;
		int lowestMembers = 1000000;
		int memberCount;
		
		for(int i = 0; i < GameServerParty.partyList.Count; i++) {
			var pty = GameServerParty.partyList[i];
			memberCount = pty.memberCount;
			
			if(memberCount < pty.expectedMemberCount && memberCount < lowestMembers) {
				partyId = i;
				lowestMembers = memberCount;
			}
		}
		
		// On FFA servers we have a fixed number of parties now
		/*if(partyId == Party.Undefined) {
			
		}*/
		
		return partyId;
	}
	
	// SendGameStart
	public void SendGameStart() {
		gameMode.SendGameStart();
	}
	
	// SendGameStartCountdown
	public void SendGameStartCountdown() {
		gameMode.SendGameStartCountdown();
	}
	
	// GUI
	void OnGUI() {
		if(!batchMode) {
			if(uZone.Instance.wasStartedByuZone && uZone.Instance.isConnected)
				GUI.Label(new Rect(5, 25, 200, 25), "Connected to uZone");
			else
				GUI.Label(new Rect(5, 25, 200, 25), "Not connected to uZone");
		}
	}
	
	// Used to fix a memory leak in batchmode
	public static void DisableRenderers(GameObject obj) {
		Component[] renderers;
		renderers = obj.GetComponentsInChildren<Renderer>();
		
		foreach(Renderer rend in renderers) {
			rend.enabled = false;
		}
	}
	
	// DO NOT CALL THIS IN THE EDITOR
	public void DestroyServerAssets() {
		LogManager.General.Log("Going to destroy unneeded server assets");
		
		/*// Remove all textures
		var allTextures = Resources.FindObjectsOfTypeAll(typeof(Texture));
		LogManager.General.Log(allTextures.Length.ToString() + " textures loaded, going to destroy.");
		foreach(var obj in allTextures) {
			Destroy(obj);
		}
		LogManager.General.Log("Textures destroyed.");
		
		// Remove all audio clips
		var allAudioClips = Resources.FindObjectsOfTypeAll(typeof(AudioClip));
		LogManager.General.Log(allAudioClips.Length.ToString() + " audio clips loaded, going to destroy.");
		foreach(var obj in allAudioClips) {
			Destroy(obj);
		}
		LogManager.General.Log("Audio clips destroyed.");
		
		// Remove all materials
		var allMaterials = Resources.FindObjectsOfTypeAll(typeof(Material));
		LogManager.General.Log(allMaterials.Length.ToString() + " materials loaded, going to destroy.");
		foreach(var obj in allMaterials) {
			Destroy(obj);
		}
		LogManager.General.Log("Materials destroyed.");*/
		
		// Remove occlusion area data
		if(MapManager.occlusionArea) {
			Destroy(MapManager.occlusionArea);
			MapManager.occlusionArea = null;
		}
		
		// Try to free up some RAM
		PerformanceMonitor.FreeRAM();
		
		// DO NOT DELETE THE MESHES, YOU WILL GET PROBLEMS
	}
	
	// --------------------------------------------------------------------------------
	// Callbacks
	// --------------------------------------------------------------------------------
	
	// Server successfully initialized
	void uLink_OnServerInitialized() {
		LogManager.General.Log("Server successfully started!");
		
		// Set private key
		ServerInit.InitPrivateKey();
		
		// Enable game mode
		gameMode.enabled = true;
		
		// Lower CPU consumption
		LogManager.General.Log("Setting frame rate to " + serverFrameRate);
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = serverFrameRate;//(int)uLink.Network.sendRate * 2;
		
		// Max score needed
		if(GameManager.isArena && !isTestServer) {
			int playerCount = accountToParty.Count;
			gameMode.scoreNeededToWin = playerCount / 2 * 500;
			LogManager.General.Log("A team score of " + gameMode.scoreNeededToWin + " is needed to win the game.");
		} else {
			gameMode.scoreNeededToWin = 999999999;
		}
		
		// Start conditions
		if(GameManager.isTown || GameManager.isFFA || isTestServer) {
			this.SendGameStart();
		}
		
		// Disable network emulation on server
		LogManager.General.Log("Disabling network emulation");
		NetworkHelper.DisableNetworkEmulation();
		
		// Fastest quality level on server
		LogManager.General.Log("Enabling lowest quality level");
		QualitySettings.SetQualityLevel(0);
	}

	// Player tries to connect
	void uLink_OnPlayerApproval(NetworkPlayerApproval approval) {
		LogManager.General.Log("A player is trying to connect...check for approval");
		
		string accountId;
		bool accountIdAvailable = approval.loginData.TryRead<string>(out accountId);
		
		// Account ID was sent
		if(restrictedAccounts) {
			if(accountIdAvailable) {
				// Is the account in our account list
				if(accountToParty.ContainsKey(accountId)) {
					approval.Approve();
				} else {
					LogManager.General.Log("Denied client because the account ID '" + accountId + "' doesn't appear in the list.");
					approval.Deny();
				}
			// Client didn't send an account ID
			} else {
				LogManager.General.Log("Denied client because he didn't send any account information.");
				approval.Deny();
			// Client isn't restricted to certain accounts (test client)
			}
		} else {
			LogManager.General.Log(string.Format("Approved client with ID '{0}' because server is not account restricted.", accountId));
			approval.Approve();
		}
	}
	
	// Player connected
	void uLink_OnPlayerConnected(uLink.NetworkPlayer netPlayer) {
		LogManager.General.Log("Player successfully connected from " + netPlayer.ipAddress + ":" + netPlayer.port);
		
		// Instantly let him know about the server type
		networkView.RPC("ReceiveServerType", netPlayer, GameManager.serverType);
		networkView.RPC("LoadMap", netPlayer, mapName);
		
		string accountId;
		string playerName;
		
		if(!netPlayer.loginData.TryRead<string>(out accountId))
			playerName = "Client " + netPlayer.id;
		else
			playerName = accountId;
		
		// Retrieve player info
		if(isTestServer) {
			InstantiatePlayer(netPlayer, "", playerName, new PlayerStats());
		} else {
			StartCoroutine(RetrievePlayerInformation(netPlayer, accountId));
		}
	}
	
	// Player disconnected
	void uLink_OnPlayerDisconnected(uLink.NetworkPlayer netPlayer) {
		LogManager.General.Log("Player " + netPlayer.id + " disconnected!");
		
		// Save FFA stats
		if(GameManager.isFFA) {
			var player = (Player)netPlayer.localData;
			
			if(player != null)
				gameMode.SendPlayerFFAStats(player);
			else
				LogManager.DB.LogError("Couldn't find player by ID " + netPlayer.id + ". FFA stats won't be saved.");
			
			/*string accountId = "";
			if(netPlayer.loginData.TryRead<string>(out accountId)) {
				LogManager.General.Log("Disconnected player " + netPlayer.id + " had account ID '" + accountId + "'!");
				
				var player = Player.FindPlayerByAccountId(accountId);
				
				if(player != null)
					gameMode.SendPlayerFFAStats(player);
				else
					LogManager.DB.LogError("Couldn't find player by account ID " + accountId + ". FFA stats won't be saved.");
			} else {
				LogManager.DB.LogError("Couldn't get account ID of player " + netPlayer.id + ". FFA stats won't be saved.");
			}*/
		}
		
		uLink.Network.DestroyPlayerObjects(netPlayer);
		uLink.Network.RemoveRPCs(netPlayer);
	}

	// uZone initialized
	void uZone_OnInitialized(uZone.InstanceID assignedId) {
		LogManager.General.Log("Instance successfully initialized and connected to its parent node, id: " + assignedId);
	}
	
	// On application quit we close log files
	void OnApplicationQuit() {
		LogManager.CloseAll();
	}
	
	// --------------------------------------------------------------------------------
	// Security
	// --------------------------------------------------------------------------------

	// InitPrivateKey
	public static void InitPrivateKey() {
		// TODO: Load private key dynamically from a file
		LogManager.General.Log("Initializing private key");
		uLink.Network.privateKey = new uLink.PrivateKey(
@"r6tfUZ4YwT16YA4GGXN7xdd1A5rTIwSi5Yn6euIGK/Z0WrTkkgBVHnMtxFLtqmh8kh
aUbPeoIIU5C/zwZitj5Ef7pd91LTrabTIDd4T9V/eMo2wHXfOxLJm6oC372pvMFQKL
Cr4/8FWgrh1kGpVXYg3a5HzckLpC286Y7b07g7aAJJv/WcUJfuxtmR+0tN6eUs1evD
qFK2VRdmlB9sWSpUDZ/5LGdWBof2MDLFBfKZ7pg/av6fXKrfufgXRHu/7sn/Awrysr
3Lt+Ajj3f/9pOE39mwIqioSUjBjXoh9N+zacGxv+iKlPOQsGMs8kqTpVHRrjUs8BHh
uzf/lC9NYarw==", @"EQ==", @"zSlbiDsMfiApy6/QXugNNh3/nChL9dSgJCrToc
3f+CR6RZCUO9DnpChW6mh4TSzmj6pujDWiGAc7IXYg/Ufe+zjAMN8a5gHBuPN/PBP2
d+SPooIFMdjc2mVEEXWMfzMlmnRMTWe45oiBDOr0r60MjHQkd/kbNHiWCB39Qj++Ev
8=", @"2zMmZzIuY8kdbWafZrlDNJv/4dbl7po38pBpEAah+d3GrPrwHyFqrfST5uS
RVuNZkmg2yns0oA4icrqtnZPQoChJU3LfTGayhYL+YEi3coVdq84BvIagFHDIgtdiO
jig5Mx+k1czoxmXmzPbAFVejE2diBCzXWowAs6FmVaF6FE=", @"nONkHOHcYHLyyO
/bk96gsOnDd2ob+DkvKrdWisqcNjoDJiNELb3eUFsVSddrDdcKqhjrH+zHP40PGZaR
sqBfOJTPNG5f3RBm9thSPQA08kVex5Caj4e38k1wSZYgJQj+o0nf/vT2zmhir4Z+4L
GCEQ2FTK9vKB/6QnFJMqk2/3c=", @"tIR5+qHL2bSu0pC/gcW+4Af/5ylyD8pMMSu
h0PZnRj4rJQrj3WbQUwWm+mHhGmDgWnPw4vwNOIQcXnueCUyNsRIeRLj0IM0LfQJ3I
h3EQDGYb15byG7eLvN380f2ikzAvGwsAN5mwo2L6TnDaa+3Rl4JQuCTtldy1SKMI+z
mv1E=", @"Uy8Nk2p+jc5tvLJyg8Tw1LyBREC3WL5x7h6n8b0a2TtxXVTJdYPtPBxQ
FnWRLBiPf5vxTmXJPYDbSX9VvY6X8Yex4Jqb2IZx+FYmgqKgza0Rccr03hdG6hRVul
ds1MBDaErC+sO3aaAXM0PsHYyYjgfuIirz559FIdQfAsoW750=", @"M6rfzLYHR+T
nweYBy0AVWE5ttcQf+z2ZUpH+YGCYZ0h8kyYlG+H66taU/YHNblr3dkLgXEjXGJ+nX
eD7aVgOUjNKA5wTZ6fl4+GIjJCGv4ULPx/F7nTZwdLciXbvuMRpM1sZ1fu4c9zzBgi
k+MKDHNbmBveMKpEiuR6lcxmJ+Y+KwmIErvt1DeVLrfeCjUfxDtMj+1zj+ziYzaOx6
/XF1ZaPXl6YF3ydLAU72EH6yzn+bJ90mFRu4ZflNdZgBbz5xzTJSJXvBBT0koJmGXJ
SeO9CnZUv0GupNYsi3NINk5lx5dufWXPBpN3fb0siTy3kk1Zg91p7EXOL7yLTG9Cf0
Q==");
	}
}