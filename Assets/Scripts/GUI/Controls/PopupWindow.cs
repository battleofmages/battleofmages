using UnityEngine;
using System.Collections;

// Delegate
public delegate void CallBack();

// PopupWindow
public class PopupWindow {
	protected int controlID;
	protected int popupWindowHash;

	// Init
	protected void Init() {
		controlID = GUIUtility.GetControlID(popupWindowHash, FocusType.Passive);
		GUIUtility.hotControl = controlID;
		Login.instance.nextPopupWindow = this;
	}
	
	public virtual void Update() {}
	public virtual void Draw() {}

	// DrawAll
	public void DrawAll() {
		if(GUIUtility.hotControl == 0)
			GUIUtility.hotControl = controlID;
		
		this.Draw();
	}

	// Close
	public void Close() {
		GUIUtility.hotControl = 0;
		Login.instance.nextPopupWindow = null;
		Login.instance.clearFlag = true;
		Event.current.Use();
	}
}
