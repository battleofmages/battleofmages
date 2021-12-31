using System;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class EnergyData : NetworkBehaviour {
		[SerializeField] protected NetworkVariable<float> energy;
		[SerializeField] protected NetworkVariable<float> maxEnergy;
		[SerializeField] protected float regenerationSpeed;
	}

	// Logic
	public partial class Energy : EnergyData {
		public event Action<float> Changed;
		public event Action<float> PercentChanged;

		public bool hasEnergy { get => energy.Value > 0f; }

		private void Awake() {
			energy.OnValueChanged += (oldEnergy, newEnergy) => {
				Changed?.Invoke(newEnergy);
				PercentChanged?.Invoke(newEnergy / maxEnergy.Value);
			};
		}

		public override void OnNetworkSpawn() {
			// if(IsServer) {
			// 	maxEnergy.Value = 200f;
			// }

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

		public void Reset() {
			energy.Value = maxEnergy.Value;
		}
	}
}
