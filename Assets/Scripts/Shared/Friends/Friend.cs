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
		var playerName = account.playerName;

		if(playerName.available)
			return string.Format("[Friend: {0} ({1})]", playerName.value, accountId);

		return string.Format("[Friend: {0}]", accountId);
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