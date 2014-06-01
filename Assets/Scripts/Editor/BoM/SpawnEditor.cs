using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spawn))]
[CanEditMultipleObjects]
public class SpawnEditor : Editor {
	// Inspector
	public override void OnInspectorGUI() {
		var spawn = (Spawn)target;
		spawn.angleStep = EditorGUILayout.FloatField("Angle Step", spawn.angleStep);
		
		if(GUI.changed)
			EditorUtility.SetDirty(spawn);
	}
	
	// Scene
	public void OnSceneGUI() {
		var spawn = (Spawn)target;
		var spawnRadius = ((SphereCollider)spawn.collider).radius;
		
		Handles.color = Color.white;
		
		// Show rotation
		Handles.ArrowCap(
			0,
			spawn.transform.position,
			spawn.transform.rotation,
			spawnRadius
		);
		
		// Show possible spawn locations
		float angle = 0f;
		for(int i = 0; i < 10; i++) {
			angle = spawn.angleStep * i * Mathf.Deg2Rad;
			var position = Spawn.GetSpawnPosition(angle, spawnRadius, spawn.transform);
			
			Handles.CubeCap(
				i,
				position,
				spawn.transform.rotation,
				1f
			);
		}
		
		if(GUI.changed)
			EditorUtility.SetDirty(spawn);
	}
}
