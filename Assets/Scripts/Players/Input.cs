using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace BoM.Players {
	public class Input: NetworkBehaviour {
		public Player player;
		public Client client;
		public Server server;
		public Cursor cursor;
		public Cameras.Controller camController;

		public void Move(InputAction.CallbackContext context) {
			var value = context.ReadValue<Vector2>();
			client.inputDirection = new Vector3(value.x, 0, value.y);
		}

		public void Look(InputAction.CallbackContext context) {
			var value = context.ReadValue<Vector2>();

			if(context.control.device is Mouse) {
				camController.MouseLook(value);
			} else {
				camController.GamepadLook(value);
			}
		}

		public void Jump(InputAction.CallbackContext context) {
			if(!player.gravity.Jump()) {
				return;
			}

			server.JumpServerRpc();
		}

		public void StartBlock(InputAction.CallbackContext context) {
			player.animations.Animator.SetBool("Block", true);
		}

		public void StopBlock(InputAction.CallbackContext context) {
			player.animations.Animator.SetBool("Block", false);
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

		public async void UseSkill(byte slotIndex) {
			if(slotIndex >= player.currentElement.skills.Count) {
				return;
			}
			
			var skill = player.currentElement.skills[slotIndex];

			if(skill == null) {
				return;
			}

			server.UseSkillServerRpc(slotIndex, cursor.Position);
			player.animations.Animator.SetBool("Attack", true);
			await Task.Delay(300);
			player.UseSkill(skill, cursor.Position);
		}
	}
}