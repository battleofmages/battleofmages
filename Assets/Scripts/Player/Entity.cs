using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public delegate void SkillUsedHandler(Skill skill);

public class Entity : NetworkBehaviour {
	public Skeleton skeleton;
	public event SkillUsedHandler SkillUsed;

	[System.NonSerialized]
	public List<Element> elements = new List<Element>();
	public Element currentElement { get; protected set; }

	public void UseSkill(Skill skill, Vector3 cursor) {
		Vector3 skillPosition = Vector3.zero;
		Quaternion skillRotation = Quaternion.identity;

		switch(skill.position) {
			case SkillPositionType.Hands:
				skillPosition = skeleton.handsCenter;
				break;
		}

		switch(skill.rotation) {
			case SkillRotationType.ToCursor:
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