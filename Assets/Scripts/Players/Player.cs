using BoM.Core;
using System;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class PlayerData : NetworkBehaviour {
		public Account Account;
		public Latency Latency;
		public CharacterController Controller;
		public Camera Cam;
		public ulong ClientId { get; set; }
		public Vector3 RemotePosition { get; set; }
		public Vector3 RemoteDirection { get; set; }

		[SerializeField] protected Health health;
		[SerializeField] protected Energy energy;
		[SerializeField] protected Cursor cursor;
		[SerializeField] protected Rotation rotation;
		[SerializeField] protected Label label;
		[SerializeField] protected Cameras.Center camCenter;
		[SerializeField] protected Teams.Manager teamManager;
		[SerializeField] protected NetworkVariable<int> teamId;
	}

	// Logic
	public class Player : PlayerData, IPlayer {
		public static event Action<Player> Added;
		public static event Action<Player> Removed;
		public static Player Main;

		public int Ping { get => Latency.OneWayInMilliseconds; }
		public string Nick { get => gameObject.name; }
		public GameObject GameObject { get => gameObject; }
		public Transform Transform { get => transform; }

		public int TeamId {
			get => teamId.Value;
			set => teamId.Value = value;
		}

		public Teams.Team Team { get => teamManager.teams[TeamId]; }

		private void Awake() {
			Account.NickChanged += nick => {
				gameObject.name = nick;
				label.SetText(nick);
			};

			teamId.OnValueChanged += (oldTeam, newTeam) => {
				var layer = Team.layer;
				transform.SetLayer(layer);

				var defaultLayer = LayerMask.NameToLayer("Default");
				cursor.LayerMask = (1 << defaultLayer) | teamManager.GetEnemyTeamsLayerMask(Team);
			};
		}

		public override void OnNetworkSpawn() {
			// Network information
			ClientId = OwnerClientId;
			RemotePosition = transform.position;

			// Trigger "value changed" event
			teamId.OnValueChanged(-1, teamId.Value);

			// Enable client/server components depending on the network type
			EnableNetworkComponents();

			// Trigger "player added" event
			Player.Added?.Invoke(this);

			// Set main player
			if(IsOwner) {
				Player.Main = this;
			}
		}

		private void OnDisable() {
			Player.Removed?.Invoke(this);

			if(IsOwner) {
				Player.Main = null;
			}
		}

		private void EnableNetworkComponents() {
			if(IsOwner) {
				GetComponent<OwnerMovement>().enabled = true;
				GetComponent<OwnerSendPosition>().enabled = true;
				GetComponent<Cursor>().enabled = true;
				GetComponent<Ready>().enabled = true;
			}

			if(!IsOwner) {
				GetComponent<ProxyMovement>().enabled = true;
				GetComponent<Snap>().enabled = true;
			}

			if(IsServer) {
				GetComponent<ServerSendPosition>().enabled = true;
				GetComponent<Account>().enabled = true;
			}
		}

		public void Respawn(Vector3 newPosition, Quaternion newRotation) {
			// Position
			transform.position = newPosition;
			RemotePosition = newPosition;
			RemoteDirection = Const.ZeroVector;
			Physics.SyncTransforms();

			// Character rotation
			rotation.SetRotation(newRotation);

			// Camera rotation
			camCenter.SetRotation(newRotation);

			if(IsServer) {
				RespawnClientRpc(newPosition, newRotation);
				health.Reset();
				energy.Reset();
			}
		}

		[ClientRpc]
		public void RespawnClientRpc(Vector3 newPosition, Quaternion newRotation) {
			if(IsHost) {
				return;
			}

			Respawn(newPosition, newRotation);
		}
	}
}
