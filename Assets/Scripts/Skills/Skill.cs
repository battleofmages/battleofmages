using System;
using UnityEngine;

[Serializable]
public class Skill {
	public string name;
	public GameObject prefab;
	public SkillPositionType position;
	public SkillRotationType rotation;

	[NonSerialized]
	public Element element;
}
