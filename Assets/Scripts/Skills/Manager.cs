using UnityEngine;
using System.Collections.Generic;

namespace BoM.Skills {
	public class Manager : MonoBehaviour {
		public List<Element> elements;

		private void Awake() {
			foreach(var element in elements) {
				foreach(var skill in element.skills) {
					skill.element = element;
				}
			}
		}
	}
}
