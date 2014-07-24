using UnityEngine;

public class PlayerMain : PlayerOnClient {
	private GameObject camPivot;
	private CameraMode cameraMode;

	// Awake
	protected override void Awake() {
		base.Awake();
		
		// Controller
		controller = new HumanController(this);
		
		// Input manager
		inputManager = InputManager.instance;
		buttons = new HumanController.InputButtons(inputManager);
		
		// Make the camera follow me
		var cameraModes = GameObject.Find("CameraModes");
		cameraModes.transform.parent = myTransform;
		cameraModes.transform.localPosition = Cache.vector3Zero;
		cameraModes.transform.localRotation = Cache.quaternionIdentity;

		// Assign values
		camPivot = GameObject.FindGameObjectWithTag("CamPivot");
		cameraMode = camPivot.GetComponent<CameraMode>();
		crossHair = GetComponent<CrossHair>();
		
		// Set main player
		Player.main = this;
		Player.main.isVisible = true;

		// Destroy
		onDestroy += () => {
			Player.main = null;
		};

		// ChangeParty
		onChangeParty += UpdateCameraYRotation;
	}
	
	// Start
	void Start() {
		Screen.showCursor = false;
		crossHair.enabled = true;

		InvokeRepeating("SendToServer", 0.001f, 1.0f / (uLink.Network.sendRate * 2.0f));
	}
	
#region Update
	// Update my position to the position the server sent me
	void Update() {
		// Player is ready
		CheckReadiness();
		
		// Input
		UpdateInput();
		
		// Movement
		UpdateOwnerMovement();
		
		// Debug
		UpdateDebugger();
		
		// Animations
		UpdateSkillAnimations();
		
		// Map boundaries
		StayInMapBoundaries();
	}
	
	// Movement of the owner
	void UpdateOwnerMovement() {
		// Interpolate position
		if(!ignoreNewPosition) {
			float serverDistanceSqr = (myTransform.position - serverPosition).sqrMagnitude;
			if(serverDistanceSqr >= 500.0f) {
				// Snap to correct position
				if(disableSnappingToNewPosition == 0)
					myTransform.position = serverPosition;
			} else if(!hasControlOverMovement) {
				// For example on Shockwave and Black Hole we interpolate faster
				characterController.Move((serverPosition - myTransform.position) * Time.deltaTime * Config.instance.ownerInterpolationSpeed * 16.0f);
			} else if(serverDistanceSqr >= maxDistanceToServerSqr) {
				characterController.Move((serverPosition - myTransform.position) * Time.deltaTime * Config.instance.ownerInterpolationSpeed);
			}
		}
		
		// Did the game start yet?
		if(!GameManager.gameStarted)
			return;
		
		// Move speed
		ApplyMotorMoveSpeed(this.moveSpeed);
		
		// Target and I alive?
		if(target != null && (!this.isAlive || !target.isAlive) && !noTargetSent) {
			networkView.RPC("ClientNoTarget", uLink.RPCMode.Server);
			noTargetSent = true;
		}
		
		// Get move vector
		if(cantMove) {
			moveVector = Cache.vector3Zero;
			movementKeysPressed = false;
			motor.inputJump = false;
		} else {
			controller.Update();
		}
		
		// Movement animation
		animator.SetBool("Moving", movementKeysPressed);
		
		// Normalize move vector when needed, same speed for all directions
		//if(moveVector.sqrMagnitude > 1)
		if(moveVector != Cache.vector3Zero)
			moveVector.Normalize();
		
		// Move speed
		ApplyMotorMoveSpeed(this.moveSpeed);
		
		// Client side
		motor.inputMoveDirection = moveVector;
		motor.inputFly = hovering;
		
		jumping = motor.jumping.jumping && !motor.grounded; //&& !characterController.isGrounded;
		animator.SetBool("Jump", jumping);
		
		// What are we aiming at?
		UpdateRaycast();
	}
	
	// Update input
	void UpdateInput() {
		if(!isAlive || GameManager.gameEnded || !GameManager.gameStarted) {
			// Cancel old skill
			if(currentSkill != null) {
				InterruptCast(); //false
			}
			
			return;
		}
		
		// Update controller
		controller.UpdateInput();
		
		// Casts
		if(skillBuild != null) {
			// New skill casts
			UpdateStartCast();
			
			// Weapon switching
			UpdateWeaponSwitching();
			
			// Attunement switching
			UpdateAttunementSwitching();
		}
		
		// Blocking
		blockKeyDown = inputManager.GetButton(buttons.Block);
		
		if(stunned == 0 && slept == 0 && blockKeyDown != lastBlockStateSent) {
			if(blockKeyDown) {
				if(energy >= Config.instance.blockMinimumEnergyForUsage) {
					networkView.RPC("ClientStartBlock", uLink.RPCMode.Server);
					lastBlockStateSent = blockKeyDown;
				}
			} else {
				networkView.RPC("ClientEndBlock", uLink.RPCMode.Server);
				lastBlockStateSent = blockKeyDown;
			}
		}
		
		// Hovering
		bool hoverKeyPressed = inputManager.GetButton(buttons.Hover);
		bool canHoverNow = movementKeysPressed && hoverKeyPressed && this.canHover;
		
		if(!hovering && canHoverNow && energy >= Config.instance.blockMinimumEnergyForUsage) {
			if(StartHover())
				networkView.RPC("ClientStartHover", uLink.RPCMode.Server);
		} else if(hovering && (!canHoverNow || energy <= 0)) {
			if(EndHover())
				networkView.RPC("ClientEndHover", uLink.RPCMode.Server);
		}
		
		// Jumping
		motor.inputJump = inputManager.GetButton(buttons.Jump);
		
		if(motor.inputJump != lastJumpSent) {
			networkView.RPC("ClientJumpState", uLink.RPCMode.Server, motor.inputJump);
			
			lastJumpSent = motor.inputJump;
		}
		
		// Target focus
		if(inputManager.GetButtonDown(buttons.ToggleTargetFocus)) {
			// TODO: Lose target on enemy death
			// TODO: RPC to server
			if(target == null && aboutToTarget != null) {
				networkView.RPC("ClientTarget", uLink.RPCMode.Server, aboutToTarget.networkView.owner.id);
			} else {
				networkView.RPC("ClientNoTarget", uLink.RPCMode.Server);
			}
		}
		
		// Action key
		if(actionTarget != null && actionTarget.CanAction(this) && !MainMenu.instance.enabled) {
			// TODO: Draw info that user can use an action key
			
			if(inputManager.GetButtonDown(buttons.Action))
				actionTarget.OnAction(this);
		}
		
		// Show all entities
		if(inputManager.GetButton(buttons.ShowAllEntities)) {
			Player.showAllEntities = true;
		} else {
			Player.showAllEntities = false;
		}
		
		// Show all entities
		if(inputManager.GetButton(buttons.TargetMyself)) {
			target = this;
		} else {
			target = null;
		}
	}
	
	// Update attunement switching
	void UpdateAttunementSwitching() {
		byte attunementId = Attunement.Undefined;
		
		if(inputManager.GetButtonDown(buttons.PreviousAttunement)) {
			if(currentAttunementId == 0)
				attunementId = (byte)(attunements.Count - 1);
			else
				attunementId = (byte)(currentAttunementId - 1);
		} else if(inputManager.GetButtonDown(buttons.NextAttunement)) {
			if(currentAttunementId == attunements.Count - 1)
				attunementId = 0;
			else
				attunementId = (byte)(currentAttunementId + 1);
		} else {
			for(byte counter = 0; counter < attunements.Count; counter++) {
				if(inputManager.GetButton(buttons.Attunements[counter])) {
					attunementId = counter;
					break;
				}
			}
		}
		
		if(attunementId != Attunement.Undefined) {
			Attunement requestedAttunement = attunements[attunementId];
			
			if(requestedAttunement != currentAttunement && !requestedAttunement.isOnCooldown) {
				// TODO: Do not send multiple times
				networkView.RPC("ClientAttunement", uLink.RPCMode.Server, attunementId);
				
				// We assume that the server allowed it
				SwitchAttunement(attunementId);
			}
		}
	}
	
	// Update weapon switching
	void UpdateWeaponSwitching() {
		byte weaponId = Weapon.Undefined;
		
		for(byte counter = 0; counter < weapons.Count; counter++) {
			if(inputManager.GetButton(buttons.Weapons[counter])) {
				weaponId = counter;
				break;
			}
		}
		
		if(weaponId != Weapon.Undefined) {
			Weapon requestedWeapon = weapons[weaponId];
			
			if(requestedWeapon != currentWeapon) {
				// TODO: Do not send multiple times
				networkView.RPC("ClientWeapon", uLink.RPCMode.Server, weaponId);
			}
		}
	}
	
	// Ray cast to target position
	void UpdateRaycast() {
		// Reset selected player
		this.selectedEntity = null;
		
		Ray ray = cam.ViewportPointToRay(cameraMode.GetCursorPosition3D());
		
		// Do the raycast
		if(Physics.Raycast(ray, out raycastHit, Config.instance.raycastMaxDistance)) {
			// TODO: Targeting info
			GameObject targetObject = raycastHit.collider.gameObject;
			var targetEntity = targetObject.GetComponent<Entity>();
			
			if(targetEntity != this) {
				if(targetEntity != null) {
					if(targetEntity.layer != this.layer) {
						crossHair.color = Color.red;
					} else {
						crossHair.color = Color.green;
					}
					
					crossHair.width = (int)(crossHair.defaultWidth * 1.1f);
					crossHair.height = (int)(crossHair.defaultHeight * 1.1f);
					
					// Remove hitPoint Y
					raycastHitPointOnGround = new Vector3(raycastHit.point.x, targetEntity.myTransform.position.y, raycastHit.point.z);
					
					this.selectedEntity = targetEntity;
				} else {
					crossHair.color = crossHair.defaultColor;
					crossHair.width = crossHair.defaultWidth;
					crossHair.height = crossHair.defaultHeight;
					raycastHitPointOnGround = raycastHit.point;
				}
				
				raycastSuccess = true;
			} else {
				crossHair.color = crossHair.defaultColor;
				raycastSuccess = false;
			}
		} else {
			crossHair.color = crossHair.defaultColor;
			raycastSuccess = false;
		}
	}
	
	// Update debugger
	void UpdateDebugger() {
		if(Debugger.instance.debugCasting) {
			Debugger.Label("CurrentSkill: " + (currentSkill != null ? currentSkill.skillName : "null"));
	        Debugger.Label("CurrentSpellID: " + currentSpellId);
			Debugger.Label("LastEndCastSpellID: " + lastEndCastSpellId);
			Debugger.Label("SkillEnded: " + animator.GetBool(skillEndedHash));
			Debugger.Label("Interrupted: " + animator.GetBool(interruptedHash));
		}

		if(Debugger.instance.debugAnimator) {
			Debugger.Label("Speed: " + animator.speed);

			Debugger.Label("Has root motion: " + animator.hasRootMotion);
			Debugger.Label("Has transform hierarchy: " + animator.hasTransformHierarchy);
			Debugger.Label("Human scale: " + animator.humanScale);

			Debugger.Label("Root position: " + animator.rootPosition);
			Debugger.Label("Root rotation: " + animator.rootRotation);

			Debugger.Label("Body position: " + animator.bodyPosition);
			Debugger.Label("Body rotation: " + animator.bodyRotation);

			Debugger.Label("Target position: " + animator.targetPosition);
			Debugger.Label("Target rotation: " + animator.targetRotation);

			Debugger.Label("Stabilize feet: " + animator.stabilizeFeet);
			Debugger.Label("Left feet bottom height: " + animator.leftFeetBottomHeight);
			Debugger.Label("Right feet bottom height: " + animator.rightFeetBottomHeight);

			var infoArray = animator.GetCurrentAnimationClipState(1);
			Debugger.Label("Array elements: " + infoArray.Length);
			foreach(var info in infoArray) {
				Debugger.Label(" - " + info.clip.name + ": " + info.clip.length);
			}
		}
	}

	// UpdateCameraYRotation
	public void UpdateCameraYRotation() {
		if(party == null)
			return;
		
		if(party.spawn == null)
			return;
		
		// Set camera Y rotation
		UpdateCameraYRotation(party.spawn);
	}

	// UpdateCameraYRotation
	public void UpdateCameraYRotation(Transform spawn) {
		SetCameraYRotation(spawn.transform.eulerAngles.y);
	}
#endregion
	
	// Checks when the loading screen can be disabled
	void CheckReadiness() {
		// Check if we are ready
		if(!MapManager.mapLoaded || !appliedCharacterStats || customization == null)
			return;

		// Don't send Ready RPC again
		if(isReady)
			return;

		// Fade out loading screen
		if(LoadingScreen.instance != null)
			LoadingScreen.instance.Disable();
		
		// Send readiness RPC
		LogManager.General.Log("Client is ready, informing the server...");
		networkView.RPC("ClientReady", uLink.RPCMode.Server);
		Ready();
		
		// Start intro
		var intro = MapManager.mapIntro;
		if(intro != null)
			intro.Restart();
	}
	
	// Informs the auth. server about the movement we are requesting
	void SendToServer() {
		if(!isReady)
			return;
		
		var myBitMask = this.bitMask;
		
		if(movementKeysPressed || jumping || motor.movement.velocity != Cache.vector3Zero) {
			// Send unreliable state RPC
			networkView.UnreliableRPC(rpcClientState, uLink.RPCMode.Server,
				myTransform.position,
				moveVector,
				(byte)myBitMask
				//camTransform.position
			);
			
			//lastCamPosition = camTransform.position;
			reliablePosSent = false;
		} else {
			// Send reliable state RPC
			if(!reliablePosSent || bitMask != lastReliableBitMaskSent) {
				networkView.RPC(rpcClientState, uLink.RPCMode.Server,
					myTransform.position,
					moveVector,
					(byte)myBitMask
					//camTransform.position
				);
				
				//lastCamPosition = camTransform.position;
				reliablePosSent = true;
				lastReliableBitMaskSent = bitMask;
				//LogManager.General.Log("Sent reliable move RPC");
			}
			
			// Send camera only
			/*if(camTransform.position != lastCamPosition) {
				// Send unreliable camera state RPC
				networkView.UnreliableRPC(rpcCameraState, uLink.RPCMode.Server,
					camTransform.position
				);
				
				lastCamPosition = camTransform.position;
				reliableCamPosSent = false;
			} else {
				// Send reliable camera state RPC
				if(!reliableCamPosSent) {
					networkView.RPC(rpcCameraState, uLink.RPCMode.Server,
						camTransform.position
					);
					
					reliableCamPosSent = true;
					//LogManager.General.Log("Sent reliable cam RPC");
				}
			}*/
		}
		
		// Send hit point when holding skill
		if(currentSkill != null && currentSkill.canHold && lastSkillInstance) {
			networkView.UnreliableRPC(rpcClientHitPoint, uLink.RPCMode.Server, GetHitPoint());
		}
	}
	
	// GetHitPoint
	protected override Vector3 GetHitPoint() {
		if(castStart.target != null) {
			// Make sure we select the hit point that is on the ground.
			// Otherwise the skill would be created ABOVE the target and not hit it.
			if(currentSkill.currentStage.posType == Skill.PositionType.AtGround) {
				raycastHitPointOnGround = castStart.target.center;
				raycastHitPointOnGround.y = castStart.target.myTransform.position.y;
				return raycastHitPointOnGround;
			} else {
				return castStart.target.center;
			}
		}
		
		// Firing with or without target?
		if(raycastSuccess) {
			// Make sure we select the hit point that is on the ground.
			// Otherwise the skill would be created ABOVE the target and not hit it.
			if(currentSkill.currentStage.posType == Skill.PositionType.AtGround) {
				return raycastHitPointOnGround;
			} else {
				return raycastHit.point;
			}
			//projectileRotation = Quaternion.LookRotation(raycastHit.point - rightHand.position);
			//DLog("Firing with target");
		} else {
			return myTransform.position + cam.transform.forward * 30.0f;
			//projectileRotation = cam.transform.rotation;
			//DLog("Firing without target");
		}
	}

	// OnCastRestart
	protected override void OnCastRestart() {
		StartCoroutine(
			UpdateEndCast(
				currentSkill.currentStage.castDuration * attackSpeedMultiplier,
				currentSpellId
			)
		);
	}
	
	// Finds a new target
	Entity FindTarget(float radius) {
		var colliders = Physics.OverlapSphere(this.myTransform.position, radius, this.enemiesLayerMask);
		
		// Sort by distance
		Entity foundTarget = null;
		float lowestDistanceSqr = float.MaxValue;
		
		foreach(var collider in colliders) {
			var nTarget = collider.GetComponent<Entity>();
			if(nTarget != null) {
				float distanceSqr = (collider.transform.position - myTransform.position).sqrMagnitude;
				if(distanceSqr < lowestDistanceSqr) {
					foundTarget = nTarget;
				}
			}
		}
		
		return foundTarget;
	}
	
	// Did we disconnect
	bool Disconnected() {
		return uLink.Network.peerType == uLink.NetworkPeerType.Disconnected;
	}
	
	// Process client chat commands
	public override bool ProcessClientChatCommand(string msg) {
		msg = msg.ReplaceCommands();
		
		int spacePos = msg.IndexOf(' ');
		
		string msgUntilWhitespace = msg;
		string msgFromWhitespace = "";
		
		if(spacePos != -1) {
			msgUntilWhitespace = msg.Substring(0, spacePos);
			msgFromWhitespace = msg.Substring(spacePos);
		}
		
		// Shortcuts
		switch(msgUntilWhitespace) {
			case "//ssp":
				msgUntilWhitespace = "//show serverPosition";
				break;
				
			case "//hsp":
				msgUntilWhitespace = "//hide serverPosition";
				break;
				
			case "//isp":
				msgUntilWhitespace = "//ignore serverPosition";
				break;
				
			case "//asp":
				msgUntilWhitespace = "//acknowledge serverPosition";
				break;
				
			case "//gmdts":
				msgUntilWhitespace = "//get maxDistanceToServer";
				break;
				
			case "//sle":
				msgUntilWhitespace = "//set latencyEmulation";
				break;
				
			case "//gle":
				msgUntilWhitespace = "//get latencyEmulation";
				break;
				
			case "//stm":
				msgUntilWhitespace = "//skillTestMode";
				break;
				
			case "//dgui":
				msgUntilWhitespace = "//debug gui";
				break;
				
			case "//dcast":
				msgUntilWhitespace = "//debug cast";
				break;
				
			case "//si":
				msgUntilWhitespace = "//show intro";
				break;
				
			case "//dc":
				msgUntilWhitespace = "//disconnect";
				break;
		}
		
		msg = msgUntilWhitespace + msgFromWhitespace;
		
		bool show = msg.StartsWith("//show ");
		bool hide = msg.StartsWith("//hide ");
		
		bool paramSet = msg.StartsWith("//set ");
		bool paramGet = msg.StartsWith("//get ");
		
		if(show || hide) {
			string subject = msg.Substring(7);
			bool visible = show || !hide;
			
			if(subject == "serverPosition") {
				Debugger.instance.showServerPosition = visible;
				
				// Show server position for all players
				/*foreach(Party pty in GameServerParty.partyList) {
					foreach(Player player in pty.members) {
						PlayerMovementClient pmc = player.GetComponent<PlayerMovementClient>();
						pmc.serverPositionVisualization.gameObject.SetActive(showServerPosition);
						pmc.serverPositionVisualization.transform.position = pmc.serverPosition;
						pmc.showServerPosition = showServerPosition;
					}
				}*/
				
				return true;
			} else if(subject == "intro") {
				MapManager.mapInstance.GetComponent<Intro>().Restart();
				
				return true;
			}
		} else if(msg.StartsWith("//debug ")) {
			string subject = msg.Substring(8);
			
			// Cast debug
			if(subject == "cast" || subject == "casting") {
				Debugger.instance.debugCasting = !Debugger.instance.debugCasting;
				return true;
			// GUI debug
			} else if(subject == "gui") {
				Debugger.instance.debugGUI = !Debugger.instance.debugGUI;
				return true;
			} 
		} else if(paramSet || paramGet) {
			string[] parts = msg.Split(' ');
			
			if(paramSet && parts.Length == 3) {
				string key = parts[1];
				float val = float.Parse(parts[2]);
				
				if(key == "maxDistanceToServer") {
					maxDistanceToServer = val;
					return true;
				} else if(key == "genericAnimDuration") {
					Skill.genericAnimDuration = val;
					return true;
				} else if(key == "latencyEmulation") {
					uLink.Network.emulation.minLatency = val / 1000f;
					uLink.Network.emulation.maxLatency = uLink.Network.emulation.minLatency;
					return true;
				} /*else {
					// TODO: Remove this on the final release
					var field = typeof(PlayerMovementClient).GetField(key);
					field.SetValue(this, val);
					return true;
				}*/
			} else if (paramGet && parts.Length == 2) {
				string key = parts[1];
				
				if(key == "maxDistanceToServer") {
					lobbyChat.AddEntry(maxDistanceToServer.ToString("0.00"));
					return true;
				} else if(key == "genericAnimDuration") {
					lobbyChat.AddEntry(Skill.genericAnimDuration.ToString("0.00"));
					return true;
				} else if(key == "latencyEmulation") {
					lobbyChat.AddEntry((uLink.Network.emulation.minLatency * 1000).ToString("0") + " ms");
					return true;
				}
			}
		} else if(msg == "//help") {
			lobbyChat.AddEntry("F1-F4 = Skill sets");
			lobbyChat.AddEntry("1-5 = Skills");
			lobbyChat.AddEntry("Right mouse button = Block");
			lobbyChat.AddEntry("Tab = Scoreboard");
			lobbyChat.AddEntry("Shift = Fly/Hover");
			lobbyChat.AddEntry("F = Action");
			lobbyChat.AddEntry("Esc = Lock/unlock mouse");
			
			return true;
		} else if(msg == "//disconnect") {
			if(Login.instance != null)
				MainMenu.instance.Return();
			else
				uLink.Network.Disconnect();
			
			return true;
		} else if(msg == "//ignore serverPosition") {
			ignoreNewPosition = true;
			
			return true;
		} else if(msg == "//ignore serverPosition") {
			ignoreNewPosition = true;
			
			return true;
		} else if(msg == "//acknowledge serverPosition") {
			ignoreNewPosition = false;
			
			return true;
		} else if(msg == "//skillTestMode") {
			Debugger.instance.skillTestMode = !Debugger.instance.skillTestMode;
			
			return true;
		}
		
		// Client simulation commands
		/*if(msg == "//simulate die") {
			this.curHP = 0;
			this.OnPlayerDeath();
			return true;
		}*/
		
		return false;
	}

	[RPC]
	public void SetCameraYRotation(float yAngle) {
		camPivot.transform.eulerAngles = new Vector3(
			camPivot.transform.eulerAngles.x,
			yAngle,
			camPivot.transform.eulerAngles.z
		);
	}
}
