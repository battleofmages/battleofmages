using System.Collections.Generic;

public class Party : JSONSerializable<Party> {
	public const int maxSize = 5;

	// Accounts
	public List<string> accountIds;

	// Constructor
	public Party() {
		accountIds = new List<string>(maxSize);
	}

	// Add
	public void Add(PlayerAccountBase account) {
		Add(account.id);
	}

	// Add
	public void Add(string accountId) {
		accountIds.Add(accountId);
	}

	// CanAdd
	public bool CanAdd(PlayerAccountBase account) {
		return account != null
			&& accountIds.Count < maxSize
			&& !accountIds.Contains(account.id)
			&& account.onlineStatus.value != OnlineStatus.Offline
			&& account.onlineStatus.value != OnlineStatus.InMatch;
	}
}