using UnityEngine;

public class FPSWalk : MonoBehaviour {
	public float speed;
	public float jumpSpeed;
	public float gravity;

	protected Vector3 moveDirection;
	protected CharacterController charController;
	protected bool grounded;

	// Start
	void Start() {
		charController = GetComponent<CharacterController>();
	}
	
	// FixedUpdate
	void FixedUpdate () {
		if(grounded) {
			moveDirection = new Vector3(
				(Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f),
				0,
				(Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f)
			);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;

			if(Input.GetKey(KeyCode.Space)) {
				moveDirection.y = jumpSpeed;
			}
		}

		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller
		var flags = charController.Move(moveDirection * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	}
}
