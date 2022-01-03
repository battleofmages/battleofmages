using BoM.Core;
using TMPro;
using UnityEngine;

namespace BoM.UI {
	// Data
	public class ScoreboardRowData : MonoBehaviour {
		public IPlayer Player { get; set; }

		[SerializeField] protected TextMeshProUGUI playerName;
		[SerializeField] protected TextMeshProUGUI score;
		[SerializeField] protected TextMeshProUGUI damage;
		[SerializeField] protected TextMeshProUGUI kills;
		[SerializeField] protected TextMeshProUGUI ping;
	}

	// Logic
	public class ScoreboardRow : ScoreboardRowData {
		private void Start() {
			playerName.text = "-";
			score.text = "-";
			damage.text = "-";
			kills.text = "-";
			ping.text = "-";
		}

		private void Update() {
			if(Player == null) {
				return;
			}

			playerName.text = Player.Nick;
			score.text = "-";
			damage.text = "-";
			kills.text = "-";
			ping.text = $"{Player.Ping}";
		}
	}
}
