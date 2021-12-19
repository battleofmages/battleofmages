using BoM.Core;
using System.Collections.Generic;

namespace BoM.Database {
	public class Memory : IDatabase {
		private Dictionary<string, Account> accounts;
		
		public Memory() {
			accounts = new Dictionary<string, Account>();
			accounts.Add("id1", new Account("id1", "Player 1 名前", "test1@example.com"));
			accounts.Add("id2", new Account("id2", "Player 2 名前", "test2@example.com"));
			accounts.Add("id3", new Account("id3", "Player 3 名前", "test3@example.com"));
		}

		public Account GetAccount(string id) {
			return accounts[id];
		}
	}
}
