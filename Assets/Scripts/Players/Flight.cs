using System;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class Flight : NetworkBehaviour {
		public event Action<bool> StateChanged;
		private NetworkVariable<bool> isActive;
		public Movement movement;
		public Gravity gravity;
		public Animations animations;
		public Health health;
		public Energy energy;
		public float speed;
		public float energyConsumption;
		private float controllerHeight;

		public bool canFly {
			get {
				return health.isAlive && energy.hasEnergy;
			}
		}

		private void Awake() {
			isActive = new NetworkVariable<bool>();
			isActive.OnValueChanged += OnFlightStateChanged;
		}

		public override void OnNetworkSpawn() {
			if(IsClient) {
				isActive.OnValueChanged(enabled, isActive.Value);
			}
		}

		private void OnFlightStateChanged(bool oldState, bool newState) {
			enabled = newState;
		}

		public void Activate() {
			if(!canFly) {
				return;
			}

			if(IsServer) {
				isActive.Value = true;
				return;
			}

			if(IsOwner) {
				StartServerRpc();
			}

			enabled = true;
		}

		public void Deactivate() {
			if(IsServer) {
				isActive.Value = false;
				return;
			}

			if(IsOwner) {
				StopServerRpc();
			}

			enabled = false;
		}

		private void OnEnable() {
			animations.Animator.SetBool("Flying", true);
			movement.speed = speed;
			controllerHeight = movement.controller.height;
			movement.controller.height = 1f;
			StateChanged?.Invoke(true);
		}

		private void OnDisable() {
			animations.Animator.SetBool("Flying", false);
			movement.speed = movement.baseSpeed;
			movement.controller.height = controllerHeight;
			StateChanged?.Invoke(false);
		}

		private void Update() {
			gravity.Speed = 0f;

			if(IsServer) {
				energy.ConsumeOverTime(energyConsumption);
			}

			if(!canFly) {
				Deactivate();
			}
		}

		[ServerRpc]
		public void StartServerRpc() {
			if(!canFly) {
				return;
			}

			isActive.Value = true;
		}

		[ServerRpc]
		public void StopServerRpc() {
			isActive.Value = false;
		}
	}
}
