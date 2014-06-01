using UnityEngine;
using System.Collections;

public class PopupMenu {
	// Delegate
	public delegate void CallBack();
	
	public Rect position;
	public GUIContent[] contents;
	public CallBack[] callBacks;
	private int popupMenuHash;
	private int controlID;
	//private int myHotControl = 0;
	
	private const float buttonHeight = 24f;
	private const float margin = 1f;
	
	public PopupMenu(GUIContent[] nContents, PopupMenu.CallBack[] nCallBacks) {
		// Find maximum width
		float maxWidth = 0f;
		foreach(var content in nContents) {
			var contentWidth = GUI.skin.button.CalcSize(content).x;
			if(contentWidth > maxWidth) {
				maxWidth = contentWidth;
			}
		}
		maxWidth += 4f;
		
		var pos = InputManager.GetMousePosition();
		position = new Rect(pos.x + 1, pos.y + 1, maxWidth, (buttonHeight + margin) * nContents.Length);
		contents = nContents;
		callBacks = nCallBacks;
		popupMenuHash = "PopupMenu".GetHashCode();
		controlID = GUIUtility.GetControlID(popupMenuHash, FocusType.Passive);
		GUIUtility.hotControl = controlID;
		
		Login.instance.popupMenu = this;
	}
	
	/*public void Update() {
		//controlID = GUIUtility.GetControlID(popupMenuHash, FocusType.Passive);
		//GUIUtility.hotControl = controlID;
	}*/
	
	public void Draw() {
		var oldControl = GUIUtility.hotControl;
		GUIUtility.hotControl = 0;
		
		float curY = position.y;
		
		var oldAlignment = GUI.skin.button.alignment;
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		
		if(position.x + position.width > Screen.width)
			position.x = Screen.width - position.width;
		
		if(position.y + position.height > Screen.height)
			position.y = Screen.height - position.height;
		
		for(int i = 0; i < contents.Length; i++) {
			//GUILayoutUtility.GetRect(position.width, 24)
			if(GUIHelper.Button(new Rect(position.x, curY, position.width, buttonHeight), contents[i])) {
				Sounds.instance.PlayButtonClick();
				
				//GUIUtility.hotControl = controlID;
				var callBack = callBacks[i];
				if(callBack != null)
					callBack();
				else
					LogManager.General.LogError("Callback for index " + i + " not implemented");
				
				this.Close();
			}
			
			curY += buttonHeight + margin;
		}
		
		GUI.skin.button.alignment = oldAlignment;
		
		if(Login.instance.popupMenu != null) {
			if(GUIUtility.hotControl != 0)
				GUIUtility.hotControl = oldControl;
			else
				GUIUtility.hotControl = controlID;
		}
		//oldHotControl = GUIUtility.hotControl;
	}
	
	public void Close() {
		Event.current.Use();
		GUIUtility.hotControl = 0;
		Login.instance.popupMenu = null;
	}
}
