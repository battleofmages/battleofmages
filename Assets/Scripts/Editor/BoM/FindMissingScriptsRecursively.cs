using UnityEngine;
using UnityEditor;

public class FindMissingScriptsRecursively : EditorWindow {
	static int goCount, componentCount, missingCount;

	[MenuItem("Battle of Mages/Find missing scripts recursively")]
	public static void ShowWindow() {
		FindInSelected();
	}

	// FindInSelected
	private static void FindInSelected() {
		GameObject[] go = Selection.gameObjects;

		goCount = 0;
		componentCount = 0;
		missingCount = 0;

		foreach (GameObject g in go) {
			FindInGameObject(g);
		}

		Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", goCount, componentCount, missingCount));
	}

	private static void FindInGameObject(GameObject g) {
		goCount++;
		Component[] components = g.GetComponents<Component>();

		for(int i = 0; i < components.Length; i++) {
			componentCount++;

			if (components[i] == null) {
				missingCount++;
				string s = g.name;
				Transform t = g.transform;

				while(t.parent != null) {
					s = t.parent.name + "/" + s;
					t = t.parent;
				}

				Debug.Log (s + " has an empty script attached in position: " + i, g);
			}
		}

		// Now recurse through each child GO (if there are any):
		foreach(Transform childT in g.transform) {
			//Debug.Log("Searching " + childT.name  + " " );
			FindInGameObject(childT.gameObject);
		}
	}
}