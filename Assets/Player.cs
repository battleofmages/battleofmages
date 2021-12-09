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
		chat.Write("System", nick + " connected.");

		// Events
		if(IsOwner) {
			chat.NewMessage += NewMessageServerRpc;
			inputActions.FindAction("Move").performed += OnMove;
			inputActions.FindAction("Move").canceled += OnMove;
			inputActions.FindAction("Fire").performed += OnFire;
		}
	}

	public override void OnDestroy() {
		chat.Write("System", nick + " disconnected.");

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
		Debug.Log("OnMove: " + value.ToString());
	}

	public void OnFire(InputAction.CallbackContext context) {
		Debug.Log("OnFire");
	}

	[ServerRpc]
	public void NewMessageServerRpc(string message) {
		NewMessageClientRpc(message);
	}

	[ClientRpc]
	public void NewMessageClientRpc(string message) {
		chat.Write("Map", $"{nick}: {message}");
	}
}
