using BoM.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace BoM {
	// Data
	public class GameData : MonoBehaviour {
		[SerializeField] protected GameObject menu;
		[SerializeField] protected Skills.Manager skillManager;
		[SerializeField] protected PlayerInput inputSystem;
		[SerializeField] protected Network.Server server;
		[SerializeField] protected UI.Chat chatUI;
		[SerializeField] protected UI.KillFeed killFeedUI;
		[SerializeField] protected UI.Scoreboard scoreboardUI;
		[SerializeField] protected UI.Latency latencyUI;
		[SerializeField] protected UI.SkillBar skillBarUI;
		[SerializeField] protected VolumeProfile volumeProfile;
		protected MotionBlur motionBlur;
	}

	// Logic
	public class Game : GameData {
		private void Awake() {
			ConnectToDatabase();
			Players.Player.Added += OnPlayerAdded;
			Players.Player.Added += scoreboardUI.OnPlayerAdded;
			Players.Player.Removed += OnPlayerRemoved;
			Players.Player.Removed += scoreboardUI.OnPlayerRemoved;
			Players.Chat.MessageReceived += OnMessageReceived;

			if(!volumeProfile.TryGet(out motionBlur)) {
				throw new System.NullReferenceException(nameof(motionBlur));
			}
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

			server.Database = database;
		}

		private void OnPlayerAdded(Players.Player player) {
			PlayerManager.AddPlayer(player);
			Cameras.Manager.AddCamera(player.Cam);
			player.Team.AddPlayer(player);

			if(player.IsOwner) {
				Bind(player);
			}

			BindHealth(player);
		}

		private void OnPlayerRemoved(Players.Player player) {
			PlayerManager.RemovePlayer(player);
			Cameras.Manager.RemoveCamera(player.Cam);
			player.Team.RemovePlayer(player);

			if(player.IsOwner) {
				Unbind(player);
			}
		}

		private void OnPlayerDamage(Players.DamageEvent damageEvent) {
			chatUI.Write("Combat", $"{damageEvent.Receiver.Nick} received {damageEvent.Damage} damage.");
		}

		private void OnPlayerDeath(Players.DamageEvent damageEvent) {
			killFeedUI.Add(damageEvent.Caster.Nick, damageEvent.Skill.Icon, damageEvent.Receiver.Nick);
			chatUI.Write("Combat", $"{damageEvent.Receiver.Nick} died.");
		}

		private void OnMessageReceived(Players.Player player, string message) {
			chatUI.Write("Map", $"{player.Nick}: {message}");
		}

		public void Bind(Players.Player player) {
			// Get player components
			var (chat, flight, input, latency) = GetPlayerComponents(player);

			// Disable main menu
			menu.SetActive(false);

			// Disable interactive UI
			UI.Manager.Deactivate();

			// Swap camera to player camera
			Cameras.Manager.ActiveCamera = player.Cam;

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
			actions["Fly"].performed += input.StartFlight;
			actions["Fly"].canceled += input.StopFlight;
			actions["Jump"].performed += input.Jump;
			actions["Scoreboard"].performed += scoreboardUI.Show;
			actions["Scoreboard"].canceled += scoreboardUI.Hide;
			actions["Chat"].performed += UI.Manager.ActivateAndSelectChat;
			actions["Show cursor"].performed += UI.Manager.Activate;

			// Effects
			flight.StateChanged += SetMotionBlur;

			// Bind UI events
			UI.Chat.MessageSubmitted += chat.SubmitMessage;
			latency.Received += latencyUI.OnLatencyReceived;
			BindSkillBar(player);
		}

		public void Unbind(Players.Player player) {
			// Get player components
			var (chat, flight, input, latency) = GetPlayerComponents(player);

			// Enable main menu
			menu.SetActive(true);

			// Enable interactive UI
			UI.Manager.Activate();

			// Swap camera to default camera
			Cameras.Manager.ActiveCamera = null;

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
			actions["Fly"].performed -= input.StartFlight;
			actions["Fly"].canceled -= input.StopFlight;
			actions["Jump"].performed -= input.Jump;
			actions["Scoreboard"].performed -= scoreboardUI.Show;
			actions["Scoreboard"].canceled -= scoreboardUI.Hide;
			actions["Chat"].performed -= UI.Manager.ActivateAndSelectChat;
			actions["Show cursor"].performed -= UI.Manager.Activate;

			// Effects
			flight.StateChanged -= SetMotionBlur;

			// Unbind chat events
			UI.Chat.MessageSubmitted -= chat.SubmitMessage;
			latency.Received -= latencyUI.OnLatencyReceived;
			UnbindSkillBar(player);
		}

		private void BindHealth(Players.Player player) {
			var canvas = player.GetComponentInChildren<Canvas>();
			var visibility = player.GetComponentInChildren<Players.Visibility>();

			visibility.BecameVisible += () => {
				canvas.gameObject.SetActive(true);
			};

			visibility.BecameInvisible += () => {
				canvas.gameObject.SetActive(false);
			};

			var bars = canvas.GetComponentsInChildren<UI.Overlays.Bar>();
			var health = player.GetComponent<Players.Health>();
			var healthBar = bars[0];

			health.Damaged += OnPlayerDamage;
			health.Died += OnPlayerDeath;
			health.PercentChanged += healthBar.SetFillAmount;

			var energy = player.GetComponent<Players.Energy>();
			var energyBar = bars[1];

			energy.PercentChanged += energyBar.SetFillAmount;
		}

		private void BindSkillBar(Players.Player player) {
			var skillSystem = player.GetComponent<Players.SkillSystem>();

			ForEachSkillSlot(skillSystem, (index, ui, slot) => {
				ui.Slot = slot;
				ui.Button.onClick.AddListener(() => skillSystem.CastSkillAtIndex(index));
			});
		}

		private void UnbindSkillBar(Players.Player player) {
			var skillSystem = player.GetComponent<Players.SkillSystem>();

			ForEachSkillSlot(skillSystem, (index, ui, slot) => {
				ui.Slot = null;
				ui.Button.onClick.RemoveAllListeners();
			});
		}

		private void ForEachSkillSlot(Players.SkillSystem skillSystem, System.Action<byte, UI.SkillSlot, Skills.Slot> action) {
			var skillSlots = skillSystem.Build.Elements[0].SkillSlots;

			for(byte i = 0; i < skillSlots.Length; i++) {
				action.Invoke(i, skillBarUI.Slots[i], skillSlots[i]);
			}
		}

		private void SetMotionBlur(bool active) {
			motionBlur.active = active;
		}

		private (Players.Chat, Players.Flight, Players.Input, Players.Latency) GetPlayerComponents(Players.Player player) {
			var chat = player.GetComponent<Players.Chat>();
			var flight = player.GetComponent<Players.Flight>();
			var input = player.GetComponent<Players.Input>();
			var latency = player.GetComponent<Players.Latency>();
			return (chat, flight, input, latency);
		}
	}
}
