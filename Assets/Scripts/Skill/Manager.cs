using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace BoM.Skill {
	public class Manager : MonoBehaviour {
		public List<Element> elements;

		private void Awake() {
			foreach(var element in elements) {
				foreach(var skill in element.skills) {
					skill.element = element;
					skill.pool = new ObjectPool<Instance>(() => {
						return GameObject.Instantiate(skill.prefab);
					}, instance => {
						instance.gameObject.SetActive(true);
					}, instance => {
						instance.gameObject.SetActive(false);
					}, instance => {
						Destroy(instance.gameObject);
					}, false, 32, 64);

					PoolManager.pools.Add(skill.prefab, skill.pool);
				}
			}
		}
	}
}
