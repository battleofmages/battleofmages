using BoM.Core;
using System;
using UnityEngine;

namespace BoM.Skills {
	[Serializable]
	public class Slot : ISkillSlot {
		public ISkill Skill { get => skill; }
		public float LastUsed { get; set; }
		public bool IsReady { get => Time.time - LastUsed > skill.CoolDown; }

		[SerializeField] protected Skill skill;
	}
}
