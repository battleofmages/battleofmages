using UnityEngine;

namespace BoM.Input {
	// InputManager
	public class InputManager : SingletonMonoBehaviour<InputManager> {
		public static bool ignoreInput = false;
		
		//private float _mouseSensitivity = 5f;
		public InputControl[] controls;
		
		// Start
		void Start() {
			#pragma warning disable 0618
			// If we don't do this, KeyUp event is never registered for the Input class
			UnityEngine.Input.eatKeyPressOnTextFieldFocus = false;
			#pragma warning restore 0618
		}
		
		// Clear
		public void Clear() {
			foreach(var control in controls) {
				UnityEngine.Input.GetKey(control.keyCode);
				UnityEngine.Input.GetKey(control.altKeyCode);
				UnityEngine.Input.GetKey(control.gamePadKeyCode);
				
				UnityEngine.Input.GetKeyDown(control.keyCode);
				UnityEngine.Input.GetKeyDown(control.altKeyCode);
				UnityEngine.Input.GetKeyDown(control.gamePadKeyCode);
			}
		}
		
		// GetButton
		public bool GetButton(int index) {
			if(ignoreInput)
				return false;
			
			var control = controls[index];
			return UnityEngine.Input.GetKey(control.keyCode) || UnityEngine.Input.GetKey(control.altKeyCode) || UnityEngine.Input.GetKey(control.gamePadKeyCode);
		}
		
		// GetButtonDown
		public bool GetButtonDown(int index) {
			if(ignoreInput)
				return false;
			
			var control = controls[index];
			return UnityEngine.Input.GetKeyDown(control.keyCode) || UnityEngine.Input.GetKeyDown(control.altKeyCode) || UnityEngine.Input.GetKeyDown(control.gamePadKeyCode);
		}
		
		// GetButtonFloat
		public float GetButtonFloat(int index) {
			if(ignoreInput)
				return 0f;
			
			var control = controls[index];
			return UnityEngine.Input.GetKey(control.keyCode) || UnityEngine.Input.GetKey(control.altKeyCode) || UnityEngine.Input.GetKey(control.gamePadKeyCode) ? 1.0f : 0.0f;
		}
		
		// GetButtonIndex
		public int GetButtonIndex(string id) {
			for(int i = 0; i < controls.Length; i++) {
				if(controls[i].id == id)
					return i;
			}
			
			LogManager.General.LogWarning("Control '" + id + "' doesn't exist");
			return -1;
		}
		
		// CopySettingsFrom
		public void CopySettingsFrom(InputSettings inputSettings) {
			foreach(var control in inputSettings.controls) {
				int index = GetButtonIndex(control.id);
				
				if(index != -1) {
					var myControl = controls[index];
					myControl.keyCode = control.keyCode;
					myControl.altKeyCode = control.altKeyCode;
					//myControl.gamePadKeyCode = control.gamePadKeyCode;
				}
			}
		}
		
		/*// Mouse sensitivity
		public float mouseSensitivity {
			get { return _mouseSensitivity; }
			set {
				if(_mouseSensitivity != value) {
					_mouseSensitivity = value;
					PlayerPrefs.SetFloat("Input_MouseSensitivity", _mouseSensitivity);
					var camPivot = GameObject.FindGameObjectWithTag("CamPivot");
					
					if(camPivot != null) {
						var mouseLook = camPivot.GetComponent<MouseLook>();
						mouseLook.sensitivityX = _mouseSensitivity;
						mouseLook.sensitivityY = _mouseSensitivity;
					}
				}
			}
		}*/
		
		// GetMousePosition
		public static Vector2 GetMousePosition() {
			return new Vector2(UnityEngine.Input.mousePosition.x, (Screen.height - UnityEngine.Input.mousePosition.y));
		}
		
		// GetRelativeMousePositionToScreen
		public static Vector3 GetRelativeMousePositionToScreen() {
			return new Vector3(UnityEngine.Input.mousePosition.x / Screen.width, UnityEngine.Input.mousePosition.y / Screen.height, 0f);
		}
	}
}