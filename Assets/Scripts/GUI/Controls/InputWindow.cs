using UnityEngine;
using System.Collections;

// Delegate
public delegate string GUIInputCallBack(string text, params GUILayoutOption[] options);
public delegate void StringCallBack(string text);

// InputWindow
public class InputWindow : AcceptWindow<StringCallBack> {
	protected string input;
	protected GUIInputCallBack func;
	private bool focused;
	
	// Constructor
	public InputWindow(string nTitle, string nInput, StringCallBack nYes, CallBack nNo = null) : base(nTitle, nYes, nNo) {
		input = nInput;
		focused = false;
		allowAcceptWithReturn = false;
		acceptText = "Save";
		cancelText = "Cancel";
		popupWindowHash = "TextAreaWindow".GetHashCode();
		
		this.Init();
	}
	
	// Draw
	public override void Draw() {
		// Confirm box
		using(new GUIVertical("box")) {
			// Title
			DrawText();

			using(new GUIHorizontal()) {
				GUILayout.Space(4);

				GUI.SetNextControlName("InputWindowInput");
				input = func(input);

				if(!focused) {
					GUIHelper.Focus("InputWindowInput");
					focused = true;
				}
				
				if(Event.current.isKey)
					Event.current.Use();
				
				GUILayout.Space(4);
			}

			// Yes / No
			DrawButtons();
		}
	}

	// Accept
	public override void Accept() {
		accept(input);
	}
}
