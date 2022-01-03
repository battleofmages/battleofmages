using BoM.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BoM.UI {
	// Data
	public class SkillSlotData : MonoBehaviour {
		[SerializeField] protected Image image;
		protected ISkillSlot slot;
	}

	// Logic
	public class SkillSlot : SkillSlotData {
		public ISkillSlot Slot {
			get => slot;
			set {
				slot = value;

				if(slot == null || slot.Skill == null) {
					image.color = new Color(1f, 1f, 1f, 0.06f);
				} else {
					image.color = Color.white;
					image.sprite = slot.Skill.Icon;
				}
			}
		}

		private void Update() {
			if(slot == null || slot.Skill == null) {
				return;
			}

			if(!slot.IsReady) {
				//var progress = (Time.time - slot.LastUsed) / slot.Skill.CoolDown;
				image.color = new Color(1f, 1f, 1f, 0.5f);
			} else {
				image.color = Color.white;
			}
		}
	}
}
