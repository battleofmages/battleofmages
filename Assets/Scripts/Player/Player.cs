using TMPro;
using UnityEngine;
using Unity.Netcode;

public class Player : Entity, IPlayer {
	public TextMeshPro label;
	public CharacterController controller;
	public Gravity gravity;
	
	public float moveSpeed;
	public Transform model;
	public float modelYOffset;
	public GameObject networkShadow;
	public ulong ClientId { get; set; }
	public Vector3 RemoteDirection { get; set; }

	private Vector3 realPosition;

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

	public Vector3 RemotePosition {
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

		RemotePosition = transform.position;
		model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);
		EnableNetworkComponents();
		Register();
		var playerRoot = GameObject.Find("Players");
		transform.SetParent(playerRoot.transform);
		networkShadow.transform.SetParent(playerRoot.transform, true);
	}

	private void OnDisable() {
		Unregister();
		Destroy(networkShadow);
	}

	private void EnableNetworkComponents() {
		if(IsClient && IsServer && IsOwner) {
			GetComponent<Client>().enabled = true;
			GetComponent<Server>().enabled = true;
			return;
		}

		if(IsServer) {
			GetComponent<Server>().enabled = true;
			GetComponent<Proxy>().enabled = true;
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
		direction.y = GetComponent<Gravity>().Speed;

		controller.Move(direction * Time.deltaTime);
	}
}
