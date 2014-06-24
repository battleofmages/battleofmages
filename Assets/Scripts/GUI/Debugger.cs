using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Debugger : LobbyModule<Debugger> {
	public bool debugGUI;
	public bool debugCasting;
	public bool debugAnimator;
	public bool showServerPosition;
	public bool skillTestMode;
	
	protected List<string> messages = new List<string>();
	protected List<string> nextMessages = new List<string>();

	// Draw
	public override void Draw() {
		// Swap message lists
		if(Event.current.type == EventType.Layout) {
			messages.Clear();
			var tmpMessages = messages;
			messages = nextMessages;
			nextMessages = tmpMessages;
		}
		
		if(messages.Count == 0)
			return;
		
		// Draw debug info
		using(new GUIArea(new Rect(5, 5, GUIArea.width - 10, GUIArea.height - 10))) {
			foreach(var msg in messages) {
				GUILayout.Label(msg);
			}
		}
		
		// Objects
//		if(networkViewIsMine && Input.GetKey(KeyCode.N)) {
//			GUILayout.BeginArea(new Rect(5, 5, 200, 200));
//			GUILayout.Label("All: " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
//	        GUILayout.Label("Textures: " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length);
//	        GUILayout.Label("AudioClips: " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
//	        GUILayout.Label("Meshes: " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
//	        GUILayout.Label("Materials: " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
//	        GUILayout.Label("GameObjects: " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
//	        GUILayout.Label("Components: " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);
//			GUILayout.EndArea();
//		}
		
		// Interpolation rate
//		if(networkViewIsProxy) {
//			proxyInterpolationSpeed = GUI.HorizontalSlider(new Rect(5, 30, Screen.width - 100, 24), proxyInterpolationSpeed, 0f, 20f);
//			Application.targetFrameRate = (int)GUI.HorizontalSlider(new Rect(5, 50, Screen.width - 100, 24), Application.targetFrameRate, 5, 100);
//		}
	}

	// Label
	public static void Label(string info) {
		Debugger.instance.nextMessages.Add(info);
	}

	// OnGUI
	void OnGUI() {
		Draw();
	}
}
