using UnityEngine;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add a rigid body to the capsule
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSWalker script to the capsule

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationX = 0F;
	float rotationY = 0F;
	
	Quaternion originalRotation;
	Quaternion xQuaternion;
	Quaternion yQuaternion;

	//float oldX;
	//float oldY;

	// Update
	void Update() {
		/*var mousePos = Input.mousePosition;
		var mouseX = mousePos.x;
		var mouseY = mousePos.y;
		
		var xMovement = (mouseX - oldX) * 0.1f + Input.GetAxis("Gamepad Mouse X");
		var yMovement = (mouseY - oldY) * 0.1f + Input.GetAxis("Gamepad Mouse Y");

		oldX = mouseX;
		oldY = mouseY;*/

		var xMovement = Input.GetAxis("Mouse X") + Input.GetAxis("Gamepad Mouse X");
		var yMovement = Input.GetAxis("Mouse Y") + Input.GetAxis("Gamepad Mouse Y");

		if(axes == RotationAxes.MouseXAndY) {
			// Read the mouse input axis
			rotationX += xMovement * sensitivityX;
			rotationY += yMovement * sensitivityY;

			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY, maximumY);
			
			xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
			yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
			
			transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		} else if(axes == RotationAxes.MouseX) {
			rotationX += xMovement * sensitivityX;
			rotationX = ClampAngle (rotationX, minimumX, maximumX);

			xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation = originalRotation * xQuaternion;
		} else {
			rotationY += yMovement * sensitivityY;
			rotationY = ClampAngle (rotationY, minimumY, maximumY);

			yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
			transform.localRotation = originalRotation * yQuaternion;
		}
	}

	// Start
	void Start() {
		// Make the rigid body not change rotation
		if(rigidbody)
			rigidbody.freezeRotation = true;
		originalRotation = transform.localRotation;
	}

	// ClampAngle
	public static float ClampAngle(float angle, float min, float max) {
		if(angle < -360F)
			angle += 360F;

		if(angle > 360F)
			angle -= 360F;

		return Mathf.Clamp(angle, min, max);
	}
}