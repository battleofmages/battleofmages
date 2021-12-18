using TMPro;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Player {
	public class Player : Entity, Core.IPlayer {
		public NetworkObject networkObject;
		public TextMeshPro label;
		public CharacterController controller;
		public Gravity gravity;
		
		public float moveSpeed;
		public Transform model;
		public float modelYOffset;
		public GameObject networkShadow;
		public Core.Account Account { get; private set; }
		public ulong ClientId { get; set; }
		public Vector3 RemoteDirection { get; set; }

		private Vector3 realPosition;

		public string Name {
			get {
				return gameObject.name;
			}
			
			set {
				gameObject.name = value;
				label.text = value;
				networkShadow.gameObject.name = $"{value} - Shadow";
			}
		}

		public Vector3 RemotePosition {
			get {
				return realPosition;
			}

			set {
				realPosition = value;
				networkShadow.transform.position = value;
			}
		}

		public override void OnNetworkSpawn() {
			Account = new Core.Account("123", "Test", "test@example.com");
			ClientId = networkObject.OwnerClientId;

			if(IsOwner) {
				Name = "Player " + ClientId;
			} else {
				Name = "Proxy " + ClientId;
			}

			RemotePosition = transform.position;
			elements = Game.Skills.elements;
			model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);
			EnableNetworkComponents();
			Register();
			var playerRoot = GameObject.Find("Players");
			transform.SetParent(playerRoot.transform);
			networkShadow.transform.SetParent(playerRoot.transform, true);
		}

		private void OnDisable() {
			Unregister();
			Destroy(networkShadow);
		}

		private void EnableNetworkComponents() {
			if(IsClient && IsServer && IsOwner) {
				GetComponent<Client>().enabled = true;
				GetComponent<Server>().enabled = true;
				return;
			}

			if(IsServer) {
				GetComponent<Server>().enabled = true;
				GetComponent<Proxy>().enabled = true;
				return;
			}

			if(IsClient && IsOwner) {
				GetComponent<Client>().enabled = true;
				return;
			}

			if(IsClient && !IsOwner) {
				GetComponent<Proxy>().enabled = true;
				return;
			}
		}

		private void Register() {
			Network.PlayerManager.Add(this);
		}

		private void Unregister() {
			Network.PlayerManager.Remove(this);
		}

		public void Move(Vector3 direction) {
			if(direction.sqrMagnitude > 1f) {
				direction.Normalize();
			}

			direction *= moveSpeed;
			direction.y = gravity.Speed;

			controller.Move(direction * Time.deltaTime);
		}
	}
}
