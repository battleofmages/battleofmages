using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// Data
	public class KillFeedRowData : MonoBehaviour {
		[SerializeField] protected float duration;
		[SerializeField] protected TextMeshProUGUI killerLabel;
		[SerializeField] protected Image skillIcon;
		[SerializeField] protected TextMeshProUGUI victimLabel;
		[SerializeField] protected CanvasGroup canvasGroup;
	}

	// Logic
	public class KillFeedRow : KillFeedRowData {
		public void Init(string killer, Sprite icon, string victim) {
			killerLabel.text = killer;
			skillIcon.sprite = icon;
			victimLabel.text = victim;
			canvasGroup.alpha = 0f;

			this.FadeIn(
				UI.Settings.FadeDuration,
				progress => canvasGroup.alpha = progress,
				() => Invoke(nameof(StartFadeOut), duration)
			);
		}

		private void StartFadeOut() {
			this.FadeOut(
				UI.Settings.FadeDuration,
				progress => canvasGroup.alpha = progress,
				() => Destroy(gameObject)
			);
		}
	}
}
