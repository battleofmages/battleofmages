using System;
using Unity.Netcode;

namespace BoM.Players {
	public class Health : NetworkBehaviour {
		public event Action<int> Changed;
		public NetworkVariable<int> health = new NetworkVariable<int>(200);

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
	}
}
