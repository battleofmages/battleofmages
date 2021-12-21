using BoM.Core;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace BoM.Network {
	public static class Server {
		public static event System.Action Ready;
		public static Transform spawn;
		public static IDatabase database;
		private static float spawnRadius;
		private static Match match;

		public static void Init() {
			CreateMatch();
			NetworkManager.Singleton.ConnectionApprovalCallback += OnApprovalCheck;
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
			SceneManager.sceneLoaded += SceneLoaded;
			SceneManager.LoadScene("Arena", LoadSceneMode.Additive);
		}

		private static void CreateMatch() {
			match = new Match();

			var team1 = new List<string>();
			team1.Add("id0");
			team1.Add("id2");
			team1.Add("id4");
			team1.Add("id6");
			team1.Add("id8");

			var team2 = new List<string>();
			team2.Add("id1");
			team2.Add("id3");
			team2.Add("id5");
			team2.Add("id7");
			team2.Add("id9");

			match.teams.Add(team1);
			match.teams.Add(team2);
		}

		public static void Start() {
			Ready += () => {
				NetworkManager.Singleton.StartServer();
				Listen();
			};

			Init();
		}

		public static void Listen() {
			NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("client position", ClientPosition);
		}

		public static void SceneLoaded(Scene scene, LoadSceneMode mode) {
			spawn = GameObject.FindGameObjectWithTag("Spawn").transform;
			spawnRadius = spawn.GetComponent<SphereCollider>().radius;
			Ready?.Invoke();
		}

		public static void OnApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate respond) {
			var approve = false;
			
			// Account
			var accountId = System.Text.Encoding.UTF8.GetString(connectionData, 0, connectionData.Length);
			var account = database.GetAccount(accountId);
			Accounts.Manager.AddClient(clientId, account);
			
			// Spawn
			var offset = Random.insideUnitCircle * spawnRadius;
			var position = new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);

			// Team
			var teamId = match.GetTeamIdByAccountId(accountId);

			if(teamId != -1) {
				approve = true;
			}

			respond(approve, null, approve, position, spawn.rotation);
		}

		public static void OnClientConnected(ulong clientId) {
			var account = Accounts.Manager.GetByClientId(clientId);
			Debug.Log($"Account ID {account.Id} connected.");
		}

		public static void OnClientDisconnected(ulong clientId) {
			var account = Accounts.Manager.GetByClientId(clientId);
			Debug.Log($"Account ID {account.Id} disconnected.");
		}

		public static void ClientPosition(ulong senderClientId, FastBufferReader reader) {
			var sender = PlayerManager.GetByClientId(senderClientId);

			if(sender == null) {
				return;
			}

			reader.ReadValueSafe(out Vector3 position);
			reader.ReadValueSafe(out Vector3 direction);
			sender.RemotePosition = position;
			sender.RemoteDirection = direction.normalized;
		}
	}
}