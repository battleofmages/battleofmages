using System.Collections.Generic;

public class Party : JSONSerializable<Party> {
	public const int maxSize = 5;

	// Accounts
	public List<string> accountIds;

	// Constructor
	public Party() {
		accountIds = new List<string>(maxSize);
	}

	// CanAdd
	public bool CanAdd(PlayerAccountBase account) {
		return !accountIds.Contains(account.id);
	}
}