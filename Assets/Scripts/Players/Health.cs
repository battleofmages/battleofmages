using Unity.Netcode;

namespace BoM.Players {
	public class Health : NetworkBehaviour {
		public NetworkVariable<int> health = new NetworkVariable<int>(200);
	}
}
