using UnityEngine;
using System.Collections;

// Delegate
public delegate void PWChangeCallBack(string oldPW, string newPW);

public class PasswordChangeWindow : PopupWindow {
	protected string text;
	protected PWChangeCallBack accept;
	protected CallBack cancel;
	
	protected string oldPassword;
	protected string oldPasswordEncrypted;
	protected string newPassword;
	
	// Constructor
	public PasswordChangeWindow(string nText, PWChangeCallBack nAccept, CallBack nCancel = null) {
		text = nText;
		accept = nAccept;
		cancel = nCancel;
		popupWindowHash = "PasswordChangeWindow".GetHashCode();
		
		oldPassword = "";
		oldPasswordEncrypted = "";
		newPassword = "";
		
		this.Init();
	}
	
	// Draw
	public override void Draw() {
		// Confirm box
		using(new GUIVertical("box")) {
			// Title
			using(new GUIHorizontal()) {
				GUILayout.Space(4);
				GUILayout.Label(text);
				GUILayout.Space(4);
			}
			
			var width = GUILayout.Width(300);
			
			// Old password
			using(new GUIHorizontal()) {
				GUILayout.Label("Old password:");
				GUI.SetNextControlName("OldPassword");
				var newOldPassword = GUILayout.PasswordField(oldPassword, '*', 50, width);
				
				if(oldPassword != newOldPassword) {
					oldPassword = newOldPassword;
					oldPasswordEncrypted = GameDB.EncryptPasswordString(oldPassword);
				}
			}
			
			// New password
			GUI.enabled = oldPassword != newPassword && (oldPassword == Login.instance.accountPassword || oldPasswordEncrypted == Login.instance.accountPasswordEncrypted);
			using(new GUIHorizontal()) {
				GUILayout.Label("New password:");
				GUI.SetNextControlName("NewPassword");
				newPassword = GUILayout.PasswordField(newPassword, '*', 50, width);
			}
			
			// Accept / Cancel
			using(new GUIHorizontalCenter()) {
				GUI.enabled = GUI.enabled && (
					GameDB.IsTestAccount(Login.instance.accountEmail) ||
					Validator.password.IsMatch(newPassword)
				);
				if(GUIHelper.Button("Change", controlID, GUILayout.Width(80))) {
					Sounds.instance.PlayButtonClick();
					
					if(accept != null)
						accept(oldPassword, newPassword);
					
					Login.instance.accountPassword = newPassword;
					Login.instance.accountPasswordEncrypted = null;
					this.Close();
				}
				
				GUI.enabled = true;
				if(GUIHelper.Button("Cancel", controlID, GUILayout.Width(80))) {
					Sounds.instance.PlayButtonClick();
					
					if(cancel != null)
						cancel();
					
					this.Close();
				}
			}
		}
	}
}
