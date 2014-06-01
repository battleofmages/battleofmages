using UnityEngine;
using System.Collections;

// TextAreaWindow
public class TextAreaWindow : InputWindow {
	// Constructor
	public TextAreaWindow(string nTitle, string nText, StringCallBack nYes, CallBack nNo = null) : base(nTitle, nText, nYes, nNo) {
		func = GUILayout.TextArea;
	}
}
