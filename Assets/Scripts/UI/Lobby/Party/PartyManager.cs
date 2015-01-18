using UnityEngine;
using UnityEngine.UI;

public class PartyManager : MonoBehaviour {
	public GameObject partyMemberPrefab;
	public GameObject emptySlotPrefab;

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
}
