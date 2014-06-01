using UnityEngine;
using UnityEditor;

public class RepositionY : EditorWindow {
	[MenuItem("Battle of Mages/Selected objects/Reposition Y on terrain")]
	public static void Reposition() {
		RaycastHit hit;

		foreach(var obj in Selection.gameObjects) {
			if(Physics.Raycast(obj.transform.position, Vector3.down, out hit, 1000f)) {
				obj.transform.position = hit.point;
			}
		}
	}

	[MenuItem("Battle of Mages/Selected objects/Reposition Y on terrain with offset")]
	public static void RepositionWithOffset() {
		RaycastHit hit;
		Vector3 offset = new Vector3(0, 3f, 0);
		
		foreach(var obj in Selection.gameObjects) {
			if(Physics.Raycast(obj.transform.position, Vector3.down, out hit, 1000f)) {
				obj.transform.position = hit.point + offset;
			}
		}
	}

	[MenuItem("Battle of Mages/Selected objects/Randomize Y rotation")]
	public static void RandomizeYRotation() {
		foreach(var obj in Selection.gameObjects) {
			var euler = obj.transform.eulerAngles;
			euler.y = Random.Range(0, 360);
			obj.transform.eulerAngles = euler;
		}
	}

	[MenuItem("Battle of Mages/Selected objects/Randomize XZ rotation")]
	public static void RandomizeXZRotation() {
		foreach(var obj in Selection.gameObjects) {
			var euler = obj.transform.eulerAngles;
			euler.x = Random.Range(-10f, 10f);
			euler.z = 90f + Random.Range(-10f, 10f);
			obj.transform.eulerAngles = euler;
		}
	}

	[MenuItem("Battle of Mages/Selected objects/Randomize scale")]
	public static void RandomizeScale() {
		foreach(var obj in Selection.gameObjects) {
			var scale = Random.Range(0.3f, 0.5f);
			obj.transform.localScale = new Vector3(scale, scale, scale);
		}
	}

	[MenuItem("Battle of Mages/Selected objects/Randomly delete")]
	public static void RandomlyDelete() {
		float probability = 0.1f;

		foreach(var obj in Selection.gameObjects) {
			if(Random.Range(0f, 1f) <= probability)
				DestroyImmediate(obj);
		}
	}

	[MenuItem("Battle of Mages/Selected objects/Procedural generation")]
	public static void ProceduralGeneration() {
		GameObject newObj;
		var obj = Selection.activeGameObject;

		Object prefabRoot = PrefabUtility.GetPrefabParent(obj);

		const float distance = 22.0f;
		const float positionRandomize = 8.0f;
		float maxX = 970f;
		float maxZ = 970f;
		float xRotRandomize = 10f;
		float zRotRandomize = 10f;

		for(float x = Random.Range(0f, positionRandomize); x < maxX; x += Random.Range(distance - positionRandomize, distance + positionRandomize)) {
			for(float z = Random.Range(0f, positionRandomize); z < maxZ; z += Random.Range(distance - positionRandomize, distance + positionRandomize)) {
				if(prefabRoot != null)
					newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
				else
					newObj = (GameObject)Instantiate(obj);
				
				newObj.transform.parent = obj.transform.parent;
				newObj.transform.localPosition = obj.transform.localPosition + new Vector3(x, 0, z);
				newObj.transform.localScale = obj.transform.localScale;

				var euler = obj.transform.eulerAngles;
				euler.x = euler.x + Random.Range(-xRotRandomize, xRotRandomize);
				euler.y = Random.Range(0, 360);
				euler.z = euler.z + Random.Range(-zRotRandomize, zRotRandomize);
				newObj.transform.eulerAngles = euler;
			}
		}
	}
}