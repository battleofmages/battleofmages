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
	
	public static Player Player { get{ return Instance.player; } }
	public static Client Client { get{ return Instance.client; } }
	public static Chat Chat { get{ return Instance.chat; } }
	public static PlayerInput Input { get{ return Instance.input; } }
	public static GameObject Menu { get{ return Instance.menu; } }
	public static Config Config { get{ return Instance.config; } }

	private void Awake() {
		Instance = this;
	}

	public static void Start() {
		//Application.targetFrameRate = 10;
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
		Input.actions["Fire"].performed += Client.Fire;
		Input.actions["Jump"].performed += Client.Jump;
		Input.actions["Chat"].performed += UI.ActivateAndSelectChat;
		Input.actions["ShowCursor"].performed += UI.Activate;

		// Bind chat events
		Chat.NewMessage += Client.NewMessage;
	}

	public static void Stop() {
		// Enable main menu
		Menu.SetActive(true);

		// Enable interactive UI
		UI.Activate();

		// // Swap camera to default camera
		CameraManager.SetActiveCamera(null);

		// Unbind gameplay events
		Input.actions["Move"].performed -= Client.Move;
		Input.actions["Move"].canceled -= Client.Move;
		Input.actions["Look"].performed -= Client.Look;
		Input.actions["Look"].canceled -= Client.Look;
		Input.actions["Fire"].performed -= Client.Fire;
		Input.actions["Jump"].performed -= Client.Jump;
		Input.actions["Chat"].performed -= UI.ActivateAndSelectChat;
		Input.actions["ShowCursor"].performed -= UI.Activate;

		// Unbind chat events
		Chat.NewMessage -= Client.NewMessage;
	}
}
