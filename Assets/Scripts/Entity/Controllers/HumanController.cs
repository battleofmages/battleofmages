using UnityEngine;

public class HumanController : Controller {
	private InputManager inputManager;
	private InputButtons buttons;
	private Vector3 inputVector;
	private int currentAdvanceButton;
	protected LobbyChat lobbyChat;
	private byte slotId;

	// Input buttons
	public class InputButtons {
		public int Action;
		public int Block;
		public int Jump;
		public int Up;
		public int Down;
		public int Left;
		public int Right;
		public int PreviousAttunement;
		public int NextAttunement;
		public int Hover;
		public int ToggleTargetFocus;
		public int TargetMyself;
		public int ShowAllEntities;
		
		public int[] Weapons;
		public int[] Attunements;
		public int[] Skills;

		// Constructor
		public InputButtons(InputManager inputMgr) {
			Action = inputMgr.GetButtonIndex("action");
			Block = inputMgr.GetButtonIndex("block");
			Jump = inputMgr.GetButtonIndex("jump");
			Up = inputMgr.GetButtonIndex("forward");
			Down = inputMgr.GetButtonIndex("backward");
			Left = inputMgr.GetButtonIndex("left");
			Right = inputMgr.GetButtonIndex("right");
			PreviousAttunement = inputMgr.GetButtonIndex("prev_attunement");
			NextAttunement = inputMgr.GetButtonIndex("next_attunement");
			Hover = inputMgr.GetButtonIndex("hover");
			ToggleTargetFocus = inputMgr.GetButtonIndex("toggle_target_focus");
			TargetMyself = inputMgr.GetButtonIndex("target_myself");
			ShowAllEntities = inputMgr.GetButtonIndex("show_all_entities");
			
			Skills = new int[5];
			Attunements = new int[4];
			Weapons = new int[2];
			
			for(int i = 0; i < Weapons.Length; i++) {
				Weapons[i] = inputMgr.GetButtonIndex("weapon_" + (i + 1));
			}
			
			for(int i = 0; i < Attunements.Length; i++) {
				Attunements[i] = inputMgr.GetButtonIndex("attunement_" + (i + 1));
			}
			
			for(int i = 0; i < Skills.Length; i++) {
				Skills[i] = inputMgr.GetButtonIndex("skill_" + (i + 1));
			}
		}
	}
	
	private PlayerOnClient player;
	
	private float horizontalMovement;
	private float verticalMovement;
	
	// Constructor
	public HumanController(PlayerOnClient nPlayer) {
		player = nPlayer;

		if(InGameLobby.instance != null)
			lobbyChat = InGameLobby.instance.lobbyChat;
		
		// Input manager
		inputManager = InputManager.instance;
		buttons = new InputButtons(inputManager);
	}
	
	// Update
	public void UpdateInput() {
		// Movement
		horizontalMovement = inputManager.GetButtonFloat(buttons.Right) - inputManager.GetButtonFloat(buttons.Left) + Input.GetAxis("Horizontal");
		verticalMovement = inputManager.GetButtonFloat(buttons.Up) - inputManager.GetButtonFloat(buttons.Down) + Input.GetAxis("Vertical");
		
		player.movementKeysPressed = (horizontalMovement != 0f || verticalMovement != 0f);
	}
	
	// Move vector
	public void Update() {
		var camTransform = player.camTransform;
		
		// Auto target
		bool hasTarget = false; //target != null;
		Quaternion targetRotation = Cache.quaternionIdentity;
		
		if(hasTarget) {
			var target = player.target;
			var myTransform = player.myTransform;

			player.moveVector = (target.myTransform.position - myTransform.position);
			
			if(player.moveVector != Cache.vector3Zero) {
				targetRotation = Quaternion.LookRotation(player.moveVector);
			}
			
			// TODO: Cache camPivot
			var camPivotTransform = GameObject.FindWithTag("CamPivot").transform;
			Quaternion camPivotTargetRotation = Quaternion.LookRotation(target.center - camTransform.position);
			//camPivot.transform.rotation = Quaternion.Lerp(camPivot.transform.rotation, camPivotTargetRotation, Time.deltaTime * 20f);
			//camPivot.GetComponent<MouseLook>().enabled = false;
			
			float fromY = camPivotTransform.localEulerAngles.y;
			float toY = Entity.FixAngleClamping(fromY, camPivotTargetRotation.eulerAngles.y);
			
			camPivotTransform.eulerAngles = new Vector3(
				camPivotTransform.localEulerAngles.x,
				Mathf.Lerp(fromY, toY, Time.deltaTime * 20f),
				camPivotTransform.localEulerAngles.z
			);
		} else {
			targetRotation = camTransform.rotation;
		}
		
		// Only rotate around Y
		targetRotation = CameraMode.current.FixTargetRotation(player, targetRotation);

		// Movement
		inputVector.x = horizontalMovement;
		inputVector.z = verticalMovement;
		player.moveVector = targetRotation * inputVector;
		
		if(hasTarget) {
			player.charGraphics.rotation = targetRotation;
		} else {
			player.LerpCharGraphicsToMoveVector();
		}
	}

	// GetNextSkill
	public byte GetNextSkill() {
		slotId = byte.MaxValue;
		
		// Skill slot buttons
		if(GUIUtility.hotControl == 0) {
			for(byte counter = 0; counter < player.skills.Count; counter++) {
				var skillButton = buttons.Skills[counter];
				
				if(inputManager.GetButton(skillButton)) {
					slotId = counter;
					currentAdvanceButton = skillButton;
					break;
				}
			}
		}
		
		return slotId;
	}

	// OnSkillIsOnCooldown
	public void OnSkillIsOnCooldown() {
		if(inputManager.GetButtonDown(buttons.Skills[slotId]) && lobbyChat != null)
			lobbyChat.AddEntry(player.selectedSkill.skillName + " is still on cooldown.");
	}

	// OnNotEnoughEnergyForSkillCast
	public void OnNotEnoughEnergyForSkillCast() {
		if(inputManager.GetButtonDown(buttons.Skills[slotId]) && lobbyChat != null)
			lobbyChat.AddEntry("Not enough block points for " + player.selectedSkill.skillName + ".");
	}

	// Can start cast
	public bool canStartCast {
		get {
			if(player.skillBuild == null)
				return false;
			
			if(player.talkingWithNPC)
				return false;

			return CameraMode.current.CanStartCast();
		}
	}

	// Wants to advance skill
	public bool holdsSkill {
		get {
			return inputManager.GetButton(currentAdvanceButton);
		}
	}
}