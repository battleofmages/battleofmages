using UnityEngine;
using System.Collections;

public class Confirm : AcceptWindow<CallBack> {
	// Constructor
	public Confirm(string nText, CallBack nYes, CallBack nNo = null) : base(nText, nYes, nNo) {
		acceptText = "Yes";
		cancelText = "No";
		popupWindowHash = "Confirm".GetHashCode();
		
		this.Init();
	}
	
	// Draw
	public override void Draw() {
		// Confirm box
		using(new GUIVertical("box")) {
			// Title
			DrawText();
			
			// Yes / No
			DrawButtons();
		}
	}

	// Accept
	public override void Accept() {
		accept();
	}
}
