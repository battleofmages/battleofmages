using TMPro;
using UnityEngine;

namespace BoM.UI {
	public class ScoreboardRow : MonoBehaviour {
		public Core.IPlayer player;
		public TextMeshProUGUI playerName;
		public TextMeshProUGUI score;
		public TextMeshProUGUI damage;
		public TextMeshProUGUI kills;
		public TextMeshProUGUI ping;

		private void Start() {
			playerName.text = "-";
			score.text = "-";
			damage.text = "-";
			kills.text = "-";
			ping.text = "-";
		}

		private void Update() {
			if(player == null) {
				return;
			}

			playerName.text = player.Account.Nick;
			score.text = "-";
			damage.text = "-";
			kills.text = "-";
			ping.text = $"{player.Latency}";
		}
	}
}
