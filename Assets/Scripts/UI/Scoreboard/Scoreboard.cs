using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace BoM.UI {
	public class Scoreboard : MonoBehaviour {
		public CanvasGroup canvasGroup;
		public RectTransform playersContainer;
		public ScoreboardRow rowPrefab;
		public Dictionary<Core.IPlayer, ScoreboardRow> rows = new Dictionary<Core.IPlayer, ScoreboardRow>();

		public void OnPlayerAdded(Core.IPlayer player) {
			var row = GameObject.Instantiate(rowPrefab);
			row.player = player;
			row.transform.SetParent(playersContainer, false);

			if(canvasGroup.alpha == 0f) {
				row.gameObject.SetActive(false);
			}

			rows.Add(player, row);
		}

		public void OnPlayerRemoved(Core.IPlayer player) {
			GameObject.Destroy(rows[player]);
			rows.Remove(player);
		}

		public void Show(InputAction.CallbackContext context) {
			EnableUpdates();

			this.FadeIn(
				UI.Settings.FadeDuration,
				value => canvasGroup.alpha = value,
				() => canvasGroup.interactable = true
			);
		}

		public void Hide(InputAction.CallbackContext context) {
			this.FadeOut(
				UI.Settings.FadeDuration,
				value => canvasGroup.alpha = value,
				() => {
					canvasGroup.interactable = false;
					DisableUpdates();
				}
			);
		}

		private void EnableUpdates() {
			foreach(var row in rows.Values) {
				row.gameObject.SetActive(true);
			}
		}

		private void DisableUpdates() {
			foreach(var row in rows.Values) {
				row.gameObject.SetActive(false);
			}
		}
	}
}
