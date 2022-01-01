using BoM.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace BoM.UI {
	// Data
	public class ScoreboardData : MonoBehaviour {
		[SerializeField] protected CanvasGroup canvasGroup;
		[SerializeField] protected RectTransform playersContainer;
		[SerializeField] protected ScoreboardRow rowPrefab;
		[SerializeField] protected Dictionary<IPlayer, ScoreboardRow> rows;
	}

	// Logic
	public class Scoreboard : ScoreboardData {
		private void Awake() {
			rows = new Dictionary<IPlayer, ScoreboardRow>();
		}

		public void OnPlayerAdded(IPlayer player) {
			var row = GameObject.Instantiate(rowPrefab);
			row.Player = player;
			row.transform.SetParent(playersContainer, false);

			if(canvasGroup.alpha == 0f) {
				row.gameObject.SetActive(false);
			}

			rows.Add(player, row);
		}

		public void OnPlayerRemoved(IPlayer player) {
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
