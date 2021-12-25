using UnityEngine;
using System.Collections.Generic;

namespace BoM.Skills {
	[CreateAssetMenu(fileName = "Skills", menuName = "ScriptableObjects/SkillList", order = 1)]
	public class SkillList : ScriptableObject {
		public List<Skill> skills;

		private void OnEnable() {
			short count = 0;

			foreach(var skill in skills) {
				skill.Id = count;
				skill.pool = PoolManager.GetPool(skill.prefab);
				count++;
			}
		}

		public Skill GetSkillById(short id) {
			return skills[id];
		}
	}
}
