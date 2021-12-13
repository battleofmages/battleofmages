using TMPro;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IPlayer {
	public TextMeshPro label;
	public CharacterController controller;
	public float moveSpeed;
	public float jumpHeight;
	public Transform model;
	public float modelYOffset;
	public GameObject networkShadow;
	public ulong ClientId { get; set; }
	public Vector3 Direction { get; set; }
	public float gravityWhenGrounded;
	public float allowJumpOverMaxGroundDistanceTime;
	public float allowJumpWhenNotGroundedTime;
	public float maxDistanceToGroundOnJump;

	[System.NonSerialized]
	public float notGroundedTime;

	[System.NonSerialized]
	public bool jump;

	[System.NonSerialized]
	public float gravity;

	private Vector3 realPosition;
	private float originalStepOffset;

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
		originalStepOffset = controller.stepOffset;
		model.localPosition = new Vector3(0f, -controller.skinWidth + modelYOffset, 0f);
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

		UpdateGravity();
		direction.y = gravity;

		controller.Move(direction * Time.deltaTime);
	}

	private void UpdateGravity() {
		var isGrounded = controller.isGrounded;
		notGroundedTime += Time.deltaTime;

		if(isGrounded && gravity < 0f) {
			notGroundedTime = 0f;
			gravity = gravityWhenGrounded;
			controller.stepOffset = originalStepOffset;
		} else {
			controller.stepOffset = 0f;
		}
		
		gravity += Physics.gravity.y * Time.deltaTime;

		if(jump && canJump) {
			gravity = Mathf.Sqrt(jumpHeight * 2 * -Physics.gravity.y);
			jump = false;
		}
	}

	public bool canJump {
		get {
			if(notGroundedTime < allowJumpOverMaxGroundDistanceTime) {
				return true;
			}

			if(notGroundedTime < allowJumpWhenNotGroundedTime && Physics.Raycast(transform.position, Vector3.down, maxDistanceToGroundOnJump)) {
				return true;
			}

			return false;
		}
	}

	public bool Jump() {
		if(!canJump) {
			return false;
		}

		jump = true;
		return true;
	}
}
