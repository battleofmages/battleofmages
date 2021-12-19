using UnityEngine;
using UnityEngine.InputSystem;

namespace BoM {
	public class Game : MonoBehaviour {
		public GameObject menu;
		public Skills.Manager skills;
		public PlayerInput input;
		public UI.Chat chat;
		public UI.LatencyView latencyView;

		private Players.Client client;
		private Players.Latency latency;

		private void Awake() {
			Players.Player.Added += OnPlayerAdded;
			Players.Player.Removed += OnPlayerRemoved;
			Players.Player.MessageReceived += OnMessageReceived;
		}

		private void OnPlayerAdded(Players.Player player) {
			Network.PlayerManager.AddPlayer(player);
			CameraManager.AddCamera(player.cam);

			if(player.IsOwner) {
				SetPlayer(player);
				Bind();
			}
		}

		private void OnPlayerRemoved(Players.Player player) {
			Network.PlayerManager.RemovePlayer(player);
			CameraManager.RemoveCamera(player.cam);

			if(player.IsOwner) {
				Unbind();
				SetPlayer(null);
			}
		}

		private void OnMessageReceived(Players.Player player, string message) {
			chat.Write("Map", $"{player.Name}: {message}");
		}

		public void SetPlayer(Players.Player player) {
			if(player == null) {
				client = null;
				latency = null;
				return;
			}
			
			client = player.GetComponent<Players.Client>();
			latency = player.GetComponent<Players.Latency>();
			player.elements = skills.elements;
		}

		public void Bind() {
			// Disable main menu
			menu.SetActive(false);

			// Disable interactive UI
			UI.Manager.Deactivate();

			// Swap camera to player camera
			CameraManager.SetActiveCamera(client.player.cam);

			// Bind gameplay events
			input.actions["Move"].performed += client.Move;
			input.actions["Move"].canceled += client.Move;
			input.actions["Look"].performed += client.Look;
			input.actions["Look"].canceled += client.Look;
			input.actions["Skill 1"].performed += client.Skill1;
			input.actions["Skill 2"].performed += client.Skill2;
			input.actions["Skill 3"].performed += client.Skill3;
			input.actions["Skill 4"].performed += client.Skill4;
			input.actions["Skill 5"].performed += client.Skill5;
			input.actions["Block"].performed += client.StartBlock;
			input.actions["Block"].canceled += client.StopBlock;
			input.actions["Jump"].performed += client.Jump;
			input.actions["Chat"].performed += UI.Manager.ActivateAndSelectChat;
			input.actions["Show cursor"].performed += UI.Manager.Activate;

			// Bind UI events
			UI.Chat.MessageSubmitted += client.SendChatMessage;
			latency.Received += latencyView.OnLatencyReceived;
		}

		public void Unbind() {
			// Enable main menu
			menu.SetActive(true);

			// Enable interactive UI
			UI.Manager.Activate();

			// Swap camera to default camera
			CameraManager.SetActiveCamera(null);

			// Unbind gameplay events
			input.actions["Move"].performed -= client.Move;
			input.actions["Move"].canceled -= client.Move;
			input.actions["Look"].performed -= client.Look;
			input.actions["Look"].canceled -= client.Look;
			input.actions["Skill 1"].performed -= client.Skill1;
			input.actions["Skill 2"].performed -= client.Skill2;
			input.actions["Skill 3"].performed -= client.Skill3;
			input.actions["Skill 4"].performed -= client.Skill4;
			input.actions["Skill 5"].performed -= client.Skill5;
			input.actions["Block"].performed -= client.StartBlock;
			input.actions["Block"].canceled -= client.StopBlock;
			input.actions["Jump"].performed -= client.Jump;
			input.actions["Chat"].performed -= UI.Manager.ActivateAndSelectChat;
			input.actions["Show cursor"].performed -= UI.Manager.Activate;

			// Unbind chat events
			UI.Chat.MessageSubmitted -= client.SendChatMessage;
			latency.Received -= latencyView.OnLatencyReceived;
		}
	}
}
