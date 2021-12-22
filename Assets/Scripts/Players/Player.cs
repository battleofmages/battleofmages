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

		public NetworkVariable<FixedString64Bytes> id;
		public NetworkVariable<FixedString64Bytes> nick;

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
		public ulong ClientId { get; set; }
		public Vector3 RemoteDirection { get; set; }
		public Vector3 Direction { get; set; }
		private Vector3 remotePosition;

		public string Nick {
			get {
				return nick.Value.ToString();
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

		private void Awake() {
			nick.OnValueChanged += OnNickChanged;
		}

		public override void OnNetworkSpawn() {
			// Network information
			ClientId = networkObject.OwnerClientId;
			RemotePosition = transform.position;

			// Initiate network variables
			OnNickChanged("", nick.Value);

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

		private void OnNickChanged(FixedString64Bytes oldNickFixed, FixedString64Bytes newNickFixed) {
			var newNick = newNickFixed.ToString();
			gameObject.name = newNick;
			networkShadow.gameObject.name = newNick + " - Shadow";
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
				GetComponent<ServerAccount>().enabled = true;
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
