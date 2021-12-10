using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour {
	private static Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();
	
	public TextMeshPro label;
	public InputActionAsset inputActions;
	public Camera cam;
	public CharacterController controller;
	public float moveSpeed;
	private ulong clientId;
	private Vector3 moveVector;

	public override void OnNetworkSpawn() {
		clientId = GetComponent<NetworkObject>().OwnerClientId;
		players.Add(clientId, this);

		// Set label text
		name = "Player " + clientId;
		label.text = name;
		
		// Camera
		CameraManager.AddCamera(cam);

		// Events
		if(IsOwner) {
			Game.Instance.player = this;
			Game.Start();
		}

		// Chat
		Game.Chat.Write("System", $"{name} connected.");
	}

	public void OnDisable() {
		players.Remove(clientId);

		// Camera
		CameraManager.RemoveCamera(cam);

		// Events
		if(IsOwner) {
			Game.Stop();
			Game.Instance.player = null;
		}

		Game.Chat.Write("System", $"{name} disconnected.");
	}

	private void FixedUpdate() {
		if(moveVector == Vector3.zero) {
			return;
		}

		controller.Move(moveVector * moveSpeed);

		if(IsOwner) {
			using FastBufferWriter writer = new FastBufferWriter(12, Allocator.Temp);
			writer.WriteValueSafe(transform.position);
			var receiver = NetworkManager.Singleton.ServerClientId;
			Debug.Log($"sending {writer.Length} bytes to {receiver}");
			NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("pos", receiver, writer, NetworkDelivery.UnreliableSequenced);
		}
	}

	public void Move(InputAction.CallbackContext context) {
		var value = context.ReadValue<Vector2>();
		moveVector = new Vector3(value.x, 0, value.y);
	}

	public void Fire(InputAction.CallbackContext context) {
		Debug.Log("Fire");
	}

	public void OnPositionReceived(Vector3 newPosition) {
		Debug.Log($"received new position {newPosition}");
		transform.position = newPosition;
	}

	public void NewMessage(string message) {
		if(message == "/dc") {
			NetworkManager.Shutdown();
			return;
		}
		
		if(message == "/net") {
			Game.Chat.Write("System", $"Server ID: {NetworkManager.Singleton.ServerClientId}");
			Game.Chat.Write("System", $"{NetworkManager.Singleton.ConnectedClientsList.Count} connected clients");

			foreach(var client in NetworkManager.Singleton.ConnectedClientsList) {
				Game.Chat.Write("System", $"ID {client.ClientId}: {client.PlayerObject.name}");
			}

			return;
		}

		NewMessageServerRpc(message);
	}

	public static Player ByClientId(ulong clientId) {
		return players[clientId];
	}

	[ServerRpc]
	public void NewMessageServerRpc(string message) {
		NewMessageClientRpc(message);
	}

	[ClientRpc]
	public void NewMessageClientRpc(string message) {
		Game.Chat.Write("Map", $"{name}: {message}");
	}
}
