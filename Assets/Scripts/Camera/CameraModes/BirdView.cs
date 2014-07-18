using UnityEngine;

public class BirdView : CameraMode {
	// FixTargetRotation
	public override Quaternion FixTargetRotation(Player player, Quaternion targetRotation) {
		return Quaternion.AngleAxis(targetRotation.eulerAngles.y, Vector3.up);
	}
}