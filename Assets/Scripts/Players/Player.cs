using BoM.Core;
using System;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Player : NetworkBehaviour, IPlayer {
		public static Player main;
		public static event Action<Player> Added;
		public static event Action<Player> Removed;

		public Account account;
		public CharacterController controller;
		public Camera cam;
		public Cameras.Controller camController;
		public Gravity gravity;
		public Latency latency;
		public float moveSpeed;
		public Transform model;
		public float modelYOffset;
		public NetworkVariable<int> teamId;

		// IPlayer implementation
		public ulong ClientId {
			get;
			set;
		}

		public string Nick {
			get {
				return gameObject.name;
			}
		}

		public int TeamId {
			get {
				return teamId.Value;
			}

			set {
				teamId.Value = value;
			}
		}

		public Teams.Team Team {
			get {
				return Teams.Manager.Teams[TeamId];
			}
		}

		public int Ping {
			get {
				return latency.oneWayInMilliseconds;
			}
		}

		public Vector3 RemotePosition {
			get;
			set;
		}

		public Vector3 RemoteDirection {
			get;
			set;
		}

		// Unity events
		private void Awake() {
			account.NickChanged += nick => {
				gameObject.name = nick;
			};

			teamId.OnValueChanged += (oldTeam, newTeam) => {
				transform.SetLayer(10 + newTeam);
			};
		}

		public override void OnNetworkSpawn() {
			// Network information
			ClientId = OwnerClientId;
			RemotePosition = transform.position;

			// Change team
			transform.SetLayer(10 + teamId.Value);

			// Move player into the "Players" root object
			Reparent();

			// Enable client/server components depending on the network type
			EnableNetworkComponents();
			
			// Adjust model position
			model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);

			// Adjust camera rotation
			camController.SetRotation(transform.rotation);

			// Trigger "player added" event
			if(IsOwner) {
				Player.main = this;
			}

			Player.Added?.Invoke(this);
		}

		private void Reparent() {
			var playerRoot = GameObject.Find("Players");
			transform.SetParent(playerRoot.transform);
		}

		private void OnDisable() {
			if(IsOwner) {
				Player.main = null;
			}

			Removed?.Invoke(this);
		}

		private void EnableNetworkComponents() {
			if(!IsOwner) {
				GetComponent<ProxyMovement>().enabled = true;
				GetComponent<Snap>().enabled = true;
			}

			if(IsOwner) {
				GetComponent<OwnerMovement>().enabled = true;
				GetComponent<OwnerSendPosition>().enabled = true;
				GetComponent<Cursor>().enabled = true;
				GetComponent<Ready>().enabled = true;
			}

			if(IsServer) {
				GetComponent<Account>().enabled = true;
				GetComponent<ServerSendPosition>().enabled = true;
			}
		}

		public void Move(Vector3 direction) {
			direction.y = 0f;
			direction.Normalize();

			direction *= moveSpeed;
			direction.y = gravity.Speed;

			controller.Move(direction * Time.deltaTime);
		}
	}
}
