using UnityEngine;
using System.Collections;

// TextFieldWindow
public class TextFieldWindow : InputWindow {
	// Constructor
	public TextFieldWindow(string nTitle, string nText, StringCallBack nYes, CallBack nNo = null) : base(nTitle, nText, nYes, nNo) {
		func = GUILayout.TextField;
	}
}
