using UnityEngine;

public class ThirdPerson : CameraMode {
	private Vector3 centerOfScreen = new Vector3(0.5f, 0.5f, 0);

	// GetCursorPosition3D
	public override Vector3 GetCursorPosition3D() {
		if(ToggleMouseLook.instance.mouseLook.enabled) {
			if(Player.main != null && Player.main.actionTarget != null)
				return Camera.main.WorldToViewportPoint(Player.main.actionTarget.GetCursorPosition());

			return centerOfScreen;
		}

		return InputManager.GetRelativeMousePositionToScreen();
	}

	// CanStartCast
	public override bool CanStartCast() {
		return ToggleMouseLook.instance.mouseLook.enabled;
	}

	// Continue
	public override void Continue() {
		ToggleMouseLook.instance.EnableMouseLook();
	}

	// FixTargetRotation
	public override Quaternion FixTargetRotation(Player player, Quaternion targetRotation) {
		if(!player.hovering)
			return Quaternion.AngleAxis(targetRotation.eulerAngles.y, Vector3.up);
		
		return targetRotation;
	}

	// OnNPCAction
	public override void OnNPCAction(NPC npc) {
		ToggleMouseLook.instance.DisableMouseLook();
	}
}