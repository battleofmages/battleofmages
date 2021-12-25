using BoM.Core;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace BoM.Network {
	[CreateAssetMenu(fileName = "Server", menuName = "BoM/Server", order = 51)]
	public class Server : ScriptableObject {
		public string mapName;
		public Match match;

		public event System.Action Ready;
		public IDatabase database;

		public void Init() {
			NetworkManager.Singleton.ConnectionApprovalCallback += OnApprovalCheck;
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
			SceneManager.sceneLoaded += SceneLoaded;
			SceneManager.LoadScene(mapName, LoadSceneMode.Additive);
		}

		public void Start() {
			Ready += () => {
				NetworkManager.Singleton.StartServer();
				Listen();
			};

			Init();
		}

		public void Listen() {
			NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("client position", ClientPosition);
		}

		public void SceneLoaded(Scene scene, LoadSceneMode mode) {
			//SceneManager.SetActiveScene(scene);
			Ready?.Invoke();
		}

		public void OnApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate sendResponse) {
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

		public void OnClientConnected(ulong clientId) {
			var account = Accounts.Manager.GetByClientId(clientId);
			Debug.Log($"Account ID {account.Id} connected.");
		}

		public void OnClientDisconnected(ulong clientId) {
			var account = Accounts.Manager.GetByClientId(clientId);
			Debug.Log($"Account ID {account.Id} disconnected.");
		}

		public void ClientPosition(ulong senderClientId, FastBufferReader reader) {
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
