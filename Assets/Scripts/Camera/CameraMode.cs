using UnityEngine;

public abstract class CameraMode : MonoBehaviour {
	public static CameraMode current = null;

	// Fields
	public float fieldOfView;

	// OnEnable
	protected void OnEnable() {
		CameraMode.current = this;
		Camera.main.fieldOfView = fieldOfView;
	}

	// GetCursorPosition3D
	public virtual Vector3 GetCursorPosition3D() {
		return InputManager.GetRelativeMousePositionToScreen();
	}

	// CanStartCast
	public virtual bool CanStartCast() {
		return true;
	}

	// Continue
	public virtual void Continue() {
		MainMenu.instance.enabled = false;
	}

	// FixTargetRotation
	public virtual Quaternion FixTargetRotation(Player player, Quaternion targetRotation) {
		return targetRotation;
	}

	// OnNPCAction
	public virtual void OnNPCAction(NPC npc) {
		// ...
	}
}