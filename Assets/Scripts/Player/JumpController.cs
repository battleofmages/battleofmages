using UnityEngine;

public class JumpController : MonoBehaviour {
	public float allowAnyJumpTime;
	public float allowJumpCloseToGroundTime;
	public float maxGroundDistance;
	private float notGroundedTime;

	public bool canJump {
		get {
			if(notGroundedTime < allowAnyJumpTime) {
				return true;
			}

			if(notGroundedTime < allowJumpCloseToGroundTime && Physics.Raycast(transform.position, Vector3.down, maxGroundDistance)) {
				return true;
			}

			return false;
		}
	}
}
