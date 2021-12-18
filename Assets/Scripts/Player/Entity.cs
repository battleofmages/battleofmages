using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BoM {
	public class Entity : NetworkBehaviour {
		public Skeleton skeleton;
		public delegate void SkillUsedHandler(Skills.Skill skill);
		public event SkillUsedHandler SkillUsed;

		[System.NonSerialized]
		public List<Skills.Element> elements = new List<Skills.Element>();
		public Skills.Element currentElement { get; protected set; }

		public void UseSkill(Skills.Skill skill, Vector3 cursor) {
			Vector3 skillPosition = Vector3.zero;
			Quaternion skillRotation = Quaternion.identity;

			switch(skill.position) {
				case Skills.PositionType.Cursor:
					skillPosition = cursor;
					break;

				case Skills.PositionType.Hands:
					skillPosition = skeleton.handsCenter;
					break;
			}

			switch(skill.rotation) {
				case Skills.RotationType.ToCursor:
					var towardsCursor = cursor - skillPosition;

					if(towardsCursor != Vector3.zero) {
						skillRotation = Quaternion.LookRotation(towardsCursor);
					}

					break;
			}
			
			GameObject.Instantiate(skill.prefab, skillPosition, skillRotation);
			SkillUsed?.Invoke(skill);
		}
	}
}