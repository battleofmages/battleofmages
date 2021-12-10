using TMPro;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IPlayer {
	public TextMeshPro label;
	public CharacterController controller;
	public float moveSpeed;
	public GameObject networkShadow;
	private Vector3 realPosition;
	public ulong ClientId { get; set; }
	public Vector3 Direction { get; set; }

	public string Name {
		get {
			return gameObject.name;
		}
		
		set {
			gameObject.name = value;
			label.text = value;
			networkShadow.gameObject.name = $"{value} - Shadow";
		}
	}

	public Vector3 Position {
		get {
			return realPosition;
		}

		set {
			realPosition = value;
			networkShadow.transform.position = value;
		}
	}

	public override void OnNetworkSpawn() {
		ClientId = GetComponent<NetworkObject>().OwnerClientId;

		if(IsOwner) {
			Name = "Player " + ClientId;
		} else {
			Name = "Proxy " + ClientId;
		}

		Position = transform.position;
		EnableNetworkComponents();
		Register();
		networkShadow.transform.SetParent(null, true);
	}

	private void OnDisable() {
		Unregister();
		Destroy(networkShadow);
	}

	private void EnableNetworkComponents() {
		if(IsClient && IsServer && IsOwner) {
			GetComponent<Client>().enabled = true;
			// GetComponent<Server>().enabled = true;
			return;
		}

		if(IsServer) {
			GetComponent<Server>().enabled = true;
			return;
		}

		if(IsClient && IsOwner) {
			GetComponent<Client>().enabled = true;
			return;
		}

		if(IsClient && !IsOwner) {
			GetComponent<Proxy>().enabled = true;
			return;
		}
	}

	private void Register() {
		PlayerManager.Add(this);
		Game.Chat.Write("System", $"{Name} connected.");
	}

	private void Unregister() {
		PlayerManager.Remove(this);
		Game.Chat.Write("System", $"{Name} disconnected.");
	}

	public void Move(Vector3 direction) {
		if(direction.sqrMagnitude > 1f) {
			direction.Normalize();
		}

		direction *= moveSpeed;
		direction += Physics.gravity;

		controller.Move(direction * Time.fixedDeltaTime);
	}
}
