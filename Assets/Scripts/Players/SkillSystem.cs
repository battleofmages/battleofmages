using BoM.Skills;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace BoM.Players {
	public class SkillSystem : NetworkBehaviour {
		public Player player;
		public Skeleton skeleton;
		public Animations animations;
		public List<Element> elements { get; set; }
		private int currentElementIndex;

		public Element currentElement {
			get {
				return elements[currentElementIndex];
			}
		}

		public void UseSkill(Skill skill, Vector3 cursor) {
			Vector3 skillPosition = Vector3.zero;
			Quaternion skillRotation = Quaternion.identity;

			switch(skill.position) {
				case PositionType.Cursor:
					skillPosition = cursor;
					break;

				case PositionType.Hands:
					skillPosition = skeleton.handsCenter;
					break;
			}

			switch(skill.rotation) {
				case RotationType.ToCursor:
					var towardsCursor = cursor - skillPosition;

					if(towardsCursor != Vector3.zero) {
						skillRotation = Quaternion.LookRotation(towardsCursor);
					}

					break;
			}

			var instance = skill.Instantiate();
			instance.transform.position = skillPosition;
			instance.transform.rotation = skillRotation;
			instance.pool = skill.pool;
			instance.Init();
		}

		[ClientRpc]
		public async void UseSkillClientRpc(byte index, Vector3 cursorPosition) {
			if(IsOwner || IsServer) {
				return;
			}

			animations.Animator.SetBool("Attack", true);
			await Task.Delay(300);
			UseSkill(currentElement.skills[index], cursorPosition);
		}

		[ServerRpc]
		public async void UseSkillServerRpc(byte index, Vector3 cursorPosition) {
			if(IsHost && IsOwner) {
				UseSkillClientRpc(index, cursorPosition);
				return;
			}
			
			animations.Animator.SetBool("Attack", true);
			await Task.Delay(300);
			UseSkill(currentElement.skills[index], cursorPosition);
			UseSkillClientRpc(index, cursorPosition);
		}
	}
}