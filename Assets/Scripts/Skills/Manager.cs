using UnityEngine;
using System.Collections.Generic;

namespace BoM.Skills {
	public class Manager : MonoBehaviour {
		public static Manager Instance { get; private set; }
		public List<Skill> skills;

		public static List<Skill> All {
			get {
				return Instance.skills;
			}
		}

		private void Awake() {
			Instance = this;
			short count = 0;

			foreach(var skill in skills) {
				skill.Id = count;
				skill.pool = PoolManager.GetPool(skill.prefab);
				count++;
			}
		}

		public static Skill GetSkillById(short id) {
			return Instance.skills[id];
		}
	}
}
