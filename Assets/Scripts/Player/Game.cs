using UnityEngine;
using UnityEngine.InputSystem;

namespace BoM.Player {
	public class Game : MonoBehaviour {
		public static Game Instance { get; private set; }

		public static Player Player { get{ return Instance.player; } }
		public static Client Client { get{ return Instance.client; } }
		public static Skill.Manager Skills { get{ return Instance.skills; } }
		public static GameObject Menu { get{ return Instance.menu; } }
		public static PlayerInput Input { get{ return Instance.input; } }
		public static Latency Latency { get{ return Instance.latency; } }
		public static UI.Chat.Chat Chat { get{ return Instance.chat; } }
		public static UI.LatencyView LatencyView { get{ return Instance.latencyView; } }
		
		public GameObject menu;
		public Skill.Manager skills;
		public PlayerInput input;
		public UI.Chat.Chat chat;
		public UI.LatencyView latencyView;

		private Player player;
		private Client client;
		private Latency latency;

		private void Awake() {
			Instance = this;
		}

		public static void SetPlayerObject(GameObject playerObject) {
			if(playerObject == null) {
				Instance.player = null;
				Instance.client = null;
				Instance.latency = null;
				return;
			}
			
			Instance.player = playerObject.GetComponent<Player>();
			Instance.client = playerObject.GetComponent<Client>();
			Instance.latency = playerObject.GetComponent<Latency>();
		}

		public static void Start() {
			// Disable main menu
			Menu.SetActive(false);

			// Disable interactive UI
			UI.Manager.Deactivate();

			// Swap camera to player camera
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
			Input.actions["Chat"].performed += UI.Manager.ActivateAndSelectChat;
			Input.actions["Show cursor"].performed += UI.Manager.Activate;

			// Bind UI events
			Chat.NewMessage += Client.NewMessage;
			Latency.Received += LatencyView.OnLatencyReceived;
		}

		public static void Stop() {
			// Enable main menu
			Menu.SetActive(true);

			// Enable interactive UI
			UI.Manager.Activate();

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
			Input.actions["Chat"].performed -= UI.Manager.ActivateAndSelectChat;
			Input.actions["Show cursor"].performed -= UI.Manager.Activate;

			// Unbind chat events
			Chat.NewMessage -= Client.NewMessage;
			Latency.Received -= LatencyView.OnLatencyReceived;
		}
	}
}
