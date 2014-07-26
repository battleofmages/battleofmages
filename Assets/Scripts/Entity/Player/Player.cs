using UnityEngine;
using System.Collections.Generic;

public abstract class Player : Entity {
	// Static
	public static PlayerMain main = null;
	public static Transform playerRoot;
	public static int count = 0;
	public static Dictionary<string, Player> accountIdToPlayer = new Dictionary<string, Player>();
	public static List<Player> allPlayers = new List<Player>();
	public static bool showAllEntities = false;
	
	// Inspector
	public AudioSource movementSoundsSource;
	public AudioClip flightSound;
	public float flightSoundFadeTime;

	// ChangeParty event
	public event CallBack onChangeParty;

	// Respawn event
	public delegate void RespawnHandler(Vector3 position);
	public event RespawnHandler onRespawn;

	// Cam position
	protected Vector3 camPosition;
	
	// Movement
	[HideInInspector]
	public bool movementKeysPressed = false;
	
	[HideInInspector]
	public Vector3 moveVector;
	
	protected bool jumpButtonPressed;
	protected bool lastJumpSent;
	
	// Misc
	protected bool lastBlockStateSent;
	protected bool noTargetSent;
	protected bool appliedCharacterStats;

	// Components
	protected LobbyChat lobbyChat;

	// Account
	[HideInInspector]
	public PlayerAccount account;
	
	[HideInInspector]
	public ArtifactTree artifactTree = null;
	
	[HideInInspector]
	public Artifact artifactReward;
	
	[HideInInspector]
	public CharacterCustomization customization;
	
	// Server side data
	[HideInInspector]
	public string accountId;
	
	// NPC
	[HideInInspector]
	public NPC talkingWithNPC;
	
	private CastEffect castEffect;
	
	[HideInInspector]
	public byte currentAttunementId;
	
	[HideInInspector]
	public int newBestRanking = -1;
	
	[HideInInspector]
	public double lastDeathTime;
	
	[HideInInspector]
	public CharacterMotor motor;

	protected VoIP voIP;
	
	protected float motorGravity;
	protected float motorAirAcceleration;
	
	// Local references
	[HideInInspector]
	public GameServerParty winnerParty;
	
	protected GameObject attunementVisuals;
	
	// LoS
	[HideInInspector]
	public Dictionary<Entity, bool> playerVisibility;
	
	// Awake
	protected override void Awake() {
		base.Awake();
		
		baseMoveSpeed = Config.instance.playerMoveSpeed;
		
		// Parent
		myTransform.parent = Player.playerRoot;
		
		// Reset customization
		customization = null;
		
		// Full block duration
		playerVisibility = new Dictionary<Entity, bool>();
		
		this.skillBuild = null;
		this.charStats = null;
		this.artifactTree = null;
		this.artifactReward = null;
		
		// Components
		motor = GetComponent<CharacterMotor>();
		comboCounter = GetComponent<ComboCounter>();
		voIP = GetComponent<VoIP>();
		
		// Used for hovering / flying
		if(motor != null) {
			motorGravity = motor.movement.gravity;
			motorAirAcceleration = motor.movement.maxAirAcceleration;
		}

		charGraphicsModel = InstantiateChild(Config.instance.femalePrefab, charGraphics);

		// Destroy
		onDestroy += () => {
			// Remove from global lists
			Player.accountIdToPlayer.Remove(this.accountId);
			Player.allPlayers.Remove(this);
			Entity.idToEntity.Remove(this.id);
			
			// In case we stored some old pointers, react as if he is dead.
			// Particularly useful for the Enemy threat based target finding.
			health = 0;
		};
		
		// Update player count
		Player.allPlayers.Add(this);
	}
	
	// On network instantiation
	protected override void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info) {
		base.uLink_OnNetworkInstantiate(info);

		if(info.networkView.initialData.TryRead<string>(out accountId)) {
			account = PlayerAccount.Get(accountId);
			Player.accountIdToPlayer[accountId] = this;
		}

		// Health bar width
		if(networkViewIsMine) {
			healthBarWidth = Config.instance.ownHealthBarWidth;
		} else {
			healthBarWidth = Config.instance.healthBarWidth;
		}
	}
	
	// UpdateLineOfSight
	protected void UpdateLineOfSight<T>(List<T> entityList) where T : Entity {
		uLink.NetworkPlayer netPlayer = this.networkView.owner;
		
		// Check if I can see all the other enemies
		foreach(var enemy in entityList) {
			// Ignore myself
			if(enemy == this)
				continue;
			
			if(!enemy.isAlive)
				continue;
			
			//uLink.NetworkPlayer enemyNetPlayer = enemy.networkView.owner;
			bool canSeeEnemy = CanSee(enemy);
			
			// First time only
			if(!this.playerVisibility.ContainsKey(enemy)) {
				this.playerVisibility[enemy] = false;//canSeeEnemy;
			}
			
			// If I can see an enemy...
			if(canSeeEnemy) {
				// Were we invisible to each other before?
				bool invisibleToEachOther = !(this.playerVisibility[enemy]);
					//this.networkView.GetScope(enemyNetPlayer) == false ||
					//enemy.networkView.GetScope(netPlayer) == false;
				
				// Start sending each other data
				//enemy.networkView.SetScope(netPlayer, true);
				//this.networkView.SetScope(enemyNetPlayer, true);
				
				if(invisibleToEachOther) {
					// Enemy becomes visible for me
					if(uLink.Network.isServer) {
						enemy.networkView.RPC("Visible", netPlayer,
							enemy.myTransform.position
						);
					} else {
						enemy.Visible(enemy.serverPosition);
					}
					
					// And I will be visible for him
					/*this.networkView.RPC("Visible", enemyNetPlayer,
						this.myTransform.position
					);*/
				}
			// If I can't see an enemy...
			// Stay visible if the enemy can see me
			} else { //if(!(enemy.playerVisibility.ContainsKey(this) && enemy.playerVisibility[this])) {
				// Were we visible to each other before?
				bool visibleToEachOther = this.playerVisibility[enemy];
					//this.networkView.GetScope(enemyNetPlayer) == true ||
					//enemy.networkView.GetScope(netPlayer) == true;
				
				if(visibleToEachOther) {
					// Enemy becomes invisible for me
					if(uLink.Network.isServer) {
						enemy.networkView.RPC("Invisible", netPlayer);
					} else {
						enemy.Invisible();
					}
					
					// And I will be invisible for him
					//this.networkView.RPC("Invisible", enemyNetPlayer);
				}
				
				// Stop sending each other data
				//enemy.networkView.SetScope(netPlayer, false);
				//this.networkView.SetScope(enemyNetPlayer, false);
			}
			
			// Update the visibility
			this.playerVisibility[enemy] = canSeeEnemy;
		}
	}
	
	// Returns true if I can see the player
	protected bool CanSee(Entity enemy) {
		if(GameManager.isTown && Player.showAllEntities)
			return true;
		
		// Player not even in my viewing frustum?
		if(!uLink.Network.isServer && !enemy.charGraphicsBody.renderer.isVisible)
			return false;
		
		RaycastHit hit;
		
		float yOffset = enemy.characterController.height * 0.1f;
		//Vector3 cameraOffset = new Vector3(0, 15.0f, 0);
		//Vector3 feetOffset = new Vector3(0, 2.5f, 0);
		//Vector3 enemyFeetOffset = new Vector3(0, 2.5f, 0); // characterController.height
		Vector3 enemyFeetPosition = enemy.myTransform.position; //+ enemyFeetOffset;
		enemyFeetPosition.y += yOffset;
		
		Vector3 enemyHeadPosition = enemy.myTransform.position;
		enemyHeadPosition.y += enemy.characterController.height - yOffset;
		
		// We check all layers except for transparent objects
		int layerMask = ~(1 << 1); // ~((1 << 1) | (1 << this.gameObject.layer));
		
		// TODO: Allowing the own collider is actually a bug,
		//       it might make enemies in the far visible.
		//       Only allow if the distance is small enough.
		
		// Check head position
		bool lineCastSuccess = Physics.Linecast(
			camPosition,
			enemyHeadPosition,
			out hit,
			layerMask
		);
		
		if(lineCastSuccess) {
			if(
				hit.collider == enemy.collider ||
				hit.collider == enemy.blockSphere.collider
			) {
				return true;
			}
		
			// Check own collider
			if((
				hit.collider == collider ||
				hit.collider == blockSphere.collider
			) && (enemyHeadPosition - myTransform.position).sqrMagnitude <= Config.maxSquaredDistanceOwnColliderAllowedInLoS) {
				return true;
			}
		} else {
			// In this case there are no colliders between you and the entity
			return true;
		}
		
		// Check feet position
		lineCastSuccess = Physics.Linecast(
			camPosition,
			enemyFeetPosition,
			out hit,
			layerMask
		);
		
		if(lineCastSuccess) {
			if(
				hit.collider == enemy.collider ||
				hit.collider == enemy.blockSphere.collider
			) {
				return true;
			}
			
			// Check own collider
			if((
				hit.collider == this.collider ||
				hit.collider == this.blockSphere.collider
			) && (enemyFeetPosition - myTransform.position).sqrMagnitude <= Config.maxSquaredDistanceOwnColliderAllowedInLoS) {
				return true;
			}
		} else {
			// In this case there are no colliders between you and the entity
			return true;
		}
		
		return false;
	}
	
	// Finds a player by his account ID
	// TODO: Use a dictionary
	public static Player FindPlayerByAccountId(string searchId) {
		foreach(var pty in GameServerParty.partyList) {
			foreach(Entity member in pty.members) {
				var player = (Player)member;
				
				if(player.accountId == searchId)
					return player;
			}
		}
		
		return null;
	}
	
	// Sets the movement speed of the character motor
	public void ApplyMotorMoveSpeed(float mspd) {
		if(mspd != motor.movement.maxForwardSpeed) {
			motor.movement.maxForwardSpeed = mspd;
			
			// TODO: We could actually ignore these...
			motor.movement.maxBackwardsSpeed = mspd;
			motor.movement.maxSidewaysSpeed = mspd;
		}
	}
	
	// Applies character stats - only called ONCE
	public void ApplyCharacterStats(bool charStatsChanged = false) {
		// Wait until we have both
		if(this.charStats == null || this.artifactTree == null || this.skillBuild == null)
			return;
		
		var finalCharStats = new CharacterStats(charStats);
		
		// Artifact tree
		finalCharStats.ApplyOffset(artifactTree.charStats);
		
		// For balancing reasons
		finalCharStats.ApplyOffset(-16);
		
		Log("Character stats after applying artifacts: " + finalCharStats);
		
		attackDmgMultiplier = finalCharStats.attackDmgMultiplier;
		defenseDmgMultiplier = finalCharStats.defenseDmgMultiplier;
		attackSpeedMultiplier = finalCharStats.attackSpeedMultiplier;
		
		// Skill cooldowns
		foreach(Attunement att in attunements) {
			foreach(Skill skill in att.skills) {
				foreach(Skill.Stage stage in skill.stages) {
					if(stage != null)
						stage.cooldown = stage.originalCooldown * finalCharStats.cooldownMultiplier;
				}
			}
		}
		
		maxEnergy = Config.instance.entityEnergy * finalCharStats.energyMultiplier;
		moveSpeedModifier = finalCharStats.moveSpeedMultiplier;
		
		if(animator) {
			animator.SetFloat("MoveSpeedMultiplier", moveSpeedModifier);
			animator.SetFloat("CastSpeedMultiplier", attackSpeedMultiplier);
			animator.SetFloat("AttackSpeedMultiplier", attackSpeedMultiplier);
			
			//LogManager.General.Log("MoveSpeed: " + this.moveSpeed);
			//LogManager.General.Log("AttackSpeedMultiplier: " + attackSpeedMultiplier + " (AttackSpeed: " + charStats.attackSpeed + ")");
		}
		
		appliedCharacterStats = true;
	}
	
#region Properties
	// Bit mask
	protected byte bitMask {
		get {
			return (byte)(
				(movementKeysPressed ? 1 : 0) |
				(jumping ? 2 : 0) |
				(motor.grounded ? 4 : 0)
			);
		}
	}

	// Level
	public override double level {
		get {
			if(account != null)
				return account.level;

			return 0d;
		}
	}
#endregion
	
#region Skill system
	public override void StartMeleeAttack() {
		if(weaponModel == null)
			return;
		
		// Enable colliders
		weaponModelCollider.enabled = true;
		weaponModelProjectileDeflector.enabled = true;
		
		/*if(uLink.Network.isServer) {
			animator.enabled = true;
		}*/
	}
	
	public override void StopMeleeAttack() {
		if(weaponModel == null)
			return;
		
		// Disable colliders
		weaponModelCollider.enabled = false;
		weaponModelProjectileDeflector.enabled = false;
		
		// Allow the weapon to hit targets again
		weaponModel.GetComponent<MeleeWeapon>().hitList.Clear();
		
		/*if(uLink.Network.isServer) {
			animator.enabled = false;
		}*/
	}
	
	// Creates particle systems on both hands
	protected override void StartCastEffect(GameObject prefab) {
		if(castEffect != null) {
			StopCastEffect();
		}
		
		// ...
		castEffect = (CastEffect)SpawnSkillInstance(
			prefab,
			currentSkill,
			currentSkill != null ? currentSkill.currentStage : null,
			Cache.vector3Zero,
			Cache.quaternionIdentity,
			Cache.vector3Zero
		);
	}
	
	// Destroys particle systems on both hands
	protected override void StopCastEffect() {
		if(castEffect != null)
			castEffect.Stop();
	}
#endregion
	
	// Client chat commands
	public virtual bool ProcessClientChatCommand(string msg) {
		return false;
	}
	
	// Re-enable collider after respawn
	void EnableCollider() {
		collider.enabled = true;
		
		if(motor != null)
			motor.enabled = true;
	}

	// Dead players disable their collider after some time
	void DisableCollider() {
		collider.enabled = false;
		
		if(motor != null)
			motor.enabled = false;
	}
	
	// DO NOT CALL THIS DIRECTLY
	protected void BasicRespawn(Vector3 spawnPosition) {
		Log("Respawn at " + spawnPosition);

		// For spawn protection
		lastRespawn = uLink.Network.time;
		
		// Remove debuffs
		foreach(SkillEffect effect in skillEffects) {
			effect.Remove(this);
		}
		
		skillEffects.Clear();
		
		// INFO: myTransform might not be initialized yet.
		// Reset position
		myTransform = transform;
		
		// Select party
		GameServerParty partyRespawn;
		if(GameManager.isTown)
			partyRespawn = GameServerParty.partyList[0];
		else
			partyRespawn = this.party;
		
		// Set position and rotation
		if(partyRespawn == null) {
			LogWarning("I don't have a party assigned to me!");
		} else if(myTransform == null) {
			LogWarning("I don't have a valid transform!");
		} else if(partyRespawn.spawn != null) {
			myTransform.position = spawnPosition;
			charGraphics.eulerAngles = new Vector3(0, partyRespawn.spawn.eulerAngles.y, 0);
		}
		
		if(motor != null) {
			//DLog ("Reset motor");
			motor.inputMoveDirection = Cache.vector3Zero;
			motor.inputJump = false;
			motor.movement.velocity = Cache.vector3Zero;
		}
		
		moveVector = Cache.vector3Zero;
		hasControlOverMovement = true;
		movementKeysPressed = false;
		jumping = false;
		hovering = false;
		
		// Re-enable collider
		EnableCollider();
		
		// Just to be safe
		stunned = 0;
		slept = 0;
		stagger = 0;
		
		// Reset animator
		if(animator != null) {
			// Reset animation state
			SetCastAnimationState(0, 0);
			animator.SetBool("Dead", false);
			animator.SetBool(interruptedHash, false);
			animator.SetBool("Moving", false);
			animator.SetBool("Jump", false);
			animator.SetBool("Hover", false);
			animator.SetInteger("Stunned", 0);
			animator.SetInteger("Stagger", 0);
		}
		
		// Client side
		if(uLink.Network.isClient) {
			// Show name label again because we disabled it on death
			if(nameLabel != null)
				nameLabel.enabled = true;
			
			// Necessary to unfocus the scroll view from DeathReview
			if(Login.instance != null)
				Login.instance.clearFlag = true;
			else
				GUIUtility.hotControl = 0;
		}
		
		// Clear death review
		skillDamageReceived.Clear();
		hitsByPlayer.Clear();
		
		// Clear rune levels
		ResetRuneLevels();
		
		// Fill energy
		energy = maxEnergy;
		
		// Fill HP
		health = maxHealth;

		// Event handler
		if(onRespawn != null)
			onRespawn(spawnPosition);
	}
	
	/*
	// ApplyColor
	void ApplyColor(Transform tr, float r, float g, float b, float a = 1.0f) {
		tr.renderer.material.color = new Color(r, g, b, a);
	}
	
	// ApplyColor
	void ApplyColor(Transform tr, Color c) {
		tr.renderer.material.color = c;
	}
	*/
	
	// Makes character face the direction we run to
	public void LerpCharGraphicsToMoveVector() {
		if(moveVector != Cache.vector3Zero) {
			Quaternion rot = Quaternion.LookRotation(moveVector);
			rot.x = 0.0f;
			rot.z = 0.0f;
			
			charGraphics.rotation = Quaternion.Lerp(charGraphics.rotation, rot, Time.deltaTime * Config.rotationInterpolationSpeed);
		}
	}
	
	// GetAccountId
	public override string GetAccountId() {
		return this.accountId;
	}
	
	// When a player connected, re-send all data about myself to him
	public override void ResendDataToPlayer(uLink.NetworkPlayer player) {
		base.ResendDataToPlayer(player);
	}
	
#region RPCs
	// --------------------------------------------------------------------------------
	// RPCs
	// --------------------------------------------------------------------------------
	
	// TODO: Change algorithm for object labels to O(n)
	[RPC]
	protected void ChangeParty(int partyId) {
		EntityLabel nameLabel;
		
		if(uLink.Network.isClient) {
			if(this.party != null) {
				// Old group red
				foreach(Player member in this.party.members) {
					// Object label
					nameLabel = member.GetComponent<EntityLabel>();
					if(nameLabel != null) {
						var partyColor = this.party.color;
						nameLabel.textColor = new Color(
							partyColor.r,
							partyColor.g,
							partyColor.b,
							nameLabel.textColor.a
						);
					}
				}
			}
		}
		
		LeaveParty();
		
		GameServerParty.partyList[partyId].AddMember(this);
		
		// Default color
		nameLabel = this.GetComponent<EntityLabel>();
		if(nameLabel != null) {
			var partyColor = this.party.color;
			nameLabel.textColor = new Color(
				partyColor.r,
				partyColor.g,
				partyColor.b,
				nameLabel.textColor.a
			);
		}
		
		// New group white
		if(uLink.Network.isClient && Player.main && Player.main.party == this.party) {
			foreach(Player member in Player.main.party.members) {
				// Object label
				nameLabel = member.GetComponent<EntityLabel>();
				if(nameLabel != null) {
					nameLabel.textColor = new Color(
						1f,
						1f,
						1f,
						nameLabel.textColor.a
					);
				}
			}
		}

		// Invoke event
		if(onChangeParty != null)
			onChangeParty();
	}
	
	[RPC]
	protected void ChangeLayer(int nLayer) {
		// Change his physics layer to the new party so he doesn't receive
		// collisions from his new team mates.
		layer = nLayer;
		skillLayer = nLayer;
	}
	
	[RPC]
	protected void LeaveParty() {
		if(this.party != null) {
			this.party.RemoveMember(this);
		}
	}
	
	[RPC]
	protected void SwitchAttunement(byte attunementId) {
		if(currentWeapon == null || currentAttunement == null)
			return;
		
		// DO NOT check for the correct attunement ID here since only the weapon might have changed
		
		// Set the attunement I was in on cooldown
		currentAttunement.lastUse = uLink.Network.time;
		
		// Set new attunement
		SetAttunement(attunementId);
		
		// Use attunement skill
		if(uLink.Network.isClient) {
			// TODO: Store prefab in attunement class
			var prefab = Resources.Load("Attunements/" + currentAttunement.name + "Attunement");
			if(prefab != null) {
				var rotation = Cache.quaternionIdentity;
				
				if(attunementVisuals != null) {
					rotation = attunementVisuals.transform.rotation;
					Destroy(attunementVisuals);
				}
				
				attunementVisuals = (GameObject)GameObject.Instantiate(prefab, this.center, rotation);
				attunementVisuals.transform.parent = hips;
				attunementVisuals.transform.localPosition = Cache.vector3Zero;
				
				attunementVisuals.GetComponent<ConstantRotation>().rotationSpeed *= 2.5f;
				attunementVisuals.GetComponent<LerpRotationSpeed>().enabled = true;
				
				if(networkViewIsMine) {
					attunementVisuals.GetComponent<DisableLightsWhenNeeded>().enabled = false;
				}
			}
		}
		
		// TODO: Add attunement bonus
	}
	
	// Set new attunement without getting any bonuses
	protected void SetAttunement(byte attunementId) {
		currentWeapon.currentAttunementId = attunementId;
		currentAttunementId = attunementId;
		currentAttunement = attunements[attunementId];
	}
	
	[RPC]
	protected void SwitchWeapon(byte weaponSlotId) {
		currentWeaponSlotId = weaponSlotId;
		currentWeapon = weapons[weaponSlotId];
		
		// Summon skill
		if(currentWeapon.summonSkill != null) {
			UseSkill(currentWeapon.summonSkill, Cache.vector3Zero, 0);
		} else {
			LogManager.General.LogError("Couldn't find summon skill for weapon: " + currentWeapon.name);
		}
		
		if(!currentWeapon.autoTarget) {
			target = null;
			aboutToTarget = null;
		}
		
		if(crossHair != null) {
			crossHair.enabled = true;
		}

		if(animator != null) {
			animator.SetInteger("Weapon ID", currentWeapon.id);
		}
		
		SwitchAttunement(currentWeapon.currentAttunementId);
	}
	
	[RPC]
	protected void ReceivePlayerInfo(string playerName, int bestRanking) {
		Log("Received player name '" + playerName + "' and ranking " + bestRanking);
		
		name = playerName;
		stats.bestRanking = bestRanking;
		
		// As long as we aren't in client test mode
		if(account != null)
			account.playerName = playerName;
	}
	
	[RPC]
	protected void ReceiveMainGuildInfo(string newGuildName, string newGuildTag) {
		Log("Received guild name '" + newGuildName + "' and tag " + newGuildTag);

		guildName = newGuildName;
		guildTag = newGuildTag;
	}
	
	[RPC]
	protected void ReceiveSkillBuild(SkillBuild newBuild) {
		Log("Received skill build with X weapons: " + newBuild.weapons.Length);
		
		skillBuild = newBuild;
		
		ApplyCharacterStats();
		
		// Enable skill bar
		if(uLink.Network.isClient) {
			var skillBar = this.GetComponent<SkillBar>();
			if(skillBar != null)
				skillBar.enabled = true;
		}
	}
	
	[RPC]
	// TODO: Bitstream variant
	protected void ReceiveArtifactTree(string jsonTree) {
		Log("Received artifact tree! " + jsonTree);
		
		this.artifactTree = Jboy.Json.ReadObject<ArtifactTree>(jsonTree);
		this.ApplyCharacterStats();
	}
	
	[RPC]
	protected void ReceiveCharacterStats(CharacterStats newCharStats) {
		Log(string.Format("Received character stats: {0}!", newCharStats));
		
		// Cheater?
		if(!newCharStats.valid) {
			LogError("Received character stats are not valid!");
			return;
		}
		
		// TODO: Check sender (?)
		charStats = newCharStats;
		ApplyCharacterStats();
	}
	
	[RPC]
	public void ReceiveCharacterCustomization(CharacterCustomization nCustom) {
		if(customization != nCustom) {
			Log("Received character customization!");
			customization = nCustom;
		}
		
		// Height set on server as well so the hand position is accurate
		charGraphicsModel.localScale = customization.scaleVector;
		heightMultiplier = charGraphicsModel.localScale.y;
		centerOffset = new Vector3(0, height / 2, 0);

		// Update block sphere
		blockSphere.localScale = customization.scaleVector;
		blockSphere.localPosition = centerOffset;
		
		if(!uLink.Network.isServer) {
			audio.pitch = customization.finalVoicePitch;
			customization.UpdateMaterials(charGraphicsModel);
		}
	}
	
	[RPC]
	protected void ReceiveNewBestRanking(int nNewBestRanking) {
		// TODO: Check sender (?)
		newBestRanking = nNewBestRanking;
	}
	
	[RPC]
	protected void ReceiveArtifactReward(int itemId) {
		artifactReward = new Artifact(itemId);
		Log("Received artifact reward: " + artifactReward.id);
	}
	
	[RPC]
	protected override bool StartHover() {
		if(hovering)
			return false;
		
		LogSpam("Start hover!");
		hovering = true;
		moveSpeedModifier += Config.instance.hoverSpeedBonus;
		
		if(!uLink.Network.isServer)
			myTransform.FindChild("HoverWindZone").gameObject.SetActive(true);
		
		if(motor != null) {
			motor.movement.gravity = 0f;
			motor.movement.maxAirAcceleration = 10000f;
			motor.jumping.jumping = false;
			motor.jumping.enabled = false;
		}
		
		if(movementSoundsSource != null) {
			movementSoundsSource.volume = 0f;
			movementSoundsSource.clip = flightSound;
			movementSoundsSource.Play();
			
			this.Fade(
				flightSoundFadeTime,
				val => { movementSoundsSource.volume = val; }
			);
		}
		
		if(animator != null)
			animator.SetBool("Hover", true);
		
		return true;
	}
	
	[RPC]
	protected override bool EndHover() {
		if(!hovering)
			return false;
		
		LogSpam("End hover!");
		hovering = false;
		moveSpeedModifier -= Config.instance.hoverSpeedBonus;
		
		if(!uLink.Network.isServer)
			myTransform.FindChild("HoverWindZone").gameObject.SetActive(false);
		
		if(motor != null) {
			motor.movement.gravity = motorGravity;
			motor.movement.maxAirAcceleration = motorAirAcceleration;
			motor.jumping.enabled = true;
		}
		
		if(movementSoundsSource != null) {
			this.Fade(
				flightSoundFadeTime,
				val => { movementSoundsSource.volume = 1f - val; },
				()  => { movementSoundsSource.Stop(); }
			);
		}
		
		if(animator != null)
			animator.SetBool("Hover", false);
		
		return true;
	}
	
	/*[RPC]
	protected void StartRoll(Vector3 nRollVector, float hMovement, float vMovement) {
		blockCapacity -= Player.rollCost;
		
		// Animate this on the server as well to prevent abuse with sword skill + rolling
		if(animator != null)
			animator.SetBool("Roll", true);
		
		Invoke("EndRoll", Player.rollDuration);
		moveSpeedModifier += Player.rollSpeedBonus;
		rolling = true;
		rollVector = nRollVector;
		rollVectorNonTransformed = new Vector3(hMovement, 0, vMovement);
	}
	
	// End roll
	protected EndRoll() {
		rolling = false;
		
		if(animator != null)
			animator.SetBool("Roll", false);
		
		moveSpeedModifier -= Player.rollSpeedBonus;
	}*/
	
	[RPC]
	protected void ReceiveHitPoint(Vector3 nHitPoint) {
		if(lastSkillInstance) {
			lastSkillInstance.hitPoint = nHitPoint;
		}
	}
	
	[RPC]
	protected void SetNoTarget() {
		target = null;
		noTargetSent = false;
	}
	
	[RPC]
	protected void SwordClash(Vector3 diff) {
		var shockWaveId = Skill.nameToId["Shockwave"];
		var shockWave = Skill.idToSkill[shockWaveId];
		
		// TODO: Get correct position for the shockwave
		var skillInstance = UseSkill(shockWave, this.myTransform.position + diff, 0);
		
		// Reduce pull power
		var shockWaveInstance = skillInstance.gameObject.GetComponent<AoEPush>();
		//shockWaveInstance.pullPower *= 0.25f;
		shockWaveInstance.radius *= 0.5f;
		shockWaveInstance.maxPushRadius *= 0.5f;
		shockWaveInstance.gameObject.particleSystem.startLifetime *= 0.5f;
		
		this.StopMeleeAttack();
		this.InterruptCast();
		//shockWaveInstance.gameObject.particleSystem.emissionRate *= 0.25f;
		
		//characterController.Move(-diff * 5);
	}
	
	[RPC]
	protected void StartGame() {
		GameManager.gameStarted = true;
		
		if(this == Player.main && lobbyChat != null && GameManager.isArena) {
			lobbyChat.AddEntry("Game started.");
		}
	}
	
	[RPC]
	protected void Ready() {
		Log("Ready!");
		
		if(this == Player.main && GameManager.isArena) {
			GetComponent<ScoreBoard>().enabled = true;
			GetComponent<WinnerTeam>().enabled = true;
			GetComponent<MatchRewards>().enabled = true;
		}
		
		isReady = true;
	}
	
	[RPC]
	protected void VoIPData(byte[] bytes, int len, uLink.NetworkMessageInfo info) {
		voIP.OnVoIPData(bytes, len, info);
	}
#endregion
}
