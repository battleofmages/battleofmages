using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Collections;

public class Client: NetworkBehaviour {
	public Player player;
	public Server server;
	public InputActionAsset inputActions;
	public Camera cam;
	private Vector3 moveVector;
	private Vector3 lastPositionSent;

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
		player.Move(moveVector);

		if(transform.position != lastPositionSent) {
			SendPositionToServer();
			lastPositionSent = transform.position;
		}
	}

	private void SendPositionToServer() {
		if(IsServer) {
			player.Position = transform.position;
			server.BroadcastPosition();
			return;
		}

		using FastBufferWriter writer = new FastBufferWriter(12, Allocator.Temp);
		writer.WriteValueSafe(transform.position);
		var receiver = NetworkManager.Singleton.ServerClientId;
		NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("client position", receiver, writer, NetworkDelivery.UnreliableSequenced);
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
		moveVector = new Vector3(value.x, 0, value.y);
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