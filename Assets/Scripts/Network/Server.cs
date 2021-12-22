using BoM.Core;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace BoM.Network {
	public static class Server {
		public static string mapName = "Arena";
		public static event System.Action Ready;
		public static IDatabase database;
		private static Match match;

		public static void Init() {
			CreateMatch();
			NetworkManager.Singleton.ConnectionApprovalCallback += OnApprovalCheck;
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
			SceneManager.sceneLoaded += SceneLoaded;
			SceneManager.LoadScene(mapName, LoadSceneMode.Additive);
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

			match.AddTeam(team1);
			match.AddTeam(team2);
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
			//SceneManager.SetActiveScene(scene);
			Ready?.Invoke();
		}

		public static void OnApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate sendResponse) {
			// Account
			var accountId = System.Text.Encoding.UTF8.GetString(connectionData, 0, connectionData.Length);
			var account = database.GetAccount(accountId);
			Accounts.Manager.AddClient(clientId, account);

			// Team
			var teamId = match.GetTeamIdByAccountId(accountId);

			if(teamId == -1) {
				// Deny connection
				sendResponse(false, null, false, null, null);
				return;
			}

			// Accept connection
			sendResponse(false, null, true, null, null);

			// Find the spawn point
			var spawn = GameObject.Find($"Spawn {teamId + 1}").transform;
			var spawnRadius = spawn.GetComponent<SphereCollider>().radius;
			var offset = Random.insideUnitCircle * spawnRadius;
			var position = new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);
			var rotation = spawn.rotation;

			// Create player object
			var playerPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
			var playerObject = GameObject.Instantiate(playerPrefab, position, rotation);

			// Assign team
			var player = playerObject.GetComponent<IPlayer>();
			player.TeamId = teamId;

			// Spawn player on every client
			var networkObject = playerObject.GetComponent<NetworkObject>();
			networkObject.SpawnAsPlayerObject(clientId);
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