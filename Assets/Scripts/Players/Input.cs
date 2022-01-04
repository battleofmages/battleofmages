using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace BoM.Players {
	// Data
	public class InputData : NetworkBehaviour {
		[SerializeField] protected Player player;
		[SerializeField] protected OwnerMovement movement;
		[SerializeField] protected Jump jump;
		[SerializeField] protected Flight flight;
		[SerializeField] protected SkillSystem skillSystem;
		[SerializeField] protected Block block;
		[SerializeField] protected Cameras.Center camCenter;
	}

	// Logic
	public class Input : InputData {
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
			block.enabled = true;
		}

		public void StopBlock(InputAction.CallbackContext context) {
			block.enabled = false;
		}

		public void StartFlight(InputAction.CallbackContext context) {
			flight.Activate();
		}

		public void StopFlight(InputAction.CallbackContext context) {
			flight.Deactivate();
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
