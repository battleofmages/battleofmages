using UnityEngine;
using System.Collections.Generic;

public interface Initializable {
	void Init();
}

public class StartUp : MonoBehaviour {
	public List<GameObject> initList = new List<GameObject>();

	// Start
	void Start() {
		foreach(var gameObj in initList) {
			var initObjects = gameObj.GetComponents<MonoBehaviour>();

			for(int i = 0; i < initObjects.Length; i++) {
				var initObj = initObjects[i] as Initializable;

				if(initObj == null)
					return;

				initObj.Init();
			}
		}
	}
}