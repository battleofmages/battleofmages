namespace BoM.Friends {
	// Friend
	[System.Serializable]
	public class Friend : JSONSerializable<Friend> {
		public string accountId;
		public string note;

		// Constructor
		public Friend() {
			accountId = "";
			note = "";
		}

		// Constructor
		public Friend(string nAccountId) {
			accountId = nAccountId;
			note = "";
		}

		// ToString
		public override string ToString() {
			return account.ToString();
		}

	#region Properties
		// Account
		public PlayerAccount account {
			get {
				return PlayerAccount.Get(accountId);
			}
		}
	#endregion
	}
}