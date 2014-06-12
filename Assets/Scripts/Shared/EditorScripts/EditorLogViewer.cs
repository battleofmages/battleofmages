#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class LogCategoryView {
	// Constructor
	public LogCategoryView(string nName) {
		name = nName;
		active = true;
	}
	
	public bool active;
	public string name;
	public string path;
	public string content;
	public Vector2 scrollPosition;
	
	public Thread tailThread;
	
	// Tail
	public void Tail() {
		using(
			StreamReader reader = new StreamReader(
				new FileStream(
					path, 
					FileMode.Open,
					FileAccess.Read,
					FileShare.ReadWrite
				)
			)
		) {
			long lastMaxOffset = 0;
			
			// start at the end of the file
			//long lastMaxOffset = reader.BaseStream.Length;
			
			while(true) {
				System.Threading.Thread.Sleep(100);
				
				// if the file size has not changed, idle
				if(reader.BaseStream.Length == lastMaxOffset)
					continue;
				
				// seek to the last max offset
				reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);
				
				// read out of the file until the EOF
				string line = "";
				while((line = reader.ReadLine()) != null)
					content += line + "\n";
				
				// update the last max offset
				lastMaxOffset = reader.BaseStream.Position;
			}
		}
	}
}

public class EditorLogViewer : EditorWindow {
	[MenuItem("Battle of Mages/Log Viewer")]
	static void ShowWindow() {
		var logViewer = EditorWindow.GetWindow<EditorLogViewer>("Log Viewer");
		
		// THIS NEEDS TO BE EXECUTED ***HERE*** AND NOWHERE ELSE.
		// YOU HAVE BEEN WARNED.
		EditorApplication.playmodeStateChanged += logViewer.HandleOnPlayModeChanged;
	}
	
	private List<string> dateList;
	private List<string> timeList;
	private string datePath;
	private string timePath;
	
	private LogCategoryView[] categories = new LogCategoryView[] {
		new LogCategoryView("General"),
		new LogCategoryView("Online"),
		new LogCategoryView("Chat"),
		new LogCategoryView("DB"),
	};
	
	// Awake
	public void Awake() {
		//Debug.Log("Awake");
		Refresh();
	}
	
	// HandleOnPlayModeChanged
	public void HandleOnPlayModeChanged() {
		if(EditorApplication.isPlaying) {
			Refresh();
		} else {
			Stop();
		}
	}
	
	// OnGUI
	void OnGUI() {
		using(new GUIHorizontal()) {
			if(GUILayout.Button("Refresh"))
				Refresh();
			
			GUILayout.FlexibleSpace();
			
			for(int i = 0; i < categories.Length; i++) {
				var cat = categories[i];
				cat.active = GUILayout.Toggle(cat.active, cat.name);
			}
		}
		
		using(new GUIHorizontal()) {
			for(int i = 0; i < categories.Length; i++) {
				var cat = categories[i];
				if(!cat.active)
					continue;
				
				using(new GUIVertical()) {
					GUI.contentColor = Color.yellow;
					GUILayout.Label(cat.name);
					
					GUI.contentColor = Color.white;
					using(new GUIScrollView(ref cat.scrollPosition)) {
						GUILayout.Label(cat.content);
					}
				}
			}
		}
	}
	
	// Stop
	void Stop() {
		foreach(var cat in categories) {
			if(cat.tailThread != null && cat.tailThread.IsAlive)
				cat.tailThread.Abort();
		}
	}
	
	// Refresh
	void Refresh() {
		dateList = new List<string>(Directory.GetDirectories("./Logs/"));
		dateList.Sort();
		datePath = dateList.Last();
		
		timeList = new List<string>(Directory.GetDirectories(datePath));
		timeList.Sort();
		timePath = timeList.Last();
		
		foreach(var cat in categories) {
			
			
			if(!cat.active)
				continue;
			
			cat.path = timePath + "/" + cat.name + ".log";
			cat.content = "";
			cat.tailThread = new Thread(new ThreadStart(cat.Tail));
			//cat.tailThread.IsBackground = true;
			cat.tailThread.Start();
			cat.scrollPosition.y = float.MaxValue;
		}
	}
}
#endif