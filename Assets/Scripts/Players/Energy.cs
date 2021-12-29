using System;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class Energy : NetworkBehaviour {
		public event Action<float> Changed;
		public event Action<float> PercentChanged;
		public NetworkVariable<float> energy;
		public NetworkVariable<float> maxEnergy;
		public float regenerationSpeed;

		public bool hasEnergy {
			get {
				return energy.Value > 0f;
			}
		}

		private void Awake() {
			energy.OnValueChanged += (oldEnergy, newEnergy) => {
				Changed?.Invoke(newEnergy);
				PercentChanged?.Invoke(newEnergy / maxEnergy.Value);
			};
		}

		public override void OnNetworkSpawn() {
			if(IsServer) {
				maxEnergy.Value = 200f;
			}

			if(IsClient) {
				Changed?.Invoke(energy.Value);
				PercentChanged?.Invoke(energy.Value / maxEnergy.Value);
			}
		}

		private void Update() {
			if(!IsServer) {
				return;
			}

			energy.Value += regenerationSpeed * Time.deltaTime;

			if(energy.Value > maxEnergy.Value) {
				energy.Value = maxEnergy.Value;
			}
		}

		public void Consume(float value) {
			energy.Value -= value;
		}

		public void ConsumeOverTime(float value) {
			energy.Value -= (regenerationSpeed + value) * Time.deltaTime;
		}
	}
}
