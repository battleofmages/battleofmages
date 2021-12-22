using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace BoM.Players {
	public class Input: NetworkBehaviour {
		public Player player;
		public OwnerMovement movement;
		public Jump jump;
		public SkillSystem skillSystem;
		public Cursor cursor;
		public Cameras.Controller camController;

		public void Move(InputAction.CallbackContext context) {
			var value = context.ReadValue<Vector2>();
			movement.inputDirection = new Vector3(value.x, 0, value.y);
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

			jump.JumpServerRpc();
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
			if(slotIndex >= skillSystem.currentElement.skills.Count) {
				return;
			}
			
			var skill = skillSystem.currentElement.skills[slotIndex];

			if(skill == null) {
				return;
			}

			skillSystem.UseSkillServerRpc(slotIndex, cursor.Position);
			player.animations.Animator.SetBool("Attack", true);
			await Task.Delay(300);
			skillSystem.UseSkill(skill, cursor.Position);
		}
	}
}