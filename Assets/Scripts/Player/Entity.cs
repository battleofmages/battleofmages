using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Player {
	public class Entity : NetworkBehaviour {
		public Skeleton skeleton;
		public delegate void SkillUsedHandler(Skill.Skill skill);
		public event SkillUsedHandler SkillUsed;

		[System.NonSerialized]
		public List<Skill.Element> elements = new List<Skill.Element>();
		public int currentElementIndex;
		public Skill.Element currentElement {
			get {
				return elements[currentElementIndex];
			}
		}

		public void UseSkill(Skill.Skill skill, Vector3 cursor) {
			Vector3 skillPosition = Vector3.zero;
			Quaternion skillRotation = Quaternion.identity;

			switch(skill.position) {
				case Skill.PositionType.Cursor:
					skillPosition = cursor;
					break;

				case Skill.PositionType.Hands:
					skillPosition = skeleton.handsCenter;
					break;
			}

			switch(skill.rotation) {
				case Skill.RotationType.ToCursor:
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