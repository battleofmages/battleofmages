using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace BoM.Network {
	public static class Server {
		public static event System.Action Ready;
		public static Transform spawn;

		public static void Init() {
			NetworkManager.Singleton.ConnectionApprovalCallback += OnApprovalCheck;
			SceneManager.sceneLoaded += SceneLoaded;
			SceneManager.LoadScene("Arena", LoadSceneMode.Additive);
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
			Ready?.Invoke();
		}

		public static void OnApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callBack) {
			var offset = Random.insideUnitCircle * spawn.GetComponent<SphereCollider>().radius;
			var position = new Vector3(spawn.position.x + offset.x, spawn.position.y, spawn.position.z + offset.y);
			callBack(true, null, true, position, spawn.rotation);
		}

		public static void ClientPosition(ulong senderClientId, FastBufferReader reader) {
			var sender = PlayerManager.FindClientId(senderClientId);

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