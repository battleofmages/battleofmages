using UnityEngine;
using UnityEngine.InputSystem;

namespace BoM {
	public class Game : MonoBehaviour {
		public GameObject menu;
		public Skills.SkillList skills;
		public PlayerInput inputSystem;
		public UI.Chat chatUI;
		public UI.Scoreboard scoreboardUI;
		public UI.Latency latencyUI;

		private void Awake() {
			ConnectToDatabase();
			Players.Player.Added += OnPlayerAdded;
			Players.Player.Added += scoreboardUI.OnPlayerAdded;
			Players.Player.Removed += OnPlayerRemoved;
			Players.Player.Removed += scoreboardUI.OnPlayerRemoved;
			Players.Chat.MessageReceived += OnMessageReceived;
		}

		private void ConnectToDatabase() {
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
		}

		private void OnPlayerAdded(Players.Player player) {
			Network.PlayerManager.AddPlayer(player);
			Cameras.Manager.AddCamera(player.cam);

			if(player.IsOwner) {
				Bind(player);
			}

			var skillSystem = player.GetComponent<Players.SkillSystem>();
			skillSystem.elements = new System.Collections.Generic.List<Skills.Element>();
			var testElement = new Skills.Element();
			testElement.skills = skills.skills;
			skillSystem.elements.Add(testElement);
			BindHealth(player);
		}

		private void OnPlayerRemoved(Players.Player player) {
			Network.PlayerManager.RemovePlayer(player);
			Cameras.Manager.RemoveCamera(player.cam);

			if(player.IsOwner) {
				Unbind(player);
			}
		}

		private void OnPlayerDamage(Players.DamageEvent damageEvent) {
			chatUI.Write("Combat", $"{damageEvent.Receiver.Nick} received {damageEvent.Damage} damage.");
		}

		private void OnPlayerDeath(Players.DamageEvent damageEvent) {
			chatUI.Write("Combat", $"{damageEvent.Receiver.Nick} died.");
		}

		private void OnMessageReceived(Players.Player player, string message) {
			chatUI.Write("Map", $"{player.Nick}: {message}");
		}

		public void Bind(Players.Player player) {
			var latency = player.GetComponent<Players.Latency>();
			var input = player.GetComponent<Players.Input>();
			var chat = player.GetComponent<Players.Chat>();

			// Disable main menu
			menu.SetActive(false);

			// Disable interactive UI
			UI.Manager.Deactivate();

			// Swap camera to player camera
			Cameras.Manager.ActiveCamera = player.cam;

			// Bind gameplay events
			var actions = inputSystem.actions;
			actions["Move"].performed += input.Move;
			actions["Move"].canceled += input.Move;
			actions["Look"].performed += input.Look;
			actions["Look"].canceled += input.Look;
			actions["Skill 1"].performed += input.Skill1;
			actions["Skill 2"].performed += input.Skill2;
			actions["Skill 3"].performed += input.Skill3;
			actions["Skill 4"].performed += input.Skill4;
			actions["Skill 5"].performed += input.Skill5;
			actions["Block"].performed += input.StartBlock;
			actions["Block"].canceled += input.StopBlock;
			actions["Jump"].performed += input.Jump;
			actions["Scoreboard"].performed += scoreboardUI.Show;
			actions["Scoreboard"].canceled += scoreboardUI.Hide;
			actions["Chat"].performed += UI.Manager.ActivateAndSelectChat;
			actions["Show cursor"].performed += UI.Manager.Activate;

			// Bind UI events
			UI.Chat.MessageSubmitted += chat.SubmitMessage;
			latency.Received += latencyUI.OnLatencyReceived;
		}

		public void Unbind(Players.Player player) {
			var latency = player.GetComponent<Players.Latency>();
			var input = player.GetComponent<Players.Input>();
			var chat = player.GetComponent<Players.Chat>();

			// Enable main menu
			menu.SetActive(true);

			// Enable interactive UI
			UI.Manager.Activate();

			// Swap camera to default camera
			Cameras.Manager.ActiveCamera = Cameras.Manager.Instance.cameras[0];

			// Unbind gameplay events
			var actions = inputSystem.actions;
			actions["Move"].performed -= input.Move;
			actions["Move"].canceled -= input.Move;
			actions["Look"].performed -= input.Look;
			actions["Look"].canceled -= input.Look;
			actions["Skill 1"].performed -= input.Skill1;
			actions["Skill 2"].performed -= input.Skill2;
			actions["Skill 3"].performed -= input.Skill3;
			actions["Skill 4"].performed -= input.Skill4;
			actions["Skill 5"].performed -= input.Skill5;
			actions["Block"].performed -= input.StartBlock;
			actions["Block"].canceled -= input.StopBlock;
			actions["Jump"].performed -= input.Jump;
			actions["Scoreboard"].performed -= scoreboardUI.Show;
			actions["Scoreboard"].canceled -= scoreboardUI.Hide;
			actions["Chat"].performed -= UI.Manager.ActivateAndSelectChat;
			actions["Show cursor"].performed -= UI.Manager.Activate;

			// Unbind chat events
			UI.Chat.MessageSubmitted -= chat.SubmitMessage;
			latency.Received -= latencyUI.OnLatencyReceived;
		}

		void BindHealth(Players.Player player) {
			var health = player.GetComponent<Players.Health>();
			health.Damaged += OnPlayerDamage;
			health.Died += OnPlayerDeath;

			var healthBar = player.GetComponentInChildren<UI.Overlays.Bar>();
			var visibility = player.GetComponentInChildren<Players.Visibility>();

			visibility.BecameVisible += () => {
				healthBar.gameObject.SetActive(true);
			};

			visibility.BecameInvisible += () => {
				healthBar.gameObject.SetActive(false);
			};

			health.PercentChanged += healthBar.SetFillAmount;
		}
	}
}
