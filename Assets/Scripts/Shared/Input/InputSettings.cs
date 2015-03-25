namespace BoM.Input {
	// InputSettings
	public class InputSettings {
		//public float mouseSensitivity;
		public InputControl[] controls;

		// Empty constructor
		public InputSettings() {
			//mouseSensitivity = 0.5f;
			controls = new InputControl[0];
		}

		// Constructor
		public InputSettings(InputManager inputMgr) {
			//mouseSensitivity = inputMgr.mouseSensitivity;
			controls = inputMgr.controls;
		}
	}
}