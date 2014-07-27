using uLink;
using UnityEngine;
using System.Collections.Generic;

public class PlayerOnServer : Player, CasterOnServer {
	private float maxDistanceToClientHackLog = 50f;
	private float maxDistanceToClientHackLogSqr;
	
	[HideInInspector]
	public bool clientGrounded;

	private int respawnCount;
	private int lastHPSent;
	private Vector3 lastPositionSent;
	private Vector3 lastPositionSaved;
	private bool lastMovementKeysPressedReceived;
	private double startedSkillCast;
	
	// Reliable / unreliable switches
	private int lastReliableBitMaskSent;
	
	private GameMode gameMode;
	private bool lastGameEndedState;
	
	// RPC names
	private string rpcState = "M";
	
	// Last timestamps - lts
	private double ltsClientVelocity;
	private double ltsClientJumpState;
	//private double ltsClientStartCast;
	//private double ltsClientEndCast;
	private double ltsClientStartBlock;
	private double ltsClientEndBlock;
	private double ltsClientAttunement;
	private double ltsClientWeapon;
	private double ltsClientChat;
	private double ltsClientTarget;
	//private double ltsClientNoTarget;
	
	// Sets up calling SendToClients constantly
	protected override void Awake() {
		base.Awake();
		
		maxDistanceToClientHackLogSqr = maxDistanceToClientHackLog * maxDistanceToClientHackLog;
		gameMode = ServerInit.instance.gameMode;

		// Events
		onDeath += OnDeath;

		// Count respawns so we know when we can save position data
		onRespawn += pos => {
			respawnCount += 1;
		};
	}

	// Start
	void Start() {
		InvokeRepeating("SendToClients", 0.01f, 1.0f / uLink.Network.sendRate);
		
		if(GameManager.isPvP) {
			InvokeRepeating("SendInstanceStats", 0.01f, Config.instance.matchStatsSendDelay);
		} else {
			InvokeRepeating("SendPing", 0.01f, Config.instance.pingSendDelay);
			InvokeRepeating("SavePosition", 0.01f, Config.instance.savePositionDelay);
		}
	}
	
#region Update
	// Updates that need to be executed in every frame
	void Update () {
		// Movement
		UpdateMovement();

		// Animations
		//if(animator != null)
		//	UpdateSkillAnimations();

		// Map boundaries
		StayInMapBoundariesServer();
	}
	
	// Updates that can be executed at a fixed rate
	void FixedUpdate() {
		// Game ended?
		UpdateGameMode();
		
		// Debuffs
		UpdateSkillEffects();
		
		// Energy
		UpdateEnergyOnServer();
	}
	
	// Check whether the game ended in the mode we play
	void UpdateGameMode() {
		GameManager.gameEnded = gameMode.gameEnded;
		
		if(GameManager.gameEnded && lastGameEndedState == false) {
			// We send it to all the other representations
			networkView.RPC("EndGame", uLink.RPCMode.Others, gameMode.winnerParty.id);
		}
		
		lastGameEndedState = GameManager.gameEnded;
	}
	
	// UpdateMovement
	void UpdateMovement() {
		if(!isReady)
			return;
		
		// Reset rotation
		ResetRotation();
		
		if(!hasControlOverMovement) {
			clientPosition = myTransform.position;
			return;
		}
		
		// Move speed
		ApplyMotorMoveSpeed(moveSpeed);
		
		// Auto target
		bool useAutoTarget = target != null;
		Quaternion targetRotation = Cache.quaternionIdentity;
		
		if(useAutoTarget) {
			Vector3 diff = (target.myTransform.position - myTransform.position);
			
			if(diff != Cache.vector3Zero) {
				targetRotation = Quaternion.LookRotation(diff);
				targetRotation.x = 0.0f;
				targetRotation.z = 0.0f;
			}
		}
		
		// Flying
		motor.inputFly = hovering;
		
		// Movement
		if(cantMove) {
			// Stop movement on stun
			moveVector = Cache.vector3Zero;
			motor.inputJump = false;
			movementKeysPressed = false;
			motor.inputMoveDirection = Cache.vector3Zero;
		} else {
			if(useAutoTarget) {
				charGraphics.rotation = targetRotation;
			} else {
				if(moveVector != Cache.vector3Zero) {
					charGraphics.rotation = Quaternion.LookRotation(moveVector);
				}
			}
			
			motor.inputJump = jumpButtonPressed;
			
			// Max distance to client
			//clientPosition.y = myTransform.position.y;
			Vector3 toClient = clientPosition - myTransform.position;
			
			// Snap to client position if we were falling down
			// while the client landed on top of something.
			if(disableSnappingToNewPosition == 0 && clientGrounded && !motor.grounded && motor.movement.velocity.y < -1f) {
				myTransform.position = clientPosition;
			}
			
			// We will try to run towards the client if we got too far away
			motor.inputMoveDirection = moveVector + toClient;
			
			float distanceToClientPredictedSqr = motor.inputMoveDirection.sqrMagnitude;
			if(distanceToClientPredictedSqr > 1f) {
				motor.inputMoveDirection.Normalize();
				
				// Someone trying to hack?
				if(distanceToClientPredictedSqr > maxDistanceToClientHackLogSqr)
					LogWarning("Hacking attempt, distance to server: " + distanceToClientPredictedSqr);
			}
			
			// If we haven't received movement RPCs for more than 0.5 seconds
			// we can safely assume the client has stopped moving.
			if(uLink.Network.time - ltsClientVelocity > 0.5)
				movementKeysPressed = false;
			else
				movementKeysPressed = lastMovementKeysPressedReceived;
		}
	}
#endregion
	
	// Server sends the result of the movement to all clients
	void SendToClients() {
		// Line of sight
		//UpdateLineOfSight();
		
		// Jump animation state
		if(motor.jumping.jumping && !motor.grounded) { //!characterController.isGrounded) {
			jumping = true;
		} else {
			jumping = false;
		}
		
		// State & Movement
		var myBitMask = this.bitMask;
		
		if(movementKeysPressed || jumping || motor.movement.velocity != Cache.vector3Zero || myTransform.position != lastPositionSent) {
			// Send unreliable state RPC as we move
			networkView.UnreliableRPC(rpcState, uLink.RPCMode.Others,
				myTransform.position,
				moveVector,
				(ushort)(charGraphics.eulerAngles.y * Cache.rotationFloatToShort),
				(byte)myBitMask
			);
			
			reliablePosSent = false;
		} else {
			// Send reliable state RPC when we stop
			if(!reliablePosSent || bitMask != lastReliableBitMaskSent) {
				networkView.RPC(rpcState, uLink.RPCMode.Others,
					myTransform.position,
					Cache.vector3Zero,
					(ushort)(charGraphics.eulerAngles.y * Cache.rotationFloatToShort),
					(byte)myBitMask
				);
				
				reliablePosSent = true;
				lastReliableBitMaskSent = bitMask;
			}
		}
		
		lastPositionSent = myTransform.position;
		
		// Send hit point when holding skills
		if(isHoldingSkill) {
			networkView.UnreliableRPC(
				"ReceiveHitPoint",
				uLink.RPCMode.OthersExceptOwner,
				lastSkillInstance.hitPoint
			);
		}
		
		// HP
		if(health != lastHPSent) {
			networkView.RPC("SetHP", uLink.RPCMode.Others, (ushort)health);
			
			lastHPSent = health;

			// Death
			if(health == 0) {
				// Let others know about the kill
				networkView.RPC("RegisterKill", uLink.RPCMode.All, lastHitBy.id, this.id, (short)lastHitBySkill.id);
			}
		}
	}
	
	// Send scoreboard stats to players
	void SendInstanceStats() {
		stats.ping = networkView.owner.averagePing;
		networkView.UnreliableRPC("UpdateStats", uLink.RPCMode.Others,
			(int)stats.total.damage,
			(ushort)stats.total.cc,
			(byte)stats.total.kills,
			(byte)stats.total.deaths,
			(byte)stats.total.assists,
			(ushort)stats.ping,
			(int)Random.seed
		);
	}
	
	// Send ping to players
	void SendPing() {
		stats.ping = networkView.owner.averagePing;
		networkView.UnreliableRPC("UpdatePing", uLink.RPCMode.Others, (ushort)stats.ping);
	}
	
	// After death state
	void SendRespawn() {
		Spawn spawn;
		
		if(GameManager.isTown) {
			spawn = GameServerParty.partyList[0].spawnComp;
		} else {
			spawn = party.spawnComp;
		}

		var spawnPos = spawn.GetNextSpawnPosition();
		networkView.RPC("Respawn", uLink.RPCMode.Others, spawnPos);
		networkView.RPC("SetCameraYRotation", uLink.RPCMode.Owner, spawn.transform.eulerAngles.y);
		Respawn(spawnPos);
	}

	// Save position in database
	void SavePosition() {
		// Did we already save the position?
		if(position == lastPositionSaved)
			return;

		// Only save position when we already loaded it and respawned
		if(respawnCount == 0)
			return;

		// Account ID not available for some reason?
		if(string.IsNullOrEmpty(accountId))
			return;

		// Save in DB
		PositionsDB.SetPosition(accountId, position, null);

		lastPositionSaved = position;
	}
	
	// Stay in map boundaries on server side
	public void StayInMapBoundariesServer() {
		Vector3 pos = MapManager.StayInMapBoundaries(myTransform.position);
		
		if(pos != myTransform.position) {
			if(myTransform.position.y < pos.y && myTransform.position != Cache.vector3Zero) {
				// Did we fall down below map boundaries?
				SendRespawn();
			} else {
				// Snap to map boundaries
				myTransform.position = pos;
			}
		}
	}
	
	// InterruptCast
	public override void InterruptCast() {
		if(currentSkill != null)
			LogSpam("Cast has been interrupted: " + currentSkill.skillName);
		
		if(hovering)
			EndHover();
		
		if(StopHoldingSkill()) {
			// Actually we do not need to send this because we send each interrupt source
			//networkView.RPC("StopHoldingSkill", uLink.RPCMode.Others);
		}
		
		currentSkill = null;
	}
	
	// When a player connected, re-send all data about myself to him
	public override void ResendDataToPlayer(uLink.NetworkPlayer player) {
		base.ResendDataToPlayer(player);
		
		if(networkView != null) {
			Log("Resending data to player " + player.id);
			
			// Party
			if(party != null) {
				networkView.RPC("ChangeParty", player, this.party.id);
			} else {
				Log("My party is null, not going to resend party data to player " + player.id);
			}
			
			networkView.RPC("ChangeLayer", player, this.layer);
			networkView.RPC("ReceivePlayerInfo", player, name, stats.ranking);
			networkView.RPC("ReceiveMainGuildInfo", player, this.guildName, this.guildTag);
			networkView.RPC("ReceiveSkillBuild", player, this.skillBuild);
			networkView.RPC("ReceiveCharacterCustomization", player, this.customization);
			networkView.RPC("ReceiveArtifactTree", player, Jboy.Json.WriteObject(this.artifactTree));
			networkView.RPC("ReceiveCharacterStats", player, this.charStats);
			
			// Current attunements in all weapons
			for(byte i = 0; i < weapons.Count; i++) {
				networkView.RPC("WeaponAttunement", player, i, weapons[i].currentAttunementId);
			}
			
			networkView.RPC("SwitchWeapon", player, currentWeaponSlotId);
			networkView.RPC("SwitchAttunement", player, currentAttunementId);

			// Position and state
			networkView.RPC(
				rpcState,
				player,
				myTransform.position,
				moveVector,
				(ushort)(charGraphics.eulerAngles.y * Cache.rotationFloatToShort),
				(byte)bitMask
			);
			
			Log("Resent data to player " + player.id);
		} else {
			Log("My networkView is null, not going to resend data to player " + player.id);
		}
	}
	
	// Moves to the position where the player probably would be right now
	void PredictClientPosition(float timePacketArrival, Vector3 newMoveVector) {
		// Limit time
		if(timePacketArrival > Config.instance.maxPositionPredictionTime)
			timePacketArrival = Config.instance.maxPositionPredictionTime;
		
		Vector3 oldMoveVector = moveVector;
		
		Vector3 movementOffset = (newMoveVector - oldMoveVector) * this.moveSpeed * timePacketArrival;
		characterController.Move(movementOffset);
		
		if(movementOffset != Cache.vector3Zero && (newMoveVector == Cache.vector3Zero || oldMoveVector == Cache.vector3Zero))
			LogSpam("Client Position Prediction: MoveOffset: " + movementOffset + " (OldMoveVector: " + oldMoveVector + ", NewMoveVector: " + newMoveVector + ", Packet Arrival Time: " + timePacketArrival + ")");
		
		this.moveVector = newMoveVector;
	}
	
	// Moves to the position where the player probably would be right now
	/*void PredictClientPosition(float timePacketArrival, Vector3 newMoveVector, float newMoveSpeedModifier) {
		// Limit time
		if(timePacketArrival > 0.1f) {
			timePacketArrival = 0.1f;
		}
		
		Vector3 oldMoveVector = moveVector;
		float oldMoveSpeed = this.moveSpeed;
		
		this.moveSpeedModifier = newMoveSpeedModifier;
		
		Vector3 movementOffset = (newMoveVector * this.moveSpeed - oldMoveVector * oldMoveSpeed) * timePacketArrival;
		characterController.Move(movementOffset);
		
		if(movementOffset != Cache.vector3Zero && (newMoveVector == Cache.vector3Zero || oldMoveVector == Cache.vector3Zero))
			DSpamLog("Client Position Prediction: MoveOffset: " + movementOffset + " (OldMoveVector: " + oldMoveVector + ", NewMoveVector: " + newMoveVector + ", Packet Arrival Time: " + timePacketArrival + ")");
		
		this.moveVector = newMoveVector;
	}*/
	
	// --------------------------------------------------------------------------------
	// Callbacks
	// --------------------------------------------------------------------------------
	
	// Handle disconnects
	void uLink_OnPlayerDisconnected(uLink.NetworkPlayer player) {
		if(networkView.owner == player) {
			networkView.RPC("LeaveParty", uLink.RPCMode.All);
			networkView.RPC("Chat", uLink.RPCMode.Others, this.name + " disconnected.");
		}
	}
	
	// This is called when THIS prefab itself gets instantiated
	protected override void uLink_OnNetworkInstantiate(uLink.NetworkMessageInfo info) {
		base.uLink_OnNetworkInstantiate(info);

		Log("OnNetworkInstantiate: " + info.networkView.viewID);
		
		// Resend important data to newly connected player
		foreach(var otherNetworkView in uLink.Network.networkViews) {
			if(otherNetworkView == this.networkView)
				continue;
			
			var entity = otherNetworkView.GetComponent<Entity>();
			
			if(!entity)
				continue;
			
			entity.ResendDataToPlayer(info.networkView.owner);
		}

		// Set client position
		clientPosition = myTransform.position;
	}
	
	// When a player dies...
	void OnDeath() {
		// Send him his death review
		var receiver = networkView.owner;
		foreach(KeyValuePair<int, int> entry in skillDamageReceived) {
			//DLog(entry.Key + " did " + entry.Value + " damage.");
			networkView.RPC("SkillDamage", receiver, (short)entry.Key, entry.Value);
		}
		networkView.RPC("SkillDamageSent", receiver);
		
		// Credits for assisting
		foreach(KeyValuePair<Entity, int> entry in hitsByPlayer) {
			if(lastHitBy != entry.Key) {
				entry.Key.stats.total.assists += 1;
			}
		}
		
		// Disable collider
		Invoke("DisableCollider", Config.instance.deathColliderDisableTime);
		
		// Respawn him after a certain time
		Invoke("SendRespawn", Config.instance.playerRespawnTime);
	}

#region Properties
	// Can act
	public bool canAct {
		get {
			return isAlive && !gameMode.gameEnded && GameManager.gameStarted;
		}
	}
#endregion
	
#region RPCs
	// Server receives input data from the client
	[RPC]
	void S(Vector3 nClientPosition, Vector3 newMoveVector, byte nBitMask, uLink.NetworkMessageInfo info) {
		// Make sure we throw away messages from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		if(info.timestamp <= ltsClientVelocity || info.timestamp <= ignoreNewPositionEarlierThanTimestamp)
			return;
		
		//camPosition = nCamPosition;
		
		bool nMovementKeysPressed = (nBitMask & 1) != 0;
		bool nJumping = (nBitMask & 2) != 0;
		clientGrounded = (nBitMask & 4) != 0;
		
		// Always accept client position coordinates
		clientPosition = nClientPosition;
		
		// Can't move while stunned
		if(stunned > 0 || immobilized > 0 || slept > 0 || !canAct)
			return;
		
		if(newMoveVector.sqrMagnitude > 1) {
			newMoveVector.Normalize();
		}
		
		// Predict client's current position if he just started running
		PredictClientPosition(info.GetPacketArrivalTime(), newMoveVector);
		
		//float directionAngle = directionAngleAsUShort * rotationShortToFloat * degToRad;
		//horizontalMovement = newMoveVector.x; //Mathf.Sin(directionAngle);
		//verticalMovement = newMoveVector.z; //Mathf.Cos(directionAngle);
		movementKeysPressed = nMovementKeysPressed;
		lastMovementKeysPressedReceived = nMovementKeysPressed;
		jumping = nJumping;
		
		ltsClientVelocity = info.timestamp;
	}
	
	// Server receives camera position from the client
	[RPC]
	void C(Vector3 nCamPosition, uLink.NetworkMessageInfo info) {
		// Make sure we throw away messages from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		camPosition = nCamPosition;
	}
	
	// Server receives hit point position
	[RPC]
	void CHP(Vector3 nHitPoint, uLink.NetworkMessageInfo info) {
		// Make sure we throw away messages from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		ReceiveHitPoint(nHitPoint);
	}
	
	[RPC]
	public void ClientReady(uLink.NetworkMessageInfo info) {
		if(info.sender != networkView.owner)
			return;
		
		// Let the others know
		this.networkView.RPC("Ready", uLink.RPCMode.OthersExceptOwner);
		this.Ready();
		
		if(!GameManager.gameStarted) {
			// Start conditions
			if(GameServerParty.AllPartiesReady()) {
				// Start game after a few seconds
				gameMode.SendGameStartCountdown();
			}
		}
	}
	
	[RPC]
	void ClientJumpState(bool state, uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner || info.timestamp <= ltsClientJumpState)
			return;
		
		// Can't jump while stunned
		if(stunned > 0 || immobilized > 0 || slept > 0 || !canAct)
			return;
		
		//DLog ("Jump: " + val);
		//networkView.RPC("Jump", uLink.RPCMode.Others);
		jumpButtonPressed = state;
		
		ltsClientJumpState = info.timestamp;
	}
	
	[RPC]
	public void ClientStartCast(byte slotId, uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		/*if(info.timestamp <= ltsClientStartCast) {
			DLogWarning("Old ClientStartCast message arrived");
			return;
		}*/
		
		// Invalid skill ID?
		if(slotId < 0 || slotId >= skills.Count) {
			LogWarning("StartCastRejected: " + CastError.InvalidSkillId.ToString());
			networkView.RPC("StartCastRejected", info.sender, CastError.InvalidSkillId);
			return;
		}
		
		// Can't cast while casting already
		if(currentSkill != null) {
			LogWarning("StartCastRejected: " + CastError.CurrentSkillNotNull.ToString());
			networkView.RPC("StartCastRejected", info.sender, CastError.CurrentSkillNotNull);
			return;
		}
		
		// Are we able to cast it?
		if(!canCast || !canAct) {
			LogWarning("StartCastRejected: " + CastError.NoControl.ToString());
			networkView.RPC("StartCastRejected", info.sender, CastError.NoControl);
			return;
		}
		
		// Select skill
		currentSkill = skills[slotId];
		
		// Anti cooldown hack
		if(currentSkill.currentStage.isOnCooldown) {
			LogWarning("Detected cooldown hack: " + currentSkill.skillName);
			networkView.RPC("StartCastRejected", info.sender, CastError.CooldownHack);
			
			// We need to reset currentSkill, otherwise we'd constantly be stuck with this skill on the server side
			currentSkill = null;
			
			return;
		}
		
		// Anti skill requirement hack
		if(energy < currentSkill.currentStage.energyCostAbs) {
			LogWarning("Detected block capacity requirement bypass hack: " + currentSkill.skillName);
			networkView.RPC("StartCastRejected", info.sender, CastError.RequirementBypassHack);
			
			// We need to reset currentSkill, otherwise we'd constantly be stuck with this skill on the server side
			currentSkill = null;
			
			return;
		}
		
		// Save timestamp
		startedSkillCast = uLink.Network.time - info.GetPacketArrivalTimeDouble();
		
		LogSpam("Start casting of: " + currentSkill.skillName);
		networkView.RPC("StartCast", uLink.RPCMode.OthersExceptOwner, slotId);
		
		//Invoke("ClientEndCast", currentSkill.castDuration);
		//ltsClientStartCast = info.timestamp;
	}
	
	[RPC]
	public void ClientAdvanceCast(uLink.NetworkMessageInfo info) {
		// Make sure we throw away messages from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		// NOTE: TEMPORARY
		/*if(currentSkill.canAdvance)
			currentSkill.currentStageIndex += 1;
		networkView.RPC("AdvanceCast", uLink.RPCMode.Others);
		return;*/
		
		// Are we able to cast it?
		if(!canCast || !canAct) {
			LogWarning("AdvanceCastRejected: " + CastError.NoControl);
			networkView.RPC("AdvanceCastRejected", uLink.RPCMode.Others, CastError.NoControl);
			return;
		}
		
		// Can't cast while casting already
		if(currentSkill == null) {
			LogWarning("AdvanceCastRejected: " + CastError.CurrentSkillNull);
			networkView.RPC("AdvanceCastRejected", uLink.RPCMode.Others, CastError.CurrentSkillNull);
			return;
		}
		
		// Is there a next stage?
		if(!currentSkill.canAdvance) {
			LogWarning("AdvanceCastRejected: " + CastError.MaxAdvanceStage);
			networkView.RPC("AdvanceCastRejected", uLink.RPCMode.Others, CastError.MaxAdvanceStage);
			return;
		}
		
		// TODO: cast duration check
		
		currentSkill.currentStageIndex += 1;
		
		LogSpam("Advance casting of: " + currentSkill.skillName);
		networkView.RPC("AdvanceCast", uLink.RPCMode.OthersExceptOwner);
	}
	
	[RPC]
	public void ClientCouldntCast(uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		InterruptCast();
		
		networkView.RPC("CouldntCast", uLink.RPCMode.Others);
	}
	
	[RPC]
	public void ClientEndCast(Vector3 hitPoint, uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		/*if(info.timestamp <= ltsClientEndCast) {
			DLogWarning("Old ClientEndCast message arrived");
			//networkView.RPC("EndCastRejected", info.sender, CastError.);
			return;
		}*/
		
		// Did we start a cast and are we still casting it?
		if(currentSkill == null) {
			LogWarning("EndCastRejected: " + CastError.CurrentSkillNull);
			networkView.RPC("EndCastRejected", uLink.RPCMode.Others, CastError.CurrentSkillNull);
			return;
		}
		
		// Can't end casts while stunned
		if(stagger > 0 || stunned > 0 || slept > 0 || !isAlive || blocking) {
			LogWarning("EndCastRejected: " + CastError.NoControl);
			networkView.RPC("EndCastRejected", uLink.RPCMode.Others, CastError.NoControl);
			EndSkill();
			return;
		}
		
		// Anti cast speed hack
		double castDuration = uLink.Network.time - startedSkillCast;
		if(castDuration + 0.1f  < currentSkill.currentStage.castDuration * attackSpeedMultiplier) {
			LogWarning("Detected cast speed hack: " + currentSkill.skillName + " with " + castDuration + " / " + (currentSkill.currentStage.castDuration * attackSpeedMultiplier));
			networkView.RPC("EndCastRejected", uLink.RPCMode.Others, CastError.CastSpeedHack);
			EndSkill();
			return;
			//networkView.RPC("EndCast", uLink.RPCMode.Owner, hitPoint);
			//return;
		} else {
			LogSpam(currentSkill.skillStageName + " ended cast with cast duration: " + castDuration.ToString("0.00") + " / " + (currentSkill.currentStage.castDuration * attackSpeedMultiplier).ToString("0.00"));
		}
		
		// Instantiate it locally
		try {
			// We transform time backwards for cooldown calculation to prevent
			// false cooldown hack messages.
			var lastUsage = (float)(uLink.Network.time) - info.GetPacketArrivalTime();
			
			// Finally we can activate the skill
			UseSkill(currentSkill, hitPoint, -1, lastUsage);
		} catch {
			throw;
		} finally {
			// Whatever happens, we want the client to finish the animation
			networkView.RPC("EndCast", uLink.RPCMode.OthersExceptOwner, hitPoint);
			
			// Reset to stage 1
			currentSkill.currentStageIndex = 0;
			
			// Anim duration
			if(!currentSkill.canHold)
				DelayedEndSkill(info.timestamp);
			
			// Here currentSkill could be null already
			
			//ltsClientEndCast = info.timestamp;
		}
	}
	
	[RPC]
	public void ClientInstantCast(byte skillId, Vector3 hitPoint, uLink.NetworkMessageInfo info) {
		// TODO: Security checks
		// TODO: Stagger / stun check
		
		// Invalid skill ID?
		if(skillId < 0 || skillId >= skills.Count) {
			LogWarning("InstantCastRejected: " + CastError.InvalidSkillId.ToString());
			networkView.RPC("InstantCastRejected", info.sender, CastError.InvalidSkillId);
			return;
		}
		
		// Can't cast while casting already
		if(currentSkill != null) {
			LogWarning("InstantCastRejected: " + CastError.CurrentSkillNotNull.ToString());
			networkView.RPC("InstantCastRejected", info.sender, CastError.CurrentSkillNotNull);
			return;
		}
		
		// Select skill
		currentSkill = skills[skillId];
		
		// Check if it's insta-castable
		if(currentSkill.currentStage.castDuration != 0f) {
			LogWarning("InstantCastRejected: " + CastError.CastSpeedHack.ToString());
			networkView.RPC("InstantCastRejected", info.sender, CastError.CastSpeedHack);
			return;
		}
		
		// We transform time backwards for cooldown calculation to prevent
		// false cooldown hack messages.
		var lastUsage = (float)(uLink.Network.time) - info.GetPacketArrivalTime();
		
		// Use the skill
		UseSkill(currentSkill, hitPoint, -1, lastUsage);
		
		// Let others know
		networkView.RPC("InstantCast", uLink.RPCMode.OthersExceptOwner, skillId, hitPoint);
		
		// Animation
		if(animator != null) {
			animator.SetBool(interruptedHash, false);
			animator.SetBool(skillEndedHash, false);
		}
		
		SetCastAnimationState(0, 1);
		
		// Anim duration
		DelayedEndSkill(info.timestamp);
	}
	
	// Anim duration
	void DelayedEndSkill(double timestamp) {
		float timeToArrive = (float)(uLink.Network.time - timestamp);
		float animDuration = (currentSkill.currentStage.attackAnimDuration * attackSpeedMultiplier) - timeToArrive;
		
		if(animDuration <= 0) {
			EndSkill();
		} else {
			Invoke("EndSkill", animDuration);
		}
		
		//Invoke("EndSkill", currentSkill.currentStage.attackAnimDuration * attackSpeedMultiplier);
	}
	
	// Respawning
	[RPC]
	public void Respawn(Vector3 spawnPosition) {
		this.BasicRespawn(spawnPosition);
		
		// Set client position
		clientPosition = myTransform.position;
	}
	
	[RPC]
	void ClientStartHover(uLink.NetworkMessageInfo info) {
		// Make sure we throw away messages from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		if(energy < Config.instance.blockMinimumEnergyForUsage)
			return;
		
		if(!canAct)
			return;
		
		if(!canHover)
			return;
		
		if(!hovering) {
			// Predict client's current position
			//PredictClientPosition(info.GetPacketArrivalTime(), this.moveVector, this.moveSpeedModifier + Player.hoverSpeedBonus);
			
			if(this.StartHover()) {
				networkView.RPC("StartHover", uLink.RPCMode.OthersExceptOwner);
			}
		}
	}
	
	[RPC]
	void ClientEndHover(uLink.NetworkMessageInfo info) {
		// Make sure we throw away messages from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		if(hovering) {
			// Predict client's current position
			//PredictClientPosition(info.GetPacketArrivalTime(), this.moveVector, this.moveSpeedModifier - Player.hoverSpeedBonus);
			
			if(EndHover()) {
				networkView.RPC("EndHover", uLink.RPCMode.OthersExceptOwner);
			}
		}
	}
	
	[RPC]
	void ClientStartBlock(uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner || info.timestamp <= ltsClientStartBlock)
			return;
		
		// Can't block while stunned
		if(stunned > 0 || slept > 0 || !canAct)
			return;
		
		// Not enough block capacity?
		if(energy < Config.instance.blockMinimumEnergyForUsage)
			return;
		
		// Predict client position
		float timePacketArrival = (float)(uLink.Network.time - info.timestamp);
		PredictClientPosition(timePacketArrival, moveVector);
		
		// Start blocking
		networkView.RPC("StartBlock", uLink.RPCMode.Others);
		StartBlock();
		
		ltsClientStartBlock = info.timestamp;
	}
	
	[RPC]
	void ClientEndBlock(uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner || info.timestamp <= ltsClientEndBlock)
			return;
		
		// End blocking
		networkView.RPC("EndBlock", uLink.RPCMode.Others);
		EndBlock();
		
		ltsClientEndBlock = info.timestamp;
	}
	
	[RPC]
	void ClientAttunement(byte attunementId, uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner || info.timestamp <= ltsClientAttunement)
			return;
		
		// Invalid ID
		if(attunementId >= attunements.Count)
			return;
		
		// Dead or game ended or didn't start?
		if(!canAct)
			return;
		
		Attunement requestedAttunement = attunements[attunementId];
		
		// Anti attunement bonus hack
		if(currentAttunement == requestedAttunement)
			return;
		
		// Anti cooldown hack
		if(requestedAttunement.isOnCooldown)
			return;
		
		networkView.RPC("SwitchAttunement", uLink.RPCMode.OthersExceptOwner, attunementId);
		SwitchAttunement(attunementId);
		
		ltsClientAttunement = info.timestamp;
	}
	
	[RPC]
	public void ClientStopHolding(uLink.NetworkMessageInfo info) {
		// Make sure we throw away messages from the wrong client
		if(info.sender != networkView.owner)
			return;
		
		networkView.RPC("StopHoldingSkill", uLink.RPCMode.OthersExceptOwner);
		StopHoldingSkill();
	}
	
	[RPC]
	void ClientWeapon(byte weaponSlotId, uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner || info.timestamp <= ltsClientWeapon)
			return;
		
		// Invalid ID
		if(weaponSlotId >= weapons.Count)
			return;
		
		// Dead or game ended or didn't start?
		if(!canAct)
			return;
		
		Weapon requestedWeapon = weapons[weaponSlotId];
		
		// Anti weapon bonus hack
		if(currentWeapon == requestedWeapon)
			return;
		
		networkView.RPC("SwitchWeapon", uLink.RPCMode.All, weaponSlotId);
		
		ltsClientWeapon = info.timestamp;
	}
	
	[RPC]
	public void ClientSetTarget(ushort entityId, uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner || info.timestamp <= ltsClientTarget)
			return;
		
		// Dead or game ended or didn't start?
		if(!canAct)
			return;
		
		networkView.RPC("SetTarget", uLink.RPCMode.All, entityId);
		
		ltsClientTarget = info.timestamp;
	}
	
	[RPC]
	void ClientChat(string entry, uLink.NetworkMessageInfo info) {
		// Make sure we throw away late and duplicate RPC messages, or from the wrong client
		if(info.sender != networkView.owner || info.timestamp <= ltsClientChat)
			return;
		
		// TODO: Chat commands
		if(entry == "") {
			// ...
		} else {
			Log("Chat message: " + entry);
			networkView.RPC("Chat", uLink.RPCMode.Others, this.name + ": " + entry);
		}
		
		ltsClientChat = info.timestamp;
	}
#endregion

#region Live update RPCs
	[RPC]
	void ClientCharacterCustomization(CharacterCustomization newCustom, uLink.NetworkMessageInfo info) {
		if(info.sender != networkView.owner)
			return;
		
		this.networkView.RPC("ReceiveCharacterCustomization", uLink.RPCMode.OthersExceptOwner, newCustom);
		ReceiveCharacterCustomization(newCustom);
	}
	
	// Live updates
	
	[RPC]
	void GuildRepresentRequest(string guildId, bool represent, uLink.NetworkMessageInfo info) {
		if(info.sender != networkView.owner)
			return;
		
		// Stop representing
		if(!represent) {
			networkView.RPC("ReceiveMainGuildInfo", uLink.RPCMode.All, "", "");
			return;
		}
		
		// Represent new guild
		GuildsDB.GetGuild(guildId, guild => {
			if(guild != null) {
				networkView.RPC("ReceiveMainGuildInfo", uLink.RPCMode.All, guild.name, guild.tag);
			}
		});
	}
	
	[RPC]
	void CharacterStatsUpdate(CharacterStats nCharStats, uLink.NetworkMessageInfo info) {
		if(info.sender != networkView.owner)
			return;
		
		networkView.RPC("ReceiveCharacterStats", uLink.RPCMode.All, nCharStats);
	}
	
	[RPC]
	void ArtifactTreeUpdate(string jsonArtifactTree, uLink.NetworkMessageInfo info) {
		if(info.sender != networkView.owner)
			return;
		
		networkView.RPC("ReceiveArtifactTree", uLink.RPCMode.All, jsonArtifactTree);
	}
	
	[RPC]
	void SkillBuildUpdate(SkillBuild nSkillBuild, uLink.NetworkMessageInfo info) {
		if(info.sender != networkView.owner)
			return;
		
		networkView.RPC("ReceiveSkillBuild", uLink.RPCMode.All, nSkillBuild);
	}
#endregion
}
