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
		public Cameras.Center camCenter;
		public Gravity gravity;
		public Cursor cursor;
		public Latency latency;
		public float moveSpeed;
		public Transform model;
		public float modelYOffset;
		public Teams.Manager teamManager;
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
				return teamManager.teams[TeamId];
			}
		}

		public int Ping {
			get {
				return latency.oneWayInMilliseconds;
			}
		}

		public GameObject GameObject {
			get {
				return gameObject;
			}
		}

		public Transform Transform {
			get {
				return transform;
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
				var layer = Team.layer;
				transform.SetLayer(layer);

				var defaultLayer = LayerMask.NameToLayer("Default");
				cursor.LayerMask = (1 << defaultLayer) | teamManager.GetEnemyTeamsLayerMask(Team);
			};

			var modelRenderer = model.GetComponentInChildren<SkinnedMeshRenderer>();
			modelRenderer.gameObject.AddComponent<Visibility>();
		}

		public override void OnNetworkSpawn() {
			// Network information
			ClientId = OwnerClientId;
			RemotePosition = transform.localPosition;

			// Change team
			teamId.OnValueChanged(-1, teamId.Value);

			// Enable client/server components depending on the network type
			EnableNetworkComponents();

			// Adjust model position
			model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);

			// Adjust camera rotation
			camCenter.SetRotation(transform.rotation);

			// Trigger "player added" event
			if(IsOwner) {
				Player.main = this;
			}

			Player.Added?.Invoke(this);
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
