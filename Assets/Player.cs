using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Collections;
using System.Collections.Generic;

public partial class Player : NetworkBehaviour {
	private static List<Player> players = new List<Player>();
	private static Dictionary<ulong, Player> clientIdToPlayer = new Dictionary<ulong, Player>();
	
	public TextMeshPro label;
	public InputActionAsset inputActions;
	public Camera cam;
	public CharacterController controller;
	public float moveSpeed;
	private ulong clientId;
	private Vector3 moveVector;
	private CustomMessagingManager messagingManager;

	public override void OnNetworkSpawn() {
		clientId = GetComponent<NetworkObject>().OwnerClientId;
		messagingManager = NetworkManager.Singleton.CustomMessagingManager;

		// Set label text
		if(IsOwner) {
			name = "Player " + clientId;
		} else {
			name = "Proxy " + clientId;
		}
		
		label.text = name;

		// Add to global player list
		players.Add(this);
		clientIdToPlayer.Add(clientId, this);
		
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
		// Remove from the global player list
		players.Remove(this);
		clientIdToPlayer.Remove(clientId);

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
			if(IsServer) {
				using FastBufferWriter writer = new FastBufferWriter(20, Allocator.Temp);
				writer.WriteValueSafe(clientId);
				writer.WriteValueSafe(transform.position);

				messagingManager.SendNamedMessageToAll("position confirmed", writer, NetworkDelivery.UnreliableSequenced);
			} else {
				using FastBufferWriter writer = new FastBufferWriter(12, Allocator.Temp);
				writer.WriteValueSafe(transform.position);

				var receiver = NetworkManager.Singleton.ServerClientId;
				messagingManager.SendNamedMessage("position request", receiver, writer, NetworkDelivery.UnreliableSequenced);
			}
		}
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
		return clientIdToPlayer[clientId];
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
