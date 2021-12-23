using BoM.Core;
using System;
using Unity.Netcode;

namespace BoM.Players {
	public class Health : NetworkBehaviour, IHealth {
		public event Action<int> Changed;
		public NetworkVariable<int> health;

		private void Awake() {
			health.OnValueChanged += (oldHealth, newHealth) => {
				Changed?.Invoke(newHealth);
			};
		}

		public override void OnNetworkSpawn() {
			if(IsClient) {
				Changed?.Invoke(health.Value);
			}
		}

		public void TakeDamage(int damage, ISkill skill, IPlayer caster) {
			if(!IsServer) {
				return;
			}

			health.Value -= damage;
		}
	}
}
