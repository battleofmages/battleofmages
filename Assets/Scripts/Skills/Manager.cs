using UnityEngine;
using System.Collections.Generic;

namespace BoM.Skills {
	[CreateAssetMenu(fileName = "Skills", menuName = "BoM/Skill Manager", order = 100)]
	public class Manager : ScriptableObject {
		public List<Skill> Skills;

		private void OnEnable() {
			short count = 0;

			foreach(var skill in Skills) {
				skill.Id = count;
				skill.pool = PoolManager.GetPool(skill.prefab);
				count++;
			}
		}

		public Skill GetSkillById(short id) {
			return Skills[id];
		}
	}
}
