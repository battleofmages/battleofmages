using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace BoM.Players {
	public class Input : NetworkBehaviour {
		public Player player;
		public OwnerMovement movement;
		public Jump jump;
		public SkillSystem skillSystem;
		public Animations animations;
		public Cameras.Center camCenter;

		public void Move(InputAction.CallbackContext context) {
			var value = context.ReadValue<Vector2>();
			movement.inputDirection = new Vector3(value.x, 0, value.y);
		}

		public void Look(InputAction.CallbackContext context) {
			var value = context.ReadValue<Vector2>();

			if(context.control.device is Mouse) {
				camCenter.MouseLook(value);
			} else {
				camCenter.GamepadLook(value);
			}
		}

		public void Jump(InputAction.CallbackContext context) {
			if(!jump.TryJump()) {
				return;
			}

			jump.JumpServerRpc();
		}

		public void StartBlock(InputAction.CallbackContext context) {
			animations.Animator.SetBool("Block", true);
		}

		public void StopBlock(InputAction.CallbackContext context) {
			animations.Animator.SetBool("Block", false);
		}

		public void Skill1(InputAction.CallbackContext context) {
			skillSystem.CastSkillAtIndex(0);
		}

		public void Skill2(InputAction.CallbackContext context) {
			skillSystem.CastSkillAtIndex(1);
		}

		public void Skill3(InputAction.CallbackContext context) {
			skillSystem.CastSkillAtIndex(2);
		}

		public void Skill4(InputAction.CallbackContext context) {
			skillSystem.CastSkillAtIndex(3);
		}

		public void Skill5(InputAction.CallbackContext context) {
			skillSystem.CastSkillAtIndex(4);
		}
	}
}
