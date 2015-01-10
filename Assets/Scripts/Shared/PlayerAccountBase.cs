public class PlayerAccountBase {
	// Player name
	public AsyncProperty<string> playerName;
	public AsyncProperty<string> email;
	public AsyncProperty<FriendsList> friendsList;
	//public AsyncProperty<AccessLevel> accessLevel;

	// Init
	public void Init(AsyncRequester req) {
		playerName = new AsyncProperty<string>(req, "playerName");
		email = new AsyncProperty<string>(req, "email");
		friendsList = new AsyncProperty<FriendsList>(req, "friendsList");
	}

	// Index operator
	public AsyncPropertyBase this[string propertyName] {
		get {
			return (AsyncPropertyBase)this.GetType().GetField(propertyName).GetValue(this);
		}
	}

#region Properties
	// Account ID
	public string id {
		get;
		protected set;
	}
#endregion
}
