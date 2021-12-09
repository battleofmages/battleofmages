using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Player : NetworkBehaviour {
	public TextMeshPro label;
	public InputActionAsset inputActions;
	private string nick;
	private ulong clientId;
	private Chat chat;

	public override void OnNetworkSpawn() {
		// Set label text
		clientId = GetComponent<NetworkObject>().OwnerClientId;
		nick = "Player " + clientId;
		label.text = nick;

		// Chat
		chat = GameObject.Find("Chat").GetComponent<Chat>();
		chat.Write(nick + " connected.");

		// Events
		if(IsOwner) {
			chat.NewMessage += NewMessageServerRpc;
			inputActions.FindAction("Move").performed += OnMove;
			inputActions.FindAction("Move").canceled += OnMove;
			inputActions.FindAction("Fire").performed += OnFire;
		}
	}

	public override void OnDestroy() {
		chat.Write(nick + " disconnected.");

		// Events
		if(IsOwner) {
			chat.NewMessage -= NewMessageServerRpc;
			inputActions.FindAction("Move").performed -= OnMove;
			inputActions.FindAction("Move").canceled -= OnMove;
			inputActions.FindAction("Fire").performed -= OnFire;
		}
	}

	public void OnMove(InputAction.CallbackContext context) {
		var value = context.ReadValue<Vector2>();
		chat.Write("OnMove: " + value.ToString());
	}

	public void OnFire(InputAction.CallbackContext context) {
		chat.Write("OnFire");
	}

	[ServerRpc]
	public void NewMessageServerRpc(string message) {
		NewMessageClientRpc(message);
	}

	[ClientRpc]
	public void NewMessageClientRpc(string message) {
		chat.Write(nick + ": " + message);
	}
}
