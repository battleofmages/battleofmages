using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Player : NetworkBehaviour {
	public TextMeshPro label;
	public InputActionAsset inputActions;
	public Camera cam;
	private ulong clientId;

	public override void OnNetworkSpawn() {
		// Set label text
		clientId = GetComponent<NetworkObject>().OwnerClientId;
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
		// Camera
		CameraManager.RemoveCamera(cam);

		// Events
		if(IsOwner) {
			Game.Stop();
			Game.Instance.player = null;
		}

		Game.Chat.Write("System", $"{name} disconnected.");
	}

	public void Move(InputAction.CallbackContext context) {
		var value = context.ReadValue<Vector2>();
		Debug.Log("Move: " + value.ToString());
	}

	public void Fire(InputAction.CallbackContext context) {
		Debug.Log("Fire");
	}

	[ServerRpc]
	public void NewMessageServerRpc(string message) {
		if(message == "/dc") {
			NetworkManager.DisconnectClient(clientId);
			return;
		}

		NewMessageClientRpc(message);
	}

	[ClientRpc]
	public void NewMessageClientRpc(string message) {
		Game.Chat.Write("Map", $"{name}: {message}");
	}
}
