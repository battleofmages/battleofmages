using BoM.Skills;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BoM.Players {
	public class Entity : NetworkBehaviour {
		public Skeleton skeleton;
		public Traits traits;

		[System.NonSerialized]
		public List<Element> elements = new List<Element>();

		[System.NonSerialized]
		public int currentElementIndex;

		public Element currentElement {
			get {
				return elements[currentElementIndex];
			}
		}

		public void UseSkill(Skill skill, Vector3 cursor) {
			Vector3 skillPosition = Vector3.zero;
			Quaternion skillRotation = Quaternion.identity;

			switch(skill.position) {
				case PositionType.Cursor:
					skillPosition = cursor;
					break;

				case PositionType.Hands:
					skillPosition = skeleton.handsCenter;
					break;
			}

			switch(skill.rotation) {
				case RotationType.ToCursor:
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