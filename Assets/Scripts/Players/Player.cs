using BoM.Core;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Player : Entity, IPlayer {
		public static IDatabase database;
		public static Player main;
		public static event Action<Player> Added;
		public static event Action<Player> Removed;
		public static event Action<Player, string> MessageReceived;

		public NetworkObject networkObject;
		public CharacterController controller;
		public Animations animations;
		public Camera cam;
		public Gravity gravity;
		public Latency latency;
		public float moveSpeed;
		public Transform model;
		public float modelYOffset;
		public GameObject networkShadow;
		public Account Account { get; private set; }
		public ulong ClientId { get; set; }
		public Vector3 RemoteDirection { get; set; }
		private Vector3 remotePosition;
		public NetworkVariable<bool> isReady = new NetworkVariable<bool>(false);

		public string Name {
			get {
				return gameObject.name;
			}
			
			set {
				gameObject.name = value;
				networkShadow.gameObject.name = $"{value} - Shadow";
			}
		}

		public float Latency {
			get {
				return latency.oneWay;
			}
		}

		public Vector3 RemotePosition {
			get {
				return remotePosition;
			}

			set {
				remotePosition = value;
				networkShadow.transform.position = value;
			}
		}

		public override void OnNetworkSpawn() {
			ClientId = networkObject.OwnerClientId;
			Account = database.GetAccount("id" + ClientId);
			Name = Account.Nick;
			RemotePosition = transform.position;

			if(IsOwner) {
				Player.main = this;
			}

			var playerRoot = GameObject.Find("Players");
			transform.SetParent(playerRoot.transform);
			networkShadow.transform.SetParent(playerRoot.transform, true);
			model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);
			EnableNetworkComponents();
			Added?.Invoke(this);

			if(IsOwner) {
				ReadyServerRpc();
			}
		}

		private void OnDisable() {
			if(IsOwner) {
				Player.main = null;
			}

			Removed?.Invoke(this);
			Destroy(networkShadow);
		}

		private void EnableNetworkComponents() {
			if(IsHost && IsOwner) {
				GetComponent<Server>().enabled = true;
				GetComponent<Client>().enabled = true;
				GetComponent<Cursor>().enabled = true;
				return;
			}

			if(IsServer) {
				GetComponent<Server>().enabled = true;
				return;
			}

			if(IsClient && IsOwner) {
				GetComponent<Client>().enabled = true;
				GetComponent<Cursor>().enabled = true;
				return;
			}

			if(IsClient && !IsOwner) {
				// GetComponent<Proxy>().enabled = true;
				return;
			}
		}

		public void Move(Vector3 direction) {
			direction.y = 0f;
			direction.Normalize();

			direction *= moveSpeed;
			direction.y = gravity.Speed;

			controller.Move(direction * Time.deltaTime);
		}

		[ClientRpc]
		public void ReceiveMessageClientRpc(string message) {
			MessageReceived?.Invoke(this, message);
		}

		[ClientRpc]
		public void JumpClientRpc() {
			if(IsOwner) {
				return;
			}
			
			if(!gravity.Jump()) {
				return;
			}
		}

		[ClientRpc]
		public async void UseSkillClientRpc(byte index, Vector3 cursorPosition) {
			if(IsOwner || IsServer) {
				return;
			}

			animations.Animator.SetBool("Attack", true);
			await Task.Delay(300);
			UseSkill(currentElement.skills[index], cursorPosition);
		}

		[ServerRpc]
		public void ReadyServerRpc() {
			isReady.Value = true;
		}
	}
}
