using System;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class FlightData : NetworkBehaviour {
		[SerializeField] protected Movement movement;
		[SerializeField] protected Gravity gravity;
		[SerializeField] protected Animations animations;
		[SerializeField] protected Health health;
		[SerializeField] protected Energy energy;
		[SerializeField] protected float speed;
		[SerializeField] protected float energyConsumption;

		protected NetworkVariable<bool> isActive;
		protected float controllerHeight;
	}

	// Logic
	public class Flight : FlightData {
		public event Action<bool> StateChanged;
		public bool canFly { get => health.isAlive && energy.hasEnergy; }

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
			movement.Speed = speed;
			controllerHeight = movement.Controller.height;
			movement.Controller.height = 1f;
			StateChanged?.Invoke(true);
		}

		private void OnDisable() {
			animations.Animator.SetBool("Flying", false);
			movement.Speed = movement.BaseSpeed;
			movement.Controller.height = controllerHeight;
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
