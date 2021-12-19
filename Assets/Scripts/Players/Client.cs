using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Collections;

namespace BoM.Players {
	public class Client: NetworkBehaviour {
		public Player player;
		public Server server;
		public Animations animations;
		public Cursor cursor;
		public InputActionAsset inputActions;
		
		public Cameras.Controller camController;
		public Transform model;
		public float rotationSpeed;
		private Vector3 inputDirection;
		private Vector3 direction;
		private Vector2 look;
		private Vector3 lastPositionSent;
		private Vector3 lastDirectionSent;

		private void Update() {
			UpdateRotation();
		}

		private void FixedUpdate() {
			UpdatePosition();
			SendPositionToServer();
		}

		private void UpdatePosition() {
			direction = player.cam.transform.TransformDirection(inputDirection);
			direction.y = 0f;
			direction.Normalize();

			player.Move(direction);
		}

		private void UpdateRotation() {
			if(direction == Vector3.zero) {
				return;
			}

			model.rotation = Quaternion.Slerp(
				model.rotation,
				Quaternion.LookRotation(direction),
				Time.deltaTime * rotationSpeed
			);
		}

		private void SendPositionToServer() {
			if(IsServer) {
				player.RemotePosition = transform.position;
				player.RemoteDirection = direction;
				return;
			}

			if(transform.position == lastPositionSent && direction == lastDirectionSent) {
				return;
			}

			using FastBufferWriter writer = new FastBufferWriter(24, Allocator.Temp);
			writer.WriteValueSafe(transform.position);
			writer.WriteValueSafe(direction);

			var receiver = NetworkManager.Singleton.ServerClientId;
			var delivery = NetworkDelivery.UnreliableSequenced;

			if(direction == Vector3.zero) {
				delivery = NetworkDelivery.Reliable;
			}

			NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("client position", receiver, writer, delivery);

			lastPositionSent = transform.position;
			lastDirectionSent = direction;
		}

		public void SendChatMessage(string message) {
			if(message == "/dc") {
				NetworkManager.Shutdown();
				return;
			}

			if(message.StartsWith("/maxfps ")) {
				var fps = int.Parse(message.Split(' ')[1]);
				Application.targetFrameRate = fps;
				return;
			}

			server.SendChatMessageServerRpc(message);
		}

	#region Input
		public void Move(InputAction.CallbackContext context) {
			var value = context.ReadValue<Vector2>();
			inputDirection = new Vector3(value.x, 0, value.y);
		}

		public void Look(InputAction.CallbackContext context) {
			var value = context.ReadValue<Vector2>();

			if(context.control.device is Mouse) {
				camController.MouseLook(value);
			} else {
				camController.GamepadLook(value);
			}
		}

		public void Skill1(InputAction.CallbackContext context) {
			UseSkill(0);
		}

		public void Skill2(InputAction.CallbackContext context) {
			UseSkill(1);
		}

		public void Skill3(InputAction.CallbackContext context) {
			UseSkill(2);
		}

		public void Skill4(InputAction.CallbackContext context) {
			UseSkill(3);
		}

		public void Skill5(InputAction.CallbackContext context) {
			UseSkill(4);
		}

		public async void UseSkill(int slotIndex) {
			animations.Animator.SetBool("Attack", true);
			await Task.Delay(300);
			player.UseSkill(player.currentElement.skills[slotIndex], cursor.Position);
		}

		public void StartBlock(InputAction.CallbackContext context) {
			animations.Animator.SetBool("Block", true);
		}

		public void StopBlock(InputAction.CallbackContext context) {
			animations.Animator.SetBool("Block", false);
		}

		public void Jump(InputAction.CallbackContext context) {
			if(!player.gravity.Jump()) {
				return;
			}

			server.JumpServerRpc();
		}
	#endregion
	}
}