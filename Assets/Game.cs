using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour {
	public static Game Instance { get; private set; }

	public Player player;
	public PlayerInput input;
	public Chat chat;
	public GameObject menu;
	
	public static Player Player { get{ return Instance.player; } }
	public static Chat Chat { get{ return Instance.chat; } }
	public static PlayerInput Input { get{ return Instance.input; } }
	public static GameObject Menu { get{ return Instance.menu; } }

	private void Awake() {
		Instance = this;
	}

	public static void Start() {
		// Disable main menu
		Menu.SetActive(false);

		// Disable interactive UI
		UI.Deactivate();

		// Swap camera to player camera
		CameraManager.SetActiveCamera(Player.cam);

		// Bind gameplay events
		Input.actions["Move"].performed += Player.Move;
		Input.actions["Move"].canceled += Player.Move;
		Input.actions["Fire"].performed += Player.Fire;
		Input.actions["Chat"].performed += UI.ActivateAndSelectChat;
		Input.actions["ShowCursor"].performed += UI.Activate;

		// Bind chat events
		Chat.NewMessage += Player.NewMessageServerRpc;
	}

	public static void Stop() {
		// Enable main menu
		Menu.SetActive(true);

		// Enable interactive UI
		UI.Activate();

		// Swap camera to default camera
		CameraManager.SetActiveCamera(null);

		// Unbind gameplay events
		Input.actions["Move"].performed -= Player.Move;
		Input.actions["Move"].canceled -= Player.Move;
		Input.actions["Fire"].performed -= Player.Fire;
		Input.actions["Chat"].performed -= UI.ActivateAndSelectChat;
		Input.actions["ShowCursor"].performed -= UI.Activate;

		// Unbind chat events
		Chat.NewMessage -= Player.NewMessageServerRpc;
	}
}
