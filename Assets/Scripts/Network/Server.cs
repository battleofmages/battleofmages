using BoM.Core;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace BoM.Network {
	// Data
	public class ServerData : ScriptableObject {
		public IDatabase Database;
		public string MapName;

		[SerializeField] protected Match match;
		[SerializeField] protected Teams.Manager teamManager;
	}

	// Logic
	[CreateAssetMenu(fileName = "Server", menuName = "BoM/Server", order = 51)]
	public class Server : ServerData {
		public event System.Action Ready;

		public void Init() {
			NetworkManager.Singleton.ConnectionApprovalCallback += OnApprovalCheck;
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
			NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
			SceneManager.sceneLoaded += SceneLoaded;
			SceneManager.LoadScene(MapName, LoadSceneMode.Additive);
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
			SceneManager.SetActiveScene(scene);
			teamManager.FindSpawns();
			Ready?.Invoke();
		}

		public void OnApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate sendResponse) {
			// Account
			var accountId = System.Text.Encoding.UTF8.GetString(connectionData, 0, connectionData.Length);
			var account = Database.GetAccount(accountId);
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

			// Create player object
			var team = teamManager.teams[teamId];
			var spawnPosition = team.RandomSpawnPosition;
			var spawnRotation = team.SpawnRotation;
			var playerPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
			var playerObject = GameObject.Instantiate(playerPrefab, spawnPosition, Const.NoRotation);

			// Assign team
			var player = playerObject.GetComponent<IPlayer>();
			player.TeamId = teamId;

			// Spawn player on every client
			var networkObject = playerObject.GetComponent<NetworkObject>();
			networkObject.SpawnAsPlayerObject(clientId);

			player.Respawn(spawnPosition, spawnRotation);
		}

		public void OnClientConnected(ulong clientId) {
			var account = Accounts.Manager.GetByClientId(clientId);
		}

		public void OnClientDisconnected(ulong clientId) {
			var account = Accounts.Manager.GetByClientId(clientId);
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
