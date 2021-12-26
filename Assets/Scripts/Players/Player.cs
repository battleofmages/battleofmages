using BoM.Core;
using System;
using UnityEngine;

namespace BoM.Players {
	public class Player : Entity {
		public static event Action<Player> Added;
		public static event Action<Player> Removed;
		public static Player main;

		public MonoBehaviour[] ownerComponents;
		public MonoBehaviour[] proxyComponents;
		public MonoBehaviour[] serverComponents;

		public Camera cam;
		public Cameras.Center camCenter;

		public CharacterController controller;
		public Transform model;
		public float modelYOffset;
		public Cursor cursor;
		public Label label;
		public Account account;
		public Teams.Manager teamManager;

		public Teams.Team Team {
			get {
				return teamManager.teams[TeamId];
			}
		}

		// Unity events
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

			var modelRenderer = model.GetComponentInChildren<SkinnedMeshRenderer>();
			modelRenderer.gameObject.AddComponent<Visibility>();
			model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);

			// Adjust camera rotation
			camCenter.SetRotation(transform.rotation);
		}

		public override void OnNetworkSpawn() {
			// Network information
			ClientId = OwnerClientId;
			RemotePosition = transform.localPosition;
			RemoteDirection = Vector3.zero;

			// Enable client/server components depending on the network type
			EnableNetworkComponents();

			// Trigger "value changed" event
			teamId.OnValueChanged(-1, teamId.Value);

			// Trigger "player added" event
			Player.Added?.Invoke(this);

			// Set main player
			if(IsOwner) {
				Player.main = this;
			}
		}

		private void OnDisable() {
			Player.Removed?.Invoke(this);

			if(IsOwner) {
				Player.main = null;
			}
		}

		private void EnableNetworkComponents() {
			if(IsOwner) {
				foreach(var component in ownerComponents) {
					component.enabled = true;
				}
			}

			if(!IsOwner) {
				foreach(var component in proxyComponents) {
					component.enabled = true;
				}
			}

			if(IsServer) {
				foreach(var component in serverComponents) {
					component.enabled = true;
				}
			}
		}
	}
}
