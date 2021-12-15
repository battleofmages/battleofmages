using UnityEngine;
using System.Collections.Generic;

public class Skills : MonoBehaviour {
	public List<Element> elements;

	private void Awake() {
		foreach(var element in elements) {
			foreach(var skill in element.skills) {
				skill.element = element;
			}
		}
	}
}
