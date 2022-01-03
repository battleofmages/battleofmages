using UnityEngine;

namespace BoM.Players {
	[CreateAssetMenu(fileName = "Build", menuName = "BoM/Build", order = 30)]
	public class Build : ScriptableObject {
		public Skills.Bar[] Elements;
		public Traits Traits;

		private void OnEnable() {
			foreach(var element in Elements) {
				foreach(var slot in element.SkillSlots) {
					if(slot.Skill == null) {
						slot.LastUsed = 0f;
						continue;
					}

					slot.LastUsed = -slot.Skill.CoolDown;
				}
			}
		}
	}
}
