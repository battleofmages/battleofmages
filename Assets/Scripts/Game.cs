using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour {
	public static Game Instance { get; private set; }

	public Player player;
	public Client client;
	public PlayerInput input;
	public Chat chat;
	public GameObject menu;
	public Config config;
	public Skills skills;
	
	public static Player Player { get{ return Instance.player; } }
	public static Client Client { get{ return Instance.client; } }
	public static Chat Chat { get{ return Instance.chat; } }
	public static PlayerInput Input { get{ return Instance.input; } }
	public static GameObject Menu { get{ return Instance.menu; } }
	public static Config Config { get{ return Instance.config; } }
	public static Skills Skills { get{ return Instance.skills; } }

	private void Awake() {
		Instance = this;
	}

	public static void Start() {
		// Disable main menu
		Menu.SetActive(false);

		// Disable interactive UI
		UI.Deactivate();

		// // Swap camera to player camera
		CameraManager.SetActiveCamera(Client.cam);

		// Bind gameplay events
		Input.actions["Move"].performed += Client.Move;
		Input.actions["Move"].canceled += Client.Move;
		Input.actions["Look"].performed += Client.Look;
		Input.actions["Look"].canceled += Client.Look;
		Input.actions["Skill 1"].performed += Client.Skill1;
		Input.actions["Skill 2"].performed += Client.Skill2;
		Input.actions["Skill 3"].performed += Client.Skill3;
		Input.actions["Skill 4"].performed += Client.Skill4;
		Input.actions["Skill 5"].performed += Client.Skill5;
		Input.actions["Block"].performed += Client.StartBlock;
		Input.actions["Block"].canceled += Client.StopBlock;
		Input.actions["Jump"].performed += Client.Jump;
		Input.actions["Chat"].performed += UI.ActivateAndSelectChat;
		Input.actions["Show cursor"].performed += UI.Activate;

		// Bind chat events
		Chat.NewMessage += Client.NewMessage;
	}

	public static void Stop() {
		// Enable main menu
		Menu.SetActive(true);

		// Enable interactive UI
		UI.Activate();

		// Swap camera to default camera
		CameraManager.SetActiveCamera(null);

		// Unbind gameplay events
		Input.actions["Move"].performed -= Client.Move;
		Input.actions["Move"].canceled -= Client.Move;
		Input.actions["Look"].performed -= Client.Look;
		Input.actions["Look"].canceled -= Client.Look;
		Input.actions["Skill 1"].performed -= Client.Skill1;
		Input.actions["Skill 2"].performed -= Client.Skill2;
		Input.actions["Skill 3"].performed -= Client.Skill3;
		Input.actions["Skill 4"].performed -= Client.Skill4;
		Input.actions["Skill 5"].performed -= Client.Skill5;
		Input.actions["Block"].performed -= Client.StartBlock;
		Input.actions["Block"].canceled -= Client.StopBlock;
		Input.actions["Jump"].performed -= Client.Jump;
		Input.actions["Chat"].performed -= UI.ActivateAndSelectChat;
		Input.actions["Show cursor"].performed -= UI.Activate;

		// Unbind chat events
		Chat.NewMessage -= Client.NewMessage;
	}
}
