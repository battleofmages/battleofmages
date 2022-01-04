using System;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	// Data
	public class BlockData : NetworkBehaviour {
		[SerializeField] protected Animations animations;
		[SerializeField] protected Energy energy;
		[SerializeField] protected float energyConsumption;
	}

	// Logic
	public partial class Block : BlockData {
		private void OnEnable() {
			animations.Animator.SetBool("Block", true);
		}

		private void OnDisable() {
			animations.Animator.SetBool("Block", false);
		}

		private void Update() {
			if(!energy.hasEnergy) {
				enabled = false;
				return;
			}

			if(!IsServer) {
				return;
			}

			energy.ConsumeOverTime(energyConsumption);
		}
	}
}
