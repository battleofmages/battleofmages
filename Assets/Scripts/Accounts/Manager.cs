using BoM.Core;
using System.Collections.Generic;

namespace BoM.Accounts {
	public static class Manager {
		private static readonly Dictionary<ulong, IAccount> clientIdToAccount = new Dictionary<ulong, IAccount>();

		public static void AddClient(ulong clientId, IAccount account) {
			clientIdToAccount.Add(clientId, account);
		}

		public static void RemoveClient(ulong clientId) {
			clientIdToAccount.Remove(clientId);
		}

		public static IAccount GetByClientId(ulong clientId) {
			if(clientIdToAccount.TryGetValue(clientId, out IAccount account)) {
				return account;
			}

			return null;
		}
	}
}
