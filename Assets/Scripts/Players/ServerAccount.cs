using BoM.Core;
using Unity.Netcode;

namespace BoM.Players {
	public class ServerAccount : NetworkBehaviour {
		public Player player;
		private IAccount account;

		private void OnEnable() {
			account = Accounts.Manager.GetByClientId(player.ClientId);
			player.nick.Value = account.Nick;
		}
	}
}