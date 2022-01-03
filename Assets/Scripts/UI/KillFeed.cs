using UnityEngine;

namespace BoM.UI {
	// Data
	public class KillFeedData : MonoBehaviour {
		[SerializeField] protected KillFeedRow rowPrefab;
	}

	// Logic
	public class KillFeed : KillFeedData {
		public void Add(string killer, Sprite icon, string victim) {
			var row = GameObject.Instantiate(rowPrefab);
			row.transform.SetParent(transform, false);
			row.transform.SetAsFirstSibling();
			row.Init(killer, icon, victim);
		}
	}
}
