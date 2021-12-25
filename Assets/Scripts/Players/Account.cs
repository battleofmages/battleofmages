using BoM.Core;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace BoM.Players {
	public class Account : NetworkBehaviour {
		public event Action<string> NickChanged;
		public NetworkVariable<FixedString64Bytes> nick;
		private IAccount account;

		private void Awake() {
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
