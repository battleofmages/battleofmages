using UnityEngine;
using BoM.Input;

public class PlayerMain : Entity {
	public RayCaster rayCaster;

	private InputButtons buttons;
	private float horizontalMovement;
	private float verticalMovement;

	public GameObject fireBall;

	// Input buttons
	public class InputButtons {
		public int Forward;
		public int Backward;
		public int Left;
		public int Right;

		public int[] Skills;
		
		// Constructor
		public InputButtons(InputManager inputMgr) {
			Forward = inputMgr.GetButtonIndex("forward");
			Backward = inputMgr.GetButtonIndex("backward");
			Left = inputMgr.GetButtonIndex("left");
			Right = inputMgr.GetButtonIndex("right");
			
			Skills = new int[5];
			
			for(int i = 0; i < Skills.Length; i++) {
				Skills[i] = inputMgr.GetButtonIndex("skill_" + (i + 1));
			}
		}
	}

	// Start
	void Start() {
		buttons = new InputButtons(InputManager.instance);
	}
	
	// Update
	void Update() {
		// Movement
		motor.SetMoveVector(
			InputManager.instance.GetButtonFloat(buttons.Right) - InputManager.instance.GetButtonFloat(buttons.Left) + Input.GetAxis("Horizontal"),
			0f,
			InputManager.instance.GetButtonFloat(buttons.Forward) - InputManager.instance.GetButtonFloat(buttons.Backward) + Input.GetAxis("Vertical")
		);

		// Skill
		if(InputManager.instance.GetButtonDown(buttons.Skills[0])) {
			var clone = (GameObject)GameObject.Instantiate(fireBall, center, Quaternion.LookRotation(rayCaster.hit.point - center));
			clone.GetComponent<SkillInstance>().caster = this;
			clone.transform.parent = Root.instance.skills;
		}
	}

#region Properties
	// Height
	public Vector3 height {
		get {
			return new Vector3(0f, 2f, 0f);
		}
	}

	// Center
	public Vector3 center {
		get {
			return transform.position + height * 0.5f;
		}
	}
#endregion
}
