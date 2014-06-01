using UnityEngine;
using System.Collections.Generic;

public abstract class AcceptWindow<AcceptCallBackType> : PopupWindow {
	protected string text;
	protected AcceptCallBackType accept;
	protected CallBack cancel;
	public string acceptText;
	public string cancelText;
	protected bool allowAcceptWithReturn;

	// Constructor
	public AcceptWindow(string nText, AcceptCallBackType nYes, CallBack nNo = null) {
		text = nText;
		accept = nYes;
		cancel = nNo;
		acceptText = "Yes";
		cancelText = "No";
		popupWindowHash = "Confirm".GetHashCode();
		allowAcceptWithReturn = true;
		
		this.Init();
	}

	// DrawText
	protected void DrawText() {
		using(new GUIHorizontal()) {
			GUILayout.Space(4);
			GUILayout.Label(text);
			GUILayout.Space(4);
		}
	}
	
	// DrawButtons
	protected void DrawButtons() {
		using(new GUIHorizontalCenter()) {
			GUI.backgroundColor = Color.green;
			
			// Accept by key press
			bool acceptedWithKey = (allowAcceptWithReturn && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return);
			if(acceptedWithKey)
				Event.current.Use();
			
			if(GUIHelper.Button(acceptText, controlID, GUILayout.Width(60)) || acceptedWithKey) {
				Sounds.instance.PlayButtonClick();
				
				if(accept != null)
					this.Accept();
				
				this.Close();
			}

			GUI.backgroundColor = Color.red;
			if(GUIHelper.Button(cancelText, controlID, GUILayout.Width(60))) {
				Sounds.instance.PlayButtonClick();
				
				if(cancel != null)
					cancel();
				
				this.Close();
			}

			GUI.backgroundColor = Color.white;
		}
	}

	public virtual void Accept() {}
}
