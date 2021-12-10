using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Collections;

public class Client: NetworkBehaviour {
	public Player player;
	public Server server;
	public InputActionAsset inputActions;
	public Camera cam;
	private Vector3 direction;
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

	private void FixedUpdate() {
		player.Move(direction);

		if(transform.position != lastPositionSent || direction != lastDirectionSent) {
			SendPositionToServer();
			lastPositionSent = transform.position;
			lastDirectionSent = direction;
		}
	}

	private void SendPositionToServer() {
		if(IsServer) {
			player.Position = transform.position;
			player.Direction = direction;
			server.BroadcastPosition();
			return;
		}

		using FastBufferWriter writer = new FastBufferWriter(24, Allocator.Temp);
		writer.WriteValueSafe(transform.position);
		writer.WriteValueSafe(direction);
		var receiver = NetworkManager.Singleton.ServerClientId;
		var delivery = NetworkDelivery.UnreliableSequenced;

		if(direction == Vector3.zero) {
			delivery = NetworkDelivery.ReliableSequenced;
		}

		NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("client position", receiver, writer, delivery);
	}

	public void NewMessage(string message) {
		if(message == "/dc") {
			NetworkManager.Shutdown();
			return;
		}

		server.NewMessageServerRpc(message);
	}

#region Input
	public void Move(InputAction.CallbackContext context) {
		var value = context.ReadValue<Vector2>();
		// Debug.Log("Move " + value);
		direction = new Vector3(value.x, 0, value.y);
	}

	public void Fire(InputAction.CallbackContext context) {
		// Debug.Log("Fire");
	}
#endregion

#region RPC
	[ClientRpc]
	public void NewMessageClientRpc(string message) {
		Game.Chat.Write("Map", $"{name}: {message}");
	}
#endregion
}