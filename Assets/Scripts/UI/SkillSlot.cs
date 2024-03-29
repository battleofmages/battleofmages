using BoM.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// Data
	public class SkillSlotData : MonoBehaviour {
		public Button Button;

		[SerializeField] protected Image image;
		[SerializeField] protected TextMeshProUGUI coolDownLabel;
		protected ISkillSlot slot;
	}

	// Logic
	public class SkillSlot : SkillSlotData {
		public ISkillSlot Slot {
			get => slot;
			set {
				slot = value;

				if(slot == null || slot.Skill == null) {
					Button.interactable = false;
				} else {
					image.sprite = slot.Skill.Icon;
					Button.interactable = true;
				}
			}
		}

		private void Awake() {
			Button.interactable = false;
			coolDownLabel.enabled = false;
		}

		private void Update() {
			if(slot == null || slot.Skill == null) {
				return;
			}

			var isReady = slot.IsReady;
			var progress = Mathf.Clamp((Time.time - slot.LastUsed) / slot.Skill.CoolDown, 0f, 1f);

			if(isReady) {
				if(Button.interactable == false) {
					Button.interactable = true;
					image.fillAmount = 1f;
					image.color = Color.white;
					coolDownLabel.enabled = false;
				}

				return;
			} else {
				if(Button.interactable == true) {
					Button.interactable = false;

					if(slot.Skill.CoolDown >= 1f) {
						coolDownLabel.enabled = true;
					}
				}

				image.fillAmount = progress;
				image.color = new Color(1f, 1f, 1f, 0.3f + progress * 0.7f);

				if(slot.Skill.CoolDown >= 1f) {
					coolDownLabel.text = Mathf.CeilToInt(slot.Skill.CoolDown * (1f - progress)).ToString();
					coolDownLabel.alpha = 0.1f + progress * 0.3f;
				}
			}
		}
	}
}
