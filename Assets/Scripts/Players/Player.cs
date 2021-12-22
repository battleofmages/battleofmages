using BoM.Core;
using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	public class Player : NetworkBehaviour, IPlayer {
		public static Player main;
		public static event Action<Player> Added;
		public static event Action<Player> Removed;

		public Account account;
		public CharacterController controller;
		public Camera cam;
		public Gravity gravity;
		public Latency latency;
		public float moveSpeed;
		public Transform model;
		public float modelYOffset;
		public GameObject networkShadow;
		public ulong ClientId { get; set; }
		public Vector3 RemoteDirection { get; set; }
		private Vector3 remotePosition;

		public string Nick {
			get {
				return gameObject.name;
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

				if(networkShadow.activeSelf) {
					networkShadow.transform.position = ProxyMovement.CalculatePosition(remotePosition, RemoteDirection, latency.oneWay);
				}
			}
		}

		private void Awake() {
			account.NickChanged += OnNickChanged;
		}

		private void OnNickChanged(string nick) {
			gameObject.name = nick;
			networkShadow.gameObject.name = nick + " - Shadow";
		}

		public override void OnNetworkSpawn() {
			// Network information
			ClientId = OwnerClientId;
			RemotePosition = transform.position;

			// Move player into the "Players" root object
			Reparent();

			// Enable client/server components depending on the network type
			EnableNetworkComponents();
			
			// Adjust model position
			model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);

			// Trigger "player added" event
			if(IsOwner) {
				Player.main = this;
			}

			Player.Added?.Invoke(this);
		}

		private void Reparent() {
			var playerRoot = GameObject.Find("Players");
			transform.SetParent(playerRoot.transform);
			networkShadow.transform.SetParent(playerRoot.transform, true);
		}

		private void OnDisable() {
			if(IsOwner) {
				Player.main = null;
			}

			Removed?.Invoke(this);
			Destroy(networkShadow);
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
