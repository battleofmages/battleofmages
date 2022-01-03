using BoM.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// Data
	public class SkillSlotData : MonoBehaviour {
		[SerializeField] protected Image image;
		[SerializeField] protected Button button;
		protected ISkillSlot slot;
	}

	// Logic
	public class SkillSlot : SkillSlotData {
		public ISkillSlot Slot {
			get => slot;
			set {
				slot = value;

				if(slot == null || slot.Skill == null) {
					button.interactable = false;
				} else {
					image.color = Color.white;
					image.sprite = slot.Skill.Icon;
					button.interactable = true;
				}
			}
		}

		private void Update() {
			if(slot == null || slot.Skill == null) {
				return;
			}

			button.interactable = slot.IsReady;
		}
	}
}
