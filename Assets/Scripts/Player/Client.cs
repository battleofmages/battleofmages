using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Collections;

public class Client: NetworkBehaviour {
	public Player player;
	public Server server;
	public InputActionAsset inputActions;
	public Camera cam;
	public CameraController camController;
	public Transform model;
	public Animator animator;
	public float rotationSpeed;
	public GameObject spawnOnFire;
	private Vector3 inputDirection;
	private Vector3 direction;
	private Vector2 look;
	private bool fire;
	private bool block;
	private Vector3 lastPositionSent;
	private Vector3 lastDirectionSent;

	private void OnEnable() {
		CameraManager.AddCamera(cam);
		Game.Instance.player = player;
		Game.Instance.client = this;
		Game.Start();
	}

	private void OnDisable() {
		CameraManager.RemoveCamera(cam);
		Game.Stop();
		Game.Instance.client = null;
		Game.Instance.player = null;
	}

	private void Update() {
		UpdateRotation();
		UpdateAnimation();
	}

	private void FixedUpdate() {
		UpdatePosition();
		SendPositionToServer();
	}

	private void UpdatePosition() {
		direction = cam.transform.TransformDirection(inputDirection);
		direction.y = 0f;
		direction.Normalize();

		player.Move(direction);
	}

	private void UpdateRotation() {
		if(direction == Vector3.zero) {
			return;
		}

		model.rotation = Quaternion.Slerp(
			model.rotation,
			Quaternion.LookRotation(direction),
			Time.deltaTime * rotationSpeed
		);
	}

	private void UpdateAnimation() {
		animator.SetFloat("Speed", direction.sqrMagnitude);
		animator.SetFloat("Gravity", player.gravity);
		animator.SetBool("Grounded", player.controller.isGrounded);
		animator.SetBool("Jump", player.jump);
		animator.SetBool("Attack", fire);
		animator.SetBool("Block", block);
		fire = false;
	}

	private void SendPositionToServer() {
		if(IsServer) {
			player.Position = transform.position;
			player.Direction = direction;
			return;
		}

		if(transform.position == lastPositionSent && direction == lastDirectionSent) {
			return;
		}

		using FastBufferWriter writer = new FastBufferWriter(24, Allocator.Temp);
		writer.WriteValueSafe(transform.position);
		writer.WriteValueSafe(direction);

		var receiver = NetworkManager.Singleton.ServerClientId;
		var delivery = NetworkDelivery.UnreliableSequenced;

		if(direction == Vector3.zero) {
			delivery = NetworkDelivery.Reliable;
		}

		NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("client position", receiver, writer, delivery);

		lastPositionSent = transform.position;
		lastDirectionSent = direction;
	}

	public void NewMessage(string message) {
		if(message == "/dc") {
			NetworkManager.Shutdown();
			return;
		}

		if(message.StartsWith("/maxfps ")) {
			var fps = int.Parse(message.Split(' ')[1]);
			Application.targetFrameRate = fps;
			return;
		}

		server.NewMessageServerRpc(message);
	}

#region Input
	public void Move(InputAction.CallbackContext context) {
		var value = context.ReadValue<Vector2>();
		inputDirection = new Vector3(value.x, 0, value.y);
	}

	public void Look(InputAction.CallbackContext context) {
		var value = context.ReadValue<Vector2>();

		if(context.control.device is Mouse) {
			camController.MouseLook(value);
		} else {
			camController.GamepadLook(value);
		}
	}

	public void Fire(InputAction.CallbackContext context) {
		fire = true;
		
		// Ray ray = cam.ScreenPointToRay(new Vector2(0.5f, 0.5f));
		// Physics.Raycast(ray)

		GameObject.Instantiate(spawnOnFire, transform.position + Vector3.up, model.rotation);
	}

	public void StartBlock(InputAction.CallbackContext context) {
		block = true;
	}

	public void StopBlock(InputAction.CallbackContext context) {
		block = false;
	}

	public void Jump(InputAction.CallbackContext context) {
		if(!player.Jump()) {
			return;
		}

		animator.SetBool("Jump", true);
		server.JumpServerRpc();
	}
#endregion

#region RPC
	[ClientRpc]
	public void NewMessageClientRpc(string message) {
		Game.Chat.Write("Map", $"{name}: {message}");
	}
#endregion
}