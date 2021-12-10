using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public partial class Player : NetworkBehaviour {
	public void Move(InputAction.CallbackContext context) {
		var value = context.ReadValue<Vector2>();
		moveVector = new Vector3(value.x, 0, value.y);
	}

	public void Fire(InputAction.CallbackContext context) {
		Debug.Log("Fire");
	}
}
