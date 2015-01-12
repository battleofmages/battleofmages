using UnityEngine;

public class PartyManager : MonoBehaviour {
	public GameObject partyMemberPrefab;

	// OnEnable
	void OnEnable() {
		if(PlayerAccount.mine == null)
			return;
		
		PlayerAccount.mine.party.Get(data => {
			BuildParty();
		});
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
				clone.GetComponent<PartyMemberWidget>().accountId = id;
			}
		);
	}

	// DeletePartyList
	void DeletePartyList() {
		transform.DeleteChildrenWithComponent<PartyMemberWidget>();
	}
}
