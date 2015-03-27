using BoM.Async;
using BoM.Friends;
using System.Collections.Generic;

public class PlayerAccountBase {
	// Player name
	public AsyncProperty<string> playerName;
	public AsyncProperty<string> email;
	public AsyncProperty<FriendsList> friendsList;
	public AsyncProperty<Party> party;
	public AsyncProperty<OnlineStatus> onlineStatus;
	public AsyncProperty<string> avatarURL;
	public AsyncProperty<string> country;
	public AsyncProperty<List<string>> followers;
	//public AsyncProperty<AccessLevel> accessLevel;

	// Init
	public void Init(AsyncRequester req) {
		// TODO: This is not following DRY very well...
		playerName = new AsyncProperty<string>(req, "playerName");
		email = new AsyncProperty<string>(req, "email");
		friendsList = new AsyncProperty<FriendsList>(req, "friendsList");
		party = new AsyncProperty<Party>(req, "party");
		onlineStatus = new AsyncProperty<OnlineStatus>(req, "onlineStatus");
		avatarURL = new AsyncProperty<string>(req, "avatarURL");
		country = new AsyncProperty<string>(req, "country");
		followers = new AsyncProperty<List<string>>(req, "followers");
	}

	// Index operator
	public AsyncPropertyBase this[string propertyName] {
		get {
			return (AsyncPropertyBase)this.GetType().GetField(propertyName).GetValue(this);
		}
	}

	// ToString
	public override string ToString() {
		if(playerName.available)
			return string.Format("{0} ({1})", playerName.value, id);
		
		return string.Format("[Account: {0}]", id);
	}

#region Properties
	// Account ID
	public string id {
		get;
		protected set;
	}
#endregion
}
