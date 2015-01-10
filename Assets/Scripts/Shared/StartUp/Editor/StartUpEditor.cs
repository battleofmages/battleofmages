﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomEditor(typeof(StartUp))]
public class StartUpEditor : Editor {
	private static readonly GUIContent header = new GUIContent("Initialization Order", "Sorted list of game objects that are initialized in a certain order.");
	
	private StartUp startUp;
	private ReorderableList itemList;
	private SerializedProperty items;
	
	// OnEnable
	private void OnEnable() {
		startUp = target as StartUp;
		items = serializedObject.FindProperty("initList");
		
		itemList = new ReorderableList(serializedObject, items, true, true, true, true);
		
		itemList.drawHeaderCallback += rect => GUI.Label(rect, header);
		
		itemList.drawElementCallback += (rect, index, active, focused) => {
			rect.height = 16;
			rect.y += 2;
			
			if(startUp == null)
				return;
			
			if(startUp.initList == null)
				return;
			
			if(index >= startUp.initList.Count)
				return;
			
			var item = items.GetArrayElementAtIndex(index);
			//item.stringValue = EditorGUI.TextField(rect, item.stringValue);
			item.objectReferenceValue = EditorGUI.ObjectField(rect, item.objectReferenceValue, typeof(GameObject), true);
			
			//EditorGUI.ObjectField(rect, item, typeof(Initializable), true);
		};
	}
	
	public override void OnInspectorGUI() {
		serializedObject.Update();
		
		itemList.DoLayoutList();
		
		serializedObject.ApplyModifiedProperties();
		
		if(GUI.changed)
			EditorUtility.SetDirty(startUp);
	}
}