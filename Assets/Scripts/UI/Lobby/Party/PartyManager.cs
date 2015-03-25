using UnityEngine;
using UnityEngine.UI;
using uLobby;
using BoM.UI;
using BoM.UI.Notifications;
using BoM.UI.Lobby;

public class PartyManager : MonoBehaviour, Initializable {
	public GameObject partyMemberPrefab;
	public GameObject emptySlotPrefab;

	// Init
	public void Init() {
		Lobby.AddListener(this);

		// Construct party list on login
		Login.instance.onLogIn += account => {
			account.party.Connect(
				this,
				data => {
					BuildParty();
				},
				false
			);
		};

		// Disconnect listeners on logout
		Login.instance.onLogOut += (account) => {
			account.party.Disconnect(this);
		};
	}

	// BuildParty
	void BuildParty() {
		var party = PlayerAccount.mine.party.value;

		DeletePartyList();

		UIListBuilder<string>.Build(
			party.accountIds,
			partyMemberPrefab,
			transform,
			(clone, id) => {
				// Set info
				clone.GetComponent<PartyMemberWidget>().account = PlayerAccount.Get(id);
			}
		);

		// Show empty slots
		for(int i = party.accountIds.Count; i < Party.maxSize; i++) {
			var clone = (GameObject)Object.Instantiate(emptySlotPrefab);
			clone.transform.SetParent(transform, false);
			clone.transform.SetSiblingIndex(i);
		}
	}

	// DeletePartyList
	void DeletePartyList() {
		transform.DeleteChildrenWithComponent<Button>();
	}

#region RPCs
	[RPC]
	void PartyInvitation(string accountId) {
		var account = PlayerAccount.Get(accountId);
		LogManager.General.Log("Received a party invitation from " + account);

		account.playerName.Get(playerName => {
			NotificationManager.instance.CreatePartyInvitation(string.Format("<color=#ffff00>{0}</color> has sent you a group invitation.", playerName), account, 0f);
		});
	}
#endregion
}
