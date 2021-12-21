using UnityEngine;
using UnityEngine.InputSystem;

namespace BoM {
	public class Game : MonoBehaviour {
		public GameObject menu;
		public Skills.Manager skills;
		public PlayerInput input;
		public UI.Chat chat;
		public UI.Scoreboard scoreboard;
		public UI.LatencyView latencyView;

		private Players.Client client;
		private Players.Latency latency;

		private void Awake() {
			var database = new Database.Memory();

			database.AddAccount(new Accounts.Account("id0", "Player 0 名前", "test0@example.com"));
			database.AddAccount(new Accounts.Account("id1", "Player 1 名前", "test1@example.com"));
			database.AddAccount(new Accounts.Account("id2", "Player 2 名前", "test2@example.com"));
			database.AddAccount(new Accounts.Account("id3", "Player 3 名前", "test3@example.com"));
			database.AddAccount(new Accounts.Account("id4", "Player 4 名前", "test4@example.com"));
			database.AddAccount(new Accounts.Account("id5", "Player 5 名前", "test5@example.com"));
			database.AddAccount(new Accounts.Account("id6", "Player 6 名前", "test6@example.com"));
			database.AddAccount(new Accounts.Account("id7", "Player 7 名前", "test7@example.com"));
			database.AddAccount(new Accounts.Account("id8", "Player 8 名前", "test8@example.com"));
			database.AddAccount(new Accounts.Account("id9", "Player 9 名前", "test9@example.com"));

			Network.Server.database = database;
			Players.Player.database = database;
			Players.Player.Added += OnPlayerAdded;
			Players.Player.Added += scoreboard.OnPlayerAdded;
			Players.Player.Removed += OnPlayerRemoved;
			Players.Player.Removed += scoreboard.OnPlayerRemoved;
			Players.Player.MessageReceived += OnMessageReceived;
		}

		private void OnPlayerAdded(Players.Player player) {
			Network.PlayerManager.AddPlayer(player);
			Cameras.Manager.AddCamera(player.cam);
			player.elements = skills.elements;

			if(player.IsOwner) {
				SetPlayer(player);
				Bind();
			}
		}

		private void OnPlayerRemoved(Players.Player player) {
			Network.PlayerManager.RemovePlayer(player);
			Cameras.Manager.RemoveCamera(player.cam);

			if(player.IsOwner) {
				Unbind();
				SetPlayer(null);
			}
		}

		private void OnMessageReceived(Players.Player player, string message) {
			chat.Write("Map", $"{player.Nick}: {message}");
		}

		public void SetPlayer(Players.Player player) {
			if(player == null) {
				client = null;
				latency = null;
				return;
			}
			
			client = player.GetComponent<Players.Client>();
			latency = player.GetComponent<Players.Latency>();
		}

		public void Bind() {
			// Disable main menu
			menu.SetActive(false);

			// Disable interactive UI
			UI.Manager.Deactivate();

			// Swap camera to player camera
			Cameras.Manager.SetActiveCamera(client.player.cam);

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
			input.actions["Scoreboard"].performed += scoreboard.Show;
			input.actions["Scoreboard"].canceled += scoreboard.Hide;
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
			Cameras.Manager.SetActiveCamera(null);

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
			input.actions["Scoreboard"].performed -= scoreboard.Show;
			input.actions["Scoreboard"].canceled -= scoreboard.Hide;
			input.actions["Chat"].performed -= UI.Manager.ActivateAndSelectChat;
			input.actions["Show cursor"].performed -= UI.Manager.Activate;

			// Unbind chat events
			UI.Chat.MessageSubmitted -= client.SendChatMessage;
			latency.Received -= latencyView.OnLatencyReceived;
		}
	}
}
