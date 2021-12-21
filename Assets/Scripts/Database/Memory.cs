using BoM.Core;
using System.Collections.Generic;

namespace BoM.Database {
	public class Memory : IDatabase {
		private Dictionary<string, IAccount> accounts;
		
		public Memory() {
			accounts = new Dictionary<string, IAccount>();
		}

		public void AddAccount(IAccount account) {
			accounts.Add(account.Id, account);
		}

		public IAccount GetAccount(string id) {
			return accounts[id];
		}
	}
}
