using BoM.Core;
using System;
using UnityEngine;

namespace BoM.Players {
	public class Player : Entity {
		public static event Action<Player> Added;
		public static event Action<Player> Removed;
		public static Player Main;

		public Camera cam;
		public CharacterController controller;
		public Cursor cursor;
		public Account account;

		[SerializeField] private Label label;
		[SerializeField] private Teams.Manager teamManager;

		public Teams.Team Team {
			get {
				return teamManager.teams[TeamId];
			}
		}

		private void Awake() {
			account.NickChanged += nick => {
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
	}
}
