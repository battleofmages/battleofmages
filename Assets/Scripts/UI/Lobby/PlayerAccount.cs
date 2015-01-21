using uLobby;
using System.Collections.Generic;

// Delegate
public delegate void AccountChangedCallBack(PlayerAccount account);

// PlayerAccount
public class PlayerAccount : PlayerAccountBase, AsyncRequester {
	public static Dictionary<string, PlayerAccount> idToAccount = new Dictionary<string, PlayerAccount>();
	public static Dictionary<string, PlayerAccount> playerNameToAccount = new Dictionary<string, PlayerAccount>();
	public static PlayerAccount mine;

	// Private constructor
	private PlayerAccount() {
		base.Init(this);
	}
	
	// Get
	public static PlayerAccount Get(string accountId) {
		PlayerAccount acc;
		
		// Load from cache or create new account
		if(!PlayerAccount.idToAccount.TryGetValue(accountId, out acc)) {
			acc = new PlayerAccount {
				id = accountId
			};
			
			PlayerAccount.idToAccount[accountId] = acc;
		}
		
		return acc;
	}
	
	// GetByPlayerName
	public static PlayerAccount GetByPlayerName(string playerNameSearch) {
		if(string.IsNullOrEmpty(playerNameSearch))
			return null;
		
		if(PlayerAccount.playerNameToAccount.ContainsKey(playerNameSearch))
			return PlayerAccount.playerNameToAccount[playerNameSearch];
		
		return null;
	}

	// RequestAsyncProperty
	public void RequestAsyncProperty(string propertyName) {
		Lobby.RPC("RequestAccountInfo", Lobby.lobby, id, propertyName);
	}

	// WriteAsyncProperty
	public void WriteAsyncProperty(string propertyName, object val, WriteAsyncPropertyCallBack callBack) {
		callBack(val);
	}

	// ToString
	public override string ToString () {
		return string.Format("[PlayerAccount: {0}]", id);
	}

#region Properties
	// Is mine
	public bool isMine {
		get {
			return this == PlayerAccount.mine;
		}
	}
#endregion
}