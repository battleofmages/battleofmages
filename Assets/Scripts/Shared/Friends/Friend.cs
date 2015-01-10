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
}