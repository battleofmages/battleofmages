using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Entity : NetworkBehaviour {
		public Skeleton skeleton;
		public Traits traits;

		[System.NonSerialized]
		public List<Skills.Element> elements = new List<Skills.Element>();

		[System.NonSerialized]
		public int currentElementIndex;

		public Skills.Element currentElement {
			get {
				return elements[currentElementIndex];
			}
		}

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

			var instance = skill.Instantiate();
			instance.transform.position = skillPosition;
			instance.transform.rotation = skillRotation;
			instance.pool = skill.pool;
			instance.Init();
		}
	}
}