using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapBoundary))]
[CanEditMultipleObjects]
public class MapBoundaryEditor : Editor {
	// Inspector
	public override void OnInspectorGUI() {
		var boundary = (MapBoundary)target;
		boundary.bounds.min = EditorGUILayout.Vector3Field("Min", boundary.bounds.min);
		boundary.bounds.max = EditorGUILayout.Vector3Field("Max", boundary.bounds.max);
		
		if(GUI.changed)
			EditorUtility.SetDirty(boundary);
	}
	
	// Scene
	public void OnSceneGUI() {
		var boundary = (MapBoundary)target;
		
		Handles.color = Color.white;
		
		// Bottom
		Handles.DrawSolidRectangleWithOutline(
			new Vector3[] {
				new Vector3(boundary.bounds.min.x, boundary.bounds.min.y, boundary.bounds.min.z),
				new Vector3(boundary.bounds.max.x, boundary.bounds.min.y, boundary.bounds.min.z),
				new Vector3(boundary.bounds.max.x, boundary.bounds.min.y, boundary.bounds.max.z),
				new Vector3(boundary.bounds.min.x, boundary.bounds.min.y, boundary.bounds.max.z),
			},
			new Color(1f, 1f, 1f, 0.2f),
			Color.black
		);
		
		// Top
		Handles.DrawSolidRectangleWithOutline(
			new Vector3[] {
				new Vector3(boundary.bounds.min.x, boundary.bounds.max.y, boundary.bounds.min.z),
				new Vector3(boundary.bounds.max.x, boundary.bounds.max.y, boundary.bounds.min.z),
				new Vector3(boundary.bounds.max.x, boundary.bounds.max.y, boundary.bounds.max.z),
				new Vector3(boundary.bounds.min.x, boundary.bounds.max.y, boundary.bounds.max.z),
			},
			new Color(1f, 1f, 1f, 0.2f),
			Color.black
		);
		
		if(GUI.changed)
			EditorUtility.SetDirty(boundary);
	}
}
