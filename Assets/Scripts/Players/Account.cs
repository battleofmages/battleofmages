using BoM.Core;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class AccountData : NetworkBehaviour {
		protected NetworkVariable<FixedString64Bytes> nick;
		protected IAccount account;
	}

	// Logic
	public class Account : AccountData {
		public event Action<string> NickChanged;

		private void Awake() {
			nick = new NetworkVariable<FixedString64Bytes>();
			nick.OnValueChanged += OnNickChanged;
		}

		public override void OnNetworkSpawn() {
			if(IsClient) {
				NickChanged?.Invoke(nick.Value.ToString());
			}

			if(IsServer) {
				ServerFetchAccount();
			}
		}

		private void ServerFetchAccount() {
			account = Accounts.Manager.GetByClientId(OwnerClientId);
			nick.Value = account.Nick;
		}

		private void OnNickChanged(FixedString64Bytes oldNickFixed, FixedString64Bytes newNickFixed) {
			var newNick = newNickFixed.ToString();
			NickChanged?.Invoke(newNick);
		}
	}
}
