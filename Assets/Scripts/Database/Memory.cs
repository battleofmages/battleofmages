using BoM.Core;
using System.Collections.Generic;

namespace BoM.Database {
	public class Memory : IDatabase {
		private Dictionary<string, Account> accounts;
		
		public Memory() {
			accounts = new Dictionary<string, Account>();
			accounts.Add("id0", new Account("id0", "Player 0 名前", "test0@example.com"));
			accounts.Add("id1", new Account("id1", "Player 1 名前", "test1@example.com"));
			accounts.Add("id2", new Account("id2", "Player 2 名前", "test2@example.com"));
			accounts.Add("id3", new Account("id3", "Player 3 名前", "test3@example.com"));
			accounts.Add("id4", new Account("id4", "Player 4 名前", "test4@example.com"));
			accounts.Add("id5", new Account("id5", "Player 5 名前", "test5@example.com"));
			accounts.Add("id6", new Account("id6", "Player 6 名前", "test6@example.com"));
			accounts.Add("id7", new Account("id7", "Player 7 名前", "test7@example.com"));
		}

		public Account GetAccount(string id) {
			return accounts[id];
		}
	}
}
