using UnityEngine;
using UnityEngine.UI;

public class InviteToPartyButton : MonoBehaviour, Initializable {
	// Init
	public void Init() {
		gameObject.SetActive(false);

		// On displayed account change
		Profile.instance.onDisplayedAccountChanged += account => {
			PlayerAccount.mine.party.Get(party => {
				gameObject.SetActive(party.CanAdd(account));
			});
		};
	}
}
